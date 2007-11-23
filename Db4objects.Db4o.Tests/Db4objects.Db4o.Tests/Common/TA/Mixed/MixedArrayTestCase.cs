/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Tests.Common.TA.Mixed;

namespace Db4objects.Db4o.Tests.Common.TA.Mixed
{
	public class MixedArrayTestCase : ItemTestCaseBase
	{
		public static void Main(string[] args)
		{
			new MixedArrayTestCase().RunAll();
		}

		/// <exception cref="Exception"></exception>
		protected override object CreateItem()
		{
			return new MixedArrayItem(42);
		}

		/// <exception cref="Exception"></exception>
		protected override void AssertItemValue(object obj)
		{
			MixedArrayItem item = (MixedArrayItem)obj;
			object[] objects = item.objects;
			Assert.AreEqual(42, ((TItem)objects[1]).Value());
			Assert.AreEqual(42, ((TItem)objects[3]).Value());
		}

		/// <exception cref="Exception"></exception>
		protected override void AssertRetrievedItem(object obj)
		{
			MixedArrayItem item = (MixedArrayItem)obj;
			object[] objects = item.objects;
			Assert.IsNotNull(objects);
			for (int i = 0; i < objects.Length; ++i)
			{
				Assert.IsNotNull(objects[i]);
			}
			Assert.AreEqual(LinkedList.NewList(42), objects[0]);
			Assert.AreEqual(0, ((TItem)objects[1]).value);
			Assert.AreEqual(LinkedList.NewList(42), objects[2]);
			Assert.AreEqual(0, ((TItem)objects[3]).value);
		}
	}
}