/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;

namespace Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand
{
	public interface IComparisonOperandAnchor : IComparisonOperand
	{
		IComparisonOperandAnchor Parent();

		IComparisonOperandAnchor Root();
	}
}
