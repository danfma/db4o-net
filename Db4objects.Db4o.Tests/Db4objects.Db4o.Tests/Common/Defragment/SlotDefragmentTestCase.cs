/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Defragment;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class SlotDefragmentTestCase : ITestLifeCycle
	{
		/// <exception cref="Exception"></exception>
		public virtual void TestPrimitiveIndex()
		{
			SlotDefragmentFixture.AssertIndex(SlotDefragmentFixture.PRIMITIVE_FIELDNAME);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWrapperIndex()
		{
			SlotDefragmentFixture.AssertIndex(SlotDefragmentFixture.WRAPPER_FIELDNAME);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestTypedObjectIndex()
		{
			SlotDefragmentFixture.ForceIndex();
			Db4objects.Db4o.Defragment.Defragment.Defrag(SlotDefragmentTestConstants.FILENAME
				, SlotDefragmentTestConstants.BACKUPFILENAME);
			IObjectContainer db = Db4oFactory.OpenFile(Db4oFactory.NewConfiguration(), SlotDefragmentTestConstants
				.FILENAME);
			IQuery query = db.Query();
			query.Constrain(typeof(SlotDefragmentFixture.Data));
			query.Descend(SlotDefragmentFixture.TYPEDOBJECT_FIELDNAME).Descend(SlotDefragmentFixture
				.PRIMITIVE_FIELDNAME).Constrain(SlotDefragmentFixture.VALUE);
			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Size());
			db.Close();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestNoForceDelete()
		{
			Db4objects.Db4o.Defragment.Defragment.Defrag(SlotDefragmentTestConstants.FILENAME
				, SlotDefragmentTestConstants.BACKUPFILENAME);
			Assert.Expect(typeof(IOException), new _ICodeBlock_37(this));
		}

		private sealed class _ICodeBlock_37 : ICodeBlock
		{
			public _ICodeBlock_37(SlotDefragmentTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				Db4objects.Db4o.Defragment.Defragment.Defrag(SlotDefragmentTestConstants.FILENAME
					, SlotDefragmentTestConstants.BACKUPFILENAME);
			}

			private readonly SlotDefragmentTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			new Sharpen.IO.File(SlotDefragmentTestConstants.FILENAME).Delete();
			new Sharpen.IO.File(SlotDefragmentTestConstants.BACKUPFILENAME).Delete();
			SlotDefragmentFixture.CreateFile(SlotDefragmentTestConstants.FILENAME);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
		}
	}
}
