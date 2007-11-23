/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA.TA
{
	public abstract class TAItemTestCaseBase : ItemTestCaseBase, IOptOutTA
	{
		/// <exception cref="Exception"></exception>
		public virtual void TestGetByID()
		{
			object item = Db().Ext().GetByID(id);
			AssertNullItem(item);
			AssertItemValue(item);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestGetByUUID()
		{
			object item = Db().Ext().GetByUUID(uuid);
			AssertNullItem(item);
			AssertItemValue(item);
		}

		/// <exception cref="Exception"></exception>
		protected override void AssertRetrievedItem(object obj)
		{
			AssertNullItem(obj);
		}
	}
}
