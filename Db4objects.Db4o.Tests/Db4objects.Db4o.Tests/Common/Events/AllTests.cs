/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Events.AllTests().RunAll();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(ActivationEventsTestCase), typeof(ClassRegistrationEventsTestCase
				), typeof(CreationEventsTestCase), typeof(DeleteOnDeletingCallbackTestCase), typeof(
				DeletionEventExceptionTestCase), typeof(DeletionEventsTestCase), typeof(EventArgsTransactionTestCase
				), typeof(InstantiationEventsTestCase), typeof(ObjectContainerEventsTestCase), typeof(
				ObjectContainerOpenEventTestCase), typeof(EventCountTestCase), typeof(DeleteEventOnClientTestCase
				), typeof(ExceptionPropagationInEventsTestSuite), typeof(UpdateInCallbackThrowsTestCase
				) };
		}
	}
}
