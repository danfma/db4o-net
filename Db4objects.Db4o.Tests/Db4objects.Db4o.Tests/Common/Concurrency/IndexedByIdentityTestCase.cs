/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Concurrency;
using Db4objects.Db4o.Tests.Common.Persistent;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class IndexedByIdentityTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new IndexedByIdentityTestCase().RunConcurrency();
		}

		public Atom atom;

		internal const int COUNT = 10;

		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(this).ObjectField("atom").Indexed(true);
		}

		protected override void Store()
		{
			for (int i = 0; i < COUNT; i++)
			{
				IndexedByIdentityTestCase ibi = new IndexedByIdentityTestCase();
				ibi.atom = new Atom("ibi" + i);
				Store(ibi);
			}
		}

		public virtual void ConcRead(IExtObjectContainer oc)
		{
			for (int i = 0; i < COUNT; i++)
			{
				IQuery q = oc.Query();
				q.Constrain(typeof(Atom));
				q.Descend("name").Constrain("ibi" + i);
				IObjectSet objectSet = q.Execute();
				Assert.AreEqual(1, objectSet.Size());
				Atom child = (Atom)objectSet.Next();
				q = oc.Query();
				q.Constrain(typeof(IndexedByIdentityTestCase));
				q.Descend("atom").Constrain(child).Identity();
				objectSet = q.Execute();
				Assert.AreEqual(1, objectSet.Size());
				IndexedByIdentityTestCase ibi = (IndexedByIdentityTestCase)objectSet.Next();
				Assert.AreSame(child, ibi.atom);
			}
		}

		/// <exception cref="Exception"></exception>
		public virtual void ConcUpdate(IExtObjectContainer oc, int seq)
		{
			oc.Configure().ObjectClass(typeof(IndexedByIdentityTestCase)).CascadeOnUpdate(true
				);
			IQuery q = oc.Query();
			q.Constrain(typeof(IndexedByIdentityTestCase));
			IObjectSet os = q.Execute();
			Assert.AreEqual(COUNT, os.Size());
			while (os.HasNext())
			{
				IndexedByIdentityTestCase idi = (IndexedByIdentityTestCase)os.Next();
				idi.atom.name = "updated" + seq;
				oc.Set(idi);
				Thread.Sleep(100);
			}
		}

		public virtual void CheckUpdate(IExtObjectContainer oc)
		{
			IQuery q = oc.Query();
			q.Constrain(typeof(IndexedByIdentityTestCase));
			IObjectSet os = q.Execute();
			Assert.AreEqual(COUNT, os.Size());
			string expected = null;
			while (os.HasNext())
			{
				IndexedByIdentityTestCase idi = (IndexedByIdentityTestCase)os.Next();
				if (expected == null)
				{
					expected = idi.atom.name;
					Assert.IsTrue(expected.StartsWith("updated"));
					Assert.IsTrue(expected.Length > "updated".Length);
				}
				Assert.AreEqual(expected, idi.atom.name);
			}
		}
	}
}
