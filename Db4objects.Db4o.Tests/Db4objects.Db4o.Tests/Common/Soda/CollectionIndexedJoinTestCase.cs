using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Soda;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Soda
{
	public class CollectionIndexedJoinTestCase : AbstractDb4oTestCase
	{
		private static readonly string COLLECTIONFIELDNAME = "_data";

		private static readonly string IDFIELDNAME = "_id";

		private const int NUMENTRIES = 3;

		public class DataHolder
		{
			public ArrayList _data;

			public DataHolder(int id)
			{
				_data = new ArrayList();
				_data.Add(new CollectionIndexedJoinTestCase.Data(id));
			}
		}

		public class Data
		{
			public int _id;

			public Data(int id)
			{
				this._id = id;
			}
		}

		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(CollectionIndexedJoinTestCase.Data)).ObjectField(IDFIELDNAME
				).Indexed(true);
		}

		protected override void Store()
		{
			for (int i = 0; i < NUMENTRIES; i++)
			{
				Store(new CollectionIndexedJoinTestCase.DataHolder(i));
			}
		}

		public virtual void TestIndexedOrTwo()
		{
			AssertIndexedOr(new int[] { 0, 1, -1 }, 2);
		}

		private void AssertIndexedOr(int[] values, int expectedResultCount)
		{
			CollectionIndexedJoinTestCase.TestConfig config = new CollectionIndexedJoinTestCase.TestConfig
				(values.Length);
			while (config.MoveNext())
			{
				AssertIndexedOr(values, expectedResultCount, config.RootIndex(), config.ConnectLeft
					());
			}
		}

		public virtual void TestIndexedOrAll()
		{
			AssertIndexedOr(new int[] { 0, 1, 2 }, 3);
		}

		public virtual void TestTwoJoinLegs()
		{
			IQuery query = NewQuery(typeof(CollectionIndexedJoinTestCase.DataHolder)).Descend
				(COLLECTIONFIELDNAME);
			IConstraint left = query.Descend(IDFIELDNAME).Constrain(0);
			left.Or(query.Descend(IDFIELDNAME).Constrain(1));
			IConstraint right = query.Descend(IDFIELDNAME).Constrain(2);
			right.Or(query.Descend(IDFIELDNAME).Constrain(-1));
			left.Or(right);
			IObjectSet result = query.Execute();
			Assert.AreEqual(3, result.Size());
		}

		public virtual void AssertIndexedOr(int[] values, int expectedResultCount, int rootIdx
			, bool connectLeft)
		{
			IQuery query = NewQuery(typeof(CollectionIndexedJoinTestCase.DataHolder)).Descend
				(COLLECTIONFIELDNAME);
			IConstraint constraint = query.Descend(IDFIELDNAME).Constrain(values[rootIdx]);
			for (int idx = 0; idx < values.Length; idx++)
			{
				if (idx != rootIdx)
				{
					IConstraint curConstraint = query.Descend(IDFIELDNAME).Constrain(values[idx]);
					if (connectLeft)
					{
						constraint.Or(curConstraint);
					}
					else
					{
						curConstraint.Or(constraint);
					}
				}
			}
			IObjectSet result = query.Execute();
			Assert.AreEqual(expectedResultCount, result.Size());
		}

		private class TestConfig : PermutingTestConfig
		{
			public TestConfig(int numValues) : base(new object[][] { new object[] { 0, numValues
				 - 1 }, new object[] { false, true } })
			{
			}

			public virtual int RootIndex()
			{
				return ((int)Current(0));
			}

			public virtual bool ConnectLeft()
			{
				return ((bool)Current(1));
			}
		}
	}
}
