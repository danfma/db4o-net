/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal;
using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal
{
	public sealed class ClientTransaction : Transaction
	{
		private readonly ClientObjectContainer i_client;

		protected Tree i_yapObjectsToGc;

		internal ClientTransaction(ClientObjectContainer container, Transaction parentTransaction
			, TransactionalReferenceSystem referenceSystem) : base(container, parentTransaction
			, referenceSystem)
		{
			i_client = container;
		}

		public override void Commit()
		{
			CommitTransactionListeners();
			ClearAll();
			if (IsSystemTransaction())
			{
				i_client.Write(Msg.CommitSystemtrans);
			}
			else
			{
				i_client.Write(Msg.Commit);
				i_client.ExpectedResponse(Msg.Ok);
			}
		}

		protected override void Clear()
		{
			RemoveYapObjectReferences();
		}

		private void RemoveYapObjectReferences()
		{
			if (i_yapObjectsToGc != null)
			{
				i_yapObjectsToGc.Traverse(new _IVisitor4_37(this));
			}
			i_yapObjectsToGc = null;
		}

		private sealed class _IVisitor4_37 : IVisitor4
		{
			public _IVisitor4_37(ClientTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				ObjectReference yo = (ObjectReference)((TreeIntObject)a_object)._object;
				this._enclosing.RemoveReference(yo);
			}

			private readonly ClientTransaction _enclosing;
		}

		public override bool Delete(ObjectReference @ref, int id, int cascade)
		{
			if (!base.Delete(@ref, id, cascade))
			{
				return false;
			}
			MsgD msg = Msg.TaDelete.GetWriterForInts(this, new int[] { id, cascade });
			i_client.WriteBatchedMessage(msg);
			return true;
		}

		public override bool IsDeleted(int a_id)
		{
			// This one really is a hack.
			// It only helps to get information about the current
			// transaction.
			// We need a better strategy for C/S concurrency behaviour.
			MsgD msg = Msg.TaIsDeleted.GetWriterForInt(this, a_id);
			i_client.Write(msg);
			int res = i_client.ExpectedByteResponse(Msg.TaIsDeleted).ReadInt();
			return res == 1;
		}

		public sealed override HardObjectReference GetHardReferenceBySignature(long a_uuid
			, byte[] a_signature)
		{
			int messageLength = Const4.LongLength + Const4.IntLength + a_signature.Length;
			MsgD message = Msg.ObjectByUuid.GetWriterForLength(this, messageLength);
			message.WriteLong(a_uuid);
			message.WriteInt(a_signature.Length);
			message.WriteBytes(a_signature);
			i_client.Write(message);
			message = (MsgD)i_client.ExpectedResponse(Msg.ObjectByUuid);
			int id = message.ReadInt();
			if (id > 0)
			{
				return Container().GetHardObjectReferenceById(this, id);
			}
			return HardObjectReference.Invalid;
		}

		public override void ProcessDeletes()
		{
			IVisitor4 deleteVisitor = new _IVisitor4_85(this);
			TraverseDelete(deleteVisitor);
			i_client.WriteBatchedMessage(Msg.ProcessDeletes);
		}

		private sealed class _IVisitor4_85 : IVisitor4
		{
			public _IVisitor4_85(ClientTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				DeleteInfo info = (DeleteInfo)a_object;
				if (info._reference != null)
				{
					this._enclosing.i_yapObjectsToGc = Tree.Add(this._enclosing.i_yapObjectsToGc, new 
						TreeIntObject(info._key, info._reference));
				}
			}

			private readonly ClientTransaction _enclosing;
		}

		public override void Rollback()
		{
			i_yapObjectsToGc = null;
			RollBackTransactionListeners();
			ClearAll();
		}

		public override void WriteUpdateAdjustIndexes(int id, ClassMetadata classMetadata
			, ArrayType arrayType, int cascade)
		{
		}
		// do nothing
	}
}