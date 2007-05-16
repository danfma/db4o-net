/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MWriteUpdateDeleteMembers : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			lock (StreamLock())
			{
				Transaction().WriteUpdateDeleteMembers(ReadInt(), Stream().ClassMetadataForId(ReadInt
					()), ReadInt(), ReadInt());
			}
			return true;
		}
	}
}
