/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class DoubleHandler : LongHandler
	{
		private static readonly double DEFAULT_VALUE = System.Convert.ToDouble(0);

		public DoubleHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Coercion4.ToDouble(obj);
		}

		public override object DefaultValue()
		{
			return DEFAULT_VALUE;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(double);
		}

		public override object PrimitiveNull()
		{
			return DEFAULT_VALUE;
		}

		public override object Read(MarshallerFamily mf, StatefulBuffer buffer, bool redirect
			)
		{
			return mf._primitive.ReadDouble(buffer);
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer buffer)
		{
			return PrimitiveMarshaller().ReadDouble(buffer);
		}

		public override void Write(object a_object, Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			a_bytes.WriteLong(Platform4.DoubleToLong(((double)a_object)));
		}

		private double i_compareToDouble;

		private double Dval(object obj)
		{
			return ((double)obj);
		}

		internal override void PrepareComparison1(object obj)
		{
			i_compareToDouble = Dval(obj);
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is double && Dval(obj) == i_compareToDouble;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is double && Dval(obj) > i_compareToDouble;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is double && Dval(obj) < i_compareToDouble;
		}

		public override object Read(IReadContext context)
		{
			long l = (long)base.Read(context);
			return Platform4.LongToDouble(l);
		}

		public override void Write(IWriteContext context, object obj)
		{
			context.WriteLong(Platform4.DoubleToLong(((double)obj)));
		}
	}
}
