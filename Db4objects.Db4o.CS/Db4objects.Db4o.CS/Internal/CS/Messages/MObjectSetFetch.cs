/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.CS.Objectexchange;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetFetch : MObjectSet, IMessageWithResponse
	{
		public virtual bool ProcessAtServer()
		{
			int queryResultID = ReadInt();
			int fetchSize = ReadInt();
			int fetchDepth = ReadInt();
			MsgD message = null;
			lock (StreamLock())
			{
				IIntIterator4 idIterator = Stub(queryResultID).IdIterator();
				ByteArrayBuffer payload = ObjectExchangeStrategyFactory.ForConfig(new ObjectExchangeConfiguration
					(fetchDepth, fetchSize)).Marshall((LocalTransaction)Transaction(), idIterator, fetchSize
					);
				message = IdList.GetWriterForBuffer(Transaction(), payload);
			}
			Write(message);
			return true;
		}
	}
}
