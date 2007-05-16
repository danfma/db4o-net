/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Internal.Btree;

namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class SearcherLowestHighestTestCase : ITestCase, ITestLifeCycle
	{
		private Searcher _searcher;

		private const int SEARCH_FOR = 9;

		private static readonly int[] EVEN_EVEN_VALUES = new int[] { 4, 9, 9, 9, 9, 11, 13
			, 17 };

		private static readonly int[] EVEN_ODD_VALUES = new int[] { 4, 5, 9, 9, 9, 11, 13
			, 17 };

		private static readonly int[] ODD_EVEN_VALUES = new int[] { 4, 9, 9, 9, 9, 11, 13
			 };

		private static readonly int[] ODD_ODD_VALUES = new int[] { 4, 5, 9, 9, 9, 11, 13 };

		private static readonly int[] NO_MATCH_EVEN = new int[] { 4, 5, 10, 10, 10, 11 };

		private static readonly int[] NO_MATCH_ODD = new int[] { 4, 5, 10, 10, 10, 11, 13
			 };

		private static readonly int[][] MATCH_VALUES = new int[][] { EVEN_EVEN_VALUES, EVEN_ODD_VALUES
			, ODD_EVEN_VALUES, ODD_ODD_VALUES };

		private static readonly int[][] NO_MATCH_VALUES = new int[][] { NO_MATCH_EVEN, NO_MATCH_ODD
			 };

		private static readonly SearchTarget[] ALL_TARGETS = new SearchTarget[] { SearchTarget
			.LOWEST, SearchTarget.ANY, SearchTarget.HIGHEST };

		public virtual void TestMatch()
		{
			for (int i = 0; i < MATCH_VALUES.Length; i++)
			{
				int[] values = MATCH_VALUES[i];
				int lo = LowMatch(values);
				Search(values, SearchTarget.LOWEST);
				Assert.AreEqual(lo, _searcher.Cursor());
				Assert.IsTrue(_searcher.FoundMatch());
				int hi = HighMatch(values);
				Search(values, SearchTarget.HIGHEST);
				Assert.AreEqual(hi, _searcher.Cursor());
				Assert.IsTrue(_searcher.FoundMatch());
			}
		}

		public virtual void TestNoMatch()
		{
			for (int i = 0; i < NO_MATCH_VALUES.Length; i++)
			{
				int[] values = NO_MATCH_VALUES[i];
				int lo = LowMatch(values);
				Search(values, SearchTarget.LOWEST);
				Assert.AreEqual(lo, _searcher.Cursor());
				Assert.IsFalse(_searcher.FoundMatch());
				int hi = HighMatch(values);
				Search(values, SearchTarget.HIGHEST);
				Assert.AreEqual(hi, _searcher.Cursor());
				Assert.IsFalse(_searcher.FoundMatch());
			}
		}

		public virtual void TestEmpty()
		{
			int[] values = new int[] {  };
			for (int i = 0; i < ALL_TARGETS.Length; i++)
			{
				Search(values, ALL_TARGETS[i]);
				Assert.AreEqual(0, _searcher.Cursor());
				Assert.IsFalse(_searcher.FoundMatch());
				Assert.IsFalse(_searcher.BeforeFirst());
				Assert.IsFalse(_searcher.AfterLast());
			}
		}

		public virtual void TestOneValueMatch()
		{
			int[] values = new int[] { 9 };
			for (int i = 0; i < ALL_TARGETS.Length; i++)
			{
				Search(values, ALL_TARGETS[i]);
				Assert.AreEqual(0, _searcher.Cursor());
				Assert.IsTrue(_searcher.FoundMatch());
				Assert.IsFalse(_searcher.BeforeFirst());
				Assert.IsFalse(_searcher.AfterLast());
			}
		}

		public virtual void TestOneValueLower()
		{
			int[] values = new int[] { 8 };
			for (int i = 0; i < ALL_TARGETS.Length; i++)
			{
				Search(values, ALL_TARGETS[i]);
				Assert.AreEqual(0, _searcher.Cursor());
				Assert.IsFalse(_searcher.FoundMatch());
				Assert.IsFalse(_searcher.BeforeFirst());
				Assert.IsTrue(_searcher.AfterLast());
			}
		}

		public virtual void TestOneValueHigher()
		{
			int[] values = new int[] { 8 };
			for (int i = 0; i < ALL_TARGETS.Length; i++)
			{
				Search(values, ALL_TARGETS[i]);
				Assert.AreEqual(0, _searcher.Cursor());
				Assert.IsFalse(_searcher.FoundMatch());
				Assert.IsFalse(_searcher.BeforeFirst());
				Assert.IsTrue(_searcher.AfterLast());
			}
		}

		public virtual void TestTwoValuesMatch()
		{
			int[] values = new int[] { 9, 9 };
			Search(values, SearchTarget.LOWEST);
			Assert.AreEqual(0, _searcher.Cursor());
			Assert.IsTrue(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
			Search(values, SearchTarget.ANY);
			Assert.IsTrue(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
			Search(values, SearchTarget.HIGHEST);
			Assert.AreEqual(1, _searcher.Cursor());
			Assert.IsTrue(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
		}

		public virtual void TestTwoValuesLowMatch()
		{
			int[] values = new int[] { 9, 10 };
			Search(values, SearchTarget.LOWEST);
			Assert.AreEqual(0, _searcher.Cursor());
			Assert.IsTrue(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
			Search(values, SearchTarget.ANY);
			Assert.AreEqual(0, _searcher.Cursor());
			Assert.IsTrue(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
			Search(values, SearchTarget.HIGHEST);
			Assert.AreEqual(0, _searcher.Cursor());
			Assert.IsTrue(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
		}

		public virtual void TestTwoValuesHighMatch()
		{
			int[] values = new int[] { 6, 9 };
			Search(values, SearchTarget.LOWEST);
			Assert.AreEqual(1, _searcher.Cursor());
			Assert.IsTrue(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
			Search(values, SearchTarget.ANY);
			Assert.AreEqual(1, _searcher.Cursor());
			Assert.IsTrue(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
			Search(values, SearchTarget.HIGHEST);
			Assert.AreEqual(1, _searcher.Cursor());
			Assert.IsTrue(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
		}

		public virtual void TestTwoValuesInBetween()
		{
			int[] values = new int[] { 8, 10 };
			Search(values, SearchTarget.LOWEST);
			Assert.AreEqual(0, _searcher.Cursor());
			Assert.IsFalse(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
			Search(values, SearchTarget.ANY);
			Assert.AreEqual(0, _searcher.Cursor());
			Assert.IsFalse(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
			Search(values, SearchTarget.HIGHEST);
			Assert.AreEqual(0, _searcher.Cursor());
			Assert.IsFalse(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
		}

		public virtual void TestTwoValuesLower()
		{
			int[] values = new int[] { 7, 8 };
			Search(values, SearchTarget.LOWEST);
			Assert.AreEqual(1, _searcher.Cursor());
			Assert.IsFalse(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsTrue(_searcher.AfterLast());
			Search(values, SearchTarget.ANY);
			Assert.AreEqual(1, _searcher.Cursor());
			Assert.IsFalse(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsTrue(_searcher.AfterLast());
			Search(values, SearchTarget.HIGHEST);
			Assert.AreEqual(1, _searcher.Cursor());
			Assert.IsFalse(_searcher.FoundMatch());
			Assert.IsFalse(_searcher.BeforeFirst());
			Assert.IsTrue(_searcher.AfterLast());
		}

		public virtual void TestTwoValuesHigher()
		{
			int[] values = new int[] { 10, 11 };
			Search(values, SearchTarget.LOWEST);
			Assert.AreEqual(0, _searcher.Cursor());
			Assert.IsFalse(_searcher.FoundMatch());
			Assert.IsTrue(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
			Search(values, SearchTarget.ANY);
			Assert.AreEqual(0, _searcher.Cursor());
			Assert.IsFalse(_searcher.FoundMatch());
			Assert.IsTrue(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
			Search(values, SearchTarget.HIGHEST);
			Assert.AreEqual(0, _searcher.Cursor());
			Assert.IsFalse(_searcher.FoundMatch());
			Assert.IsTrue(_searcher.BeforeFirst());
			Assert.IsFalse(_searcher.AfterLast());
		}

		private int Search(int[] values, SearchTarget target)
		{
			_searcher = new Searcher(target, values.Length);
			while (_searcher.Incomplete())
			{
				_searcher.ResultIs(values[_searcher.Cursor()] - SEARCH_FOR);
			}
			return _searcher.Cursor();
		}

		private int LowMatch(int[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] == SEARCH_FOR)
				{
					return i;
				}
				if (values[i] > SEARCH_FOR)
				{
					if (i == 0)
					{
						return 0;
					}
					return i - 1;
				}
			}
			throw new ArgumentException("values");
		}

		private int HighMatch(int[] values)
		{
			for (int i = values.Length - 1; i >= 0; i--)
			{
				if (values[i] <= SEARCH_FOR)
				{
					return i;
				}
			}
			throw new ArgumentException("values");
		}

		public virtual void SetUp()
		{
			_searcher = null;
		}

		public virtual void TearDown()
		{
		}
	}
}
