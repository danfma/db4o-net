/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.CS.Caching;
using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.CS.Internal.Config;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.CS.Caching;

namespace Db4objects.Db4o.Tests.Common.CS.Caching
{
	/// <summary>
	/// removed for JDK 1.1 because there is no access to the private field
	/// _clientSlotCache in ClientObjectContainer
	/// </summary>
	public class ClientSlotCacheTestCase : AbstractDb4oTestCase, IOptOutAllButNetworkingCS
	{
		private const int SlotCacheSize = 5;

		/// <exception cref="System.Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			IClientConfiguration clientConfiguration = Db4oClientServerLegacyConfigurationBridge
				.AsClientConfiguration(config);
			clientConfiguration.PrefetchSlotCacheSize = SlotCacheSize;
		}

		public virtual void TestSlotCacheIsTransactionBased()
		{
			WithCache(new _IProcedure4_29(this));
		}

		private sealed class _IProcedure4_29 : IProcedure4
		{
			public _IProcedure4_29(ClientSlotCacheTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Apply(object cache)
			{
				Transaction t1 = this._enclosing.NewTransaction();
				Transaction t2 = this._enclosing.NewTransaction();
				ByteArrayBuffer slot = new ByteArrayBuffer(0);
				((IClientSlotCache)cache).Add(t1, 42, slot);
				Assert.AreSame(slot, ((IClientSlotCache)cache).Get(t1, 42));
				Assert.IsNull(((IClientSlotCache)cache).Get(t2, 42));
				t1.Commit();
				Assert.IsNull(((IClientSlotCache)cache).Get(t1, 42));
			}

			private readonly ClientSlotCacheTestCase _enclosing;
		}

		public virtual void TestCacheIsCleanUponTransactionCommit()
		{
			AssertCacheIsCleanAfterTransactionOperation(new _IProcedure4_47());
		}

		private sealed class _IProcedure4_47 : IProcedure4
		{
			public _IProcedure4_47()
			{
			}

			public void Apply(object value)
			{
				((Transaction)value).Commit();
			}
		}

		public virtual void TestCacheIsCleanUponTransactionRollback()
		{
			AssertCacheIsCleanAfterTransactionOperation(new _IProcedure4_55());
		}

		private sealed class _IProcedure4_55 : IProcedure4
		{
			public _IProcedure4_55()
			{
			}

			public void Apply(object value)
			{
				((Transaction)value).Rollback();
			}
		}

		private void AssertCacheIsCleanAfterTransactionOperation(IProcedure4 operation)
		{
			WithCache(new _IProcedure4_63(this, operation));
		}

		private sealed class _IProcedure4_63 : IProcedure4
		{
			public _IProcedure4_63(ClientSlotCacheTestCase _enclosing, IProcedure4 operation)
			{
				this._enclosing = _enclosing;
				this.operation = operation;
			}

			public void Apply(object cache)
			{
				ByteArrayBuffer slot = new ByteArrayBuffer(0);
				((IClientSlotCache)cache).Add(this._enclosing.Trans(), 42, slot);
				operation.Apply(this._enclosing.Trans());
				Assert.IsNull(((IClientSlotCache)cache).Get(this._enclosing.Trans(), 42));
			}

			private readonly ClientSlotCacheTestCase _enclosing;

			private readonly IProcedure4 operation;
		}

		public virtual void TestSlotCacheEntryIsPurgedUponActivation()
		{
			ClientSlotCacheTestCase.Item item = new ClientSlotCacheTestCase.Item();
			Db().Store(item);
			int id = (int)Db().GetID(item);
			Db().Purge(item);
			Db().Configure().ClientServer().PrefetchDepth(1);
			WithCache(new _IProcedure4_82(this, id));
		}

		private sealed class _IProcedure4_82 : IProcedure4
		{
			public _IProcedure4_82(ClientSlotCacheTestCase _enclosing, int id)
			{
				this._enclosing = _enclosing;
				this.id = id;
			}

			public void Apply(object cache)
			{
				IObjectSet items = this._enclosing.NewQuery(typeof(ClientSlotCacheTestCase.Item))
					.Execute();
				Assert.IsNotNull(((IClientSlotCache)cache).Get(this._enclosing.Trans(), id));
				Assert.IsNotNull(((ClientSlotCacheTestCase.Item)items.Next()));
				Assert.IsNull(((IClientSlotCache)cache).Get(this._enclosing.Trans(), id), "activation should have purged slot from cache"
					);
			}

			private readonly ClientSlotCacheTestCase _enclosing;

			private readonly int id;
		}

		public virtual void TestAddOverridesExistingEntry()
		{
			WithCache(new _IProcedure4_93(this));
		}

		private sealed class _IProcedure4_93 : IProcedure4
		{
			public _IProcedure4_93(ClientSlotCacheTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Apply(object cache)
			{
				((IClientSlotCache)cache).Add(this._enclosing.Trans(), 42, new ByteArrayBuffer(0)
					);
				((IClientSlotCache)cache).Add(this._enclosing.Trans(), 42, new ByteArrayBuffer(1)
					);
				Assert.AreEqual(1, ((IClientSlotCache)cache).Get(this._enclosing.Trans(), 42).Length
					());
			}

			private readonly ClientSlotCacheTestCase _enclosing;
		}

		public virtual void TestCacheSizeIsBounded()
		{
			WithCache(new _IProcedure4_103(this));
		}

		private sealed class _IProcedure4_103 : IProcedure4
		{
			public _IProcedure4_103(ClientSlotCacheTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Apply(object cache)
			{
				for (int i = 0; i < ClientSlotCacheTestCase.SlotCacheSize + 1; i++)
				{
					((IClientSlotCache)cache).Add(this._enclosing.Trans(), i, new ByteArrayBuffer(i));
				}
				Assert.IsNull(((IClientSlotCache)cache).Get(this._enclosing.Trans(), 0));
				for (int i = 1; i < ClientSlotCacheTestCase.SlotCacheSize + 1; i++)
				{
					Assert.AreEqual(i, ((IClientSlotCache)cache).Get(this._enclosing.Trans(), i).Length
						());
				}
			}

			private readonly ClientSlotCacheTestCase _enclosing;
		}

		private void WithCache(IProcedure4 procedure)
		{
			IClientSlotCache clientSlotCache = null;
			try
			{
				clientSlotCache = (IClientSlotCache)Reflection4.GetFieldValue(Container(), "_clientSlotCache"
					);
			}
			catch (MemberAccessException e)
			{
				Assert.Fail("Can't get field _clientSlotCache on  container. " + e.ToString());
			}
			procedure.Apply(clientSlotCache);
		}

		public class Item
		{
		}
	}
}
#endif // !SILVERLIGHT
