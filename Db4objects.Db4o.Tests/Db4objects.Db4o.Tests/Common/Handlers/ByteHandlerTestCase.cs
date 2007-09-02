/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class ByteHandlerTestCase : TypeHandlerTestCaseBase
	{
		public static void Main(string[] args)
		{
			new ByteHandlerTestCase().RunSolo();
		}

		public class Item
		{
			public byte _byte;

			public byte _byteWrapper;

			public Item(byte b, byte wrapper)
			{
				_byte = b;
				_byteWrapper = wrapper;
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
				{
					return true;
				}
				if (!(obj is ByteHandlerTestCase.Item))
				{
					return false;
				}
				ByteHandlerTestCase.Item other = (ByteHandlerTestCase.Item)obj;
				return (other._byte == this._byte) && this._byteWrapper.Equals(other._byteWrapper
					);
			}

			public override string ToString()
			{
				return "[" + _byte + "," + _byteWrapper + "]";
			}
		}

		private ByteHandler ByteHandler()
		{
			return new ByteHandler(Stream());
		}

		public virtual void TestReadWrite()
		{
			MockWriteContext writeContext = new MockWriteContext(Db());
			byte expected = (byte)unchecked((int)(0x61));
			ByteHandler().Write(writeContext, expected);
			MockReadContext readContext = new MockReadContext(writeContext);
			byte byteValue = (byte)ByteHandler().Read(readContext);
			Assert.AreEqual(expected, byteValue);
		}

		public virtual void TestStoreObject()
		{
			ByteHandlerTestCase.Item storedItem = new ByteHandlerTestCase.Item((byte)5, (byte
				)6);
			DoTestStoreObject(storedItem);
		}
	}
}
