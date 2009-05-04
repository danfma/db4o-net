/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Types
{
	/// <summary>db4o Map implementation for database-aware maps.</summary>
	/// <remarks>
	/// db4o Map implementation for database-aware maps.
	/// <br /><br />
	/// A <code>Db4oMap</code> supplies the methods specified in java.util.Map.<br /><br />
	/// All access to the map is controlled by the
	/// <see cref="Db4objects.Db4o.IObjectContainer">IObjectContainer</see>
	/// to help the
	/// programmer produce expected results with as little work as possible:<br />
	/// - newly added objects are automatically persisted.<br />
	/// - map elements are automatically activated when they are needed. The activation
	/// depth is configurable with
	/// <see cref="Db4objects.Db4o.Types.IDb4oCollection.ActivationDepth">Db4objects.Db4o.Types.IDb4oCollection.ActivationDepth
	/// 	</see>
	/// .<br />
	/// - removed objects can be deleted automatically, if the list is configured
	/// with
	/// <see cref="Db4objects.Db4o.Types.IDb4oCollection.DeleteRemoved">Db4objects.Db4o.Types.IDb4oCollection.DeleteRemoved
	/// 	</see>
	/// <br /><br />
	/// Usage:<br />
	/// - declare a <code>java.util.Map</code> variable on your persistent classes.<br />
	/// - fill this variable with a method in the ObjectContainer collection factory.<br /><br />
	/// <b>Example:</b><br /><br />
	/// <code>class MyClass{<br />
	/// &nbsp;&nbsp;Map myMap;<br />
	/// }<br /><br />
	/// MyClass myObject = new MyClass();<br />
	/// myObject.myMap = objectContainer.ext().collections().newHashMap();
	/// </remarks>
	/// <seealso cref="com.db4o.ext.ExtObjectContainer#collections">com.db4o.ext.ExtObjectContainer#collections
	/// 	</seealso>
	[System.ObsoleteAttribute(@"since 7.0")]
	public interface IDb4oMap : IDb4oCollection, IDictionary
	{
	}
}
