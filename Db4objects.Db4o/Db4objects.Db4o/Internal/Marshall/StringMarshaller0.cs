/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	public class StringMarshaller0 : StringMarshaller
	{
		public override bool InlinedStrings()
		{
			return false;
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="Db4oIOException"></exception>
		public override Db4objects.Db4o.Internal.Buffer ReadIndexEntry(StatefulBuffer parentSlot
			)
		{
			return parentSlot.GetStream().ReadWriterByAddress(parentSlot.GetTransaction(), parentSlot
				.ReadInt(), parentSlot.ReadInt());
		}

		public override void Defrag(ISlotBuffer reader)
		{
		}
	}
}
