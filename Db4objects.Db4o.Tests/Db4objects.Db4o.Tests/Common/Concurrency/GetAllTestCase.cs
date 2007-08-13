/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Concurrency;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class GetAllTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new GetAllTestCase().RunConcurrency();
		}

		protected override void Store()
		{
			Store(new GetAllTestCase());
			Store(new GetAllTestCase());
		}

		public virtual void Conc(IExtObjectContainer oc)
		{
			Assert.AreEqual(2, oc.Get(null).Size());
		}

		public virtual void ConcSODA(IExtObjectContainer oc)
		{
			Assert.AreEqual(2, oc.Query().Execute().Size());
		}
	}
}
