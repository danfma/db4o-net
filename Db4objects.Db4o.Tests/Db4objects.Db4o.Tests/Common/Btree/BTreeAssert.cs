namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeAssert
	{
		public static Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor CreateExpectingVisitor
			(int value, int count)
		{
			int[] values = new int[count];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = value;
			}
			return new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4
				.ToObjectArray(values));
		}

		public static Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor CreateExpectingVisitor
			(int[] keys)
		{
			return new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4
				.ToObjectArray(keys));
		}

		private static Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor CreateSortedExpectingVisitor
			(int[] keys)
		{
			return new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4
				.ToObjectArray(keys), true, false);
		}

		public static void TraverseKeys(Db4objects.Db4o.Internal.Btree.IBTreeRange result
			, Db4objects.Db4o.Foundation.IVisitor4 visitor)
		{
			System.Collections.IEnumerator i = result.Keys();
			while (i.MoveNext())
			{
				visitor.Visit(i.Current);
			}
		}

		public static void AssertKeys(Db4objects.Db4o.Internal.Transaction transaction, Db4objects.Db4o.Internal.Btree.BTree
			 btree, int[] keys)
		{
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor visitor = CreateExpectingVisitor
				(keys);
			btree.TraverseKeys(transaction, visitor);
			visitor.AssertExpectations();
		}

		public static void AssertEmpty(Db4objects.Db4o.Internal.Transaction transaction, 
			Db4objects.Db4o.Internal.Btree.BTree tree)
		{
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor visitor = new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				(new object[0]);
			tree.TraverseKeys(transaction, visitor);
			visitor.AssertExpectations();
			Db4oUnit.Assert.AreEqual(0, tree.Size(transaction));
		}

		public static void DumpKeys(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Btree.BTree
			 tree)
		{
			tree.TraverseKeys(trans, new _AnonymousInnerClass50());
		}

		private sealed class _AnonymousInnerClass50 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass50()
			{
			}

			public void Visit(object obj)
			{
				Sharpen.Runtime.Out.WriteLine(obj);
			}
		}

		public static Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor CreateExpectingVisitor
			(int expectedID)
		{
			return CreateExpectingVisitor(expectedID, 1);
		}

		public static int FillSize(Db4objects.Db4o.Internal.Btree.BTree btree)
		{
			return btree.NodeSize() + 1;
		}

		public static int[] NewBTreeNodeSizedArray(Db4objects.Db4o.Internal.Btree.BTree btree
			, int value)
		{
			return Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.Fill(new int[FillSize(btree
				)], value);
		}

		public static void AssertRange(int[] expectedKeys, Db4objects.Db4o.Internal.Btree.IBTreeRange
			 range)
		{
			Db4oUnit.Assert.IsNotNull(range);
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor visitor = CreateSortedExpectingVisitor
				(expectedKeys);
			TraverseKeys(range, visitor);
			visitor.AssertExpectations();
		}

		public static Db4objects.Db4o.Internal.Btree.BTree CreateIntKeyBTree(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, int id, int nodeSize)
		{
			return new Db4objects.Db4o.Internal.Btree.BTree(stream.SystemTransaction(), id, new 
				Db4objects.Db4o.Internal.Handlers.IntHandler(stream), nodeSize, stream.ConfigImpl
				().BTreeCacheHeight());
		}

		public static Db4objects.Db4o.Internal.Btree.BTree CreateIntKeyBTree(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, int id, int treeCacheHeight, int nodeSize)
		{
			return new Db4objects.Db4o.Internal.Btree.BTree(stream.SystemTransaction(), id, new 
				Db4objects.Db4o.Internal.Handlers.IntHandler(stream), nodeSize, treeCacheHeight);
		}

		public static void AssertSingleElement(Db4objects.Db4o.Internal.Transaction trans
			, Db4objects.Db4o.Internal.Btree.BTree btree, object element)
		{
			Db4oUnit.Assert.AreEqual(1, btree.Size(trans));
			Db4objects.Db4o.Internal.Btree.IBTreeRange result = btree.Search(trans, element);
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor expectingVisitor = new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				(new object[] { element });
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.TraverseKeys(result, expectingVisitor
				);
			expectingVisitor.AssertExpectations();
			expectingVisitor = new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor(new object
				[] { element });
			btree.TraverseKeys(trans, expectingVisitor);
			expectingVisitor.AssertExpectations();
		}
	}
}
