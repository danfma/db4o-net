/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class StandardReferenceTypeHandler0 : StandardReferenceTypeHandler
	{
		protected override bool IsNull(IFieldListInfo fieldList, int fieldIndex)
		{
			return false;
		}
	}
}
