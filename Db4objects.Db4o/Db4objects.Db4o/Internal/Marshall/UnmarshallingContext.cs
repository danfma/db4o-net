/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class UnmarshallingContext : AbstractReadContext, IFieldListInfo, IMarshallingInfo
	{
		private readonly ObjectReference _reference;

		private object _object;

		private ObjectHeader _objectHeader;

		private int _addToIDTree;

		private bool _checkIDTree;

		public UnmarshallingContext(Transaction transaction, Db4objects.Db4o.Internal.Buffer
			 buffer, ObjectReference @ref, int addToIDTree, bool checkIDTree) : base(transaction
			, buffer)
		{
			_reference = @ref;
			_addToIDTree = addToIDTree;
			_checkIDTree = checkIDTree;
		}

		public UnmarshallingContext(Transaction transaction, ObjectReference @ref, int addToIDTree
			, bool checkIDTree) : this(transaction, null, @ref, addToIDTree, checkIDTree)
		{
		}

		public virtual Db4objects.Db4o.Internal.StatefulBuffer StatefulBuffer()
		{
			Db4objects.Db4o.Internal.StatefulBuffer buffer = new Db4objects.Db4o.Internal.StatefulBuffer
				(_transaction, _buffer.Length());
			buffer.SetID(ObjectID());
			buffer.SetInstantiationDepth(ActivationDepth());
			_buffer.CopyTo(buffer, 0, 0, _buffer.Length());
			buffer.Offset(_buffer.Offset());
			return buffer;
		}

		public virtual int ObjectID()
		{
			return _reference.GetID();
		}

		public virtual object Read()
		{
			return ReadInternal(false);
		}

		public virtual object ReadPrefetch()
		{
			return ReadInternal(true);
		}

		private object ReadInternal(bool doAdjustActivationDepthForPrefetch)
		{
			if (!BeginProcessing())
			{
				return _object;
			}
			ReadBuffer(ObjectID());
			if (_buffer == null)
			{
				EndProcessing();
				return _object;
			}
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = ReadObjectHeader();
			if (classMetadata == null)
			{
				EndProcessing();
				return _object;
			}
			_reference.ClassMetadata(classMetadata);
			if (doAdjustActivationDepthForPrefetch)
			{
				AdjustActivationDepthForPrefetch();
			}
			if (_checkIDTree)
			{
				object objectInCacheFromClassCreation = _transaction.ObjectForIdFromCache(ObjectID
					());
				if (objectInCacheFromClassCreation != null)
				{
					_object = objectInCacheFromClassCreation;
					EndProcessing();
					return _object;
				}
			}
			if (PeekPersisted())
			{
				_object = ClassMetadata().InstantiateTransient(this);
			}
			else
			{
				_object = ClassMetadata().Instantiate(this);
			}
			EndProcessing();
			return _object;
		}

		private void AdjustActivationDepthForPrefetch()
		{
			int depth = ClassMetadata().ConfigOrAncestorConfig() == null ? 1 : 0;
			ActivationDepth(depth);
		}

		public virtual object ReadFieldValue(FieldMetadata field)
		{
			ReadBuffer(ObjectID());
			if (_buffer == null)
			{
				return null;
			}
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = ReadObjectHeader();
			if (classMetadata == null)
			{
				return null;
			}
			if (!_objectHeader.ObjectMarshaller().FindOffset(classMetadata, _objectHeader._headerAttributes
				, _buffer, field))
			{
				return null;
			}
			return field.Read(this);
		}

		private Db4objects.Db4o.Internal.ClassMetadata ReadObjectHeader()
		{
			_objectHeader = new ObjectHeader(Container(), _buffer);
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = _objectHeader.ClassMetadata
				();
			if (classMetadata == null)
			{
				return null;
			}
			return classMetadata;
		}

		private void ReadBuffer(int id)
		{
			if (_buffer == null && id > 0)
			{
				_buffer = Container().ReadReaderByID(_transaction, id);
			}
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _reference.ClassMetadata();
		}

		private bool BeginProcessing()
		{
			return _reference.BeginProcessing();
		}

		private void EndProcessing()
		{
			_reference.EndProcessing();
		}

		public virtual void SetStateClean()
		{
			_reference.SetStateClean();
		}

		public virtual object PersistentObject()
		{
			return _object;
		}

		public virtual void SetObjectWeak(object obj)
		{
			_reference.SetObjectWeak(Container(), obj);
		}

		public override object ReadObject()
		{
			int id = ReadInt();
			int depth = _activationDepth - 1;
			if (PeekPersisted())
			{
				return Container().PeekPersisted(Transaction(), id, depth, false);
			}
			object obj = Container().GetByID2(Transaction(), id);
			if (obj is IDb4oTypeImpl)
			{
				depth = ((IDb4oTypeImpl)obj).AdjustReadDepth(depth);
			}
			Container().StillToActivate(Transaction(), obj, depth);
			return obj;
		}

		private bool PeekPersisted()
		{
			return _addToIDTree == Const4.TRANSIENT;
		}

		public override object ReadObject(ITypeHandler4 handlerType)
		{
			ITypeHandler4 handler = CorrectHandlerVersion(handlerType);
			if (!IsIndirected(handler))
			{
				return handler.Read(this);
			}
			int payLoadOffset = ReadInt();
			ReadInt();
			if (payLoadOffset == 0)
			{
				return null;
			}
			int savedOffset = Offset();
			Seek(payLoadOffset);
			object obj = handler.Read(this);
			Seek(savedOffset);
			return obj;
		}

		public virtual void AdjustInstantiationDepth()
		{
			Config4Class classConfig = ClassConfig();
			if (classConfig != null)
			{
				_activationDepth = classConfig.AdjustActivationDepth(_activationDepth);
			}
		}

		public virtual Config4Class ClassConfig()
		{
			return ClassMetadata().Config();
		}

		public virtual ObjectReference Reference()
		{
			return _reference;
		}

		public virtual void AddToIDTree()
		{
			if (_addToIDTree == Const4.ADD_TO_ID_TREE)
			{
				_reference.AddExistingReferenceToIdTree(Transaction());
			}
		}

		public virtual void PersistentObject(object obj)
		{
			_object = obj;
		}

		public virtual ObjectHeaderAttributes HeaderAttributes()
		{
			return _objectHeader._headerAttributes;
		}

		public virtual bool IsNull(int fieldIndex)
		{
			return HeaderAttributes().IsNull(fieldIndex);
		}

		public override int HandlerVersion()
		{
			return _objectHeader.HandlerVersion();
		}
	}
}
