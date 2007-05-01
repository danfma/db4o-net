using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o
{
	/// <summary>factory class to start db4o database engines.</summary>
	/// <remarks>
	/// factory class to start db4o database engines.
	/// <br /><br />This class provides static methods to<br />
	/// - open single-user databases
	/// <see cref="Db4oFactory.OpenFile">Db4oFactory.OpenFile</see>
	/// <br />
	/// - open db4o servers
	/// <see cref="Db4oFactory.OpenServer">Db4oFactory.OpenServer</see>
	/// <br />
	/// - connect to db4o servers
	/// <see cref="Db4oFactory.OpenClient">Db4oFactory.OpenClient</see>
	/// <br />
	/// - provide access to the global configuration context
	/// <see cref="Db4oFactory.Configure">Db4oFactory.Configure</see>
	/// <br />
	/// - print the version number of this db4o version
	/// <see cref="Db4oFactory.Main">Db4oFactory.Main</see>
	/// 
	/// </remarks>
	/// <seealso cref="ExtDb4oFactory">ExtDb4o for extended functionality.</seealso>
	public class Db4oFactory
	{
		internal static readonly Config4Impl i_config = new Config4Impl();

		static Db4oFactory()
		{
			Platform4.GetDefaultConfiguration(i_config);
		}

		/// <summary>prints the version name of this db4o version to <code>System.out</code>.
		/// 	</summary>
		/// <remarks>prints the version name of this db4o version to <code>System.out</code>.
		/// 	</remarks>
		public static void Main(string[] args)
		{
			Sharpen.Runtime.Out.WriteLine(Version());
		}

		/// <summary>
		/// returns the global db4o
		/// <see cref="IConfiguration">Configuration</see>
		/// context
		/// for the running JVM session.
		/// <br /><br />
		/// The
		/// <see cref="IConfiguration">Configuration</see>
		/// can be overriden in each
		/// <see cref="IExtObjectContainer.Configure">ObjectContainer</see>
		/// .<br /><br />
		/// </summary>
		/// <returns>
		/// the global
		/// <see cref="IConfiguration">configuration</see>
		/// context
		/// </returns>
		public static IConfiguration Configure()
		{
			return i_config;
		}

		/// <summary>
		/// Creates a fresh
		/// <see cref="IConfiguration">Configuration</see>
		/// instance.
		/// </summary>
		/// <returns>a fresh, independent configuration with all options set to their default values
		/// 	</returns>
		public static IConfiguration NewConfiguration()
		{
			Config4Impl config = new Config4Impl();
			Platform4.GetDefaultConfiguration(config);
			return config;
		}

		/// <summary>
		/// Creates a clone of the global db4o
		/// <see cref="IConfiguration">Configuration</see>
		/// .
		/// </summary>
		/// <returns>
		/// a fresh configuration with all option values set to the values
		/// currently configured for the global db4o configuration context
		/// </returns>
		public static IConfiguration CloneConfiguration()
		{
			return (Config4Impl)((IDeepClone)Db4oFactory.Configure()).DeepClone(null);
		}

		/// <summary>
		/// Operates just like
		/// <see cref="Db4oFactory.OpenClient">Db4oFactory.OpenClient</see>
		/// , but uses
		/// the global db4o
		/// <see cref="IConfiguration">Configuration</see>
		/// context.
		/// opens an
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// client and connects it to the specified named server and port.
		/// <br /><br />
		/// The server needs to
		/// <see cref="IObjectServer.GrantAccess">allow access</see>
		/// for the specified user and password.
		/// <br /><br />
		/// A client
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// can be cast to
		/// <see cref="IExtClient">ExtClient</see>
		/// to use extended
		/// <see cref="IExtObjectContainer">ExtObjectContainer</see>
		/// 
		/// and
		/// <see cref="IExtClient">ExtClient</see>
		/// methods.
		/// <br /><br />
		/// </summary>
		/// <param name="hostName">the host name</param>
		/// <param name="port">the port the server is using</param>
		/// <param name="user">the user name</param>
		/// <param name="password">the user password</param>
		/// <returns>
		/// an open
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// </returns>
		/// <seealso cref="IObjectServer.GrantAccess">IObjectServer.GrantAccess</seealso>
		public static IObjectContainer OpenClient(string hostName, int port, string user, 
			string password)
		{
			return OpenClient(Db4oFactory.CloneConfiguration(), hostName, port, user, password
				);
		}

		/// <summary>
		/// opens an
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// client and connects it to the specified named server and port.
		/// <br /><br />
		/// The server needs to
		/// <see cref="IObjectServer.GrantAccess">allow access</see>
		/// for the specified user and password.
		/// <br /><br />
		/// A client
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// can be cast to
		/// <see cref="IExtClient">ExtClient</see>
		/// to use extended
		/// <see cref="IExtObjectContainer">ExtObjectContainer</see>
		/// 
		/// and
		/// <see cref="IExtClient">ExtClient</see>
		/// methods.
		/// <br /><br />
		/// </summary>
		/// <param name="config">
		/// a custom
		/// <see cref="IConfiguration">Configuration</see>
		/// instance to be obtained via
		/// <see cref="Db4oFactory.NewConfiguration">Db4oFactory.NewConfiguration</see>
		/// </param>
		/// <param name="hostName">the host name</param>
		/// <param name="port">the port the server is using</param>
		/// <param name="user">the user name</param>
		/// <param name="password">the user password</param>
		/// <returns>
		/// an open
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// </returns>
		/// <seealso cref="IObjectServer.GrantAccess">IObjectServer.GrantAccess</seealso>
		public static IObjectContainer OpenClient(IConfiguration config, string hostName, 
			int port, string user, string password)
		{
			try
			{
				NetworkSocket networkSocket = new NetworkSocket(hostName, port);
				return new ClientObjectContainer(config, networkSocket, user, password, true);
			}
			catch (IOException e)
			{
				throw new OpenDatabaseException(e);
			}
		}

		/// <summary>
		/// Operates just like
		/// <see cref="Db4oFactory.OpenFile">Db4oFactory.OpenFile</see>
		/// , but uses
		/// the global db4o
		/// <see cref="IConfiguration">Configuration</see>
		/// context.
		/// opens an
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// on the specified database file for local use.
		/// <br /><br />A database file can only be opened once, subsequent attempts to open
		/// another
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// against the same file will result in
		/// a
		/// <see cref="DatabaseFileLockedException">DatabaseFileLockedException</see>
		/// .<br /><br />
		/// Database files can only be accessed for readwrite access from one process
		/// (one Java VM) at one time. All versions except for db4o mobile edition use an
		/// internal mechanism to lock the database file for other processes.
		/// <br /><br />
		/// </summary>
		/// <param name="databaseFileName">an absolute or relative path to the database file</param>
		/// <returns>
		/// an open
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// </returns>
		/// <seealso cref="IConfiguration.ReadOnly">IConfiguration.ReadOnly</seealso>
		/// <seealso cref="IConfiguration.Encrypt">IConfiguration.Encrypt</seealso>
		/// <seealso cref="IConfiguration.Password">IConfiguration.Password</seealso>
		public static IObjectContainer OpenFile(string databaseFileName)
		{
			return OpenFile(CloneConfiguration(), databaseFileName);
		}

		/// <summary>
		/// opens an
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// on the specified database file for local use.
		/// <br /><br />A database file can only be opened once, subsequent attempts to open
		/// another
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// against the same file will result in
		/// a
		/// <see cref="DatabaseFileLockedException">DatabaseFileLockedException</see>
		/// .<br /><br />
		/// Database files can only be accessed for readwrite access from one process
		/// (one Java VM) at one time. All versions except for db4o mobile edition use an
		/// internal mechanism to lock the database file for other processes.
		/// <br /><br />
		/// </summary>
		/// <param name="config">
		/// a custom
		/// <see cref="IConfiguration">Configuration</see>
		/// instance to be obtained via
		/// <see cref="Db4oFactory.NewConfiguration">Db4oFactory.NewConfiguration</see>
		/// </param>
		/// <param name="databaseFileName">an absolute or relative path to the database file</param>
		/// <returns>
		/// an open
		/// <see cref="IObjectContainer">ObjectContainer</see>
		/// </returns>
		/// <seealso cref="IConfiguration.ReadOnly">IConfiguration.ReadOnly</seealso>
		/// <seealso cref="IConfiguration.Encrypt">IConfiguration.Encrypt</seealso>
		/// <seealso cref="IConfiguration.Password">IConfiguration.Password</seealso>
		public static IObjectContainer OpenFile(IConfiguration config, string databaseFileName
			)
		{
			return ObjectContainerFactory.OpenObjectContainer(config, databaseFileName);
		}

		protected static IObjectContainer OpenMemoryFile1(IConfiguration config, MemoryFile
			 memoryFile)
		{
			if (memoryFile == null)
			{
				memoryFile = new MemoryFile();
			}
			try
			{
				IObjectContainer oc = new InMemoryObjectContainer(config, memoryFile);
				Db4objects.Db4o.Internal.Messages.LogMsg(i_config, 5, "Memory File");
				return oc;
			}
			catch (IOException)
			{
				Exceptions4.ShouldNeverHappen();
				return null;
			}
		}

		/// <summary>
		/// Operates just like
		/// <see cref="Db4oFactory.OpenServer">Db4oFactory.OpenServer</see>
		/// , but uses
		/// the global db4o
		/// <see cref="IConfiguration">Configuration</see>
		/// context.
		/// opens an
		/// <see cref="IObjectServer">ObjectServer</see>
		/// on the specified database file and port.
		/// <br /><br />
		/// If the server does not need to listen on a port because it will only be used
		/// in embedded mode with
		/// <see cref="IObjectServer.OpenClient">IObjectServer.OpenClient</see>
		/// , specify '0' as the
		/// port number.
		/// </summary>
		/// <param name="databaseFileName">an absolute or relative path to the database file</param>
		/// <param name="port">
		/// the port to be used, or 0, if the server should not open a port,
		/// because it will only be used with
		/// <see cref="IObjectServer.OpenClient">IObjectServer.OpenClient</see>
		/// </param>
		/// <returns>
		/// an
		/// <see cref="IObjectServer">ObjectServer</see>
		/// listening
		/// on the specified port.
		/// </returns>
		/// <seealso cref="IConfiguration.ReadOnly">IConfiguration.ReadOnly</seealso>
		/// <seealso cref="IConfiguration.Encrypt">IConfiguration.Encrypt</seealso>
		/// <seealso cref="IConfiguration.Password">IConfiguration.Password</seealso>
		public static IObjectServer OpenServer(string databaseFileName, int port)
		{
			return OpenServer(CloneConfiguration(), databaseFileName, port);
		}

		/// <summary>
		/// opens an
		/// <see cref="IObjectServer">ObjectServer</see>
		/// on the specified database file and port.
		/// <br /><br />
		/// If the server does not need to listen on a port because it will only be used
		/// in embedded mode with
		/// <see cref="IObjectServer.OpenClient">IObjectServer.OpenClient</see>
		/// , specify '0' as the
		/// port number.
		/// </summary>
		/// <param name="config">
		/// a custom
		/// <see cref="IConfiguration">Configuration</see>
		/// instance to be obtained via
		/// <see cref="Db4oFactory.NewConfiguration">Db4oFactory.NewConfiguration</see>
		/// </param>
		/// <param name="databaseFileName">an absolute or relative path to the database file</param>
		/// <param name="port">
		/// the port to be used, or 0, if the server should not open a port,
		/// because it will only be used with
		/// <see cref="IObjectServer.OpenClient">IObjectServer.OpenClient</see>
		/// </param>
		/// <returns>
		/// an
		/// <see cref="IObjectServer">ObjectServer</see>
		/// listening
		/// on the specified port.
		/// </returns>
		/// <seealso cref="IConfiguration.ReadOnly">IConfiguration.ReadOnly</seealso>
		/// <seealso cref="IConfiguration.Encrypt">IConfiguration.Encrypt</seealso>
		/// <seealso cref="IConfiguration.Password">IConfiguration.Password</seealso>
		public static IObjectServer OpenServer(IConfiguration config, string databaseFileName
			, int port)
		{
			LocalObjectContainer stream = (LocalObjectContainer)OpenFile(config, databaseFileName
				);
			if (stream == null)
			{
				return null;
			}
			lock (stream.Lock())
			{
				return new ObjectServerImpl(stream, port);
			}
		}

		internal static IReflector Reflector()
		{
			return i_config.Reflector();
		}

		/// <summary>returns the version name of the used db4o version.</summary>
		/// <remarks>
		/// returns the version name of the used db4o version.
		/// <br /><br />
		/// </remarks>
		/// <returns>version information as a <code>String</code>.</returns>
		public static string Version()
		{
			return "db4o " + Db4oVersion.NAME;
		}
	}
}
