/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface ICommittedCallbackDispatcher
	{
		bool WillDispatchCommitted();

		void DispatchCommitted(CallbackObjectInfoCollections committedInfo);
	}
}