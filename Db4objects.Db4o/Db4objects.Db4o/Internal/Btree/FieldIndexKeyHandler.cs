/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class FieldIndexKeyHandler : IIndexable4
	{
		private readonly IIndexable4 _valueHandler;

		private readonly IDHandler _parentIdHandler;

		public FieldIndexKeyHandler(IIndexable4 delegate_)
		{
			_parentIdHandler = new IDHandler();
			_valueHandler = delegate_;
		}

		public virtual int LinkLength()
		{
			return _valueHandler.LinkLength() + Const4.IntLength;
		}

		public virtual object ReadIndexEntry(IContext context, ByteArrayBuffer a_reader)
		{
			// TODO: could read int directly here with a_reader.readInt()
			int parentID = ReadParentID(context, a_reader);
			object objPart = _valueHandler.ReadIndexEntry(context, a_reader);
			if (parentID < 0)
			{
				objPart = null;
				parentID = -parentID;
			}
			return new FieldIndexKeyImpl(parentID, objPart);
		}

		private int ReadParentID(IContext context, ByteArrayBuffer a_reader)
		{
			return ((int)_parentIdHandler.ReadIndexEntry(context, a_reader));
		}

		public virtual void WriteIndexEntry(IContext context, ByteArrayBuffer writer, object
			 obj)
		{
			IFieldIndexKey composite = (IFieldIndexKey)obj;
			int parentID = composite.ParentID();
			object value = composite.Value();
			if (value == null)
			{
				parentID = -parentID;
			}
			_parentIdHandler.Write(parentID, writer);
			_valueHandler.WriteIndexEntry(context, writer, composite.Value());
		}

		public virtual IIndexable4 ValueHandler()
		{
			return _valueHandler;
		}

		public virtual void DefragIndexEntry(DefragmentContextImpl context)
		{
			_parentIdHandler.DefragIndexEntry(context);
			_valueHandler.DefragIndexEntry(context);
		}

		public virtual IPreparedComparison PrepareComparison(IContext context, object fieldIndexKey
			)
		{
			IFieldIndexKey source = (IFieldIndexKey)fieldIndexKey;
			IPreparedComparison preparedValueComparison = _valueHandler.PrepareComparison(context
				, source.Value());
			IPreparedComparison preparedParentIdComparison = _parentIdHandler.NewPrepareCompare
				(source.ParentID());
			return new _IPreparedComparison_67(preparedValueComparison, preparedParentIdComparison
				);
		}

		private sealed class _IPreparedComparison_67 : IPreparedComparison
		{
			public _IPreparedComparison_67(IPreparedComparison preparedValueComparison, IPreparedComparison
				 preparedParentIdComparison)
			{
				this.preparedValueComparison = preparedValueComparison;
				this.preparedParentIdComparison = preparedParentIdComparison;
			}

			public int CompareTo(object obj)
			{
				IFieldIndexKey target = (IFieldIndexKey)obj;
				try
				{
					int delegateResult = preparedValueComparison.CompareTo(target.Value());
					if (delegateResult != 0)
					{
						return delegateResult;
					}
				}
				catch (IllegalComparisonException)
				{
				}
				// can happen, is expected
				return preparedParentIdComparison.CompareTo(target.ParentID());
			}

			private readonly IPreparedComparison preparedValueComparison;

			private readonly IPreparedComparison preparedParentIdComparison;
		}
	}
}
