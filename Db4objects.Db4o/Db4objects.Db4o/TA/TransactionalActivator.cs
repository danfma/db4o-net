/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.TA
{
	/// <summary>
	/// An
	/// <see cref="IActivator">IActivator</see>
	/// implementation that activates an object on a specific
	/// transaction.
	/// </summary>
	/// <exclude></exclude>
	internal sealed class TransactionalActivator : IActivator
	{
		private readonly Transaction _transaction;

		private readonly ObjectReference _objectReference;

		public TransactionalActivator(Transaction transaction, ObjectReference objectReference
			)
		{
			_objectReference = objectReference;
			_transaction = transaction;
		}

		public void Activate()
		{
			_objectReference.ActivateOn(_transaction);
		}
	}
}