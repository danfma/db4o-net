/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class SimplestPossibleParentChildTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new SimplestPossibleParentChildTestCase().RunAll();
		}

		public class ParentItem
		{
			public object child;
		}

		public class ChildItem
		{
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			SimplestPossibleParentChildTestCase.ParentItem parentItem = new SimplestPossibleParentChildTestCase.ParentItem
				();
			parentItem.child = new SimplestPossibleParentChildTestCase.ChildItem();
			Store(parentItem);
		}

		public virtual void Test()
		{
			SimplestPossibleParentChildTestCase.ParentItem parentItem = (SimplestPossibleParentChildTestCase.ParentItem
				)RetrieveOnlyInstance(typeof(SimplestPossibleParentChildTestCase.ParentItem));
			Assert.IsInstanceOf(typeof(SimplestPossibleParentChildTestCase.ChildItem), parentItem
				.child);
		}
	}
}