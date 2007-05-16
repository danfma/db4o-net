/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Internal.Query
{
	public interface IDb4oNQOptimizer
	{
		object Optimize(IQuery query, Predicate filter);
	}
}
