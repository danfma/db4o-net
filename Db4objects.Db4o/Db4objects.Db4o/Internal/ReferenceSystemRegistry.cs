/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class ReferenceSystemRegistry
	{
		private readonly Collection4 _referenceSystems = new Collection4();

		public virtual void RemoveId(int id)
		{
			RemoveReference(new _IReferenceSource_16(this, id));
		}

		private sealed class _IReferenceSource_16 : ReferenceSystemRegistry.IReferenceSource
		{
			public _IReferenceSource_16(ReferenceSystemRegistry _enclosing, int id)
			{
				this._enclosing = _enclosing;
				this.id = id;
			}

			public ObjectReference ReferenceFrom(IReferenceSystem referenceSystem)
			{
				return referenceSystem.ReferenceForId(id);
			}

			private readonly ReferenceSystemRegistry _enclosing;

			private readonly int id;
		}

		public virtual void RemoveObject(object obj)
		{
			RemoveReference(new _IReferenceSource_24(this, obj));
		}

		private sealed class _IReferenceSource_24 : ReferenceSystemRegistry.IReferenceSource
		{
			public _IReferenceSource_24(ReferenceSystemRegistry _enclosing, object obj)
			{
				this._enclosing = _enclosing;
				this.obj = obj;
			}

			public ObjectReference ReferenceFrom(IReferenceSystem referenceSystem)
			{
				return referenceSystem.ReferenceForObject(obj);
			}

			private readonly ReferenceSystemRegistry _enclosing;

			private readonly object obj;
		}

		public virtual void RemoveReference(ObjectReference reference)
		{
			RemoveReference(new _IReferenceSource_32(this, reference));
		}

		private sealed class _IReferenceSource_32 : ReferenceSystemRegistry.IReferenceSource
		{
			public _IReferenceSource_32(ReferenceSystemRegistry _enclosing, ObjectReference reference
				)
			{
				this._enclosing = _enclosing;
				this.reference = reference;
			}

			public ObjectReference ReferenceFrom(IReferenceSystem referenceSystem)
			{
				return reference;
			}

			private readonly ReferenceSystemRegistry _enclosing;

			private readonly ObjectReference reference;
		}

		private void RemoveReference(ReferenceSystemRegistry.IReferenceSource referenceSource
			)
		{
			IEnumerator i = _referenceSystems.GetEnumerator();
			while (i.MoveNext())
			{
				IReferenceSystem referenceSystem = (IReferenceSystem)i.Current;
				ObjectReference reference = referenceSource.ReferenceFrom(referenceSystem);
				if (reference != null)
				{
					referenceSystem.RemoveReference(reference);
				}
			}
		}

		public virtual void AddReferenceSystem(IReferenceSystem referenceSystem)
		{
			_referenceSystems.Add(referenceSystem);
		}

		public virtual void RemoveReferenceSystem(IReferenceSystem referenceSystem)
		{
			_referenceSystems.Remove(referenceSystem);
		}

		private interface IReferenceSource
		{
			ObjectReference ReferenceFrom(IReferenceSystem referenceSystem);
		}
	}
}
