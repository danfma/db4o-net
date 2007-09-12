/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Soda;

namespace Db4objects.Db4o.Tests.Common.Soda
{
	public class UntypedEvaluationTestCase : AbstractDb4oTestCase
	{
		private static readonly Type EXTENT = typeof(object);

		public class Data
		{
			public int _id;

			public Data(int id)
			{
				_id = id;
			}
		}

		[System.Serializable]
		public class UntypedEvaluation : IEvaluation
		{
			public bool _value;

			public UntypedEvaluation(bool value)
			{
				_value = value;
			}

			public virtual void Evaluate(ICandidate candidate)
			{
				candidate.Include(_value);
			}
		}

		protected override void Store()
		{
			Store(new UntypedEvaluationTestCase.Data(42));
		}

		public virtual void TestUntypedRaw()
		{
			IQuery query = NewQuery(EXTENT);
			Assert.AreEqual(1, query.Execute().Size());
		}

		public virtual void TestUntypedEvaluationNone()
		{
			IQuery query = NewQuery(EXTENT);
			query.Constrain(new UntypedEvaluationTestCase.UntypedEvaluation(false));
			Assert.AreEqual(0, query.Execute().Size());
		}

		public virtual void TestUntypedEvaluationAll()
		{
			IQuery query = NewQuery(EXTENT);
			query.Constrain(new UntypedEvaluationTestCase.UntypedEvaluation(true));
			Assert.AreEqual(1, query.Execute().Size());
		}
	}
}