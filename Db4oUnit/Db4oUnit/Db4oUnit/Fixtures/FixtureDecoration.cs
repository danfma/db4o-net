/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Foundation;
using Sharpen.Lang;

namespace Db4oUnit.Fixtures
{
	public sealed class FixtureDecoration : ITestDecoration
	{
		private readonly ITest _test;

		private readonly ContextVariable _variable;

		private readonly object _value;

		private readonly string _fixtureLabel;

		public FixtureDecoration(ITest test, string fixtureLabel, ContextVariable fixtureVariable
			, object fixtureValue)
		{
			_test = test;
			_fixtureLabel = fixtureLabel;
			_variable = fixtureVariable;
			_value = fixtureValue;
		}

		public string GetLabel()
		{
			ObjectByRef label = new ObjectByRef();
			RunDecorated(new _IRunnable_22(this, label));
			return (string)label.value;
		}

		private sealed class _IRunnable_22 : IRunnable
		{
			public _IRunnable_22(FixtureDecoration _enclosing, ObjectByRef label)
			{
				this._enclosing = _enclosing;
				this.label = label;
			}

			public void Run()
			{
				label.value = "(" + this._enclosing.FixtureLabel() + ") " + this._enclosing._test
					.GetLabel();
			}

			private readonly FixtureDecoration _enclosing;

			private readonly ObjectByRef label;
		}

		public void Run()
		{
			RunDecorated(_test);
		}

		public ITest Test()
		{
			return _test;
		}

		private void RunDecorated(IRunnable block)
		{
			_variable.With(Value(), block);
		}

		private object Value()
		{
			return _value is IDeferred4 ? ((IDeferred4)_value).Value() : _value;
		}

		private object FixtureLabel()
		{
			return (_fixtureLabel == null ? _value : _fixtureLabel);
		}
	}
}