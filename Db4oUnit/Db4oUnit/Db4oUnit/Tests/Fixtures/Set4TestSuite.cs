/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4oUnit.Tests.Fixtures;

namespace Db4oUnit.Tests.Fixtures
{
	public class Set4TestSuite : FixtureBasedTestSuite
	{
		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(Set4TestSuite)).Run();
		}

		public override IFixtureProvider[] FixtureProviders()
		{
			return new IFixtureProvider[] { new SubjectFixtureProvider(new object[] { new _IDeferred4_17
				(), new _IDeferred4_22() }), new MultiValueFixtureProvider(new object[][] { new 
				object[] {  }, new object[] { "foo", "bar", "baz" }, new object[] { "foo" }, new 
				object[] { 42, -1 } }) };
		}

		private sealed class _IDeferred4_17 : IDeferred4
		{
			public _IDeferred4_17()
			{
			}

			public object Value()
			{
				return new CollectionSet4();
			}
		}

		private sealed class _IDeferred4_22 : IDeferred4
		{
			public _IDeferred4_22()
			{
			}

			public object Value()
			{
				return new HashtableSet4();
			}
		}

		public override Type[] TestUnits()
		{
			return new Type[] { typeof(Set4TestUnit) };
		}
		//			Iterable4TestUnit.class,
	}
}