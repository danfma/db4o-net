/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public sealed class SearchTarget
	{
		public static readonly Db4objects.Db4o.Internal.Btree.SearchTarget LOWEST = new Db4objects.Db4o.Internal.Btree.SearchTarget
			("Lowest");

		public static readonly Db4objects.Db4o.Internal.Btree.SearchTarget ANY = new Db4objects.Db4o.Internal.Btree.SearchTarget
			("Any");

		public static readonly Db4objects.Db4o.Internal.Btree.SearchTarget HIGHEST = new 
			Db4objects.Db4o.Internal.Btree.SearchTarget("Highest");

		private readonly string _target;

		public SearchTarget(string target)
		{
			_target = target;
		}

		public override string ToString()
		{
			return _target;
		}
	}
}
