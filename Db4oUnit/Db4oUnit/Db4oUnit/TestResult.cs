/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4oUnit.Util;

namespace Db4oUnit
{
	public class TestResult : Printable
	{
		private TestFailureCollection _failures = new TestFailureCollection();

		private int _testCount = 0;

		private readonly StopWatch _watch = new StopWatch();

		private readonly TextWriter _writer;

		public TestResult(TextWriter writer)
		{
			_writer = writer;
		}

		public TestResult() : this(TestPlatform.GetNullWriter())
		{
		}

		public virtual void TestStarted(ITest test)
		{
			++_testCount;
			Print(test.GetLabel());
		}

		public virtual void TestFailed(ITest test, Exception failure)
		{
			PrintFailure(failure);
			_failures.Add(new TestFailure(test, failure));
		}

		private void PrintFailure(Exception failure)
		{
			if (failure == null)
			{
				Print("\t!");
			}
			else
			{
				Print("\t! " + failure);
			}
		}

		public virtual bool Green()
		{
			return _failures.Size() == 0;
		}

		public virtual TestFailureCollection Failures()
		{
			return _failures;
		}

		/// <exception cref="IOException"></exception>
		public override void Print(TextWriter writer)
		{
			if (Green())
			{
				writer.Write("GREEN (" + _testCount + " tests) - " + ElapsedString() + TestPlatform
					.NEWLINE);
				return;
			}
			writer.Write("RED (" + _failures.Size() + " out of " + _testCount + " tests failed) - "
				 + ElapsedString() + TestPlatform.NEWLINE);
			_failures.Print(writer);
		}

		private string ElapsedString()
		{
			return _watch.ToString();
		}

		public virtual int Assertions()
		{
			return 0;
		}

		public virtual void RunStarted()
		{
			_watch.Start();
		}

		public virtual void RunFinished()
		{
			_watch.Stop();
		}

		private void Print(string message)
		{
			try
			{
				_writer.Write(message + TestPlatform.NEWLINE);
				_writer.Flush();
			}
			catch (IOException x)
			{
				TestPlatform.PrintStackTrace(_writer, x);
			}
		}
	}
}