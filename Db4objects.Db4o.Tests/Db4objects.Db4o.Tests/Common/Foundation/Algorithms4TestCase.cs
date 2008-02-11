/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class Algorithms4TestCase : ITestCase
	{
		public class QuickSortableIntArray : IQuickSortable4
		{
			private int[] ints;

			public QuickSortableIntArray(int[] ints)
			{
				this.ints = ints;
			}

			public virtual int Compare(int leftIndex, int rightIndex)
			{
				return ints[leftIndex] - ints[rightIndex];
			}

			public virtual int Size()
			{
				return ints.Length;
			}

			public virtual void Swap(int leftIndex, int rightIndex)
			{
				int temp = ints[leftIndex];
				ints[leftIndex] = ints[rightIndex];
				ints[rightIndex] = temp;
			}

			public virtual void AssertSorted()
			{
				for (int i = 0; i < ints.Length; i++)
				{
					Assert.AreEqual(i + 1, ints[i]);
				}
			}
		}

		public virtual void TestUnsorted()
		{
			int[] ints = new int[] { 3, 5, 2, 1, 4 };
			AssertQSort(ints);
		}

		public virtual void TestStackUsage()
		{
			int[] ints = new int[50000];
			for (int i = 0; i < ints.Length; i++)
			{
				ints[i] = i + 1;
			}
			AssertQSort(ints);
		}

		private void AssertQSort(int[] ints)
		{
			Algorithms4TestCase.QuickSortableIntArray sample = new Algorithms4TestCase.QuickSortableIntArray
				(ints);
			Algorithms4.Qsort(sample);
			sample.AssertSorted();
		}
	}
}
