/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class ArrayHandlerTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ArrayHandlerTestCase().RunSolo();
		}

		public class IntArrayHolder
		{
			public int[] _ints;

			public IntArrayHolder(int[] ints)
			{
				_ints = ints;
			}
		}

		public class StringArrayHolder
		{
			public string[] _strings;

			public StringArrayHolder(string[] strings)
			{
				_strings = strings;
			}
		}

		private ArrayHandler IntArrayHandler()
		{
			return ArrayHandler(typeof(int), true);
		}

		private ArrayHandler StringArrayHandler()
		{
			return ArrayHandler(typeof(string), false);
		}

		private ArrayHandler ArrayHandler(Type clazz, bool isPrimitive)
		{
			ClassMetadata classMetadata = Stream().ProduceClassMetadata(Reflector().ForClass(
				clazz));
			return new ArrayHandler(Stream(), classMetadata.TypeHandler(), isPrimitive);
		}

		public virtual void TestIntArrayReadWrite()
		{
			MockWriteContext writeContext = new MockWriteContext(Db());
			int[] expected = new int[] { 7, 8, 9 };
			IntArrayHandler().Write(writeContext, expected);
			MockReadContext readContext = new MockReadContext(writeContext);
			int[] actual = (int[])IntArrayHandler().Read(readContext);
			ArrayAssert.AreEqual(expected, actual);
		}

		public virtual void TestIntArrayStoreObject()
		{
			ArrayHandlerTestCase.IntArrayHolder expectedItem = new ArrayHandlerTestCase.IntArrayHolder
				(new int[] { 1, 2, 3 });
			Db().Set(expectedItem);
			Db().Purge(expectedItem);
			ArrayHandlerTestCase.IntArrayHolder readItem = (ArrayHandlerTestCase.IntArrayHolder
				)RetrieveOnlyInstance(typeof(ArrayHandlerTestCase.IntArrayHolder));
			Assert.AreNotSame(expectedItem, readItem);
			ArrayAssert.AreEqual(expectedItem._ints, readItem._ints);
		}

		public virtual void TestStringArrayReadWrite()
		{
			MockWriteContext writeContext = new MockWriteContext(Db());
			string[] expected = new string[] { "one", "two", "three" };
			StringArrayHandler().Write(writeContext, expected);
			MockReadContext readContext = new MockReadContext(writeContext);
			string[] actual = (string[])StringArrayHandler().Read(readContext);
			ArrayAssert.AreEqual(expected, actual);
		}

		public virtual void TestStringArrayStoreObject()
		{
			ArrayHandlerTestCase.StringArrayHolder expectedItem = new ArrayHandlerTestCase.StringArrayHolder
				(new string[] { "one", "two", "three" });
			Db().Set(expectedItem);
			Db().Purge(expectedItem);
			ArrayHandlerTestCase.StringArrayHolder readItem = (ArrayHandlerTestCase.StringArrayHolder
				)RetrieveOnlyInstance(typeof(ArrayHandlerTestCase.StringArrayHolder));
			Assert.AreNotSame(expectedItem, readItem);
			ArrayAssert.AreEqual(expectedItem._strings, readItem._strings);
		}
	}
}
