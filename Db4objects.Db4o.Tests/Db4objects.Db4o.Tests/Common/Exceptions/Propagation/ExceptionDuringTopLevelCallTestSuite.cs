/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Exceptions;
using Db4objects.Db4o.Tests.Common.Exceptions.Propagation;

namespace Db4objects.Db4o.Tests.Common.Exceptions.Propagation
{
	public class ExceptionDuringTopLevelCallTestSuite : FixtureBasedTestSuite, IDb4oTestCase
		, IOptOutNetworkingCS
	{
		public class ExceptionDuringTopLevelCallTestUnit : AbstractDb4oTestCase
		{
			private ExceptionSimulatingStorage _storage;

			private object _unactivated;

			public class Item
			{
				public string _name;
			}

			/// <exception cref="System.Exception"></exception>
			protected override void Configure(IConfiguration config)
			{
				IExceptionPropagationFixture propagationFixture = CurPropagationFixture();
				_storage = new ExceptionSimulatingStorage(config.Storage, new _IExceptionFactory_25
					(propagationFixture));
				config.Storage = _storage;
			}

			private sealed class _IExceptionFactory_25 : IExceptionFactory
			{
				public _IExceptionFactory_25(IExceptionPropagationFixture propagationFixture)
				{
					this.propagationFixture = propagationFixture;
					this._alreadyCalled = false;
				}

				private bool _alreadyCalled;

				public void ThrowException()
				{
					try
					{
						if (!this._alreadyCalled)
						{
							propagationFixture.ThrowInitialException();
						}
						else
						{
							propagationFixture.ThrowShutdownException();
						}
					}
					finally
					{
						this._alreadyCalled = true;
					}
				}

				public void ThrowOnClose()
				{
					propagationFixture.ThrowCloseException();
				}

				private readonly IExceptionPropagationFixture propagationFixture;
			}

			/// <exception cref="System.Exception"></exception>
			protected override void Db4oSetupAfterStore()
			{
				Store(new ExceptionDuringTopLevelCallTestSuite.ExceptionDuringTopLevelCallTestUnit.Item
					());
			}

			public virtual void TestExceptionDuringTopLevelCall()
			{
				_unactivated = ((ExceptionDuringTopLevelCallTestSuite.ExceptionDuringTopLevelCallTestUnit.Item
					)RetrieveOnlyInstance(typeof(ExceptionDuringTopLevelCallTestSuite.ExceptionDuringTopLevelCallTestUnit.Item
					)));
				Db().Deactivate(_unactivated);
				_storage.TriggerException(true);
				CurPropagationFixture().AssertExecute(new DatabaseContext(Db(), _unactivated), CurTopLevelFixture
					());
			}

			private IExceptionPropagationFixture CurPropagationFixture()
			{
				return ((IExceptionPropagationFixture)PropagationFixture.Value);
			}

			private TopLevelOperation CurTopLevelFixture()
			{
				return ((TopLevelOperation)ToplevelFixture.Value);
			}

			/// <exception cref="System.Exception"></exception>
			protected override void Db4oTearDownBeforeClean()
			{
				_storage.TriggerException(false);
			}
		}

		private static FixtureVariable PropagationFixture = FixtureVariable.NewInstance("exc"
			);

		private static FixtureVariable ToplevelFixture = FixtureVariable.NewInstance("op"
			);

		public override IFixtureProvider[] FixtureProviders()
		{
			return new IFixtureProvider[] { new Db4oFixtureProvider(), new SimpleFixtureProvider
				(PropagationFixture, new object[] { new OutOfMemoryErrorPropagationFixture(), new 
				OneTimeDb4oExceptionPropagationFixture(), new OneTimeRuntimeExceptionPropagationFixture
				(), new RecurringDb4oExceptionPropagationFixture(), new RecurringRuntimeExceptionPropagationFixture
				(), new RecoverableExceptionPropagationFixture() }), new SimpleFixtureProvider(ToplevelFixture
				, new object[] { new _TopLevelOperation_84("commit"), new _TopLevelOperation_88(
				"store"), new _TopLevelOperation_92("activate"), new _TopLevelOperation_106("peek"
				), new _TopLevelOperation_110("qbe"), new _TopLevelOperation_114("query") }) };
		}

		private sealed class _TopLevelOperation_84 : TopLevelOperation
		{
			public _TopLevelOperation_84(string baseArg1) : base(baseArg1)
			{
			}

			public override void Apply(DatabaseContext context)
			{
				context._db.Commit();
			}
		}

		private sealed class _TopLevelOperation_88 : TopLevelOperation
		{
			public _TopLevelOperation_88(string baseArg1) : base(baseArg1)
			{
			}

			public override void Apply(DatabaseContext context)
			{
				context._db.Store(new Item());
			}
		}

		private sealed class _TopLevelOperation_92 : TopLevelOperation
		{
			public _TopLevelOperation_92(string baseArg1) : base(baseArg1)
			{
			}

			public override void Apply(DatabaseContext context)
			{
				context._db.Activate(context._unactivated, int.MaxValue);
			}
		}

		private sealed class _TopLevelOperation_106 : TopLevelOperation
		{
			public _TopLevelOperation_106(string baseArg1) : base(baseArg1)
			{
			}

			// - no deactivate test, since it doesn't trigger I/O activity
			// - no getByID test, not refactored to asTopLevelCall, since it has custom, more relaxed exception handling -> InvalidSlotExceptionTestCase
			// FIXME doesn't trigger initial exception - deletes are processed in finally block
			//					new TopLevelOperation("delete") {
			//						@Override
			//						public void apply(DatabaseContext context) {
			//							context._db.delete(context._unactivated);
			//						}
			//					},
			public override void Apply(DatabaseContext context)
			{
				context._db.Ext().PeekPersisted(context._unactivated, 1, true);
			}
		}

		private sealed class _TopLevelOperation_110 : TopLevelOperation
		{
			public _TopLevelOperation_110(string baseArg1) : base(baseArg1)
			{
			}

			public override void Apply(DatabaseContext context)
			{
				context._db.QueryByExample(new Item());
			}
		}

		private sealed class _TopLevelOperation_114 : TopLevelOperation
		{
			public _TopLevelOperation_114(string baseArg1) : base(baseArg1)
			{
			}

			public override void Apply(DatabaseContext context)
			{
				IObjectSet result = context._db.Query().Execute();
				if (result.HasNext())
				{
					result.Next();
				}
			}
		}

		public override Type[] TestUnits()
		{
			return new Type[] { typeof(ExceptionDuringTopLevelCallTestSuite.ExceptionDuringTopLevelCallTestUnit
				) };
		}
	}
}
