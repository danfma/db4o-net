/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ServerPortUsedTestCase : Db4oClientServerTestCase
	{
		private static readonly string DB = "PortUsed.db";

		public static void Main(string[] args)
		{
			new ServerPortUsedTestCase().RunAll();
		}

		protected override void Db4oTearDownBeforeClean()
		{
			File4.Delete(DB);
		}

		public virtual void Test()
		{
			int port = ClientServerFixture().ServerPort();
			Assert.Expect(typeof(Db4oIOException), new _ICodeBlock_26(this, port));
		}

		private sealed class _ICodeBlock_26 : ICodeBlock
		{
			public _ICodeBlock_26(ServerPortUsedTestCase _enclosing, int port)
			{
				this._enclosing = _enclosing;
				this.port = port;
			}

			public void Run()
			{
				Db4oFactory.OpenServer(ServerPortUsedTestCase.DB, port);
			}

			private readonly ServerPortUsedTestCase _enclosing;

			private readonly int port;
		}
	}
}