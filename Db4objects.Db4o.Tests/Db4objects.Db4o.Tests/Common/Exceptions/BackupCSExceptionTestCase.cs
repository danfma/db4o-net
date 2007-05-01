using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class BackupCSExceptionTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new BackupCSExceptionTestCase().RunAll();
		}

		public virtual void Test()
		{
			Assert.Expect(typeof(NotSupportedException), new _AnonymousInnerClass15(this));
		}

		private sealed class _AnonymousInnerClass15 : ICodeBlock
		{
			public _AnonymousInnerClass15(BackupCSExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Backup(string.Empty);
			}

			private readonly BackupCSExceptionTestCase _enclosing;
		}
	}
}