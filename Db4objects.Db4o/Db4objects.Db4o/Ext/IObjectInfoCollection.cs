/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// Interface to an iterable collection
	/// <see cref="IObjectInfo">IObjectInfo</see>
	/// objects.<br /><br />
	/// ObjectInfoCollection is used reference a number of stored objects.
	/// </summary>
	/// <seealso cref="IObjectInfo">IObjectInfo</seealso>
	public interface IObjectInfoCollection : IEnumerable
	{
	}
}
