/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4oUnit;

namespace Db4oUnit
{
	/// <summary>Reflection based db4ounit.Test implementation.</summary>
	/// <remarks>Reflection based db4ounit.Test implementation.</remarks>
	public class TestMethod : TestAdapter
	{
		private sealed class _ILabelProvider_11 : ILabelProvider
		{
			public _ILabelProvider_11()
			{
			}

			public string GetLabel(Db4oUnit.TestMethod method)
			{
				return method.GetSubject().GetType().FullName + "." + method.GetMethod().Name;
			}
		}

		public static ILabelProvider DEFAULT_LABEL_PROVIDER = new _ILabelProvider_11();

		private readonly object _subject;

		private readonly MethodInfo _method;

		private readonly ILabelProvider _labelProvider;

		public TestMethod(object instance, MethodInfo method) : this(instance, method, DEFAULT_LABEL_PROVIDER
			)
		{
		}

		public TestMethod(object instance, MethodInfo method, ILabelProvider labelProvider
			)
		{
			if (null == instance)
			{
				throw new ArgumentException("instance");
			}
			if (null == method)
			{
				throw new ArgumentException("method");
			}
			if (null == labelProvider)
			{
				throw new ArgumentException("labelProvider");
			}
			_subject = instance;
			_method = method;
			_labelProvider = labelProvider;
		}

		public virtual object GetSubject()
		{
			return _subject;
		}

		public virtual MethodInfo GetMethod()
		{
			return _method;
		}

		public override string GetLabel()
		{
			return _labelProvider.GetLabel(this);
		}

		protected override void RunTest()
		{
			Invoke();
		}

		protected virtual void Invoke()
		{
			_method.Invoke(_subject, new object[0]);
		}

		protected override void TearDown()
		{
			if (_subject is ITestLifeCycle)
			{
				try
				{
					((ITestLifeCycle)_subject).TearDown();
				}
				catch (Exception e)
				{
					throw new TearDownFailureException(e);
				}
			}
		}

		protected override void SetUp()
		{
			if (_subject is ITestLifeCycle)
			{
				try
				{
					((ITestLifeCycle)_subject).SetUp();
				}
				catch (Exception e)
				{
					throw new SetupFailureException(e);
				}
			}
		}
	}
}
