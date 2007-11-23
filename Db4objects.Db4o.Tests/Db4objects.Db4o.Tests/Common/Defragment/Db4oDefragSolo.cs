/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class Db4oDefragSolo : Db4oSolo
	{
		public Db4oDefragSolo(IConfigurationSource configSource) : base(configSource)
		{
		}

		protected override IObjectContainer CreateDatabase(IConfiguration config)
		{
			Sharpen.IO.File origFile = new Sharpen.IO.File(GetAbsolutePath());
			if (origFile.Exists())
			{
				try
				{
					string backupFile = GetAbsolutePath() + ".defrag.backup";
					IContextIDMapping mapping = new TreeIDMapping();
					DefragmentConfig defragConfig = new DefragmentConfig(GetAbsolutePath(), backupFile
						, mapping);
					defragConfig.ForceBackupDelete(true);
					IConfiguration clonedConfig = (IConfiguration)((IDeepClone)config).DeepClone(null
						);
					defragConfig.Db4oConfig(clonedConfig);
					Db4objects.Db4o.Defragment.Defragment.Defrag(defragConfig, new _IDefragmentListener_35
						(this));
				}
				catch (IOException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
			return base.CreateDatabase(config);
		}

		private sealed class _IDefragmentListener_35 : IDefragmentListener
		{
			public _IDefragmentListener_35(Db4oDefragSolo _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void NotifyDefragmentInfo(DefragmentInfo info)
			{
				Sharpen.Runtime.Err.WriteLine(info);
			}

			private readonly Db4oDefragSolo _enclosing;
		}

		public override bool Accept(Type clazz)
		{
			return base.Accept(clazz) && !typeof(IOptOutDefragSolo).IsAssignableFrom(clazz);
		}

		public override string GetLabel()
		{
			return "Defrag-" + base.GetLabel();
		}
	}
}
