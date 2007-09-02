/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Marshall
{
	/// <summary>
	/// common functionality for
	/// <see cref="IReadContext">IReadContext</see>
	/// and
	/// <see cref="IWriteContext">IWriteContext</see>
	/// </summary>
	public interface IContext
	{
		IObjectContainer ObjectContainer();

		Db4objects.Db4o.Internal.Transaction Transaction();
	}
}
