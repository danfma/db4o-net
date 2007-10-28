/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.NativeQueries
{
	class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
			{
				typeof(Cats.TestCatConsistency),
				typeof(Cat),
				typeof(DoubleNQTestCase),
				typeof(ListElementByIdentity),
#if !CF_2_0
				typeof(MultipleAssemblySupportTestCase),
#endif
				typeof(NativeQueriesTestCase),
				typeof(OptimizationFailuresTestCase),
				typeof(StringComparisonTestCase),
			};
		}
	}
}
