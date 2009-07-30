/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Sharpen.Lang;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public sealed class SimpleTimer : IRunnable
	{
		private readonly IRunnable _runnable;

		private readonly int _interval;

		private readonly string _name;

		private Lock4 _lock;

		public volatile bool stopped = false;

		public SimpleTimer(IRunnable runnable, int interval, string name)
		{
			_runnable = runnable;
			_interval = interval;
			_name = name;
			_lock = new Lock4();
		}

		public void Stop()
		{
			stopped = true;
			_lock.Run(new _IClosure4_30(this));
		}

		private sealed class _IClosure4_30 : IClosure4
		{
			public _IClosure4_30(SimpleTimer _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				this._enclosing._lock.Awake();
				return null;
			}

			private readonly SimpleTimer _enclosing;
		}

		public void Run()
		{
			SetThreadName();
			TimerLoop();
		}

		private void TimerLoop()
		{
			while (!stopped)
			{
				_lock.Run(new _IClosure4_45(this));
				if (!stopped)
				{
					_runnable.Run();
				}
			}
		}

		private sealed class _IClosure4_45 : IClosure4
		{
			public _IClosure4_45(SimpleTimer _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				this._enclosing._lock.Snooze(this._enclosing._interval);
				return null;
			}

			private readonly SimpleTimer _enclosing;
		}

		private void SetThreadName()
		{
			Thread.CurrentThread().SetName(_name);
		}
	}
}
