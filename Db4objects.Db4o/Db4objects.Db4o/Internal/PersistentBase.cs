/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class PersistentBase : IPersistent
	{
		protected int _id;

		protected int _state = 2;

		public bool BeginProcessing()
		{
			if (BitIsTrue(Const4.PROCESSING))
			{
				return false;
			}
			BitTrue(Const4.PROCESSING);
			return true;
		}

		internal void BitFalse(int bitPos)
		{
			_state &= ~(1 << bitPos);
		}

		internal bool BitIsFalse(int bitPos)
		{
			return (_state | (1 << bitPos)) != _state;
		}

		internal bool BitIsTrue(int bitPos)
		{
			return (_state | (1 << bitPos)) == _state;
		}

		internal void BitTrue(int bitPos)
		{
			_state |= (1 << bitPos);
		}

		internal virtual void CacheDirty(Collection4 col)
		{
			if (!BitIsTrue(Const4.CACHED_DIRTY))
			{
				BitTrue(Const4.CACHED_DIRTY);
				col.Add(this);
			}
		}

		public virtual void EndProcessing()
		{
			BitFalse(Const4.PROCESSING);
		}

		public virtual void Free(Transaction trans)
		{
			trans.SystemTransaction().SlotFreePointerOnCommit(GetID());
		}

		public virtual int GetID()
		{
			return _id;
		}

		public bool IsActive()
		{
			return BitIsTrue(Const4.ACTIVE);
		}

		public virtual bool IsDirty()
		{
			return BitIsTrue(Const4.ACTIVE) && (!BitIsTrue(Const4.CLEAN));
		}

		public bool IsNew()
		{
			return GetID() == 0;
		}

		public virtual int LinkLength()
		{
			return Const4.ID_LENGTH;
		}

		internal void NotCachedDirty()
		{
			BitFalse(Const4.CACHED_DIRTY);
		}

		public virtual void Read(Transaction trans)
		{
			if (!BeginProcessing())
			{
				return;
			}
			try
			{
				Db4objects.Db4o.Internal.Buffer reader = trans.Container().ReadReaderByID(trans, 
					GetID());
				ReadThis(trans, reader);
				SetStateOnRead(reader);
			}
			finally
			{
				EndProcessing();
			}
		}

		public virtual void SetID(int a_id)
		{
			if (DTrace.enabled)
			{
				DTrace.YAPMETA_SET_ID.Log(a_id);
			}
			_id = a_id;
		}

		public void SetStateClean()
		{
			BitTrue(Const4.ACTIVE);
			BitTrue(Const4.CLEAN);
		}

		public void SetStateDeactivated()
		{
			BitFalse(Const4.ACTIVE);
		}

		public virtual void SetStateDirty()
		{
			BitTrue(Const4.ACTIVE);
			BitFalse(Const4.CLEAN);
		}

		internal virtual void SetStateOnRead(Db4objects.Db4o.Internal.Buffer reader)
		{
			if (BitIsTrue(Const4.CACHED_DIRTY))
			{
				SetStateDirty();
			}
			else
			{
				SetStateClean();
			}
		}

		public void Write(Transaction trans)
		{
			if (!WriteObjectBegin())
			{
				return;
			}
			try
			{
				LocalObjectContainer stream = (LocalObjectContainer)trans.Container();
				int length = OwnLength();
				length = stream.BlockAlignedBytes(length);
				Db4objects.Db4o.Internal.Buffer writer = new Db4objects.Db4o.Internal.Buffer(length
					);
				Slot slot;
				if (IsNew())
				{
					Pointer4 pointer = stream.NewSlot(length);
					SetID(pointer._id);
					slot = pointer._slot;
					trans.SetPointer(pointer);
				}
				else
				{
					slot = stream.GetSlot(length);
					trans.SlotFreeOnRollbackCommitSetPointer(_id, slot, IsFreespaceComponent());
				}
				WriteToFile(trans, writer, slot);
			}
			finally
			{
				EndProcessing();
			}
		}

		public virtual bool IsFreespaceComponent()
		{
			return false;
		}

		private void WriteToFile(Transaction trans, Db4objects.Db4o.Internal.Buffer writer
			, Slot slot)
		{
			if (DTrace.enabled)
			{
				DTrace.YAPMETA_WRITE.Log(GetID());
			}
			LocalObjectContainer container = (LocalObjectContainer)trans.Container();
			WriteThis(trans, writer);
			container.WriteEncrypt(writer, slot.Address(), 0);
			if (IsActive())
			{
				SetStateClean();
			}
		}

		public virtual bool WriteObjectBegin()
		{
			if (IsDirty())
			{
				return BeginProcessing();
			}
			return false;
		}

		public virtual void WriteOwnID(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 writer)
		{
			Write(trans);
			writer.WriteInt(GetID());
		}

		public override int GetHashCode()
		{
			if (IsNew())
			{
				throw new InvalidOperationException();
			}
			return GetID();
		}

		public abstract byte GetIdentifier();

		public abstract int OwnLength();

		public abstract void ReadThis(Transaction arg1, Db4objects.Db4o.Internal.Buffer arg2
			);

		public abstract void WriteThis(Transaction arg1, Db4objects.Db4o.Internal.Buffer 
			arg2);
	}
}
