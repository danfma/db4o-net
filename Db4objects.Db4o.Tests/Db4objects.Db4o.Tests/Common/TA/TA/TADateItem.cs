/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA.TA
{
	public class TADateItem : ActivatableImpl
	{
		public const long DAY = 1000 * 60 * 60 * 24;

		public DateTime _typed;

		public object _untyped;

		public virtual DateTime GetTyped()
		{
			Activate();
			return _typed;
		}

		public virtual object GetUntyped()
		{
			Activate();
			return _untyped;
		}

		public override string ToString()
		{
			Activate();
			return _typed.ToString();
		}
	}
}
