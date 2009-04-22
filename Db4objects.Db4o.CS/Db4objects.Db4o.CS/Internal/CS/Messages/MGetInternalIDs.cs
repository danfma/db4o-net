/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MGetInternalIDs : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			ByteArrayBuffer bytes = this.GetByteLoad();
			long[] ids;
			lock (StreamLock())
			{
				try
				{
					ids = Stream().ClassMetadataForID(bytes.ReadInt()).GetIDs(Transaction());
				}
				catch (Exception)
				{
					ids = new long[0];
				}
			}
			int size = ids.Length;
			MsgD message = Msg.IdList.GetWriterForLength(Transaction(), Const4.IdLength * (size
				 + 1));
			ByteArrayBuffer writer = message.PayLoad();
			writer.WriteInt(size);
			for (int i = 0; i < size; i++)
			{
				writer.WriteInt((int)ids[i]);
			}
			Write(message);
			return true;
		}
	}
}
