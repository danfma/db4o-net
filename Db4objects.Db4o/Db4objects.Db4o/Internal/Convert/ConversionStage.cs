using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Convert;

namespace Db4objects.Db4o.Internal.Convert
{
	/// <exclude></exclude>
	public abstract class ConversionStage
	{
		public sealed class ClassCollectionAvailableStage : ConversionStage
		{
			public ClassCollectionAvailableStage(LocalObjectContainer file) : base(file)
			{
			}

			public override void Accept(Conversion conversion)
			{
				conversion.Convert(this);
			}
		}

		public sealed class SystemUpStage : ConversionStage
		{
			public SystemUpStage(LocalObjectContainer file) : base(file)
			{
			}

			public override void Accept(Conversion conversion)
			{
				conversion.Convert(this);
			}
		}

		private LocalObjectContainer _file;

		protected ConversionStage(LocalObjectContainer file)
		{
			_file = file;
		}

		public virtual LocalObjectContainer File()
		{
			return _file;
		}

		public virtual SystemData SystemData()
		{
			return _file.SystemData();
		}

		public abstract void Accept(Conversion conversion);
	}
}
