/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Extensions
{
	/// <exclude></exclude>
	public class Db4oConcurrenyTestCase : Db4oClientServerTestCase
	{
		private bool[] _done;

		/// <exception cref="Exception"></exception>
		protected override void Db4oSetupAfterStore()
		{
			InitTasksDoneFlag();
			base.Db4oSetupAfterStore();
		}

		private void InitTasksDoneFlag()
		{
			_done = new bool[ThreadCount()];
		}

		protected virtual void MarkTaskDone(int seq, bool done)
		{
			_done[seq] = done;
		}

		/// <exception cref="Exception"></exception>
		protected virtual void WaitForAllTasksDone()
		{
			while (!AreAllTasksDone())
			{
				Cool.SleepIgnoringInterruption(1);
			}
		}

		private bool AreAllTasksDone()
		{
			for (int i = 0; i < _done.Length; ++i)
			{
				if (!_done[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}