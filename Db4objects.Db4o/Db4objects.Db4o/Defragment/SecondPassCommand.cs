/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Defragment
{
	/// <summary>
	/// Second step in the defragmenting process: Fills in target file pointer slots, copies
	/// content slots from source to target and triggers ID remapping therein by calling the
	/// appropriate yap/marshaller defrag() implementations.
	/// </summary>
	/// <remarks>
	/// Second step in the defragmenting process: Fills in target file pointer slots, copies
	/// content slots from source to target and triggers ID remapping therein by calling the
	/// appropriate yap/marshaller defrag() implementations. During the process, the actual address
	/// mappings for the content slots are registered for use with string indices.
	/// </remarks>
	/// <exclude></exclude>
	internal sealed class SecondPassCommand : IPassCommand
	{
		protected readonly int _objectCommitFrequency;

		protected int _objectCount = 0;

		public SecondPassCommand(int objectCommitFrequency)
		{
			_objectCommitFrequency = objectCommitFrequency;
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public void ProcessClass(DefragContextImpl context, ClassMetadata yapClass, int id
			, int classIndexID)
		{
			if (context.MappedID(id, -1) == -1)
			{
				Sharpen.Runtime.Err.WriteLine("MAPPING NOT FOUND: " + id);
			}
			BufferPair.ProcessCopy(context, id, new _ISlotCopyHandler_35(this, yapClass, classIndexID
				));
		}

		private sealed class _ISlotCopyHandler_35 : ISlotCopyHandler
		{
			public _ISlotCopyHandler_35(SecondPassCommand _enclosing, ClassMetadata yapClass, 
				int classIndexID)
			{
				this._enclosing = _enclosing;
				this.yapClass = yapClass;
				this.classIndexID = classIndexID;
			}

			/// <exception cref="CorruptionException"></exception>
			/// <exception cref="IOException"></exception>
			public void ProcessCopy(BufferPair readers)
			{
				yapClass.DefragClass(readers, classIndexID);
			}

			private readonly SecondPassCommand _enclosing;

			private readonly ClassMetadata yapClass;

			private readonly int classIndexID;
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public void ProcessObjectSlot(DefragContextImpl context, ClassMetadata yapClass, 
			int id)
		{
			Db4objects.Db4o.Internal.Buffer sourceBuffer = context.SourceBufferByID(id);
			ObjectHeader objHead = context.SourceObjectHeader(sourceBuffer);
			sourceBuffer._offset = 0;
			bool registerAddresses = context.HasFieldIndex(objHead.ClassMetadata());
			BufferPair.ProcessCopy(context, id, new _ISlotCopyHandler_47(this, context), registerAddresses
				, sourceBuffer);
		}

		private sealed class _ISlotCopyHandler_47 : ISlotCopyHandler
		{
			public _ISlotCopyHandler_47(SecondPassCommand _enclosing, DefragContextImpl context
				)
			{
				this._enclosing = _enclosing;
				this.context = context;
			}

			public void ProcessCopy(BufferPair buffers)
			{
				ClassMetadata.DefragObject(buffers);
				if (this._enclosing._objectCommitFrequency > 0)
				{
					this._enclosing._objectCount++;
					if (this._enclosing._objectCount == this._enclosing._objectCommitFrequency)
					{
						context.TargetCommit();
						this._enclosing._objectCount = 0;
					}
				}
			}

			private readonly SecondPassCommand _enclosing;

			private readonly DefragContextImpl context;
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public void ProcessClassCollection(DefragContextImpl context)
		{
			BufferPair.ProcessCopy(context, context.SourceClassCollectionID(), new _ISlotCopyHandler_62
				(this));
		}

		private sealed class _ISlotCopyHandler_62 : ISlotCopyHandler
		{
			public _ISlotCopyHandler_62(SecondPassCommand _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ProcessCopy(BufferPair readers)
			{
				ClassMetadataRepository.Defrag(readers);
			}

			private readonly SecondPassCommand _enclosing;
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public void ProcessBTree(DefragContextImpl context, BTree btree)
		{
			btree.DefragBTree(context);
		}

		public void Flush(DefragContextImpl context)
		{
		}
	}
}
