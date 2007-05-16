/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Messaging
{
	/// <summary>message sender for client/server messaging.</summary>
	/// <remarks>
	/// message sender for client/server messaging.
	/// <br /><br />db4o allows using the client/server TCP connection to send
	/// messages from the client to the server. Any object that can be
	/// stored to a db4o database file may be used as a message.<br /><br />
	/// See the sample in ../com/db4o/samples/messaging/ on how to
	/// use the messaging feature. It is also used to stop the server
	/// in ../com/db4o/samples/clientserver/StopServer.java<br /><br />
	/// <b>See Also:</b><br />
	/// <see cref="com.db4o.config.Configuration#getMessageSender()">com.db4o.config.Configuration#getMessageSender()
	/// 	</see>
	/// ,<br />
	/// <see cref="IMessageRecipient">IMessageRecipient</see>
	/// ,<br />
	/// <see cref="com.db4o.config.Configuration#setMessageRecipient(MessageRecipient)">com.db4o.config.Configuration#setMessageRecipient(MessageRecipient)
	/// 	</see>
	/// </remarks>
	public interface IMessageSender
	{
		/// <summary>sends a message to the server.</summary>
		/// <remarks>sends a message to the server.</remarks>
		/// <param name="obj">the message parameter, any object may be used.</param>
		void Send(object obj);
	}
}
