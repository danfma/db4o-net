/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;

namespace Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand
{
	public class PredicateFieldRoot : ComparisonOperandRoot
	{
		public static readonly Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand.PredicateFieldRoot
			 INSTANCE = new Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand.PredicateFieldRoot
			();

		private PredicateFieldRoot()
		{
		}

		public override string ToString()
		{
			return "PREDICATE";
		}

		public override void Accept(IComparisonOperandVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}