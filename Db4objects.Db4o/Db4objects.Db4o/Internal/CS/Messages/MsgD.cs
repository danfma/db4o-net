namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <summary>Messages with Data for Client/Server Communication</summary>
	public class MsgD : Db4objects.Db4o.Internal.CS.Messages.Msg
	{
		internal Db4objects.Db4o.Internal.StatefulBuffer _payLoad;

		internal MsgD() : base()
		{
		}

		internal MsgD(string aName) : base(aName)
		{
		}

		internal override void FakePayLoad(Db4objects.Db4o.Internal.Transaction a_trans)
		{
		}

		public override Db4objects.Db4o.Internal.Buffer GetByteLoad()
		{
			return _payLoad;
		}

		public sealed override Db4objects.Db4o.Internal.StatefulBuffer PayLoad()
		{
			return _payLoad;
		}

		public virtual void PayLoad(Db4objects.Db4o.Internal.StatefulBuffer writer)
		{
			_payLoad = writer;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForByte(Db4objects.Db4o.Internal.Transaction
			 trans, byte b)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = GetWriterForLength(trans, 1);
			msg._payLoad.Append(b);
			return msg;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForLength(Db4objects.Db4o.Internal.Transaction
			 trans, int length)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = (Db4objects.Db4o.Internal.CS.Messages.MsgD
				)PublicClone();
			message.SetTransaction(trans);
			message._payLoad = new Db4objects.Db4o.Internal.StatefulBuffer(trans, length + Db4objects.Db4o.Internal.Const4
				.MESSAGE_LENGTH);
			message.WriteInt(_msgID);
			message.WriteInt(length);
			if (trans.ParentTransaction() == null)
			{
				message._payLoad.Append(Db4objects.Db4o.Internal.Const4.SYSTEM_TRANS);
			}
			else
			{
				message._payLoad.Append(Db4objects.Db4o.Internal.Const4.USER_TRANS);
			}
			return message;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriter(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			return GetWriterForLength(trans, 0);
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForInts(Db4objects.Db4o.Internal.Transaction
			 trans, int[] ints)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(trans, Db4objects.Db4o.Internal.Const4
				.INT_LENGTH * ints.Length);
			for (int i = 0; i < ints.Length; i++)
			{
				message.WriteInt(ints[i]);
			}
			return message;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForIntArray(Db4objects.Db4o.Internal.Transaction
			 a_trans, int[] ints, int length)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(a_trans, Db4objects.Db4o.Internal.Const4
				.INT_LENGTH * (length + 1));
			message.WriteInt(length);
			for (int i = 0; i < length; i++)
			{
				message.WriteInt(ints[i]);
			}
			return message;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForInt(Db4objects.Db4o.Internal.Transaction
			 a_trans, int id)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(a_trans, Db4objects.Db4o.Internal.Const4
				.INT_LENGTH);
			message.WriteInt(id);
			return message;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForIntString(Db4objects.Db4o.Internal.Transaction
			 a_trans, int anInt, string str)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(a_trans, Db4objects.Db4o.Internal.Const4
				.stringIO.Length(str) + Db4objects.Db4o.Internal.Const4.INT_LENGTH * 2);
			message.WriteInt(anInt);
			message.WriteString(str);
			return message;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForLong(Db4objects.Db4o.Internal.Transaction
			 a_trans, long a_long)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(a_trans, Db4objects.Db4o.Internal.Const4
				.LONG_LENGTH);
			message.WriteLong(a_long);
			return message;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForString(Db4objects.Db4o.Internal.Transaction
			 a_trans, string str)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(a_trans, Db4objects.Db4o.Internal.Const4
				.stringIO.Length(str) + Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			message.WriteString(str);
			return message;
		}

		public virtual Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriter(Db4objects.Db4o.Internal.StatefulBuffer
			 bytes)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(bytes.GetTransaction
				(), bytes.GetLength());
			message._payLoad.Append(bytes._buffer);
			return message;
		}

		public virtual byte[] ReadBytes()
		{
			return _payLoad.ReadBytes(ReadInt());
		}

		public int ReadInt()
		{
			return _payLoad.ReadInt();
		}

		public long ReadLong()
		{
			return _payLoad.ReadLong();
		}

		public bool ReadBoolean()
		{
			return _payLoad.ReadByte() != 0;
		}

		public virtual object ReadObjectFromPayLoad()
		{
			return Db4objects.Db4o.Internal.Serializer.Unmarshall(Stream(), _payLoad);
		}

		internal sealed override Db4objects.Db4o.Internal.CS.Messages.Msg ReadPayLoad(Db4objects.Db4o.Internal.CS.Messages.IMessageDispatcher
			 messageDispatcher, Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Foundation.Network.ISocket4
			 sock, Db4objects.Db4o.Internal.Buffer reader)
		{
			int length = reader.ReadInt();
			a_trans = CheckParentTransaction(a_trans, reader);
			Db4objects.Db4o.Internal.CS.Messages.MsgD command = (Db4objects.Db4o.Internal.CS.Messages.MsgD
				)PublicClone();
			command.SetTransaction(a_trans);
			command.SetMessageDispatcher(messageDispatcher);
			command._payLoad = ReadMessageBuffer(a_trans, sock, length);
			return command;
		}

		public string ReadString()
		{
			int length = ReadInt();
			return Db4objects.Db4o.Internal.Const4.stringIO.Read(_payLoad, length);
		}

		public void WriteBytes(byte[] aBytes)
		{
			WriteInt(aBytes.Length);
			_payLoad.Append(aBytes);
		}

		public void WriteInt(int aInt)
		{
			_payLoad.WriteInt(aInt);
		}

		public void WriteLong(long l)
		{
			_payLoad.WriteLong(l);
		}

		public void WriteString(string aStr)
		{
			_payLoad.WriteInt(aStr.Length);
			Db4objects.Db4o.Internal.Const4.stringIO.Write(_payLoad, aStr);
		}
	}
}
