namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy
{
	public class STUH2
	{
		public object h3;

		public object foo2;

		public STUH2()
		{
		}

		public STUH2(Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH3 a3)
		{
			h3 = a3;
		}

		public STUH2(string str)
		{
			foo2 = str;
		}

		public STUH2(Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH3 a3, 
			string str)
		{
			h3 = a3;
			foo2 = str;
		}
	}
}
