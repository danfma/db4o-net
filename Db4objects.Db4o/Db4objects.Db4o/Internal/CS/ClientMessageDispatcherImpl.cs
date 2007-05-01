using System.IO;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS
{
	internal class ClientMessageDispatcherImpl : Thread, IClientMessageDispatcher
	{
		private ClientObjectContainer i_stream;

		private ISocket4 i_socket;

		private readonly BlockingQueue _messageQueue;

		internal ClientMessageDispatcherImpl(ClientObjectContainer client, ISocket4 a_socket
			, BlockingQueue messageQueue_)
		{
			i_stream = client;
			_messageQueue = messageQueue_;
			i_socket = a_socket;
		}

		public virtual bool IsMessageDispatcherAlive()
		{
			lock (this)
			{
				return i_socket != null;
			}
		}

		public virtual bool Close()
		{
			lock (this)
			{
				i_stream = null;
				i_socket = null;
				return true;
			}
		}

		public override void Run()
		{
			while (IsMessageDispatcherAlive())
			{
				Msg message = null;
				try
				{
					message = Msg.ReadMessage(this, i_stream.GetTransaction(), i_socket);
					if (message is IClientSideMessage)
					{
						if (((IClientSideMessage)message).ProcessAtClient())
						{
							continue;
						}
					}
				}
				catch (IOException)
				{
					Close();
					message = Msg.ERROR;
				}
				_messageQueue.Add(message);
			}
		}

		public virtual void Write(Msg msg)
		{
			i_stream.Write(msg);
		}

		public virtual void SetDispatcherName(string name)
		{
			SetName("db4o message client for user " + name);
		}

		public virtual void StartDispatcher()
		{
			Start();
		}
	}
}