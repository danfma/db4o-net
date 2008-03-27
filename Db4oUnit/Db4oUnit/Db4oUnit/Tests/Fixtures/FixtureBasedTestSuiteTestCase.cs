/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4oUnit.Mocking;
using Db4oUnit.Tests.Fixtures;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Tests.Fixtures
{
	public class FixtureBasedTestSuiteTestCase : ITestCase
	{
		internal static ContextVariable RecorderFixture = new ContextVariable();

		internal static ContextVariable Fixture1 = new ContextVariable();

		internal static ContextVariable Fixture2 = new ContextVariable();

		public sealed class TestUnit : ITestCase
		{
			public void TestFoo()
			{
				Record("testFoo");
			}

			public void TestBar()
			{
				Record("testBar");
			}

			private void Record(string test)
			{
				Recorder().Record(new MethodCall(test, Fixture1.Value(), Fixture2.Value()));
			}

			private MethodCallRecorder Recorder()
			{
				return ((MethodCallRecorder)RecorderFixture.Value());
			}
		}

		public virtual void Test()
		{
			MethodCallRecorder recorder = new MethodCallRecorder();
			new TestRunner(new _FixtureBasedTestSuite_41(recorder)).Run(new TestResult());
			//		System.out.println(CodeGenerator.generateMethodCallArray(recorder));
			recorder.Verify(new MethodCall[] { new MethodCall("testFoo", new object[] { "f11"
				, "f21" }), new MethodCall("testFoo", new object[] { "f11", "f22" }), new MethodCall
				("testFoo", new object[] { "f12", "f21" }), new MethodCall("testFoo", new object
				[] { "f12", "f22" }), new MethodCall("testBar", new object[] { "f11", "f21" }), 
				new MethodCall("testBar", new object[] { "f11", "f22" }), new MethodCall("testBar"
				, new object[] { "f12", "f21" }), new MethodCall("testBar", new object[] { "f12"
				, "f22" }) });
		}

		private sealed class _FixtureBasedTestSuite_41 : FixtureBasedTestSuite
		{
			public _FixtureBasedTestSuite_41(MethodCallRecorder recorder)
			{
				this.recorder = recorder;
			}

			public override IFixtureProvider[] FixtureProviders()
			{
				return new IFixtureProvider[] { new SimpleFixtureProvider(FixtureBasedTestSuiteTestCase
					.RecorderFixture, new object[] { recorder }), new SimpleFixtureProvider(FixtureBasedTestSuiteTestCase
					.Fixture1, new object[] { "f11", "f12" }), new SimpleFixtureProvider(FixtureBasedTestSuiteTestCase
					.Fixture2, new object[] { "f21", "f22" }) };
			}

			public override Type[] TestUnits()
			{
				return new Type[] { typeof(FixtureBasedTestSuiteTestCase.TestUnit) };
			}

			private readonly MethodCallRecorder recorder;
		}

		public virtual void TestLabel()
		{
			FixtureBasedTestSuite suite = new _FixtureBasedTestSuite_70();
			IEnumerable labels = Iterators.Map(suite, new _IFunction4_82());
			Iterator4Assert.AreEqual(new object[] { TestLabel("testFoo", 0, 0), TestLabel("testFoo"
				, 1, 0), TestLabel("testFoo", 0, 1), TestLabel("testFoo", 1, 1), TestLabel("testBar"
				, 0, 0), TestLabel("testBar", 1, 0), TestLabel("testBar", 0, 1), TestLabel("testBar"
				, 1, 1) }, labels.GetEnumerator());
		}

		private sealed class _FixtureBasedTestSuite_70 : FixtureBasedTestSuite
		{
			public _FixtureBasedTestSuite_70()
			{
			}

			public override IFixtureProvider[] FixtureProviders()
			{
				return new IFixtureProvider[] { new SimpleFixtureProvider("f1", FixtureBasedTestSuiteTestCase
					.Fixture1, new object[] { "f11", "f12" }), new SimpleFixtureProvider("f2", FixtureBasedTestSuiteTestCase
					.Fixture2, new object[] { "f21", "f22" }) };
			}

			public override Type[] TestUnits()
			{
				return new Type[] { typeof(FixtureBasedTestSuiteTestCase.TestUnit) };
			}
		}

		private sealed class _IFunction4_82 : IFunction4
		{
			public _IFunction4_82()
			{
			}

			public object Apply(object arg)
			{
				return ((ITest)arg).GetLabel();
			}
		}

		private string TestLabel(string testMethod, int fixture1Index, int fixture2Index)
		{
			string prefix = "(f2[" + fixture1Index + "]) (f1[" + fixture2Index + "]) ";
			return prefix + typeof(FixtureBasedTestSuiteTestCase.TestUnit).FullName + "." + testMethod;
		}
	}
}