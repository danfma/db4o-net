using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class IntHandler : PrimitiveHandler
	{
		private static readonly int i_primitive = 0;

		public IntHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Coercion4.ToInt(obj);
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int GetID()
		{
			return 1;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(int);
		}

		public override int LinkLength()
		{
			return Const4.INT_LENGTH;
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		public override object Read(MarshallerFamily mf, StatefulBuffer writer, bool redirect
			)
		{
			return mf._primitive.ReadInteger(writer);
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			return a_bytes.ReadInt();
		}

		public override void Write(object obj, Db4objects.Db4o.Internal.Buffer writer)
		{
			Write(((int)obj), writer);
		}

		public virtual void Write(int intValue, Db4objects.Db4o.Internal.Buffer writer)
		{
			WriteInt(intValue, writer);
		}

		public static void WriteInt(int a_int, Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			a_bytes.WriteInt(a_int);
		}

		private int i_compareTo;

		protected int Val(object obj)
		{
			return ((int)obj);
		}

		public virtual int CompareTo(int other)
		{
			return other - i_compareTo;
		}

		public virtual void PrepareComparison(int i)
		{
			i_compareTo = i;
		}

		internal override void PrepareComparison1(object obj)
		{
			PrepareComparison(Val(obj));
		}

		public override object Current1()
		{
			return CurrentInt();
		}

		public virtual int CurrentInt()
		{
			return i_compareTo;
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is int && Val(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is int && Val(obj) > i_compareTo;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is int && Val(obj) < i_compareTo;
		}

		public override void DefragIndexEntry(ReaderPair readers)
		{
			readers.IncrementIntSize();
		}
	}
}
