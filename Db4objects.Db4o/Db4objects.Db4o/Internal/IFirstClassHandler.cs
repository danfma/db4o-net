/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IFirstClassHandler
	{
		void CascadeActivation(ActivationContext4 context);

		ITypeHandler4 ReadCandidateHandler(QueryingReadContext context);

		void CollectIDs(QueryingReadContext context);
	}
}
