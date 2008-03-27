/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit.Tests.Fixtures;

namespace Db4oUnit.Tests.Fixtures
{
	public class HashtableSet4 : ISet4
	{
		internal Hashtable _table = new Hashtable();

		public virtual void Add(object value)
		{
			_table.Add(value, value);
		}

		public virtual bool Contains(object value)
		{
			return _table.Contains(value);
		}

		public virtual int Size()
		{
			return _table.Count;
		}
	}
}