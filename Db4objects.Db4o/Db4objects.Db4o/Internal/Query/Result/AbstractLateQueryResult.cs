/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Internal.Query.Result;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public abstract class AbstractLateQueryResult : AbstractQueryResult
	{
		protected IEnumerable _iterable;

		public AbstractLateQueryResult(Transaction transaction) : base(transaction)
		{
		}

		public override AbstractQueryResult SupportSize()
		{
			return ToIdTree();
		}

		public override AbstractQueryResult SupportSort()
		{
			return ToIdList();
		}

		public override AbstractQueryResult SupportElementAccess()
		{
			return ToIdList();
		}

		protected override int KnownSize()
		{
			return 0;
		}

		public override IIntIterator4 IterateIDs()
		{
			if (_iterable == null)
			{
				throw new InvalidOperationException();
			}
			return new IntIterator4Adaptor(_iterable);
		}

		public override AbstractQueryResult ToIdList()
		{
			return ToIdTree().ToIdList();
		}

		public virtual bool SkipClass(ClassMetadata yapClass)
		{
			if (yapClass.GetName() == null)
			{
				return true;
			}
			IReflectClass claxx = yapClass.ClassReflector();
			if (Stream().i_handlers.ICLASS_INTERNAL.IsAssignableFrom(claxx))
			{
				return true;
			}
			return false;
		}

		protected virtual IEnumerable ClassIndexesIterable(ClassMetadataIterator classCollectionIterator
			)
		{
			return new _AnonymousInnerClass61(this, classCollectionIterator);
		}

		private sealed class _AnonymousInnerClass61 : IEnumerable
		{
			public _AnonymousInnerClass61(AbstractLateQueryResult _enclosing, ClassMetadataIterator
				 classCollectionIterator)
			{
				this._enclosing = _enclosing;
				this.classCollectionIterator = classCollectionIterator;
			}

			public IEnumerator GetEnumerator()
			{
				return new CompositeIterator4(new _AnonymousInnerClass64(this, classCollectionIterator
					));
			}

			private sealed class _AnonymousInnerClass64 : MappingIterator
			{
				public _AnonymousInnerClass64(_AnonymousInnerClass61 _enclosing, ClassMetadataIterator
					 baseArg1) : base(baseArg1)
				{
					this._enclosing = _enclosing;
				}

				protected override object Map(object current)
				{
					ClassMetadata yapClass = (ClassMetadata)current;
					if (this._enclosing._enclosing.SkipClass(yapClass))
					{
						return MappingIterator.SKIP;
					}
					return this._enclosing._enclosing.ClassIndexIterator(yapClass);
				}

				private readonly _AnonymousInnerClass61 _enclosing;
			}

			private readonly AbstractLateQueryResult _enclosing;

			private readonly ClassMetadataIterator classCollectionIterator;
		}

		protected virtual IEnumerable ClassIndexIterable(ClassMetadata clazz)
		{
			return new _AnonymousInnerClass79(this, clazz);
		}

		private sealed class _AnonymousInnerClass79 : IEnumerable
		{
			public _AnonymousInnerClass79(AbstractLateQueryResult _enclosing, ClassMetadata clazz
				)
			{
				this._enclosing = _enclosing;
				this.clazz = clazz;
			}

			public IEnumerator GetEnumerator()
			{
				return this._enclosing.ClassIndexIterator(clazz);
			}

			private readonly AbstractLateQueryResult _enclosing;

			private readonly ClassMetadata clazz;
		}

		public virtual IEnumerator ClassIndexIterator(ClassMetadata clazz)
		{
			return BTreeClassIndexStrategy.Iterate(clazz, Transaction());
		}
	}
}
