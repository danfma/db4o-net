/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Handlers;
using Sharpen;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class IntHandlerUpdateTestCase : HandlerUpdateTestCaseBase
	{
		private static readonly int[] data = new int[] { int.MinValue, int.MinValue + 1, 
			-5, -1, 0, 1, 5, int.MaxValue - 1, int.MaxValue };

		public static void Main(string[] args)
		{
			new TestRunner(typeof(IntHandlerUpdateTestCase)).Run();
		}

		protected override string TypeName()
		{
			return "int";
		}

		public class Item
		{
			public int _typedPrimitive;

			public int _typedWrapper;

			public object _untyped;
		}

		public class ItemArrays
		{
			public int[] _typedPrimitiveArray;

			public int[] _typedWrapperArray;

			public object[] _untypedObjectArray;

			public object _primitiveArrayInObject;

			public object _wrapperArrayInObject;
		}

		protected override object[] CreateValues()
		{
			IntHandlerUpdateTestCase.Item[] values = new IntHandlerUpdateTestCase.Item[data.Length
				 + 1];
			for (int i = 0; i < data.Length; i++)
			{
				IntHandlerUpdateTestCase.Item item = new IntHandlerUpdateTestCase.Item();
				values[i] = item;
				item._typedPrimitive = data[i];
				item._typedWrapper = data[i];
				item._untyped = data[i];
			}
			values[values.Length - 1] = new IntHandlerUpdateTestCase.Item();
			return values;
		}

		protected override object CreateArrays()
		{
			IntHandlerUpdateTestCase.ItemArrays item = new IntHandlerUpdateTestCase.ItemArrays
				();
			CreateTypedPrimitiveArray(item);
			CreateTypedWrapperArray(item);
			CreatePrimitiveArrayInObject(item);
			CreateWrapperArrayInObject(item);
			return item;
		}

		private void CreateTypedPrimitiveArray(IntHandlerUpdateTestCase.ItemArrays item)
		{
			item._typedPrimitiveArray = new int[data.Length];
			System.Array.Copy(data, 0, item._typedPrimitiveArray, 0, data.Length);
		}

		private void CreateTypedWrapperArray(IntHandlerUpdateTestCase.ItemArrays item)
		{
			item._typedWrapperArray = new int[data.Length + 1];
			for (int i = 0; i < data.Length; i++)
			{
				item._typedWrapperArray[i] = data[i];
			}
		}

		private void CreatePrimitiveArrayInObject(IntHandlerUpdateTestCase.ItemArrays item
			)
		{
			int[] arr = new int[data.Length];
			System.Array.Copy(data, 0, arr, 0, data.Length);
			item._primitiveArrayInObject = arr;
		}

		private void CreateWrapperArrayInObject(IntHandlerUpdateTestCase.ItemArrays item)
		{
			int[] arr = new int[data.Length + 1];
			for (int i = 0; i < data.Length; i++)
			{
				arr[i] = data[i];
			}
			item._wrapperArrayInObject = arr;
		}

		protected override void AssertValues(object[] values)
		{
			for (int i = 0; i < data.Length; i++)
			{
				IntHandlerUpdateTestCase.Item item = (IntHandlerUpdateTestCase.Item)values[i];
				Assert.AreEqual(item._typedPrimitive, data[i]);
				Assert.AreEqual(item._typedWrapper, data[i]);
				Assert.AreEqual(item._untyped, data[i]);
			}
			IntHandlerUpdateTestCase.Item nullItem = (IntHandlerUpdateTestCase.Item)values[values
				.Length - 1];
			Assert.AreEqual(0, nullItem._typedPrimitive);
			Assert.IsNull(nullItem._typedWrapper);
			Assert.IsNull(nullItem._untyped);
		}

		protected override void AssertArrays(object obj)
		{
			IntHandlerUpdateTestCase.ItemArrays item = (IntHandlerUpdateTestCase.ItemArrays)obj;
			AssertTypedPrimitiveArray(item);
			AssertTypedWrapperArray(item);
			AssertPrimitiveArrayInObject(item);
			AssertWrapperArrayInObject(item);
		}

		private void AssertTypedPrimitiveArray(IntHandlerUpdateTestCase.ItemArrays item)
		{
			AssertData(item._typedPrimitiveArray);
		}

		private void AssertTypedWrapperArray(IntHandlerUpdateTestCase.ItemArrays item)
		{
			AssertWrapperData(item._typedWrapperArray);
		}

		private void AssertPrimitiveArrayInObject(IntHandlerUpdateTestCase.ItemArrays item
			)
		{
			AssertData((int[])item._primitiveArrayInObject);
		}

		private void AssertWrapperArrayInObject(IntHandlerUpdateTestCase.ItemArrays item)
		{
			AssertWrapperData((int[])item._wrapperArrayInObject);
		}

		private void AssertData(int[] values)
		{
			for (int i = 0; i < data.Length; i++)
			{
				Assert.AreEqual(data[i], values[i]);
			}
		}

		private void AssertWrapperData(int[] values)
		{
			for (int i = 0; i < data.Length; i++)
			{
				Assert.AreEqual(data[i], values[i]);
			}
		}
	}
}