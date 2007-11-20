/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
namespace Db4objects.Db4o.Tests.CLI2.Collections
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[] { 
                typeof(GenericDictionaryTestCase),
                typeof(ArrayDictionary4TestCase),
                typeof(ArrayDictionary4TATestCase),
            };
		}
	}
}
