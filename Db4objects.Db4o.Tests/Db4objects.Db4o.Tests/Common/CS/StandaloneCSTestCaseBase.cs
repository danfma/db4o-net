/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public abstract class StandaloneCSTestCaseBase : ITestCase
	{
		private readonly int _port = Db4oClientServer.FindFreePort();

		public sealed class Item
		{
		}

		public interface IClientBlock
		{
			void Run(IObjectContainer client);
		}

		public virtual void Test()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			Configure(config);
			IObjectServer server = Db4oFactory.OpenServer(config, DatabaseFile(), _port);
			try
			{
				server.GrantAccess("db4o", "db4o");
				RunTest();
			}
			finally
			{
				server.Close();
				File4.Delete(DatabaseFile());
			}
		}

		protected virtual void WithClient(StandaloneCSTestCaseBase.IClientBlock block)
		{
			IObjectContainer client = OpenClient();
			try
			{
				block.Run(client);
			}
			finally
			{
				client.Close();
			}
		}

		protected virtual ClientObjectContainer OpenClient()
		{
			return (ClientObjectContainer)Db4oFactory.OpenClient("localhost", _port, "db4o", 
				"db4o");
		}

		protected virtual int Port()
		{
			return _port;
		}

		protected abstract void RunTest();

		protected abstract void Configure(IConfiguration config);

		private string DatabaseFile()
		{
			return Path.Combine(Path.GetTempPath(), "cc.db4o");
		}
	}
}