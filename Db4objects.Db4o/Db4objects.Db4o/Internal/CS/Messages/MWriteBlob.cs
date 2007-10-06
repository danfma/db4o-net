/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.IO;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MWriteBlob : MsgBlob, IServerSideMessage
	{
		/// <exception cref="IOException"></exception>
		public override void ProcessClient(ISocket4 sock)
		{
			Msg message = Msg.ReadMessage(MessageDispatcher(), Transaction(), sock);
			if (message.Equals(Msg.OK))
			{
				try
				{
					_currentByte = 0;
					_length = this._blob.GetLength();
					_blob.GetStatusFrom(this);
					_blob.SetStatus(Status.PROCESSING);
					FileInputStream inBlob = this._blob.GetClientInputStream();
					Copy(inBlob, sock, true);
					sock.Flush();
					ObjectContainerBase stream = Stream();
					message = Msg.ReadMessage(MessageDispatcher(), Transaction(), sock);
					if (message.Equals(Msg.OK))
					{
						stream.Deactivate(Transaction(), _blob, int.MaxValue);
						stream.Activate(Transaction(), _blob, int.MaxValue);
						this._blob.SetStatus(Status.COMPLETED);
					}
					else
					{
						this._blob.SetStatus(Status.ERROR);
					}
				}
				catch (Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
		}

		public virtual bool ProcessAtServer()
		{
			try
			{
				BlobImpl blobImpl = this.ServerGetBlobImpl();
				if (blobImpl != null)
				{
					blobImpl.SetTrans(Transaction());
					Sharpen.IO.File file = blobImpl.ServerFile(null, true);
					ISocket4 sock = ServerMessageDispatcher().Socket();
					Msg.OK.Write(sock);
					FileOutputStream fout = new FileOutputStream(file);
					Copy(sock, fout, blobImpl.GetLength(), false);
					Msg.OK.Write(sock);
				}
			}
			catch (Exception)
			{
			}
			return true;
		}
	}
}
