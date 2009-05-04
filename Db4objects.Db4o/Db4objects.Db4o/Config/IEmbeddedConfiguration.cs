/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.Config
{
	/// <summary>Configuration interface for db4o in embedded use.</summary>
	/// <remarks>Configuration interface for db4o in embedded use.</remarks>
	/// <since>7.5</since>
	public interface IEmbeddedConfiguration : IFileConfigurationProvider, ICommonConfigurationProvider
		, ICacheConfigurationProvider
	{
	}
}
