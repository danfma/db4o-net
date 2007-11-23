/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA.Nonta
{
	public abstract class NonTAItemTestCaseBase : ItemTestCaseBase
	{
		protected override void AssertRetrievedItem(object obj)
		{
			return;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestGetByID()
		{
			object item = Db().Ext().GetByID(id);
			AssertNullItem(item);
			Db().Activate(item, 15);
			AssertItemValue(item);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestGetByUUID()
		{
			object item = Db().Ext().GetByUUID(uuid);
			AssertNullItem(item);
			Db().Activate(item, 15);
			AssertItemValue(item);
		}
	}
}