using Db4objects.Db4o;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.TA.Internal;

namespace Db4objects.Db4o.Collections.Internal
{
	public class Page : IActivatable
	{
		public const int PAGESIZE = 100;

		private object[] _data = new object[PAGESIZE];

		private int _top = 0;

		private int _pageIndex;

		[System.NonSerialized]
		private bool _dirty = false;

		[System.NonSerialized]
		internal Activator _activator;

		public Page(int pageIndex)
		{
			_pageIndex = pageIndex;
		}

		public virtual bool Add(object obj)
		{
			Activate();
			_dirty = true;
			_data[_top++] = obj;
			return true;
		}

		public virtual int Size()
		{
			Activate();
			return _top;
		}

		public virtual object Get(int indexInPage)
		{
			Activate();
			Sharpen.Runtime.Out.WriteLine("got from page: " + _pageIndex);
			_dirty = true;
			return _data[indexInPage];
		}

		public virtual bool IsDirty()
		{
			Activate();
			return _dirty;
		}

		public virtual void SetDirty(bool dirty)
		{
			Activate();
			_dirty = dirty;
		}

		public virtual int GetPageIndex()
		{
			Activate();
			return _pageIndex;
		}

		public virtual bool AtCapacity()
		{
			Activate();
			return Size() == Db4objects.Db4o.Collections.Internal.Page.PAGESIZE;
		}

		public virtual int Capacity()
		{
			Activate();
			return Db4objects.Db4o.Collections.Internal.Page.PAGESIZE - Size();
		}

		public virtual void Bind(IObjectContainer container)
		{
			if (null != _activator)
			{
				_activator.AssertCompatible(container);
				return;
			}
			_activator = new Activator(container, this);
		}

		private void Activate()
		{
			if (_activator == null)
			{
				return;
			}
			_activator.Activate();
		}
	}
}