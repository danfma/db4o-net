using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class TSerializableOnInstantiateIOExceptionTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new TSerializableOnInstantiateIOExceptionTestCase().RunAll();
		}
	}
}