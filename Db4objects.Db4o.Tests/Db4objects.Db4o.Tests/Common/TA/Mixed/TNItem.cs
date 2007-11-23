/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA.Mixed
{
	/// <exclude></exclude>
	public class TNItem : ActivatableImpl
	{
		public LinkedList list;

		public TNItem()
		{
		}

		public TNItem(int v)
		{
			list = LinkedList.NewList(v);
		}

		public virtual LinkedList Value()
		{
			Activate();
			return list;
		}
	}
}
