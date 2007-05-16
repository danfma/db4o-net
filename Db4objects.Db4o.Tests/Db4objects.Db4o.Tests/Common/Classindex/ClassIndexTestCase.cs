/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Tests.Common.Btree;
using Db4objects.Db4o.Tests.Common.Classindex;

namespace Db4objects.Db4o.Tests.Common.Classindex
{
	public class ClassIndexTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		public class Item
		{
			public string name;

			public Item(string _name)
			{
				this.name = _name;
			}
		}

		public static void Main(string[] args)
		{
			new ClassIndexTestCase().RunSolo();
		}

		public virtual void TestDelete()
		{
			ClassIndexTestCase.Item item = new ClassIndexTestCase.Item("test");
			Store(item);
			int id = (int)Db().GetID(item);
			AssertID(id);
			Reopen();
			item = (ClassIndexTestCase.Item)Db().Get(item).Next();
			id = (int)Db().GetID(item);
			AssertID(id);
			Db().Delete(item);
			Db().Commit();
			AssertEmpty();
			Reopen();
			AssertEmpty();
		}

		private void AssertID(int id)
		{
			AssertIndex(new object[] { id });
		}

		private void AssertEmpty()
		{
			AssertIndex(new object[] {  });
		}

		private void AssertIndex(object[] expected)
		{
			ClassMetadata clazz = Stream().ClassMetadataForReflectClass(Reflector().ForClass(
				typeof(ClassIndexTestCase.Item)));
			ExpectingVisitor visitor = new ExpectingVisitor(expected);
			IClassIndexStrategy index = clazz.Index();
			index.TraverseAll(Trans(), visitor);
			visitor.AssertExpectations();
		}
	}
}
