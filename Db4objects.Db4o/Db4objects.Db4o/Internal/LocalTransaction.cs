/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Caching;
using Db4objects.Db4o.Internal.Callbacks;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Internal.Transactionlog;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class LocalTransaction : Db4objects.Db4o.Internal.Transaction
	{
		private readonly byte[] _pointerBuffer = new byte[Const4.PointerLength];

		protected readonly StatefulBuffer i_pointerIo;

		private readonly IdentitySet4 _participants = new IdentitySet4();

		private readonly LockedTree _slotChanges = new LockedTree();

		internal Tree _writtenUpdateAdjustedIndexes;

		protected readonly LocalObjectContainer _file;

		private readonly ICommittedCallbackDispatcher _committedCallbackDispatcher;

		private readonly ICache4 _slotCache;

		private TransactionLogHandler _transactionLogHandler = new EmbeddedTransactionLogHandler
			();

		public LocalTransaction(ObjectContainerBase container, Db4objects.Db4o.Internal.Transaction
			 parentTransaction, TransactionalReferenceSystem referenceSystem) : base(container
			, parentTransaction, referenceSystem)
		{
			_file = (LocalObjectContainer)container;
			i_pointerIo = new StatefulBuffer(this, Const4.PointerLength);
			_committedCallbackDispatcher = new _ICommittedCallbackDispatcher_42(this);
			_slotCache = CreateSlotCache();
			InitializeTransactionLogHandler();
		}

		private sealed class _ICommittedCallbackDispatcher_42 : ICommittedCallbackDispatcher
		{
			public _ICommittedCallbackDispatcher_42(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool WillDispatchCommitted()
			{
				return this._enclosing.Callbacks().CaresAboutCommitted();
			}

			public void DispatchCommitted(CallbackObjectInfoCollections committedInfo)
			{
				this._enclosing.Callbacks().CommitOnCompleted(this._enclosing, committedInfo);
			}

			private readonly LocalTransaction _enclosing;
		}

		private ICache4 CreateSlotCache()
		{
			if (IsSystemTransaction())
			{
				int slotCacheSize = Config().SlotCacheSize();
				if (slotCacheSize > 0)
				{
					return CacheFactory.New2QCache(slotCacheSize);
				}
			}
			return new NullCache4();
		}

		private void InitializeTransactionLogHandler()
		{
			if (!IsSystemTransaction())
			{
				_transactionLogHandler = ((LocalTransaction)SystemTransaction())._transactionLogHandler;
				return;
			}
			bool fileBased = Config().FileBasedTransactionLog() && Container() is IoAdaptedObjectContainer;
			if (!fileBased)
			{
				_transactionLogHandler = new EmbeddedTransactionLogHandler();
				return;
			}
			string fileName = ((IoAdaptedObjectContainer)Container()).FileName();
			_transactionLogHandler = new FileBasedTransactionLogHandler(this, fileName);
		}

		public virtual Config4Impl Config()
		{
			return Container().Config();
		}

		public virtual LocalObjectContainer File()
		{
			return _file;
		}

		public override void Commit()
		{
			Commit(_committedCallbackDispatcher);
		}

		public virtual void Commit(ICommittedCallbackDispatcher dispatcher)
		{
			lock (Container().Lock())
			{
				DispatchCommittingCallback();
				if (!DoCommittedCallbacks(dispatcher))
				{
					CommitListeners();
					CommitImpl();
					CommitClearAll();
				}
				else
				{
					CommitListeners();
					Collection4 deleted = CollectCommittedCallbackDeletedInfo();
					CommitImpl();
					CallbackObjectInfoCollections committedInfo = CollectCommittedCallbackInfo(deleted
						);
					CommitClearAll();
					dispatcher.DispatchCommitted(CallbackObjectInfoCollections.Emtpy == committedInfo
						 ? committedInfo : new CallbackObjectInfoCollections(committedInfo.added, committedInfo
						.updated, new ObjectInfoCollectionImpl(deleted)));
				}
			}
		}

		private void DispatchCommittingCallback()
		{
			if (DoCommittingCallbacks())
			{
				Callbacks().CommitOnStarted(this, CollectCommittingCallbackInfo());
			}
		}

		private bool DoCommittedCallbacks(ICommittedCallbackDispatcher dispatcher)
		{
			if (IsSystemTransaction())
			{
				return false;
			}
			return dispatcher.WillDispatchCommitted();
		}

		private bool DoCommittingCallbacks()
		{
			if (IsSystemTransaction())
			{
				return false;
			}
			return Callbacks().CaresAboutCommitting();
		}

		public virtual void Enlist(ITransactionParticipant participant)
		{
			if (null == participant)
			{
				throw new ArgumentNullException();
			}
			CheckSynchronization();
			if (!_participants.Contains(participant))
			{
				_participants.Add(participant);
			}
		}

		private void CommitImpl()
		{
			if (DTrace.enabled)
			{
				DTrace.TransCommit.LogInfo("server == " + Container().IsServer() + ", systemtrans == "
					 + IsSystemTransaction());
			}
			Commit3Stream();
			CommitParticipants();
			Container().WriteDirty();
			Slot reservedSlot = _transactionLogHandler.AllocateSlot(this, false);
			FreeSlotChanges(false);
			FreespaceBeginCommit();
			CommitFreespace();
			FreeSlotChanges(true);
			_transactionLogHandler.ApplySlotChanges(this, reservedSlot);
			FreespaceEndCommit();
		}

		public void FreeSlotChanges(bool forFreespace)
		{
			IVisitor4 visitor = new _IVisitor4_174(this, forFreespace);
			if (IsSystemTransaction())
			{
				_slotChanges.TraverseMutable(visitor);
				return;
			}
			_slotChanges.TraverseLocked(visitor);
			if (_systemTransaction != null)
			{
				ParentLocalTransaction().FreeSlotChanges(forFreespace);
			}
		}

		private sealed class _IVisitor4_174 : IVisitor4
		{
			public _IVisitor4_174(LocalTransaction _enclosing, bool forFreespace)
			{
				this._enclosing = _enclosing;
				this.forFreespace = forFreespace;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).FreeDuringCommit(this._enclosing._file, forFreespace);
			}

			private readonly LocalTransaction _enclosing;

			private readonly bool forFreespace;
		}

		private void CommitListeners()
		{
			CommitParentListeners();
			CommitTransactionListeners();
		}

		private void CommitParentListeners()
		{
			if (_systemTransaction != null)
			{
				ParentLocalTransaction().CommitListeners();
			}
		}

		private void CommitParticipants()
		{
			if (ParentLocalTransaction() != null)
			{
				ParentLocalTransaction().CommitParticipants();
			}
			IEnumerator iterator = _participants.GetEnumerator();
			while (iterator.MoveNext())
			{
				((ITransactionParticipant)iterator.Current).Commit(this);
			}
		}

		private void Commit3Stream()
		{
			Container().ProcessPendingClassUpdates();
			Container().WriteDirty();
			Container().ClassCollection().Write(Container().SystemTransaction());
		}

		private LocalTransaction ParentLocalTransaction()
		{
			return (LocalTransaction)_systemTransaction;
		}

		private void CommitClearAll()
		{
			if (_systemTransaction != null)
			{
				ParentLocalTransaction().CommitClearAll();
			}
			ClearAll();
		}

		protected override void Clear()
		{
			_slotChanges.Clear();
			DisposeParticipants();
			_participants.Clear();
		}

		private void DisposeParticipants()
		{
			IEnumerator iterator = _participants.ValuesIterator();
			while (iterator.MoveNext())
			{
				((ITransactionParticipant)iterator.Current).Dispose(this);
			}
		}

		public override void Rollback()
		{
			lock (Container().Lock())
			{
				RollbackParticipants();
				RollbackSlotChanges();
				RollBackTransactionListeners();
				ClearAll();
			}
		}

		private void RollbackParticipants()
		{
			IEnumerator iterator = _participants.ValuesIterator();
			while (iterator.MoveNext())
			{
				((ITransactionParticipant)iterator.Current).Rollback(this);
			}
		}

		protected virtual void RollbackSlotChanges()
		{
			_slotChanges.TraverseLocked(new _IVisitor4_263(this));
		}

		private sealed class _IVisitor4_263 : IVisitor4
		{
			public _IVisitor4_263(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((SlotChange)a_object).Rollback(this._enclosing._file);
			}

			private readonly LocalTransaction _enclosing;
		}

		public override bool IsDeleted(int id)
		{
			return SlotChangeIsFlaggedDeleted(id);
		}

		public virtual void WriteZeroPointer(int id)
		{
			WritePointer(id, Slot.Zero);
		}

		public virtual void WritePointer(Pointer4 pointer)
		{
			WritePointer(pointer._id, pointer._slot);
		}

		public virtual void WritePointer(int id, Slot slot)
		{
			if (DTrace.enabled)
			{
				DTrace.WritePointer.Log(id);
				DTrace.WritePointer.LogLength(slot);
			}
			CheckSynchronization();
			i_pointerIo.UseSlot(id);
			i_pointerIo.WriteInt(slot.Address());
			i_pointerIo.WriteInt(slot.Length());
			if (Debug4.xbytes && Deploy.overwrite)
			{
				i_pointerIo.SetID(Const4.IgnoreId);
			}
			i_pointerIo.Write();
		}

		public virtual bool WriteSlots()
		{
			BooleanByRef ret = new BooleanByRef();
			TraverseSlotChanges(new _IVisitor4_305(this, ret));
			return ret.value;
		}

		private sealed class _IVisitor4_305 : IVisitor4
		{
			public _IVisitor4_305(LocalTransaction _enclosing, BooleanByRef ret)
			{
				this._enclosing = _enclosing;
				this.ret = ret;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).WritePointer(this._enclosing);
				ret.value = true;
			}

			private readonly LocalTransaction _enclosing;

			private readonly BooleanByRef ret;
		}

		public virtual void FlushFile()
		{
			if (DTrace.enabled)
			{
				DTrace.TransFlush.Log();
			}
			_file.SyncFiles();
		}

		private SlotChange ProduceSlotChange(int id)
		{
			if (DTrace.enabled)
			{
				DTrace.ProduceSlotChange.Log(id);
			}
			SlotChange slot = new SlotChange(id);
			_slotChanges.Add(slot);
			return (SlotChange)slot.AddedOrExisting();
		}

		public SlotChange FindSlotChange(int a_id)
		{
			CheckSynchronization();
			return (SlotChange)_slotChanges.Find(a_id);
		}

		public virtual Slot GetCurrentSlotOfID(int id)
		{
			CheckSynchronization();
			if (id == 0)
			{
				return null;
			}
			SlotChange change = FindSlotChange(id);
			if (change != null)
			{
				if (change.IsSetPointer())
				{
					return change.NewSlot();
				}
			}
			if (_systemTransaction != null)
			{
				Slot parentSlot = ParentLocalTransaction().GetCurrentSlotOfID(id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return ReadPointer(id)._slot;
		}

		public virtual Slot GetCommittedSlotOfID(int id)
		{
			if (id == 0)
			{
				return null;
			}
			SlotChange change = FindSlotChange(id);
			if (change != null)
			{
				Slot slot = change.OldSlot();
				if (slot != null)
				{
					return slot;
				}
			}
			if (_systemTransaction != null)
			{
				Slot parentSlot = ParentLocalTransaction().GetCommittedSlotOfID(id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return ReadPointer(id)._slot;
		}

		public virtual Pointer4 ReadPointer(int id)
		{
			if (!IsValidId(id))
			{
				throw new InvalidIDException(id);
			}
			_file.ReadBytes(_pointerBuffer, id, Const4.PointerLength);
			int address = (_pointerBuffer[3] & 255) | (_pointerBuffer[2] & 255) << 8 | (_pointerBuffer
				[1] & 255) << 16 | _pointerBuffer[0] << 24;
			int length = (_pointerBuffer[7] & 255) | (_pointerBuffer[6] & 255) << 8 | (_pointerBuffer
				[5] & 255) << 16 | _pointerBuffer[4] << 24;
			if (!IsValidSlot(address, length))
			{
				throw new InvalidSlotException(address, length, id);
			}
			return new Pointer4(id, new Slot(address, length));
		}

		private bool IsValidId(int id)
		{
			return _file.FileLength() >= id;
		}

		private bool IsValidSlot(int address, int length)
		{
			// just in case overflow 
			long fileLength = _file.FileLength();
			bool validAddress = fileLength >= address;
			bool validLength = fileLength >= length;
			bool validSlot = fileLength >= (address + length);
			return validAddress && validLength && validSlot;
		}

		private Pointer4 DebugReadPointer(int id)
		{
			return null;
		}

		public override void SetPointer(int a_id, Slot slot)
		{
			if (DTrace.enabled)
			{
				DTrace.SlotSetPointer.Log(a_id);
				DTrace.SlotSetPointer.LogLength(slot);
			}
			CheckSynchronization();
			ProduceSlotChange(a_id).SetPointer(slot);
		}

		private bool SlotChangeIsFlaggedDeleted(int id)
		{
			SlotChange slot = FindSlotChange(id);
			if (slot != null)
			{
				return slot.IsDeleted();
			}
			if (_systemTransaction != null)
			{
				return ParentLocalTransaction().SlotChangeIsFlaggedDeleted(id);
			}
			return false;
		}

		internal void CompleteInterruptedTransaction()
		{
			lock (Container().Lock())
			{
				_transactionLogHandler.CompleteInterruptedTransaction(this);
			}
		}

		public virtual void TraverseSlotChanges(IVisitor4 visitor)
		{
			if (_systemTransaction != null)
			{
				ParentLocalTransaction().TraverseSlotChanges(visitor);
			}
			_slotChanges.TraverseLocked(visitor);
		}

		public override void SlotDelete(int id, Slot slot)
		{
			CheckSynchronization();
			if (DTrace.enabled)
			{
				DTrace.SlotDelete.Log(id);
				DTrace.SlotDelete.LogLength(slot);
			}
			if (id == 0)
			{
				return;
			}
			SlotChange slotChange = ProduceSlotChange(id);
			slotChange.FreeOnCommit(_file, slot);
			slotChange.SetPointer(Slot.Zero);
		}

		public override void SlotFreeOnCommit(int id, Slot slot)
		{
			CheckSynchronization();
			if (DTrace.enabled)
			{
				DTrace.SlotFreeOnCommit.Log(id);
				DTrace.SlotFreeOnCommit.LogLength(slot);
			}
			if (id == 0)
			{
				return;
			}
			ProduceSlotChange(id).FreeOnCommit(_file, slot);
		}

		public override void SlotFreeOnRollback(int id, Slot slot)
		{
			CheckSynchronization();
			if (DTrace.enabled)
			{
				DTrace.SlotFreeOnRollbackId.Log(id);
				DTrace.SlotFreeOnRollbackAddress.LogLength(slot);
			}
			ProduceSlotChange(id).FreeOnRollback(slot);
		}

		internal override void SlotFreeOnRollbackCommitSetPointer(int id, Slot newSlot, bool
			 forFreespace)
		{
			Slot oldSlot = GetCurrentSlotOfID(id);
			if (oldSlot == null)
			{
				return;
			}
			CheckSynchronization();
			if (DTrace.enabled)
			{
				DTrace.FreeOnRollback.Log(id);
				DTrace.FreeOnRollback.LogLength(newSlot);
				DTrace.FreeOnCommit.Log(id);
				DTrace.FreeOnCommit.LogLength(oldSlot);
			}
			SlotChange change = ProduceSlotChange(id);
			change.FreeOnRollbackSetPointer(newSlot);
			change.FreeOnCommit(_file, oldSlot);
			change.ForFreespace(forFreespace);
		}

		internal override void ProduceUpdateSlotChange(int id, Slot slot)
		{
			CheckSynchronization();
			if (DTrace.enabled)
			{
				DTrace.FreeOnRollback.Log(id);
				DTrace.FreeOnRollback.LogLength(slot);
			}
			SlotChange slotChange = ProduceSlotChange(id);
			slotChange.FreeOnRollbackSetPointer(slot);
		}

		public override void SlotFreePointerOnCommit(int a_id)
		{
			CheckSynchronization();
			Slot slot = GetCurrentSlotOfID(a_id);
			if (slot == null)
			{
				return;
			}
			// FIXME: From looking at this it should call slotFreePointerOnCommit
			//        Write a test case and check.
			//        Looking at references, this method is only called from freed
			//        BTree nodes. Indeed it should be checked what happens here.
			SlotFreeOnCommit(a_id, slot);
		}

		internal override void SlotFreePointerOnCommit(int a_id, Slot slot)
		{
			CheckSynchronization();
			SlotFreeOnCommit(slot.Address(), slot);
			// FIXME: This does not look nice
			SlotFreeOnCommit(a_id, slot);
		}

		// FIXME: It should rather work like this:
		// produceSlotChange(a_id).freePointerOnCommit();
		public override void SlotFreePointerOnRollback(int id)
		{
			ProduceSlotChange(id).FreePointerOnRollback();
		}

		public override void ProcessDeletes()
		{
			if (_delete == null)
			{
				_writtenUpdateAdjustedIndexes = null;
				return;
			}
			while (_delete != null)
			{
				Tree delete = _delete;
				_delete = null;
				delete.Traverse(new _IVisitor4_572(this));
			}
			// if the object has been deleted
			// We need to hold a hard reference here, otherwise we can get 
			// intermediate garbage collection kicking in.
			// This means the object was gc'd.
			// Let's try to read it again, but this may fail in
			// CS mode if another transaction has deleted it. 
			_writtenUpdateAdjustedIndexes = null;
		}

		private sealed class _IVisitor4_572 : IVisitor4
		{
			public _IVisitor4_572(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				DeleteInfo info = (DeleteInfo)a_object;
				if (this._enclosing.IsDeleted(info._key))
				{
					return;
				}
				object obj = null;
				if (info._reference != null)
				{
					obj = info._reference.GetObject();
				}
				if (obj == null || info._reference.GetID() < 0)
				{
					HardObjectReference hardRef = this._enclosing.Container().GetHardObjectReferenceById
						(this._enclosing, info._key);
					if (hardRef == HardObjectReference.Invalid)
					{
						return;
					}
					info._reference = hardRef._reference;
					info._reference.FlagForDelete(this._enclosing.Container().TopLevelCallId());
					obj = info._reference.GetObject();
				}
				this._enclosing.Container().Delete3(this._enclosing, info._reference, obj, info._cascade
					, false);
			}

			private readonly LocalTransaction _enclosing;
		}

		public override void WriteUpdateAdjustIndexes(int id, ClassMetadata clazz, ArrayType
			 typeInfo, int cascade)
		{
			new WriteUpdateProcessor(this, id, clazz, typeInfo, cascade).Run();
		}

		private ICallbacks Callbacks()
		{
			return Container().Callbacks();
		}

		private Collection4 CollectCommittedCallbackDeletedInfo()
		{
			Collection4 deleted = new Collection4();
			CollectSlotChanges(new _ISlotChangeCollector_622(this, deleted));
			return deleted;
		}

		private sealed class _ISlotChangeCollector_622 : ISlotChangeCollector
		{
			public _ISlotChangeCollector_622(LocalTransaction _enclosing, Collection4 deleted
				)
			{
				this._enclosing = _enclosing;
				this.deleted = deleted;
			}

			public void Deleted(int id)
			{
				deleted.Add(this._enclosing.FrozenReferenceFor(id));
			}

			public void Updated(int id)
			{
			}

			public void Added(int id)
			{
			}

			private readonly LocalTransaction _enclosing;

			private readonly Collection4 deleted;
		}

		private CallbackObjectInfoCollections CollectCommittedCallbackInfo(Collection4 deleted
			)
		{
			if (null == _slotChanges)
			{
				return CallbackObjectInfoCollections.Emtpy;
			}
			Collection4 added = new Collection4();
			Collection4 updated = new Collection4();
			CollectSlotChanges(new _ISlotChangeCollector_643(this, added, updated));
			return NewCallbackObjectInfoCollections(added, updated, deleted);
		}

		private sealed class _ISlotChangeCollector_643 : ISlotChangeCollector
		{
			public _ISlotChangeCollector_643(LocalTransaction _enclosing, Collection4 added, 
				Collection4 updated)
			{
				this._enclosing = _enclosing;
				this.added = added;
				this.updated = updated;
			}

			public void Added(int id)
			{
				added.Add(this._enclosing.LazyReferenceFor(id));
			}

			public void Updated(int id)
			{
				updated.Add(this._enclosing.LazyReferenceFor(id));
			}

			public void Deleted(int id)
			{
			}

			private readonly LocalTransaction _enclosing;

			private readonly Collection4 added;

			private readonly Collection4 updated;
		}

		private CallbackObjectInfoCollections CollectCommittingCallbackInfo()
		{
			if (null == _slotChanges)
			{
				return CallbackObjectInfoCollections.Emtpy;
			}
			Collection4 added = new Collection4();
			Collection4 deleted = new Collection4();
			Collection4 updated = new Collection4();
			CollectSlotChanges(new _ISlotChangeCollector_666(this, added, updated, deleted));
			return NewCallbackObjectInfoCollections(added, updated, deleted);
		}

		private sealed class _ISlotChangeCollector_666 : ISlotChangeCollector
		{
			public _ISlotChangeCollector_666(LocalTransaction _enclosing, Collection4 added, 
				Collection4 updated, Collection4 deleted)
			{
				this._enclosing = _enclosing;
				this.added = added;
				this.updated = updated;
				this.deleted = deleted;
			}

			public void Added(int id)
			{
				added.Add(this._enclosing.LazyReferenceFor(id));
			}

			public void Updated(int id)
			{
				updated.Add(this._enclosing.LazyReferenceFor(id));
			}

			public void Deleted(int id)
			{
				deleted.Add(this._enclosing.FrozenReferenceFor(id));
			}

			private readonly LocalTransaction _enclosing;

			private readonly Collection4 added;

			private readonly Collection4 updated;

			private readonly Collection4 deleted;
		}

		private CallbackObjectInfoCollections NewCallbackObjectInfoCollections(Collection4
			 added, Collection4 updated, Collection4 deleted)
		{
			return new CallbackObjectInfoCollections(new ObjectInfoCollectionImpl(added), new 
				ObjectInfoCollectionImpl(updated), new ObjectInfoCollectionImpl(deleted));
		}

		private void CollectSlotChanges(ISlotChangeCollector collector)
		{
			if (null == _slotChanges)
			{
				return;
			}
			_slotChanges.TraverseLocked(new _IVisitor4_696(collector));
		}

		private sealed class _IVisitor4_696 : IVisitor4
		{
			public _IVisitor4_696(ISlotChangeCollector collector)
			{
				this.collector = collector;
			}

			public void Visit(object obj)
			{
				SlotChange slotChange = ((SlotChange)obj);
				int id = slotChange._key;
				if (slotChange.IsDeleted())
				{
					collector.Deleted(id);
				}
				else
				{
					if (slotChange.IsNew())
					{
						collector.Added(id);
					}
					else
					{
						collector.Updated(id);
					}
				}
			}

			private readonly ISlotChangeCollector collector;
		}

		private IObjectInfo FrozenReferenceFor(int id)
		{
			return new FrozenObjectInfo(this, ReferenceForId(id));
		}

		public static Db4objects.Db4o.Internal.Transaction ReadInterruptedTransaction(LocalObjectContainer
			 file, ByteArrayBuffer reader)
		{
			LocalTransaction transaction = (LocalTransaction)file.NewTransaction(null, null);
			if (transaction.WasInterrupted(reader))
			{
				return transaction;
			}
			return null;
		}

		public virtual bool WasInterrupted(ByteArrayBuffer reader)
		{
			return _transactionLogHandler.CheckForInterruptedTransaction(this, reader);
		}

		public virtual IFreespaceManager FreespaceManager()
		{
			return _file.FreespaceManager();
		}

		private void FreespaceBeginCommit()
		{
			if (FreespaceManager() == null)
			{
				return;
			}
			FreespaceManager().BeginCommit();
		}

		private void FreespaceEndCommit()
		{
			if (FreespaceManager() == null)
			{
				return;
			}
			FreespaceManager().EndCommit();
		}

		private void CommitFreespace()
		{
			if (FreespaceManager() == null)
			{
				return;
			}
			FreespaceManager().Commit();
		}

		private LazyObjectReference LazyReferenceFor(int id)
		{
			return new LazyObjectReference(this, id);
		}

		public virtual ICache4 SlotCache()
		{
			return _slotCache;
		}

		public virtual void ReadSlotChanges(ByteArrayBuffer buffer)
		{
			_slotChanges.Read(buffer, new SlotChange(0));
		}

		public virtual void Close()
		{
			_transactionLogHandler.Close();
		}
	}
}
