﻿/* Copyright (C) 2007 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Linq.Tests.Queries
{
	internal class QueryPrettyPrinter
	{
		private readonly StringBuilder _builder = new StringBuilder();
		private readonly StringBuilder _orderBy = new StringBuilder();
		private readonly HashSet<QConJoin> _visitedJoins = new HashSet<QConJoin>();

		public QueryPrettyPrinter(IQuery query)
		{
			VisitAll(query.Constraints().ToArray());
		}

		private void Visit(IConstraint constraint)
		{
			var constrainType = constraint.GetType().Name;
			switch (constrainType)
			{
				case "QConClass":
					Visit(constraint as QConClass);
					break;
				case "QConObject":
					Visit(constraint as QConObject);
					break;
				case "QConJoin":
					Visit(constraint as QConJoin);
					break;
				case "QConPath":
					Visit(constraint as QConPath);
					break;
				
				default:
					throw new ArgumentException(constrainType);
			 }
		}

		protected virtual void Visit(QConClass klass)
		{
			_builder.Append("(");
			_builder.Append(GetClassName(klass.GetClassName()));

			if (klass._children != null)
			{
				ConsumeList<IConstraint>(klass._children, cons => Visit(cons));
			}
			FlushOrderBy();
			_builder.Append(")");
		}

		private void FlushOrderBy()
		{
			_builder.Append(_orderBy);
			_orderBy.Length = 0;
		}

		private static void ConsumeList<TElement>(List4 list, Action<TElement> action)
		{
			while (list != null)
			{
				action((TElement)list._element);
				list = list._next;
			}
		}

		protected virtual void Visit(QConPath path)
		{
			if (HasOrder(path))
			{
				PrintOrderBy(path);
				return;
			}

			if (IsOrderByPath(path))
			{
				VisitAllChildrenOf(path);
				return;
			}

			PrintDescend(path);
		}

		private static bool IsOrderByPath(QConPath path)
		{
			return Iterators.Iterable(path.IterateChildren()).Cast<QCon>().All(IsOrderByElement);
		}

		private static bool IsOrderByElement(QCon candidate)
		{
			if (HasOrder(candidate))
				return true;
			
			var path = candidate as QConPath;
			if (path != null)
				return IsOrderByPath(path);

			return false;
		}

		private static bool HasOrder(QCon path)
		{
			return path.i_orderID != 0;
		}

		private void PrintDescend(QConPath path)
		{
			_builder.AppendFormat("({0}", path.GetField().i_name);

			VisitAllChildrenOf(path);

			_builder.Append(")");
		}

		private void VisitAllChildrenOf(QCon path)
		{
			VisitAll(Iterators.Iterable(path.IterateChildren()).Cast<IConstraint>());
		}

		private void VisitAll(IEnumerable<IConstraint> constraints)
		{
			foreach (var constraint in constraints)
			{
				Visit(constraint);
			}
		}

		private void PrintOrderBy(QConObject path)
		{
			_orderBy.AppendFormat("(orderby {0} {1})",
				PathFor(path),
				OrderIdToString(path.i_orderID));
		}

		private static string PathFor(QConObject path)
		{
			var fieldName = path.i_field.i_name;

			QConPath parentField = path.Parent() as QConPath;
			if (parentField != null)
				return PathFor(parentField) + "." + fieldName;
			
			return fieldName;
		}

		private static string OrderIdToString(int order)
		{
			return order < 0 ? "desc" : "asc";
		}

		protected virtual void Visit(QConJoin join)
		{
			if (_visitedJoins.Contains(join)) return;
			_builder.Append("(");
			VisitJoinBranch(join.i_constraint2);
			_builder.AppendFormat(" {0} ", join.i_and ? "and" : "or");
			VisitJoinBranch(join.i_constraint1);
			_builder.Append(")");
			_visitedJoins.Add(join);
		}

		private void VisitJoinBranch(QCon branch)
		{
			if (branch is QConJoin)
			{
				Visit(branch as QConJoin);
				return;
			}

			if (branch is QConObject)
			{
				PrintQConObject(branch as QConObject);
				return;
			}

			throw new NotSupportedException();
		}

		protected virtual void Visit(QConObject obj)
		{
			if (obj.i_joins != null)
			{
				foreach (IConstraint constraint in obj.i_joins)
				{
					Visit(constraint);
				}
				return;
			}
			PrintQConObject(obj);
		}

		private void PrintQConObject(QConObject obj)
		{
			if (HasOrder(obj))
			{
				PrintOrderBy(obj);
			}
			
			_builder.AppendFormat("({0} {1} {2})",
				                      obj.GetField().i_name,
				                      EvaluatorToString(obj.i_evaluator),
				                      ValueToString(obj.i_object));
		}

		private static string EvaluatorToString(QE evaluator)
		{
			switch (evaluator.GetType().Name)
			{
				case "QEMulti":
				{
					StringBuilder sb = new StringBuilder();
					foreach (QE qe in ((QEMulti)evaluator).i_evaluators)
					{
						sb.Append(EvaluatorToString(qe));
					}
					return sb.ToString();
				}
				case "QE": return "==";
				case "QESmaller": return "<";
				case "QEGreater": return ">";
				case "QEEqual": return "=";
				case "QENot": return "not";
				case "QEStartsWith": return "startswith";
				case "QEEndsWith": return "endswith";
				case "QEContains": return "contains";
			}

			throw new NotSupportedException();
		}

		private static string ValueToString(object value)
		{	
			if (value is string) return string.Format("'{0}'", value);

			return value.ToString();
		}

		private static string GetClassName(string fullname)
		{
			int pos = fullname.LastIndexOf(",");
			if (pos > -1)
				fullname = fullname.Substring(0, pos);

			pos = fullname.LastIndexOf("+");
			if (pos > -1)
				return ExtractSuffix(fullname, pos);

			pos = fullname.LastIndexOf(".");
			if (pos > -1)
				return ExtractSuffix(fullname, pos);

			return fullname;
		}

		private static string ExtractSuffix(string str, int pos)
		{
			return str.Substring(pos + 1, str.Length - pos - 1);
		}

		public override string ToString()
		{
			return _builder.ToString() + _orderBy;
		}
	}
}
