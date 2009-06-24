/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public class MTaDelete : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int id = _payLoad.ReadInt();
			int cascade = _payLoad.ReadInt();
			Transaction trans = Transaction();
			lock (StreamLock())
			{
				trans.Delete(null, id, cascade);
				return true;
			}
		}
	}
}