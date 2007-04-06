using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Hashtable4 : IDeepClone
	{
		private const float FILL = 0.5F;

		private int i_tableSize;

		private int i_mask;

		private int i_maximumSize;

		private int i_size;

		private HashtableIntEntry[] i_table;

		public Hashtable4(int a_size)
		{
			a_size = NewSize(a_size);
			i_tableSize = 1;
			while (i_tableSize < a_size)
			{
				i_tableSize = i_tableSize << 1;
			}
			i_mask = i_tableSize - 1;
			i_maximumSize = (int)(i_tableSize * FILL);
			i_table = new HashtableIntEntry[i_tableSize];
		}

		public Hashtable4() : this(1)
		{
		}

		protected Hashtable4(IDeepClone cloneOnlyCtor)
		{
		}

		public virtual int Size()
		{
			return i_size;
		}

		public virtual object DeepClone(object obj)
		{
			return DeepCloneInternal(new Db4objects.Db4o.Foundation.Hashtable4((IDeepClone)null
				), obj);
		}

		public virtual void ForEachKey(IVisitor4 visitor)
		{
			for (int i = 0; i < i_table.Length; i++)
			{
				HashtableIntEntry entry = i_table[i];
				while (entry != null)
				{
					entry.AcceptKeyVisitor(visitor);
					entry = entry.i_next;
				}
			}
		}

		public virtual void ForEachKeyForIdentity(IVisitor4 visitor, object a_identity)
		{
			for (int i = 0; i < i_table.Length; i++)
			{
				HashtableIntEntry entry = i_table[i];
				while (entry != null)
				{
					if (entry.i_object == a_identity)
					{
						entry.AcceptKeyVisitor(visitor);
					}
					entry = entry.i_next;
				}
			}
		}

		public virtual void ForEachValue(IVisitor4 visitor)
		{
			for (int i = 0; i < i_table.Length; i++)
			{
				HashtableIntEntry entry = i_table[i];
				while (entry != null)
				{
					visitor.Visit(entry.i_object);
					entry = entry.i_next;
				}
			}
		}

		public virtual object Get(byte[] key)
		{
			int intKey = HashtableByteArrayEntry.Hash(key);
			return GetFromObjectEntry(intKey, key);
		}

		public virtual object Get(int key)
		{
			HashtableIntEntry entry = i_table[key & i_mask];
			while (entry != null)
			{
				if (entry.i_key == key)
				{
					return entry.i_object;
				}
				entry = entry.i_next;
			}
			return null;
		}

		public virtual object Get(object key)
		{
			if (key == null)
			{
				return null;
			}
			return GetFromObjectEntry(key.GetHashCode(), key);
		}

		public virtual bool ContainsKey(object key)
		{
			if (null == key)
			{
				return false;
			}
			return null != GetObjectEntry(key.GetHashCode(), key);
		}

		public virtual void Put(byte[] key, object value)
		{
			PutEntry(new HashtableByteArrayEntry(key, value));
		}

		public virtual void Put(int key, object value)
		{
			PutEntry(new HashtableIntEntry(key, value));
		}

		public virtual void Put(object key, object value)
		{
			PutEntry(new HashtableObjectEntry(key, value));
		}

		public virtual object Remove(byte[] key)
		{
			int intKey = HashtableByteArrayEntry.Hash(key);
			return RemoveObjectEntry(intKey, key);
		}

		public virtual void Remove(int a_key)
		{
			HashtableIntEntry entry = i_table[a_key & i_mask];
			HashtableIntEntry predecessor = null;
			while (entry != null)
			{
				if (entry.i_key == a_key)
				{
					RemoveEntry(predecessor, entry);
					return;
				}
				predecessor = entry;
				entry = entry.i_next;
			}
		}

		public virtual void Remove(object objectKey)
		{
			int intKey = objectKey.GetHashCode();
			RemoveObjectEntry(intKey, objectKey);
		}

		protected virtual Db4objects.Db4o.Foundation.Hashtable4 DeepCloneInternal(Db4objects.Db4o.Foundation.Hashtable4
			 ret, object obj)
		{
			ret.i_mask = i_mask;
			ret.i_maximumSize = i_maximumSize;
			ret.i_size = i_size;
			ret.i_tableSize = i_tableSize;
			ret.i_table = new HashtableIntEntry[i_tableSize];
			for (int i = 0; i < i_tableSize; i++)
			{
				if (i_table[i] != null)
				{
					ret.i_table[i] = (HashtableIntEntry)i_table[i].DeepClone(obj);
				}
			}
			return ret;
		}

		private int EntryIndex(HashtableIntEntry entry)
		{
			return entry.i_key & i_mask;
		}

		private HashtableIntEntry FindWithSameKey(HashtableIntEntry newEntry)
		{
			HashtableIntEntry existing = i_table[EntryIndex(newEntry)];
			while (null != existing)
			{
				if (existing.SameKeyAs(newEntry))
				{
					return existing;
				}
				existing = existing.i_next;
			}
			return null;
		}

		private object GetFromObjectEntry(int intKey, object objectKey)
		{
			HashtableObjectEntry entry = GetObjectEntry(intKey, objectKey);
			return entry == null ? null : entry.i_object;
		}

		private HashtableObjectEntry GetObjectEntry(int intKey, object objectKey)
		{
			HashtableObjectEntry entry = (HashtableObjectEntry)i_table[intKey & i_mask];
			while (entry != null)
			{
				if (entry.i_key == intKey && entry.HasKey(objectKey))
				{
					return entry;
				}
				entry = (HashtableObjectEntry)entry.i_next;
			}
			return null;
		}

		private void IncreaseSize()
		{
			i_tableSize = i_tableSize << 1;
			i_maximumSize = i_maximumSize << 1;
			i_mask = i_tableSize - 1;
			HashtableIntEntry[] temp = i_table;
			i_table = new HashtableIntEntry[i_tableSize];
			for (int i = 0; i < temp.Length; i++)
			{
				Reposition(temp[i]);
			}
		}

		private void Insert(HashtableIntEntry newEntry)
		{
			i_size++;
			if (i_size > i_maximumSize)
			{
				IncreaseSize();
			}
			int index = EntryIndex(newEntry);
			newEntry.i_next = i_table[index];
			i_table[index] = newEntry;
		}

		private int NewSize(int a_size)
		{
			return (int)(a_size / FILL);
		}

		private void PutEntry(HashtableIntEntry newEntry)
		{
			HashtableIntEntry existing = FindWithSameKey(newEntry);
			if (null != existing)
			{
				Replace(existing, newEntry);
			}
			else
			{
				Insert(newEntry);
			}
		}

		private void RemoveEntry(HashtableIntEntry predecessor, HashtableIntEntry entry)
		{
			if (predecessor != null)
			{
				predecessor.i_next = entry.i_next;
			}
			else
			{
				i_table[EntryIndex(entry)] = entry.i_next;
			}
			i_size--;
		}

		private object RemoveObjectEntry(int intKey, object objectKey)
		{
			HashtableObjectEntry entry = (HashtableObjectEntry)i_table[intKey & i_mask];
			HashtableObjectEntry predecessor = null;
			while (entry != null)
			{
				if (entry.i_key == intKey && entry.HasKey(objectKey))
				{
					RemoveEntry(predecessor, entry);
					return entry.i_object;
				}
				predecessor = entry;
				entry = (HashtableObjectEntry)entry.i_next;
			}
			return null;
		}

		private void Replace(HashtableIntEntry existing, HashtableIntEntry newEntry)
		{
			newEntry.i_next = existing.i_next;
			HashtableIntEntry entry = i_table[EntryIndex(existing)];
			if (entry == existing)
			{
				i_table[EntryIndex(existing)] = newEntry;
			}
			else
			{
				while (entry.i_next != existing)
				{
					entry = entry.i_next;
				}
				entry.i_next = newEntry;
			}
		}

		private void Reposition(HashtableIntEntry a_entry)
		{
			if (a_entry != null)
			{
				Reposition(a_entry.i_next);
				a_entry.i_next = i_table[EntryIndex(a_entry)];
				i_table[EntryIndex(a_entry)] = a_entry;
			}
		}
	}
}
