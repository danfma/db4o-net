/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	/// <summary>required for CsSchemaUpdateTestCase</summary>
	public class CsSchemaMigrationSourceCode
	{
		public class Item
		{
			//update
			//assert
		}

		private static readonly string File = "csmig.db4o";

		private const int Port = 4447;

		/// <exception cref="IOException"></exception>
		public static void Main(string[] arguments)
		{
			new CsSchemaMigrationSourceCode().Run();
		}

		public virtual void Run()
		{
			//store
			IConfiguration conf = Db4oFactory.NewConfiguration();
			IObjectServer server = Db4oFactory.OpenServer(conf, File, Port);
			server.GrantAccess("db4o", "db4o");
			//store
			//update
			//assert
			server.Close();
		}

		//assert
		private void StoreItem()
		{
			IObjectContainer client = OpenClient();
			CsSchemaMigrationSourceCode.Item item = new CsSchemaMigrationSourceCode.Item();
			client.Store(item);
			client.Close();
		}

		//store
		private void UpdateItem()
		{
			IObjectContainer client = OpenClient();
			IQuery query = client.Query();
			query.Constrain(typeof(CsSchemaMigrationSourceCode.Item));
			CsSchemaMigrationSourceCode.Item item = (CsSchemaMigrationSourceCode.Item)query.Execute
				().Next();
			//update
			//assert
			client.Store(item);
			client.Close();
		}

		private IObjectContainer OpenClient()
		{
			return Db4oFactory.OpenClient(Db4oFactory.NewConfiguration(), "localhost", Port, 
				"db4o", "db4o");
		}

		private void AssertItem()
		{
			IObjectContainer client = OpenClient();
			IQuery query = client.Query();
			query.Constrain(typeof(CsSchemaMigrationSourceCode.Item));
			CsSchemaMigrationSourceCode.Item item = (CsSchemaMigrationSourceCode.Item)query.Execute
				().Next();
			Sharpen.Runtime.Out.WriteLine(item);
			client.Close();
		}
	}
}