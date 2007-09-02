/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Callbacks;
using Db4objects.Db4o.Internal.Query;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IInternalObjectContainer : IExtObjectContainer
	{
		void Callbacks(ICallbacks cb);

		ICallbacks Callbacks();

		ObjectContainerBase Container();

		Db4objects.Db4o.Internal.Transaction Transaction();

		void OnCommittedListener();

		NativeQueryHandler GetNativeQueryHandler();

		ClassMetadata ClassMetadataForReflectClass(IReflectClass reflectClass);

		ClassMetadata ClassMetadataForName(string name);

		ClassMetadata ClassMetadataForId(int id);

		HandlerRegistry Handlers();
	}
}
