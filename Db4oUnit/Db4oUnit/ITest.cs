/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;

namespace Db4oUnit
{
	public interface ITest
	{
		string GetLabel();

		void Run(TestResult result);
	}
}
