using System;
using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class SortedCollection4
	{
		private readonly IComparison4 _comparison;

		private Tree _tree;

		public SortedCollection4(IComparison4 comparison)
		{
			if (null == comparison)
			{
				throw new ArgumentNullException();
			}
			_comparison = comparison;
			_tree = null;
		}

		public virtual object SingleElement()
		{
			if (1 != Size())
			{
				throw new InvalidOperationException();
			}
			return _tree.Key();
		}

		public virtual void AddAll(IEnumerator iterator)
		{
			while (iterator.MoveNext())
			{
				Add(iterator.Current);
			}
		}

		public virtual void Add(object element)
		{
			_tree = Tree.Add(_tree, new TreeObject(element, _comparison));
		}

		public virtual void Remove(object element)
		{
			_tree = Tree.RemoveLike(_tree, new TreeObject(element, _comparison));
		}

		public virtual object[] ToArray(object[] array)
		{
			Tree.Traverse(_tree, new _AnonymousInnerClass43(this, array));
			return array;
		}

		private sealed class _AnonymousInnerClass43 : IVisitor4
		{
			public _AnonymousInnerClass43(SortedCollection4 _enclosing, object[] array)
			{
				this._enclosing = _enclosing;
				this.array = array;
			}

			internal int i = 0;

			public void Visit(object obj)
			{
				array[this.i++] = ((TreeObject)obj).Key();
			}

			private readonly SortedCollection4 _enclosing;

			private readonly object[] array;
		}

		public virtual int Size()
		{
			return Tree.Size(_tree);
		}
	}
}
