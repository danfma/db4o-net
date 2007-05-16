/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MRollback : Msg, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Transaction().Rollback();
			return true;
		}
	}
}
