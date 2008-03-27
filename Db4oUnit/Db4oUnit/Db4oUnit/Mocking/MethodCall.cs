/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Mocking;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Mocking
{
	public class MethodCall
	{
		private sealed class _object_9 : object
		{
			public _object_9()
			{
			}

			public override string ToString()
			{
				return "...";
			}
		}

		public static readonly object IgnoredArgument = new _object_9();

		public interface IArgumentCondition
		{
			void Verify(object argument);
		}

		public readonly string methodName;

		public readonly object[] args;

		public MethodCall(string methodName) : this(methodName, new object[0])
		{
		}

		public MethodCall(string methodName, object[] args)
		{
			this.methodName = methodName;
			this.args = args;
		}

		public MethodCall(string methodName, object singleArg) : this(methodName, new object
			[] { singleArg })
		{
		}

		public MethodCall(string methodName, object arg1, object arg2) : this(methodName, 
			new object[] { arg1, arg2 })
		{
		}

		public override string ToString()
		{
			return methodName + "(" + Iterators.Join(Iterators.Iterate(args), ", ") + ")";
		}

		public override bool Equals(object obj)
		{
			if (null == obj)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			MethodCall other = (MethodCall)obj;
			if (!methodName.Equals(other.methodName))
			{
				return false;
			}
			if (args.Length != other.args.Length)
			{
				return false;
			}
			for (int i = 0; i < args.Length; ++i)
			{
				object expectedArg = args[i];
				if (expectedArg == IgnoredArgument)
				{
					continue;
				}
				object actualArg = other.args[i];
				if (expectedArg is MethodCall.IArgumentCondition)
				{
					((MethodCall.IArgumentCondition)expectedArg).Verify(actualArg);
					continue;
				}
				if (!Check.ObjectsAreEqual(expectedArg, actualArg))
				{
					return false;
				}
			}
			return true;
		}
	}
}