/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class MockReadContext : MockMarshallingContext, IReadContext
	{
		public MockReadContext(IObjectContainer objectContainer) : base(objectContainer)
		{
		}

		public MockReadContext(MockWriteContext writeContext) : this(writeContext.ObjectContainer
			())
		{
			writeContext._header.CopyTo(_header, 0, 0, writeContext._header.Length());
			writeContext._payLoad.CopyTo(_payLoad, 0, 0, writeContext._payLoad.Length());
		}
	}
}
