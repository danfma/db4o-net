/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Tests.Common.Concurrency;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class ClientDisconnectTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] arguments)
		{
			new ClientDisconnectTestCase().RunConcurrency();
		}

		public virtual void _concDelete(IExtObjectContainer oc, int seq)
		{
			ClientObjectContainer client = (ClientObjectContainer)oc;
			try
			{
				if (seq % 2 == 0)
				{
					client.Get(null);
				}
				else
				{
					client.Socket().Close();
					Assert.IsFalse(oc.IsClosed());
					Assert.Expect(typeof(Db4oException), new _ICodeBlock_26(this, client));
				}
			}
			finally
			{
				oc.Close();
				Assert.IsTrue(oc.IsClosed());
			}
		}

		private sealed class _ICodeBlock_26 : ICodeBlock
		{
			public _ICodeBlock_26(ClientDisconnectTestCase _enclosing, ClientObjectContainer 
				client)
			{
				this._enclosing = _enclosing;
				this.client = client;
			}

			public void Run()
			{
				client.Get(null);
			}

			private readonly ClientDisconnectTestCase _enclosing;

			private readonly ClientObjectContainer client;
		}
	}
}
