using System.Collections;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public sealed class ObjectInfoCollectionImpl : IObjectInfoCollection
	{
		public static readonly IObjectInfoCollection EMPTY = new Db4objects.Db4o.Internal.ObjectInfoCollectionImpl
			(Iterators.EMPTY_ITERABLE);

		public IEnumerable _collection;

		public ObjectInfoCollectionImpl(IEnumerable collection)
		{
			_collection = collection;
		}

		public IEnumerator GetEnumerator()
		{
			return _collection.GetEnumerator();
		}
	}
}