/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Classindex;

namespace Db4objects.Db4o.Defragment
{
	/// <summary>defragments database files.</summary>
	/// <remarks>
	/// defragments database files.
	/// <br/><br/>db4o structures storage inside database files as free and occupied slots, very
	/// much like a file system - and just like a file system it can be fragmented.<br/><br/>
	/// The simplest way to defragment a database file:<br/><br/>
	/// <code>Defragment.Defrag("sample.yap");</code><br/><br/>
	/// This will move the file to "sample.yap.backup", then create a defragmented
	/// version of this file in the original position, using a temporary file
	/// "sample.yap.mapping". If the backup file already exists, this will throw an
	/// exception and no action will be taken.<br/><br/>
	/// For more detailed configuration of the defragmentation process, provide a
	/// DefragmentConfig instance:<br/><br/>
	/// <code>
	/// DefragmentConfig config=new DefragmentConfig("sample.yap","sample.bap",new BTreeIDMapping("sample.map"));<br/>
	/// config.ForceBackupDelete(true);<br/>
	/// config.StoredClassFilter(new AvailableClassFilter());<br/>
	/// config.Db4oConfig(db4oConfig);<br/>
	/// Defragment.Defrag(config);
	/// </code><br/><br/>
	/// This will move the file to "sample.bap", then create a defragmented version
	/// of this file in the original position, using a temporary file "sample.map" for BTree mapping.
	/// If the backup file already exists, it will be deleted. The defragmentation
	/// process will skip all classes that have instances stored within the yap file,
	/// but that are not available on the class path (through the current
	/// classloader). Custom db4o configuration options are read from the
	/// <see cref="IConfiguration">IConfiguration</see>
	/// passed as db4oConfig.
	/// <strong>Note:</strong> For some specific, non-default configuration settings like
	/// UUID generation, etc., you <strong>must</strong> pass an appropriate db4o configuration,
	/// just like you'd use it within your application for normal database operation.
	/// </remarks>
	public class Defragment
	{
		/// <summary>
		/// Renames the file at the given original path to a backup file and then
		/// builds a defragmented version of the file in the original place.
		/// </summary>
		/// <remarks>
		/// Renames the file at the given original path to a backup file and then
		/// builds a defragmented version of the file in the original place.
		/// </remarks>
		/// <param name="origPath">The path to the file to be defragmented.</param>
		/// <exception cref="IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(string origPath)
		{
			Defrag(new DefragmentConfig(origPath), new Defragment.NullListener());
		}

		/// <summary>
		/// Renames the file at the given original path to the given backup file and
		/// then builds a defragmented version of the file in the original place.
		/// </summary>
		/// <remarks>
		/// Renames the file at the given original path to the given backup file and
		/// then builds a defragmented version of the file in the original place.
		/// </remarks>
		/// <param name="origPath">The path to the file to be defragmented.</param>
		/// <param name="backupPath">The path to the backup file to be created.</param>
		/// <exception cref="IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(string origPath, string backupPath)
		{
			Defrag(new DefragmentConfig(origPath, backupPath), new Defragment.NullListener());
		}

		/// <summary>
		/// Renames the file at the configured original path to the configured backup
		/// path and then builds a defragmented version of the file in the original
		/// place.
		/// </summary>
		/// <remarks>
		/// Renames the file at the configured original path to the configured backup
		/// path and then builds a defragmented version of the file in the original
		/// place.
		/// </remarks>
		/// <param name="config">The configuration for this defragmentation run.</param>
		/// <exception cref="IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(DefragmentConfig config)
		{
			Defrag(config, new Defragment.NullListener());
		}

		/// <summary>
		/// Renames the file at the configured original path to the configured backup
		/// path and then builds a defragmented version of the file in the original
		/// place.
		/// </summary>
		/// <remarks>
		/// Renames the file at the configured original path to the configured backup
		/// path and then builds a defragmented version of the file in the original
		/// place.
		/// </remarks>
		/// <param name="config">The configuration for this defragmentation run.</param>
		/// <param name="listener">
		/// A listener for status notifications during the defragmentation
		/// process.
		/// </param>
		/// <exception cref="IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(DefragmentConfig config, IDefragmentListener listener)
		{
			Sharpen.IO.File backupFile = new Sharpen.IO.File(config.BackupPath());
			if (backupFile.Exists())
			{
				if (!config.ForceBackupDelete())
				{
					throw new IOException("Could not use '" + config.BackupPath() + "' as backup path - file exists."
						);
				}
				backupFile.Delete();
			}
			System.IO.File.Move(config.OrigPath(), config.BackupPath());
			if (config.FileNeedsUpgrade())
			{
				UpgradeFile(config);
			}
			DefragContextImpl context = new DefragContextImpl(config, listener);
			int newClassCollectionID = 0;
			int targetIdentityID = 0;
			int targetUuidIndexID = 0;
			try
			{
				FirstPass(context, config);
				SecondPass(context, config);
				DefragUnindexed(context);
				newClassCollectionID = context.MappedID(context.SourceClassCollectionID());
				context.TargetClassCollectionID(newClassCollectionID);
				int sourceIdentityID = context.DatabaseIdentityID(DefragContextImpl.SOURCEDB);
				targetIdentityID = context.MappedID(sourceIdentityID, 0);
				targetUuidIndexID = context.MappedID(context.SourceUuidIndexID(), 0);
			}
			catch (CorruptionException exc)
			{
				Sharpen.Runtime.PrintStackTrace(exc);
			}
			finally
			{
				context.Close();
			}
			if (targetIdentityID > 0)
			{
				SetIdentity(config, targetIdentityID, targetUuidIndexID);
			}
			else
			{
				listener.NotifyDefragmentInfo(new DefragmentInfo("No database identity found in original file."
					));
			}
		}

