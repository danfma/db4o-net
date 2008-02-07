/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oInMemory : AbstractSoloDb4oFixture
	{
		public Db4oInMemory() : base(new IndependentConfigurationSource())
		{
		}

		public Db4oInMemory(IConfigurationSource configSource) : base(configSource)
		{
		}

		public Db4oInMemory(IFixtureConfiguration fc) : this()
		{
			FixtureConfiguration(fc);
		}

		private MemoryFile _memoryFile;

		protected override IObjectContainer CreateDatabase(IConfiguration config)
		{
			if (null == _memoryFile)
			{
				_memoryFile = new MemoryFile();
			}
			return ExtDb4oFactory.OpenMemoryFile(config, _memoryFile);
		}

		protected override void DoClean()
		{
			_memoryFile = null;
		}

		public override string GetLabel()
		{
			return BuildLabel("IN-MEMORY");
		}

		/// <exception cref="Exception"></exception>
		public override void Defragment()
		{
		}
		// do nothing
		// defragment is file-based for now
	}
}
