/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class SystemData
	{
		private int _classCollectionID;

		private int _converterVersion;

		private int _freespaceAddress;

		private int _freespaceID;

		private byte _freespaceSystem;

		private Db4oDatabase _identity;

		private int _identityId;

		private long _lastTimeStampID;

		private byte _stringEncoding;

		private int _uuidIndexId;

		private byte _idSystemType;

		private int _idSystemID;

		private int _transactionPointer1;

		private int _transactionPointer2;

		public SystemData()
		{
		}

		public virtual void IdSystemType(byte idSystem)
		{
			_idSystemType = idSystem;
		}

		public virtual byte IdSystemType()
		{
			return _idSystemType;
		}

		public virtual void IdSystemID(int idSystemID)
		{
			_idSystemID = idSystemID;
		}

		public virtual int IdSystemID()
		{
			return _idSystemID;
		}

		public virtual int ClassCollectionID()
		{
			return _classCollectionID;
		}

		public virtual void ClassCollectionID(int id)
		{
			_classCollectionID = id;
		}

		public virtual int ConverterVersion()
		{
			return _converterVersion;
		}

		public virtual void ConverterVersion(int version)
		{
			_converterVersion = version;
		}

		public virtual int FreespaceAddress()
		{
			return _freespaceAddress;
		}

		public virtual void FreespaceAddress(int address)
		{
			_freespaceAddress = address;
		}

		public virtual int FreespaceID()
		{
			return _freespaceID;
		}

		public virtual void FreespaceID(int id)
		{
			_freespaceID = id;
		}

		public virtual byte FreespaceSystem()
		{
			return _freespaceSystem;
		}

		public virtual void FreespaceSystem(byte freespaceSystemtype)
		{
			_freespaceSystem = freespaceSystemtype;
		}

		public virtual Db4oDatabase Identity()
		{
			return _identity;
		}

		public virtual void Identity(Db4oDatabase identityObject)
		{
			_identity = identityObject;
		}

		public virtual long LastTimeStampID()
		{
			return _lastTimeStampID;
		}

		public virtual void LastTimeStampID(long id)
		{
			_lastTimeStampID = id;
		}

		public virtual byte StringEncoding()
		{
			return _stringEncoding;
		}

		public virtual void StringEncoding(byte encodingByte)
		{
			_stringEncoding = encodingByte;
		}

		public virtual int UuidIndexId()
		{
			return _uuidIndexId;
		}

		public virtual void UuidIndexId(int id)
		{
			_uuidIndexId = id;
		}

		public virtual void IdentityId(int id)
		{
			_identityId = id;
		}

		public virtual int IdentityId()
		{
			return _identityId;
		}

		public virtual void TransactionPointer1(int pointer)
		{
			_transactionPointer1 = pointer;
		}

		public virtual void TransactionPointer2(int pointer)
		{
			_transactionPointer2 = pointer;
		}

		public virtual int TransactionPointer1()
		{
			return _transactionPointer1;
		}

		public virtual int TransactionPointer2()
		{
			return _transactionPointer2;
		}
	}
}
