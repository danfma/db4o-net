/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <summary>Object constraint on queries</summary>
	/// <exclude></exclude>
	public class QConObject : QCon
	{
		public object i_object;

		public int i_objectID;

		[System.NonSerialized]
		internal ClassMetadata i_yapClass;

		public int i_yapClassID;

		public QField i_field;

		[System.NonSerialized]
		internal IComparable4 i_comparator;

		public IObjectAttribute i_attributeProvider;

		[System.NonSerialized]
		private bool i_selfComparison = false;

		public QConObject()
		{
		}

		public QConObject(Transaction a_trans, QCon a_parent, QField a_field, object a_object
			) : base(a_trans)
		{
			i_parent = a_parent;
			if (a_object is ICompare)
			{
				a_object = ((ICompare)a_object).Compare();
			}
			i_object = a_object;
			i_field = a_field;
			AssociateYapClass(a_trans, a_object);
		}

		private void AssociateYapClass(Transaction a_trans, object a_object)
		{
			if (a_object == null)
			{
				i_object = null;
				i_comparator = Null.INSTANCE;
				i_yapClass = null;
			}
			else
			{
				i_yapClass = a_trans.Container().ProduceClassMetadata(a_trans.Reflector().ForObject
					(a_object));
				if (i_yapClass != null)
				{
					i_object = i_yapClass.GetComparableObject(a_object);
					if (a_object != i_object)
					{
						i_attributeProvider = i_yapClass.Config().QueryAttributeProvider();
						i_yapClass = a_trans.Container().ProduceClassMetadata(a_trans.Reflector().ForObject
							(i_object));
					}
					if (i_yapClass != null)
					{
						i_yapClass.CollectConstraints(a_trans, this, i_object, new _IVisitor4_82(this));
					}
					else
					{
						AssociateYapClass(a_trans, null);
					}
				}
				else
				{
					AssociateYapClass(a_trans, null);
				}
			}
		}

		private sealed class _IVisitor4_82 : IVisitor4
		{
			public _IVisitor4_82(QConObject _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				this._enclosing.AddConstraint((QCon)obj);
			}

			private readonly QConObject _enclosing;
		}

		public override bool CanBeIndexLeaf()
		{
			return (i_yapClass != null && i_yapClass.IsPrimitive()) || Evaluator().Identity();
		}

		public override bool CanLoadByIndex()
		{
			if (i_field == null)
			{
				return false;
			}
			if (i_field.i_yapField == null)
			{
				return false;
			}
			if (!i_field.i_yapField.HasIndex())
			{
				return false;
			}
			if (!i_evaluator.SupportsIndex())
			{
				return false;
			}
			return i_field.i_yapField.CanLoadByIndex();
		}

		internal override bool Evaluate(QCandidate a_candidate)
		{
			try
			{
				return a_candidate.Evaluate(this, i_evaluator);
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal override void EvaluateEvaluationsExec(QCandidates a_candidates, bool rereadObject
			)
		{
			if (i_field.IsSimple())
			{
				bool hasEvaluation = false;
				IEnumerator i = IterateChildren();
				while (i.MoveNext())
				{
					if (i.Current is QConEvaluation)
					{
						hasEvaluation = true;
						break;
					}
				}
				if (hasEvaluation)
				{
					a_candidates.Traverse(i_field);
					IEnumerator j = IterateChildren();
					while (j.MoveNext())
					{
						((QCon)j.Current).EvaluateEvaluationsExec(a_candidates, false);
					}
				}
			}
		}

		internal override void EvaluateSelf()
		{
			if (i_yapClass != null)
			{
				if (!(i_yapClass is PrimitiveFieldHandler))
				{
					if (!i_evaluator.Identity())
					{
						i_selfComparison = true;
					}
					object transactionalObject = i_yapClass.WrapWithTransactionContext(Transaction(), 
						i_object);
					i_comparator = i_yapClass.PrepareComparison(transactionalObject);
				}
			}
			base.EvaluateSelf();
			i_selfComparison = false;
		}

		internal override void Collect(QCandidates a_candidates)
		{
			if (i_field.IsClass())
			{
				a_candidates.Traverse(i_field);
				a_candidates.Filter(i_candidates);
			}
		}

		internal override void EvaluateSimpleExec(QCandidates a_candidates)
		{
			if (i_field.IsSimple() || IsNullConstraint())
			{
				a_candidates.Traverse(i_field);
				PrepareComparison(i_field);
				a_candidates.Filter(this);
			}
		}

		internal virtual IComparable4 GetComparator(QCandidate a_candidate)
		{
			if (i_comparator == null)
			{
				return a_candidate.PrepareComparison(i_trans.Container(), i_object);
			}
			return i_comparator;
		}

		internal override ClassMetadata GetYapClass()
		{
			return i_yapClass;
		}

		public override QField GetField()
		{
			return i_field;
		}

		internal virtual int GetObjectID()
		{
			if (i_objectID == 0)
			{
				i_objectID = i_trans.Container().GetID(i_trans, i_object);
				if (i_objectID == 0)
				{
					i_objectID = -1;
				}
			}
			return i_objectID;
		}

		public override bool HasObjectInParentPath(object obj)
		{
			if (obj == i_object)
			{
				return true;
			}
			return base.HasObjectInParentPath(obj);
		}

		public override int IdentityID()
		{
			if (i_evaluator.Identity())
			{
				int id = GetObjectID();
				if (id != 0)
				{
					if (!(i_evaluator is QENot))
					{
						return id;
					}
				}
			}
			return 0;
		}

		internal override bool IsNullConstraint()
		{
			return i_object == null;
		}

		internal override void Log(string indent)
		{
		}

		internal override string LogObject()
		{
			return string.Empty;
		}

		internal override void Marshall()
		{
			base.Marshall();
			GetObjectID();
			if (i_yapClass != null)
			{
				i_yapClassID = i_yapClass.GetID();
			}
		}

		public override bool OnSameFieldAs(QCon other)
		{
			if (!(other is Db4objects.Db4o.Internal.Query.Processor.QConObject))
			{
				return false;
			}
			return i_field == ((Db4objects.Db4o.Internal.Query.Processor.QConObject)other).i_field;
		}

		internal virtual void PrepareComparison(QField a_field)
		{
			if (IsNullConstraint() & !a_field.IsArray())
			{
				i_comparator = Null.INSTANCE;
			}
			else
			{
				i_comparator = a_field.PrepareComparison(i_object);
			}
		}

		internal override void RemoveChildrenJoins()
		{
			base.RemoveChildrenJoins();
			_children = null;
		}

		internal override QCon ShareParent(object a_object, bool[] removeExisting)
		{
			if (i_parent == null)
			{
				return null;
			}
			object obj = i_field.Coerce(a_object);
			if (obj == No4.INSTANCE)
			{
				return null;
			}
			return i_parent.AddSharedConstraint(i_field, obj);
		}

		internal override QConClass ShareParentForClass(IReflectClass a_class, bool[] removeExisting
			)
		{
			if (i_parent == null)
			{
				return null;
			}
			if (!i_field.CanHold(a_class))
			{
				return null;
			}
			QConClass newConstraint = new QConClass(i_trans, i_parent, i_field, a_class);
			i_parent.AddConstraint(newConstraint);
			return newConstraint;
		}

		internal object Translate(object candidate)
		{
			if (i_attributeProvider != null)
			{
				i_candidates.i_trans.Container().ActivateDefaultDepth(i_candidates.i_trans, candidate
					);
				return i_attributeProvider.Attribute(candidate);
			}
			return candidate;
		}

		internal override void Unmarshall(Transaction trans)
		{
			if (i_trans == null)
			{
				base.Unmarshall(trans);
				if (i_object == null)
				{
					i_comparator = Null.INSTANCE;
				}
				if (i_yapClassID != 0)
				{
					i_yapClass = trans.Container().ClassMetadataForId(i_yapClassID);
				}
				if (i_field != null)
				{
					i_field.Unmarshall(trans);
				}
				if (i_objectID > 0)
				{
					object obj = trans.Container().GetByID(trans, i_objectID);
					if (obj != null)
					{
						i_object = obj;
					}
				}
			}
		}

		public override void Visit(object obj)
		{
			QCandidate qc = (QCandidate)obj;
			bool res = true;
			bool processed = false;
			if (i_selfComparison)
			{
				ClassMetadata yc = qc.ReadYapClass();
				if (yc != null)
				{
					res = i_evaluator.Not(i_yapClass.GetHigherHierarchy(yc) == i_yapClass);
					processed = true;
				}
			}
			if (!processed)
			{
				res = Evaluate(qc);
			}
			if (HasOrdering() && res && qc.FieldIsAvailable())
			{
				object cmp = qc.Value();
				if (cmp != null && i_field != null)
				{
					IComparable4 comparatorBackup = i_comparator;
					i_comparator = i_field.PrepareComparison(qc.Value());
					i_candidates.AddOrder(new QOrder(this, qc));
					i_comparator = comparatorBackup.PrepareComparison(i_object);
				}
			}
			Visit1(qc.GetRoot(), this, res);
		}

		public override IConstraint Contains()
		{
			lock (StreamLock())
			{
				i_evaluator = i_evaluator.Add(new QEContains(true));
				return this;
			}
		}

		public override IConstraint Equal()
		{
			lock (StreamLock())
			{
				i_evaluator = i_evaluator.Add(new QEEqual());
				return this;
			}
		}

		public override object GetObject()
		{
			lock (StreamLock())
			{
				return i_object;
			}
		}

		public override IConstraint Greater()
		{
			lock (StreamLock())
			{
				i_evaluator = i_evaluator.Add(new QEGreater());
				return this;
			}
		}

		public override IConstraint Identity()
		{
			lock (StreamLock())
			{
				if (i_object == null)
				{
					return this;
				}
				int id = GetObjectID();
				if (id <= 0)
				{
					i_objectID = 0;
					Exceptions4.ThrowRuntimeException(51);
				}
				RemoveChildrenJoins();
				i_evaluator = i_evaluator.Add(new QEIdentity());
				return this;
			}
		}

		public override IConstraint Like()
		{
			lock (StreamLock())
			{
				i_evaluator = i_evaluator.Add(new QEContains(false));
				return this;
			}
		}

		public override IConstraint Smaller()
		{
			lock (StreamLock())
			{
				i_evaluator = i_evaluator.Add(new QESmaller());
				return this;
			}
		}

		public override IConstraint StartsWith(bool caseSensitive)
		{
			lock (StreamLock())
			{
				i_evaluator = i_evaluator.Add(new QEStartsWith(caseSensitive));
				return this;
			}
		}

		public override IConstraint EndsWith(bool caseSensitive)
		{
			lock (StreamLock())
			{
				i_evaluator = i_evaluator.Add(new QEEndsWith(caseSensitive));
				return this;
			}
		}

		public override string ToString()
		{
			return base.ToString();
			string str = "QConObject ";
			if (i_object != null)
			{
				str += i_object.ToString();
			}
			return str;
		}
	}
}
