/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class PrimitiveFieldHandler : ClassMetadata
	{
		private readonly ITypeHandler4 _handler;

		internal PrimitiveFieldHandler(ObjectContainerBase container, ITypeHandler4 handler
			, int handlerID, IReflectClass classReflector) : base(container, classReflector)
		{
			i_fields = FieldMetadata.EMPTY_ARRAY;
			_handler = handler;
			_id = handlerID;
		}

		internal override void ActivateFields(Transaction trans, object obj, int depth)
		{
		}

		internal sealed override void AddToIndex(LocalObjectContainer container, Transaction
			 trans, int id)
		{
		}

		internal override bool AllowsQueries()
		{
			return false;
		}

		internal override void CacheDirty(Collection4 col)
		{
		}

		public override void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer a_bytes)
		{
			if (mf._primitive.UseNormalClassRead())
			{
				base.DeleteEmbedded(mf, a_bytes);
				return;
			}
		}

		public override void DeleteEmbedded1(MarshallerFamily mf, StatefulBuffer a_bytes, 
			int a_id)
		{
			if (_handler is ArrayHandler)
			{
				ArrayHandler ya = (ArrayHandler)_handler;
				if (ya._usePrimitiveClassReflector)
				{
					ya.DeletePrimitiveEmbedded(a_bytes, this);
					a_bytes.SlotDelete();
					return;
				}
			}
			if (_handler is UntypedFieldHandler)
			{
				a_bytes.IncrementOffset(LinkLength());
			}
			else
			{
				_handler.DeleteEmbedded(mf, a_bytes);
			}
			Free(a_bytes, a_id);
		}

		internal override void DeleteMembers(MarshallerFamily mf, ObjectHeaderAttributes 
			attributes, StatefulBuffer a_bytes, int a_type, bool isUpdate)
		{
			if (a_type == Const4.TYPE_ARRAY)
			{
				new ArrayHandler(a_bytes.GetStream(), this, true).DeletePrimitiveEmbedded(a_bytes
					, this);
			}
			else
			{
				if (a_type == Const4.TYPE_NARRAY)
				{
					new MultidimensionalArrayHandler(a_bytes.GetStream(), this, true).DeletePrimitiveEmbedded
						(a_bytes, this);
				}
			}
		}

		internal void Free(StatefulBuffer a_bytes, int a_id)
		{
			a_bytes.GetTransaction().SlotFreePointerOnCommit(a_id, a_bytes.Slot());
		}

		public override bool HasClassIndex()
		{
			return false;
		}

		public override object Instantiate(UnmarshallingContext context)
		{
			object obj = context.PersistentObject();
			if (obj == null)
			{
				obj = context.Read(_handler);
				context.SetObjectWeak(obj);
			}
			context.SetStateClean();
			return obj;
		}

		public override object InstantiateTransient(UnmarshallingContext context)
		{
			return _handler.Read(context);
		}

		internal override void InstantiateFields(UnmarshallingContext context)
		{
			object obj = context.Read(_handler);
			if (obj != null && (_handler is Db4objects.Db4o.Internal.Handlers.DateHandler))
			{
				object existing = context.PersistentObject();
				context.PersistentObject(DateHandler().CopyValue(obj, existing));
			}
		}

		private Db4objects.Db4o.Internal.Handlers.DateHandler DateHandler()
		{
			return ((Db4objects.Db4o.Internal.Handlers.DateHandler)_handler);
		}

		public override bool IsArray()
		{
			return _id == Handlers4.ANY_ARRAY_ID || _id == Handlers4.ANY_ARRAY_N_ID;
		}

		public override bool IsPrimitive()
		{
			return true;
		}

		public override bool IsStrongTyped()
		{
			return false;
		}

		public override IComparable4 PrepareComparison(object a_constraint)
		{
			_handler.PrepareComparison(a_constraint);
			return _handler;
		}

		public override ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer[] a_bytes)
		{
			if (IsArray())
			{
				return _handler;
			}
			return null;
		}

		public override ObjectID ReadObjectID(IInternalReadContext context)
		{
			if (_handler is ClassMetadata)
			{
				return ((ClassMetadata)_handler).ReadObjectID(context);
			}
			if (_handler is ArrayHandler)
			{
				return ObjectID.IGNORE;
			}
			return ObjectID.NOT_POSSIBLE;
		}

		internal override void RemoveFromIndex(Transaction ta, int id)
		{
		}

		public sealed override bool WriteObjectBegin()
		{
			return false;
		}

		public override string ToString()
		{
			return "Wraps " + _handler.ToString() + " in YapClassPrimitive";
		}

		public override void Defrag(MarshallerFamily mf, BufferPair readers, bool redirect
			)
		{
			if (mf._primitive.UseNormalClassRead())
			{
				base.Defrag(mf, readers, redirect);
			}
			else
			{
				_handler.Defrag(mf, readers, false);
			}
		}

		public override object WrapWithTransactionContext(Transaction transaction, object
			 value)
		{
			return value;
		}

		public override object Read(IReadContext context)
		{
			return _handler.Read(context);
		}

		public override void Write(IWriteContext context, object obj)
		{
			_handler.Write(context, obj);
		}

		public override ITypeHandler4 TypeHandler()
		{
			return _handler;
		}
	}
}
