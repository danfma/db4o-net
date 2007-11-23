/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;

namespace Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand
{
	public class StaticFieldRoot : ComparisonOperandRoot
	{
		private string _className;

		public StaticFieldRoot(string className)
		{
			this._className = className;
		}

		public virtual string ClassName()
		{
			return _className;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand.StaticFieldRoot casted = (Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand.StaticFieldRoot
				)obj;
			return _className.Equals(casted._className);
		}

		public override int GetHashCode()
		{
			return _className.GetHashCode();
		}

		public override string ToString()
		{
			return _className;
		}

		public override void Accept(IComparisonOperandVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
