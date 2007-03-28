namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public abstract class PrimitiveHandler : Db4objects.Db4o.Internal.ITypeHandler4
	{
		protected readonly Db4objects.Db4o.Internal.ObjectContainerBase _stream;

		protected Db4objects.Db4o.Reflect.IReflectClass _classReflector;

		private Db4objects.Db4o.Reflect.IReflectClass _primitiveClassReflector;

		public PrimitiveHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream)
		{
			_stream = stream;
		}

		private bool i_compareToIsNull;

		public virtual bool CanHold(Db4objects.Db4o.Reflect.IReflectClass claxx)
		{
			return claxx.Equals(ClassReflector());
		}

		public virtual void CascadeActivation(Db4objects.Db4o.Internal.Transaction a_trans
			, object a_object, int a_depth, bool a_activate)
		{
		}

		public virtual object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object 
			obj)
		{
			return CanHold(claxx) ? obj : Db4objects.Db4o.Foundation.No4.INSTANCE;
		}

		public virtual object ComparableObject(Db4objects.Db4o.Internal.Transaction a_trans
			, object a_object)
		{
			return a_object;
		}

		public virtual void CopyValue(object a_from, object a_to)
		{
		}

		public abstract object DefaultValue();

		public virtual void DeleteEmbedded(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.StatefulBuffer a_bytes)
		{
			a_bytes.IncrementOffset(LinkLength());
		}

		public virtual bool Equals(Db4objects.Db4o.Internal.ITypeHandler4 a_dataType)
		{
			return (this == a_dataType);
		}

		public virtual int GetTypeID()
		{
			return Db4objects.Db4o.Internal.Const4.TYPE_SIMPLE;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata GetClassMetadata(Db4objects.Db4o.Internal.ObjectContainerBase
			 a_stream)
		{
			return a_stream.i_handlers.PrimitiveClassById(GetID());
		}

		public virtual bool HasFixedLength()
		{
			return true;
		}

		public virtual object IndexEntryToObject(Db4objects.Db4o.Internal.Transaction trans
			, object indexEntry)
		{
			return indexEntry;
		}

		public virtual bool IndexNullHandling()
		{
			return false;
		}

		public virtual Db4objects.Db4o.Foundation.TernaryBool IsSecondClass()
		{
			return Db4objects.Db4o.Foundation.TernaryBool.YES;
		}

		public virtual void CalculateLengths(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes header, bool topLevel, 
			object obj, bool withIndirection)
		{
			if (topLevel)
			{
				header.AddBaseLength(LinkLength());
			}
			else
			{
				header.AddPayLoadLength(LinkLength());
			}
		}

		public virtual void PrepareComparison(Db4objects.Db4o.Internal.Transaction a_trans
			, object obj)
		{
			PrepareComparison(obj);
		}

		protected abstract System.Type PrimitiveJavaClass();

		public abstract object PrimitiveNull();

		public virtual bool ReadArray(object array, Db4objects.Db4o.Internal.Buffer reader
			)
		{
			return false;
		}

		public virtual Db4objects.Db4o.Internal.ITypeHandler4 ReadArrayHandler(Db4objects.Db4o.Internal.Transaction
			 a_trans, Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer[]
			 a_bytes)
		{
			return null;
		}

		public virtual object ReadQuery(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, bool withRedirection, Db4objects.Db4o.Internal.Buffer reader, bool toArray)
		{
			return Read1(reader);
		}

		public virtual object Read(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, 
			Db4objects.Db4o.Internal.StatefulBuffer buffer, bool redirect)
		{
			return Read1(buffer);
		}

		internal abstract object Read1(Db4objects.Db4o.Internal.Buffer reader);

		public virtual void ReadCandidates(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer a_bytes, Db4objects.Db4o.Internal.Query.Processor.QCandidates
			 a_candidates)
		{
		}

		public virtual Db4objects.Db4o.Internal.Query.Processor.QCandidate ReadSubCandidate
			(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, Db4objects.Db4o.Internal.Query.Processor.QCandidates candidates, bool withIndirection
			)
		{
			try
			{
				object obj = ReadQuery(candidates.i_trans, mf, withIndirection, reader, true);
				if (obj != null)
				{
					return new Db4objects.Db4o.Internal.Query.Processor.QCandidate(candidates, obj, 0
						, true);
				}
			}
			catch (Db4objects.Db4o.CorruptionException)
			{
			}
			return null;
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer a_reader)
		{
			try
			{
				return Read1(a_reader);
			}
			catch (Db4objects.Db4o.CorruptionException)
			{
			}
			return null;
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.StatefulBuffer a_writer)
		{
			return Read(mf, a_writer, true);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ClassReflector()
		{
			if (_classReflector != null)
			{
				return _classReflector;
			}
			_classReflector = _stream.Reflector().ForClass(DefaultValue().GetType());
			System.Type clazz = PrimitiveJavaClass();
			if (clazz != null)
			{
				_primitiveClassReflector = _stream.Reflector().ForClass(clazz);
			}
			return _classReflector;
		}

		/// <summary>classReflector() has to be called first, before this returns a value</summary>
		public virtual Db4objects.Db4o.Reflect.IReflectClass PrimitiveClassReflector()
		{
			return _primitiveClassReflector;
		}

		public virtual bool SupportsIndex()
		{
			return true;
		}

		public abstract void Write(object a_object, Db4objects.Db4o.Internal.Buffer a_bytes
			);

		public virtual bool WriteArray(object array, Db4objects.Db4o.Internal.Buffer reader
			)
		{
			return false;
		}

		public virtual void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer a_writer, object
			 a_object)
		{
			if (a_object == null)
			{
				a_object = PrimitiveNull();
			}
			Write(a_object, a_writer);
		}

		public virtual object WriteNew(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, object a_object, bool topLevel, Db4objects.Db4o.Internal.StatefulBuffer a_bytes
			, bool withIndirection, bool restoreLinkeOffset)
		{
			if (a_object == null)
			{
				a_object = PrimitiveNull();
			}
			Write(a_object, a_bytes);
			return a_object;
		}

		public virtual Db4objects.Db4o.Internal.IComparable4 PrepareComparison(object obj
			)
		{
			if (obj == null)
			{
				i_compareToIsNull = true;
				return Db4objects.Db4o.Internal.Null.INSTANCE;
			}
			i_compareToIsNull = false;
			PrepareComparison1(obj);
			return this;
		}

		public virtual object Current()
		{
			if (i_compareToIsNull)
			{
				return null;
			}
			return Current1();
		}

		internal abstract void PrepareComparison1(object obj);

		public abstract object Current1();

		public virtual int CompareTo(object obj)
		{
			if (i_compareToIsNull)
			{
				if (obj == null)
				{
					return 0;
				}
				return 1;
			}
			if (obj == null)
			{
				return -1;
			}
			if (IsEqual1(obj))
			{
				return 0;
			}
			if (IsGreater1(obj))
			{
				return 1;
			}
			return -1;
		}

		public virtual bool IsEqual(object obj)
		{
			if (i_compareToIsNull)
			{
				return obj == null;
			}
			return IsEqual1(obj);
		}

		internal abstract bool IsEqual1(object obj);

		public virtual bool IsGreater(object obj)
		{
			if (i_compareToIsNull)
			{
				return obj != null;
			}
			return IsGreater1(obj);
		}

		internal abstract bool IsGreater1(object obj);

		public virtual bool IsSmaller(object obj)
		{
			if (i_compareToIsNull)
			{
				return false;
			}
			return IsSmaller1(obj);
		}

		internal abstract bool IsSmaller1(object obj);

		public abstract int LinkLength();

		public void Defrag(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, Db4objects.Db4o.Internal.ReaderPair
			 readers, bool redirect)
		{
			int linkLength = LinkLength();
			readers.IncrementOffset(linkLength);
		}

		public virtual void DefragIndexEntry(Db4objects.Db4o.Internal.ReaderPair readers)
		{
			try
			{
				Read1(readers.Source());
				Read1(readers.Target());
			}
			catch (Db4objects.Db4o.CorruptionException)
			{
				Db4objects.Db4o.Internal.Exceptions4.VirtualException();
			}
		}

		protected virtual Db4objects.Db4o.Internal.Marshall.PrimitiveMarshaller PrimitiveMarshaller
			()
		{
			return Db4objects.Db4o.Internal.Marshall.MarshallerFamily.Current()._primitive;
		}

		public abstract int GetID();
	}
}
