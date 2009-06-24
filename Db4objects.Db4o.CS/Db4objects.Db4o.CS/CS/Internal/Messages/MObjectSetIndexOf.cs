/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	/// <exclude></exclude>
	public class MObjectSetIndexOf : MObjectSet, IMessageWithResponse
	{
		public virtual bool ProcessAtServer()
		{
			AbstractQueryResult queryResult = QueryResult(ReadInt());
			lock (StreamLock())
			{
				int id = queryResult.IndexOf(ReadInt());
				Write(Msg.ObjectsetIndexof.GetWriterForInt(Transaction(), id));
			}
			return true;
		}
	}
}