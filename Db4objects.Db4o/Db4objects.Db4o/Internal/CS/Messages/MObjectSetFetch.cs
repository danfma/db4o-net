/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetFetch : MObjectSet, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			int queryResultID = ReadInt();
			int fetchSize = ReadInt();
			IIntIterator4 idIterator = Stub(queryResultID).IdIterator();
			MsgD message = ID_LIST.GetWriterForLength(Transaction(), BufferLength(fetchSize));
			StatefulBuffer writer = message.PayLoad();
			writer.WriteIDs(idIterator, fetchSize);
			Write(message);
			return true;
		}

		private int BufferLength(int fetchSize)
		{
			return Const4.INT_LENGTH * (fetchSize + 1);
		}
	}
}
