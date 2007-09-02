/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Fieldindex;

namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class SecondLevelIndexTestCase : AbstractDb4oTestCase, IDiagnosticListener
	{
		public static void Main(string[] arguments)
		{
			new SecondLevelIndexTestCase().RunSolo();
		}

		public class ItemPair
		{
			public SecondLevelIndexTestCase.Item item1;

			public SecondLevelIndexTestCase.Item item2;

			public ItemPair()
			{
			}

			public ItemPair(SecondLevelIndexTestCase.Item item_, SecondLevelIndexTestCase.Item
				 item2_)
			{
				item1 = item_;
				item2 = item2_;
			}
		}

		public class Item
		{
			public string name;

			public Item()
			{
			}

			public Item(string name_)
			{
				name = name_;
			}
		}

		protected override void Configure(IConfiguration config)
		{
			config.Diagnostic().AddListener(this);
			config.ObjectClass(typeof(SecondLevelIndexTestCase.Item)).ObjectField("name").Indexed
				(true);
			config.ObjectClass(typeof(SecondLevelIndexTestCase.ItemPair)).ObjectField("item1"
				).Indexed(true);
			config.ObjectClass(typeof(SecondLevelIndexTestCase.ItemPair)).ObjectField("item2"
				).Indexed(true);
			base.Configure(config);
		}

		protected override void Db4oTearDownBeforeClean()
		{
			Db4oFactory.Configure().Diagnostic().RemoveAllListeners();
		}

		public virtual void Test()
		{
			SecondLevelIndexTestCase.Item itemOne = new SecondLevelIndexTestCase.Item("one");
			SecondLevelIndexTestCase.Item itemTwo = new SecondLevelIndexTestCase.Item("two");
			Store(new SecondLevelIndexTestCase.ItemPair(itemOne, itemTwo));
			IQuery query = NewQuery(typeof(SecondLevelIndexTestCase.ItemPair));
			query.Descend("item2").Descend("name").Constrain("two");
			IObjectSet objectSet = query.Execute();
			Assert.AreEqual(((SecondLevelIndexTestCase.ItemPair)objectSet.Next()).item1, itemOne
				);
		}

		public virtual void OnDiagnostic(IDiagnostic d)
		{
			Assert.IsFalse(d is LoadedFromClassIndex);
		}
	}
}
