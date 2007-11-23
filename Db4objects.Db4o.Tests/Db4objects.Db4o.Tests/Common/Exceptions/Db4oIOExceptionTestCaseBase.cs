/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class Db4oIOExceptionTestCaseBase : AbstractDb4oTestCase, IOptOutCS, IOptOutTA
	{
		protected override void Configure(IConfiguration config)
		{
			config.LockDatabaseFile(false);
			config.Io(new ExceptionIOAdapter());
		}

		/// <exception cref="Exception"></exception>
		protected override void Db4oSetupBeforeStore()
		{
			ExceptionIOAdapter.exception = false;
		}

		/// <exception cref="Exception"></exception>
		protected override void Db4oTearDownBeforeClean()
		{
			ExceptionIOAdapter.exception = false;
		}
	}
}
