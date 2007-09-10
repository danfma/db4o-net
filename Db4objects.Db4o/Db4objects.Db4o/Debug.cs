/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public abstract class Debug : Debug4
	{
		/// <summary>indexes all fields</summary>
		public const bool indexAllFields = false;

		/// <summary>prints query graph information to the console</summary>
		public const bool queries = false;

		/// <summary>prints more stack traces</summary>
		public const bool atHome = false;

		/// <summary>makes C/S timeouts longer, so C/S does not time out in the debugger</summary>
		public const bool longTimeOuts = false;

		/// <summary>turns freespace debuggin on</summary>
		public const bool freespace = Deploy.debug;

		/// <summary>
		/// fills deleted slots with 'X' and overrides any configured
		/// freespace filler
		/// </summary>
		public const bool xbytes = freespace;

		/// <summary>
		/// checks monitor conditions to make sure only the thread
		/// with the global monitor is allowed entry to the core
		/// </summary>
		public const bool checkSychronization = false;

		/// <summary>
		/// makes sure a configuration entry is generated for each persistent
		/// class
		/// </summary>
		public const bool configureAllClasses = indexAllFields;

		/// <summary>
		/// makes sure a configuration entry is generated for each persistent
		/// field
		/// </summary>
		public const bool configureAllFields = indexAllFields;

		/// <summary>allows turning weak references off</summary>
		public const bool weakReferences = true;

		/// <summary>prints all communicated messages to the console</summary>
		public const bool messages = false;

		/// <summary>allows turning NIO off on Java</summary>
		public const bool nio = true;

		/// <summary>allows overriding the file locking mechanism to turn it off</summary>
		public const bool lockFile = true;

		/// <summary>
		/// allows faking the Db4oDatabase identity object, so the first
		/// stored object in the debugger is the actually persisted object
		/// </summary>
		public const bool staticIdentity = false;

		/// <summary>
		/// turn to false, to prevent reading old PBootRecord object
		/// for debugging updating database files.
		/// </summary>
		/// <remarks>
		/// turn to false, to prevent reading old PBootRecord object
		/// for debugging updating database files.
		/// </remarks>
		public const bool readBootRecord = true;

		public static void Expect(bool cond)
		{
			if (!cond)
			{
				throw new Exception("Should never happen");
			}
		}

		public static void EnsureLock(object obj)
		{
		}

		public static bool ExceedsMaximumBlockSize(int a_length)
		{
			if (a_length > Const4.MAXIMUM_BLOCK_SIZE)
			{
				return true;
			}
			return false;
		}

		public static bool ExceedsMaximumArrayEntries(int a_entries, bool a_primitive)
		{
			if (a_entries > (a_primitive ? Const4.MAXIMUM_ARRAY_ENTRIES_PRIMITIVE : Const4.MAXIMUM_ARRAY_ENTRIES
				))
			{
				return true;
			}
			return false;
		}

		public static void ReadBegin(IReadBuffer buffer, byte identifier)
		{
		}

		public static void ReadEnd(IReadBuffer buffer)
		{
			if (Deploy.debug && Deploy.brackets)
			{
				if (buffer.ReadByte() != Const4.YAPEND)
				{
					throw new Exception("Debug.readEnd() YAPEND expected");
				}
			}
		}

		public static void WriteBegin(IWriteBuffer buffer, byte identifier)
		{
		}

		public static void WriteEnd(IWriteBuffer buffer)
		{
			if (Deploy.debug && Deploy.brackets)
			{
				if (buffer is MarshallingContext)
				{
					((MarshallingContext)buffer).DebugWriteEnd(Const4.YAPEND);
					return;
				}
				buffer.WriteByte(Const4.YAPEND);
			}
		}
	}
}
