namespace Db4oAdmin.Tests.Core
{
	class PreserveDebugInfoTestCase : SingleResourceTestCase
	{
		protected override string ResourceName
		{
			get { return "PreserveDebugInfoSubject"; }
		}

		protected override string CommandLine
		{
			get
			{
				return "-debug"
					+ " -instrumentation:Db4oAdmin.Tests.Core.TraceInstrumentation,Db4oAdmin.Tests";
			}
		}

		// FIXME: remove this method after updating cecil
		// (waiting for Module.FullLoad patch to be applied)
		protected override void InstrumentAssembly(string path)
		{
			 //base.InstrumentAssembly(path);
		}
	}
}