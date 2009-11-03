/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class CloseServerBeforeClientTestCase : Db4oClientServerTestCase, IOptOutAllButNetworkingCS
	{
		public static void Main(string[] arguments)
		{
			new CloseServerBeforeClientTestCase().RunNetworking();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test()
		{
			IExtObjectContainer client = OpenNewSession();
			try
			{
				ClientServerFixture().Server().Close();
			}
			finally
			{
				try
				{
					client.Close();
				}
				catch (Db4oException)
				{
				}
				// database may have been closed
				try
				{
					Fixture().Close();
				}
				catch (Db4oException)
				{
				}
			}
		}
		// database may have been closed
	}
}
#endif // !SILVERLIGHT
