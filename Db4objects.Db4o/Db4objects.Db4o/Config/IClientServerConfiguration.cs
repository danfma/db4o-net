/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Config
{
	/// <summary>Client/Server configuration interface.</summary>
	/// <remarks>Client/Server configuration interface.</remarks>
	public interface IClientServerConfiguration
	{
		/// <summary>
		/// Sets the number of IDs to be pre-allocated in the database for new
		/// objects created on the client.
		/// </summary>
		/// <remarks>
		/// Sets the number of IDs to be pre-allocated in the database for new
		/// objects created on the client.
		/// This setting should be used on the client side. In embedded mode this setting
		/// has no effect.
		/// </remarks>
		/// <param name="prefetchIDCount">The number of IDs to be prefetched</param>
		void PrefetchIDCount(int prefetchIDCount);

		/// <summary>Sets the number of objects to be prefetched for an ObjectSet in C/S mode.
		/// 	</summary>
		/// <remarks>
		/// Sets the number of objects to be prefetched for an ObjectSet in C/S mode.
		/// This setting should be used on the server side. In embedded mode this setting
		/// has no effect.
		/// </remarks>
		/// <param name="prefetchObjectCount">The number of objects to be prefetched</param>
		void PrefetchObjectCount(int prefetchObjectCount);

		/// <summary>sets the MessageRecipient to receive Client Server messages.</summary>
		/// <remarks>
		/// sets the MessageRecipient to receive Client Server messages. <br />
		/// <br />
		/// This setting should be used on the server side.<br /><br />
		/// </remarks>
		/// <param name="messageRecipient">the MessageRecipient to be used</param>
		void SetMessageRecipient(IMessageRecipient messageRecipient);

		/// <summary>returns the MessageSender for this Configuration context.</summary>
		/// <remarks>
		/// returns the MessageSender for this Configuration context.
		/// This setting should be used on the client side.
		/// </remarks>
		/// <returns>MessageSender</returns>
		IMessageSender GetMessageSender();

		/// <summary>
		/// configures the time a client waits for a message response from the
		/// server.
		/// </summary>
		/// <remarks>
		/// configures the time a client waits for a message response from the
		/// server. <br />
		/// <br />
		/// Default value: 300000ms (5 minutes)<br />
		/// <br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <param name="milliseconds">time in milliseconds</param>
		void TimeoutClientSocket(int milliseconds);

		/// <summary>configures the timeout of the serverside socket.</summary>
		/// <remarks>
		/// configures the timeout of the serverside socket. <br />
		/// <br />
		/// All server connection threads jump out of the socket read statement on a
		/// regular interval to check if the server was shut down. Use this method to
		/// configure the duration of the interval.<br />
		/// <br />
		/// Default value: 5000ms (5 seconds)<br />
		/// <br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <param name="milliseconds">time in milliseconds</param>
		void TimeoutServerSocket(int milliseconds);

		/// <summary>
		/// configures the delay time after which the server starts pinging connected
		/// clients to check the connection.
		/// </summary>
		/// <remarks>
		/// configures the delay time after which the server starts pinging connected
		/// clients to check the connection. <br />
		/// <br />
		/// If no client messages are received by the server for the configured
		/// interval, the server sends a "PING" message to the client and wait's for
		/// an "PONG" response. <br />
		/// <br />
		/// This value may need to be increased for single-threaded clients, since
		/// they can't respond instantaneously. <br />
		/// <br />
		/// Default value: 180000ms (3 minutes)<br />
		/// <br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <param name="milliseconds">time in milliseconds</param>
		/// <seealso cref="IClientServerConfiguration.SingleThreadedClient">IClientServerConfiguration.SingleThreadedClient
		/// 	</seealso>
		void PingInterval(int milliseconds);

		/// <summary>
		/// configures the client messaging system to be single threaded
		/// or multithreaded.
		/// </summary>
		/// <remarks>
		/// configures the client messaging system to be single threaded
		/// or multithreaded.
		/// <br /><br />Recommended settings:<br />
		/// - <code>true</code> for low resource systems.<br />
		/// - <code>false</code> for best asynchronous performance and fast
		/// GUI response.
		/// <br /><br />Default value:<br />
		/// - .NET Compactframework: <code>true</code><br />
		/// - all other platforms: <code>false</code><br /><br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <param name="flag">the desired setting</param>
		void SingleThreadedClient(bool flag);

		/// <summary>Configures to batch messages between client and server.</summary>
		/// <remarks>
		/// Configures to batch messages between client and server. By default, batch
		/// mode is enabled.<br /><br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <param name="flag">false, to turn message batching off.</param>
		void BatchMessages(bool flag);

		/// <summary>Configures the maximum memory buffer size for batched message.</summary>
		/// <remarks>
		/// Configures the maximum memory buffer size for batched message. If the
		/// size of batched messages is greater than <code>maxSize</code>, batched
		/// messages will be sent to server.<br /><br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <param name="maxSize"></param>
		void MaxBatchQueueSize(int maxSize);
	}
}
