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
	public sealed class ShortHandler : PrimitiveHandler
	{
		internal const int LENGTH = Const4.SHORT_BYTES + Const4.ADDED_LENGTH;

		private static readonly short i_primitive = (short)0;

		public ShortHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Coercion4.ToShort(obj);
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int GetID()
		{
			return 8;
		}

		public override int LinkLength()
		{
			return LENGTH;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(short);
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		public override object Read(MarshallerFamily mf, StatefulBuffer buffer, bool redirect
			)
		{
			return mf._primitive.ReadShort(buffer);
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer buffer)
		{
			return PrimitiveMarshaller().ReadShort(buffer);
		}

		public override void Write(object a_object, Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			WriteShort(((short)a_object), a_bytes);
		}

		internal static void WriteShort(int a_short, Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			for (int i = 0; i < Const4.SHORT_BYTES; i++)
			{
				a_bytes._buffer[a_bytes._offset++] = (byte)(a_short >> ((Const4.SHORT_BYTES - 1 -
					 i) * 8));
			}
		}

		private short i_compareTo;

		private short Val(object obj)
		{
			return ((short)obj);
		}

		internal override void PrepareComparison1(object obj)
		{
			i_compareTo = Val(obj);
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is short && Val(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is short && Val(obj) > i_compareTo;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is short && Val(obj) < i_compareTo;
		}

		public override object Read(IReadContext context)
		{
			int value = 0;
			for (int i = 0; i < Const4.SHORT_BYTES; i++)
			{
				value = ((value << 8) + context.ReadByte());
			}
			return (short)value;
		}

		public override void Write(IWriteContext context, object obj)
		{
			short shortValue = ((short)obj);
			context.WriteBytes(new byte[] { (byte)(shortValue >> 8), (byte)shortValue });
		}
	}
}
