/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Tests;

namespace Db4oUnit.Tests
{
	public class AllTests : ReflectionTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[] { typeof(TestExceptionWithInnerCause), typeof(AssertTestCase), 
				typeof(CompositeTestListenerTestCase), typeof(ExceptionInTearDownDoesNotShadowTestCase
				), typeof(FrameworkTestCase), typeof(ReflectionTestSuiteBuilderTestCase), typeof(
				ReinstantiatePerMethodTest), typeof(TestLifeCycleTestCase), typeof(TestRunnerTestCase
				), typeof(Db4oUnit.Tests.Data.AllTests), typeof(Db4oUnit.Tests.Fixtures.AllTests
				) };
		}

		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(Db4oUnit.Tests.AllTests)).Run();
		}
	}
}
