/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fileheader;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeader0 : FileHeader
	{
		internal const int LENGTH = 2 + (Const4.INT_LENGTH * 4);

		private ConfigBlock _configBlock;

		private PBootRecord _bootRecord;

		public override void Close()
		{
			_configBlock.Close();
		}

		protected override FileHeader NewOnSignatureMatch(LocalObjectContainer file, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			byte firstFileByte = reader.ReadByte();
			if (firstFileByte != Const4.YAPBEGIN)
			{
				if (firstFileByte != Const4.YAPFILEVERSION)
				{
					return null;
				}
				file.BlockSizeReadFromFile(reader.ReadByte());
			}
			else
			{
				if (reader.ReadByte() != Const4.YAPFILE)
				{
					return null;
				}
			}
			return new FileHeader0();
		}

		protected override void ReadFixedPart(LocalObjectContainer file, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			_configBlock = ConfigBlock.ForExistingFile(file, reader.ReadInt());
			SkipConfigurationLockTime(reader);
			ReadClassCollectionAndFreeSpace(file, reader);
		}

		private void SkipConfigurationLockTime(Db4objects.Db4o.Internal.Buffer reader)
		{
			reader.IncrementOffset(Const4.ID_LENGTH);
		}

		public override void ReadVariablePart(LocalObjectContainer file)
		{
			if (_configBlock._bootRecordID <= 0)
			{
				return;
			}
			object bootRecord = GetBootRecord(file);
			if (!(bootRecord is PBootRecord))
			{
				InitBootRecord(file);
				file.GenerateNewIdentity();
				return;
			}
			_bootRecord = (PBootRecord)bootRecord;
			file.Activate(bootRecord, int.MaxValue);
			file.SetNextTimeStampId(_bootRecord.i_versionGenerator);
			file.SystemData().Identity(_bootRecord.i_db);
		}

		private object GetBootRecord(LocalObjectContainer file)
		{
			file.ShowInternalClasses(true);
			try
			{
				return file.GetByID(file.SystemTransaction(), _configBlock._bootRecordID);
			}
			finally
			{
				file.ShowInternalClasses(false);
			}
		}

		public override void InitNew(LocalObjectContainer file)
		{
			_configBlock = ConfigBlock.ForNewFile(file);
			InitBootRecord(file);
		}

		private void InitBootRecord(LocalObjectContainer file)
		{
			file.ShowInternalClasses(true);
			try
			{
				_bootRecord = new PBootRecord();
				file.SetInternal(file.SystemTransaction(), _bootRecord, false);
				_configBlock._bootRecordID = file.GetID(file.SystemTransaction(), _bootRecord);
				WriteVariablePart(file, 1);
			}
			finally
			{
				file.ShowInternalClasses(false);
			}
		}

		public override Transaction InterruptedTransaction()
		{
			return _configBlock.GetTransactionToCommit();
		}

		public override void WriteTransactionPointer(Transaction systemTransaction, int transactionAddress
			)
		{
			WriteTransactionPointer(systemTransaction, transactionAddress, _configBlock.Address
				(), ConfigBlock.TRANSACTION_OFFSET);
		}

		public virtual MetaIndex GetUUIDMetaIndex()
		{
			return _bootRecord.GetUUIDMetaIndex();
		}

		public override int Length()
		{
			return LENGTH;
		}

		public override void WriteFixedPart(LocalObjectContainer file, bool startFileLockingThread
			, bool shuttingDown, StatefulBuffer writer, int blockSize_, int freespaceID)
		{
			writer.Append(Const4.YAPFILEVERSION);
			writer.Append((byte)blockSize_);
			writer.WriteInt(_configBlock.Address());
			writer.WriteInt((int)TimeToWrite(_configBlock.OpenTime(), shuttingDown));
			writer.WriteInt(file.SystemData().ClassCollectionID());
			writer.WriteInt(freespaceID);
			if (Debug.xbytes && Deploy.overwrite)
			{
				writer.SetID(Const4.IGNORE_ID);
			}
			writer.Write();
			file.SyncFiles();
		}

		public override void WriteVariablePart(LocalObjectContainer file, int part)
		{
			if (part == 1)
			{
				_configBlock.Write();
			}
			else
			{
				if (part == 2)
				{
					_bootRecord.Write(file);
				}
			}
		}
	}
}
