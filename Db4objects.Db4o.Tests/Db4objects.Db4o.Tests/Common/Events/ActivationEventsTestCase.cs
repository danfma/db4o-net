/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class ActivationEventsTestCase : EventsTestCaseBase
	{
		protected override void Configure(IConfiguration config)
		{
			config.ActivationDepth(1);
		}

		public virtual void TestActivationEvents()
		{
			EventsTestCaseBase.EventLog activationLog = new EventsTestCaseBase.EventLog();
			EventRegistry().Activating += new Db4objects.Db4o.Events.CancellableObjectEventHandler
				(new _AnonymousInnerClass19(this, activationLog).OnEvent);
			EventRegistry().Activated += new Db4objects.Db4o.Events.ObjectEventHandler(new _AnonymousInnerClass24
				(this, activationLog).OnEvent);
			RetrieveOnlyInstance(typeof(EventsTestCaseBase.Item));
			Assert.IsTrue(activationLog.xing);
			Assert.IsTrue(activationLog.xed);
		}

		private sealed class _AnonymousInnerClass19
		{
			public _AnonymousInnerClass19(ActivationEventsTestCase _enclosing, EventsTestCaseBase.EventLog
				 activationLog)
			{
				this._enclosing = _enclosing;
				this.activationLog = activationLog;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CancellableObjectEventArgs
				 args)
			{
				activationLog.xing = true;
			}

			private readonly ActivationEventsTestCase _enclosing;

			private readonly EventsTestCaseBase.EventLog activationLog;
		}

		private sealed class _AnonymousInnerClass24
		{
			public _AnonymousInnerClass24(ActivationEventsTestCase _enclosing, EventsTestCaseBase.EventLog
				 activationLog)
			{
				this._enclosing = _enclosing;
				this.activationLog = activationLog;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				activationLog.xed = true;
			}

			private readonly ActivationEventsTestCase _enclosing;

			private readonly EventsTestCaseBase.EventLog activationLog;
		}
	}
}
