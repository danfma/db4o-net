/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Tests.Common.TA.Mixed;

namespace Db4objects.Db4o.Tests.Common.TA.Mixed
{
	/// <exclude></exclude>
	public class NTNItem
	{
		public TNItem tnItem;

		public NTNItem()
		{
		}

		public NTNItem(int v)
		{
			tnItem = new TNItem(v);
		}
	}
}
