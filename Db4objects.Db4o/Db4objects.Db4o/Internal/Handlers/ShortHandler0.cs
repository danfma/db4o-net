/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	public class ShortHandler0 : ShortHandler
	{
		public ShortHandler0(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Read(IReadContext context)
		{
			short value = (short)base.Read(context);
			if (value == short.MaxValue)
			{
				return null;
			}
			return value;
		}
	}
}