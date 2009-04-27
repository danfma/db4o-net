/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	public sealed class InterfaceTypeHandler : Db4objects.Db4o.Internal.OpenTypeHandler
	{
		public InterfaceTypeHandler(ObjectContainerBase container) : base(container)
		{
		}

		public override bool Equals(object obj)
		{
			return obj is InterfaceTypeHandler;
		}
	}
}