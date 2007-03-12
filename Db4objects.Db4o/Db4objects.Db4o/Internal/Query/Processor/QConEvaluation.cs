namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QConEvaluation : Db4objects.Db4o.Internal.Query.Processor.QCon
	{
		[System.NonSerialized]
		private object i_evaluation;

		public byte[] i_marshalledEvaluation;

		public int i_marshalledID;

		public QConEvaluation()
		{
		}

		public QConEvaluation(Db4objects.Db4o.Internal.Transaction a_trans, object a_evaluation
			) : base(a_trans)
		{
			i_evaluation = a_evaluation;
		}

		internal override void EvaluateEvaluationsExec(Db4objects.Db4o.Internal.Query.Processor.QCandidates
			 a_candidates, bool rereadObject)
		{
			if (rereadObject)
			{
				a_candidates.Traverse(new _AnonymousInnerClass32(this));
			}
			a_candidates.Filter(this);
		}

		private sealed class _AnonymousInnerClass32 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass32(QConEvaluation _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((Db4objects.Db4o.Internal.Query.Processor.QCandidate)a_object).UseField(null);
			}

			private readonly QConEvaluation _enclosing;
		}

		internal override void Marshall()
		{
			base.Marshall();
			int[] id = new int[] { 0 };
			i_marshalledEvaluation = i_trans.Stream().Marshall(Db4objects.Db4o.Internal.Platform4
				.WrapEvaluation(i_evaluation), id);
			i_marshalledID = id[0];
		}

		internal override void Unmarshall(Db4objects.Db4o.Internal.Transaction a_trans)
		{
			if (i_trans == null)
			{
				base.Unmarshall(a_trans);
				i_evaluation = i_trans.Stream().Unmarshall(i_marshalledEvaluation, i_marshalledID
					);
			}
		}

		public override void Visit(object obj)
		{
			Db4objects.Db4o.Internal.Query.Processor.QCandidate candidate = (Db4objects.Db4o.Internal.Query.Processor.QCandidate
				)obj;
			try
			{
				Db4objects.Db4o.Internal.Platform4.EvaluationEvaluate(i_evaluation, candidate);
				if (!candidate._include)
				{
					DoNotInclude(candidate.GetRoot());
				}
			}
			catch
			{
				candidate.Include(false);
				DoNotInclude(candidate.GetRoot());
			}
		}

		internal virtual bool SupportsIndex()
		{
			return false;
		}
	}
}
