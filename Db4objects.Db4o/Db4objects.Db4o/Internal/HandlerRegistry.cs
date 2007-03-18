namespace Db4objects.Db4o.Internal
{
	/// <exclude>
	/// TODO: This class was written to make ObjectContainerBase
	/// leaner, so TransportObjectContainer has less members.
	/// All funcionality of this class should become part of
	/// ObjectContainerBase and the functionality in
	/// ObjectContainerBase should delegate to independant
	/// modules without circular references.
	/// </exclude>
	public sealed class HandlerRegistry
	{
		private readonly Db4objects.Db4o.Internal.ObjectContainerBase _masterStream;

		private static readonly Db4objects.Db4o.Internal.IDb4oTypeImpl[] i_db4oTypes = new 
			Db4objects.Db4o.Internal.IDb4oTypeImpl[] { new Db4objects.Db4o.Internal.BlobImpl
			() };

		public const int ANY_ARRAY_ID = 12;

		public const int ANY_ARRAY_N_ID = 13;

		private const int CLASSCOUNT = 11;

		private Db4objects.Db4o.Internal.ClassMetadata i_anyArray;

		private Db4objects.Db4o.Internal.ClassMetadata i_anyArrayN;

		public readonly Db4objects.Db4o.Internal.Handlers.StringHandler i_stringHandler;

		private Db4objects.Db4o.Internal.ITypeHandler4[] i_handlers;

		private int i_maxTypeID = ANY_ARRAY_N_ID + 1;

		private Db4objects.Db4o.Internal.Handlers.NetTypeHandler[] i_platformTypes;

		private const int PRIMITIVECOUNT = 8;

		internal Db4objects.Db4o.Internal.ClassMetadata[] i_yapClasses;

		private const int ANY_INDEX = 10;

		public const int ANY_ID = 11;

		private readonly Db4objects.Db4o.Internal.VirtualFieldMetadata[] _virtualFields = 
			new Db4objects.Db4o.Internal.VirtualFieldMetadata[2];

		private readonly Db4objects.Db4o.Foundation.Hashtable4 i_classByClass = new Db4objects.Db4o.Foundation.Hashtable4
			(32);

		internal Db4objects.Db4o.Types.IDb4oCollections i_collections;

		internal Db4objects.Db4o.Internal.SharedIndexedFields i_indexes;

		internal Db4objects.Db4o.ReplicationImpl i_replication;

		internal Db4objects.Db4o.Internal.Replication.MigrationConnection i_migration;

		internal Db4objects.Db4o.Internal.Replication.IDb4oReplicationReferenceProvider _replicationReferenceProvider;

		public readonly Db4objects.Db4o.Internal.Diagnostic.DiagnosticProcessor _diagnosticProcessor;

		public bool i_encrypt;

		internal byte[] i_encryptor;

		internal int i_lastEncryptorByte;

		internal readonly Db4objects.Db4o.Reflect.Generic.GenericReflector _reflector;

		public Db4objects.Db4o.Reflect.IReflectClass ICLASS_COMPARE;

		internal Db4objects.Db4o.Reflect.IReflectClass ICLASS_DB4OTYPE;

		internal Db4objects.Db4o.Reflect.IReflectClass ICLASS_DB4OTYPEIMPL;

		public Db4objects.Db4o.Reflect.IReflectClass ICLASS_INTERNAL;

		internal Db4objects.Db4o.Reflect.IReflectClass ICLASS_UNVERSIONED;

		public Db4objects.Db4o.Reflect.IReflectClass ICLASS_OBJECT;

		internal Db4objects.Db4o.Reflect.IReflectClass ICLASS_OBJECTCONTAINER;

		public Db4objects.Db4o.Reflect.IReflectClass ICLASS_STATICCLASS;

		public Db4objects.Db4o.Reflect.IReflectClass ICLASS_STRING;

		internal Db4objects.Db4o.Reflect.IReflectClass ICLASS_TRANSIENTCLASS;

		internal HandlerRegistry(Db4objects.Db4o.Internal.ObjectContainerBase a_stream, byte
			 stringEncoding, Db4objects.Db4o.Reflect.Generic.GenericReflector reflector)
		{
			_masterStream = a_stream;
			a_stream.i_handlers = this;
			_reflector = reflector;
			_diagnosticProcessor = a_stream.ConfigImpl().DiagnosticProcessor();
			InitClassReflectors(reflector);
			i_indexes = new Db4objects.Db4o.Internal.SharedIndexedFields(a_stream);
			_virtualFields[0] = i_indexes.i_fieldVersion;
			_virtualFields[1] = i_indexes.i_fieldUUID;
			i_stringHandler = new Db4objects.Db4o.Internal.Handlers.StringHandler(a_stream, Db4objects.Db4o.Internal.LatinStringIO
				.ForEncoding(stringEncoding));
			i_handlers = new Db4objects.Db4o.Internal.ITypeHandler4[] { new Db4objects.Db4o.Internal.Handlers.IntHandler
				(a_stream), new Db4objects.Db4o.Internal.Handlers.LongHandler(a_stream), new Db4objects.Db4o.Internal.Handlers.FloatHandler
				(a_stream), new Db4objects.Db4o.Internal.Handlers.BooleanHandler(a_stream), new 
				Db4objects.Db4o.Internal.Handlers.DoubleHandler(a_stream), new Db4objects.Db4o.Internal.Handlers.ByteHandler
				(a_stream), new Db4objects.Db4o.Internal.Handlers.CharHandler(a_stream), new Db4objects.Db4o.Internal.Handlers.ShortHandler
				(a_stream), i_stringHandler, new Db4objects.Db4o.Internal.Handlers.DateHandler(a_stream
				), new Db4objects.Db4o.Internal.UntypedFieldHandler(a_stream) };
			i_platformTypes = Db4objects.Db4o.Internal.Platform4.Types(a_stream);
			if (i_platformTypes.Length > 0)
			{
				for (int i = 0; i < i_platformTypes.Length; i++)
				{
					i_platformTypes[i].Initialize();
					if (i_platformTypes[i].GetID() > i_maxTypeID)
					{
						i_maxTypeID = i_platformTypes[i].GetID();
					}
				}
				Db4objects.Db4o.Internal.ITypeHandler4[] temp = i_handlers;
				i_handlers = new Db4objects.Db4o.Internal.ITypeHandler4[i_maxTypeID];
				System.Array.Copy(temp, 0, i_handlers, 0, temp.Length);
				for (int i = 0; i < i_platformTypes.Length; i++)
				{
					int idx = i_platformTypes[i].GetID() - 1;
					i_handlers[idx] = i_platformTypes[i];
				}
			}
			i_yapClasses = new Db4objects.Db4o.Internal.ClassMetadata[i_maxTypeID + 1];
			for (int i = 0; i < CLASSCOUNT; i++)
			{
				int id = i + 1;
				i_yapClasses[i] = new Db4objects.Db4o.Internal.PrimitiveFieldHandler(a_stream, i_handlers
					[i]);
				i_yapClasses[i].SetID(id);
				i_classByClass.Put(i_handlers[i].ClassReflector(), i_yapClasses[i]);
				if (i < ANY_INDEX)
				{
					reflector.RegisterPrimitiveClass(id, i_handlers[i].ClassReflector().GetName(), null
						);
				}
			}
			for (int i = 0; i < i_platformTypes.Length; i++)
			{
				int id = i_platformTypes[i].GetID();
				int idx = id - 1;
				Db4objects.Db4o.Reflect.Generic.IGenericConverter converter = (i_platformTypes[i]
					 is Db4objects.Db4o.Reflect.Generic.IGenericConverter) ? (Db4objects.Db4o.Reflect.Generic.IGenericConverter
					)i_platformTypes[i] : null;
				reflector.RegisterPrimitiveClass(id, i_platformTypes[i].GetName(), converter);
				i_handlers[idx] = i_platformTypes[i];
				i_yapClasses[idx] = new Db4objects.Db4o.Internal.PrimitiveFieldHandler(a_stream, 
					i_platformTypes[i]);
				i_yapClasses[idx].SetID(id);
				if (id > i_maxTypeID)
				{
					i_maxTypeID = idx;
				}
				i_classByClass.Put(i_platformTypes[i].ClassReflector(), i_yapClasses[idx]);
			}
			i_anyArray = new Db4objects.Db4o.Internal.PrimitiveFieldHandler(a_stream, new Db4objects.Db4o.Internal.Handlers.ArrayHandler
				(_masterStream, UntypedHandler(), false));
			i_anyArray.SetID(ANY_ARRAY_ID);
			i_yapClasses[ANY_ARRAY_ID - 1] = i_anyArray;
			i_anyArrayN = new Db4objects.Db4o.Internal.PrimitiveFieldHandler(a_stream, new Db4objects.Db4o.Internal.Handlers.MultidimensionalArrayHandler
				(_masterStream, UntypedHandler(), false));
			i_anyArrayN.SetID(ANY_ARRAY_N_ID);
			i_yapClasses[ANY_ARRAY_N_ID - 1] = i_anyArrayN;
		}

		internal int ArrayType(object a_object)
		{
			Db4objects.Db4o.Reflect.IReflectClass claxx = _masterStream.Reflector().ForObject
				(a_object);
			if (!claxx.IsArray())
			{
				return 0;
			}
			if (_masterStream.Reflector().Array().IsNDimensional(claxx))
			{
				return Db4objects.Db4o.Internal.Const4.TYPE_NARRAY;
			}
			return Db4objects.Db4o.Internal.Const4.TYPE_ARRAY;
		}

		internal bool CreateConstructor(Db4objects.Db4o.Reflect.IReflectClass claxx, bool
			 skipConstructor)
		{
			if (claxx == null)
			{
				return false;
			}
			if (claxx.IsAbstract() || claxx.IsInterface())
			{
				return true;
			}
			if (!Db4objects.Db4o.Internal.Platform4.CallConstructor())
			{
				if (claxx.SkipConstructor(skipConstructor))
				{
					return true;
				}
			}
			if (!_masterStream.ConfigImpl().TestConstructors())
			{
				return true;
			}
			if (claxx.NewInstance() != null)
			{
				return true;
			}
			if (_masterStream.Reflector().ConstructorCallsSupported())
			{
				Db4objects.Db4o.Foundation.Tree sortedConstructors = SortConstructorsByParamsCount
					(claxx);
				return FindConstructor(claxx, sortedConstructors);
			}
			return false;
		}

		private bool FindConstructor(Db4objects.Db4o.Reflect.IReflectClass claxx, Db4objects.Db4o.Foundation.Tree
			 sortedConstructors)
		{
			if (sortedConstructors == null)
			{
				return false;
			}
			System.Collections.IEnumerator iter = new Db4objects.Db4o.Foundation.TreeNodeIterator
				(sortedConstructors);
			while (iter.MoveNext())
			{
				object obj = iter.Current;
				Db4objects.Db4o.Reflect.IReflectConstructor constructor = (Db4objects.Db4o.Reflect.IReflectConstructor
					)((Db4objects.Db4o.Internal.TreeIntObject)obj)._object;
				Db4objects.Db4o.Reflect.IReflectClass[] paramTypes = constructor.GetParameterTypes
					();
				object[] @params = new object[paramTypes.Length];
				for (int j = 0; j < @params.Length; j++)
				{
					@params[j] = NullValue(paramTypes[j]);
				}
				object res = constructor.NewInstance(@params);
				if (res != null)
				{
					claxx.UseConstructor(constructor, @params);
					return true;
				}
			}
			return false;
		}

		private object NullValue(Db4objects.Db4o.Reflect.IReflectClass clazz)
		{
			for (int k = 0; k < PRIMITIVECOUNT; k++)
			{
				if (clazz.Equals(i_handlers[k].PrimitiveClassReflector()))
				{
					return ((Db4objects.Db4o.Internal.Handlers.PrimitiveHandler)i_handlers[k]).PrimitiveNull
						();
				}
			}
			return null;
		}

		private Db4objects.Db4o.Foundation.Tree SortConstructorsByParamsCount(Db4objects.Db4o.Reflect.IReflectClass
			 claxx)
		{
			Db4objects.Db4o.Reflect.IReflectConstructor[] constructors = claxx.GetDeclaredConstructors
				();
			Db4objects.Db4o.Foundation.Tree sortedConstructors = null;
			for (int i = 0; i < constructors.Length; i++)
			{
				try
				{
					constructors[i].SetAccessible();
					int parameterCount = constructors[i].GetParameterTypes().Length;
					sortedConstructors = Db4objects.Db4o.Foundation.Tree.Add(sortedConstructors, new 
						Db4objects.Db4o.Internal.TreeIntObject(i + constructors.Length * parameterCount, 
						constructors[i]));
				}
				catch (System.Security.SecurityException e)
				{
				}
			}
			return sortedConstructors;
		}

		public void Decrypt(Db4objects.Db4o.Internal.Buffer reader)
		{
			if (i_encrypt)
			{
				int encryptorOffSet = i_lastEncryptorByte;
				byte[] bytes = reader._buffer;
				for (int i = reader.GetLength() - 1; i >= 0; i--)
				{
					bytes[i] += i_encryptor[encryptorOffSet];
					if (encryptorOffSet == 0)
					{
						encryptorOffSet = i_lastEncryptorByte;
					}
					else
					{
						encryptorOffSet--;
					}
				}
			}
		}

		public void Encrypt(Db4objects.Db4o.Internal.Buffer reader)
		{
			if (i_encrypt)
			{
				byte[] bytes = reader._buffer;
				int encryptorOffSet = i_lastEncryptorByte;
				for (int i = reader.GetLength() - 1; i >= 0; i--)
				{
					bytes[i] -= i_encryptor[encryptorOffSet];
					if (encryptorOffSet == 0)
					{
						encryptorOffSet = i_lastEncryptorByte;
					}
					else
					{
						encryptorOffSet--;
					}
				}
			}
		}

		public void OldEncryptionOff()
		{
			i_encrypt = false;
			i_encryptor = null;
			i_lastEncryptorByte = 0;
			_masterStream.ConfigImpl().OldEncryptionOff();
		}

		internal Db4objects.Db4o.Internal.ITypeHandler4 GetHandler(int a_index)
		{
			return i_handlers[a_index - 1];
		}

		internal Db4objects.Db4o.Internal.ITypeHandler4 HandlerForClass(Db4objects.Db4o.Reflect.IReflectClass
			 a_class, Db4objects.Db4o.Reflect.IReflectClass[] a_Supported)
		{
			for (int i = 0; i < a_Supported.Length; i++)
			{
				if (a_Supported[i].Equals(a_class))
				{
					return i_handlers[i];
				}
			}
			return null;
		}

		public Db4objects.Db4o.Internal.ITypeHandler4 HandlerForClass(Db4objects.Db4o.Internal.ObjectContainerBase
			 a_stream, Db4objects.Db4o.Reflect.IReflectClass a_class)
		{
			if (a_class == null)
			{
				return null;
			}
			if (a_class.IsArray())
			{
				return HandlerForClass(a_stream, a_class.GetComponentType());
			}
			Db4objects.Db4o.Internal.ClassMetadata yc = GetYapClassStatic(a_class);
			if (yc != null)
			{
				return ((Db4objects.Db4o.Internal.PrimitiveFieldHandler)yc).i_handler;
			}
			return a_stream.ProduceClassMetadata(a_class);
		}

		public Db4objects.Db4o.Internal.ITypeHandler4 UntypedHandler()
		{
			return i_handlers[ANY_INDEX];
		}

		private void InitClassReflectors(Db4objects.Db4o.Reflect.Generic.GenericReflector
			 reflector)
		{
			ICLASS_COMPARE = reflector.ForClass(Db4objects.Db4o.Internal.Const4.CLASS_COMPARE
				);
			ICLASS_DB4OTYPE = reflector.ForClass(Db4objects.Db4o.Internal.Const4.CLASS_DB4OTYPE
				);
			ICLASS_DB4OTYPEIMPL = reflector.ForClass(Db4objects.Db4o.Internal.Const4.CLASS_DB4OTYPEIMPL
				);
			ICLASS_INTERNAL = reflector.ForClass(Db4objects.Db4o.Internal.Const4.CLASS_INTERNAL
				);
			ICLASS_UNVERSIONED = reflector.ForClass(Db4objects.Db4o.Internal.Const4.CLASS_UNVERSIONED
				);
			ICLASS_OBJECT = reflector.ForClass(Db4objects.Db4o.Internal.Const4.CLASS_OBJECT);
			ICLASS_OBJECTCONTAINER = reflector.ForClass(Db4objects.Db4o.Internal.Const4.CLASS_OBJECTCONTAINER
				);
			ICLASS_STATICCLASS = reflector.ForClass(Db4objects.Db4o.Internal.Const4.CLASS_STATICCLASS
				);
			ICLASS_STRING = reflector.ForClass(typeof(string));
			ICLASS_TRANSIENTCLASS = reflector.ForClass(Db4objects.Db4o.Internal.Const4.CLASS_TRANSIENTCLASS
				);
			Db4objects.Db4o.Internal.Platform4.RegisterCollections(reflector);
		}

		internal void InitEncryption(Db4objects.Db4o.Internal.Config4Impl a_config)
		{
			if (a_config.Encrypt() && a_config.Password() != null && a_config.Password().Length
				 > 0)
			{
				i_encrypt = true;
				i_encryptor = new byte[a_config.Password().Length];
				for (int i = 0; i < i_encryptor.Length; i++)
				{
					i_encryptor[i] = (byte)(Sharpen.Runtime.GetCharAt(a_config.Password(), i) & unchecked(
						(int)(0xff)));
				}
				i_lastEncryptorByte = a_config.Password().Length - 1;
				return;
			}
			OldEncryptionOff();
		}

		internal static Db4objects.Db4o.Internal.IDb4oTypeImpl GetDb4oType(Db4objects.Db4o.Reflect.IReflectClass
			 clazz)
		{
			for (int i = 0; i < i_db4oTypes.Length; i++)
			{
				if (clazz.IsInstance(i_db4oTypes[i]))
				{
					return i_db4oTypes[i];
				}
			}
			return null;
		}

		public Db4objects.Db4o.Internal.ClassMetadata GetYapClassStatic(int a_id)
		{
			if (a_id > 0 && a_id <= i_maxTypeID)
			{
				return i_yapClasses[a_id - 1];
			}
			return null;
		}

		internal Db4objects.Db4o.Internal.ClassMetadata GetYapClassStatic(Db4objects.Db4o.Reflect.IReflectClass
			 a_class)
		{
			if (a_class == null)
			{
				return null;
			}
			if (a_class.IsArray())
			{
				if (_masterStream.Reflector().Array().IsNDimensional(a_class))
				{
					return i_anyArrayN;
				}
				return i_anyArray;
			}
			return (Db4objects.Db4o.Internal.ClassMetadata)i_classByClass.Get(a_class);
		}

		public bool IsSecondClass(object a_object)
		{
			if (a_object != null)
			{
				Db4objects.Db4o.Reflect.IReflectClass claxx = _masterStream.Reflector().ForObject
					(a_object);
				if (i_classByClass.Get(claxx) != null)
				{
					return true;
				}
				return Db4objects.Db4o.Internal.Platform4.IsValueType(claxx);
			}
			return false;
		}

		public bool IsSystemHandler(int id)
		{
			return id <= i_maxTypeID;
		}

		public void MigrationConnection(Db4objects.Db4o.Internal.Replication.MigrationConnection
			 mgc)
		{
			i_migration = mgc;
		}

		public Db4objects.Db4o.Internal.Replication.MigrationConnection MigrationConnection
			()
		{
			return i_migration;
		}

		public void Replication(Db4objects.Db4o.ReplicationImpl impl)
		{
			i_replication = impl;
		}

		public Db4objects.Db4o.ReplicationImpl Replication()
		{
			return i_replication;
		}

		public Db4objects.Db4o.Internal.ClassMetadata PrimitiveClassById(int id)
		{
			return i_yapClasses[id - 1];
		}

		public Db4objects.Db4o.Internal.VirtualFieldMetadata VirtualFieldByName(string name
			)
		{
			for (int i = 0; i < _virtualFields.Length; i++)
			{
				if (name.Equals(_virtualFields[i].GetName()))
				{
					return _virtualFields[i];
				}
			}
			return null;
		}
	}
}
