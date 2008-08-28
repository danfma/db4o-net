/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Migration;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
	public class AllCommonTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new AllCommonTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(Db4oMigrationTestSuite), typeof(FieldsToTypeHandlerMigrationTestCase
				), typeof(TranslatorToTypehandlerMigrationTestCase) };
		}
	}
}
