/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Typehandlers
{
	/// <summary>allows installing a Typehandler for a single class.</summary>
	/// <remarks>allows installing a Typehandler for a single class.</remarks>
	public sealed class SingleClassTypeHandlerPredicate : ITypeHandlerPredicate
	{
		private readonly Type _class;

		public SingleClassTypeHandlerPredicate(Type clazz)
		{
			_class = clazz;
		}

		public bool Match(IReflectClass classReflector)
		{
			IReflectClass reflectClass = classReflector.Reflector().ForClass(_class);
			return classReflector == reflectClass;
		}
	}
}
