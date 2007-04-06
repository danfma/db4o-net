using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class CanUpdateFalseRefreshTestCase : AbstractDb4oTestCase
	{
		public class Item
		{
			public int _id;

			public string _name;

			public Item(int id, string name)
			{
				_id = id;
				_name = name;
			}

			public virtual bool ObjectCanUpdate(IObjectContainer container)
			{
				return false;
			}
		}

		protected override void Store()
		{
			Store(new CanUpdateFalseRefreshTestCase.Item(1, "one"));
		}

		public virtual void Test()
		{
			CanUpdateFalseRefreshTestCase.Item item = (CanUpdateFalseRefreshTestCase.Item)RetrieveOnlyInstance
				(typeof(CanUpdateFalseRefreshTestCase.Item));
			item._name = "two";
			Db().Set(item);
			Assert.AreEqual("two", item._name);
			Db().Refresh(item, 2);
			Assert.AreEqual("one", item._name);
		}

		public static void Main(string[] args)
		{
			new CanUpdateFalseRefreshTestCase().RunSoloAndClientServer();
		}
	}
}
