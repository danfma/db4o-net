using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Defrag
{
	class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
			       {
#if !CF
			       	typeof(DefragUnavailableGenericArgumentTestCase)
#endif
			       };
		}
	}
}
