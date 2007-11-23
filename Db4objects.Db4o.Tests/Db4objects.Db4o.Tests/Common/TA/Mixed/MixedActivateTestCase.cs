/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Tests.Common.TA.Mixed;

namespace Db4objects.Db4o.Tests.Common.TA.Mixed
{
	public class MixedActivateTestCase : ItemTestCaseBase
	{
		private const int ITEM_DEPTH = 10;

		public static void Main(string[] args)
		{
			new MixedActivateTestCase().RunAll();
		}

		/// <exception cref="Exception"></exception>
		protected override void AssertItemValue(object obj)
		{
			AssertActivatedItemByMethod((MixedActivateTestCase.Item)obj, ITEM_DEPTH);
		}

		internal virtual void AssertActivatedItemByMethod(MixedActivateTestCase.Item item
			, int level)
		{
			for (int i = 0; i < ITEM_DEPTH; i++)
			{
				Assert.AreEqual("Item " + (ITEM_DEPTH - i), item.GetName());
				Assert.AreEqual(ITEM_DEPTH - i, item.GetValue());
				if (i < ITEM_DEPTH - 1)
				{
					Assert.IsNotNull(item.Next());
				}
				else
				{
					Assert.IsNull(item.Next());
				}
				item = item.Next();
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void AssertRetrievedItem(object obj)
		{
			MixedActivateTestCase.Item item = (MixedActivateTestCase.Item)obj;
			for (int i = 0; i < ITEM_DEPTH; i++)
			{
				AssertNullItem(item, ITEM_DEPTH - i);
				item = item.Next();
			}
		}

		private void AssertNullItem(MixedActivateTestCase.Item item, int level)
		{
			if (level % 2 == 0)
			{
				Assert.IsNull(item._name);
				Assert.IsNull(item._next);
				Assert.AreEqual(0, item._value);
			}
			else
			{
				Assert.AreEqual("Item " + level, item._name);
				Assert.AreEqual(level, item._value);
				if (level == 1)
				{
					Assert.IsNull(item._next);
				}
				else
				{
					Assert.IsNotNull(item._next);
				}
			}
		}

		/// <exception cref="Exception"></exception>
		protected override object CreateItem()
		{
			MixedActivateTestCase.TAItem item = MixedActivateTestCase.TAItem.NewTAITem(10);
			item._isRoot = true;
			return item;
		}

		public virtual void TestActivate()
		{
			MixedActivateTestCase.Item item = (MixedActivateTestCase.Item)RetrieveOnlyInstance
				(typeof(MixedActivateTestCase.TAItem));
			Assert.IsNull(item._name);
			Assert.IsNull(item._next);
			Assert.AreEqual(0, item._value);
			Db().Activate(item, 0);
			Assert.IsNull(item._name);
			Assert.IsNull(item._next);
			Assert.AreEqual(0, item._value);
			Db().Activate(item, 1);
			AssertActivatedItemByField(item, 1);
			Db().Activate(item, 5);
			AssertActivatedItemByField(item, 5);
			Db().Activate(item, 10);
			AssertActivatedItemByField(item, 10);
		}

		internal virtual void AssertActivatedItemByField(MixedActivateTestCase.Item item, 
			int level)
		{
			for (int i = 0; i < level; i++)
			{
				Assert.AreEqual("Item " + (ITEM_DEPTH - i), item._name);
				Assert.AreEqual(ITEM_DEPTH - i, item._value);
				if (i < ITEM_DEPTH - 1)
				{
					Assert.IsNotNull(item._next);
				}
				else
				{
					Assert.IsNull(item._next);
				}
				item = item._next;
			}
			if (level < ITEM_DEPTH)
			{
				Assert.IsNull(item._name);
				Assert.IsNull(item._next);
				Assert.AreEqual(0, item._value);
			}
		}

		public override object RetrieveOnlyInstance(Type clazz)
		{
			IQuery q = Db().Query();
			q.Constrain(clazz);
			q.Descend("_isRoot").Constrain(true);
			return q.Execute().Next();
		}

		public class Item
		{
			public string _name;

			public int _value;

			public MixedActivateTestCase.Item _next;

			public bool _isRoot;

			public Item()
			{
			}

			public Item(string name, int value)
			{
				_name = name;
				_value = value;
			}

			public static MixedActivateTestCase.Item NewItem(int depth)
			{
				if (depth == 0)
				{
					return null;
				}
				MixedActivateTestCase.Item header = new MixedActivateTestCase.Item("Item " + depth
					, depth);
				header._next = MixedActivateTestCase.TAItem.NewTAITem(depth - 1);
				return header;
			}

			public virtual string GetName()
			{
				return _name;
			}

			public virtual int GetValue()
			{
				return _value;
			}

			public virtual MixedActivateTestCase.Item Next()
			{
				return _next;
			}
		}

		public class TAItem : MixedActivateTestCase.Item, IActivatable
		{
			[System.NonSerialized]
			private IActivator _activator;

			public TAItem(string name, int value) : base(name, value)
			{
			}

			public static MixedActivateTestCase.TAItem NewTAITem(int depth)
			{
				if (depth == 0)
				{
					return null;
				}
				MixedActivateTestCase.TAItem header = new MixedActivateTestCase.TAItem("Item " + 
					depth, depth);
				header._next = MixedActivateTestCase.Item.NewItem(depth - 1);
				return header;
			}

			public override string GetName()
			{
				Activate();
				return _name;
			}

			public override int GetValue()
			{
				Activate();
				return _value;
			}

			public override MixedActivateTestCase.Item Next()
			{
				Activate();
				return _next;
			}

			public virtual void Activate()
			{
				if (_activator == null)
				{
					return;
				}
				_activator.Activate();
			}

			public virtual void Bind(IActivator activator)
			{
				if (null != _activator)
				{
					throw new InvalidOperationException();
				}
				_activator = activator;
			}
		}
	}
}