namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public abstract class AbstractLateQueryResult : Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult
	{
		protected System.Collections.IEnumerable _iterable;

		public AbstractLateQueryResult(Db4objects.Db4o.Internal.Transaction transaction) : 
			base(transaction)
		{
		}

		public override Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult SupportSize
			()
		{
			return ToIdTree();
		}

		public override Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult SupportSort
			()
		{
			return ToIdList();
		}

		public override Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult SupportElementAccess
			()
		{
			return ToIdList();
		}

		protected override int KnownSize()
		{
			return 0;
		}

		public override Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			if (_iterable == null)
			{
				throw new System.InvalidOperationException();
			}
			return new Db4objects.Db4o.Foundation.IntIterator4Adaptor(_iterable);
		}

		public override Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult ToIdList
			()
		{
			return ToIdTree().ToIdList();
		}

		public virtual bool SkipClass(Db4objects.Db4o.Internal.ClassMetadata yapClass)
		{
			if (yapClass.GetName() == null)
			{
				return true;
			}
			Db4objects.Db4o.Reflect.IReflectClass claxx = yapClass.ClassReflector();
			if (Stream().i_handlers.ICLASS_INTERNAL.IsAssignableFrom(claxx))
			{
				return true;
			}
			return false;
		}

		protected virtual System.Collections.IEnumerable ClassIndexesIterable(Db4objects.Db4o.Internal.ClassMetadataIterator
			 classCollectionIterator)
		{
			return new _AnonymousInnerClass61(this, classCollectionIterator);
		}

		private sealed class _AnonymousInnerClass61 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass61(AbstractLateQueryResult _enclosing, Db4objects.Db4o.Internal.ClassMetadataIterator
				 classCollectionIterator)
			{
				this._enclosing = _enclosing;
				this.classCollectionIterator = classCollectionIterator;
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return new Db4objects.Db4o.Foundation.CompositeIterator4(new _AnonymousInnerClass64
					(this, classCollectionIterator));
			}

			private sealed class _AnonymousInnerClass64 : Db4objects.Db4o.Foundation.MappingIterator
			{
				public _AnonymousInnerClass64(_AnonymousInnerClass61 _enclosing, Db4objects.Db4o.Internal.ClassMetadataIterator
					 baseArg1) : base(baseArg1)
				{
					this._enclosing = _enclosing;
				}

				protected override object Map(object current)
				{
					Db4objects.Db4o.Internal.ClassMetadata yapClass = (Db4objects.Db4o.Internal.ClassMetadata
						)current;
					if (this._enclosing._enclosing.SkipClass(yapClass))
					{
						return Db4objects.Db4o.Foundation.MappingIterator.SKIP;
					}
					return this._enclosing._enclosing.ClassIndexIterator(yapClass);
				}

				private readonly _AnonymousInnerClass61 _enclosing;
			}

			private readonly AbstractLateQueryResult _enclosing;

			private readonly Db4objects.Db4o.Internal.ClassMetadataIterator classCollectionIterator;
		}

		protected virtual System.Collections.IEnumerable ClassIndexIterable(Db4objects.Db4o.Internal.ClassMetadata
			 clazz)
		{
			return new _AnonymousInnerClass79(this, clazz);
		}

		private sealed class _AnonymousInnerClass79 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass79(AbstractLateQueryResult _enclosing, Db4objects.Db4o.Internal.ClassMetadata
				 clazz)
			{
				this._enclosing = _enclosing;
				this.clazz = clazz;
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return this._enclosing.ClassIndexIterator(clazz);
			}

			private readonly AbstractLateQueryResult _enclosing;

			private readonly Db4objects.Db4o.Internal.ClassMetadata clazz;
		}

		public virtual System.Collections.IEnumerator ClassIndexIterator(Db4objects.Db4o.Internal.ClassMetadata
			 clazz)
		{
			return Db4objects.Db4o.Internal.Classindex.BTreeClassIndexStrategy.Iterate(clazz, 
				Transaction());
		}
	}
}