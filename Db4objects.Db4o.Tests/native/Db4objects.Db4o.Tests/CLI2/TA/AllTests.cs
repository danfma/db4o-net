/* Copyright (C) 2004-2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.TA
{
	class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
				{
                #if NET_2_0
                    typeof(ValueTypeActivationTestCase),
                #endif
				#if NET_2_0 || CF_2_0
					typeof(NullableTypeActivationTestCase)
				#endif
				};
		}
	}
}
