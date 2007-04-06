using Db4objects.Db4o.Internal.Query.Result;
using Db4objects.Db4o.Tests.Common.Querying;

namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class LazyQueryResultTestCase : QueryResultTestCase
	{
		public static void Main(string[] args)
		{
			new LazyQueryResultTestCase().RunSolo();
		}

		protected override AbstractQueryResult NewQueryResult()
		{
			return new LazyQueryResult(Trans());
		}
	}
}
