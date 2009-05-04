/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Internal.Caching;
using Db4objects.Db4o.Tests.Common.Caching;

namespace Db4objects.Db4o.Tests.Common.Caching
{
	public class CacheTestSuite : FixtureTestSuiteDescription
	{
		public CacheTestSuite()
		{
			{
				FixtureProviders(new IFixtureProvider[] { new SubjectFixtureProvider(new IDeferred4
					[] { new _IDeferred4_10(), new _IDeferred4_14(), new _IDeferred4_18() }) });
				TestUnits(new Type[] { typeof(CacheTestUnit) });
			}
		}

		private sealed class _IDeferred4_10 : IDeferred4
		{
			public _IDeferred4_10()
			{
			}

			public object Value()
			{
				return CacheFactory.NewLRUCache(10);
			}
		}

		private sealed class _IDeferred4_14 : IDeferred4
		{
			public _IDeferred4_14()
			{
			}

			public object Value()
			{
				return CacheFactory.New2QCache(10);
			}
		}

		private sealed class _IDeferred4_18 : IDeferred4
		{
			public _IDeferred4_18()
			{
			}

			public object Value()
			{
				return CacheFactory.New2QXCache(10);
			}
		}
	}
}
