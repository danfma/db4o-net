/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.IX;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.IX
{
	/// <summary>
	/// Index Path to represent a list of traversed index tree entries,
	/// used by IxTraverser
	/// </summary>
	internal class IxPath : IShallowClone, IVisitor4
	{
		internal int i_comparisonResult;

		internal int[] i_lowerAndUpperMatch;

		internal int i_upperNull = -1;

		internal Db4objects.Db4o.Internal.IX.IxPath i_next;

		internal IxTraverser i_traverser;

		internal IxTree i_tree;

		internal IVisitor4 _visitor;

		internal IxPath(IxTraverser a_traverser, Db4objects.Db4o.Internal.IX.IxPath a_next
			, IxTree a_tree, int a_comparisonResult, int[] lowerAndUpperMatch)
		{
			i_traverser = a_traverser;
			i_next = a_next;
			i_tree = a_tree;
			i_comparisonResult = a_comparisonResult;
			i_lowerAndUpperMatch = lowerAndUpperMatch;
		}

		internal virtual void Add(IVisitor4 visitor)
		{
			if (i_comparisonResult == 0 && i_traverser.i_take[QE.EQUAL])
			{
				i_tree.Visit(visitor, i_lowerAndUpperMatch);
			}
		}

		internal virtual void AddPrecedingToCandidatesTree(IVisitor4 visitor)
		{
			_visitor = visitor;
			if (i_tree._preceding != null)
			{
				if (i_next == null || i_next.i_tree != i_tree._preceding)
				{
					i_tree._preceding.Traverse(this);
				}
			}
			if (i_lowerAndUpperMatch != null)
			{
				int[] lowerAndUpperMatch = new int[] { i_upperNull, i_lowerAndUpperMatch[0] - 1 };
				i_tree.Visit(visitor, lowerAndUpperMatch);
			}
			else
			{
				if (i_comparisonResult < 0)
				{
					Visit(i_tree);
				}
			}
		}

		internal virtual void AddSubsequentToCandidatesTree(IVisitor4 visitor)
		{
			_visitor = visitor;
			if (i_tree._subsequent != null)
			{
				if (i_next == null || i_next.i_tree != i_tree._subsequent)
				{
					i_tree._subsequent.Traverse(this);
				}
			}
			if (i_lowerAndUpperMatch != null)
			{
				int[] lowerAndUpperMatch = new int[] { i_lowerAndUpperMatch[1] + 1, ((IxFileRange
					)i_tree)._entries - 1 };
				i_tree.Visit(visitor, lowerAndUpperMatch);
			}
			else
			{
				if (i_comparisonResult > 0)
				{
					Visit(i_tree);
				}
			}
		}

		internal virtual Db4objects.Db4o.Internal.IX.IxPath Append(Db4objects.Db4o.Internal.IX.IxPath
			 a_head, Db4objects.Db4o.Internal.IX.IxPath a_tail)
		{
			if (a_head == null)
			{
				return this;
			}
			i_next = a_head;
			return a_tail;
		}

		internal virtual Db4objects.Db4o.Internal.IX.IxPath Append(IxTree a_tree, int a_comparisonResult
			, int[] lowerAndUpperMatch)
		{
			i_next = new Db4objects.Db4o.Internal.IX.IxPath(i_traverser, null, a_tree, a_comparisonResult
				, lowerAndUpperMatch);
			i_next.i_tree = a_tree;
			return i_next;
		}

		internal virtual bool CarriesTheSame(Db4objects.Db4o.Internal.IX.IxPath a_path)
		{
			return i_tree == a_path.i_tree;
		}

		private void CheckUpperNull()
		{
			if (i_upperNull == -1)
			{
				i_upperNull = 0;
				i_traverser.i_handler.PrepareComparison(null);
				int res = i_tree.Compare(null);
				if (res != 0)
				{
					return;
				}
				int[] nullMatches = i_tree.LowerAndUpperMatch();
				if (nullMatches[0] == 0)
				{
					i_upperNull = nullMatches[1] + 1;
				}
				else
				{
					i_upperNull = 0;
				}
			}
		}

		public virtual void VisitMatch(FreespaceVisitor visitor)
		{
			if (i_next != null)
			{
				i_next.VisitMatch(visitor);
			}
			if (visitor.Visited())
			{
				return;
			}
			if (i_comparisonResult != 0)
			{
				return;
			}
			if (i_lowerAndUpperMatch == null)
			{
				i_tree.FreespaceVisit(visitor, 0);
				return;
			}
			if (i_lowerAndUpperMatch[1] < i_lowerAndUpperMatch[0])
			{
				return;
			}
			int ix = i_lowerAndUpperMatch[0];
			if (ix >= 0)
			{
				i_tree.FreespaceVisit(visitor, ix);
			}
		}

		public virtual void VisitPreceding(FreespaceVisitor visitor)
		{
			if (i_next != null)
			{
				i_next.VisitPreceding(visitor);
				if (visitor.Visited())
				{
					return;
				}
			}
			if (i_lowerAndUpperMatch != null)
			{
				int ix = i_lowerAndUpperMatch[0] - 1;
				if (ix >= 0)
				{
					i_tree.FreespaceVisit(visitor, ix);
				}
			}
			else
			{
				if (i_comparisonResult < 0)
				{
					i_tree.FreespaceVisit(visitor, 0);
				}
			}
			if (visitor.Visited())
			{
				return;
			}
			if (i_tree._preceding != null)
			{
				if (i_next == null || i_next.i_tree != i_tree._preceding)
				{
					((IxTree)i_tree._preceding).VisitLast(visitor);
				}
			}
		}

		public virtual void VisitSubsequent(FreespaceVisitor visitor)
		{
			if (i_next != null)
			{
				i_next.VisitSubsequent(visitor);
				if (visitor.Visited())
				{
					return;
				}
			}
			if (i_lowerAndUpperMatch != null)
			{
				int ix = i_lowerAndUpperMatch[1] + 1;
				if (ix < ((IxFileRange)i_tree)._entries)
				{
					i_tree.FreespaceVisit(visitor, ix);
				}
			}
			else
			{
				if (i_comparisonResult > 0)
				{
					i_tree.FreespaceVisit(visitor, 0);
				}
			}
			if (visitor.Visited())
			{
				return;
			}
			if (i_tree._subsequent != null)
			{
				if (i_next == null || i_next.i_tree != i_tree._subsequent)
				{
					((IxTree)i_tree._subsequent).VisitFirst(visitor);
				}
			}
		}

		internal virtual int CountMatching()
		{
			if (i_comparisonResult == 0)
			{
				if (i_lowerAndUpperMatch == null)
				{
					if (i_tree is IxRemove)
					{
						return 0;
					}
					return 1;
				}
				return i_lowerAndUpperMatch[1] - i_lowerAndUpperMatch[0] + 1;
			}
			return 0;
		}

		internal virtual int CountPreceding(bool a_takenulls)
		{
			int preceding = 0;
			if (i_tree._preceding != null)
			{
				if (i_next == null || i_next.i_tree != i_tree._preceding)
				{
					preceding += i_tree._preceding.Size();
				}
			}
			if (i_lowerAndUpperMatch != null)
			{
				if (a_takenulls)
				{
					i_upperNull = 0;
				}
				else
				{
					CheckUpperNull();
				}
				preceding += i_lowerAndUpperMatch[0] - i_upperNull;
			}
			else
			{
				if (i_comparisonResult < 0 && !(i_tree is IxRemove))
				{
					preceding++;
				}
			}
			return preceding;
		}

		internal virtual int CountSubsequent()
		{
			int subsequent = 0;
			if (i_tree._subsequent != null)
			{
				if (i_next == null || i_next.i_tree != i_tree._subsequent)
				{
					subsequent += i_tree._subsequent.Size();
				}
			}
			if (i_lowerAndUpperMatch != null)
			{
				subsequent += ((IxFileRange)i_tree)._entries - i_lowerAndUpperMatch[1] - 1;
			}
			else
			{
				if (i_comparisonResult > 0 && !(i_tree is IxRemove))
				{
					subsequent++;
				}
			}
			return subsequent;
		}

		public virtual object ShallowClone()
		{
			int[] lowerAndUpperMatch = null;
			if (i_lowerAndUpperMatch != null)
			{
				lowerAndUpperMatch = new int[] { i_lowerAndUpperMatch[0], i_lowerAndUpperMatch[1]
					 };
			}
			Db4objects.Db4o.Internal.IX.IxPath ret = new Db4objects.Db4o.Internal.IX.IxPath(i_traverser
				, i_next, i_tree, i_comparisonResult, lowerAndUpperMatch);
			ret.i_upperNull = i_upperNull;
			ret._visitor = _visitor;
			return ret;
		}

		public override string ToString()
		{
			return base.ToString();
			return i_tree.ToString();
		}

		public virtual void Visit(object a_object)
		{
			((IVisitor4)a_object).Visit(_visitor);
		}
	}
}
