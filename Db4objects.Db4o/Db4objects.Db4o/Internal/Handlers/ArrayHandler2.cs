/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class ArrayHandler2 : ArrayHandler
	{
		protected override int PreparePayloadRead(IDefragmentContext context)
		{
			int newPayLoadOffset = context.ReadInt();
			context.ReadInt();
			// skip length, not needed
			int linkOffSet = context.Offset();
			context.Seek(newPayLoadOffset);
			return linkOffSet;
		}
	}
}