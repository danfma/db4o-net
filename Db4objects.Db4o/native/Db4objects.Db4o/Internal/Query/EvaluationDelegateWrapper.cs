/* Copyright (C) 2004   Versant Inc.   http://www.db4o.com */

using System;
using System.Reflection;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Internal.Query
{
    // TODO: Use DelegateEnvelope to build a generic delegate translator
    internal class DelegateEnvelope
    {
        System.Type _delegateType;
        object _target;
        System.Type _type;
        string _method;

        [NonSerialized]
        Delegate _content;

        public DelegateEnvelope()
        {
        }

        public DelegateEnvelope(Delegate content)
        {
            _content = content;
            Marshal();
        }

        protected Delegate GetContent()
        {
            if (null == _content)
            {
                _content = Unmarshal();
            }
            return _content;
        }

        private void Marshal()
        {
            _delegateType = _content.GetType();
#if !CF_2_0
            _target = _content.Target;
            _method = _content.Method.Name;
            _type = _content.Method.DeclaringType;
#endif
        }

        private Delegate Unmarshal()
        {
#if CF_2_0
            throw new NotSupportedException();
#else
			if (null == _target)
			{
				return System.Delegate.CreateDelegate(_delegateType, null, _type.GetMethod(_method,  BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public));
			}
				
			return System.Delegate.CreateDelegate(_delegateType, _target, _target.GetType().GetMethod(_method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
#endif
        }
    }

    internal class EvaluationDelegateWrapper : DelegateEnvelope, IEvaluation
    {	
        public EvaluationDelegateWrapper()
        {
        }
		
        public EvaluationDelegateWrapper(EvaluationDelegate evaluation) : base(evaluation)
        {	
        }
		
        EvaluationDelegate GetEvaluationDelegate()
        {
            return (EvaluationDelegate)GetContent();
        }
		
        public void Evaluate(ICandidate candidate)
        {
			// use starting _ for PascalCase conversion purposes
            EvaluationDelegate _evaluation = GetEvaluationDelegate();
            _evaluation(candidate);
        }
    }
}