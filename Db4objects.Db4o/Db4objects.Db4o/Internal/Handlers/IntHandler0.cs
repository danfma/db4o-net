/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class IntHandler0 : IntHandler
	{
		public IntHandler0(ObjectContainerBase container) : base(container)
		{
		}

		public override object Read(IReadContext context)
		{
			int i = context.ReadInt();
			if (i == int.MaxValue)
			{
				return null;
			}
			return i;
		}
	}
}