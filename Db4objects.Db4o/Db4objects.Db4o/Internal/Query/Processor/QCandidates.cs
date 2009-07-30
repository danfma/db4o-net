/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Internal.Fieldindex;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <summary>
	/// Holds the tree of
	/// <see cref="Db4objects.Db4o.Internal.Query.Processor.QCandidate">Db4objects.Db4o.Internal.Query.Processor.QCandidate
	/// 	</see>
	/// objects and the list of
	/// <see cref="Db4objects.Db4o.Internal.Query.Processor.QCon">Db4objects.Db4o.Internal.Query.Processor.QCon
	/// 	</see>
	/// during query evaluation.
	/// The query work (adding and removing nodes) happens here.
	/// Candidates during query evaluation.
	/// <see cref="Db4objects.Db4o.Internal.Query.Processor.QCandidate">Db4objects.Db4o.Internal.Query.Processor.QCandidate
	/// 	</see>
	/// objects are stored in i_root
	/// </summary>
	/// <exclude></exclude>
	public sealed class QCandidates : IVisitor4
	{
		public readonly LocalTransaction i_trans;

		public Tree i_root;

		private List4 _constraints;

		internal ClassMetadata i_classMetadata;

		private QField _field;

		internal QCon i_currentConstraint;

		internal Tree i_ordered;

		private int _majorOrderingID;

		private IDGenerator _idGenerator;

		private bool _loadedFromClassIndex;

		internal QCandidates(LocalTransaction a_trans, ClassMetadata a_classMetadata, QField
			 a_field)
		{
			// Transaction necessary as reference to stream
			// root of the QCandidate tree
			// collection of all constraints
			// possible class information
			// possible field information
			// current executing constraint, only set where needed
			// QOrder tree
			// 
			i_trans = a_trans;
			i_classMetadata = a_classMetadata;
			_field = a_field;
			if (a_field == null || a_field._fieldMetadata == null || !(a_field._fieldMetadata
				.GetHandler() is StandardReferenceTypeHandler))
			{
				return;
			}
			ClassMetadata yc = ((StandardReferenceTypeHandler)a_field._fieldMetadata.GetHandler
				()).ClassMetadata();
			if (i_classMetadata == null)
			{
				i_classMetadata = yc;
			}
			else
			{
				yc = i_classMetadata.GetHigherOrCommonHierarchy(yc);
				if (yc != null)
				{
					i_classMetadata = yc;
				}
			}
		}

		public QCandidate Add(QCandidate candidate)
		{
			i_root = Tree.Add(i_root, candidate);
			if (candidate._size == 0)
			{
				// This means that the candidate was already present
				// and QCandidate does not allow duplicates.
				// In this case QCandidate#isDuplicateOf will have
				// placed the existing QCandidate in the i_root
				// variable of the new candidate. We return it here: 
				return candidate.GetRoot();
			}
			return candidate;
		}

		internal void AddConstraint(QCon a_constraint)
		{
			_constraints = new List4(_constraints, a_constraint);
		}

		internal void AddOrder(QOrder a_order)
		{
			i_ordered = Tree.Add(i_ordered, a_order);
		}

		internal void ApplyOrdering(Tree orderedCandidates, int orderingID)
		{
			if (orderedCandidates == null || i_root == null)
			{
				return;
			}
			int absoluteOrderingID = Math.Abs(orderingID);
			bool major = TreatOrderingIDAsMajor(absoluteOrderingID);
			if (major && !IsUnordered())
			{
				SwapMajorOrderToMinor();
			}
			HintNewOrder(orderedCandidates, major);
			i_root = RecreateTreeFromCandidates();
			if (major)
			{
				_majorOrderingID = absoluteOrderingID;
			}
		}

		public QCandidate ReadSubCandidate(QueryingReadContext context, ITypeHandler4 handler
			)
		{
			ObjectID objectID = ObjectID.NotPossible;
			try
			{
				int offset = context.Offset();
				if (handler is IReadsObjectIds)
				{
					objectID = ((IReadsObjectIds)handler).ReadObjectID(context);
				}
				if (objectID.IsValid())
				{
					return new QCandidate(this, null, objectID._id);
				}
				if (objectID == ObjectID.NotPossible)
				{
					context.Seek(offset);
					object obj = context.Read(handler);
					if (obj != null)
					{
						return new QCandidate(this, obj, context.Container().GetID(context.Transaction(), 
							obj));
					}
				}
			}
			catch (Exception)
			{
			}
			// FIXME: Catchall
			return null;
		}

		private Tree RecreateTreeFromCandidates()
		{
			Collection4 col = CollectCandidates();
			Tree newTree = null;
			IEnumerator i = col.GetEnumerator();
			while (i.MoveNext())
			{
				QCandidate candidate = (QCandidate)i.Current;
				candidate._preceding = null;
				candidate._subsequent = null;
				candidate._size = 1;
				newTree = Tree.Add(newTree, candidate);
			}
			return newTree;
		}

		private Collection4 CollectCandidates()
		{
			Collection4 col = new Collection4();
			i_root.Traverse(new _IVisitor4_171(col));
			return col;
		}

		private sealed class _IVisitor4_171 : IVisitor4
		{
			public _IVisitor4_171(Collection4 col)
			{
				this.col = col;
			}

			public void Visit(object a_object)
			{
				QCandidate candidate = (QCandidate)a_object;
				col.Add(candidate);
			}

			private readonly Collection4 col;
		}

		private void HintNewOrder(Tree orderedCandidates, bool major)
		{
			int[] currentOrder = new int[] { 0 };
			QOrder[] lastOrder = new QOrder[] { null };
			orderedCandidates.Traverse(new _IVisitor4_184(lastOrder, currentOrder, major));
		}

		private sealed class _IVisitor4_184 : IVisitor4
		{
			public _IVisitor4_184(QOrder[] lastOrder, int[] currentOrder, bool major)
			{
				this.lastOrder = lastOrder;
				this.currentOrder = currentOrder;
				this.major = major;
			}

			public void Visit(object a_object)
			{
				QOrder qo = (QOrder)a_object;
				if (!qo.IsEqual(lastOrder[0]))
				{
					currentOrder[0]++;
				}
				QCandidate candidate = qo._candidate.GetRoot();
				candidate.HintOrder(currentOrder[0], major);
				lastOrder[0] = qo;
			}

			private readonly QOrder[] lastOrder;

			private readonly int[] currentOrder;

			private readonly bool major;
		}

		private void SwapMajorOrderToMinor()
		{
			i_root.Traverse(new _IVisitor4_198());
		}

		private sealed class _IVisitor4_198 : IVisitor4
		{
			public _IVisitor4_198()
			{
			}

			public void Visit(object obj)
			{
				QCandidate candidate = (QCandidate)obj;
				Order order = (Order)candidate._order;
				order.SwapMajorToMinor();
			}
		}

		private bool TreatOrderingIDAsMajor(int absoluteOrderingID)
		{
			return (IsUnordered()) || (IsMoreRelevantOrderingID(absoluteOrderingID));
		}

		private bool IsUnordered()
		{
			return _majorOrderingID == 0;
		}

		private bool IsMoreRelevantOrderingID(int absoluteOrderingID)
		{
			return absoluteOrderingID < _majorOrderingID;
		}

		internal void Collect(Db4objects.Db4o.Internal.Query.Processor.QCandidates a_candidates
			)
		{
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				QCon qCon = (QCon)i.Current;
				SetCurrentConstraint(qCon);
				qCon.Collect(a_candidates);
			}
			SetCurrentConstraint(null);
		}

		internal void Execute()
		{
			if (DTrace.enabled)
			{
				DTrace.QueryProcess.Log();
			}
			FieldIndexProcessorResult result = ProcessFieldIndexes();
			if (result.FoundIndex())
			{
				i_root = result.ToQCandidate(this);
			}
			else
			{
				LoadFromClassIndex();
			}
			Evaluate();
		}

		public IEnumerator ExecuteSnapshot(Collection4 executionPath)
		{
			IIntIterator4 indexIterator = new IntIterator4Adaptor(IterateIndex(ProcessFieldIndexes
				()));
			Tree idRoot = TreeInt.AddAll(null, indexIterator);
			IEnumerator snapshotIterator = new TreeKeyIterator(idRoot);
			IEnumerator singleObjectQueryIterator = SingleObjectSodaProcessor(snapshotIterator
				);
			return MapIdsToExecutionPath(singleObjectQueryIterator, executionPath);
		}

		private IEnumerator SingleObjectSodaProcessor(IEnumerator indexIterator)
		{
			return Iterators.Map(indexIterator, new _IFunction4_251(this));
		}

		private sealed class _IFunction4_251 : IFunction4
		{
			public _IFunction4_251(QCandidates _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object current)
			{
				int id = ((int)current);
				QCandidate candidate = new QCandidate(this._enclosing, null, id);
				this._enclosing.i_root = candidate;
				this._enclosing.Evaluate();
				if (!candidate.Include())
				{
					return Iterators.Skip;
				}
				return current;
			}

			private readonly QCandidates _enclosing;
		}

		public IEnumerator ExecuteLazy(Collection4 executionPath)
		{
			IEnumerator indexIterator = IterateIndex(ProcessFieldIndexes());
			IEnumerator singleObjectQueryIterator = SingleObjectSodaProcessor(indexIterator);
			return MapIdsToExecutionPath(singleObjectQueryIterator, executionPath);
		}

		private IEnumerator IterateIndex(FieldIndexProcessorResult result)
		{
			if (result.NoMatch())
			{
				return Iterators.EmptyIterator;
			}
			if (result.FoundIndex())
			{
				return result.IterateIDs();
			}
			if (i_classMetadata.IsPrimitive())
			{
				return Iterators.EmptyIterator;
			}
			return BTreeClassIndexStrategy.Iterate(i_classMetadata, i_trans);
		}

		private IEnumerator MapIdsToExecutionPath(IEnumerator singleObjectQueryIterator, 
			Collection4 executionPath)
		{
			if (executionPath == null)
			{
				return singleObjectQueryIterator;
			}
			IEnumerator res = singleObjectQueryIterator;
			IEnumerator executionPathIterator = executionPath.GetEnumerator();
			while (executionPathIterator.MoveNext())
			{
				string fieldName = (string)executionPathIterator.Current;
				res = Iterators.Concat(Iterators.Map(res, new _IFunction4_297(this, fieldName)));
			}
			return res;
		}

		private sealed class _IFunction4_297 : IFunction4
		{
			public _IFunction4_297(QCandidates _enclosing, string fieldName)
			{
				this._enclosing = _enclosing;
				this.fieldName = fieldName;
			}

			public object Apply(object current)
			{
				int id = ((int)current);
				CollectIdContext context = CollectIdContext.ForID(this._enclosing.i_trans, id);
				if (context == null)
				{
					return Iterators.Skip;
				}
				context.ClassMetadata().CollectIDs(context, fieldName);
				return new TreeKeyIterator(context.Ids());
			}

			private readonly QCandidates _enclosing;

			private readonly string fieldName;
		}

		public ObjectContainerBase Stream()
		{
			return i_trans.Container();
		}

		public int ClassIndexEntryCount()
		{
			return i_classMetadata.IndexEntryCount(i_trans);
		}

		private FieldIndexProcessorResult ProcessFieldIndexes()
		{
			if (_constraints == null)
			{
				return FieldIndexProcessorResult.NoIndexFound;
			}
			return new FieldIndexProcessor(this).Run();
		}

		internal void Evaluate()
		{
			if (_constraints == null)
			{
				return;
			}
			ForEachConstraint(new _IProcedure4_335(this));
			ForEachConstraint(new _IProcedure4_343());
			ForEachConstraint(new _IProcedure4_349());
			ForEachConstraint(new _IProcedure4_355());
			ForEachConstraint(new _IProcedure4_361());
			ForEachConstraint(new _IProcedure4_367());
		}

		private sealed class _IProcedure4_335 : IProcedure4
		{
			public _IProcedure4_335(QCandidates _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Apply(object arg)
			{
				QCon qCon = (QCon)arg;
				qCon.SetCandidates(this._enclosing);
				qCon.EvaluateSelf();
			}

			private readonly QCandidates _enclosing;
		}

		private sealed class _IProcedure4_343 : IProcedure4
		{
			public _IProcedure4_343()
			{
			}

			public void Apply(object arg)
			{
				((QCon)arg).EvaluateSimpleChildren();
			}
		}

		private sealed class _IProcedure4_349 : IProcedure4
		{
			public _IProcedure4_349()
			{
			}

			public void Apply(object arg)
			{
				((QCon)arg).EvaluateEvaluations();
			}
		}

		private sealed class _IProcedure4_355 : IProcedure4
		{
			public _IProcedure4_355()
			{
			}

			public void Apply(object arg)
			{
				((QCon)arg).EvaluateCreateChildrenCandidates();
			}
		}

		private sealed class _IProcedure4_361 : IProcedure4
		{
			public _IProcedure4_361()
			{
			}

			public void Apply(object arg)
			{
				((QCon)arg).EvaluateCollectChildren();
			}
		}

		private sealed class _IProcedure4_367 : IProcedure4
		{
			public _IProcedure4_367()
			{
			}

			public void Apply(object arg)
			{
				((QCon)arg).EvaluateChildren();
			}
		}

		private void ForEachConstraint(IProcedure4 proc)
		{
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				QCon constraint = (QCon)i.Current;
				if (!constraint.ProcessedByIndex())
				{
					proc.Apply(constraint);
				}
			}
		}

		internal bool IsEmpty()
		{
			bool[] ret = new bool[] { true };
			Traverse(new _IVisitor4_387(ret));
			return ret[0];
		}

		private sealed class _IVisitor4_387 : IVisitor4
		{
			public _IVisitor4_387(bool[] ret)
			{
				this.ret = ret;
			}

			public void Visit(object obj)
			{
				if (((QCandidate)obj)._include)
				{
					ret[0] = false;
				}
			}

			private readonly bool[] ret;
		}

		internal bool Filter(IVisitor4 a_host)
		{
			if (i_root != null)
			{
				i_root.Traverse(a_host);
				i_root = i_root.Filter(new _IPredicate4_400());
			}
			return i_root != null;
		}

		private sealed class _IPredicate4_400 : IPredicate4
		{
			public _IPredicate4_400()
			{
			}

			public bool Match(object a_candidate)
			{
				return ((QCandidate)a_candidate)._include;
			}
		}

		internal int GenerateCandidateId()
		{
			if (_idGenerator == null)
			{
				_idGenerator = new IDGenerator();
			}
			return -_idGenerator.Next();
		}

		public IEnumerator IterateConstraints()
		{
			if (_constraints == null)
			{
				return Iterators.EmptyIterator;
			}
			return new Iterator4Impl(_constraints);
		}

		internal sealed class TreeIntBuilder
		{
			public TreeInt tree;

			public void Add(TreeInt node)
			{
				tree = (TreeInt)Tree.Add(tree, node);
			}
		}

		internal void LoadFromClassIndex()
		{
			if (!IsEmpty())
			{
				return;
			}
			QCandidates.TreeIntBuilder result = new QCandidates.TreeIntBuilder();
			IClassIndexStrategy index = i_classMetadata.Index();
			index.TraverseAll(i_trans, new _IVisitor4_438(this, result));
			i_root = result.tree;
			DiagnosticProcessor dp = i_trans.Container()._handlers._diagnosticProcessor;
			if (dp.Enabled() && !IsClassOnlyQuery())
			{
				dp.LoadedFromClassIndex(i_classMetadata);
			}
			_loadedFromClassIndex = true;
		}

		private sealed class _IVisitor4_438 : IVisitor4
		{
			public _IVisitor4_438(QCandidates _enclosing, QCandidates.TreeIntBuilder result)
			{
				this._enclosing = _enclosing;
				this.result = result;
			}

			public void Visit(object obj)
			{
				result.Add(new QCandidate(this._enclosing, null, ((int)obj)));
			}

			private readonly QCandidates _enclosing;

			private readonly QCandidates.TreeIntBuilder result;
		}

		internal void SetCurrentConstraint(QCon a_constraint)
		{
			i_currentConstraint = a_constraint;
		}

		internal void Traverse(IVisitor4 a_visitor)
		{
			if (i_root != null)
			{
				i_root.Traverse(a_visitor);
			}
		}

		// FIXME: This method should go completely.
		//        We changed the code to create the QCandidates graph in two steps:
		//        (1) call fitsIntoExistingConstraintHierarchy to determine whether
		//            or not we need more QCandidates objects
		//        (2) add all constraints
		//        This method tries to do both in one, which results in missing
		//        constraints. Not all are added to all QCandiates.
		//        Right methodology is in 
		//        QQueryBase#createCandidateCollection
		//        and
		//        QQueryBase#createQCandidatesList
		internal bool TryAddConstraint(QCon a_constraint)
		{
			if (_field != null)
			{
				QField qf = a_constraint.GetField();
				if (qf != null)
				{
					if (_field.Name() != null && !_field.Name().Equals(qf.Name()))
					{
						return false;
					}
				}
			}
			if (i_classMetadata == null || a_constraint.IsNullConstraint())
			{
				AddConstraint(a_constraint);
				return true;
			}
			ClassMetadata yc = a_constraint.GetYapClass();
			if (yc != null)
			{
				yc = i_classMetadata.GetHigherOrCommonHierarchy(yc);
				if (yc != null)
				{
					i_classMetadata = yc;
					AddConstraint(a_constraint);
					return true;
				}
			}
			AddConstraint(a_constraint);
			return false;
		}

		public void Visit(object a_tree)
		{
			QCandidate parent = (QCandidate)a_tree;
			if (parent.CreateChild(this))
			{
				return;
			}
			// No object found.
			// All children constraints are necessarily false.
			// Check immediately.
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				((QCon)i.Current).VisitOnNull(parent.GetRoot());
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			i_root.Traverse(new _IVisitor4_522(sb));
			return sb.ToString();
		}

		private sealed class _IVisitor4_522 : IVisitor4
		{
			public _IVisitor4_522(StringBuilder sb)
			{
				this.sb = sb;
			}

			public void Visit(object obj)
			{
				QCandidate candidate = (QCandidate)obj;
				sb.Append(" ");
				sb.Append(candidate._key);
			}

			private readonly StringBuilder sb;
		}

		public void ClearOrdering()
		{
			i_ordered = null;
		}

		public Transaction Transaction()
		{
			return i_trans;
		}

		public bool WasLoadedFromClassIndex()
		{
			return _loadedFromClassIndex;
		}

		public bool FitsIntoExistingConstraintHierarchy(QCon constraint)
		{
			if (_field != null)
			{
				QField qf = constraint.GetField();
				if (qf != null)
				{
					if (_field.Name() != null && !_field.Name().Equals(qf.Name()))
					{
						return false;
					}
				}
			}
			if (i_classMetadata == null || constraint.IsNullConstraint())
			{
				return true;
			}
			ClassMetadata classMetadata = constraint.GetYapClass();
			if (classMetadata == null)
			{
				return false;
			}
			classMetadata = i_classMetadata.GetHigherOrCommonHierarchy(classMetadata);
			if (classMetadata == null)
			{
				return false;
			}
			i_classMetadata = classMetadata;
			return true;
		}

		private bool IsClassOnlyQuery()
		{
			if (_constraints._next != null)
			{
				return false;
			}
			if (!(_constraints._element is QConClass))
			{
				return false;
			}
			return !((QCon)_constraints._element).HasChildren();
		}
	}
}
