﻿/* Copyright (C) 2009 Versant Inc.   http://www.db4o.com */
using System;
using System.Threading;
using System.Windows.Browser;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Extensions;

namespace Db4objects.Db4o.Silverlight.TestStart
{
	public partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();
			ThreadPool.QueueUserWorkItem(unused => TestRunnerEntry());
		}

		private void TestRunnerEntry()
		{
			try
			{
				Type[] testCases = new[] { typeof(Tests.Common.AllTests) };
				new TestRunner(SilverlightSuiteSuite(testCases)).Run(new SilverlightTestListener(Dispatcher));

				Complete();
			}
			catch(Exception ex)
			{
				AppendException(ex);
			}
		}

		private void Complete()
		{
			Dispatcher.BeginInvoke(() => HtmlPage.Window.Eval("completed();"));
		}

		private void AppendException(Exception exception)
		{
			Dispatcher.BeginInvoke(() => HtmlPage.Window.Eval("appendException(\"" + exception.ToJScriptString() + "\");"));
		}

		private static Db4oTestSuiteBuilder SilverlightSuite(params Type[] testCases)
		{
			return new Db4oTestSuiteBuilder(new SilverlightFixture(), testCases);
		}
	}
}
