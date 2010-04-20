/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Ids;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class PersistentIntegerArray : LocalPersistentBase
	{
		private int[] _ints;

		public PersistentIntegerArray(ITransactionalIdSystem idSystem, int[] arr) : base(
			idSystem)
		{
			_ints = new int[arr.Length];
			System.Array.Copy(arr, 0, _ints, 0, arr.Length);
		}

		public PersistentIntegerArray(ITransactionalIdSystem idSystem, int id) : base(idSystem
			)
		{
			SetID(id);
		}

		public PersistentIntegerArray(int id) : this(null, id)
		{
		}

		public PersistentIntegerArray(int[] arr) : this(null, arr)
		{
		}

		public override byte GetIdentifier()
		{
			return Const4.IntegerArray;
		}

		public override int OwnLength()
		{
			return (Const4.IntLength * (Size() + 1)) + Const4.AddedLength;
		}

		public override void ReadThis(Transaction trans, ByteArrayBuffer reader)
		{
			int length = reader.ReadInt();
			_ints = new int[length];
			for (int i = 0; i < length; i++)
			{
				_ints[i] = reader.ReadInt();
			}
		}

		public override void WriteThis(Transaction trans, ByteArrayBuffer writer)
		{
			writer.WriteInt(Size());
			for (int i = 0; i < _ints.Length; i++)
			{
				writer.WriteInt(_ints[i]);
			}
		}

		private int Size()
		{
			return _ints.Length;
		}

		public virtual int[] Array()
		{
			return _ints;
		}

		public override Db4objects.Db4o.Internal.Slots.SlotChangeFactory SlotChangeFactory
			()
		{
			return Db4objects.Db4o.Internal.Slots.SlotChangeFactory.FreeSpace;
		}
	}
}
