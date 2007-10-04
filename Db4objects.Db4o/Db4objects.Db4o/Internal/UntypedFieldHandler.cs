/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	public class UntypedFieldHandler : ClassMetadata, IBuiltinTypeHandler
	{
		public UntypedFieldHandler(ObjectContainerBase container) : base(container, container
			._handlers.ICLASS_OBJECT)
		{
		}

		public override void CascadeActivation(Transaction a_trans, object a_object, int 
			a_depth, bool a_activate)
		{
			ClassMetadata yc = ForObject(a_trans, a_object, false);
			if (yc != null)
			{
				yc.CascadeActivation(a_trans, a_object, a_depth, a_activate);
			}
		}

		public override void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer reader)
		{
			mf._untyped.DeleteEmbedded(reader);
		}

		public override int GetID()
		{
			return Handlers4.UNTYPED_ID;
		}

		public override bool HasField(ObjectContainerBase a_stream, string a_path)
		{
			return a_stream.ClassCollection().FieldExists(a_path);
		}

		public override bool HasClassIndex()
		{
			return false;
		}

		public override bool HoldsAnyClass()
		{
			return true;
		}

		public override bool IsStrongTyped()
		{
			return false;
		}

		public override ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer[] a_bytes)
		{
			return mf._untyped.ReadArrayHandler(a_trans, a_bytes);
		}

		public override ObjectID ReadObjectID(IInternalReadContext context)
		{
			int payloadOffset = context.ReadInt();
			if (payloadOffset == 0)
			{
				return ObjectID.IS_NULL;
			}
			ClassMetadata classMetadata = ReadClassMetadata(context, payloadOffset);
			if (classMetadata == null)
			{
				return ObjectID.IS_NULL;
			}
			SeekSecondaryOffset(context, classMetadata);
			return classMetadata.ReadObjectID(context);
		}

		public override void Defrag(MarshallerFamily mf, BufferPair readers, bool redirect
			)
		{
			if (mf._untyped.UseNormalClassRead())
			{
				base.Defrag(mf, readers, redirect);
			}
			mf._untyped.Defrag(readers);
		}

		private bool IsArray(ITypeHandler4 handler)
		{
			if (handler is ClassMetadata)
			{
				return ((ClassMetadata)handler).IsArray();
			}
			return handler is ArrayHandler;
		}

		public override object Read(IReadContext readContext)
		{
			IInternalReadContext context = (IInternalReadContext)readContext;
			int payloadOffset = context.ReadInt();
			if (payloadOffset == 0)
			{
				return null;
			}
			int savedOffSet = context.Offset();
			ClassMetadata classMetadata = ReadClassMetadata(context, payloadOffset);
			if (classMetadata == null)
			{
				context.Seek(savedOffSet);
				return null;
			}
			SeekSecondaryOffset(context, classMetadata);
			object obj = classMetadata.Read(context);
			context.Seek(savedOffSet);
			return obj;
		}

		private ClassMetadata ReadClassMetadata(IInternalReadContext context, int payloadOffset
			)
		{
			context.Seek(payloadOffset);
			ClassMetadata classMetadata = Container().ClassMetadataForId(context.ReadInt());
			return classMetadata;
		}

		private void SeekSecondaryOffset(IInternalReadContext context, ClassMetadata classMetadata
			)
		{
			if (classMetadata is PrimitiveFieldHandler && classMetadata.IsArray())
			{
				context.Seek(context.ReadInt());
			}
		}

		public override void Write(IWriteContext context, object obj)
		{
			if (obj == null)
			{
				context.WriteInt(0);
				return;
			}
			MarshallingContext marshallingContext = (MarshallingContext)context;
			ITypeHandler4 handler = ClassMetadata.ForObject(context.Transaction(), obj, true);
			if (handler == null)
			{
				context.WriteInt(0);
				return;
			}
			MarshallingContextState state = marshallingContext.CurrentState();
			marshallingContext.CreateChildBuffer(false, false);
			int id = marshallingContext.Container().Handlers().HandlerID(handler);
			context.WriteInt(id);
			if (IsArray(handler))
			{
				marshallingContext.PrepareIndirectionOfSecondWrite();
			}
			else
			{
				marshallingContext.DoNotIndirectWrites();
			}
			handler.Write(context, obj);
			marshallingContext.RestoreState(state);
		}
	}
}
