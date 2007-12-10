/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IDeleteContext : IContext, IReadBuffer
	{
		void CascadeDeleteDepth(int depth);

		int CascadeDeleteDepth();

		bool IsLegacyHandlerVersion();

		void DefragmentRecommended();

		Slot ReadSlot();
	}
}