using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QENot : QE
	{
		public QE i_evaluator;

		public QENot()
		{
		}

		internal QENot(QE a_evaluator)
		{
			i_evaluator = a_evaluator;
		}

		internal override QE Add(QE evaluator)
		{
			if (!(evaluator is Db4objects.Db4o.Internal.Query.Processor.QENot))
			{
				i_evaluator = i_evaluator.Add(evaluator);
			}
			return this;
		}

		public override bool Identity()
		{
			return i_evaluator.Identity();
		}

		internal override bool IsDefault()
		{
			return false;
		}

		internal override bool Evaluate(QConObject a_constraint, QCandidate a_candidate, 
			object a_value)
		{
			return !i_evaluator.Evaluate(a_constraint, a_candidate, a_value);
		}

		internal override bool Not(bool res)
		{
			return !res;
		}

		public override void IndexBitMap(bool[] bits)
		{
			i_evaluator.IndexBitMap(bits);
			for (int i = 0; i < 4; i++)
			{
				bits[i] = !bits[i];
			}
		}

		public override bool SupportsIndex()
		{
			return i_evaluator.SupportsIndex();
		}
	}
}
