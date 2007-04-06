using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QConEvaluation : QCon
	{
		[System.NonSerialized]
		private object i_evaluation;

		public byte[] i_marshalledEvaluation;

		public int i_marshalledID;

		public QConEvaluation()
		{
		}

		public QConEvaluation(Transaction a_trans, object a_evaluation) : base(a_trans)
		{
			i_evaluation = a_evaluation;
		}

		internal override void EvaluateEvaluationsExec(QCandidates a_candidates, bool rereadObject
			)
		{
			if (rereadObject)
			{
				a_candidates.Traverse(new _AnonymousInnerClass32(this));
			}
			a_candidates.Filter(this);
		}

		private sealed class _AnonymousInnerClass32 : IVisitor4
		{
			public _AnonymousInnerClass32(QConEvaluation _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((QCandidate)a_object).UseField(null);
			}

			private readonly QConEvaluation _enclosing;
		}

		internal override void Marshall()
		{
			base.Marshall();
			MarshallUsingDb4oFormat();
		}

		private void MarshallUsingDb4oFormat()
		{
			SerializedGraph serialized = Serializer.Marshall(Container(), i_evaluation);
			i_marshalledEvaluation = serialized._bytes;
			i_marshalledID = serialized._id;
		}

		internal override void Unmarshall(Transaction a_trans)
		{
			if (i_trans == null)
			{
				base.Unmarshall(a_trans);
				i_evaluation = Serializer.Unmarshall(Container(), i_marshalledEvaluation, i_marshalledID
					);
			}
		}

		public override void Visit(object obj)
		{
			QCandidate candidate = (QCandidate)obj;
			ForceActivation(candidate);
			try
			{
				Platform4.EvaluationEvaluate(i_evaluation, candidate);
			}
			catch (Exception)
			{
				candidate.Include(false);
			}
			if (!candidate._include)
			{
				DoNotInclude(candidate.GetRoot());
			}
		}

		private void ForceActivation(QCandidate candidate)
		{
			candidate.GetObject();
		}

		internal virtual bool SupportsIndex()
		{
			return false;
		}
	}
}
