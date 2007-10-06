/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	public class FloatHandler : IntHandler
	{
		private static readonly float i_primitive = System.Convert.ToSingle(0);

		public FloatHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Coercion4.ToFloat(obj);
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(float);
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		/// <exception cref="CorruptionException"></exception>
		public override object Read(MarshallerFamily mf, StatefulBuffer writer, bool redirect
			)
		{
			return mf._primitive.ReadFloat(writer);
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			return PrimitiveMarshaller().ReadFloat(a_bytes);
		}

		public override void Write(object a_object, Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			WriteInt(Sharpen.Runtime.FloatToIntBits(((float)a_object)), a_bytes);
		}

		private float i_compareTo;

		private float Valu(object obj)
		{
			return ((float)obj);
		}

		internal override void PrepareComparison1(object obj)
		{
			i_compareTo = Valu(obj);
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is float && Valu(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is float && Valu(obj) > i_compareTo;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is float && Valu(obj) < i_compareTo;
		}

		public override object Read(IReadContext context)
		{
			return Sharpen.Runtime.IntBitsToFloat(context.ReadInt());
		}

		public override void Write(IWriteContext context, object obj)
		{
			context.WriteInt(Sharpen.Runtime.FloatToIntBits(((float)obj)));
		}
	}
}
