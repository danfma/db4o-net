/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Extensions.Util;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Refactor;

namespace Db4objects.Db4o.Tests.Common.Refactor
{
	public class ClassRenameByStoredClassTestCase : AbstractDb4oTestCase, IOptOutNetworkingCS
	{
		private static string NAME = "test";

		public static void Main(string[] args)
		{
			new ClassRenameByStoredClassTestCase().RunAll();
		}

		public class Original
		{
			public string _name;

			public Original(string name)
			{
				this._name = name;
			}
		}

		public class Changed
		{
			public string _name;

			public string _otherName;

			public Changed(string name)
			{
				_name = name;
				_otherName = name;
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new ClassRenameByStoredClassTestCase.Original(NAME));
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWithReopen()
		{
			AssertRenamed(true);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWithoutReopen()
		{
			AssertRenamed(false);
		}

		/// <exception cref="Exception"></exception>
		private void AssertRenamed(bool doReopen)
		{
			IStoredClass originalClazz = Db().Ext().StoredClass(typeof(ClassRenameByStoredClassTestCase.Original
				));
			originalClazz.Rename(CrossPlatformServices.FullyQualifiedName(typeof(ClassRenameByStoredClassTestCase.Changed
				)));
			if (doReopen)
			{
				Reopen();
			}
			ClassRenameByStoredClassTestCase.Changed changedObject = (ClassRenameByStoredClassTestCase.Changed
				)RetrieveOnlyInstance(typeof(ClassRenameByStoredClassTestCase.Changed));
			Assert.AreEqual(NAME, changedObject._name);
			Assert.IsNull(changedObject._otherName);
		}
	}
}