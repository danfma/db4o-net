/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MRollback : Msg, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			lock (StreamLock())
			{
				Transaction().Rollback();
			}
			return true;
		}
	}
}
