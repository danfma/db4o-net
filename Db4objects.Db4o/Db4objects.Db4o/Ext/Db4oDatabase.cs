using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Types;
using Sharpen;

namespace Db4objects.Db4o.Ext
{
	/// <summary>Class to identify a database by it's signature.</summary>
	/// <remarks>
	/// Class to identify a database by it's signature.
	/// <br /><br />db4o UUID handling uses a reference to the Db4oDatabase object, that
	/// represents the database an object was created on.
	/// </remarks>
	/// <persistent></persistent>
	/// <exclude></exclude>
	public class Db4oDatabase : IDb4oType, IInternal4
	{
		/// <summary>Field is public for implementation reasons, DO NOT TOUCH!</summary>
		public byte[] i_signature;

		/// <summary>
		/// Field is public for implementation reasons, DO NOT TOUCH!
		/// This field is badly named, it really is the creation time.
		/// </summary>
		/// <remarks>
		/// Field is public for implementation reasons, DO NOT TOUCH!
		/// This field is badly named, it really is the creation time.
		/// </remarks>
		public long i_uuid;

		private static readonly string CREATIONTIME_FIELD = "i_uuid";

		/// <summary>cached ObjectContainer for getting the own ID.</summary>
		/// <remarks>cached ObjectContainer for getting the own ID.</remarks>
		[System.NonSerialized]
		private ObjectContainerBase i_stream;

		/// <summary>cached ID, only valid in combination with i_objectContainer</summary>
		[System.NonSerialized]
		private int i_id;

		public Db4oDatabase()
		{
		}

		public Db4oDatabase(byte[] signature, long creationTime)
		{
			i_signature = signature;
			i_uuid = creationTime;
		}

		/// <summary>generates a new Db4oDatabase object with a unique signature.</summary>
		/// <remarks>generates a new Db4oDatabase object with a unique signature.</remarks>
		public static Db4objects.Db4o.Ext.Db4oDatabase Generate()
		{
			return new Db4objects.Db4o.Ext.Db4oDatabase(Unobfuscated.GenerateSignature(), Runtime
				.CurrentTimeMillis());
		}

		/// <summary>comparison by signature.</summary>
		/// <remarks>comparison by signature.</remarks>
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			Db4objects.Db4o.Ext.Db4oDatabase other = (Db4objects.Db4o.Ext.Db4oDatabase)obj;
			if (null == other.i_signature || null == this.i_signature)
			{
				return false;
			}
			return Arrays4.AreEqual(other.i_signature, this.i_signature);
		}

		public override int GetHashCode()
		{
			return i_signature.GetHashCode();
		}

		/// <summary>gets the db4o ID, and may cache it for performance reasons.</summary>
		/// <remarks>gets the db4o ID, and may cache it for performance reasons.</remarks>
		/// <returns>the db4o ID for the ObjectContainer</returns>
		public virtual int GetID(Transaction trans)
		{
			ObjectContainerBase stream = trans.Stream();
			if (stream != i_stream)
			{
				i_stream = stream;
				i_id = Bind(trans);
			}
			return i_id;
		}

		public virtual long GetCreationTime()
		{
			return i_uuid;
		}

		/// <summary>returns the unique signature</summary>
		public virtual byte[] GetSignature()
		{
			return i_signature;
		}

		public override string ToString()
		{
			return "db " + i_signature;
		}

		public virtual bool IsOlderThan(Db4objects.Db4o.Ext.Db4oDatabase peer)
		{
			if (peer == this)
			{
				throw new ArgumentException();
			}
			if (i_uuid != peer.i_uuid)
			{
				return i_uuid < peer.i_uuid;
			}
			if (i_signature.Length != peer.i_signature.Length)
			{
				return i_signature.Length < peer.i_signature.Length;
			}
			for (int i = 0; i < i_signature.Length; i++)
			{
				if (i_signature[i] != peer.i_signature[i])
				{
					return i_signature[i] < peer.i_signature[i];
				}
			}
			throw new Exception();
		}

		/// <summary>make sure this Db4oDatabase is stored.</summary>
		/// <remarks>make sure this Db4oDatabase is stored. Return the ID.</remarks>
		public virtual int Bind(Transaction trans)
		{
			ObjectContainerBase stream = trans.Stream();
			Db4objects.Db4o.Ext.Db4oDatabase stored = (Db4objects.Db4o.Ext.Db4oDatabase)stream
				.Db4oTypeStored(trans, this);
			if (stored == null)
			{
				return StoreAndGetId(trans);
			}
			if (stored == this)
			{
				return stream.GetID1(this);
			}
			if (i_uuid == 0)
			{
				i_uuid = stored.i_uuid;
			}
			stream.ShowInternalClasses(true);
			try
			{
				int id = stream.GetID1(stored);
				stream.Bind(this, id);
				return id;
			}
			finally
			{
				stream.ShowInternalClasses(false);
			}
		}

		private int StoreAndGetId(Transaction trans)
		{
			ObjectContainerBase stream = trans.Stream();
			stream.ShowInternalClasses(true);
			try
			{
				stream.Set3(trans, this, 2, false);
				return stream.GetID1(this);
			}
			finally
			{
				stream.ShowInternalClasses(false);
			}
		}

		/// <summary>find a Db4oDatabase with the same signature as this one</summary>
		public virtual Db4objects.Db4o.Ext.Db4oDatabase Query(Transaction trans)
		{
			if (i_uuid > 0)
			{
				Db4objects.Db4o.Ext.Db4oDatabase res = Query(trans, true);
				if (res != null)
				{
					return res;
				}
			}
			return Query(trans, false);
		}

		private Db4objects.Db4o.Ext.Db4oDatabase Query(Transaction trans, bool constrainByUUID
			)
		{
			ObjectContainerBase stream = trans.Stream();
			IQuery q = stream.Query(trans);
			q.Constrain(GetType());
			if (constrainByUUID)
			{
				q.Descend(CREATIONTIME_FIELD).Constrain(i_uuid);
			}
			IObjectSet objectSet = q.Execute();
			while (objectSet.HasNext())
			{
				Db4objects.Db4o.Ext.Db4oDatabase storedDatabase = (Db4objects.Db4o.Ext.Db4oDatabase
					)objectSet.Next();
				stream.Activate1(null, storedDatabase, 4);
				if (storedDatabase.Equals(this))
				{
					return storedDatabase;
				}
			}
			return null;
		}
	}
}
