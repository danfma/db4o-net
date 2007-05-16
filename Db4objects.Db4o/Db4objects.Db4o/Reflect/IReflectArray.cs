/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Reflect
{
	/// <summary>representation for java.lang.reflect.Array.</summary>
	/// <remarks>
	/// representation for java.lang.reflect.Array.
	/// <br /><br />See the respective documentation in the JDK API.
	/// </remarks>
	/// <seealso cref="IReflector">IReflector</seealso>
	public interface IReflectArray
	{
		int[] Dimensions(object arr);

		int Flatten(object a_shaped, int[] a_dimensions, int a_currentDimension, object[]
			 a_flat, int a_flatElement);

		object Get(object onArray, int index);

		IReflectClass GetComponentType(IReflectClass a_class);

		int GetLength(object array);

		bool IsNDimensional(IReflectClass a_class);

		object NewInstance(IReflectClass componentType, int length);

		object NewInstance(IReflectClass componentType, int[] dimensions);

		void Set(object onArray, int index, object element);

		int Shape(object[] a_flat, int a_flatElement, object a_shaped, int[] a_dimensions
			, int a_currentDimension);
	}
}
