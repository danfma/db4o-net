/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;

namespace Db4oUnit.Extensions
{
	public interface IRuntimeConfigureAction
	{
		void Apply(IConfiguration config);
	}
}