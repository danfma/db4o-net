/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Foundation.Network;

namespace Db4objects.Db4o.Tests.Common.Foundation.Network
{
	public class AllTests : ITestSuiteBuilder
	{
		public virtual TestSuite Build()
		{
			return new ReflectionTestSuiteBuilder(new Type[] { typeof(NetworkSocketTestCase) }
				).Build();
		}

		public static void Main(string[] args)
		{
			new TestRunner(typeof(Db4objects.Db4o.Tests.Common.Foundation.Network.AllTests)).
				Run();
		}
	}
}
