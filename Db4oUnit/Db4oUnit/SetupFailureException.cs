using System;
using Db4oUnit;

namespace Db4oUnit
{
	[System.Serializable]
	public class SetupFailureException : TestException
	{
		private const long serialVersionUID = -7835097105469071064L;

		public SetupFailureException(Exception cause) : base(cause)
		{
		}
	}
}
