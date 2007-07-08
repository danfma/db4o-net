/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Reflect.Custom;

namespace Db4objects.Db4o.Tests.Common.Reflect.Custom
{
	public class CustomClass : IReflectClass
	{
		public CustomClassRepository _repository;

		public string _name;

		public IReflectField[] _fields;

		public CustomClass()
		{
		}

		public CustomClass(CustomClassRepository repository, string name, string[] fieldNames
			, Type[] fieldTypes)
		{
			_repository = repository;
			_name = name;
			_fields = CreateFields(fieldNames, fieldTypes);
		}

		private IReflectField[] CreateFields(string[] fieldNames, Type[] fieldTypes)
		{
			IReflectField[] fields = new IReflectField[fieldNames.Length + 1];
			for (int i = 0; i < fieldNames.Length; ++i)
			{
				fields[i] = new Db4objects.Db4o.Tests.Common.Reflect.Custom.CustomField(_repository
					, i, fieldNames[i], fieldTypes[i]);
			}
			fields[fields.Length - 1] = new CustomUidField(_repository);
			return fields;
		}

		public virtual IReflectClass GetComponentType()
		{
			throw new NotImplementedException();
		}

		public virtual IReflectConstructor[] GetDeclaredConstructors()
		{
			throw new NotImplementedException();
		}

		public virtual Db4objects.Db4o.Tests.Common.Reflect.Custom.CustomField CustomField
			(string name)
		{
			return (Db4objects.Db4o.Tests.Common.Reflect.Custom.CustomField)GetDeclaredField(
				name);
		}

		public virtual IReflectField GetDeclaredField(string name)
		{
			for (int i = 0; i < _fields.Length; ++i)
			{
				IReflectField field = _fields[i];
				if (field.GetName().Equals(name))
				{
					return field;
				}
			}
			return null;
		}

		public virtual IReflectField[] GetDeclaredFields()
		{
			return _fields;
		}

		public virtual IReflectClass GetDelegate()
		{
			return this;
		}

		public virtual IReflectMethod GetMethod(string methodName, IReflectClass[] paramClasses
			)
		{
			return null;
		}

		public virtual string GetName()
		{
			return _name;
		}

		public virtual IReflectClass GetSuperclass()
		{
			return null;
		}

		public virtual bool IsAbstract()
		{
			return false;
		}

		public virtual bool IsArray()
		{
			return false;
		}

		public virtual bool IsAssignableFrom(IReflectClass type)
		{
			return Equals(type);
		}

		public virtual bool IsCollection()
		{
			return false;
		}

		public virtual bool IsInstance(object obj)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsInterface()
		{
			return false;
		}

		public virtual bool IsPrimitive()
		{
			return false;
		}

		public virtual bool IsSecondClass()
		{
			return false;
		}

		public virtual object NewInstance()
		{
			return new PersistentEntry(_name, null, new object[_fields.Length - 1]);
		}

		public virtual IReflector Reflector()
		{
			throw new NotImplementedException();
		}

		public virtual bool SkipConstructor(bool flag, bool testConstructor)
		{
			return false;
		}

		public virtual object[] ToArray(object obj)
		{
			throw new NotImplementedException();
		}

		public virtual void UseConstructor(IReflectConstructor constructor, object[] @params
			)
		{
			throw new NotImplementedException();
		}

		public virtual IEnumerator CustomFields()
		{
			return Iterators.Filter(_fields, new _IPredicate4_129(this));
		}

		private sealed class _IPredicate4_129 : IPredicate4
		{
			public _IPredicate4_129(CustomClass _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Match(object candidate)
			{
				return candidate is Db4objects.Db4o.Tests.Common.Reflect.Custom.CustomField;
			}

			private readonly CustomClass _enclosing;
		}
	}
}
