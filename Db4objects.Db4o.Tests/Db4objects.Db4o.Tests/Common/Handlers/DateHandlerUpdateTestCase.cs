/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Tests.Common.Handlers;
using Sharpen;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class DateHandlerUpdateTestCase : HandlerUpdateTestCaseBase
	{
		public class Item
		{
			public DateTime _date;

			public object _untyped;
		}

		public class ItemArrays
		{
			public DateTime[] _dateArray;

			public object[] _untypedObjectArray;

			public object _arrayInObject;
		}

		[System.ObsoleteAttribute(@"so we don't have warnings because of the deprecated Date ctor"
			)]
		private static readonly DateTime[] data = new DateTime[] { new DateTime(DatePlatform
			.MIN_DATE), new DateTime(DatePlatform.MIN_DATE + 1), new DateTime(1975, 7, 21, 5
			, 32, 12), new DateTime(DatePlatform.MAX_DATE - 1), new DateTime(DatePlatform.MAX_DATE
			) };

		public static void Main(string[] args)
		{
			new TestRunner(typeof(DateHandlerUpdateTestCase)).Run();
		}

		protected override void AssertArrays(object obj)
		{
			DateHandlerUpdateTestCase.ItemArrays itemArrays = (DateHandlerUpdateTestCase.ItemArrays
				)obj;
			DateTime[] dateArray = (DateTime[])itemArrays._arrayInObject;
			for (int i = 0; i < data.Length; i++)
			{
				AssertAreEqual(data[i], itemArrays._dateArray[i]);
				AssertAreEqual(data[i], (DateTime)itemArrays._untypedObjectArray[i]);
				AssertAreEqual(data[i], dateArray[i]);
			}
			Assert.IsNull(itemArrays._untypedObjectArray[data.Length]);
			Assert.AreEqual(EmptyValue(), dateArray[data.Length]);
		}

		protected override void AssertValues(object[] values)
		{
			for (int i = 0; i < data.Length; i++)
			{
				DateHandlerUpdateTestCase.Item item = (DateHandlerUpdateTestCase.Item)values[i];
				AssertAreEqual(data[i], item._date);
				AssertAreEqual(data[i], (DateTime)item._untyped);
			}
			DateHandlerUpdateTestCase.Item emptyItem = (DateHandlerUpdateTestCase.Item)values
				[values.Length - 1];
			Assert.AreEqual(EmptyValue(), emptyItem._date);
			Assert.IsNull(emptyItem._untyped);
		}

		private object EmptyValue()
		{
			return new DateHandler(null).PrimitiveNull();
		}

		private void AssertAreEqual(DateTime expected, DateTime actual)
		{
			if (expected.Equals(new DateTime(DatePlatform.MAX_DATE)) && _handlerVersion == 0)
			{
				expected = MarshallingConstants0.NULL_DATE;
			}
			Assert.AreEqual(expected, actual);
		}

		protected override object CreateArrays()
		{
			DateHandlerUpdateTestCase.ItemArrays itemArrays = new DateHandlerUpdateTestCase.ItemArrays
				();
			itemArrays._dateArray = new DateTime[data.Length + 1];
			System.Array.Copy(data, 0, itemArrays._dateArray, 0, data.Length);
			itemArrays._untypedObjectArray = new object[data.Length + 1];
			System.Array.Copy(data, 0, itemArrays._untypedObjectArray, 0, data.Length);
			DateTime[] dateArray = new DateTime[data.Length + 1];
			System.Array.Copy(data, 0, dateArray, 0, data.Length);
			itemArrays._arrayInObject = dateArray;
			return itemArrays;
		}

		protected override object[] CreateValues()
		{
			DateHandlerUpdateTestCase.Item[] values = new DateHandlerUpdateTestCase.Item[data
				.Length + 1];
			for (int i = 0; i < data.Length; i++)
			{
				DateHandlerUpdateTestCase.Item item = new DateHandlerUpdateTestCase.Item();
				item._date = data[i];
				item._untyped = data[i];
				values[i] = item;
			}
			values[values.Length - 1] = new DateHandlerUpdateTestCase.Item();
			return values;
		}

		protected override string TypeName()
		{
			return "date";
		}
	}
}
