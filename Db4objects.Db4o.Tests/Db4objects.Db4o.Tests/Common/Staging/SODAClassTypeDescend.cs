/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Staging;

namespace Db4objects.Db4o.Tests.Common.Staging
{
	public class SODAClassTypeDescend : AbstractDb4oTestCase
	{
		public class DataA
		{
			public SODAClassTypeDescend.DataB _val;
		}

		public class DataB
		{
			public SODAClassTypeDescend.DataA _val;
		}

		public class DataC
		{
			public SODAClassTypeDescend.DataC _next;
		}

		protected override void Store()
		{
			SODAClassTypeDescend.DataA objectA = new SODAClassTypeDescend.DataA();
			SODAClassTypeDescend.DataB objectB = new SODAClassTypeDescend.DataB();
			objectA._val = objectB;
			objectB._val = objectA;
			Store(objectB);
			Store(new SODAClassTypeDescend.DataC());
		}

		public virtual void TestFieldConstrainedToType()
		{
			IQuery query = NewQuery();
			query.Descend("_val").Constrain(typeof(SODAClassTypeDescend.DataA));
			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Size());
			Assert.IsInstanceOf(typeof(SODAClassTypeDescend.DataB), result.Next());
		}
	}
}
