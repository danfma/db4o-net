/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QEStartsWith : Db4objects.Db4o.Internal.Query.Processor.QEStringCmp
	{
		/// <summary>for C/S messaging only</summary>
		public QEStartsWith()
		{
		}

		public QEStartsWith(bool caseSensitive_) : base(caseSensitive_)
		{
		}

		protected override bool CompareStrings(string candidate, string constraint)
		{
			return candidate.IndexOf(constraint) == 0;
		}
	}
}
