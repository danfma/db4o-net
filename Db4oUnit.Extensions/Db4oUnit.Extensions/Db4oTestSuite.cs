using System;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4oUnit.Extensions
{
	/// <summary>Base class for composable db4o test suites (AllTests classes inside each package, for instance).
	/// 	</summary>
	/// <remarks>Base class for composable db4o test suites (AllTests classes inside each package, for instance).
	/// 	</remarks>
	public abstract class Db4oTestSuite : AbstractDb4oTestCase, ITestSuiteBuilder
	{
		public virtual TestSuite Build()
		{
			return new Db4oTestSuiteBuilder(Fixture(), TestCases()).Build();
		}

		protected abstract override Type[] TestCases();
	}
}
