/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class MapContent
	{
		private string name;

		public MapContent()
		{
		}

		public MapContent(string name)
		{
			this.name = name;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return "name = " + name;
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || GetType() != o.GetType())
			{
				return false;
			}
			Db4objects.Drs.Tests.MapContent that = (Db4objects.Drs.Tests.MapContent)o;
			if (!name.Equals(that.name))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}
	}
}
