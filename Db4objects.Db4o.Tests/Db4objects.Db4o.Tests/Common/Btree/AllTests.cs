using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Btree;

namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Btree.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(BTreeAddRemoveTestCase), typeof(BTreeAsSetTestCase), typeof(BTreeNodeTestCase)
				, typeof(BTreeFreeTestCase), typeof(BTreePointerTestCase), typeof(BTreeRangeTestCase)
				, typeof(BTreeRollbackTestCase), typeof(BTreeSearchTestCase), typeof(BTreeSimpleTestCase)
				, typeof(SearcherLowestHighestTestCase), typeof(SearcherTestCase) };
		}
	}
}
