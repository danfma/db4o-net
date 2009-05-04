/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Btree;

namespace Db4objects.Db4o.Internal.Btree
{
	internal class BTreeRangeKeyIterator : Db4objects.Db4o.Internal.Btree.AbstractBTreeRangeIterator
	{
		public BTreeRangeKeyIterator(BTreeRangeSingle range) : base(range)
		{
		}

		public override object Current
		{
			get
			{
				return CurrentPointer().Key();
			}
		}
	}
}
