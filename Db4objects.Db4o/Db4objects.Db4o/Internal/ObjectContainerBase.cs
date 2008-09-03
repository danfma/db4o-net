/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class ObjectContainerBase : Db4objects.Db4o.Internal.PartialObjectContainer
		, System.IDisposable
	{
		public ObjectContainerBase(IConfiguration config, ObjectContainerBase a_parent) : 
			base(config, a_parent)
		{
		}
	}
}