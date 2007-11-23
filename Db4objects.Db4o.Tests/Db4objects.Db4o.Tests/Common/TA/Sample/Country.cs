/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.Common.TA.Sample;

namespace Db4objects.Db4o.Tests.Common.TA.Sample
{
	public class Country : IActivatable
	{
		public State[] _states;

		public virtual State GetState(string zipCode)
		{
			Activate();
			return _states[0];
		}

		[System.NonSerialized]
		private IActivator _activator;

		public virtual void Activate()
		{
			if (_activator != null)
			{
				_activator.Activate();
			}
		}

		public virtual void Bind(IActivator activator)
		{
			if (_activator != null || activator == null)
			{
				throw new InvalidOperationException();
			}
			_activator = activator;
		}
	}
}