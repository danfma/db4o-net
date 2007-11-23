/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when the database file format
	/// is not compatible with the applied configuration.
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when the database file format
	/// is not compatible with the applied configuration.
	/// </remarks>
	[System.Serializable]
	public class IncompatibleFileFormatException : Db4oException
	{
	}
}