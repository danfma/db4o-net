/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;

namespace Db4oUnit
{
	public abstract class Printable
	{
		public override string ToString()
		{
			StringWriter writer = new StringWriter();
			try
			{
				Print(writer);
			}
			catch (IOException)
			{
			}
			return writer.ToString();
		}

		/// <exception cref="IOException"></exception>
		public abstract void Print(TextWriter writer);
	}
}
