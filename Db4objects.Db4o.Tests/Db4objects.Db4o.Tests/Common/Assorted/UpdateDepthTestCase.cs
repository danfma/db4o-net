/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class UpdateDepthTestCase : AbstractDb4oTestCase
	{
		public sealed class Item
		{
			public string name;

			public UpdateDepthTestCase.Item child;

			public UpdateDepthTestCase.Item[] childArray;

			public ArrayList childVector;

			public Item()
			{
			}

			public Item(string name)
			{
				this.name = name;
			}

			public Item(string name, UpdateDepthTestCase.Item child) : this(name)
			{
				this.child = child;
			}

			public Item(string name, UpdateDepthTestCase.Item child, UpdateDepthTestCase.Item[]
				 childArray) : this(name, child)
			{
				this.childArray = childArray;
				this.childVector = new ArrayList();
				for (int i = 0; i < childArray.Length; ++i)
				{
					childVector.Add(childArray[i]);
				}
			}
		}

		public sealed class RootItem
		{
			public UpdateDepthTestCase.Item root;

			public RootItem()
			{
			}

			public RootItem(UpdateDepthTestCase.Item root)
			{
				this.root = root;
			}
		}

		protected override void Store()
		{
			Store(new UpdateDepthTestCase.RootItem(NewGraph()));
		}

		protected override void Configure(IConfiguration config)
		{
			IObjectClass itemClass = config.ObjectClass(typeof(UpdateDepthTestCase.Item));
			itemClass.UpdateDepth(1);
			itemClass.MinimumActivationDepth(3);
		}

		public virtual void TestDepth0()
		{
			Db().Set(PokeName(QueryRoot()), 0);
			Expect(NewGraph());
		}

		public virtual void TestDepth1()
		{
			UpdateDepthTestCase.Item item = PokeChild(PokeName(QueryRoot()));
			Db().Set(item, 1);
			Expect(PokeName(NewGraph()));
		}

		public virtual void TestDepth2()
		{
			UpdateDepthTestCase.Item root = PokeChild(PokeName(QueryRoot()));
			PokeChild(root.child);
			Db().Set(root, 2);
			Expect(PokeChild(PokeName(NewGraph())));
		}

		public virtual void TestDepth3()
		{
			UpdateDepthTestCase.Item item = PokeChild(PokeName(QueryRoot()));
			PokeChild(item.child);
			Db().Set(item, 3);
			Expect(item);
		}

		private UpdateDepthTestCase.Item NewGraph()
		{
			return new UpdateDepthTestCase.Item("Level 1", new UpdateDepthTestCase.Item("Level 2"
				, new UpdateDepthTestCase.Item("Level 3"), new UpdateDepthTestCase.Item[] { new 
				UpdateDepthTestCase.Item("Array Level 3") }), new UpdateDepthTestCase.Item[] { new 
				UpdateDepthTestCase.Item("Array Level 2") });
		}

		private UpdateDepthTestCase.Item PokeChild(UpdateDepthTestCase.Item item)
		{
			PokeName(item.child);
			if (item.childArray != null)
			{
				PokeName(item.childArray[0]);
				PokeName((UpdateDepthTestCase.Item)item.childVector[0]);
			}
			return item;
		}

		private UpdateDepthTestCase.Item PokeName(UpdateDepthTestCase.Item item)
		{
			item.name = item.name + "*";
			return item;
		}

		private void Expect(UpdateDepthTestCase.Item expected)
		{
			Reopen();
			AssertEquals(expected, QueryRoot());
		}

		private void AssertEquals(UpdateDepthTestCase.Item expected, UpdateDepthTestCase.Item
			 actual)
		{
			if (expected == null)
			{
				Assert.IsNull(actual);
				return;
			}
			Assert.IsNotNull(actual);
			Assert.AreEqual(expected.name, actual.name);
			AssertEquals(expected.child, actual.child);
			AssertEquals(expected.childArray, actual.childArray);
			AssertCollection(expected.childVector, actual.childVector);
		}

		private void AssertCollection(ArrayList expected, ArrayList actual)
		{
			if (expected == null)
			{
				Assert.IsNull(actual);
				return;
			}
			Assert.IsNotNull(actual);
			Assert.AreEqual(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; ++i)
			{
				AssertEquals((UpdateDepthTestCase.Item)expected[i], (UpdateDepthTestCase.Item)actual
					[i]);
			}
		}

		private void AssertEquals(UpdateDepthTestCase.Item[] expected, UpdateDepthTestCase.Item[]
			 actual)
		{
			if (expected == null)
			{
				Assert.IsNull(actual);
				return;
			}
			Assert.IsNotNull(actual);
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; ++i)
			{
				AssertEquals(expected[i], actual[i]);
			}
		}

		private UpdateDepthTestCase.Item QueryRoot()
		{
			return ((UpdateDepthTestCase.RootItem)NewQuery(typeof(UpdateDepthTestCase.RootItem)
				).Execute().Next()).root;
		}
	}
}