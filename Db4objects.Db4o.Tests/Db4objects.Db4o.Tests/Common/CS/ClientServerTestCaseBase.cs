/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ClientServerTestCaseBase : Db4oClientServerTestCase
	{
		protected virtual IServerMessageDispatcher ServerDispatcher()
		{
			ObjectServerImpl serverImpl = (ObjectServerImpl)ClientServerFixture().Server();
			IEnumerator iter = serverImpl.IterateDispatchers();
			iter.MoveNext();
			IServerMessageDispatcher dispatcher = (IServerMessageDispatcher)iter.Current;
			return dispatcher;
		}

		protected virtual ClientObjectContainer Client()
		{
			return (ClientObjectContainer)Db();
		}
	}
}
