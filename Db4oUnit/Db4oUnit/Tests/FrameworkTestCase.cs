using System;
using System.Collections;
using Db4oUnit;
using Db4oUnit.Tests;

namespace Db4oUnit.Tests
{
	public class FrameworkTestCase : ITestCase
	{
		public static readonly Exception EXCEPTION = new Exception();

		public virtual void TestRunsGreen()
		{
			TestResult result = new TestResult();
			new RunsGreen().Run(result);
			Assert.IsTrue(result.Failures().Size() == 0, "not green");
		}

		public virtual void TestRunsRed()
		{
			TestResult result = new TestResult();
			new RunsRed(EXCEPTION).Run(result);
			Assert.IsTrue(result.Failures().Size() == 1, "not red");
		}

		public virtual void TestTestSuite()
		{
			RunTestAndExpect(new TestSuite(new ITest[] { new RunsGreen() }), 0);
			RunTestAndExpect(new TestSuite(new ITest[] { new RunsRed(EXCEPTION) }), 1);
			RunTestAndExpect(new TestSuite(new ITest[] { new RunsGreen(), new RunsRed(EXCEPTION
				) }), 1);
			RunTestAndExpect(new TestSuite(new ITest[] { new RunsRed(EXCEPTION), new RunsRed(
				EXCEPTION) }), 2);
			RunTestAndExpect(new TestSuite(new ITest[] { new RunsRed(EXCEPTION), new RunsGreen
				() }), 1);
			RunTestAndExpect(new TestSuite(new ITest[] { new RunsGreen(), new RunsGreen() }), 
				0);
		}

		public static void RunTestAndExpect(ITest test, int expFailures)
		{
			RunTestAndExpect(test, expFailures, true);
		}

		public static void RunTestAndExpect(ITest test, int expFailures, bool checkException
			)
		{
			TestResult result = new TestResult();
			test.Run(result);
			if (expFailures != result.Failures().Size())
			{
				Assert.Fail(result.Failures().ToString());
			}
			if (checkException)
			{
				for (IEnumerator iter = result.Failures().Iterator(); iter.MoveNext(); )
				{
					TestFailure failure = (TestFailure)iter.Current;
					Assert.IsTrue(EXCEPTION.Equals(failure.GetFailure()));
				}
			}
		}
	}
}
