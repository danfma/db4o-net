/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System.Collections;
using System.Reflection;
using System.Text;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Nativequery.Expr;
using Db4objects.Db4o.Nativequery.Optimization;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tools.NativeQueries;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.NativeQueries
{
	public class AbstractNativeQueriesTestCase : AbstractDb4oTestCase
	{
		protected void AssertNQResult(object predicate, params object[] expected)
		{
			IObjectSet os = QueryFromPredicate(predicate).Execute();
			string actualString = ToString(os);
			Assert.AreEqual(expected.Length, os.Size(), "Expected: " + ToString(expected) + ", Actual: " + actualString);

			foreach (object item in expected)
			{
				Assert.IsTrue(os.Contains(item), "Expected item: " + item + " but got: " + actualString);
			}
		}

		private string ToString(IEnumerable os)
		{
			return Iterators.ToString(os.GetEnumerator());
		}

		private IQuery QueryFromPredicate(object predicate)
		{
			MethodInfo match = predicate.GetType().GetMethod("Match");
			IExpression expression = (new QueryExpressionBuilder ()).FromMethod(match);
			IQuery q = NewQuery(match.GetParameters()[0].ParameterType);
			new SODAQueryBuilder().OptimizeQuery(expression, q, predicate, new Db4objects.Db4o.Instrumentation.DefaultNativeClassFactory());
			return q;
		}
	}
}