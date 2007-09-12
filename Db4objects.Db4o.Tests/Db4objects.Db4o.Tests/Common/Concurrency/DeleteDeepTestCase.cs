/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Concurrency;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class DeleteDeepTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new DeleteDeepTestCase().RunConcurrency();
		}

		public string name;

		public DeleteDeepTestCase child;

		protected override void Store()
		{
			AddNodes(10);
			name = "root";
			Store(this);
		}

		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(DeleteDeepTestCase)).CascadeOnDelete(true);
		}

		private void AddNodes(int count)
		{
			if (count > 0)
			{
				child = new DeleteDeepTestCase();
				child.name = string.Empty + count;
				child.AddNodes(count - 1);
			}
		}

		public virtual void Conc(IExtObjectContainer oc)
		{
			IQuery q = oc.Query();
			q.Constrain(typeof(DeleteDeepTestCase));
			q.Descend("name").Constrain("root");
			IObjectSet os = q.Execute();
			if (os.Size() == 0)
			{
				return;
			}
			Assert.AreEqual(1, os.Size());
			if (!os.HasNext())
			{
				return;
			}
			DeleteDeepTestCase root = (DeleteDeepTestCase)os.Next();
			oc.Delete(root);
			oc.Commit();
			AssertOccurrences(oc, typeof(DeleteDeepTestCase), 0);
		}

		public virtual void Check(IExtObjectContainer oc)
		{
			AssertOccurrences(oc, typeof(DeleteDeepTestCase), 0);
		}
	}
}