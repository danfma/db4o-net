/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;

namespace Db4oUnit
{
	[System.Serializable]
	public class AssertionException : TestException
	{
		private const long serialVersionUID = 900088031151055525L;

		public AssertionException(string message) : base(message, null)
		{
		}

		public AssertionException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}