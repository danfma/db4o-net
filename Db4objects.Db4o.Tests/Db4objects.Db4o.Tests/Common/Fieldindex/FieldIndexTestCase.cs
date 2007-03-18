namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class FieldIndexTestCase : Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexTestCaseBase
	{
		private static readonly int[] FOOS = new int[] { 3, 7, 9, 4 };

		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexTestCase().RunSolo();
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			base.Configure(config);
		}

		protected override void Store()
		{
			StoreItems(FOOS);
		}

		public virtual void TestTraverseValues()
		{
			Db4objects.Db4o.Ext.IStoredField field = YapField();
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor expectingVisitor = new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.ToObjectArray(FOOS));
			field.TraverseValues(expectingVisitor);
			expectingVisitor.AssertExpectations();
		}

		public virtual void TestAllThere()
		{
			for (int i = 0; i < FOOS.Length; i++)
			{
				Db4objects.Db4o.Query.IQuery q = CreateQuery(FOOS[i]);
				Db4objects.Db4o.IObjectSet objectSet = q.Execute();
				Db4oUnit.Assert.AreEqual(1, objectSet.Size());
				Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem fii = (Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem
					)objectSet.Next();
				Db4oUnit.Assert.AreEqual(FOOS[i], fii.foo);
			}
		}

		public virtual void TestAccessingBTree()
		{
			Db4objects.Db4o.Internal.Btree.BTree bTree = YapField().GetIndex(Trans());
			Db4oUnit.Assert.IsNotNull(bTree);
			ExpectKeysSearch(bTree, FOOS);
		}

		private void ExpectKeysSearch(Db4objects.Db4o.Internal.Btree.BTree btree, int[] values
			)
		{
			int lastValue = int.MinValue;
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] != lastValue)
				{
					Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor expectingVisitor = Db4objects.Db4o.Tests.Common.Btree.BTreeAssert
						.CreateExpectingVisitor(values[i], Db4objects.Db4o.Tests.Common.Foundation.IntArrays4
						.Occurences(values, values[i]));
					Db4objects.Db4o.Internal.Btree.IBTreeRange range = FieldIndexKeySearch(Trans(), btree
						, values[i]);
					Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.TraverseKeys(range, new _AnonymousInnerClass64
						(this, expectingVisitor));
					expectingVisitor.AssertExpectations();
					lastValue = values[i];
				}
			}
		}

		private sealed class _AnonymousInnerClass64 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass64(FieldIndexTestCase _enclosing, Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				 expectingVisitor)
			{
				this._enclosing = _enclosing;
				this.expectingVisitor = expectingVisitor;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Btree.FieldIndexKey fik = (Db4objects.Db4o.Internal.Btree.FieldIndexKey
					)obj;
				expectingVisitor.Visit(fik.Value());
			}

			private readonly FieldIndexTestCase _enclosing;

			private readonly Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor expectingVisitor;
		}

		private Db4objects.Db4o.Internal.Btree.FieldIndexKey FieldIndexKey(int integerPart
			, object composite)
		{
			return new Db4objects.Db4o.Internal.Btree.FieldIndexKey(integerPart, composite);
		}

		private Db4objects.Db4o.Internal.Btree.IBTreeRange FieldIndexKeySearch(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Internal.Btree.BTree btree, object key)
		{
			Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult start = btree.SearchLeaf(trans
				, FieldIndexKey(0, key), Db4objects.Db4o.Internal.Btree.SearchTarget.LOWEST);
			Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult end = btree.SearchLeaf(trans
				, FieldIndexKey(int.MaxValue, key), Db4objects.Db4o.Internal.Btree.SearchTarget.
				LOWEST);
			return start.CreateIncludingRange(end);
		}

		private Db4objects.Db4o.Internal.FieldMetadata YapField()
		{
			Db4objects.Db4o.Reflect.IReflectClass claxx = Stream().Reflector().ForObject(new 
				Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem());
			Db4objects.Db4o.Internal.ClassMetadata yc = Stream().ClassMetadataForReflectClass
				(claxx);
			Db4objects.Db4o.Internal.FieldMetadata yf = yc.FieldMetadataForName("foo");
			return yf;
		}
	}
}
