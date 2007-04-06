using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Stored;

namespace Db4objects.Db4o.Tests.Common.Stored
{
	public class ArrayStoredTypeTestCase : AbstractDb4oTestCase
	{
		public class Data
		{
			public bool[] _primitiveBoolean;

			public bool[] _wrapperBoolean;

			public int[] _primitiveInt;

			public int[] _wrapperInteger;

			public Data(bool[] primitiveBoolean, bool[] wrapperBoolean, int[] primitiveInteger
				, int[] wrapperInteger)
			{
				this._primitiveBoolean = primitiveBoolean;
				this._wrapperBoolean = wrapperBoolean;
				this._primitiveInt = primitiveInteger;
				this._wrapperInteger = wrapperInteger;
			}
		}

		protected override void Store()
		{
			ArrayStoredTypeTestCase.Data data = new ArrayStoredTypeTestCase.Data(new bool[] { 
				true, false }, new bool[] { true, false }, new int[] { 0, 1, 2 }, new int[] { 4, 
				5, 6 });
			Store(data);
		}

		public virtual void TestArrayStoredTypes()
		{
			IStoredClass clazz = Db().StoredClass(typeof(ArrayStoredTypeTestCase.Data));
			AssertStoredType(clazz, "_primitiveBoolean", typeof(bool));
			AssertStoredType(clazz, "_wrapperBoolean", typeof(bool));
			AssertStoredType(clazz, "_primitiveInt", typeof(int));
			AssertStoredType(clazz, "_wrapperInteger", typeof(int));
		}

		private void AssertStoredType(IStoredClass clazz, string fieldName, Type type)
		{
			IStoredField field = clazz.StoredField(fieldName, null);
			Assert.AreEqual(type.FullName, SimpleName(field.GetStoredType().GetName()));
		}

		private string SimpleName(string name)
		{
			int index = name.IndexOf(',');
			if (index < 0)
			{
				return name;
			}
			return Sharpen.Runtime.Substring(name, 0, index);
		}
	}
}
