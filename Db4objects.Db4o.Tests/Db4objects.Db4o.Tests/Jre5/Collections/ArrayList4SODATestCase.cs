/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Tests.Jre5.Collections;

namespace Db4objects.Db4o.Tests.Jre5.Collections
{
	public class ArrayList4SODATestCase : TransparentActivationTestCaseBase
	{
		private static readonly Product PRODUCT_BATERY = new Product("BATE", "Batery 9v");

		private static readonly Product PRODUCT_KEYBOARD = new Product("KEYB", "Wireless keyboard"
			);

		private static readonly Product PRODUCT_CHOCOLATE = new Product("CHOC", "Chocolate"
			);

		private static readonly Product PRODUCT_MOUSE = new Product("MOUS", "Wireless Mouse"
			);

		private static readonly Product PRODUCT_NOTE = new Product("NOTE", "Core Quad notebook with 1 Tb memory"
			);

		private static readonly Product[] products = new Product[] { PRODUCT_BATERY, PRODUCT_CHOCOLATE
			, PRODUCT_KEYBOARD, PRODUCT_MOUSE, PRODUCT_NOTE };

		public virtual void TestSODAAutodescend()
		{
			for (int i = 0; i < products.Length; i++)
			{
				AssertCount(i);
			}
		}

		private void AssertCount(int index)
		{
			IQuery query = Db().Query();
			query.Constrain(typeof(Order));
			query.Descend("_items").Descend("_product").Descend("_code").Constrain(products[index
				].Code());
			IObjectSet results = query.Execute();
			Assert.AreEqual(products.Length - index, results.Size());
			while (results.HasNext())
			{
				Order order = (Order)results.Next();
				for (int j = 0; j < order.Size(); j++)
				{
					Assert.AreEqual(products[j].Code(), order.Item(j).Product().Code());
				}
			}
		}

		protected override void Store()
		{
			for (int i = 0; i < products.Length; i++)
			{
				Store(CreateOrder(i));
			}
		}

		private Order CreateOrder(int itemIndex)
		{
			Order o = new Order();
			for (int i = 0; i <= itemIndex; i++)
			{
				o.AddItem(new OrderItem(products[i], i));
			}
			return o;
		}

		protected virtual IDb4oClientServerFixture ClientServerFixture()
		{
			return (IDb4oClientServerFixture)Fixture();
		}

		protected virtual IExtObjectContainer OpenNewClient()
		{
			return ClientServerFixture().OpenNewClient();
		}
	}
}