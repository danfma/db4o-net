/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Common.References
{
	public class ReferenceSystemRegistryTestCase : ITestLifeCycle
	{
		private ReferenceSystemRegistry _registry;

		private IReferenceSystem _referenceSystem1;

		private IReferenceSystem _referenceSystem2;

		private static int TEST_ID = 5;

		public virtual void SetUp()
		{
			_registry = new ReferenceSystemRegistry();
			_referenceSystem1 = new TransactionalReferenceSystem();
			_referenceSystem2 = new TransactionalReferenceSystem();
			_registry.AddReferenceSystem(_referenceSystem1);
			_registry.AddReferenceSystem(_referenceSystem2);
		}

		public virtual void TearDown()
		{
		}

		public virtual void TestRemoveId()
		{
			AddTestReference();
			_registry.RemoveId(TEST_ID);
			AssertTestReferenceNotPresent();
		}

		public virtual void TestRemoveNull()
		{
			_registry.RemoveObject(null);
		}

		public virtual void TestRemoveObject()
		{
			ObjectReference testReference = AddTestReference();
			_registry.RemoveObject(testReference.GetObject());
			AssertTestReferenceNotPresent();
		}

		public virtual void TestRemoveReference()
		{
			ObjectReference testReference = AddTestReference();
			_registry.RemoveReference(testReference);
			AssertTestReferenceNotPresent();
		}

		public virtual void TestRemoveReferenceSystem()
		{
			AddTestReference();
			_registry.RemoveReferenceSystem(_referenceSystem1);
			_registry.RemoveId(TEST_ID);
			Assert.IsNotNull(_referenceSystem1.ReferenceForId(TEST_ID));
			Assert.IsNull(_referenceSystem2.ReferenceForId(TEST_ID));
		}

		private void AssertTestReferenceNotPresent()
		{
			Assert.IsNull(_referenceSystem1.ReferenceForId(TEST_ID));
			Assert.IsNull(_referenceSystem2.ReferenceForId(TEST_ID));
		}

		private ObjectReference AddTestReference()
		{
			ObjectReference @ref = new ObjectReference(TEST_ID);
			@ref.SetObject(new object());
			_referenceSystem1.AddExistingReference(@ref);
			_referenceSystem2.AddExistingReference(@ref);
			return @ref;
		}
	}
}