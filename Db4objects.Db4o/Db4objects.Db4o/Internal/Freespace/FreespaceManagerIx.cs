using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.IX;

namespace Db4objects.Db4o.Internal.Freespace
{
	public class FreespaceManagerIx : FreespaceManager
	{
		private int _slotAddress;

		private FreespaceIxAddress _addressIx;

		private FreespaceIxLength _lengthIx;

		private bool _started;

		private Collection4 _xBytes;

		private readonly bool _overwriteDeletedSlots;

		internal FreespaceManagerIx(LocalObjectContainer file) : base(file)
		{
			_overwriteDeletedSlots = Db4objects.Db4o.Debug.xbytes || file.Config().FreespaceFiller
				() != null;
		}

		private void Add(int address, int length)
		{
			_addressIx.Add(address, length);
			_lengthIx.Add(address, length);
		}

		public override void BeginCommit()
		{
			if (!Started())
			{
				return;
			}
			SlotEntryToZeroes(_file, _slotAddress);
		}

		public override void Debug()
		{
		}

		public override void EndCommit()
		{
			if (!Started())
			{
				return;
			}
			if (_overwriteDeletedSlots)
			{
				_xBytes = new Collection4();
			}
			_addressIx._index.CommitFreeSpace(_lengthIx._index);
			StatefulBuffer writer = new StatefulBuffer(_file.SystemTransaction(), _slotAddress
				, SlotLength());
			_addressIx._index._metaIndex.Write(writer);
			_lengthIx._index._metaIndex.Write(writer);
			if (_overwriteDeletedSlots)
			{
				writer.SetID(Const4.IGNORE_ID);
			}
			if (_file.ConfigImpl().FlushFileBuffers())
			{
				_file.SyncFiles();
			}
			writer.WriteEncrypt();
			if (_overwriteDeletedSlots)
			{
				IEnumerator i = _xBytes.GetEnumerator();
				_xBytes = null;
				while (i.MoveNext())
				{
					int[] addressLength = (int[])i.Current;
					OverwriteDeletedSlots(addressLength[0], addressLength[1]);
				}
			}
		}

		public override int EntryCount()
		{
			return _addressIx.EntryCount();
		}

		public override void Free(int address, int length)
		{
			if (!Started())
			{
				return;
			}
			if (address <= 0)
			{
				return;
			}
			if (length <= DiscardLimit())
			{
				return;
			}
			length = _file.BlocksFor(length);
			int freedAddress = address;
			int freedLength = length;
			_addressIx.Find(address);
			if (_addressIx.Preceding())
			{
				if (_addressIx.Address() + _addressIx.Length() == address)
				{
					Remove(_addressIx.Address(), _addressIx.Length());
					address = _addressIx.Address();
					length += _addressIx.Length();
					_addressIx.Find(freedAddress);
				}
			}
			if (_addressIx.Subsequent())
			{
				if (freedAddress + freedLength == _addressIx.Address())
				{
					Remove(_addressIx.Address(), _addressIx.Length());
					length += _addressIx.Length();
				}
			}
			Add(address, length);
			if (_overwriteDeletedSlots)
			{
				OverwriteDeletedSlots(freedAddress, freedLength);
			}
		}

		public override void FreeSelf()
		{
			if (!Started())
			{
				return;
			}
			_addressIx._index._metaIndex.Free(_file);
			_lengthIx._index._metaIndex.Free(_file);
		}

		public override int FreeSize()
		{
			return _addressIx.FreeSize();
		}

		public override int GetSlot(int length)
		{
			if (!Started())
			{
				return 0;
			}
			int address = GetSlot1(length);
			return address;
		}

		private int GetSlot1(int length)
		{
			if (!Started())
			{
				return 0;
			}
			length = _file.BlocksFor(length);
			_lengthIx.Find(length);
			if (_lengthIx.Match())
			{
				Remove(_lengthIx.Address(), _lengthIx.Length());
				return _lengthIx.Address();
			}
			if (_lengthIx.Subsequent())
			{
				int lengthRemainder = _lengthIx.Length() - length;
				int addressRemainder = _lengthIx.Address() + length;
				Remove(_lengthIx.Address(), _lengthIx.Length());
				Add(addressRemainder, lengthRemainder);
				return _lengthIx.Address();
			}
			return 0;
		}

		public override void Migrate(FreespaceManager newFM)
		{
			if (!Started())
			{
				return;
			}
			IIntObjectVisitor addToNewFM = new _AnonymousInnerClass195(this, newFM);
			Tree.Traverse(_addressIx._indexTrans.GetRoot(), new _AnonymousInnerClass200(this, 
				addToNewFM));
		}

		private sealed class _AnonymousInnerClass195 : IIntObjectVisitor
		{
			public _AnonymousInnerClass195(FreespaceManagerIx _enclosing, FreespaceManager newFM
				)
			{
				this._enclosing = _enclosing;
				this.newFM = newFM;
			}

			public void Visit(int length, object address)
			{
				newFM.Free(((int)address), length);
			}

			private readonly FreespaceManagerIx _enclosing;

			private readonly FreespaceManager newFM;
		}

		private sealed class _AnonymousInnerClass200 : IVisitor4
		{
			public _AnonymousInnerClass200(FreespaceManagerIx _enclosing, IIntObjectVisitor addToNewFM
				)
			{
				this._enclosing = _enclosing;
				this.addToNewFM = addToNewFM;
			}

			public void Visit(object a_object)
			{
				IxTree ixTree = (IxTree)a_object;
				ixTree.VisitAll(addToNewFM);
			}

			private readonly FreespaceManagerIx _enclosing;

			private readonly IIntObjectVisitor addToNewFM;
		}

		public override void OnNew(LocalObjectContainer file)
		{
			file.EnsureFreespaceSlot();
		}

		public override void Read(int freespaceID)
		{
		}

		private void Remove(int address, int length)
		{
			_addressIx.Remove(address, length);
			_lengthIx.Remove(address, length);
		}

		public override void Start(int slotAddress)
		{
			if (Started())
			{
				return;
			}
			_slotAddress = slotAddress;
			MetaIndex miAddress = new MetaIndex();
			MetaIndex miLength = new MetaIndex();
			Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(SlotLength
				());
			reader.Read(_file, slotAddress, 0);
			miAddress.Read(reader);
			miLength.Read(reader);
			_addressIx = new FreespaceIxAddress(_file, miAddress);
			_lengthIx = new FreespaceIxLength(_file, miLength);
			_started = true;
		}

		private bool Started()
		{
			return _started;
		}

		public override byte SystemType()
		{
			return FM_IX;
		}

		public override int Shutdown()
		{
			return 0;
		}

		private void OverwriteDeletedSlots(int address, int length)
		{
			if (_overwriteDeletedSlots)
			{
				if (_xBytes == null)
				{
					length = length * BlockSize();
					_file.OverwriteDeletedBytes(address, length);
				}
				else
				{
					_xBytes.Add(new int[] { address, length });
				}
			}
		}
	}
}
