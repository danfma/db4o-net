namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class FieldMetadata : Db4objects.Db4o.Ext.IStoredField
	{
		private Db4objects.Db4o.Internal.ClassMetadata i_yapClass;

		private int i_arrayPosition;

		private string i_name;

		private bool i_isArray;

		private bool i_isNArray;

		private bool i_isPrimitive;

		private Db4objects.Db4o.Reflect.IReflectField i_javaField;

		internal Db4objects.Db4o.Internal.ITypeHandler4 i_handler;

		private int i_handlerID;

		private int i_state;

		private const int NOT_LOADED = 0;

		private const int UNAVAILABLE = -1;

		private const int AVAILABLE = 1;

		private Db4objects.Db4o.Internal.Config4Field i_config;

		private Db4objects.Db4o.Internal.IDb4oTypeImpl i_db4oType;

		private Db4objects.Db4o.Internal.Btree.BTree _index;

		internal static readonly Db4objects.Db4o.Internal.FieldMetadata[] EMPTY_ARRAY = new 
			Db4objects.Db4o.Internal.FieldMetadata[0];

		public FieldMetadata(Db4objects.Db4o.Internal.ClassMetadata a_yapClass)
		{
			i_yapClass = a_yapClass;
		}

		internal FieldMetadata(Db4objects.Db4o.Internal.ClassMetadata a_yapClass, Db4objects.Db4o.Config.IObjectTranslator
			 a_translator) : this(a_yapClass)
		{
			Init(a_yapClass, a_translator.GetType().FullName);
			i_state = AVAILABLE;
			Db4objects.Db4o.Internal.ObjectContainerBase stream = GetStream();
			i_handler = stream.i_handlers.HandlerForClass(stream, stream.Reflector().ForClass
				(a_translator.StoredClass()));
		}

		internal FieldMetadata(Db4objects.Db4o.Internal.ClassMetadata containingClass, Db4objects.Db4o.Config.IObjectMarshaller
			 marshaller) : this(containingClass)
		{
			Init(containingClass, marshaller.GetType().FullName);
			i_state = AVAILABLE;
			i_handler = GetStream().i_handlers.UntypedHandler();
		}

		internal FieldMetadata(Db4objects.Db4o.Internal.ClassMetadata a_yapClass, Db4objects.Db4o.Reflect.IReflectField
			 a_field, Db4objects.Db4o.Internal.ITypeHandler4 a_handler) : this(a_yapClass)
		{
			Init(a_yapClass, a_field.GetName());
			i_javaField = a_field;
			i_javaField.SetAccessible();
			i_handler = a_handler;
			bool isPrimitive = false;
			if (a_field is Db4objects.Db4o.Reflect.Generic.GenericField)
			{
				isPrimitive = ((Db4objects.Db4o.Reflect.Generic.GenericField)a_field).IsPrimitive
					();
			}
			Configure(a_field.GetFieldType(), isPrimitive);
			CheckDb4oType();
			i_state = AVAILABLE;
		}

		public virtual void AddFieldIndex(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.StatefulBuffer
			 writer, Db4objects.Db4o.Internal.Slots.Slot oldSlot)
		{
			if (!HasIndex())
			{
				writer.IncrementOffset(LinkLength());
				return;
			}
			try
			{
				AddIndexEntry(writer, ReadIndexEntry(mf, writer));
			}
			catch (Db4objects.Db4o.CorruptionException exc)
			{
				throw new Db4objects.Db4o.Internal.FieldIndexException(exc, this);
			}
			catch (System.IO.IOException exc)
			{
				throw new Db4objects.Db4o.Internal.FieldIndexException(exc, this);
			}
		}

		protected virtual void AddIndexEntry(Db4objects.Db4o.Internal.StatefulBuffer a_bytes
			, object indexEntry)
		{
			AddIndexEntry(a_bytes.GetTransaction(), a_bytes.GetID(), indexEntry);
		}

		public virtual void AddIndexEntry(Db4objects.Db4o.Internal.Transaction trans, int
			 parentID, object indexEntry)
		{
			if (!HasIndex())
			{
				return;
			}
			Db4objects.Db4o.Internal.Btree.BTree index = GetIndex(trans);
			if (index == null)
			{
				return;
			}
			index.Add(trans, CreateFieldIndexKey(parentID, indexEntry));
		}

		private Db4objects.Db4o.Internal.Btree.FieldIndexKey CreateFieldIndexKey(int parentID
			, object indexEntry)
		{
			object convertedIndexEntry = IndexEntryFor(indexEntry);
			return new Db4objects.Db4o.Internal.Btree.FieldIndexKey(parentID, convertedIndexEntry
				);
		}

		protected virtual object IndexEntryFor(object indexEntry)
		{
			return i_javaField.IndexEntry(indexEntry);
		}

		public virtual bool CanUseNullBitmap()
		{
			return true;
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.StatefulBuffer writer)
		{
			return i_handler.ReadIndexEntry(mf, writer);
		}

		public virtual void RemoveIndexEntry(Db4objects.Db4o.Internal.Transaction trans, 
			int parentID, object indexEntry)
		{
			if (!HasIndex())
			{
				return;
			}
			if (_index == null)
			{
				return;
			}
			_index.Remove(trans, CreateFieldIndexKey(parentID, indexEntry));
		}

		public virtual bool Alive()
		{
			if (i_state == AVAILABLE)
			{
				return true;
			}
			if (i_state == NOT_LOADED)
			{
				if (i_handler == null)
				{
					i_handler = LoadJavaField1();
					if (i_handler != null)
					{
						if (i_handlerID == 0)
						{
							i_handlerID = i_handler.GetID();
						}
						else
						{
							if (i_handler.GetID() != i_handlerID)
							{
								i_handler = null;
							}
						}
					}
				}
				LoadJavaField();
				if (i_handler == null || i_javaField == null)
				{
					i_state = UNAVAILABLE;
					i_javaField = null;
				}
				else
				{
					i_handler = WrapHandlerToArrays(GetStream(), i_handler);
					i_state = AVAILABLE;
					CheckDb4oType();
				}
			}
			return i_state == AVAILABLE;
		}

		internal virtual bool CanAddToQuery(string fieldName)
		{
			if (!Alive())
			{
				return false;
			}
			return fieldName.Equals(GetName()) && GetParentYapClass() != null && !GetParentYapClass
				().IsInternal();
		}

		public virtual bool CanHold(Db4objects.Db4o.Reflect.IReflectClass claxx)
		{
			if (claxx == null)
			{
				return !i_isPrimitive;
			}
			return i_handler.CanHold(claxx);
		}

		public virtual object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object 
			obj)
		{
			if (claxx == null || obj == null)
			{
				return i_isPrimitive ? Db4objects.Db4o.Foundation.No4.INSTANCE : obj;
			}
			return i_handler.Coerce(claxx, obj);
		}

		public bool CanLoadByIndex()
		{
			if (i_handler is Db4objects.Db4o.Internal.ClassMetadata)
			{
				Db4objects.Db4o.Internal.ClassMetadata yc = (Db4objects.Db4o.Internal.ClassMetadata
					)i_handler;
				if (yc.IsArray())
				{
					return false;
				}
			}
			return true;
		}

		internal virtual void CascadeActivation(Db4objects.Db4o.Internal.Transaction a_trans
			, object a_object, int a_depth, bool a_activate)
		{
			if (Alive())
			{
				try
				{
					object cascadeTo = GetOrCreate(a_trans, a_object);
					if (cascadeTo != null && i_handler != null)
					{
						i_handler.CascadeActivation(a_trans, cascadeTo, a_depth, a_activate);
					}
				}
				catch (System.Exception)
				{
				}
			}
		}

		private void CheckDb4oType()
		{
			if (i_javaField != null)
			{
				if (GetStream().i_handlers.ICLASS_DB4OTYPE.IsAssignableFrom(i_javaField.GetFieldType
					()))
				{
					i_db4oType = Db4objects.Db4o.Internal.HandlerRegistry.GetDb4oType(i_javaField.GetFieldType
						());
				}
			}
		}

		internal virtual void CollectConstraints(Db4objects.Db4o.Internal.Transaction a_trans
			, Db4objects.Db4o.Internal.Query.Processor.QConObject a_parent, object a_template
			, Db4objects.Db4o.Foundation.IVisitor4 a_visitor)
		{
			object obj = GetOn(a_trans, a_template);
			if (obj != null)
			{
				Db4objects.Db4o.Foundation.Collection4 objs = Db4objects.Db4o.Internal.Platform4.
					FlattenCollection(a_trans.Stream(), obj);
				System.Collections.IEnumerator j = objs.GetEnumerator();
				while (j.MoveNext())
				{
					obj = j.Current;
					if (obj != null)
					{
						if (i_isPrimitive)
						{
							if (i_handler is Db4objects.Db4o.Internal.Handlers.PrimitiveHandler)
							{
								if (obj.Equals(((Db4objects.Db4o.Internal.Handlers.PrimitiveHandler)i_handler).PrimitiveNull
									()))
								{
									return;
								}
							}
						}
						if (Db4objects.Db4o.Internal.Platform4.IgnoreAsConstraint(obj))
						{
							return;
						}
						if (!a_parent.HasObjectInParentPath(obj))
						{
							a_visitor.Visit(new Db4objects.Db4o.Internal.Query.Processor.QConObject(a_trans, 
								a_parent, QField(a_trans), obj));
						}
					}
				}
			}
		}

		public Db4objects.Db4o.Internal.TreeInt CollectIDs(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.TreeInt tree, Db4objects.Db4o.Internal.StatefulBuffer
			 a_bytes)
		{
			if (Alive())
			{
				if (i_handler is Db4objects.Db4o.Internal.ClassMetadata)
				{
					tree = (Db4objects.Db4o.Internal.TreeInt)Db4objects.Db4o.Foundation.Tree.Add(tree
						, new Db4objects.Db4o.Internal.TreeInt(a_bytes.ReadInt()));
				}
				else
				{
					if (i_handler is Db4objects.Db4o.Internal.Handlers.ArrayHandler)
					{
						tree = ((Db4objects.Db4o.Internal.Handlers.ArrayHandler)i_handler).CollectIDs(mf, 
							tree, a_bytes);
					}
				}
			}
			return tree;
		}

		internal virtual void Configure(Db4objects.Db4o.Reflect.IReflectClass a_class, bool
			 isPrimitive)
		{
			i_isPrimitive = isPrimitive | a_class.IsPrimitive();
			i_isArray = a_class.IsArray();
			if (i_isArray)
			{
				Db4objects.Db4o.Reflect.IReflectArray reflectArray = GetStream().Reflector().Array
					();
				i_isNArray = reflectArray.IsNDimensional(a_class);
				a_class = reflectArray.GetComponentType(a_class);
				if (i_isNArray)
				{
					i_handler = new Db4objects.Db4o.Internal.Handlers.MultidimensionalArrayHandler(GetStream
						(), i_handler, i_isPrimitive);
				}
				else
				{
					i_handler = new Db4objects.Db4o.Internal.Handlers.ArrayHandler(GetStream(), i_handler
						, i_isPrimitive);
				}
			}
		}

		internal virtual void Deactivate(Db4objects.Db4o.Internal.Transaction a_trans, object
			 a_onObject, int a_depth)
		{
			if (!Alive())
			{
				return;
			}
			bool isEnumClass = i_yapClass.IsEnum();
			if (i_isPrimitive && !i_isArray)
			{
				if (!isEnumClass)
				{
					i_javaField.Set(a_onObject, ((Db4objects.Db4o.Internal.Handlers.PrimitiveHandler)
						i_handler).PrimitiveNull());
				}
				return;
			}
			if (a_depth > 0)
			{
				CascadeActivation(a_trans, a_onObject, a_depth, false);
			}
			if (!isEnumClass)
			{
				i_javaField.Set(a_onObject, null);
			}
		}

		public virtual void Delete(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, 
			Db4objects.Db4o.Internal.StatefulBuffer a_bytes, bool isUpdate)
		{
			if (!Alive())
			{
				IncrementOffset(a_bytes);
				return;
			}
			try
			{
				RemoveIndexEntry(mf, a_bytes);
			}
			catch (Db4objects.Db4o.CorruptionException exc)
			{
				throw new Db4objects.Db4o.Internal.FieldIndexException(exc, this);
			}
			catch (System.IO.IOException exc)
			{
				throw new Db4objects.Db4o.Internal.FieldIndexException(exc, this);
			}
			bool dotnetValueType = false;
			dotnetValueType = Db4objects.Db4o.Internal.Platform4.IsValueType(i_handler.ClassReflector
				());
			if ((i_config != null && i_config.CascadeOnDelete().DefiniteYes()) || dotnetValueType
				)
			{
				int preserveCascade = a_bytes.CascadeDeletes();
				a_bytes.SetCascadeDeletes(1);
				i_handler.DeleteEmbedded(mf, a_bytes);
				a_bytes.SetCascadeDeletes(preserveCascade);
			}
			else
			{
				if (i_config != null && i_config.CascadeOnDelete().DefiniteNo())
				{
					int preserveCascade = a_bytes.CascadeDeletes();
					a_bytes.SetCascadeDeletes(0);
					i_handler.DeleteEmbedded(mf, a_bytes);
					a_bytes.SetCascadeDeletes(preserveCascade);
				}
				else
				{
					i_handler.DeleteEmbedded(mf, a_bytes);
				}
			}
		}

		private void RemoveIndexEntry(Db4objects.Db4o.Internal.Marshall.MarshallerFamily 
			mf, Db4objects.Db4o.Internal.StatefulBuffer a_bytes)
		{
			if (!HasIndex())
			{
				return;
			}
			int offset = a_bytes._offset;
			object obj = i_handler.ReadIndexEntry(mf, a_bytes);
			RemoveIndexEntry(a_bytes.GetTransaction(), a_bytes.GetID(), obj);
			a_bytes._offset = offset;
		}

		public override bool Equals(object obj)
		{
			if (obj is Db4objects.Db4o.Internal.FieldMetadata)
			{
				Db4objects.Db4o.Internal.FieldMetadata yapField = (Db4objects.Db4o.Internal.FieldMetadata
					)obj;
				yapField.Alive();
				Alive();
				return yapField.i_isPrimitive == i_isPrimitive && yapField.i_handler.Equals(i_handler
					) && yapField.i_name.Equals(i_name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return i_name.GetHashCode();
		}

		public virtual object Get(object a_onObject)
		{
			if (i_yapClass != null)
			{
				Db4objects.Db4o.Internal.ObjectContainerBase stream = i_yapClass.GetStream();
				if (stream != null)
				{
					lock (stream.i_lock)
					{
						stream.CheckClosed();
						Db4objects.Db4o.Internal.ObjectReference yo = stream.ReferenceForObject(a_onObject
							);
						if (yo != null)
						{
							int id = yo.GetID();
							if (id > 0)
							{
								Db4objects.Db4o.Internal.StatefulBuffer writer = stream.ReadWriterByID(stream.GetTransaction
									(), id);
								if (writer != null)
								{
									writer._offset = 0;
									Db4objects.Db4o.Internal.Marshall.ObjectHeader oh = new Db4objects.Db4o.Internal.Marshall.ObjectHeader
										(stream, i_yapClass, writer);
									if (oh.ObjectMarshaller().FindOffset(i_yapClass, oh._headerAttributes, writer, this
										))
									{
										try
										{
											return Read(oh._marshallerFamily, writer);
										}
										catch (Db4objects.Db4o.CorruptionException e)
										{
										}
										catch (System.IO.IOException exc)
										{
										}
									}
								}
							}
						}
					}
				}
			}
			return null;
		}

		public virtual string GetName()
		{
			return i_name;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata GetFieldYapClass(Db4objects.Db4o.Internal.ObjectContainerBase
			 a_stream)
		{
			return i_handler.GetClassMetadata(a_stream);
		}

		public virtual Db4objects.Db4o.Internal.ITypeHandler4 GetHandler()
		{
			return i_handler;
		}

		public virtual int GetHandlerID()
		{
			return i_handlerID;
		}

		public virtual object GetOn(Db4objects.Db4o.Internal.Transaction a_trans, object 
			a_OnObject)
		{
			if (Alive())
			{
				return i_javaField.Get(a_OnObject);
			}
			return null;
		}

		/// <summary>
		/// dirty hack for com.db4o.types some of them need to be set automatically
		/// TODO: Derive from YapField for Db4oTypes
		/// </summary>
		public virtual object GetOrCreate(Db4objects.Db4o.Internal.Transaction trans, object
			 onObject)
		{
			if (!Alive())
			{
				return null;
			}
			object obj = i_javaField.Get(onObject);
			if (i_db4oType != null && obj == null)
			{
				obj = i_db4oType.CreateDefault(trans);
				i_javaField.Set(onObject, obj);
			}
			return obj;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata GetParentYapClass()
		{
			return i_yapClass;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass GetStoredType()
		{
			if (i_handler == null)
			{
				return null;
			}
			return i_handler.ClassReflector();
		}

		public virtual Db4objects.Db4o.Internal.ObjectContainerBase GetStream()
		{
			if (i_yapClass == null)
			{
				return null;
			}
			return i_yapClass.GetStream();
		}

		public virtual bool HasConfig()
		{
			return i_config != null;
		}

		public virtual bool HasIndex()
		{
			return _index != null;
		}

		public void IncrementOffset(Db4objects.Db4o.Internal.Buffer buffer)
		{
			buffer.IncrementOffset(LinkLength());
		}

		public void Init(Db4objects.Db4o.Internal.ClassMetadata a_yapClass, string a_name
			)
		{
			i_yapClass = a_yapClass;
			i_name = a_name;
			InitIndex(a_yapClass, a_name);
		}

		internal void InitIndex(Db4objects.Db4o.Internal.ClassMetadata a_yapClass, string
			 a_name)
		{
			if (a_yapClass.i_config != null)
			{
				i_config = a_yapClass.i_config.ConfigField(a_name);
			}
		}

		public virtual void Init(int handlerID, bool isPrimitive, bool isArray, bool isNArray
			)
		{
			i_handlerID = handlerID;
			i_isPrimitive = isPrimitive;
			i_isArray = isArray;
			i_isNArray = isNArray;
		}

		private bool _initialized = false;

		internal void InitConfigOnUp(Db4objects.Db4o.Internal.Transaction trans)
		{
			if (i_config != null && !_initialized)
			{
				_initialized = true;
				i_config.InitOnUp(trans, this);
			}
		}

		public virtual void Instantiate(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.ObjectReference @ref, object onObject, Db4objects.Db4o.Internal.StatefulBuffer
			 buffer)
		{
			if (!Alive())
			{
				IncrementOffset(buffer);
				return;
			}
			object toSet = Read(mf, buffer);
			if (i_db4oType != null)
			{
				if (toSet != null)
				{
					((Db4objects.Db4o.Internal.IDb4oTypeImpl)toSet).SetTrans(buffer.GetTransaction());
				}
			}
			Set(onObject, toSet);
		}

		public virtual bool IsArray()
		{
			return i_isArray;
		}

		public virtual int LinkLength()
		{
			Alive();
			if (i_handler == null)
			{
				return Db4objects.Db4o.Internal.Const4.ID_LENGTH;
			}
			return i_handler.LinkLength();
		}

		public virtual void CalculateLengths(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes header, object obj)
		{
			Alive();
			if (i_handler == null)
			{
				header.AddBaseLength(Db4objects.Db4o.Internal.Const4.ID_LENGTH);
				return;
			}
			i_handler.CalculateLengths(trans, header, true, obj, true);
		}

		public virtual void LoadHandler(Db4objects.Db4o.Internal.ObjectContainerBase a_stream
			)
		{
			i_handler = a_stream.HandlerByID(i_handlerID);
		}

		private void LoadJavaField()
		{
			Db4objects.Db4o.Internal.ITypeHandler4 handler = LoadJavaField1();
			if (handler == null || (!handler.Equals(i_handler)))
			{
				i_javaField = null;
				i_state = UNAVAILABLE;
			}
		}

		private Db4objects.Db4o.Internal.ITypeHandler4 LoadJavaField1()
		{
			try
			{
				Db4objects.Db4o.Reflect.IReflectClass claxx = i_yapClass.ClassReflector();
				if (claxx == null)
				{
					return null;
				}
				i_javaField = claxx.GetDeclaredField(i_name);
				if (i_javaField == null)
				{
					return null;
				}
				i_javaField.SetAccessible();
				Db4objects.Db4o.Internal.ObjectContainerBase stream = i_yapClass.GetStream();
				stream.ShowInternalClasses(true);
				try
				{
					return stream.i_handlers.HandlerForClass(stream, i_javaField.GetFieldType());
				}
				finally
				{
					stream.ShowInternalClasses(false);
				}
			}
			catch (System.Exception e)
			{
			}
			return null;
		}

		public virtual void Marshall(Db4objects.Db4o.Internal.ObjectReference yo, object 
			obj, Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, Db4objects.Db4o.Internal.StatefulBuffer
			 writer, Db4objects.Db4o.Internal.Config4Class config, bool isNew)
		{
			object indexEntry = null;
			if (obj != null && ((config != null && (config.CascadeOnUpdate().DefiniteYes())) 
				|| (i_config != null && (i_config.CascadeOnUpdate().DefiniteYes()))))
			{
				int min = 1;
				if (i_yapClass.IsCollection(obj))
				{
					Db4objects.Db4o.Reflect.Generic.GenericReflector reflector = i_yapClass.Reflector
						();
					min = reflector.CollectionUpdateDepth(reflector.ForObject(obj));
				}
				int updateDepth = writer.GetUpdateDepth();
				if (updateDepth < min)
				{
					writer.SetUpdateDepth(min);
				}
				indexEntry = i_handler.WriteNew(mf, obj, true, writer, true, true);
				writer.SetUpdateDepth(updateDepth);
			}
			else
			{
				indexEntry = i_handler.WriteNew(mf, obj, true, writer, true, true);
			}
			AddIndexEntry(writer, indexEntry);
		}

		public virtual bool NeedsArrayAndPrimitiveInfo()
		{
			return true;
		}

		public virtual bool NeedsHandlerId()
		{
			return true;
		}

		public virtual Db4objects.Db4o.Internal.IComparable4 PrepareComparison(object obj
			)
		{
			if (Alive())
			{
				i_handler.PrepareComparison(obj);
				return i_handler;
			}
			return null;
		}

		public virtual Db4objects.Db4o.Internal.Query.Processor.QField QField(Db4objects.Db4o.Internal.Transaction
			 a_trans)
		{
			int yapClassID = 0;
			if (i_yapClass != null)
			{
				yapClassID = i_yapClass.GetID();
			}
			return new Db4objects.Db4o.Internal.Query.Processor.QField(a_trans, i_name, this, 
				yapClassID, i_arrayPosition);
		}

		internal virtual object Read(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf
			, Db4objects.Db4o.Internal.StatefulBuffer a_bytes)
		{
			if (!Alive())
			{
				IncrementOffset(a_bytes);
				return null;
			}
			return i_handler.Read(mf, a_bytes, true);
		}

		public virtual object ReadQuery(Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer a_reader)
		{
			return i_handler.ReadQuery(a_trans, mf, true, a_reader, false);
		}

		public virtual void ReadVirtualAttribute(Db4objects.Db4o.Internal.Transaction a_trans
			, Db4objects.Db4o.Internal.Buffer a_reader, Db4objects.Db4o.Internal.ObjectReference
			 a_yapObject)
		{
			a_reader.IncrementOffset(i_handler.LinkLength());
		}

		internal virtual void Refresh()
		{
			Db4objects.Db4o.Internal.ITypeHandler4 handler = LoadJavaField1();
			if (handler != null)
			{
				handler = WrapHandlerToArrays(GetStream(), handler);
				if (handler.Equals(i_handler))
				{
					return;
				}
			}
			i_javaField = null;
			i_state = UNAVAILABLE;
		}

		public virtual void Rename(string newName)
		{
			Db4objects.Db4o.Internal.ObjectContainerBase stream = i_yapClass.GetStream();
			if (!stream.IsClient())
			{
				i_name = newName;
				i_yapClass.SetStateDirty();
				i_yapClass.Write(stream.SystemTransaction());
			}
			else
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(58);
			}
		}

		public virtual void SetArrayPosition(int a_index)
		{
			i_arrayPosition = a_index;
		}

		public virtual void Set(object onObject, object obj)
		{
			if (null == i_javaField)
			{
				return;
			}
			i_javaField.Set(onObject, obj);
		}

		internal virtual void SetName(string a_name)
		{
			i_name = a_name;
		}

		internal virtual bool SupportsIndex()
		{
			return Alive() && i_handler.SupportsIndex();
		}

		public void TraverseValues(Db4objects.Db4o.Foundation.IVisitor4 userVisitor)
		{
			if (!Alive())
			{
				return;
			}
			AssertHasIndex();
			Db4objects.Db4o.Internal.ObjectContainerBase stream = i_yapClass.GetStream();
			if (stream.IsClient())
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages
					.CLIENT_SERVER_UNSUPPORTED);
			}
			lock (stream.Lock())
			{
				Db4objects.Db4o.Internal.Transaction trans = stream.GetTransaction();
				_index.TraverseKeys(trans, new _AnonymousInnerClass788(this, userVisitor, trans));
			}
		}

		private sealed class _AnonymousInnerClass788 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass788(FieldMetadata _enclosing, Db4objects.Db4o.Foundation.IVisitor4
				 userVisitor, Db4objects.Db4o.Internal.Transaction trans)
			{
				this._enclosing = _enclosing;
				this.userVisitor = userVisitor;
				this.trans = trans;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Btree.FieldIndexKey key = (Db4objects.Db4o.Internal.Btree.FieldIndexKey
					)obj;
				userVisitor.Visit(this._enclosing.i_handler.IndexEntryToObject(trans, key.Value()
					));
			}

			private readonly FieldMetadata _enclosing;

			private readonly Db4objects.Db4o.Foundation.IVisitor4 userVisitor;

			private readonly Db4objects.Db4o.Internal.Transaction trans;
		}

		private void AssertHasIndex()
		{
			if (!HasIndex())
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages
					.ONLY_FOR_INDEXED_FIELDS);
			}
		}

		private Db4objects.Db4o.Internal.ITypeHandler4 WrapHandlerToArrays(Db4objects.Db4o.Internal.ObjectContainerBase
			 a_stream, Db4objects.Db4o.Internal.ITypeHandler4 a_handler)
		{
			if (i_isNArray)
			{
				a_handler = new Db4objects.Db4o.Internal.Handlers.MultidimensionalArrayHandler(a_stream
					, a_handler, i_isPrimitive);
			}
			else
			{
				if (i_isArray)
				{
					a_handler = new Db4objects.Db4o.Internal.Handlers.ArrayHandler(a_stream, a_handler
						, i_isPrimitive);
				}
			}
			return a_handler;
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (i_yapClass != null)
			{
				sb.Append(i_yapClass.GetName());
				sb.Append(".");
				sb.Append(GetName());
			}
			return sb.ToString();
		}

		public string ToString(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, Db4objects.Db4o.Internal.StatefulBuffer
			 writer)
		{
			string str = "\n Field " + i_name;
			if (!Alive())
			{
				IncrementOffset(writer);
			}
			else
			{
				object obj = null;
				try
				{
					obj = Read(mf, writer);
				}
				catch (System.Exception)
				{
				}
				if (obj == null)
				{
					str += "\n [null]";
				}
				else
				{
					str += "\n  " + obj.ToString();
				}
			}
			return str;
		}

		public virtual void InitIndex(Db4objects.Db4o.Internal.Transaction systemTrans)
		{
			InitIndex(systemTrans, 0);
		}

		public virtual void InitIndex(Db4objects.Db4o.Internal.Transaction systemTrans, int
			 id)
		{
			if (_index != null)
			{
				throw new System.InvalidOperationException();
			}
			if (systemTrans.Stream().IsClient())
			{
				return;
			}
			_index = NewBTree(systemTrans, id);
		}

		protected Db4objects.Db4o.Internal.Btree.BTree NewBTree(Db4objects.Db4o.Internal.Transaction
			 systemTrans, int id)
		{
			Db4objects.Db4o.Internal.ObjectContainerBase stream = systemTrans.Stream();
			Db4objects.Db4o.Internal.IX.IIndexable4 indexHandler = IndexHandler(stream);
			if (indexHandler == null)
			{
				return null;
			}
			return new Db4objects.Db4o.Internal.Btree.BTree(systemTrans, id, new Db4objects.Db4o.Internal.Btree.FieldIndexKeyHandler
				(stream, indexHandler));
		}

		protected virtual Db4objects.Db4o.Internal.IX.IIndexable4 IndexHandler(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream)
		{
			Db4objects.Db4o.Reflect.IReflectClass indexType = null;
			if (i_javaField != null)
			{
				indexType = i_javaField.IndexType();
			}
			Db4objects.Db4o.Internal.IX.IIndexable4 indexHandler = stream.i_handlers.HandlerForClass
				(stream, indexType);
			return indexHandler;
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTree GetIndex(Db4objects.Db4o.Internal.Transaction
			 transaction)
		{
			return _index;
		}

		public virtual bool IsVirtual()
		{
			return false;
		}

		public virtual bool IsPrimitive()
		{
			return i_isPrimitive;
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange Search(Db4objects.Db4o.Internal.Transaction
			 transaction, object value)
		{
			AssertHasIndex();
			Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult lowerBound = SearchLowerBound
				(transaction, value);
			Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult upperBound = SearchUpperBound
				(transaction, value);
			return lowerBound.CreateIncludingRange(upperBound);
		}

		private Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult SearchUpperBound(Db4objects.Db4o.Internal.Transaction
			 transaction, object value)
		{
			return SearchBound(transaction, int.MaxValue, value);
		}

		private Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult SearchLowerBound(Db4objects.Db4o.Internal.Transaction
			 transaction, object value)
		{
			return SearchBound(transaction, 0, value);
		}

		private Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult SearchBound(Db4objects.Db4o.Internal.Transaction
			 transaction, int parentID, object keyPart)
		{
			return GetIndex(transaction).SearchLeaf(transaction, CreateFieldIndexKey(parentID
				, keyPart), Db4objects.Db4o.Internal.Btree.SearchTarget.LOWEST);
		}

		public virtual bool RebuildIndexForClass(Db4objects.Db4o.Internal.LocalObjectContainer
			 stream, Db4objects.Db4o.Internal.ClassMetadata yapClass)
		{
			long[] ids = yapClass.GetIDs();
			for (int i = 0; i < ids.Length; i++)
			{
				RebuildIndexForObject(stream, yapClass, (int)ids[i]);
			}
			return ids.Length > 0;
		}

		protected virtual void RebuildIndexForObject(Db4objects.Db4o.Internal.LocalObjectContainer
			 stream, Db4objects.Db4o.Internal.ClassMetadata yapClass, int objectId)
		{
			Db4objects.Db4o.Internal.StatefulBuffer writer = stream.ReadWriterByID(stream.SystemTransaction
				(), objectId);
			if (writer != null)
			{
				RebuildIndexForWriter(stream, writer, objectId);
			}
		}

		protected virtual void RebuildIndexForWriter(Db4objects.Db4o.Internal.LocalObjectContainer
			 stream, Db4objects.Db4o.Internal.StatefulBuffer writer, int objectId)
		{
			Db4objects.Db4o.Internal.Marshall.ObjectHeader oh = new Db4objects.Db4o.Internal.Marshall.ObjectHeader
				(stream, writer);
			object obj = ReadIndexEntryForRebuild(writer, oh);
			AddIndexEntry(stream.SystemTransaction(), objectId, obj);
		}

		private object ReadIndexEntryForRebuild(Db4objects.Db4o.Internal.StatefulBuffer writer
			, Db4objects.Db4o.Internal.Marshall.ObjectHeader oh)
		{
			return oh.ObjectMarshaller().ReadIndexEntry(oh.YapClass(), oh._headerAttributes, 
				this, writer);
		}

		public virtual void DropIndex(Db4objects.Db4o.Internal.Transaction systemTrans)
		{
			if (_index == null)
			{
				return;
			}
			Db4objects.Db4o.Internal.ObjectContainerBase stream = systemTrans.Stream();
			if (stream.ConfigImpl().MessageLevel() > Db4objects.Db4o.Internal.Const4.NONE)
			{
				stream.Message("dropping index " + ToString());
			}
			_index.Free(systemTrans);
			stream.SetDirtyInSystemTransaction(GetParentYapClass());
			_index = null;
		}

		public virtual void DefragField(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.ReaderPair readers)
		{
			GetHandler().Defrag(mf, readers, true);
		}
	}
}
