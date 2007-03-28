namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class FieldMarshaller1 : Db4objects.Db4o.Internal.Marshall.FieldMarshaller0
	{
		private bool HasBTreeIndex(Db4objects.Db4o.Internal.FieldMetadata field)
		{
			return !field.IsVirtual();
		}

		public override void Write(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.ClassMetadata
			 clazz, Db4objects.Db4o.Internal.FieldMetadata field, Db4objects.Db4o.Internal.Buffer
			 writer)
		{
			base.Write(trans, clazz, field, writer);
			if (!HasBTreeIndex(field))
			{
				return;
			}
			writer.WriteIDOf(trans, field.GetIndex(trans));
		}

		public override Db4objects.Db4o.Internal.Marshall.RawFieldSpec ReadSpec(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.Buffer reader)
		{
			Db4objects.Db4o.Internal.Marshall.RawFieldSpec spec = base.ReadSpec(stream, reader
				);
			if (spec == null)
			{
				return null;
			}
			if (spec.IsVirtual())
			{
				return spec;
			}
			int indexID = reader.ReadInt();
			spec.IndexID(indexID);
			return spec;
		}

		protected override Db4objects.Db4o.Internal.FieldMetadata FromSpec(Db4objects.Db4o.Internal.Marshall.RawFieldSpec
			 spec, Db4objects.Db4o.Internal.ObjectContainerBase stream, Db4objects.Db4o.Internal.FieldMetadata
			 field)
		{
			Db4objects.Db4o.Internal.FieldMetadata actualField = base.FromSpec(spec, stream, 
				field);
			if (spec == null)
			{
				return field;
			}
			if (spec.IndexID() != 0)
			{
				actualField.InitIndex(stream.SystemTransaction(), spec.IndexID());
			}
			return actualField;
		}

		public override int MarshalledLength(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.FieldMetadata field)
		{
			int len = base.MarshalledLength(stream, field);
			if (!HasBTreeIndex(field))
			{
				return len;
			}
			int BTREE_ID = Db4objects.Db4o.Internal.Const4.ID_LENGTH;
			return len + BTREE_ID;
		}

		public override void Defrag(Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.FieldMetadata
			 yapField, Db4objects.Db4o.Internal.LatinStringIO sio, Db4objects.Db4o.Internal.ReaderPair
			 readers)
		{
			base.Defrag(yapClass, yapField, sio, readers);
			if (yapField.IsVirtual())
			{
				return;
			}
			if (yapField.HasIndex())
			{
				Db4objects.Db4o.Internal.Btree.BTree index = yapField.GetIndex(readers.SystemTrans
					());
				int targetIndexID = readers.CopyID();
				if (targetIndexID != 0)
				{
					index.DefragBTree(readers.Context());
				}
			}
			else
			{
				readers.WriteInt(0);
			}
		}
	}
}