		/// <exception cref="IOException"></exception>
		private static void UpgradeFile(DefragmentConfig config)
		{
			File4.Copy(config.BackupPath(), config.TempPath());
			IConfiguration db4oConfig = (IConfiguration)((Config4Impl)config.Db4oConfig()).DeepClone
				(null);
			db4oConfig.AllowVersionUpdates(true);
			IObjectContainer db = Db4oFactory.OpenFile(db4oConfig, config.TempPath());
			db.Close();
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		private static void DefragUnindexed(DefragContextImpl context)
		{
			IEnumerator unindexedIDs = context.UnindexedIDs();
			while (unindexedIDs.MoveNext())
			{
				int origID = ((int)unindexedIDs.Current);
				BufferPair.ProcessCopy(context, origID, new _ISlotCopyHandler_167(), true);
			}
		}

		private sealed class _ISlotCopyHandler_167 : ISlotCopyHandler
		{
			public _ISlotCopyHandler_167()
			{
			}

			/// <exception cref="CorruptionException"></exception>
			public void ProcessCopy(BufferPair readers)
			{
				ClassMetadata.DefragObject(readers);
			}
		}

		private static void SetIdentity(DefragmentConfig config, int targetIdentityID, int
			 targetUuidIndexID)
		{
			LocalObjectContainer targetDB = (LocalObjectContainer)Db4oFactory.OpenFile(config
				.ClonedDb4oConfig(), config.OrigPath());
			try
			{
				Db4oDatabase identity = (Db4oDatabase)targetDB.GetByID(targetIdentityID);
				targetDB.SetIdentity(identity);
				targetDB.SystemData().UuidIndexId(targetUuidIndexID);
			}
			finally
			{
				targetDB.Close();
			}
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		private static void FirstPass(DefragContextImpl context, DefragmentConfig config)
		{
			Pass(context, config, new FirstPassCommand());
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		private static void SecondPass(DefragContextImpl context, DefragmentConfig config
			)
		{
			Pass(context, config, new SecondPassCommand(config.ObjectCommitFrequency()));
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		private static void Pass(DefragContextImpl context, DefragmentConfig config, IPassCommand
			 command)
		{
			command.ProcessClassCollection(context);
			IStoredClass[] classes = context.StoredClasses(DefragContextImpl.SOURCEDB);
			for (int classIdx = 0; classIdx < classes.Length; classIdx++)
			{
				ClassMetadata yapClass = (ClassMetadata)classes[classIdx];
				if (!config.StoredClassFilter().Accept(yapClass))
				{
					continue;
				}
				ProcessYapClass(context, yapClass, command);
				command.Flush(context);
				if (config.ObjectCommitFrequency() > 0)
				{
					context.TargetCommit();
				}
			}
			BTree uuidIndex = context.SourceUuidIndex();
			if (uuidIndex != null)
			{
				command.ProcessBTree(context, uuidIndex);
			}
			command.Flush(context);
			context.TargetCommit();
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		private static void ProcessYapClass(DefragContextImpl context, ClassMetadata curClass
			, IPassCommand command)
		{
			ProcessClassIndex(context, curClass, command);
			if (!ParentHasIndex(curClass))
			{
				ProcessObjectsForYapClass(context, curClass, command);
			}
			ProcessYapClassAndFieldIndices(context, curClass, command);
		}

		private static bool ParentHasIndex(ClassMetadata curClass)
		{
			ClassMetadata parentClass = curClass.i_ancestor;
			while (parentClass != null)
			{
				if (parentClass.HasClassIndex())
				{
					return true;
				}
				parentClass = parentClass.i_ancestor;
			}
			return false;
		}

		private static void ProcessObjectsForYapClass(DefragContextImpl context, ClassMetadata
			 curClass, IPassCommand command)
		{
			context.TraverseAll(curClass, new _IVisitor4_258(command, context, curClass));
		}

		private sealed class _IVisitor4_258 : IVisitor4
		{
			public _IVisitor4_258(IPassCommand command, DefragContextImpl context, ClassMetadata
				 curClass)
			{
				this.command = command;
				this.context = context;
				this.curClass = curClass;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				try
				{
					command.ProcessObjectSlot(context, curClass, id);
				}
				catch (CorruptionException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
				catch (IOException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}

			private readonly IPassCommand command;

			private readonly DefragContextImpl context;

			private readonly ClassMetadata curClass;
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		private static void ProcessYapClassAndFieldIndices(DefragContextImpl context, ClassMetadata
			 curClass, IPassCommand command)
		{
			int sourceClassIndexID = 0;
			int targetClassIndexID = 0;
			if (curClass.HasClassIndex())
			{
				sourceClassIndexID = curClass.Index().Id();
				targetClassIndexID = context.MappedID(sourceClassIndexID, -1);
			}
			command.ProcessClass(context, curClass, curClass.GetID(), targetClassIndexID);
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		private static void ProcessClassIndex(DefragContextImpl context, ClassMetadata curClass
			, IPassCommand command)
		{
			if (curClass.HasClassIndex())
			{
				BTreeClassIndexStrategy indexStrategy = (BTreeClassIndexStrategy)curClass.Index();
				BTree btree = indexStrategy.Btree();
				command.ProcessBTree(context, btree);
			}
		}

		internal class NullListener : IDefragmentListener
		{
			public virtual void NotifyDefragmentInfo(DefragmentInfo info)
			{
			}
		}
	}
}
