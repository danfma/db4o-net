/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	/// <summary>The index record that is written to the database file.</summary>
	/// <remarks>
	/// The index record that is written to the database file.
	/// Don't obfuscate.
	/// </remarks>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class MetaIndex : IInternal4
	{
		public int indexAddress;

		public int indexEntries;

		public int indexLength;

		private readonly int patchAddress = 0;

		private readonly int patchEntries = 0;

		private readonly int patchLength = 0;

		// The number of entries an the length are redundant, because the handler should
		// return a fixed length, but we absolutely want to make sure, we don't free
		// a slot into nowhere.
		// TODO: make sure this aren't really needed
		// and remove them 
		public virtual void Read(ByteArrayBuffer reader)
		{
			indexAddress = reader.ReadInt();
			indexEntries = reader.ReadInt();
			indexLength = reader.ReadInt();
			// no longer used apparently
			reader.ReadInt();
			reader.ReadInt();
			reader.ReadInt();
		}

		public virtual void Write(ByteArrayBuffer writer)
		{
			writer.WriteInt(indexAddress);
			writer.WriteInt(indexEntries);
			writer.WriteInt(indexLength);
			writer.WriteInt(patchAddress);
			writer.WriteInt(patchEntries);
			writer.WriteInt(patchLength);
		}

		public virtual void Free(LocalObjectContainer file)
		{
			file.Free(indexAddress, indexLength);
			indexAddress = 0;
			indexLength = 0;
		}
		//        file.free(patchAddress, patchLength);
		//        patchAddress = 0;
		//        patchLength = 0;
	}
}
