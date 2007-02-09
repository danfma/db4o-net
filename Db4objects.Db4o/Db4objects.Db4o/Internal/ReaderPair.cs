namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public sealed class ReaderPair : Db4objects.Db4o.Internal.ISlotReader
	{
		private Db4objects.Db4o.Internal.Buffer _source;

		private Db4objects.Db4o.Internal.Buffer _target;

		private Db4objects.Db4o.Internal.Mapping.IDefragContext _mapping;

		private Db4objects.Db4o.Internal.Transaction _systemTrans;

		public ReaderPair(Db4objects.Db4o.Internal.Buffer source, Db4objects.Db4o.Internal.Mapping.IDefragContext
			 mapping, Db4objects.Db4o.Internal.Transaction systemTrans)
		{
			_source = source;
			_mapping = mapping;
			_target = new Db4objects.Db4o.Internal.Buffer(source.GetLength());
			_source.CopyTo(_target, 0, 0, _source.GetLength());
			_systemTrans = systemTrans;
		}

		public int Offset()
		{
			return _source.Offset();
		}

		public void Offset(int offset)
		{
			_source.Offset(offset);
			_target.Offset(offset);
		}

		public void IncrementOffset(int numBytes)
		{
			_source.IncrementOffset(numBytes);
			_target.IncrementOffset(numBytes);
		}

		public void IncrementIntSize()
		{
			IncrementOffset(Db4objects.Db4o.Internal.Const4.INT_LENGTH);
		}

		public int CopyUnindexedID()
		{
			int orig = _source.ReadInt();
			int mapped = -1;
			try
			{
				mapped = _mapping.MappedID(orig);
			}
			catch (Db4objects.Db4o.Internal.Mapping.MappingNotFoundException)
			{
				mapped = _mapping.AllocateTargetSlot(Db4objects.Db4o.Internal.Const4.POINTER_LENGTH
					);
				_mapping.MapIDs(orig, mapped, false);
				_mapping.RegisterUnindexed(orig);
			}
			_target.WriteInt(mapped);
			return mapped;
		}

		public int CopyID()
		{
			int mapped = _mapping.MappedID(_source.ReadInt(), false);
			_target.WriteInt(mapped);
			return mapped;
		}

		public int CopyID(bool flipNegative, bool lenient)
		{
			int id = _source.ReadInt();
			return InternalCopyID(flipNegative, lenient, id);
		}

		public Db4objects.Db4o.Internal.Mapping.MappedIDPair CopyIDAndRetrieveMapping()
		{
			int id = _source.ReadInt();
			return new Db4objects.Db4o.Internal.Mapping.MappedIDPair(id, InternalCopyID(false
				, false, id));
		}

		private int InternalCopyID(bool flipNegative, bool lenient, int id)
		{
			if (flipNegative && id < 0)
			{
				id = -id;
			}
			int mapped = _mapping.MappedID(id, lenient);
			if (flipNegative && id < 0)
			{
				mapped = -mapped;
			}
			_target.WriteInt(mapped);
			return mapped;
		}

		public void ReadBegin(byte identifier)
		{
			_source.ReadBegin(identifier);
			_target.ReadBegin(identifier);
		}

		public byte ReadByte()
		{
			byte value = _source.ReadByte();
			_target.IncrementOffset(1);
			return value;
		}

		public int ReadInt()
		{
			int value = _source.ReadInt();
			_target.IncrementOffset(Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			return value;
		}

		public void WriteInt(int value)
		{
			_source.IncrementOffset(Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			_target.WriteInt(value);
		}

		public void Write(Db4objects.Db4o.Internal.LocalObjectContainer file, int address
			)
		{
			file.WriteBytes(_target, address, 0);
		}

		public string ReadShortString(Db4objects.Db4o.Internal.LatinStringIO sio)
		{
			string value = Db4objects.Db4o.Internal.Marshall.StringMarshaller.ReadShort(sio, 
				false, _source);
			Db4objects.Db4o.Internal.Marshall.StringMarshaller.ReadShort(sio, false, _target);
			return value;
		}

		public Db4objects.Db4o.Internal.Buffer Source()
		{
			return _source;
		}

		public Db4objects.Db4o.Internal.Buffer Target()
		{
			return _target;
		}

		public Db4objects.Db4o.Internal.Mapping.IIDMapping Mapping()
		{
			return _mapping;
		}

		public Db4objects.Db4o.Internal.Transaction SystemTrans()
		{
			return _systemTrans;
		}

		public Db4objects.Db4o.Internal.Mapping.IDefragContext Context()
		{
			return _mapping;
		}

		public static void ProcessCopy(Db4objects.Db4o.Internal.Mapping.IDefragContext context
			, int sourceID, Db4objects.Db4o.Internal.ISlotCopyHandler command)
		{
			ProcessCopy(context, sourceID, command, false);
		}

		public static void ProcessCopy(Db4objects.Db4o.Internal.Mapping.IDefragContext context
			, int sourceID, Db4objects.Db4o.Internal.ISlotCopyHandler command, bool registerAddressMapping
			)
		{
			Db4objects.Db4o.Internal.Buffer sourceReader = (registerAddressMapping ? context.
				SourceWriterByID(sourceID) : context.SourceReaderByID(sourceID));
			int targetID = context.MappedID(sourceID);
			int targetLength = sourceReader.GetLength();
			int targetAddress = context.AllocateTargetSlot(targetLength);
			if (registerAddressMapping)
			{
				int sourceAddress = ((Db4objects.Db4o.Internal.StatefulBuffer)sourceReader).GetAddress
					();
				context.MapIDs(sourceAddress, targetAddress, false);
			}
			Db4objects.Db4o.Internal.Buffer targetPointerReader = new Db4objects.Db4o.Internal.Buffer
				(Db4objects.Db4o.Internal.Const4.POINTER_LENGTH);
			targetPointerReader.WriteInt(targetAddress);
			targetPointerReader.WriteInt(targetLength);
			context.TargetWriteBytes(targetPointerReader, targetID);
			Db4objects.Db4o.Internal.ReaderPair readers = new Db4objects.Db4o.Internal.ReaderPair
				(sourceReader, context, context.SystemTrans());
			command.ProcessCopy(readers);
			context.TargetWriteBytes(readers, targetAddress);
		}

		public void Append(byte value)
		{
			_source.IncrementOffset(1);
			_target.Append(value);
		}

		public long ReadLong()
		{
			long value = _source.ReadLong();
			_target.IncrementOffset(Db4objects.Db4o.Internal.Const4.LONG_LENGTH);
			return value;
		}

		public void WriteLong(long value)
		{
			_source.IncrementOffset(Db4objects.Db4o.Internal.Const4.LONG_LENGTH);
			_target.WriteLong(value);
		}

		public Db4objects.Db4o.Foundation.BitMap4 ReadBitMap(int bitCount)
		{
			Db4objects.Db4o.Foundation.BitMap4 value = _source.ReadBitMap(bitCount);
			_target.IncrementOffset(value.MarshalledLength());
			return value;
		}

		public void CopyBytes(byte[] target, int sourceOffset, int targetOffset, int length
			)
		{
			_source.CopyBytes(target, sourceOffset, targetOffset, length);
		}

		public void ReadEnd()
		{
			_source.ReadEnd();
			_target.ReadEnd();
		}

		public int PreparePayloadRead()
		{
			int newPayLoadOffset = ReadInt();
			ReadInt();
			int linkOffSet = Offset();
			Offset(newPayLoadOffset);
			return linkOffSet;
		}
	}
}