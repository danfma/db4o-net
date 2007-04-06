using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.IX;
using Sharpen;

namespace Db4objects.Db4o.Internal.Btree
{
	/// <summary>
	/// We work with BTreeNode in two states:
	/// - deactivated: never read, no valid members, ID correct or 0 if new
	/// - write: real representation of keys, values and children in arrays
	/// The write state can be detected with canWrite().
	/// </summary>
	/// <remarks>
	/// We work with BTreeNode in two states:
	/// - deactivated: never read, no valid members, ID correct or 0 if new
	/// - write: real representation of keys, values and children in arrays
	/// The write state can be detected with canWrite(). States can be changed
	/// as needed with prepareRead() and prepareWrite().
	/// </remarks>
	/// <exclude></exclude>
	public sealed class BTreeNode : PersistentBase
	{
		private const int COUNT_LEAF_AND_3_LINK_LENGTH = (Const4.INT_LENGTH * 4) + 1;

		private const int SLOT_LEADING_LENGTH = Const4.LEADING_LENGTH + COUNT_LEAF_AND_3_LINK_LENGTH;

		internal readonly BTree _btree;

		private int _count;

		private bool _isLeaf;

		private object[] _keys;

		/// <summary>Can contain BTreeNode or Integer for ID of BTreeNode</summary>
		private object[] _children;

		private int _parentID;

		private int _previousID;

		private int _nextID;

		private bool _cached;

		private bool _dead;

		public BTreeNode(BTree btree, int count, bool isLeaf, int parentID, int previousID
			, int nextID)
		{
			_btree = btree;
			_parentID = parentID;
			_previousID = previousID;
			_nextID = nextID;
			_count = count;
			_isLeaf = isLeaf;
			PrepareArrays();
		}

		public BTreeNode(int id, BTree btree)
		{
			_btree = btree;
			SetID(id);
			SetStateDeactivated();
		}

		public BTreeNode(Transaction trans, Db4objects.Db4o.Internal.Btree.BTreeNode firstChild
			, Db4objects.Db4o.Internal.Btree.BTreeNode secondChild) : this(firstChild._btree
			, 2, false, 0, 0, 0)
		{
			_keys[0] = firstChild._keys[0];
			_children[0] = firstChild;
			_keys[1] = secondChild._keys[0];
			_children[1] = secondChild;
			Write(trans.SystemTransaction());
			firstChild.SetParentID(trans, GetID());
			secondChild.SetParentID(trans, GetID());
		}

		public BTree Btree()
		{
			return _btree;
		}

		/// <returns>
		/// the split node if this node is split
		/// or this if the first key has changed
		/// </returns>
		public Db4objects.Db4o.Internal.Btree.BTreeNode Add(Transaction trans)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			Searcher s = Search(reader);
			if (_isLeaf)
			{
				PrepareWrite(trans);
				if (WasRemoved(trans, s))
				{
					CancelRemoval(trans, s.Cursor());
					return null;
				}
				if (s.Count() > 0 && !s.BeforeFirst())
				{
					s.MoveForward();
				}
				PrepareInsert(s.Cursor());
				_keys[s.Cursor()] = NewAddPatch(trans);
			}
			else
			{
				Db4objects.Db4o.Internal.Btree.BTreeNode childNode = Child(reader, s.Cursor());
				Db4objects.Db4o.Internal.Btree.BTreeNode childNodeOrSplit = childNode.Add(trans);
				if (childNodeOrSplit == null)
				{
					return null;
				}
				PrepareWrite(trans);
				_keys[s.Cursor()] = childNode._keys[0];
				if (childNode != childNodeOrSplit)
				{
					int splitCursor = s.Cursor() + 1;
					PrepareInsert(splitCursor);
					_keys[splitCursor] = childNodeOrSplit._keys[0];
					_children[splitCursor] = childNodeOrSplit;
				}
			}
			if (MustSplit())
			{
				return Split(trans);
			}
			if (s.Cursor() == 0)
			{
				return this;
			}
			return null;
		}

		private bool MustSplit()
		{
			return _count >= _btree.NodeSize();
		}

