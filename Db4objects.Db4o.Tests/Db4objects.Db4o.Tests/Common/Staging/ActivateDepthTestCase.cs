/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Staging;

namespace Db4objects.Db4o.Tests.Common.Staging
{
	/// <exclude></exclude>
	public class ActivateDepthTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ActivateDepthTestCase().RunAll();
		}

		public class Data
		{
			public int value;

			public Data(int i)
			{
				value = i;
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.ActivationDepth(0);
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new ActivateDepthTestCase.Data(42));
		}

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			ActivateDepthTestCase.Data data = (ActivateDepthTestCase.Data)RetrieveOnlyInstance
				(typeof(ActivateDepthTestCase.Data));
			Assert.AreEqual(0, data.value);
		}
	}
}
