/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.IX;

namespace Db4objects.Db4o.Internal.IX
{
	/// <summary>An addition to a field index.</summary>
	/// <remarks>An addition to a field index.</remarks>
	public class IxAdd : IxPatch
	{
		internal bool _keepRemoved;

		public IxAdd(IndexTransaction a_ft, int a_parentID, object a_value) : base(a_ft, 
			a_parentID, a_value)
		{
		}

		internal override void BeginMerge()
		{
			base.BeginMerge();
			Handler().PrepareComparison(IxDeprecationHelper.ComparableObject(Handler(), Trans
				(), _value));
		}

		public override void Visit(object obj)
		{
			((IVisitor4)obj).Visit(_parentID);
		}

		public override void Visit(IVisitor4 visitor, int[] lowerAndUpperMatch)
		{
			visitor.Visit(_parentID);
		}

		public override void FreespaceVisit(FreespaceVisitor visitor, int index)
		{
			visitor.Visit(_parentID, ((int)_value));
		}

		public override int Write(IIndexable4 a_handler, StatefulBuffer a_writer)
		{
			a_handler.WriteIndexEntry(a_writer, _value);
			a_writer.WriteInt(_parentID);
			a_writer.WriteForward();
			return 1;
		}

		public override string ToString()
		{
			string str = "IxAdd " + _parentID + "\n " + IxDeprecationHelper.ComparableObject(
				Handler(), Trans(), _value);
			return str;
		}

		public override void VisitAll(IIntObjectVisitor visitor)
		{
			visitor.Visit(_parentID, IxDeprecationHelper.ComparableObject(Handler(), Trans(), 
				_value));
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Internal.IX.IxAdd add = new Db4objects.Db4o.Internal.IX.IxAdd(_fieldTransaction
				, _parentID, _value);
			base.ShallowCloneInternal(add);
			add._keepRemoved = _keepRemoved;
			return add;
		}
	}
}
