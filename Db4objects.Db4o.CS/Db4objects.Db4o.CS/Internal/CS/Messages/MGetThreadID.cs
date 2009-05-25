/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MGetThreadID : Msg, IMessageWithResponse
	{
		public virtual bool ProcessAtServer()
		{
			RespondInt(ServerMessageDispatcher().DispatcherID());
			return true;
		}
	}
}
