using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.TA.Tests;

namespace Db4objects.Db4o.TA.Tests
{
	public class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[] { typeof(TransparentActivationSupportTestCase), typeof(TransparentActivationTestCase)
				 };
		}
	}
}