/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class IncompatibleFileFormatExceptionTestCase : IDb4oTestCase, ITestLifeCycle
	{
		/// <exception cref="System.Exception"></exception>
		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(IncompatibleFileFormatExceptionTestCase)).Run();
		}

		private static readonly string IncompatibleFileFormat = Path.GetTempFileName();

		/// <exception cref="System.Exception"></exception>
		public virtual void SetUp()
		{
			File4.Delete(IncompatibleFileFormat);
			IoAdapter adapter = new RandomAccessFileAdapter();
			adapter = adapter.Open(IncompatibleFileFormat, false, 0, false);
			adapter.Write(new byte[] { 1, 2, 3 }, 3);
			adapter.Close();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TearDown()
		{
			File4.Delete(IncompatibleFileFormat);
		}

		public virtual void Test()
		{
			Assert.Expect(typeof(IncompatibleFileFormatException), new _ICodeBlock_33());
			File4.Delete(IncompatibleFileFormat);
			IoAdapter adapter = new RandomAccessFileAdapter();
			Assert.IsFalse(adapter.Exists(IncompatibleFileFormat));
		}

		private sealed class _ICodeBlock_33 : ICodeBlock
		{
			public _ICodeBlock_33()
			{
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				Db4oFactory.OpenFile(IncompatibleFileFormatExceptionTestCase.IncompatibleFileFormat
					);
			}
		}
	}
}
