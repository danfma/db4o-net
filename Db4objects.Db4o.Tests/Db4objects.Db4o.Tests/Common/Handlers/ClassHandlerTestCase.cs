/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class ClassHandlerTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ClassHandlerTestCase().RunSolo();
		}

		public class Item
		{
			public string _name;

			public ClassHandlerTestCase.Item _child;

			public Item(string name, ClassHandlerTestCase.Item child)
			{
				_name = name;
				_child = child;
			}
		}

		private ClassMetadata ClassHandler()
		{
			IReflectClass claxx = Reflector().ForClass(typeof(ClassHandlerTestCase.Item));
			return Stream().Container().ProduceClassMetadata(claxx);
		}

		public virtual void TestReadWrite()
		{
			MockWriteContext writeContext = new MockWriteContext(Db());
			ClassHandlerTestCase.Item expectedItem = new ClassHandlerTestCase.Item("parent", 
				new ClassHandlerTestCase.Item("child", null));
			ClassHandler().Write(writeContext, expectedItem);
			MockReadContext readContext = new MockReadContext(writeContext);
			ClassHandlerTestCase.Item readItem = (ClassHandlerTestCase.Item)ClassHandler().Read
				(readContext);
			AssertAreEqual(expectedItem, readItem);
		}

		public virtual void TestStoreObject()
		{
			ClassHandlerTestCase.Item expectedItem = new ClassHandlerTestCase.Item("parent", 
				new ClassHandlerTestCase.Item("child", null));
			Db().Set(expectedItem);
			Db().Purge(expectedItem);
			IQuery q = Db().Query();
			q.Constrain(typeof(ClassHandlerTestCase.Item));
			q.Descend("_name").Constrain("parent");
			IObjectSet objectSet = q.Execute();
			ClassHandlerTestCase.Item readItem = (ClassHandlerTestCase.Item)objectSet.Next();
			Assert.AreNotSame(expectedItem, readItem);
			AssertAreEqual(expectedItem, readItem);
		}

		private void AssertAreEqual(ClassHandlerTestCase.Item expectedItem, ClassHandlerTestCase.Item
			 readItem)
		{
			Assert.AreEqual(expectedItem._name, readItem._name);
			Assert.AreEqual(expectedItem._child._name, readItem._child._name);
		}
	}
}
