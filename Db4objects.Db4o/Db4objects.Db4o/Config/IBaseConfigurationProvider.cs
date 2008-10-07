/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.Config
{
	/// <since>7.5</since>
	public interface IBaseConfigurationProvider
	{
		IBaseConfiguration Base
		{
			get;
		}
	}
}