/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Concurrency;
using Db4objects.Db4o.Tests.Common.Persistent;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class SetRollbackTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new SetRollbackTestCase().RunConcurrency();
		}

		public virtual void ConcSetRollback(IExtObjectContainer oc, int seq)
		{
			if (seq % 2 == 0)
			{
				for (int i = 0; i < 1000; i++)
				{
					SimpleObject c = new SimpleObject("oc " + i, i);
					oc.Set(c);
				}
			}
			else
			{
				for (int i = 0; i < 1000; i++)
				{
					SimpleObject c = new SimpleObject("oc " + i, i);
					oc.Set(c);
					oc.Rollback();
					c = new SimpleObject("oc2.2 " + i, i);
					oc.Set(c);
				}
				oc.Rollback();
			}
		}

		public virtual void CheckSetRollback(IExtObjectContainer oc)
		{
			Assert.AreEqual(ThreadCount() / 2 * 1000, oc.Query(typeof(SimpleObject)).Size());
		}
	}
}
