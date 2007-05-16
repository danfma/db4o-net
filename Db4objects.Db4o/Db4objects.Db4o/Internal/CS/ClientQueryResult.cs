/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class ClientQueryResult : IdListQueryResult
	{
		public ClientQueryResult(Transaction ta) : base(ta)
		{
		}

		public ClientQueryResult(Transaction ta, int initialSize) : base(ta, initialSize)
		{
		}

		public override IEnumerator GetEnumerator()
		{
			return ClientServerPlatform.CreateClientQueryResultIterator(this);
		}
	}
}
