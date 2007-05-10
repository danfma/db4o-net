using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Slots
{
	/// <exclude></exclude>
	public class Slot
	{
		private readonly int _address;

		private readonly int _length;

		public static readonly Db4objects.Db4o.Internal.Slots.Slot ZERO = new Db4objects.Db4o.Internal.Slots.Slot
			(0, 0);

		public Slot(int address, int length)
		{
			_address = address;
			_length = length;
		}

		public virtual int Address()
		{
			return _address;
		}

		public virtual int Length()
		{
			return _length;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (!(obj is Db4objects.Db4o.Internal.Slots.Slot))
			{
				return false;
			}
			Db4objects.Db4o.Internal.Slots.Slot other = (Db4objects.Db4o.Internal.Slots.Slot)
				obj;
			return (_address == other._address) && (Length() == other.Length());
		}

		public override int GetHashCode()
		{
			return _address ^ Length();
		}

		public virtual Db4objects.Db4o.Internal.Slots.Slot SubSlot(int offset)
		{
			return new Db4objects.Db4o.Internal.Slots.Slot(_address + offset, Length() - offset
				);
		}

		public override string ToString()
		{
			return "[A:" + _address + ",L:" + Length() + "]";
		}

		public virtual Db4objects.Db4o.Internal.Slots.Slot Truncate(int requiredLength)
		{
			return new Db4objects.Db4o.Internal.Slots.Slot(_address, requiredLength);
		}

		public static int MARSHALLED_LENGTH = Const4.INT_LENGTH * 2;

		public virtual int CompareByAddress(Db4objects.Db4o.Internal.Slots.Slot slot)
		{
			return slot._address - _address;
		}

		public virtual int CompareByLength(Db4objects.Db4o.Internal.Slots.Slot slot)
		{
			int res = slot.Length() - Length();
			if (res != 0)
			{
				return res;
			}
			return CompareByAddress(slot);
		}

		public virtual bool IsDirectlyPreceding(Db4objects.Db4o.Internal.Slots.Slot other
			)
		{
			return _address + Length() == other._address;
		}

		public virtual Db4objects.Db4o.Internal.Slots.Slot Append(Db4objects.Db4o.Internal.Slots.Slot
			 slot)
		{
			return new Db4objects.Db4o.Internal.Slots.Slot(Address(), _length + slot.Length()
				);
		}
	}
}
