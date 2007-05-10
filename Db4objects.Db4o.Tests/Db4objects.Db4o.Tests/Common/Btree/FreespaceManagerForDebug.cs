using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Btree;

namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class FreespaceManagerForDebug : AbstractFreespaceManager
	{
		private readonly ISlotListener _listener;

		public FreespaceManagerForDebug(LocalObjectContainer file, ISlotListener listener
			) : base(file)
		{
			_listener = listener;
		}

		public override void BeginCommit()
		{
		}

		public override void Commit()
		{
		}

		public override void EndCommit()
		{
		}

		public override int SlotCount()
		{
			return 0;
		}

		public override void Free(Slot slot)
		{
			_listener.OnFree(slot);
		}

		public override void FreeSelf()
		{
		}

		public override Slot GetSlot(int length)
		{
			return null;
		}

		public override int OnNew(LocalObjectContainer file)
		{
			return 0;
		}

		public override void Read(int freeSlotsID)
		{
		}

		public override void Start(int slotAddress)
		{
		}

		public override byte SystemType()
		{
			return FM_DEBUG;
		}

		public override void Traverse(IVisitor4 visitor)
		{
		}

		public override int Write()
		{
			return 0;
		}
	}
}
