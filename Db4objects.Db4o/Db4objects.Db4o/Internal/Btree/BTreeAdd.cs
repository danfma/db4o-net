using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;

namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class BTreeAdd : BTreePatch
	{
		public BTreeAdd(Transaction transaction, object obj) : base(transaction, obj)
		{
		}

		protected virtual object RolledBack(BTree btree)
		{
			btree.NotifyRemoveListener(GetObject());
			return No4.INSTANCE;
		}

		public override string ToString()
		{
			return "(+) " + base.ToString();
		}

		public override object Commit(Transaction trans, BTree btree)
		{
			if (_transaction == trans)
			{
				return GetObject();
			}
			return this;
		}

		public override BTreePatch ForTransaction(Transaction trans)
		{
			if (_transaction == trans)
			{
				return this;
			}
			return null;
		}

		public override object Key(Transaction trans)
		{
			if (_transaction != trans)
			{
				return No4.INSTANCE;
			}
			return GetObject();
		}

		public override object Rollback(Transaction trans, BTree btree)
		{
			if (_transaction == trans)
			{
				return RolledBack(btree);
			}
			return this;
		}

		public override bool IsAdd()
		{
			return true;
		}
	}
}
