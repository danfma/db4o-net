/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	/// <exclude></exclude>
	public class MCommitSystemTransaction : Msg, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			lock (StreamLock())
			{
				Transaction().SystemTransaction().Commit();
			}
			return true;
		}
	}
}