		private BTreeAdd NewAddPatch(Transaction trans)
		{
			SizeIncrement(trans);
			return new BTreeAdd(trans, CurrentKey());
		}

		private object CurrentKey()
		{
			return KeyHandler().Current();
		}

		private void CancelRemoval(Transaction trans, int index)
		{
			BTreeUpdate patch = (BTreeUpdate)KeyPatch(index);
			BTreeUpdate nextPatch = patch.RemoveFor(trans);
			_keys[index] = NewCancelledRemoval(trans, patch.GetObject(), nextPatch);
			SizeIncrement(trans);
		}

		private BTreePatch NewCancelledRemoval(Transaction trans, object originalObject, 
			BTreeUpdate existingPatches)
		{
			return new BTreeCancelledRemoval(trans, originalObject, CurrentKey(), existingPatches
				);
		}

		private void SizeIncrement(Transaction trans)
		{
			_btree.SizeChanged(trans, 1);
		}

		private bool WasRemoved(Transaction trans, Searcher s)
		{
			if (!s.FoundMatch())
			{
				return false;
			}
			BTreePatch patch = KeyPatch(trans, s.Cursor());
			return patch != null && patch.IsRemove();
		}

		internal BTreeNodeSearchResult SearchLeaf(Transaction trans, SearchTarget target)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			Searcher s = Search(reader, target);
			if (!_isLeaf)
			{
				return Child(reader, s.Cursor()).SearchLeaf(trans, target);
			}
			if (!s.FoundMatch() || target == SearchTarget.ANY || target == SearchTarget.HIGHEST
				)
			{
				return new BTreeNodeSearchResult(trans, reader, Btree(), s, this);
			}
			if (target == SearchTarget.LOWEST)
			{
				BTreeNodeSearchResult res = FindLowestLeafMatch(trans, s.Cursor() - 1);
				if (res != null)
				{
					return res;
				}
				return CreateMatchingSearchResult(trans, reader, s.Cursor());
			}
			throw new InvalidOperationException();
		}

		private BTreeNodeSearchResult FindLowestLeafMatch(Transaction trans, int index)
		{
			return FindLowestLeafMatch(trans, PrepareRead(trans), index);
		}

		private BTreeNodeSearchResult FindLowestLeafMatch(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader, int index)
		{
			if (index >= 0)
			{
				if (!CompareEquals(reader, index))
				{
					return null;
				}
				if (index > 0)
				{
					BTreeNodeSearchResult res = FindLowestLeafMatch(trans, reader, index - 1);
					if (res != null)
					{
						return res;
					}
					return CreateMatchingSearchResult(trans, reader, index);
				}
			}
			Db4objects.Db4o.Internal.Btree.BTreeNode node = PreviousNode();
			if (node != null)
			{
				Db4objects.Db4o.Internal.Buffer nodeReader = node.PrepareRead(trans);
				BTreeNodeSearchResult res = node.FindLowestLeafMatch(trans, nodeReader, node.LastIndex
					());
				if (res != null)
				{
					return res;
				}
			}
			if (index < 0)
			{
				return null;
			}
			return CreateMatchingSearchResult(trans, reader, index);
		}

		private bool CompareEquals(Db4objects.Db4o.Internal.Buffer reader, int index)
		{
			if (CanWrite())
			{
				return CompareInWriteMode(index) == 0;
			}
			return CompareInReadMode(reader, index) == 0;
		}

		private BTreeNodeSearchResult CreateMatchingSearchResult(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader, int index)
		{
			return new BTreeNodeSearchResult(trans, reader, Btree(), this, index, true);
		}

		public bool CanWrite()
		{
			return _keys != null;
		}

		internal Db4objects.Db4o.Internal.Btree.BTreeNode Child(int index)
		{
			if (_children[index] is Db4objects.Db4o.Internal.Btree.BTreeNode)
			{
				return (Db4objects.Db4o.Internal.Btree.BTreeNode)_children[index];
			}
			return _btree.ProduceNode(((int)_children[index]));
		}

		internal Db4objects.Db4o.Internal.Btree.BTreeNode Child(Db4objects.Db4o.Internal.Buffer
			 reader, int index)
		{
			if (ChildLoaded(index))
			{
				return (Db4objects.Db4o.Internal.Btree.BTreeNode)_children[index];
			}
			Db4objects.Db4o.Internal.Btree.BTreeNode child = _btree.ProduceNode(ChildID(reader
				, index));
			if (_children != null)
			{
				if (_cached || child.CanWrite())
				{
					_children[index] = child;
				}
			}
			return child;
		}

		private int ChildID(Db4objects.Db4o.Internal.Buffer reader, int index)
		{
			if (_children == null)
			{
				SeekChild(reader, index);
				return reader.ReadInt();
			}
			return ChildID(index);
		}

		private int ChildID(int index)
		{
			if (ChildLoaded(index))
			{
				return ((Db4objects.Db4o.Internal.Btree.BTreeNode)_children[index]).GetID();
			}
			return ((int)_children[index]);
		}

		private bool ChildLoaded(int index)
		{
			if (_children == null)
			{
				return false;
			}
			return _children[index] is Db4objects.Db4o.Internal.Btree.BTreeNode;
		}

		private bool ChildCanSupplyFirstKey(int index)
		{
			if (!ChildLoaded(index))
			{
				return false;
			}
			return ((Db4objects.Db4o.Internal.Btree.BTreeNode)_children[index]).CanWrite();
		}

		internal void Commit(Transaction trans)
		{
			CommitOrRollback(trans, true);
		}

		internal void CommitOrRollback(Transaction trans, bool isCommit)
		{
			if (_dead)
			{
				return;
			}
			_cached = false;
			if (!_isLeaf)
			{
				return;
			}
			if (!IsDirty(trans))
			{
				return;
			}
			object keyZero = _keys[0];
			object[] tempKeys = new object[_btree.NodeSize()];
			int count = 0;
			for (int i = 0; i < _count; i++)
			{
				object key = _keys[i];
				BTreePatch patch = KeyPatch(i);
				if (patch != null)
				{
					key = isCommit ? patch.Commit(trans, _btree) : patch.Rollback(trans, _btree);
				}
				if (key != No4.INSTANCE)
				{
					tempKeys[count] = key;
					count++;
				}
			}
			_keys = tempKeys;
			_count = count;
			if (FreeIfEmpty(trans))
			{
				return;
			}
			SetStateDirty();
			if (_keys[0] != keyZero)
			{
				TellParentAboutChangedKey(trans);
			}
		}

		private bool FreeIfEmpty(Transaction trans)
		{
			return FreeIfEmpty(trans, _count);
		}

		private bool FreeIfEmpty(Transaction trans, int count)
		{
			if (count > 0)
			{
				return false;
			}
			if (IsRoot())
			{
				return false;
			}
			Free(trans);
			return true;
		}

		private bool IsRoot()
		{
			return _btree.Root() == this;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is Db4objects.Db4o.Internal.Btree.BTreeNode))
			{
				return false;
			}
			Db4objects.Db4o.Internal.Btree.BTreeNode other = (Db4objects.Db4o.Internal.Btree.BTreeNode
				)obj;
			return GetID() == other.GetID();
		}

		public override int GetHashCode()
		{
			return GetID();
		}

		private void Free(Transaction trans)
		{
			_dead = true;
			if (!IsRoot())
			{
				Db4objects.Db4o.Internal.Btree.BTreeNode parent = _btree.ProduceNode(_parentID);
				parent.RemoveChild(trans, this);
			}
			PointPreviousTo(trans, _nextID);
			PointNextTo(trans, _previousID);
			trans.SystemTransaction().SlotFreePointerOnCommit(GetID());
			_btree.RemoveNode(this);
		}

		internal void HoldChildrenAsIDs()
		{
			if (_children == null)
			{
				return;
			}
			for (int i = 0; i < _count; i++)
			{
				if (_children[i] is Db4objects.Db4o.Internal.Btree.BTreeNode)
				{
					_children[i] = ((Db4objects.Db4o.Internal.Btree.BTreeNode)_children[i]).GetID();
				}
			}
		}

		private void RemoveChild(Transaction trans, Db4objects.Db4o.Internal.Btree.BTreeNode
			 child)
		{
			PrepareWrite(trans);
			int id = child.GetID();
			for (int i = 0; i < _count; i++)
			{
				if (ChildID(i) == id)
				{
					if (FreeIfEmpty(trans, _count - 1))
					{
						return;
					}
					Remove(i);
					if (i <= 1)
					{
						TellParentAboutChangedKey(trans);
					}
					if (_count == 0)
					{
						_isLeaf = true;
					}
					return;
				}
			}
			throw new InvalidOperationException("child not found");
		}

		private void KeyChanged(Transaction trans, Db4objects.Db4o.Internal.Btree.BTreeNode
			 child)
		{
			PrepareWrite(trans);
			int id = child.GetID();
			for (int i = 0; i < _count; i++)
			{
				if (ChildID(i) == id)
				{
					_keys[i] = child._keys[0];
					_children[i] = child;
					KeyChanged(trans, i);
					return;
				}
			}
			throw new InvalidOperationException("child not found");
		}

		private void TellParentAboutChangedKey(Transaction trans)
		{
			if (!IsRoot())
			{
				Db4objects.Db4o.Internal.Btree.BTreeNode parent = _btree.ProduceNode(_parentID);
				parent.KeyChanged(trans, this);
			}
		}

		private bool IsDirty(Transaction trans)
		{
			if (!CanWrite())
			{
				return false;
			}
			for (int i = 0; i < _count; i++)
			{
				if (KeyPatch(trans, i) != null)
				{
					return true;
				}
			}
			return false;
		}

		private int CompareInWriteMode(int index)
		{
			return KeyHandler().CompareTo(Key(index));
		}

		private int CompareInReadMode(Db4objects.Db4o.Internal.Buffer reader, int index)
		{
			SeekKey(reader, index);
			return KeyHandler().CompareTo(KeyHandler().ReadIndexEntry(reader));
		}

		public int Count()
		{
			return _count;
		}

		private int EntryLength()
		{
			int len = KeyHandler().LinkLength();
			if (!_isLeaf)
			{
				len += Const4.ID_LENGTH;
			}
			return len;
		}

		public int FirstKeyIndex(Transaction trans)
		{
			for (int ix = 0; ix < _count; ix++)
			{
				if (IndexIsValid(trans, ix))
				{
					return ix;
				}
			}
			return -1;
		}

		public int LastKeyIndex(Transaction trans)
		{
			for (int ix = _count - 1; ix >= 0; ix--)
			{
				if (IndexIsValid(trans, ix))
				{
					return ix;
				}
			}
			return -1;
		}

		public bool IndexIsValid(Transaction trans, int index)
		{
			if (!CanWrite())
			{
				return true;
			}
			BTreePatch patch = KeyPatch(index);
			if (patch == null)
			{
				return true;
			}
			return patch.Key(trans) != No4.INSTANCE;
		}

		private object FirstKey(Transaction trans)
		{
			int index = FirstKeyIndex(trans);
			if (-1 == index)
			{
				return No4.INSTANCE;
			}
			return Key(trans, index);
		}

		public override byte GetIdentifier()
		{
			return Const4.BTREE_NODE;
		}

		private void PrepareInsert(int pos)
		{
			if (pos > LastIndex())
			{
				_count++;
				return;
			}
			int len = _count - pos;
			System.Array.Copy(_keys, pos, _keys, pos + 1, len);
			if (_children != null)
			{
				System.Array.Copy(_children, pos, _children, pos + 1, len);
			}
			_count++;
		}

		private void Remove(int pos)
		{
			int len = _count - pos;
			_count--;
			System.Array.Copy(_keys, pos + 1, _keys, pos, len);
			_keys[_count] = null;
			if (_children != null)
			{
				System.Array.Copy(_children, pos + 1, _children, pos, len);
				_children[_count] = null;
			}
		}

		internal object Key(int index)
		{
			object obj = _keys[index];
			if (obj is BTreePatch)
			{
				return ((BTreePatch)obj).GetObject();
			}
			return obj;
		}

		internal object Key(Transaction trans, Db4objects.Db4o.Internal.Buffer reader, int
			 index)
		{
			if (CanWrite())
			{
				return Key(trans, index);
			}
			SeekKey(reader, index);
			return KeyHandler().ReadIndexEntry(reader);
		}

		internal object Key(Transaction trans, int index)
		{
			BTreePatch patch = KeyPatch(index);
			if (patch == null)
			{
				return _keys[index];
			}
			return patch.Key(trans);
		}

		private BTreePatch KeyPatch(int index)
		{
			object obj = _keys[index];
			if (obj is BTreePatch)
			{
				return (BTreePatch)obj;
			}
			return null;
		}

		private BTreePatch KeyPatch(Transaction trans, int index)
		{
			object obj = _keys[index];
			if (obj is BTreePatch)
			{
				return ((BTreePatch)obj).ForTransaction(trans);
			}
			return null;
		}

		private IIndexable4 KeyHandler()
		{
			return _btree.KeyHandler();
		}

		internal void MarkAsCached(int height)
		{
			_cached = true;
			_btree.AddNode(this);
			if (_isLeaf || (_children == null))
			{
				return;
			}
			height--;
			if (height < 1)
			{
				HoldChildrenAsIDs();
				return;
			}
			for (int i = 0; i < _count; i++)
			{
				if (_children[i] is Db4objects.Db4o.Internal.Btree.BTreeNode)
				{
					((Db4objects.Db4o.Internal.Btree.BTreeNode)_children[i]).MarkAsCached(height);
				}
			}
		}

		public override int OwnLength()
		{
			return SLOT_LEADING_LENGTH + (_count * EntryLength()) + Const4.BRACKETS_BYTES;
		}

		internal Db4objects.Db4o.Internal.Buffer PrepareRead(Transaction trans)
		{
			if (CanWrite())
			{
				return null;
			}
			if (IsNew())
			{
				return null;
			}
			if (_cached)
			{
				Read(trans.SystemTransaction());
				_btree.AddToProcessing(this);
				return null;
			}
			Db4objects.Db4o.Internal.Buffer reader = ((LocalTransaction)trans).File().ReadReaderByID
				(trans.SystemTransaction(), GetID());
			ReadNodeHeader(reader);
			return reader;
		}

		internal void PrepareWrite(Transaction trans)
		{
			if (_dead)
			{
				return;
			}
			if (CanWrite())
			{
				SetStateDirty();
				return;
			}
			Read(trans.SystemTransaction());
			SetStateDirty();
			_btree.AddToProcessing(this);
		}

		private void PrepareArrays()
		{
			if (CanWrite())
			{
				return;
			}
			_keys = new object[_btree.NodeSize()];
			if (!_isLeaf)
			{
				_children = new object[_btree.NodeSize()];
			}
		}

		private void ReadNodeHeader(Db4objects.Db4o.Internal.Buffer reader)
		{
			_count = reader.ReadInt();
			byte leafByte = reader.ReadByte();
			_isLeaf = (leafByte == 1);
			_parentID = reader.ReadInt();
			_previousID = reader.ReadInt();
			_nextID = reader.ReadInt();
		}

		public override void ReadThis(Transaction trans, Db4objects.Db4o.Internal.Buffer 
			reader)
		{
			ReadNodeHeader(reader);
			PrepareArrays();
			bool isInner = !_isLeaf;
			for (int i = 0; i < _count; i++)
			{
				_keys[i] = KeyHandler().ReadIndexEntry(reader);
				if (isInner)
				{
					_children[i] = reader.ReadInt();
				}
			}
		}

		public void Remove(Transaction trans, int index)
		{
			if (!_isLeaf)
			{
				throw new InvalidOperationException();
			}
			PrepareWrite(trans);
			BTreePatch patch = KeyPatch(index);
			if (patch == null)
			{
				_keys[index] = NewRemovePatch(trans);
				KeyChanged(trans, index);
				return;
			}
			BTreePatch transPatch = patch.ForTransaction(trans);
			if (transPatch != null)
			{
				if (transPatch.IsAdd())
				{
					CancelAdding(trans, index);
					return;
				}
				if (transPatch.IsCancelledRemoval())
				{
					BTreeRemove removePatch = NewRemovePatch(trans, transPatch.GetObject());
					_keys[index] = ((BTreeUpdate)patch).ReplacePatch(transPatch, removePatch);
					KeyChanged(trans, index);
					return;
				}
			}
			else
			{
				if (!patch.IsAdd())
				{
					((BTreeUpdate)patch).Append(NewRemovePatch(trans));
					return;
				}
			}
			if (index != LastIndex())
			{
				if (CompareInWriteMode(index + 1) != 0)
				{
					return;
				}
				Remove(trans, index + 1);
				return;
			}
			Db4objects.Db4o.Internal.Btree.BTreeNode node = NextNode();
			if (node == null)
			{
				return;
			}
			node.PrepareWrite(trans);
			if (node.CompareInWriteMode(0) != 0)
			{
				return;
			}
			node.Remove(trans, 0);
		}

		private void CancelAdding(Transaction trans, int index)
		{
			_btree.NotifyRemoveListener(KeyPatch(index).GetObject());
			if (FreeIfEmpty(trans, _count - 1))
			{
				SizeDecrement(trans);
				return;
			}
			Remove(index);
			KeyChanged(trans, index);
			SizeDecrement(trans);
		}

		private void SizeDecrement(Transaction trans)
		{
			_btree.SizeChanged(trans, -1);
		}

		private int LastIndex()
		{
			return _count - 1;
		}

		private BTreeRemove NewRemovePatch(Transaction trans)
		{
			return NewRemovePatch(trans, CurrentKey());
		}

		private BTreeRemove NewRemovePatch(Transaction trans, object key)
		{
			_btree.SizeChanged(trans, -1);
			return new BTreeRemove(trans, key);
		}

		private void KeyChanged(Transaction trans, int index)
		{
			if (index == 0)
			{
				TellParentAboutChangedKey(trans);
			}
		}

		internal void Rollback(Transaction trans)
		{
			CommitOrRollback(trans, false);
		}

		private Searcher Search(Db4objects.Db4o.Internal.Buffer reader)
		{
			return Search(reader, SearchTarget.ANY);
		}

		private Searcher Search(Db4objects.Db4o.Internal.Buffer reader, SearchTarget target
			)
		{
			Searcher s = new Searcher(target, _count);
			if (CanWrite())
			{
				while (s.Incomplete())
				{
					s.ResultIs(CompareInWriteMode(s.Cursor()));
				}
			}
			else
			{
				while (s.Incomplete())
				{
					s.ResultIs(CompareInReadMode(reader, s.Cursor()));
				}
			}
			return s;
		}

		private void SeekAfterKey(Db4objects.Db4o.Internal.Buffer reader, int ix)
		{
			SeekKey(reader, ix);
			reader._offset += KeyHandler().LinkLength();
		}

		private void SeekChild(Db4objects.Db4o.Internal.Buffer reader, int ix)
		{
			SeekAfterKey(reader, ix);
		}

		private void SeekKey(Db4objects.Db4o.Internal.Buffer reader, int ix)
		{
			reader._offset = SLOT_LEADING_LENGTH + (EntryLength() * ix);
		}

		private Db4objects.Db4o.Internal.Btree.BTreeNode Split(Transaction trans)
		{
			Db4objects.Db4o.Internal.Btree.BTreeNode res = new Db4objects.Db4o.Internal.Btree.BTreeNode
				(_btree, _btree._halfNodeSize, _isLeaf, _parentID, GetID(), _nextID);
			System.Array.Copy(_keys, _btree._halfNodeSize, res._keys, 0, _btree._halfNodeSize
				);
			for (int i = _btree._halfNodeSize; i < _keys.Length; i++)
			{
				_keys[i] = null;
			}
			if (_children != null)
			{
				res._children = new object[_btree.NodeSize()];
				System.Array.Copy(_children, _btree._halfNodeSize, res._children, 0, _btree._halfNodeSize
					);
				for (int i = _btree._halfNodeSize; i < _children.Length; i++)
				{
					_children[i] = null;
				}
			}
			_count = _btree._halfNodeSize;
			res.Write(trans.SystemTransaction());
			_btree.AddNode(res);
			int splitID = res.GetID();
			PointNextTo(trans, splitID);
			SetNextID(trans, splitID);
			if (_children != null)
			{
				for (int i = 0; i < _btree._halfNodeSize; i++)
				{
					if (res._children[i] == null)
					{
						break;
					}
					res.Child(i).SetParentID(trans, splitID);
				}
			}
			return res;
		}

		private void PointNextTo(Transaction trans, int id)
		{
			if (_nextID != 0)
			{
				NextNode().SetPreviousID(trans, id);
			}
		}

		private void PointPreviousTo(Transaction trans, int id)
		{
			if (_previousID != 0)
			{
				PreviousNode().SetNextID(trans, id);
			}
		}

		public Db4objects.Db4o.Internal.Btree.BTreeNode PreviousNode()
		{
			if (_previousID == 0)
			{
				return null;
			}
			return _btree.ProduceNode(_previousID);
		}

		public Db4objects.Db4o.Internal.Btree.BTreeNode NextNode()
		{
			if (_nextID == 0)
			{
				return null;
			}
			return _btree.ProduceNode(_nextID);
		}

		internal BTreePointer FirstPointer(Transaction trans)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			if (_isLeaf)
			{
				return LeafFirstPointer(trans, reader);
			}
			return BranchFirstPointer(trans, reader);
		}

		private BTreePointer BranchFirstPointer(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			for (int i = 0; i < _count; i++)
			{
				BTreePointer childFirstPointer = Child(reader, i).FirstPointer(trans);
				if (childFirstPointer != null)
				{
					return childFirstPointer;
				}
			}
			return null;
		}

		private BTreePointer LeafFirstPointer(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			int index = FirstKeyIndex(trans);
			if (index == -1)
			{
				return null;
			}
			return new BTreePointer(trans, reader, this, index);
		}

		public BTreePointer LastPointer(Transaction trans)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			if (_isLeaf)
			{
				return LeafLastPointer(trans, reader);
			}
			return BranchLastPointer(trans, reader);
		}

		private BTreePointer BranchLastPointer(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			for (int i = _count - 1; i >= 0; i--)
			{
				BTreePointer childLastPointer = Child(reader, i).LastPointer(trans);
				if (childLastPointer != null)
				{
					return childLastPointer;
				}
			}
			return null;
		}

		private BTreePointer LeafLastPointer(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			int index = LastKeyIndex(trans);
			if (index == -1)
			{
				return null;
			}
			return new BTreePointer(trans, reader, this, index);
		}

		internal void Purge()
		{
			if (_dead)
			{
				_keys = null;
				_children = null;
				return;
			}
			if (_cached)
			{
				return;
			}
			if (!CanWrite())
			{
				return;
			}
			for (int i = 0; i < _count; i++)
			{
				if (_keys[i] is BTreePatch)
				{
					HoldChildrenAsIDs();
					_btree.AddNode(this);
					return;
				}
			}
		}

		private void SetParentID(Transaction trans, int id)
		{
			PrepareWrite(trans);
			_parentID = id;
		}

		private void SetPreviousID(Transaction trans, int id)
		{
			PrepareWrite(trans);
			_previousID = id;
		}

		private void SetNextID(Transaction trans, int id)
		{
			PrepareWrite(trans);
			_nextID = id;
		}

		public void TraverseKeys(Transaction trans, IVisitor4 visitor)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			if (_isLeaf)
			{
				for (int i = 0; i < _count; i++)
				{
					object obj = Key(trans, reader, i);
					if (obj != No4.INSTANCE)
					{
						visitor.Visit(obj);
					}
				}
			}
			else
			{
				for (int i = 0; i < _count; i++)
				{
					Child(reader, i).TraverseKeys(trans, visitor);
				}
			}
		}

		public override bool WriteObjectBegin()
		{
			if (_dead)
			{
				return false;
			}
			if (!CanWrite())
			{
				return false;
			}
			return base.WriteObjectBegin();
		}

		public override void WriteThis(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 a_writer)
		{
			int count = 0;
			int startOffset = a_writer._offset;
			a_writer.IncrementOffset(COUNT_LEAF_AND_3_LINK_LENGTH);
			if (_isLeaf)
			{
				for (int i = 0; i < _count; i++)
				{
					object obj = Key(trans, i);
					if (obj != No4.INSTANCE)
					{
						count++;
						KeyHandler().WriteIndexEntry(a_writer, obj);
					}
				}
			}
			else
			{
				for (int i = 0; i < _count; i++)
				{
					if (ChildCanSupplyFirstKey(i))
					{
						Db4objects.Db4o.Internal.Btree.BTreeNode child = (Db4objects.Db4o.Internal.Btree.BTreeNode
							)_children[i];
						object childKey = child.FirstKey(trans);
						if (childKey != No4.INSTANCE)
						{
							count++;
							KeyHandler().WriteIndexEntry(a_writer, childKey);
							a_writer.WriteIDOf(trans, child);
						}
					}
					else
					{
						count++;
						KeyHandler().WriteIndexEntry(a_writer, Key(i));
						a_writer.WriteIDOf(trans, _children[i]);
					}
				}
			}
			int endOffset = a_writer._offset;
			a_writer._offset = startOffset;
			a_writer.WriteInt(count);
			a_writer.Append(_isLeaf ? (byte)1 : (byte)0);
			a_writer.WriteInt(_parentID);
			a_writer.WriteInt(_previousID);
			a_writer.WriteInt(_nextID);
			a_writer._offset = endOffset;
		}

		public override string ToString()
		{
			if (_count == 0)
			{
				return "Node " + GetID() + " not loaded";
			}
			string str = "\nBTreeNode";
			str += "\nid: " + GetID();
			str += "\nparent: " + _parentID;
			str += "\nprevious: " + _previousID;
			str += "\nnext: " + _nextID;
			str += "\ncount:" + _count;
			str += "\nleaf:" + _isLeaf + "\n";
			if (CanWrite())
			{
				str += " { ";
				bool first = true;
				for (int i = 0; i < _count; i++)
				{
					if (_keys[i] != null)
					{
						if (!first)
						{
							str += ", ";
						}
						str += _keys[i].ToString();
						first = false;
					}
				}
				str += " }";
			}
			return str;
		}

		public void DebugLoadFully(Transaction trans)
		{
			PrepareWrite(trans);
			if (_isLeaf)
			{
				return;
			}
			for (int i = 0; i < _count; ++i)
			{
				if (_children[i] is int)
				{
					_children[i] = Btree().ProduceNode(((int)_children[i]));
				}
				((Db4objects.Db4o.Internal.Btree.BTreeNode)_children[i]).DebugLoadFully(trans);
			}
		}

		public static void DefragIndex(ReaderPair readers, IIndexable4 keyHandler)
		{
			int count = readers.ReadInt();
			byte leafByte = readers.ReadByte();
			bool isLeaf = (leafByte == 1);
			readers.CopyID();
			readers.CopyID();
			readers.CopyID();
			for (int i = 0; i < count; i++)
			{
				keyHandler.DefragIndexEntry(readers);
				if (!isLeaf)
				{
					readers.CopyID();
				}
			}
		}

		public bool IsLeaf()
		{
			return _isLeaf;
		}

		/// <summary>This traversal goes over all nodes, not just leafs</summary>
		internal void TraverseAllNodes(Transaction trans, IVisitor4 command)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			command.Visit(this);
			if (_isLeaf)
			{
				return;
			}
			for (int childIdx = 0; childIdx < _count; childIdx++)
			{
				Child(reader, childIdx).TraverseAllNodes(trans, command);
			}
		}
	}
}
