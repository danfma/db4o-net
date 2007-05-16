/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Convert;

namespace Db4objects.Db4o.Internal.Convert.Conversions
{
	/// <exclude></exclude>
	public class FieldIndexesToBTrees_5_7 : Conversion
	{
		public const int VERSION = 6;

		public override void Convert(ConversionStage.SystemUpStage stage)
		{
			stage.File().ClassCollection().WriteAllClasses();
			RebuildUUIDIndex(stage.File());
			FreeOldUUIDMetaIndex(stage.File());
		}

		private void RebuildUUIDIndex(LocalObjectContainer file)
		{
			UUIDFieldMetadata uuid = file.GetUUIDIndex();
			ClassMetadataIterator i = file.ClassCollection().Iterator();
			while (i.MoveNext())
			{
				ClassMetadata clazz = i.CurrentClass();
				if (clazz.GenerateUUIDs())
				{
					uuid.RebuildIndexForClass(file, clazz);
				}
			}
		}

		private void FreeOldUUIDMetaIndex(LocalObjectContainer file)
		{
		}
	}
}
