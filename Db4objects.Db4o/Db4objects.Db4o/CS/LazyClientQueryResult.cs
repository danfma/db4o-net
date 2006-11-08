namespace Db4objects.Db4o.CS
{
	/// <exclude></exclude>
	public class LazyClientQueryResult : Db4objects.Db4o.Inside.Query.AbstractQueryResult
	{
		private const int SIZE_NOT_SET = -1;

		private readonly Db4objects.Db4o.CS.YapClient _client;

		private readonly int _queryResultID;

		private int _size = SIZE_NOT_SET;

		private readonly Db4objects.Db4o.CS.LazyClientIdIterator _iterator;

		public LazyClientQueryResult(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.CS.YapClient
			 client, int queryResultID) : base(trans)
		{
			_client = client;
			_queryResultID = queryResultID;
			_iterator = new Db4objects.Db4o.CS.LazyClientIdIterator(this);
		}

		public override object Get(int index)
		{
			lock (StreamLock())
			{
				return ActivatedObject(GetId(index));
			}
		}

		public override int GetId(int index)
		{
			return AskServer(Db4objects.Db4o.CS.Messages.Msg.OBJECTSET_GET_ID, index);
		}

		public override int IndexOf(int id)
		{
			return AskServer(Db4objects.Db4o.CS.Messages.Msg.OBJECTSET_INDEXOF, id);
		}

		private int AskServer(Db4objects.Db4o.CS.Messages.MsgD message, int param)
		{
			_client.WriteMsg(message.GetWriterForInts(_transaction, new int[] { _queryResultID
				, param }));
			return ((Db4objects.Db4o.CS.Messages.MsgD)_client.ExpectedResponse(message)).ReadInt
				();
		}

		public override Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			return _iterator;
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return new Db4objects.Db4o.CS.ClientQueryResultIterator(this);
		}

		public override int Size()
		{
			if (_size == SIZE_NOT_SET)
			{
				_client.WriteMsg(Db4objects.Db4o.CS.Messages.Msg.OBJECTSET_SIZE.GetWriterForInt(_transaction
					, _queryResultID));
				_size = ((Db4objects.Db4o.CS.Messages.MsgD)_client.ExpectedResponse(Db4objects.Db4o.CS.Messages.Msg
					.OBJECTSET_SIZE)).ReadInt();
			}
			return _size;
		}

		public override void Sort(Db4objects.Db4o.Query.IQueryComparator cmp)
		{
			throw new System.NotImplementedException();
		}

		~LazyClientQueryResult()
		{
			_client.WriteMsg(Db4objects.Db4o.CS.Messages.Msg.OBJECTSET_FINALIZED.GetWriterForInt
				(_transaction, _queryResultID));
		}

		public override void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			throw new System.NotImplementedException();
		}

		public override void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator
			 iterator)
		{
			throw new System.NotImplementedException();
		}

		public override void LoadFromIdReader(Db4objects.Db4o.YapReader reader)
		{
			_iterator.LoadFromIdReader(reader, reader.ReadInt());
		}

		public override void LoadFromQuery(Db4objects.Db4o.QQuery query)
		{
			throw new System.NotImplementedException();
		}

		public virtual void Reset()
		{
			_client.WriteMsg(Db4objects.Db4o.CS.Messages.Msg.OBJECTSET_RESET.GetWriterForInt(
				_transaction, _queryResultID));
		}

		public virtual void FetchIDs(int batchSize)
		{
			_client.WriteMsg(Db4objects.Db4o.CS.Messages.Msg.OBJECTSET_FETCH.GetWriterForInts
				(_transaction, new int[] { _queryResultID, batchSize }));
			Db4objects.Db4o.YapReader reader = _client.ExpectedByteResponse(Db4objects.Db4o.CS.Messages.Msg
				.ID_LIST);
			LoadFromIdReader(reader);
		}
	}
}