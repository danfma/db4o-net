/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class PreparedArrayContainsComparison : IPreparedComparison
	{
		private readonly ArrayHandler _arrayHandler;

		private readonly IPreparedComparison _preparedComparison;

		public PreparedArrayContainsComparison(ArrayHandler arrayHandler, ITypeHandler4 typeHandler
			, object obj)
		{
			_arrayHandler = arrayHandler;
			_preparedComparison = typeHandler.PrepareComparison(obj);
		}

		public virtual int CompareTo(object obj)
		{
			throw new InvalidOperationException();
		}

		public virtual bool IsEqual(object array)
		{
			return IsMatch(array, IntMatcher.Zero);
		}

		public virtual bool IsGreaterThan(object array)
		{
			return IsMatch(array, IntMatcher.Positive);
		}

		public virtual bool IsSmallerThan(object array)
		{
			return IsMatch(array, IntMatcher.Negative);
		}

		private bool IsMatch(object array, IntMatcher matcher)
		{
			if (array == null)
			{
				return false;
			}
			IEnumerator i = _arrayHandler.AllElements(array);
			while (i.MoveNext())
			{
				if (matcher.Match(_preparedComparison.CompareTo(i.Current)))
				{
					return true;
				}
			}
			return false;
		}
	}
}