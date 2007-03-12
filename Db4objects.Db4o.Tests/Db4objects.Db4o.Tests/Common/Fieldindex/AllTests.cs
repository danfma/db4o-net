namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Fieldindex.AllTests().RunSolo();
		}

		protected override System.Type[] TestCases()
		{
			System.Type[] fieldBased = new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Fieldindex.IndexedNodeTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexTestCase), typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexProcessorTestCase)
				 };
			System.Type[] neutral = new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Fieldindex.DoubleFieldIndexTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase), typeof(Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexCorruptionTestCase)
				 };
			System.Type[] tests = neutral;
			tests = new System.Type[fieldBased.Length + neutral.Length];
			System.Array.Copy(neutral, 0, tests, 0, neutral.Length);
			System.Array.Copy(fieldBased, 0, tests, neutral.Length, fieldBased.Length);
			return tests;
		}
	}
}
