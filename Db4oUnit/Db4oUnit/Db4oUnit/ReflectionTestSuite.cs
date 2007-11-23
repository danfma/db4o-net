/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;

namespace Db4oUnit
{
	/// <summary>Support for hierarchically chained test suites.</summary>
	/// <remarks>
	/// Support for hierarchically chained test suites.
	/// In the topmost test package define an AllTests class which extends
	/// ReflectionTestSuite and returns all subpackage.AllTests classes as
	/// testCases. Example:
	/// package org.acme.tests;
	/// public class AllTests extends ReflectionTestSuite {
	/// protected Class[] testCases() {
	/// return new Class[] {
	/// org.acme.tests.subsystem1.AllTests.class,
	/// org.acme.tests.subsystem2.AllTests.class,
	/// };
	/// }
	/// }
	/// </remarks>
	public abstract class ReflectionTestSuite : ITestSuiteBuilder
	{
		public virtual TestSuite Build()
		{
			return new ReflectionTestSuiteBuilder(TestCases()).Build();
		}

		protected abstract Type[] TestCases();

		public virtual int Run()
		{
			return new TestRunner(Build()).Run();
		}
	}
}
