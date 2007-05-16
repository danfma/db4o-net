/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class LockedTree
	{
		private Tree _tree;

		private int _version;

		public virtual void Add(Tree tree)
		{
			Changed();
			_tree = _tree == null ? tree : _tree.Add(tree);
		}

		private void Changed()
		{
			_version++;
		}

		public virtual void Clear()
		{
			Changed();
			_tree = null;
		}

		public virtual Tree Find(int key)
		{
			return TreeInt.Find(_tree, key);
		}

		public virtual void Read(Db4objects.Db4o.Internal.Buffer buffer, IReadable template
			)
		{
			Clear();
			_tree = new TreeReader(buffer, template).Read();
			Changed();
		}

		public virtual void TraverseLocked(IVisitor4 visitor)
		{
			int currentVersion = _version;
			Tree.Traverse(_tree, visitor);
			if (_version != currentVersion)
			{
				throw new InvalidOperationException();
			}
		}

		public virtual void TraverseMutable(IVisitor4 visitor)
		{
			Collection4 currentContent = new Collection4();
			TraverseLocked(new _AnonymousInnerClass51(this, currentContent));
			IEnumerator i = currentContent.GetEnumerator();
			while (i.MoveNext())
			{
				visitor.Visit(i.Current);
			}
		}

		private sealed class _AnonymousInnerClass51 : IVisitor4
		{
			public _AnonymousInnerClass51(LockedTree _enclosing, Collection4 currentContent)
			{
				this._enclosing = _enclosing;
				this.currentContent = currentContent;
			}

			public void Visit(object obj)
			{
				currentContent.Add(obj);
			}

			private readonly LockedTree _enclosing;

			private readonly Collection4 currentContent;
		}
	}
}
