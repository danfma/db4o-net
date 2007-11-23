/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Internal;

namespace Db4objects.Db4o.Tests.Common.Internal
{
	public class StoredClassTestCase : AbstractDb4oTestCase
	{
		private static readonly string FIELD_NAME = "_name";

		private static readonly string ITEM_NAME = "item";

		public class ItemParent
		{
			public string[] _array;
		}

		public class Item : StoredClassTestCase.ItemParent
		{
			public string _name;

			public Item(string name)
			{
				_name = name;
			}
		}

		private long _id;

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(StoredClassTestCase.Item)).ObjectField(FIELD_NAME).Indexed
				(true);
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			StoredClassTestCase.Item item = new StoredClassTestCase.Item(ITEM_NAME);
			Store(item);
		}

		/// <exception cref="Exception"></exception>
		protected override void Db4oSetupAfterStore()
		{
			_id = Db().GetID(RetrieveOnlyInstance(typeof(StoredClassTestCase.Item)));
		}

		public virtual void TestUnknownStoredClass()
		{
			Assert.IsNull(StoredClass(this.GetType()));
		}

		public virtual void TestStoredClassImpl()
		{
			Assert.IsInstanceOf(typeof(StoredClassImpl), ItemStoredClass());
		}

		public virtual void TestGetIds()
		{
			IStoredClass itemClass = ItemStoredClass();
			long[] ids = itemClass.GetIDs();
			Assert.AreEqual(1, ids.Length);
			Assert.AreEqual(_id, ids[0]);
		}

		public virtual void TestGetName()
		{
			IStoredClass itemClass = ItemStoredClass();
			Assert.AreEqual(Reflector().ForClass(typeof(StoredClassTestCase.Item)).GetName(), 
				itemClass.GetName());
		}

		public virtual void TestGetParentStoredClass()
		{
			IStoredClass itemClass = ItemStoredClass();
			IStoredClass parentStoredClass = itemClass.GetParentStoredClass();
			Assert.AreEqual(Reflector().ForClass(typeof(StoredClassTestCase.ItemParent)).GetName
				(), parentStoredClass.GetName());
			Assert.AreEqual(parentStoredClass, Db().StoredClass(typeof(StoredClassTestCase.ItemParent
				)));
		}

		public virtual void TestGetStoredFields()
		{
			AssertStoredField(typeof(StoredClassTestCase.Item), FIELD_NAME, ITEM_NAME, typeof(
				string), true, false);
			AssertStoredField(typeof(StoredClassTestCase.ItemParent), "_array", null, typeof(
				string), false, true);
			IStoredClass itemStoredClass = ItemStoredClass();
			IStoredField storedField = itemStoredClass.StoredField(FIELD_NAME, null);
			IStoredField sameStoredField = itemStoredClass.GetStoredFields()[0];
			IStoredField otherStoredField = StoredClass(typeof(StoredClassTestCase.ItemParent
				)).GetStoredFields()[0];
			Assert.EqualsAndHashcode(storedField, sameStoredField, otherStoredField);
			Assert.IsNull(itemStoredClass.StoredField(string.Empty, null));
		}

		private void AssertStoredField(Type objectClass, string fieldName, object expectedFieldValue
			, Type expectedFieldType, bool hasIndex, bool isArray)
		{
			IStoredClass storedClass = StoredClass(objectClass);
			IStoredField[] storedFields = storedClass.GetStoredFields();
			Assert.AreEqual(1, storedFields.Length);
			IStoredField storedField = storedFields[0];
			Assert.AreEqual(fieldName, storedField.GetName());
			IStoredField storedFieldByName = storedClass.StoredField(fieldName, expectedFieldType
				);
			Assert.AreEqual(storedField, storedFieldByName);
			object item = RetrieveOnlyInstance(objectClass);
			Assert.AreEqual(expectedFieldValue, storedField.Get(item));
			IReflectClass fieldType = storedField.GetStoredType();
			Assert.AreEqual(Reflector().ForClass(expectedFieldType), fieldType);
			Assert.AreEqual(isArray, storedField.IsArray());
			if (IsClientServer())
			{
				return;
			}
			Assert.AreEqual(hasIndex, storedField.HasIndex());
			if (!hasIndex)
			{
				Assert.Expect(typeof(Exception), new _ICodeBlock_113(this, storedField));
			}
			else
			{
				IntByRef count = new IntByRef();
				storedField.TraverseValues(new _IVisitor4_123(this, count, expectedFieldValue));
				Assert.AreEqual(1, count.value);
			}
		}

		private sealed class _ICodeBlock_113 : ICodeBlock
		{
			public _ICodeBlock_113(StoredClassTestCase _enclosing, IStoredField storedField)
			{
				this._enclosing = _enclosing;
				this.storedField = storedField;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				storedField.TraverseValues(new _IVisitor4_115(this));
			}

			private sealed class _IVisitor4_115 : IVisitor4
			{
				public _IVisitor4_115(_ICodeBlock_113 _enclosing)
				{
					this._enclosing = _enclosing;
				}

				public void Visit(object obj)
				{
				}

				private readonly _ICodeBlock_113 _enclosing;
			}

			private readonly StoredClassTestCase _enclosing;

			private readonly IStoredField storedField;
		}

		private sealed class _IVisitor4_123 : IVisitor4
		{
			public _IVisitor4_123(StoredClassTestCase _enclosing, IntByRef count, object expectedFieldValue
				)
			{
				this._enclosing = _enclosing;
				this.count = count;
				this.expectedFieldValue = expectedFieldValue;
			}

			public void Visit(object obj)
			{
				count.value++;
				Assert.AreEqual(expectedFieldValue, obj);
			}

			private readonly StoredClassTestCase _enclosing;

			private readonly IntByRef count;

			private readonly object expectedFieldValue;
		}

		public virtual void TestEqualsAndHashCode()
		{
			IStoredClass clazz = ItemStoredClass();
			IStoredClass same = ItemStoredClass();
			IStoredClass other = Db().StoredClass(typeof(StoredClassTestCase.ItemParent));
			Assert.EqualsAndHashcode(clazz, same, other);
		}

		private IStoredClass ItemStoredClass()
		{
			return StoredClass(typeof(StoredClassTestCase.Item));
		}

		private IStoredClass StoredClass(Type clazz)
		{
			return Db().StoredClass(clazz);
		}
	}
}
