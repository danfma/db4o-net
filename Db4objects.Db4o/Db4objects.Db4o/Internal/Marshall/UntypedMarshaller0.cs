/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class UntypedMarshaller0 : UntypedMarshaller
	{
		/// <exception cref="Db4oIOException"></exception>
		public override void DeleteEmbedded(StatefulBuffer parentBytes)
		{
			int objectID = parentBytes.ReadInt();
			if (objectID > 0)
			{
				StatefulBuffer reader = parentBytes.GetStream().ReadWriterByID(parentBytes.GetTransaction
					(), objectID);
				if (reader != null)
				{
					reader.SetCascadeDeletes(parentBytes.CascadeDeletes());
					ObjectHeader oh = new ObjectHeader(reader);
					if (oh.ClassMetadata() != null)
					{
						oh.ClassMetadata().DeleteEmbedded1(_family, reader, objectID);
					}
				}
			}
		}

		public override bool UseNormalClassRead()
		{
			return true;
		}

		public override ITypeHandler4 ReadArrayHandler(Transaction a_trans, Db4objects.Db4o.Internal.Buffer[]
			 a_bytes)
		{
			int id = 0;
			int offset = a_bytes[0]._offset;
			try
			{
				id = a_bytes[0].ReadInt();
			}
			catch (Exception)
			{
			}
			a_bytes[0]._offset = offset;
			if (id != 0)
			{
				StatefulBuffer reader = a_trans.Container().ReadWriterByID(a_trans, id);
				if (reader != null)
				{
					ObjectHeader oh = new ObjectHeader(reader);
					try
					{
						if (oh.ClassMetadata() != null)
						{
							a_bytes[0] = reader;
							return oh.ClassMetadata().ReadArrayHandler1(a_bytes);
						}
					}
					catch (Exception e)
					{
					}
				}
			}
			return null;
		}

		public override void Defrag(BufferPair readers)
		{
		}
	}
}
