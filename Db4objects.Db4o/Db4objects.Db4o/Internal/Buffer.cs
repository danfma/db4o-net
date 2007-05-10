using System;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Slots;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Buffer : ISlotReader
	{
		public byte[] _buffer;

		public int _offset;

		internal Buffer()
		{
		}

		public Buffer(int a_length)
		{
			_buffer = new byte[a_length];
		}

		public virtual void Seek(int offset)
		{
			_offset = offset;
		}

		public void Append(byte a_byte)
		{
			_buffer[_offset++] = a_byte;
		}

		public virtual void Append(byte[] a_bytes)
		{
			System.Array.Copy(a_bytes, 0, _buffer, _offset, a_bytes.Length);
			_offset += a_bytes.Length;
		}

		public bool ContainsTheSame(Db4objects.Db4o.Internal.Buffer other)
		{
			if (other != null)
			{
				byte[] otherBytes = other._buffer;
				if (_buffer == null)
				{
					return otherBytes == null;
				}
				if (otherBytes != null && _buffer.Length == otherBytes.Length)
				{
					int len = _buffer.Length;
					for (int i = 0; i < len; i++)
					{
						if (_buffer[i] != otherBytes[i])
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		public virtual void CopyTo(Db4objects.Db4o.Internal.Buffer to, int fromOffset, int
			 toOffset, int length)
		{
			System.Array.Copy(_buffer, fromOffset, to._buffer, toOffset, length);
		}

		public virtual int GetLength()
		{
			return _buffer.Length;
		}

		public virtual void IncrementOffset(int a_by)
		{
			_offset += a_by;
		}

		/// <summary>non-encrypted read, used for indexes</summary>
		/// <param name="a_stream"></param>
		/// <param name="a_address"></param>
		/// <exception cref="IOException"></exception>
		public virtual void Read(ObjectContainerBase stream, int address, int addressOffset
			)
		{
			stream.ReadBytes(_buffer, address, addressOffset, GetLength());
		}

		public void ReadBegin(byte a_identifier)
		{
		}

		public virtual BitMap4 ReadBitMap(int bitCount)
		{
			BitMap4 map = new BitMap4(_buffer, _offset, bitCount);
			_offset += map.MarshalledLength();
			return map;
		}

		public virtual byte ReadByte()
		{
			return _buffer[_offset++];
		}

		public virtual byte[] ReadBytes(int a_length)
		{
			byte[] bytes = new byte[a_length];
			ReadBytes(bytes);
			return bytes;
		}

		public virtual void ReadBytes(byte[] bytes)
		{
			int length = bytes.Length;
			System.Array.Copy(_buffer, _offset, bytes, 0, length);
			_offset += length;
		}

		public Db4objects.Db4o.Internal.Buffer ReadEmbeddedObject(Transaction trans)
		{
			return trans.Stream().BufferByAddress(ReadInt(), ReadInt());
		}

		public virtual void ReadEncrypt(ObjectContainerBase stream, int address)
		{
			stream.ReadBytes(_buffer, address, GetLength());
			stream.i_handlers.Decrypt(this);
		}

		public virtual void ReadEnd()
		{
			if (Deploy.debug && Deploy.brackets)
			{
				if (ReadByte() != Const4.YAPEND)
				{
					throw new Exception("YapBytes.readEnd() YAPEND expected");
				}
			}
		}

		public int ReadInt()
		{
			int o = (_offset += 4) - 1;
			return (_buffer[o] & 255) | (_buffer[--o] & 255) << 8 | (_buffer[--o] & 255) << 16
				 | _buffer[--o] << 24;
		}

		public virtual long ReadLong()
		{
			long ret = 0;
			ret = PrimitiveCodec.ReadLong(this._buffer, this._offset);
			this.IncrementOffset(Const4.LONG_BYTES);
			return ret;
		}

		public virtual Db4objects.Db4o.Internal.Buffer ReadPayloadReader(int offset, int 
			length)
		{
			Db4objects.Db4o.Internal.Buffer payLoad = new Db4objects.Db4o.Internal.Buffer(length
				);
			System.Array.Copy(_buffer, offset, payLoad._buffer, 0, length);
			return payLoad;
		}

		public virtual Slot ReadSlot()
		{
			return new Slot(ReadInt(), ReadInt());
		}

		internal virtual void ReplaceWith(byte[] a_bytes)
		{
			System.Array.Copy(a_bytes, 0, _buffer, 0, GetLength());
		}

		public override string ToString()
		{
			string str = string.Empty;
			for (int i = 0; i < _buffer.Length; i++)
			{
				if (i > 0)
				{
					str += " , ";
				}
				str += _buffer[i];
			}
			return str;
		}

		public virtual void WriteBegin(byte a_identifier)
		{
		}

		public void WriteBitMap(BitMap4 nullBitMap)
		{
			nullBitMap.WriteTo(_buffer, _offset);
			_offset += nullBitMap.MarshalledLength();
		}

		public void WriteEncrypt(LocalObjectContainer file, int address, int addressOffset
			)
		{
			file.i_handlers.Encrypt(this);
			file.WriteBytes(this, address, addressOffset);
			file.i_handlers.Decrypt(this);
		}

		public virtual void WriteEnd()
		{
			if (Deploy.debug && Deploy.brackets)
			{
				Append(Const4.YAPEND);
			}
		}

		public void WriteInt(int a_int)
		{
			int o = _offset + 4;
			_offset = o;
			byte[] b = _buffer;
			b[--o] = (byte)a_int;
			b[--o] = (byte)(a_int >>= 8);
			b[--o] = (byte)(a_int >>= 8);
			b[--o] = (byte)(a_int >> 8);
		}

		public virtual void WriteIDOf(Transaction trans, object obj)
		{
			if (obj == null)
			{
				WriteInt(0);
				return;
			}
			if (obj is PersistentBase)
			{
				WriteIDOf(trans, (PersistentBase)obj);
				return;
			}
			WriteInt(((int)obj));
		}

		public virtual void WriteIDOf(Transaction trans, PersistentBase persistent)
		{
			if (persistent == null)
			{
				WriteInt(0);
				return;
			}
			if (CanWritePersistentBase())
			{
				persistent.WriteOwnID(trans, this);
			}
			else
			{
				WriteInt(persistent.GetID());
			}
		}

		public void WriteSlot(Slot slot)
		{
			WriteInt(slot.Address());
			WriteInt(slot.Length());
		}

		protected virtual bool CanWritePersistentBase()
		{
			return true;
		}

		public virtual void WriteShortString(Transaction trans, string a_string)
		{
			trans.Stream().i_handlers.i_stringHandler.WriteShort(a_string, this);
		}

		public virtual void WriteLong(long l)
		{
			LongHandler.WriteLong(l, this);
		}

		public virtual void IncrementIntSize()
		{
			IncrementOffset(Const4.INT_LENGTH);
		}

		public virtual int Offset()
		{
			return _offset;
		}

		public virtual void Offset(int offset)
		{
			_offset = offset;
		}

		public virtual void CopyBytes(byte[] target, int sourceOffset, int targetOffset, 
			int length)
		{
			System.Array.Copy(_buffer, sourceOffset, target, targetOffset, length);
		}
	}
}
