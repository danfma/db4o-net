/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class SPCChildWithoutDefaultConstructor : SPCChild
	{
		public SPCChildWithoutDefaultConstructor(string name) : base(name)
		{
		}
	}
}
