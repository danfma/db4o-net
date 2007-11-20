/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
namespace Db4objects.Drs
{
	/// <summary>Defines the contract for handling of replication events generated from a replication session.
	/// 	</summary>
	/// <remarks>
	/// Defines the contract for handling of replication events generated from a replication session.
	/// Users can implement this interface to resolve replication conflicts according to their own business rules.
	/// </remarks>
	/// <author>Albert Kwan</author>
	/// <author>Klaus Wuestefeld</author>
	/// <version>1.2</version>
	/// <since>dRS 1.2</since>
	public interface IReplicationEventListener
	{
		/// <summary>invoked when a replication of an object occurs.</summary>
		/// <remarks>invoked when a replication of an object occurs.</remarks>
		/// <param name="e"></param>
		void OnReplicate(Db4objects.Drs.IReplicationEvent e);
	}
}