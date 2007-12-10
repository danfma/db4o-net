/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	internal class ArrayMarshaller1 : ArrayMarshaller
	{
		protected override Db4objects.Db4o.Internal.Buffer PrepareIDReader(Transaction trans
			, Db4objects.Db4o.Internal.Buffer reader)
		{
			reader._offset = reader.ReadInt();
			return reader;
		}

		public override void DefragIDs(ArrayHandler arrayHandler, BufferPair readers)
		{
			int offset = readers.PreparePayloadRead();
			arrayHandler.Defrag1(new DefragmentContext(_family, readers, true));
			readers.Offset(offset);
		}
	}
}
