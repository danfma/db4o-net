using Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy;

namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy
{
	public class STRTH3
	{
		public STRTH1TestCase grandParent;

		public STRTH2 parent;

		public string foo3;

		public STRTH3()
		{
		}

		public STRTH3(string str)
		{
			foo3 = str;
		}
	}
}
