/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Events
{
	public class ClassEventArgs : EventArgs
	{
		private Db4objects.Db4o.Internal.ClassMetadata _clazz;

		public ClassEventArgs(Db4objects.Db4o.Internal.ClassMetadata clazz)
		{
			_clazz = clazz;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _clazz;
		}
	}
}
