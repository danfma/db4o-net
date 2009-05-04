/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MCommittedCallBackRegistry : Msg, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			IServerMessageDispatcher dispatcher = ServerMessageDispatcher();
			dispatcher.CaresAboutCommitted(true);
			return true;
		}
	}
}
