/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA.TA
{
	public class TADateArrayItem : ActivatableImpl
	{
		public DateTime[] _typed;

		public object[] _untyped;

		public virtual DateTime[] GetTyped()
		{
			Activate();
			return _typed;
		}

		public virtual object[] GetUntyped()
		{
			Activate();
			return _untyped;
		}
	}
}
