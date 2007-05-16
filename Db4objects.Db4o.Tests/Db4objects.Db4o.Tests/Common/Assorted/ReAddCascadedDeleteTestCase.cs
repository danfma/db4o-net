/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ReAddCascadedDeleteTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ReAddCascadedDeleteTestCase().RunClientServer();
		}

		public class Item
		{
			public string _name;

			public ReAddCascadedDeleteTestCase.Item _member;

			public Item()
			{
			}

			public Item(string name)
			{
				_name = name;
			}

			public Item(string name, ReAddCascadedDeleteTestCase.Item member)
			{
				_name = name;
				_member = member;
			}
		}

		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(ReAddCascadedDeleteTestCase.Item)).CascadeOnDelete(true
				);
		}

		protected override void Store()
		{
			Db().Set(new ReAddCascadedDeleteTestCase.Item("parent", new ReAddCascadedDeleteTestCase.Item
				("child")));
		}

		public virtual void TestDeletingAndReaddingMember()
		{
			DeleteParentAndReAddChild();
			Reopen();
			Assert.IsNotNull(Query("child"));
			Assert.IsNull(Query("parent"));
		}

		private void DeleteParentAndReAddChild()
		{
			ReAddCascadedDeleteTestCase.Item i = Query("parent");
			Db().Delete(i);
			Db().Set(i._member);
			Db().Commit();
		}

		private ReAddCascadedDeleteTestCase.Item Query(string name)
		{
			IObjectSet objectSet = Db().Get(new ReAddCascadedDeleteTestCase.Item(name));
			if (!objectSet.HasNext())
			{
				return null;
			}
			return (ReAddCascadedDeleteTestCase.Item)objectSet.Next();
		}
	}
}
