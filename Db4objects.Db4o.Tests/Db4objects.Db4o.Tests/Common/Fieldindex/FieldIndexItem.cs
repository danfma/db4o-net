using Db4objects.Db4o.Tests.Common.Fieldindex;

namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class FieldIndexItem : IHasFoo
	{
		public int foo;

		public FieldIndexItem()
		{
		}

		public FieldIndexItem(int foo_)
		{
			foo = foo_;
		}

		public virtual int GetFoo()
		{
			return foo;
		}
	}
}
