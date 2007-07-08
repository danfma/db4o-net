/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class HashtableObjectEntry : HashtableIntEntry
	{
		public object _objectKey;

		internal HashtableObjectEntry(int a_hash, object a_key, object a_object) : base(a_hash
			, a_object)
		{
			_objectKey = a_key;
		}

		internal HashtableObjectEntry(object a_key, object a_object) : base(a_key.GetHashCode
			(), a_object)
		{
			_objectKey = a_key;
		}

		public HashtableObjectEntry() : base()
		{
		}

		public override object Key()
		{
			return _objectKey;
		}

		public override object DeepClone(object obj)
		{
			return DeepCloneInternal(new Db4objects.Db4o.Foundation.HashtableObjectEntry(), obj
				);
		}

		protected override HashtableIntEntry DeepCloneInternal(HashtableIntEntry entry, object
			 obj)
		{
			((Db4objects.Db4o.Foundation.HashtableObjectEntry)entry)._objectKey = _objectKey;
			return base.DeepCloneInternal(entry, obj);
		}

		public virtual bool HasKey(object key)
		{
			return _objectKey.Equals(key);
		}

		public override bool SameKeyAs(HashtableIntEntry other)
		{
			return other is Db4objects.Db4o.Foundation.HashtableObjectEntry ? HasKey(((Db4objects.Db4o.Foundation.HashtableObjectEntry
				)other)._objectKey) : false;
		}
	}
}
