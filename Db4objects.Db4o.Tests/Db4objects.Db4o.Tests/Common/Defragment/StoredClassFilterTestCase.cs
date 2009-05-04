/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions.Util;
using Db4objects.Db4o;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Defragment;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class StoredClassFilterTestCase : ITestCase
	{
		private static readonly string Db4oBackup = BuildTempPath("defrag.db4o.backup");

		private static readonly string Db4oFile = BuildTempPath("defrag.db4o");

		public class SimpleClass
		{
			public string _simpleField;

			public SimpleClass(string simple)
			{
				_simpleField = simple;
			}
		}

		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(StoredClassFilterTestCase)).Run();
		}

		private static string BuildTempPath(string fname)
		{
			return IOServices.BuildTempPath(fname);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test()
		{
			DeleteAllFiles();
			string fname = CreateDatabase();
			Defrag(fname);
			AssertStoredClasses(fname);
		}

		private void DeleteAllFiles()
		{
			File4.Delete(Db4oFile);
			File4.Delete(Db4oBackup);
		}

		private void AssertStoredClasses(string fname)
		{
			IObjectContainer db = Db4oFactory.OpenFile(fname);
			try
			{
				IReflectClass[] knownClasses = db.Ext().KnownClasses();
				AssertKnownClasses(knownClasses);
			}
			finally
			{
				db.Close();
			}
		}

		private void AssertKnownClasses(IReflectClass[] knownClasses)
		{
			for (int i = 0; i < knownClasses.Length; i++)
			{
				Assert.AreNotEqual(FullyQualifiedName(typeof(StoredClassFilterTestCase.SimpleClass
					)), knownClasses[i].GetName());
			}
		}

		private string FullyQualifiedName(Type klass)
		{
			return CrossPlatformServices.FullyQualifiedName(klass);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Defrag(string fname)
		{
			DefragmentConfig config = new DefragmentConfig(fname);
			config.StoredClassFilter(IgnoreClassFilter(typeof(StoredClassFilterTestCase.SimpleClass
				)));
			Db4objects.Db4o.Defragment.Defragment.Defrag(config);
		}

		private IStoredClassFilter IgnoreClassFilter(Type klass)
		{
			return new _IStoredClassFilter_73(this, klass);
		}

		private sealed class _IStoredClassFilter_73 : IStoredClassFilter
		{
			public _IStoredClassFilter_73(StoredClassFilterTestCase _enclosing, Type klass)
			{
				this._enclosing = _enclosing;
				this.klass = klass;
			}

			public bool Accept(IStoredClass storedClass)
			{
				return !storedClass.GetName().Equals(this._enclosing.FullyQualifiedName(klass));
			}

			private readonly StoredClassFilterTestCase _enclosing;

			private readonly Type klass;
		}

		private string CreateDatabase()
		{
			string fname = Db4oFile;
			IObjectContainer db = Db4oFactory.OpenFile(fname);
			try
			{
				db.Store(new StoredClassFilterTestCase.SimpleClass("verySimple"));
				db.Commit();
			}
			finally
			{
				db.Close();
			}
			return fname;
		}
	}
}
