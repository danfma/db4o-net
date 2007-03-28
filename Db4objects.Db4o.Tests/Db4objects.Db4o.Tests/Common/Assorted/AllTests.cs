namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.AllTests().RunSolo();
		}

		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.BackupStressTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.CanUpdateFalseRefreshTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.ChangeIdentity)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.ClassMetadataTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.CloseUnlocksFileTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.DatabaseUnicityTest)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.FileSizeOnRollbackTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.GetByUUIDTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.GetSingleSimpleArrayTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.HandlerRegistryTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.IndexCreateDropTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.LazyObjectReferenceTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.MaximumActivationDepthTestCase), 
				typeof(Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.NakedObjectTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.ObjectVersionTest)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.PersistTypeTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.PreventMultipleOpenTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.ReAddCascadedDeleteTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.ReferenceSystemTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.RollbackTestCase), typeof(Db4objects.Db4o.Tests.Common.Assorted.SimplestPossibleTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Assorted.SystemInfoTestCase) };
		}
	}
}
