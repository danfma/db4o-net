/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Api;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ObjectServerTestCase : TestWithTempFile
	{
		private IExtObjectServer server;

		private string fileName;

		/// <exception cref="System.Exception"></exception>
		public override void SetUp()
		{
			fileName = TempFile();
			server = Db4oFactory.OpenServer(Db4oFactory.NewConfiguration(), fileName, -1).Ext
				();
			server.GrantAccess(Credentials(), Credentials());
		}

		/// <exception cref="System.Exception"></exception>
		public override void TearDown()
		{
			server.Close();
			base.TearDown();
		}

		public virtual void TestClientCount()
		{
			AssertClientCount(0);
			IObjectContainer client1 = Db4oFactory.OpenClient(Db4oFactory.NewConfiguration(), 
				"localhost", Port(), Credentials(), Credentials());
			AssertClientCount(1);
			IObjectContainer client2 = Db4oFactory.OpenClient(Db4oFactory.NewConfiguration(), 
				"localhost", Port(), Credentials(), Credentials());
			AssertClientCount(2);
			client1.Close();
			client2.Close();
		}

		// closing is asynchronous, relying on completion is hard
		// That's why there is no test here. 
		// ClientProcessesTestCase tests closing.
		private void AssertClientCount(int count)
		{
			Assert.AreEqual(count, server.ClientCount());
		}

		private int Port()
		{
			return server.Port();
		}

		private string Credentials()
		{
			return "DB4O";
		}
	}
}
#endif // !SILVERLIGHT
