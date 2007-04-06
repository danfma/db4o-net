using System.Collections;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public class LazyQueryResult : AbstractLateQueryResult
	{
		public LazyQueryResult(Transaction trans) : base(trans)
		{
		}

		public override void LoadFromClassIndex(ClassMetadata clazz)
		{
			_iterable = ClassIndexIterable(clazz);
		}

		public override void LoadFromClassIndexes(ClassMetadataIterator classCollectionIterator
			)
		{
			_iterable = ClassIndexesIterable(classCollectionIterator);
		}

		public override void LoadFromQuery(QQuery query)
		{
			_iterable = new _AnonymousInnerClass28(this, query);
		}

		private sealed class _AnonymousInnerClass28 : IEnumerable
		{
			public _AnonymousInnerClass28(LazyQueryResult _enclosing, QQuery query)
			{
				this._enclosing = _enclosing;
				this.query = query;
			}

			public IEnumerator GetEnumerator()
			{
				return query.ExecuteLazy();
			}

			private readonly LazyQueryResult _enclosing;

			private readonly QQuery query;
		}
	}
}
