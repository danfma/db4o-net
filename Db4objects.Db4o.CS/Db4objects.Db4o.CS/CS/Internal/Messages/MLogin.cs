/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.CS.Internal;
using Db4objects.Db4o.CS.Internal.Messages;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	/// <exclude></exclude>
	public class MLogin : MsgD, IMessageWithResponse
	{
		public virtual bool ProcessAtServer()
		{
			lock (StreamLock())
			{
				string userName = ReadString();
				string password = ReadString();
				ObjectServerImpl server = ServerMessageDispatcher().Server();
				User found = server.GetUser(userName);
				if (found != null)
				{
					if (found.password.Equals(password))
					{
						ServerMessageDispatcher().SetDispatcherName(userName);
						LogMsg(32, userName);
						int blockSize = Stream().BlockSize();
						int encrypt = Stream()._handlers.i_encrypt ? 1 : 0;
						Write(Msg.LoginOk.GetWriterForInts(Transaction(), new int[] { blockSize, encrypt }
							));
						ServerMessageDispatcher().Login();
						return true;
					}
				}
			}
			Write(Msg.Failed);
			return true;
		}
	}
}