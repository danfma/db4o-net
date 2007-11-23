/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Tests.Common.Ext
{
	public class RefreshTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase().RunClientServer();
		}

		public string name;

		public Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase child;

		public RefreshTestCase()
		{
		}

		public RefreshTestCase(string name, Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase
			 child)
		{
			this.name = name;
			this.child = child;
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase r3 = new Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase
				("o3", null);
			Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase r2 = new Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase
				("o2", r3);
			Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase r1 = new Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase
				("o1", r2);
			Store(r1);
		}

		public virtual void Test()
		{
			IExtObjectContainer oc1 = OpenNewClient();
			IExtObjectContainer oc2 = OpenNewClient();
			oc2.Configure().ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase
				)).CascadeOnUpdate(true);
			try
			{
				Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase r1 = GetRoot(oc1);
				r1.name = "cc";
				oc1.Refresh(r1, 0);
				Assert.AreEqual("cc", r1.name);
				oc1.Refresh(r1, 1);
				Assert.AreEqual("o1", r1.name);
				r1.child.name = "cc";
				oc1.Refresh(r1, 1);
				Assert.AreEqual("cc", r1.child.name);
				oc1.Refresh(r1, 2);
				Assert.AreEqual("o2", r1.child.name);
				Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase r2 = GetRoot(oc2);
				r2.name = "o21";
				r2.child.name = "o22";
				r2.child.child.name = "o23";
				oc2.Set(r2);
				oc2.Commit();
				oc1.Refresh(r1, 3);
				Assert.AreEqual("o21", r1.name);
				Assert.AreEqual("o22", r1.child.name);
				Assert.AreEqual("o23", r1.child.child.name);
			}
			finally
			{
				oc1.Close();
				oc2.Close();
			}
		}

		private Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase GetRoot(IObjectContainer
			 oc)
		{
			return GetByName(oc, "o1");
		}

		private Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase GetByName(IObjectContainer
			 oc, string name)
		{
			IQuery q = oc.Query();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase));
			q.Descend("name").Constrain(name);
			IObjectSet objectSet = q.Execute();
			return (Db4objects.Db4o.Tests.Common.Ext.RefreshTestCase)objectSet.Next();
		}
	}
}
