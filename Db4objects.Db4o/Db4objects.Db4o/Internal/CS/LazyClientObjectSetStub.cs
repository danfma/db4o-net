using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class LazyClientObjectSetStub
	{
		private readonly AbstractQueryResult _queryResult;

		private IIntIterator4 _idIterator;

		public LazyClientObjectSetStub(AbstractQueryResult queryResult, IIntIterator4 idIterator
			)
		{
			_queryResult = queryResult;
			_idIterator = idIterator;
		}

		public virtual IIntIterator4 IdIterator()
		{
			return _idIterator;
		}

		public virtual AbstractQueryResult QueryResult()
		{
			return _queryResult;
		}

		public virtual void Reset()
		{
			_idIterator = _queryResult.IterateIDs();
		}
	}
}
