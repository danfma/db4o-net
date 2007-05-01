using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Constraints;

namespace Db4objects.Db4o.Tests.Common.Constraints
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Constraints.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(UniqueFieldIndexTestCase) };
		}
	}
}