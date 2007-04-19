using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>An old file was detected and could not be open.</summary>
	/// <remarks>An old file was detected and could not be open.</remarks>
	[System.Serializable]
	public class OldFormatException : Db4oException
	{
		public OldFormatException() : base(Db4objects.Db4o.Internal.Messages.OLD_DATABASE_FORMAT
			)
		{
		}
	}
}
