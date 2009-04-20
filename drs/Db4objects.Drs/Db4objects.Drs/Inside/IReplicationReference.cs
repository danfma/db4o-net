/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;

namespace Db4objects.Drs.Inside
{
	public interface IReplicationReference
	{
		Db4oUUID Uuid();

		/// <summary>
		/// IMPORTANT
		/// <p/>
		/// Must return the latests version of the object AND OF ALL COLLECTIONS IT REFERENCES IN ITS
		/// FIELDS because collections are treated as 2nd class objects (just like arrays) for Hibernate replication
		/// compatibility purposes.
		/// </summary>
		/// <remarks>
		/// IMPORTANT
		/// <p/>
		/// Must return the latests version of the object AND OF ALL COLLECTIONS IT REFERENCES IN ITS
		/// FIELDS because collections are treated as 2nd class objects (just like arrays) for Hibernate replication
		/// compatibility purposes.
		/// </remarks>
		long Version();

		object Object();

		object Counterpart();

		void SetCounterpart(object obj);

		void MarkForReplicating();

		bool IsMarkedForReplicating();

		void MarkForDeleting();

		bool IsMarkedForDeleting();

		void MarkCounterpartAsNew();

		bool IsCounterpartNew();
	}
}
