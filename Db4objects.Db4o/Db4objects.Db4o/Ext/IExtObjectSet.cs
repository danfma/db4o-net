/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// extended functionality for the
	/// <see cref="IObjectSet">IObjectSet</see>
	/// interface.
	/// <br /><br />Every db4o
	/// <see cref="IObjectSet">IObjectSet</see>
	/// always is an ExtObjectSet so a cast is possible.<br /><br />
	/// <see cref="IObjectSet.Ext">IObjectSet.Ext</see>
	/// is a convenient method to perform the cast.<br /><br />
	/// The ObjectSet functionality is split to two interfaces to allow newcomers to
	/// focus on the essential methods.
	/// </summary>
	public interface IExtObjectSet : IObjectSet
	{
		/// <summary>returns an array of internal IDs that correspond to the contained objects.
		/// 	</summary>
		/// <remarks>
		/// returns an array of internal IDs that correspond to the contained objects.
		/// <br /><br />
		/// </remarks>
		/// <seealso cref="IExtObjectContainer.GetID">IExtObjectContainer.GetID</seealso>
		/// <seealso cref="IExtObjectContainer.GetByID">IExtObjectContainer.GetByID</seealso>
		long[] GetIDs();

		/// <summary>returns the item at position [index] in this ObjectSet.</summary>
		/// <remarks>
		/// returns the item at position [index] in this ObjectSet.
		/// <br /><br />
		/// The object will be activated.
		/// </remarks>
		/// <param name="index">the index position in this ObjectSet.</param>
		/// <returns>the activated object.</returns>
		object Get(int index);
	}
}
