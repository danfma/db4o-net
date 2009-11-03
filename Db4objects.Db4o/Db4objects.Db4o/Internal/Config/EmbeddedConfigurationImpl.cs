/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Config;

namespace Db4objects.Db4o.Internal.Config
{
	public class EmbeddedConfigurationImpl : IEmbeddedConfiguration, ILegacyConfigurationProvider
	{
		private readonly Config4Impl _legacy;

		public EmbeddedConfigurationImpl(IConfiguration legacy)
		{
			_legacy = (Config4Impl)legacy;
		}

		public virtual ICacheConfiguration Cache
		{
			get
			{
				return new CacheConfigurationImpl(_legacy);
			}
		}

		public virtual IFileConfiguration File
		{
			get
			{
				return new FileConfigurationImpl(_legacy);
			}
		}

		public virtual ICommonConfiguration Common
		{
			get
			{
				return Db4oLegacyConfigurationBridge.AsCommonConfiguration(Legacy());
			}
		}

		public virtual Config4Impl Legacy()
		{
			return _legacy;
		}
	}
}
