/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Querying;

namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadeOnDeleteHierarchyTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new CascadeOnDeleteHierarchyTestCase().RunAll();
		}

		public class Item
		{
		}

		public class SubItem : CascadeOnDeleteHierarchyTestCase.Item
		{
			public CascadeOnDeleteHierarchyTestCase.Data data;

			public SubItem()
			{
				data = new CascadeOnDeleteHierarchyTestCase.Data();
			}
		}

		public class Data
		{
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(CascadeOnDeleteHierarchyTestCase.Item)).CascadeOnDelete
				(true);
			config.ObjectClass(typeof(CascadeOnDeleteHierarchyTestCase.SubItem));
			base.Configure(config);
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new CascadeOnDeleteHierarchyTestCase.SubItem());
		}

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			CascadeOnDeleteHierarchyTestCase.SubItem item = (CascadeOnDeleteHierarchyTestCase.SubItem
				)RetrieveOnlyInstance(typeof(CascadeOnDeleteHierarchyTestCase.SubItem));
			Db().Delete(item);
			AssertOccurrences(typeof(CascadeOnDeleteHierarchyTestCase.Data), 0);
			Db().Commit();
			AssertOccurrences(typeof(CascadeOnDeleteHierarchyTestCase.Data), 0);
		}
	}
}