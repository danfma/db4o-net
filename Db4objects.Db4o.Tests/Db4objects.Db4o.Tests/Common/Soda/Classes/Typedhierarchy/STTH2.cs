using Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy;

namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy
{
	public class STTH2
	{
		public STTH3 h3;

		public string foo2;

		public STTH2()
		{
		}

		public STTH2(STTH3 a3)
		{
			h3 = a3;
		}

		public STTH2(string str)
		{
			foo2 = str;
		}

		public STTH2(STTH3 a3, string str)
		{
			h3 = a3;
			foo2 = str;
		}
	}
}
