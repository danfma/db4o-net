/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Defragment;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class COR775TestCase : ITestLifeCycle
	{
		private static readonly string Original = Path.GetTempFileName();

		private static readonly string Defgared = Original + ".bk";

		internal IConfiguration db4oConfig;

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			Cleanup();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
			Cleanup();
		}

		private void Cleanup()
		{
			File4.Delete(Original);
			File4.Delete(Defgared);
		}

		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(COR775TestCase)).Run();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestCOR775()
		{
			Prepare();
			VerifyDB();
			DefragmentConfig config = new DefragmentConfig(Original, Defgared);
			config.ForceBackupDelete(true);
			//config.storedClassFilter(new AvailableClassFilter());
			config.Db4oConfig(GetConfiguration());
			Db4objects.Db4o.Defragment.Defragment.Defrag(config);
			VerifyDB();
		}

		private void Prepare()
		{
			Sharpen.IO.File file = new Sharpen.IO.File(Original);
			if (file.Exists())
			{
				file.Delete();
			}
			IObjectContainer testDB = OpenDB();
			COR775TestCase.Item item = new COR775TestCase.Item("richard", 100);
			testDB.Store(item);
			testDB.Close();
		}

		private void VerifyDB()
		{
			IObjectContainer testDB = OpenDB();
			IObjectSet result = testDB.QueryByExample(typeof(COR775TestCase.Item));
			if (result.HasNext())
			{
				COR775TestCase.Item retrievedItem = (COR775TestCase.Item)result.Next();
				Assert.AreEqual("richard", retrievedItem.name);
				Assert.AreEqual(100, retrievedItem.value);
			}
			else
			{
				Assert.Fail("Cannot retrieve the expected object.");
			}
			testDB.Close();
		}

		private IObjectContainer OpenDB()
		{
			IConfiguration db4oConfig = GetConfiguration();
			IObjectContainer testDB = Db4oFactory.OpenFile(db4oConfig, Original);
			return testDB;
		}

		private IConfiguration GetConfiguration()
		{
			if (db4oConfig == null)
			{
				db4oConfig = Db4oFactory.NewConfiguration();
				db4oConfig.ActivationDepth(int.MaxValue);
				db4oConfig.CallConstructors(true);
				IStorage storage = new COR775TestCase.MockStorage(new FileStorage(), "db4o");
				db4oConfig.Storage = storage;
			}
			return db4oConfig;
		}

		public class Item
		{
			public string name;

			public int value;

			public Item(string name, int value)
			{
				this.name = name;
				this.value = value;
			}
		}

		public class MockStorage : StorageDecorator
		{
			private string password;

			public MockStorage(IStorage storage, string password) : base(storage)
			{
				this.password = password;
			}

			protected override IBin Decorate(IBin bin)
			{
				return new COR775TestCase.MockStorage.MockBin(bin, password);
			}

			internal class MockBin : BinDecorator
			{
				private string _password;

				public MockBin(IBin bin, string password) : base(bin)
				{
					_password = password;
				}

				/// <exception cref="Db4oIOException"></exception>
				public override int Read(long pos, byte[] bytes, int length)
				{
					_bin.Read(pos, bytes, length);
					for (int i = 0; i < length; i++)
					{
						bytes[i] = (byte)(bytes[i] - _password.GetHashCode());
					}
					return length;
				}

				/// <exception cref="Db4oIOException"></exception>
				public override void Write(long pos, byte[] buffer, int length)
				{
					for (int i = 0; i < length; i++)
					{
						buffer[i] = (byte)(buffer[i] + _password.GetHashCode());
					}
					_bin.Write(pos, buffer, length);
				}
			}
		}
	}
}
