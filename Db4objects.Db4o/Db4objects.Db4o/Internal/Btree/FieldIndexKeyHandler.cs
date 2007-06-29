/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class FieldIndexKeyHandler : IIndexable4
	{
		private readonly IIndexable4 _valueHandler;

		private readonly IntHandler _parentIdHandler;

		public FieldIndexKeyHandler(ObjectContainerBase stream, IIndexable4 delegate_)
		{
			_parentIdHandler = new IDHandler(stream);
			_valueHandler = delegate_;
		}

		public virtual int LinkLength()
		{
			return _valueHandler.LinkLength() + Const4.INT_LENGTH;
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer a_reader)
		{
			int parentID = ReadParentID(a_reader);
			object objPart = _valueHandler.ReadIndexEntry(a_reader);
			if (parentID < 0)
			{
				objPart = null;
				parentID = -parentID;
			}
			return new FieldIndexKey(parentID, objPart);
		}

		private int ReadParentID(Db4objects.Db4o.Internal.Buffer a_reader)
		{
			return ((int)_parentIdHandler.ReadIndexEntry(a_reader));
		}

		public virtual void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer writer, object
			 obj)
		{
			FieldIndexKey composite = (FieldIndexKey)obj;
			int parentID = composite.ParentID();
			object value = composite.Value();
			if (value == null)
			{
				parentID = -parentID;
			}
			_parentIdHandler.Write(parentID, writer);
			_valueHandler.WriteIndexEntry(writer, composite.Value());
		}

		public virtual IIndexable4 ValueHandler()
		{
			return _valueHandler;
		}

		public virtual IComparable4 PrepareComparison(object obj)
		{
			FieldIndexKey composite = (FieldIndexKey)obj;
			_valueHandler.PrepareComparison(composite.Value());
			_parentIdHandler.PrepareComparison(composite.ParentID());
			return this;
		}

		public virtual int CompareTo(object obj)
		{
			if (null == obj)
			{
				throw new ArgumentNullException();
			}
			FieldIndexKey composite = (FieldIndexKey)obj;
			try
			{
				int delegateResult = _valueHandler.CompareTo(composite.Value());
				if (delegateResult != 0)
				{
					return delegateResult;
				}
			}
			catch (IllegalComparisonException)
			{
			}
			return _parentIdHandler.CompareTo(composite.ParentID());
		}

		public virtual void DefragIndexEntry(ReaderPair readers)
		{
			_parentIdHandler.DefragIndexEntry(readers);
			_valueHandler.DefragIndexEntry(readers);
		}
	}
}
