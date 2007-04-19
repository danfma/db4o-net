using System;
using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Iterator4Impl : IEnumerator
	{
		private List4 _first;

		private List4 _next;

		private object _current;

		public Iterator4Impl(List4 first)
		{
			_first = first;
			_next = first;
			_current = Iterators.NO_ELEMENT;
		}

		public virtual bool MoveNext()
		{
			if (_next == null)
			{
				_current = Iterators.NO_ELEMENT;
				return false;
			}
			_current = _next._element;
			_next = _next._next;
			return true;
		}

		public virtual object Current
		{
			get
			{
				if (Iterators.NO_ELEMENT == _current)
				{
					throw new InvalidOperationException();
				}
				return _current;
			}
		}

		public virtual void Reset()
		{
			_next = _first;
			_current = Iterators.NO_ELEMENT;
		}
	}
}
