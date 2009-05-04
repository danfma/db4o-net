/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	/// <exclude></exclude>
	public class InMemoryObjectContainerTestCase : ITestLifeCycle
	{
		private MemoryFile memoryFile;

		private IObjectContainer objectContainer;

		private static int StoredItems = 1000;

		/// <exception cref="System.Exception"></exception>
		[System.ObsoleteAttribute(@"using deprecated api")]
		public virtual void SetUp()
		{
			memoryFile = new MemoryFile();
			memoryFile.SetIncrementSizeBy(100);
			memoryFile.SetInitialSize(100);
			objectContainer = ExtDb4oFactory.OpenMemoryFile(memoryFile);
		}

		public class Item
		{
		}

		public virtual void TestSizeIncrement()
		{
			int lastSize = FileSize();
			for (int i = 0; i < StoredItems; i++)
			{
				objectContainer.Store(new InMemoryObjectContainerTestCase.Item());
				Assert.IsSmaller(lastSize + 1000, FileSize());
				lastSize = FileSize();
			}
		}

		private int FileSize()
		{
			return memoryFile.GetBytes().Length;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TearDown()
		{
		}
	}
}
