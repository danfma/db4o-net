/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Drs.Inside.Traversal
{
	public class FieldIterators
	{
		public static IEnumerator PersistentFields(IReflectClass claxx)
		{
			return Iterators.Filter(claxx.GetDeclaredFields(), new _IPredicate4_13());
		}

		private sealed class _IPredicate4_13 : IPredicate4
		{
			public _IPredicate4_13()
			{
			}

			public bool Match(object candidate)
			{
				IReflectField field = (IReflectField)candidate;
				if (field.IsStatic())
				{
					return false;
				}
				if (field.IsTransient())
				{
					return false;
				}
				if (Platform4.IsTransient(field.GetFieldType()))
				{
					return false;
				}
				return true;
			}
		}
	}
}