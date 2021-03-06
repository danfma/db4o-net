/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Tests.Common.Persistent
{
	public class SimpleObject
	{
		public string _s;

		public int _i;

		public SimpleObject(string s, int i)
		{
			_s = s;
			_i = i;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Db4objects.Db4o.Tests.Common.Persistent.SimpleObject))
			{
				return false;
			}
			Db4objects.Db4o.Tests.Common.Persistent.SimpleObject another = (Db4objects.Db4o.Tests.Common.Persistent.SimpleObject
				)obj;
			return _s.Equals(another._s) && (_i == another._i);
		}

		public virtual int GetI()
		{
			return _i;
		}

		public virtual void SetI(int i)
		{
			_i = i;
		}

		public virtual string GetS()
		{
			return _s;
		}

		public virtual void SetS(string s)
		{
			_s = s;
		}

		public override string ToString()
		{
			return _s + ":" + _i;
		}
	}
}
