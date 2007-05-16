/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Internal.Diagnostic
{
	/// <exclude>FIXME: remove me from the core and make me a facade over Events</exclude>
	public class DiagnosticProcessor : IDiagnosticConfiguration, IDeepClone
	{
		private Collection4 _listeners;

		public DiagnosticProcessor()
		{
		}

		private DiagnosticProcessor(Collection4 listeners)
		{
			_listeners = listeners;
		}

		public virtual void AddListener(IDiagnosticListener listener)
		{
			if (_listeners == null)
			{
				_listeners = new Collection4();
			}
			_listeners.Add(listener);
		}

		public virtual void CheckClassHasFields(ClassMetadata yc)
		{
			FieldMetadata[] fields = yc.i_fields;
			if (fields != null && fields.Length == 0)
			{
				string name = yc.GetName();
				string[] ignoredPackages = new string[] { "java.util." };
				for (int i = 0; i < ignoredPackages.Length; i++)
				{
					if (name.IndexOf(ignoredPackages[i]) == 0)
					{
						return;
					}
				}
				if (IsDb4oClass(yc))
				{
					return;
				}
				OnDiagnostic(new ClassHasNoFields(name));
			}
		}

		public virtual void CheckUpdateDepth(int depth)
		{
			if (depth > 1)
			{
				OnDiagnostic(new UpdateDepthGreaterOne(depth));
			}
		}

		public virtual object DeepClone(object context)
		{
			return new Db4objects.Db4o.Internal.Diagnostic.DiagnosticProcessor(CloneListeners
				());
		}

		private Collection4 CloneListeners()
		{
			return _listeners != null ? new Collection4(_listeners) : null;
		}

		public virtual bool Enabled()
		{
			return _listeners != null;
		}

		private bool IsDb4oClass(ClassMetadata yc)
		{
			return Platform4.IsDb4oClass(yc.GetName());
		}

		public virtual void LoadedFromClassIndex(ClassMetadata yc)
		{
			if (IsDb4oClass(yc))
			{
				return;
			}
			OnDiagnostic(new Db4objects.Db4o.Diagnostic.LoadedFromClassIndex(yc.GetName()));
		}

		public virtual void DescendIntoTranslator(ClassMetadata parent, string fieldName)
		{
			OnDiagnostic(new Db4objects.Db4o.Diagnostic.DescendIntoTranslator(parent.GetName(
				), fieldName));
		}

		public virtual void NativeQueryUnoptimized(Predicate predicate)
		{
			OnDiagnostic(new NativeQueryNotOptimized(predicate));
		}

		public virtual void OnDiagnostic(IDiagnostic d)
		{
			if (_listeners == null)
			{
				return;
			}
			IEnumerator i = _listeners.GetEnumerator();
			while (i.MoveNext())
			{
				((IDiagnosticListener)i.Current).OnDiagnostic(d);
			}
		}

		public virtual void RemoveAllListeners()
		{
			_listeners = null;
		}
	}
}
