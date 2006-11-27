namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	/// <renameto>com.db4o.inside.Comparable4</renameto>
	public interface IYapComparable
	{
		Db4objects.Db4o.IYapComparable PrepareComparison(object obj);

		int CompareTo(object obj);

		bool IsEqual(object obj);

		bool IsGreater(object obj);

		bool IsSmaller(object obj);

		object Current();
	}
}
