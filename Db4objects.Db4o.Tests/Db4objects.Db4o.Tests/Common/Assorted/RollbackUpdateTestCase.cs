/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class RollbackUpdateTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new RollbackUpdateTestCase().RunClientServer();
		}

		protected override void Store()
		{
			Store(new SimpleObject("hello", 1));
		}

		public virtual void Test()
		{
			IExtObjectContainer oc1 = OpenNewClient();
			IExtObjectContainer oc2 = OpenNewClient();
			IExtObjectContainer oc3 = OpenNewClient();
			try
			{
				SimpleObject o1 = (SimpleObject)RetrieveOnlyInstance(oc1, typeof(SimpleObject));
				o1.SetS("o1");
				oc1.Set(o1);
				SimpleObject o2 = (SimpleObject)RetrieveOnlyInstance(oc2, typeof(SimpleObject));
				Assert.AreEqual("hello", o2.GetS());
				oc1.Rollback();
				o2 = (SimpleObject)RetrieveOnlyInstance(oc2, typeof(SimpleObject));
				oc2.Refresh(o2, int.MaxValue);
				Assert.AreEqual("hello", o2.GetS());
				oc1.Commit();
				o2 = (SimpleObject)RetrieveOnlyInstance(oc2, typeof(SimpleObject));
				Assert.AreEqual("hello", o2.GetS());
				oc1.Set(o1);
				oc1.Commit();
				oc2.Refresh(o2, int.MaxValue);
				o2 = (SimpleObject)RetrieveOnlyInstance(oc2, typeof(SimpleObject));
				Assert.AreEqual("o1", o2.GetS());
				SimpleObject o3 = (SimpleObject)RetrieveOnlyInstance(oc3, typeof(SimpleObject));
				Assert.AreEqual("o1", o3.GetS());
			}
			finally
			{
				oc1.Close();
				oc2.Close();
				oc3.Close();
			}
		}
	}
}
