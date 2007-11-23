/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Mocking;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Tests.Common.Activation;

namespace Db4objects.Db4o.Tests.Common.Activation
{
	/// <summary>
	/// Ensures the container uses the provided ActivationDepthProvider instance
	/// whenever necessary.
	/// </summary>
	/// <remarks>
	/// Ensures the container uses the provided ActivationDepthProvider instance
	/// whenever necessary.
	/// </remarks>
	public class ActivationDepthProviderConfigTestCase : AbstractDb4oTestCase, IOptOutTA
	{
		public sealed class ItemRoot
		{
			public ActivationDepthProviderConfigTestCase.Item root;

			public ItemRoot(ActivationDepthProviderConfigTestCase.Item root_)
			{
				this.root = root_;
			}

			public ItemRoot()
			{
			}
		}

		public sealed class Item
		{
			public ActivationDepthProviderConfigTestCase.Item child;

			public Item()
			{
			}

			public Item(ActivationDepthProviderConfigTestCase.Item child_)
			{
				this.child = child_;
			}
		}

		private readonly MockActivationDepthProvider _dummyProvider = new MockActivationDepthProvider
			();

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			((Config4Impl)config).ActivationDepthProvider(_dummyProvider);
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new ActivationDepthProviderConfigTestCase.ItemRoot(new ActivationDepthProviderConfigTestCase.Item
				(new ActivationDepthProviderConfigTestCase.Item(new ActivationDepthProviderConfigTestCase.Item
				()))));
		}

		public virtual void TestCSActivationDepthFor()
		{
			if (!IsNetworkCS())
			{
				return;
			}
			ResetProvider();
			QueryItem();
			AssertProviderCalled(new MethodCall[] { new MethodCall("activationDepthFor", ItemRootMetadata
				(), ActivationMode.PREFETCH), new MethodCall("activationDepthFor", ItemRootMetadata
				(), ActivationMode.ACTIVATE) });
		}

		public virtual void TestSoloActivationDepthFor()
		{
			if (IsNetworkCS())
			{
				return;
			}
			ResetProvider();
			QueryItem();
			AssertProviderCalled("activationDepthFor", ItemRootMetadata(), ActivationMode.ACTIVATE
				);
		}

		public virtual void TestSpecificActivationDepth()
		{
			ActivationDepthProviderConfigTestCase.Item item = QueryItem();
			ResetProvider();
			Db().Activate(item, 3);
			AssertProviderCalled("activationDepth", 3, ActivationMode.ACTIVATE);
		}

		private bool IsNetworkCS()
		{
			return IsClientServer() && !IsMTOC();
		}

		private ClassMetadata ItemRootMetadata()
		{
			return ClassMetadataFor(typeof(ActivationDepthProviderConfigTestCase.ItemRoot));
		}

		private void AssertProviderCalled(MethodCall[] expectedCalls)
		{
			_dummyProvider.Verify(expectedCalls);
		}

		private void AssertProviderCalled(string methodName, object arg1, object arg2)
		{
			_dummyProvider.Verify(new MethodCall[] { new MethodCall(methodName, arg1, arg2) }
				);
		}

		private void ResetProvider()
		{
			_dummyProvider.Reset();
		}

		private ActivationDepthProviderConfigTestCase.Item QueryItem()
		{
			return ((ActivationDepthProviderConfigTestCase.ItemRoot)RetrieveOnlyInstance(typeof(
				ActivationDepthProviderConfigTestCase.ItemRoot))).root;
		}
	}
}