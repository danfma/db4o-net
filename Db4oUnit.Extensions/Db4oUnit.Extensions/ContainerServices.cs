/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o;

namespace Db4oUnit.Extensions
{
	public class ContainerServices
	{
		/// <exception cref="Exception"></exception>
		public static void WithContainer(IObjectContainer container, IContainerBlock block
			)
		{
			try
			{
				block.Run(container);
			}
			finally
			{
				container.Close();
			}
		}
	}
}
