/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Handlers;
using Db4objects.Db4o.Tests.Util;
using Sharpen;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class ShortHandlerUpdateTestCase : HandlerUpdateTestCaseBase
	{
		public class Item
		{
			public short _typedPrimitive;

			public short _typedWrapper;

			public object _untyped;
		}

		public class ItemArrays
		{
			public short[] _typedPrimitiveArray;

			public short[] _typedWrapperArray;

			public object[] _untypedObjectArray;

			public object _primitiveArrayInObject;

			public object _wrapperArrayInObject;
		}

		private static readonly short[] data = new short[] { short.MinValue, short.MinValue
			 + 1, -5, -1, 0, 1, 5, short.MaxValue - 1, short.MaxValue };

		public static void Main(string[] args)
		{
			new TestRunner(typeof(ShortHandlerUpdateTestCase)).Run();
		}

		protected override void AssertArrays(object obj)
		{
			ShortHandlerUpdateTestCase.ItemArrays itemArrays = (ShortHandlerUpdateTestCase.ItemArrays
				)obj;
			AssertPrimitiveArray(itemArrays._typedPrimitiveArray);
			if (_db4oHeaderVersion == VersionServices.HEADER_30_40)
			{
				AssertWrapperArray((short[])itemArrays._primitiveArrayInObject);
			}
			else
			{
				AssertPrimitiveArray((short[])itemArrays._primitiveArrayInObject);
			}
			AssertWrapperArray(itemArrays._typedWrapperArray);
			AssertWrapperArray((short[])itemArrays._wrapperArrayInObject);
		}

		private void AssertPrimitiveArray(short[] primitiveArray)
		{
			for (int i = 0; i < data.Length; i++)
			{
				AssertAreEqual(data[i], primitiveArray[i]);
			}
		}

		private void AssertWrapperArray(short[] wrapperArray)
		{
			for (int i = 0; i < data.Length; i++)
			{
				AssertAreEqual(data[i], wrapperArray[i]);
			}
		}

		protected override void AssertValues(object[] values)
		{
			for (int i = 0; i < data.Length; i++)
			{
				ShortHandlerUpdateTestCase.Item item = (ShortHandlerUpdateTestCase.Item)values[i];
				AssertAreEqual(data[i], item._typedPrimitive);
				AssertAreEqual(data[i], item._typedWrapper);
				AssertAreEqual(data[i], item._untyped);
			}
			ShortHandlerUpdateTestCase.Item nullItem = (ShortHandlerUpdateTestCase.Item)values
				[values.Length - 1];
			AssertAreEqual((short)0, nullItem._typedPrimitive);
			Assert.IsNull(nullItem._typedWrapper);
			Assert.IsNull(nullItem._untyped);
		}

		private void AssertAreEqual(short expected, short actual)
		{
			if (expected == short.MaxValue && _handlerVersion == 0)
			{
				expected = 0;
			}
			Assert.AreEqual(expected, actual);
		}

		private void AssertAreEqual(object expected, object actual)
		{
			if (short.MaxValue.Equals(expected) && _handlerVersion == 0)
			{
				expected = null;
			}
			Assert.AreEqual(expected, actual);
		}

		protected override object CreateArrays()
		{
			ShortHandlerUpdateTestCase.ItemArrays itemArrays = new ShortHandlerUpdateTestCase.ItemArrays
				();
			itemArrays._typedPrimitiveArray = new short[data.Length];
			System.Array.Copy(data, 0, itemArrays._typedPrimitiveArray, 0, data.Length);
			short[] dataWrapper = new short[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				dataWrapper[i] = data[i];
			}
			itemArrays._typedWrapperArray = new short[data.Length + 1];
			System.Array.Copy(dataWrapper, 0, itemArrays._typedWrapperArray, 0, dataWrapper.Length
				);
			short[] primitiveArray = new short[data.Length];
			System.Array.Copy(data, 0, primitiveArray, 0, data.Length);
			itemArrays._primitiveArrayInObject = primitiveArray;
			short[] wrapperArray = new short[data.Length + 1];
			System.Array.Copy(dataWrapper, 0, wrapperArray, 0, dataWrapper.Length);
			itemArrays._wrapperArrayInObject = wrapperArray;
			return itemArrays;
		}

		protected override object[] CreateValues()
		{
			ShortHandlerUpdateTestCase.Item[] values = new ShortHandlerUpdateTestCase.Item[data
				.Length + 1];
			for (int i = 0; i < data.Length; i++)
			{
				ShortHandlerUpdateTestCase.Item item = new ShortHandlerUpdateTestCase.Item();
				item._typedPrimitive = data[i];
				item._typedWrapper = data[i];
				item._untyped = data[i];
				values[i] = item;
			}
			values[values.Length - 1] = new ShortHandlerUpdateTestCase.Item();
			return values;
		}

		protected override string TypeName()
		{
			return "short";
		}
	}
}