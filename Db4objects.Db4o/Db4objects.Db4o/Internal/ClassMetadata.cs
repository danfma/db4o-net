/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Internal.Encoding;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Handlers.Array;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Reflect;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Core;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class ClassMetadata : PersistentBase, IStoredClass
	{
		/// <summary>
		/// For reference types, _typeHandler always holds a StandardReferenceTypeHandler
		/// that will use the _aspects of this class to take care of its business.
		/// </summary>
		/// <remarks>
		/// For reference types, _typeHandler always holds a StandardReferenceTypeHandler
		/// that will use the _aspects of this class to take care of its business. A custom
		/// type handler would appear as a TypeHandlerAspect in that case.
		/// For value types, _typeHandler always holds the actual value type handler be it
		/// a custom type handler or a builtin one.
		/// </remarks>
		protected ITypeHandler4 _typeHandler;

		public Db4objects.Db4o.Internal.ClassMetadata i_ancestor;

		private Config4Class i_config;

		public ClassAspect[] _aspects;

		private readonly IClassIndexStrategy _index;

		private string i_name;

		private readonly ObjectContainerBase _container;

		internal byte[] i_nameBytes;

		private ByteArrayBuffer i_reader;

		private bool _classIndexed;

		private IReflectClass _classReflector;

		private IEventDispatcher _eventDispatcher;

		private bool _internal;

		private bool _unversioned;

		private TernaryBool _canUpdateFast = TernaryBool.Unspecified;

		private TranslatedAspect _translator;

		private IModificationAware _modificationChecker = ClassMetadata.AlwaysModified.Instance;

		private IFieldAccessor _fieldAccessor;

		private IFunction4 _constructor;

		private TypeHandlerAspect _customTypeHandlerAspect;

		internal bool CanUpdateFast()
		{
			if (_canUpdateFast == TernaryBool.Unspecified)
			{
				_canUpdateFast = TernaryBool.ForBoolean(CheckCanUpdateFast());
			}
			return _canUpdateFast.BooleanValue(false);
		}

		private bool CheckCanUpdateFast()
		{
			if (i_ancestor != null && !i_ancestor.CanUpdateFast())
			{
				return false;
			}
			if (i_config != null && i_config.CascadeOnDelete() == TernaryBool.Yes)
			{
				return false;
			}
			BooleanByRef hasIndex = new BooleanByRef(false);
			ForEachDeclaredField(new _IProcedure4_96(hasIndex));
			return !hasIndex.value;
		}

		private sealed class _IProcedure4_96 : IProcedure4
		{
			public _IProcedure4_96(BooleanByRef hasIndex)
			{
				this.hasIndex = hasIndex;
			}

			public void Apply(object arg)
			{
				if (((FieldMetadata)arg).HasIndex())
				{
					hasIndex.value = true;
				}
			}

			private readonly BooleanByRef hasIndex;
		}

		public virtual bool IsInternal()
		{
			return _internal;
		}

		private IClassIndexStrategy CreateIndexStrategy()
		{
			return new BTreeClassIndexStrategy(this);
		}

		protected ClassMetadata(ObjectContainerBase container)
		{
			if (null == container)
			{
				throw new ArgumentNullException();
			}
			_container = container;
			_index = CreateIndexStrategy();
			_classIndexed = true;
			_fieldAccessor = new StrictFieldAccessor();
		}

		public ClassMetadata(ObjectContainerBase container, IReflectClass classReflector)
		{
			if (null == container)
			{
				throw new ArgumentNullException();
			}
			_container = container;
			ClassReflector(classReflector);
			_index = CreateIndexStrategy();
			_classIndexed = true;
			if (_container.Config().ExceptionsOnNotStorable())
			{
				_fieldAccessor = new StrictFieldAccessor();
			}
			else
			{
				_fieldAccessor = new LenientFieldAccessor();
			}
		}

		internal virtual IFieldAccessor FieldAccessor()
		{
			return _fieldAccessor;
		}

		private ITypeHandler4 CreateDefaultTypeHandler()
		{
			// TODO: make sure initializeAspects has been executed
			// before the actual type handler is required
			// and remove this method
			return new StandardReferenceTypeHandler(this);
		}

		public virtual void CascadeActivation(IActivationContext context)
		{
			if (ObjectCanActivate(context.Transaction(), context.TargetObject()))
			{
				ForEachAspect(new _IProcedure4_153(context));
			}
		}

		private sealed class _IProcedure4_153 : IProcedure4
		{
			public _IProcedure4_153(IActivationContext context)
			{
				this.context = context;
			}

			public void Apply(object arg)
			{
				((ClassAspect)arg).CascadeActivation(context);
			}

			private readonly IActivationContext context;
		}

		public void AddFieldIndices(StatefulBuffer buffer, Slot slot)
		{
			if (!StandardReferenceTypeHandlerIsUsed())
			{
				return;
			}
			if (HasClassIndex() || HasVirtualAttributes())
			{
				ObjectHeader oh = new ObjectHeader(_container, this, buffer);
				ObjectIdContextImpl context = new ObjectIdContextImpl(buffer.Transaction(), buffer
					, oh, buffer.GetID());
				Handlers4.FieldAwareTypeHandler(CorrectHandlerVersion(context)).AddFieldIndices(context
					, slot);
			}
		}

		// FIXME: This method wants to be removed.
		private bool StandardReferenceTypeHandlerIsUsed()
		{
			return _typeHandler is StandardReferenceTypeHandler;
		}

		internal virtual void InitializeAspects()
		{
			BitTrue(Const4.CheckedChanges);
			Collection4 aspects = new Collection4();
			if (null != _aspects)
			{
				aspects.AddAll(_aspects);
			}
			ITypeHandler4 customTypeHandler = Container().Handlers().ConfiguredTypeHandler(ClassReflector
				());
			bool dirty = IsDirty();
			if (InstallTranslator(aspects, customTypeHandler))
			{
				dirty = true;
			}
			if (Container().DetectSchemaChanges())
			{
				if (GenerateVersionNumbers())
				{
					if (!HasVersionField())
					{
						aspects.Add(Container().VersionIndex());
						dirty = true;
					}
				}
				if (GenerateUUIDs())
				{
					if (!HasUUIDField())
					{
						aspects.Add(Container().UUIDIndex());
						dirty = true;
					}
				}
			}
			if (InstallCustomTypehandler(aspects, customTypeHandler))
			{
				dirty = true;
			}
			bool defaultFieldBehaviour = _translator == null && customTypeHandler == null;
			if (Container().DetectSchemaChanges())
			{
				if (defaultFieldBehaviour)
				{
					if (CollectReflectFields(aspects))
					{
						dirty = true;
					}
				}
				if (dirty)
				{
					_container.SetDirtyInSystemTransaction(this);
				}
			}
			if (dirty || !defaultFieldBehaviour)
			{
				_aspects = ToClassAspectArray(aspects);
			}
			DiagnosticProcessor dp = _container._handlers._diagnosticProcessor;
			if (dp.Enabled())
			{
				dp.CheckClassHasFields(this);
			}
			if (_aspects == null)
			{
				_aspects = new FieldMetadata[0];
			}
			InitializeConstructor(customTypeHandler);
			if (StateDead())
			{
				return;
			}
			_container.Callbacks().ClassOnRegistered(this);
			SetStateOK();
		}

		private ClassAspect[] ToClassAspectArray(Collection4 aspects)
		{
			ClassAspect[] array = new ClassAspect[aspects.Size()];
			aspects.ToArray(array);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetHandle(i);
			}
			return array;
		}

		private bool InstallCustomTypehandler(Collection4 aspects, ITypeHandler4 customTypeHandler
			)
		{
			if (customTypeHandler == null)
			{
				return false;
			}
			if (customTypeHandler is IModificationAware)
			{
				_modificationChecker = (IModificationAware)customTypeHandler;
			}
			if (Handlers4.IsStandaloneTypeHandler(customTypeHandler))
			{
				_typeHandler = customTypeHandler;
				return false;
			}
			bool dirty = false;
			TypeHandlerAspect typeHandlerAspect = new TypeHandlerAspect(this, customTypeHandler
				);
			if (!ReplaceAspectByName(aspects, typeHandlerAspect))
			{
				aspects.Add(typeHandlerAspect);
				dirty = true;
			}
			DisableAspectsBefore(aspects, typeHandlerAspect);
			_customTypeHandlerAspect = typeHandlerAspect;
			return dirty;
		}

		private void DisableAspectsBefore(Collection4 aspects, TypeHandlerAspect typeHandlerAspect
			)
		{
			int disableFromVersion = aspects.IndexOf(typeHandlerAspect) + 1;
			IEnumerator i = aspects.GetEnumerator();
			while (i.MoveNext())
			{
				ClassAspect aspect = (ClassAspect)i.Current;
				if (aspect == typeHandlerAspect)
				{
					break;
				}
				aspect.DisableFromAspectCountVersion(disableFromVersion);
			}
		}

		private bool InstallTranslator(Collection4 aspects, ITypeHandler4 customTypeHandler
			)
		{
			if (i_config == null)
			{
				return false;
			}
			IObjectTranslator translator = i_config.GetTranslator();
			if (translator == null)
			{
				return false;
			}
			ClassAspect existingAspect = AspectByName(aspects, TranslatedAspect.FieldNameFor(
				translator));
			if (null != existingAspect)
			{
				return InstallTranslatorOnExistingAspect(translator, existingAspect, aspects);
			}
			if (customTypeHandler == null)
			{
				return InstallTranslatorOnNewAspect(translator, aspects);
			}
			return false;
		}

		private bool InstallTranslatorOnNewAspect(IObjectTranslator translator, Collection4
			 aspects)
		{
			TranslatedAspect translatedAspect = new TranslatedAspect(this, translator);
			aspects.Add(translatedAspect);
			_translator = translatedAspect;
			return true;
		}

		private bool InstallTranslatorOnExistingAspect(IObjectTranslator translator, ClassAspect
			 existingAspect, Collection4 aspects)
		{
			if (existingAspect is TranslatedAspect)
			{
				TranslatedAspect translatedAspect = (TranslatedAspect)existingAspect;
				translatedAspect.InitializeTranslator(translator);
				_translator = translatedAspect;
				return false;
			}
			// older versions didn't store the aspect type properly
			_translator = new TranslatedAspect(this, translator);
			aspects.ReplaceByIdentity(existingAspect, _translator);
			return true;
		}

		private bool ReplaceAspectByName(Collection4 aspects, ClassAspect aspect)
		{
			ClassAspect existing = AspectByName(aspects, aspect.GetName());
			if (existing == null)
			{
				return false;
			}
			aspects.ReplaceByIdentity(existing, aspect);
			return true;
		}

		private ClassAspect AspectByName(Collection4 aspects, string aspectName)
		{
			IEnumerator i = aspects.GetEnumerator();
			while (i.MoveNext())
			{
				ClassAspect current = (ClassAspect)i.Current;
				if (current.GetName().Equals(aspectName))
				{
					return current;
				}
			}
			return null;
		}

		private bool CollectReflectFields(Collection4 collectedAspects)
		{
			bool dirty = false;
			IReflectField[] reflectFieldArray = ReflectFields();
			for (int reflectFieldIndex = 0; reflectFieldIndex < reflectFieldArray.Length; ++reflectFieldIndex)
			{
				IReflectField reflectField = reflectFieldArray[reflectFieldIndex];
				if (!StoreField(reflectField))
				{
					continue;
				}
				Db4objects.Db4o.Internal.ClassMetadata classMetadata = Handlers4.ErasedFieldType(
					Container(), reflectField.GetFieldType());
				if (classMetadata == null)
				{
					continue;
				}
				FieldMetadata field = new FieldMetadata(this, reflectField, classMetadata);
				if (Contains(collectedAspects, field))
				{
					continue;
				}
				dirty = true;
				collectedAspects.Add(field);
			}
			return dirty;
		}

		private bool Contains(Collection4 collectedAspects, FieldMetadata field)
		{
			IEnumerator aspectIterator = collectedAspects.GetEnumerator();
			while (aspectIterator.MoveNext())
			{
				if (((ClassAspect)aspectIterator.Current).Equals(field))
				{
					return true;
				}
			}
			return false;
		}

		internal virtual void AddToIndex(Transaction trans, int id)
		{
			if (!trans.Container().MaintainsIndices())
			{
				return;
			}
			AddToIndex1(trans, id);
		}

		internal void AddToIndex1(Transaction a_trans, int a_id)
		{
			if (i_ancestor != null)
			{
				i_ancestor.AddToIndex1(a_trans, a_id);
			}
			if (HasClassIndex())
			{
				_index.Add(a_trans, a_id);
			}
		}

		internal virtual bool AllowsQueries()
		{
			return HasClassIndex();
		}

		public virtual bool DescendOnCascadingActivation()
		{
			return true;
		}

		internal virtual void CheckChanges()
		{
			if (StateOK())
			{
				if (!BitIsTrue(Const4.CheckedChanges))
				{
					BitTrue(Const4.CheckedChanges);
					if (i_ancestor != null)
					{
						i_ancestor.CheckChanges();
					}
					// Ancestor first, so the object length calculates
					// correctly
					if (_classReflector != null)
					{
						InitializeAspects();
						if (!_container.IsClient() && !IsReadOnlyContainer())
						{
							Write(_container.SystemTransaction());
						}
					}
				}
			}
		}

		public virtual void CheckType()
		{
			IReflectClass claxx = ClassReflector();
			if (claxx == null)
			{
				return;
			}
			if (_container._handlers.IclassInternal.IsAssignableFrom(claxx))
			{
				_internal = true;
			}
			if (_container._handlers.IclassUnversioned.IsAssignableFrom(claxx))
			{
				_unversioned = true;
			}
			if (IsDb4oTypeImpl())
			{
				IDb4oTypeImpl db4oTypeImpl = (IDb4oTypeImpl)claxx.NewInstance();
				_classIndexed = (db4oTypeImpl == null || db4oTypeImpl.HasClassIndex());
			}
			else
			{
				if (i_config != null)
				{
					_classIndexed = i_config.Indexed();
				}
			}
		}

		public virtual bool IsDb4oTypeImpl()
		{
			return _container._handlers.IclassDb4otypeimpl.IsAssignableFrom(ClassReflector());
		}

		public int AdjustUpdateDepth(Transaction trans, int depth)
		{
			Config4Class config = ConfigOrAncestorConfig();
			if (depth == Const4.Unspecified)
			{
				depth = CheckUpdateDepthUnspecified(trans.Container().ConfigImpl());
				depth = AdjustCollectionDepthToBorders(depth);
			}
			if (config == null)
			{
				return depth - 1;
			}
			bool cascadeOnDelete = config.CascadeOnDelete() == TernaryBool.Yes;
			bool cascadeOnUpdate = config.CascadeOnUpdate() == TernaryBool.Yes;
			if (cascadeOnDelete || cascadeOnUpdate)
			{
				depth = AdjustDepthToBorders(depth);
			}
			return depth - 1;
		}

		public virtual int AdjustCollectionDepthToBorders(int depth)
		{
			if (!ClassReflector().IsCollection())
			{
				return depth;
			}
			return AdjustDepthToBorders(depth);
		}

		public virtual int AdjustDepthToBorders(int depth)
		{
			int depthBorder = Reflector().CollectionUpdateDepth(ClassReflector());
			if (depth > int.MinValue && depth < depthBorder)
			{
				depth = depthBorder;
			}
			return depth;
		}

		private int CheckUpdateDepthUnspecified(Config4Impl config)
		{
			int depth = config.UpdateDepth() + 1;
			if (i_config != null && i_config.UpdateDepth() != 0)
			{
				depth = i_config.UpdateDepth() + 1;
			}
			if (i_ancestor != null)
			{
				int ancestordepth = i_ancestor.CheckUpdateDepthUnspecified(config);
				if (ancestordepth > depth)
				{
					return ancestordepth;
				}
			}
			return depth;
		}

		public virtual void CollectConstraints(Transaction trans, QConObject parentConstraint
			, object obj, IVisitor4 visitor)
		{
			ForEachField(new _IProcedure4_514(trans, parentConstraint, obj, visitor));
		}

		private sealed class _IProcedure4_514 : IProcedure4
		{
			public _IProcedure4_514(Transaction trans, QConObject parentConstraint, object obj
				, IVisitor4 visitor)
			{
				this.trans = trans;
				this.parentConstraint = parentConstraint;
				this.obj = obj;
				this.visitor = visitor;
			}

			public void Apply(object arg)
			{
				FieldMetadata fieldMetadata = (FieldMetadata)arg;
				if (fieldMetadata.IsEnabledOn(AspectVersionContextImpl.CheckAlwaysEnabled))
				{
					fieldMetadata.CollectConstraints(trans, parentConstraint, obj, visitor);
				}
			}

			private readonly Transaction trans;

			private readonly QConObject parentConstraint;

			private readonly object obj;

			private readonly IVisitor4 visitor;
		}

		public void CollectIDs(CollectIdContext context, string fieldName)
		{
			CollectIDs(context, new _IPredicate4_525(fieldName));
		}

		private sealed class _IPredicate4_525 : IPredicate4
		{
			public _IPredicate4_525(string fieldName)
			{
				this.fieldName = fieldName;
			}

			public bool Match(object candidate)
			{
				return fieldName.Equals(((ClassAspect)candidate).GetName());
			}

			private readonly string fieldName;
		}

		public void CollectIDs(CollectIdContext context)
		{
			CollectIDs(context, new _IPredicate4_534());
		}

		private sealed class _IPredicate4_534 : IPredicate4
		{
			public _IPredicate4_534()
			{
			}

			public bool Match(object candidate)
			{
				return true;
			}
		}

		private void CollectIDs(CollectIdContext context, IPredicate4 predicate)
		{
			if (!StandardReferenceTypeHandlerIsUsed())
			{
				throw new InvalidOperationException();
			}
			((StandardReferenceTypeHandler)CorrectHandlerVersion(context)).CollectIDs(context
				, predicate);
		}

		public virtual void CollectIDs(QueryingReadContext context)
		{
			if (!StandardReferenceTypeHandlerIsUsed())
			{
				throw new InvalidOperationException();
			}
			Handlers4.CollectIDs(context, CorrectHandlerVersion(context));
		}

		public virtual Config4Class Config()
		{
			return i_config;
		}

		public virtual Config4Class ConfigOrAncestorConfig()
		{
			if (i_config != null)
			{
				return i_config;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.ConfigOrAncestorConfig();
			}
			return null;
		}

		private void ResolveClassReflector(string className)
		{
			IReflectClass reflectClass = _container.Reflector().ForName(className);
			if (null == reflectClass)
			{
				throw new InvalidOperationException("Cannot initialize ClassMetadata for '" + className
					 + "'.");
			}
			ClassReflector(reflectClass);
		}

		private void InitializeConstructor(ITypeHandler4 customTypeHandler)
		{
			if (IsTransient())
			{
				_container.LogMsg(23, GetName());
				SetStateDead();
				return;
			}
			if (IsInterface() || IsAbstract())
			{
				return;
			}
			IFunction4 constructor = CreateConstructor(customTypeHandler);
			if (constructor != null)
			{
				_constructor = constructor;
				return;
			}
			NotStorable();
		}

		private bool IsAbstract()
		{
			return ClassReflector().IsAbstract();
		}

		private bool IsInterface()
		{
			return ClassReflector().IsInterface();
		}

		private IFunction4 CreateConstructor(ITypeHandler4 customTypeHandler)
		{
			if (customTypeHandler is IInstantiatingTypeHandler)
			{
				return new _IFunction4_613(this);
			}
			if (HasObjectConstructor())
			{
				return new _IFunction4_621(this);
			}
			if (ClassReflector().EnsureCanBeInstantiated())
			{
				return new _IFunction4_629(this);
			}
			return null;
		}

		private sealed class _IFunction4_613 : IFunction4
		{
			public _IFunction4_613(ClassMetadata _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object context)
			{
				return this._enclosing.InstantiateWithCustomTypeHandlerIfEnabled(((UnmarshallingContext
					)context));
			}

			private readonly ClassMetadata _enclosing;
		}

		private sealed class _IFunction4_621 : IFunction4
		{
			public _IFunction4_621(ClassMetadata _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object context)
			{
				return this._enclosing._translator.Construct(((UnmarshallingContext)context));
			}

			private readonly ClassMetadata _enclosing;
		}

		private sealed class _IFunction4_629 : IFunction4
		{
			public _IFunction4_629(ClassMetadata _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object context)
			{
				return this._enclosing.InstantiateFromReflector(((UnmarshallingContext)context).Container
					());
			}

			private readonly ClassMetadata _enclosing;
		}

		private void NotStorable()
		{
			_container.LogMsg(7, GetName());
			SetStateDead();
		}

		private bool IsTransient()
		{
			return _container._handlers.IsTransient(ClassReflector());
		}

		private void ClassReflector(IReflectClass claxx)
		{
			_classReflector = claxx;
			if (claxx == null)
			{
				_typeHandler = null;
				return;
			}
			_typeHandler = CreateDefaultTypeHandler();
		}

		public virtual void Deactivate(Transaction trans, IObjectInfo reference, IActivationDepth
			 depth)
		{
			object obj = reference.GetObject();
			if (ObjectCanDeactivate(trans, obj))
			{
				ForceDeactivation(trans, depth, obj);
				ObjectOnDeactivate(trans, reference);
			}
		}

		public virtual void ForceDeactivation(Transaction trans, IActivationDepth depth, 
			object obj)
		{
			DeactivateFields(trans.Container().ActivationContextFor(trans, obj, depth));
		}

		private void ObjectOnDeactivate(Transaction transaction, IObjectInfo obj)
		{
			ObjectContainerBase container = transaction.Container();
			container.Callbacks().ObjectOnDeactivate(transaction, obj);
			DispatchEvent(transaction, obj.GetObject(), EventDispatchers.Deactivate);
		}

		private bool ObjectCanDeactivate(Transaction transaction, object obj)
		{
			ObjectContainerBase container = transaction.Container();
			return container.Callbacks().ObjectCanDeactivate(transaction, obj) && DispatchEvent
				(transaction, obj, EventDispatchers.CanDeactivate);
		}

		internal void DeactivateFields(IActivationContext context)
		{
			ForEachAspect(new _IProcedure4_681(context));
		}

		private sealed class _IProcedure4_681 : IProcedure4
		{
			public _IProcedure4_681(IActivationContext context)
			{
				this.context = context;
			}

			public void Apply(object arg)
			{
				ClassAspect classAspect = (ClassAspect)arg;
				if (classAspect.IsEnabledOn(AspectVersionContextImpl.CheckAlwaysEnabled))
				{
					classAspect.Deactivate(context);
				}
			}

			private readonly IActivationContext context;
		}

		internal void Delete(StatefulBuffer buffer, object obj)
		{
			ObjectHeader oh = new ObjectHeader(_container, this, buffer);
			Transaction trans = buffer.Transaction();
			int id = buffer.GetID();
			ArrayType arrayType = trans.Container()._handlers.ArrayType(obj);
			RemoveFromIndex(trans, id);
			DeleteContextImpl context = new DeleteContextImpl(buffer, oh, ClassReflector(), null
				);
			DeleteMembers(context, arrayType, false);
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			CorrectHandlerVersion(context).Delete(context);
		}

		internal virtual void DeleteMembers(DeleteContextImpl context, ArrayType arrayType
			, bool isUpdate)
		{
			StatefulBuffer buffer = (StatefulBuffer)context.Buffer();
			int preserveCascade = context.CascadeDeleteDepth();
			try
			{
				if (CascadeOnDelete())
				{
					if (ClassReflector().IsCollection())
					{
						buffer.SetCascadeDeletes(CollectionDeleteDepth(context));
					}
					else
					{
						buffer.SetCascadeDeletes(1);
					}
				}
				Handlers4.FieldAwareTypeHandler(CorrectHandlerVersion(context)).DeleteMembers(context
					, isUpdate);
			}
			catch (Exception e)
			{
				// This a catch for changed class hierarchies.
				// It's very ugly to catch all here but it does
				// help to heal migration from earlier db4o
				// versions.
				DiagnosticProcessor dp = Container()._handlers._diagnosticProcessor;
				if (dp.Enabled())
				{
					dp.DeletionFailed();
				}
			}
			buffer.SetCascadeDeletes(preserveCascade);
		}

		private int CollectionDeleteDepth(DeleteContextImpl context)
		{
			int depth = Reflector().CollectionUpdateDepth(ClassReflector()) - 2;
			// Minus two ???  
			if (depth < 1)
			{
				depth = 1;
			}
			return depth;
		}

		public virtual TernaryBool CascadeOnDeleteTernary()
		{
			Config4Class config = Config();
			TernaryBool cascadeOnDelete = TernaryBool.Unspecified;
			if (config != null && (cascadeOnDelete = config.CascadeOnDelete()) != TernaryBool
				.Unspecified)
			{
				return cascadeOnDelete;
			}
			if (i_ancestor == null)
			{
				return cascadeOnDelete;
			}
			return i_ancestor.CascadeOnDeleteTernary();
		}

		public virtual bool CascadeOnDelete()
		{
			return CascadeOnDeleteTernary() == TernaryBool.Yes;
		}

		public bool DispatchEvent(Transaction trans, object obj, int message)
		{
			return EventDispatcher().Dispatch(trans, obj, message);
		}

		public bool HasEventRegistered(Transaction trans, int eventID)
		{
			return EventDispatcher().HasEventRegistered(eventID);
		}

		private IEventDispatcher EventDispatcher()
		{
			if (null != _eventDispatcher)
			{
				return _eventDispatcher;
			}
			_eventDispatcher = EventDispatchers.ForClass(_container, ClassReflector());
			return _eventDispatcher;
		}

		public int DeclaredAspectCount()
		{
			if (_aspects == null)
			{
				return 0;
			}
			return _aspects.Length;
		}

		public int AspectCount()
		{
			int count = DeclaredAspectCount();
			if (i_ancestor != null)
			{
				count += i_ancestor.AspectCount();
			}
			return count;
		}

		// Scrolls offset in passed reader to the offset the passed field should
		// be read at.
		public HandlerVersion SeekToField(Transaction trans, ByteArrayBuffer buffer, FieldMetadata
			 field)
		{
			if (buffer == null)
			{
				return HandlerVersion.Invalid;
			}
			if (!StandardReferenceTypeHandlerIsUsed())
			{
				return HandlerVersion.Invalid;
			}
			buffer.Seek(0);
			ObjectHeader oh = new ObjectHeader(_container, this, buffer);
			bool res = SeekToField(new ObjectHeaderContext(trans, buffer, oh), field);
			if (!res)
			{
				return HandlerVersion.Invalid;
			}
			return new HandlerVersion(oh.HandlerVersion());
		}

		public bool SeekToField(ObjectHeaderContext context, ClassAspect field)
		{
			return Handlers4.FieldAwareTypeHandler(CorrectHandlerVersion(context)).SeekToField
				(context, field);
		}

		public virtual bool GenerateUUIDs()
		{
			if (!GenerateVirtual())
			{
				return false;
			}
			bool configValue = (i_config == null) ? false : i_config.GenerateUUIDs();
			return Generate1(_container.Config().GenerateUUIDs(), configValue);
		}

		private bool GenerateVersionNumbers()
		{
			if (!GenerateVirtual())
			{
				return false;
			}
			bool configValue = (i_config == null) ? false : i_config.GenerateVersionNumbers();
			return Generate1(_container.Config().GenerateVersionNumbers(), configValue);
		}

		private bool GenerateVirtual()
		{
			if (_unversioned)
			{
				return false;
			}
			if (_internal)
			{
				return false;
			}
			return true;
		}

		private bool Generate1(ConfigScope globalConfig, bool individualConfig)
		{
			return globalConfig.ApplyConfig(individualConfig);
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata GetAncestor()
		{
			return i_ancestor;
		}

		public virtual object GetComparableObject(object forObject)
		{
			if (i_config != null)
			{
				if (i_config.QueryAttributeProvider() != null)
				{
					return i_config.QueryAttributeProvider().Attribute(forObject);
				}
			}
			return forObject;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata GetHigherHierarchy(Db4objects.Db4o.Internal.ClassMetadata
			 a_yapClass)
		{
			Db4objects.Db4o.Internal.ClassMetadata yc = GetHigherHierarchy1(a_yapClass);
			if (yc != null)
			{
				return yc;
			}
			return a_yapClass.GetHigherHierarchy1(this);
		}

		private Db4objects.Db4o.Internal.ClassMetadata GetHigherHierarchy1(Db4objects.Db4o.Internal.ClassMetadata
			 a_yapClass)
		{
			if (a_yapClass == this)
			{
				return this;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.GetHigherHierarchy1(a_yapClass);
			}
			return null;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata GetHigherOrCommonHierarchy(
			Db4objects.Db4o.Internal.ClassMetadata a_yapClass)
		{
			Db4objects.Db4o.Internal.ClassMetadata yc = GetHigherHierarchy1(a_yapClass);
			if (yc != null)
			{
				return yc;
			}
			if (i_ancestor != null)
			{
				yc = i_ancestor.GetHigherOrCommonHierarchy(a_yapClass);
				if (yc != null)
				{
					return yc;
				}
			}
			return a_yapClass.GetHigherHierarchy1(this);
		}

		public override byte GetIdentifier()
		{
			return Const4.Yapclass;
		}

		public virtual long[] GetIDs()
		{
			lock (Lock())
			{
				if (!StateOK())
				{
					return new long[0];
				}
				return GetIDs(_container.Transaction());
			}
		}

		public virtual long[] GetIDs(Transaction trans)
		{
			lock (Lock())
			{
				if (!StateOK())
				{
					return new long[0];
				}
				if (!HasClassIndex())
				{
					return new long[0];
				}
				return trans.Container().GetIDsForClass(trans, this);
			}
		}

		public virtual bool HasClassIndex()
		{
			if (!_classIndexed)
			{
				return false;
			}
			return StandardReferenceTypeHandlerIsUsed() || !(Handlers4.IsValueType(_typeHandler
				));
		}

		private bool AncestorHasUUIDField()
		{
			if (i_ancestor == null)
			{
				return false;
			}
			return i_ancestor.HasUUIDField();
		}

		private bool HasUUIDField()
		{
			if (AncestorHasUUIDField())
			{
				return true;
			}
			return Arrays4.ContainsInstanceOf(_aspects, typeof(UUIDFieldMetadata));
		}

		private bool AncestorHasVersionField()
		{
			if (i_ancestor == null)
			{
				return false;
			}
			return i_ancestor.HasVersionField();
		}

		private bool HasVersionField()
		{
			if (AncestorHasVersionField())
			{
				return true;
			}
			return Arrays4.ContainsInstanceOf(_aspects, typeof(VersionFieldMetadata));
		}

		public virtual IClassIndexStrategy Index()
		{
			return _index;
		}

		public virtual int IndexEntryCount(Transaction ta)
		{
			if (!StateOK())
			{
				return 0;
			}
			return _index.EntryCount(ta);
		}

		public virtual IReflectClass ClassReflector()
		{
			return _classReflector;
		}

		public virtual string GetName()
		{
			if (i_name == null)
			{
				if (_classReflector != null)
				{
					SetName(_classReflector.GetName());
				}
			}
			return i_name;
		}

		public virtual IStoredClass GetParentStoredClass()
		{
			return GetAncestor();
		}

		public virtual IStoredField[] GetStoredFields()
		{
			lock (Lock())
			{
				if (_aspects == null)
				{
					return new IStoredField[0];
				}
				Collection4 storedFields = new Collection4();
				ForEachDeclaredField(new _IProcedure4_996(storedFields));
				IStoredField[] fields = new IStoredField[storedFields.Size()];
				storedFields.ToArray(fields);
				return fields;
			}
		}

		private sealed class _IProcedure4_996 : IProcedure4
		{
			public _IProcedure4_996(Collection4 storedFields)
			{
				this.storedFields = storedFields;
			}

			public void Apply(object field)
			{
				storedFields.Add(field);
			}

			private readonly Collection4 storedFields;
		}

		public ObjectContainerBase Container()
		{
			return _container;
		}

		public virtual FieldMetadata FieldMetadataForName(string name)
		{
			ByRef byReference = new ByRef();
			ForEachField(new _IProcedure4_1013(name, byReference));
			return (FieldMetadata)byReference.value;
		}

		private sealed class _IProcedure4_1013 : IProcedure4
		{
			public _IProcedure4_1013(string name, ByRef byReference)
			{
				this.name = name;
				this.byReference = byReference;
			}

			public void Apply(object arg)
			{
				if (name.Equals(((FieldMetadata)arg).GetName()))
				{
					byReference.value = arg;
				}
			}

			private readonly string name;

			private readonly ByRef byReference;
		}

		/// <param name="container"></param>
		public virtual bool HasField(ObjectContainerBase container, string fieldName)
		{
			if (ClassReflector().IsCollection())
			{
				return true;
			}
			return FieldMetadataForName(fieldName) != null;
		}

		internal virtual bool HasVirtualAttributes()
		{
			if (_internal)
			{
				return false;
			}
			return HasVersionField() || HasUUIDField();
		}

		public virtual bool HoldsAnyClass()
		{
			return ClassReflector().IsCollection();
		}

		internal virtual void IncrementFieldsOffset1(ByteArrayBuffer a_bytes)
		{
			int length = ReadAspectCount(a_bytes);
			for (int i = 0; i < length; i++)
			{
				_aspects[i].IncrementOffset(a_bytes);
			}
		}

		internal bool Init(Db4objects.Db4o.Internal.ClassMetadata ancestor)
		{
			if (DTrace.enabled)
			{
				DTrace.ClassmetadataInit.Log(GetID());
			}
			SetConfig(ConfigImpl().ConfigClass(GetName()));
			SetAncestor(ancestor);
			CheckType();
			if (AllowsQueries())
			{
				_index.Initialize(_container);
			}
			BitTrue(Const4.CheckedChanges);
			return true;
		}

		internal void InitConfigOnUp(Transaction systemTrans)
		{
			Config4Class extendedConfig = Platform4.ExtendConfiguration(_classReflector, _container
				.Configure(), i_config);
			if (extendedConfig != null)
			{
				i_config = extendedConfig;
			}
			if (i_config == null)
			{
				return;
			}
			if (!StateOK())
			{
				return;
			}
			InitializeFieldsConfiguration(systemTrans, extendedConfig);
			CheckAllConfiguredFieldsExist(extendedConfig);
		}

		private void InitializeFieldsConfiguration(Transaction systemTrans, Config4Class 
			extendedConfig)
		{
			if (_aspects == null)
			{
				return;
			}
			for (int i = 0; i < _aspects.Length; i++)
			{
				if (_aspects[i] is FieldMetadata)
				{
					FieldMetadata field = (FieldMetadata)_aspects[i];
					string fieldName = field.GetName();
					if (!field.HasConfig() && extendedConfig != null && extendedConfig.ConfigField(fieldName
						) != null)
					{
						field.InitConfiguration(fieldName);
					}
					field.InitConfigOnUp(systemTrans);
				}
			}
		}

		private void CheckAllConfiguredFieldsExist(Config4Class config)
		{
			Hashtable4 exceptionalFields = config.ExceptionalFieldsOrNull();
			if (exceptionalFields == null)
			{
				return;
			}
			IEnumerator i = exceptionalFields.ValuesIterator();
			while (i.MoveNext())
			{
				Config4Field fieldConfig = (Config4Field)i.Current;
				if (!fieldConfig.Used())
				{
					ConfigImpl().DiagnosticProcessor().ObjectFieldDoesNotExist(GetName(), fieldConfig
						.GetName());
				}
			}
		}

		internal virtual void InitOnUp(Transaction systemTrans)
		{
			if (!StateOK())
			{
				return;
			}
			InitConfigOnUp(systemTrans);
			StoreStaticFieldValues(systemTrans, false);
		}

		public virtual object Instantiate(UnmarshallingContext context)
		{
			// overridden in PrimitiveTypeMetadata
			// never called for primitive YapAny
			// FIXME: [TA] no longer necessary?
			//        context.adjustInstantiationDepth();
			object obj = context.PersistentObject();
			bool instantiating = (obj == null);
			if (instantiating)
			{
				obj = InstantiateObject(context);
				if (obj == null)
				{
					return null;
				}
				ShareTransaction(obj, context.Transaction());
				ShareObjectReference(obj, context.ObjectReference());
				OnInstantiate(context, obj);
				if (context.ActivationDepth().Mode().IsPrefetch())
				{
					context.ObjectReference().SetStateDeactivated();
					return obj;
				}
				if (!context.ActivationDepth().RequiresActivation())
				{
					context.ObjectReference().SetStateDeactivated();
					return obj;
				}
				return Activate(context);
			}
			if (ActivatingActiveObject(context.ActivationDepth().Mode(), context.ObjectReference
				()))
			{
				IActivationDepth child = context.ActivationDepth().Descend(this);
				if (child.RequiresActivation())
				{
					CascadeActivation(new ActivationContext4(context.Transaction(), obj, child));
				}
				return obj;
			}
			return Activate(context);
		}

		protected void OnInstantiate(UnmarshallingContext context, object obj)
		{
			context.SetObjectWeak(obj);
			context.Transaction().ReferenceSystem().AddExistingReference(context.ObjectReference
				());
			ObjectOnInstantiate(context.Transaction(), context.ObjectReference());
		}

		public virtual object InstantiateTransient(UnmarshallingContext context)
		{
			// overridden in YapClassPrimitive
			// never called for primitive YapAny
			object obj = InstantiateObject(context);
			if (obj == null)
			{
				return null;
			}
			context.Container().Peeked(context.ObjectID(), obj);
			if (context.ActivationDepth().RequiresActivation())
			{
				InstantiateFields(context);
			}
			return obj;
		}

		private bool ActivatingActiveObject(ActivationMode mode, ObjectReference @ref)
		{
			return !mode.IsRefresh() && @ref.IsActive();
		}

		private object Activate(UnmarshallingContext context)
		{
			object obj = context.PersistentObject();
			ObjectReference objectReference = context.ObjectReference();
			if (!ObjectCanActivate(context.Transaction(), obj))
			{
				objectReference.SetStateDeactivated();
				return obj;
			}
			objectReference.SetStateClean();
			if (context.ActivationDepth().RequiresActivation())
			{
				InstantiateFields(context);
			}
			ObjectOnActivate(context.Transaction(), objectReference);
			return obj;
		}

		public virtual bool HasObjectConstructor()
		{
			return _translator != null && _translator.IsObjectConstructor();
		}

		private object InstantiateObject(UnmarshallingContext context)
		{
			object obj = _constructor.Apply(context);
			context.PersistentObject(obj);
			return obj;
		}

		private void ObjectOnInstantiate(Transaction transaction, IObjectInfo reference)
		{
			transaction.Container().Callbacks().ObjectOnInstantiate(transaction, reference);
		}

		private object InstantiateFromReflector(ObjectContainerBase stream)
		{
			if (_classReflector == null)
			{
				throw new InvalidOperationException();
			}
			try
			{
				return _classReflector.NewInstance();
			}
			catch (MissingMethodException)
			{
				Container().LogMsg(7, ClassReflector().GetName());
				return null;
			}
			catch (Exception)
			{
				// TODO: be more helpful here
				return null;
			}
		}

		private void ShareObjectReference(object obj, ObjectReference @ref)
		{
			if (obj is IDb4oTypeImpl)
			{
				((IDb4oTypeImpl)obj).SetObjectReference(@ref);
			}
		}

		private void ShareTransaction(object obj, Transaction transaction)
		{
			if (obj is ITransactionAware)
			{
				((ITransactionAware)obj).SetTrans(transaction);
			}
		}

		private void ObjectOnActivate(Transaction transaction, IObjectInfo obj)
		{
			ObjectContainerBase container = transaction.Container();
			container.Callbacks().ObjectOnActivate(transaction, obj);
			DispatchEvent(transaction, obj.GetObject(), EventDispatchers.Activate);
		}

		private bool ObjectCanActivate(Transaction transaction, object obj)
		{
			ObjectContainerBase container = transaction.Container();
			return container.Callbacks().ObjectCanActivate(transaction, obj) && DispatchEvent
				(transaction, obj, EventDispatchers.CanActivate);
		}

		internal virtual void InstantiateFields(UnmarshallingContext context)
		{
			ITypeHandler4 handler = CorrectHandlerVersion((IHandlerVersionContext)context);
			Handlers4.Activate(context, handler);
		}

		public virtual bool IsArray()
		{
			return ClassReflector().IsCollection();
		}

		internal virtual bool IsCollection(object obj)
		{
			return Reflector().ForObject(obj).IsCollection();
		}

		public override bool IsDirty()
		{
			if (!StateOK())
			{
				return false;
			}
			return base.IsDirty();
		}

		internal virtual bool IsEnum()
		{
			return false;
		}

		public virtual bool IsPrimitive()
		{
			return false;
		}

		/// <summary>no any, primitive, array or other tricks.</summary>
		/// <remarks>
		/// no any, primitive, array or other tricks. overriden in YapClassAny and
		/// YapClassPrimitive
		/// </remarks>
		public virtual bool IsStrongTyped()
		{
			return true;
		}

		public virtual bool IsValueType()
		{
			return Handlers4.HoldsValueType(_typeHandler);
		}

		private object Lock()
		{
			return _container.Lock();
		}

		public virtual string NameToWrite()
		{
			if (i_config != null && i_config.WriteAs() != null)
			{
				return i_config.WriteAs();
			}
			if (i_name == null)
			{
				return string.Empty;
			}
			return ConfigImpl().ResolveAliasRuntimeName(i_name);
		}

		public bool CallConstructor()
		{
			TernaryBool specialized = CallConstructorSpecialized();
			// FIXME: If specified, return yes?!?
			if (!specialized.IsUnspecified())
			{
				return specialized.DefiniteYes();
			}
			return ConfigImpl().CallConstructors().DefiniteYes();
		}

		private Config4Impl ConfigImpl()
		{
			return _container.ConfigImpl();
		}

		private TernaryBool CallConstructorSpecialized()
		{
			if (i_config != null)
			{
				TernaryBool res = i_config.CallConstructor();
				if (!res.IsUnspecified())
				{
					return res;
				}
			}
			if (IsEnum())
			{
				return TernaryBool.No;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.CallConstructorSpecialized();
			}
			return TernaryBool.Unspecified;
		}

		public override int OwnLength()
		{
			return MarshallerFamily.Current()._class.MarshalledLength(_container, this);
		}

		internal virtual void Purge()
		{
			_index.Purge();
		}

		// TODO: may want to add manual purge to Btree
		//       indexes here
		// FIXME: [TA] ActivationDepth review
		public virtual object ReadAndActivate(Transaction trans, int id, IActivationDepth
			 depth)
		{
			// Method for C# value types and for map members:
			// they need to be instantiated before setting them
			// on the parent object. 
			// For value types the set call modifies identity.
			// In maps, adding the object to the map calls #hashCode and #equals,
			// so the object needs to be activated.
			// TODO: Question: Do we want value types in the ID tree?
			// Shouldn't we treat them like strings and update
			// them every time ???		
			ObjectReference @ref = trans.ReferenceForId(id);
			if (@ref != null)
			{
				object obj = @ref.GetObject();
				if (obj == null)
				{
					trans.RemoveReference(@ref);
				}
				else
				{
					@ref.Activate(trans, obj, depth);
					return @ref.GetObject();
				}
			}
			return new ObjectReference(id).Read(trans, depth, Const4.AddToIdTree, false);
		}

		public virtual ITypeHandler4 ReadCandidateHandler(QueryingReadContext context)
		{
			ITypeHandler4 typeHandler = CorrectHandlerVersion(context);
			if (typeHandler is ICascadingTypeHandler)
			{
				return ((ICascadingTypeHandler)typeHandler).ReadCandidateHandler(context);
			}
			return null;
		}

		public virtual ITypeHandler4 SeekCandidateHandler(QueryingReadContext context)
		{
			if (IsArray())
			{
				if (Platform4.IsCollectionTranslator(this.i_config))
				{
					context.Seek(context.Offset() + Const4.IntLength);
					return new ArrayHandler(null, false);
				}
				IncrementFieldsOffset1((ByteArrayBuffer)context.Buffer());
				if (i_ancestor != null)
				{
					return i_ancestor.SeekCandidateHandler(context);
				}
			}
			return null;
		}

		public int ReadAspectCount(IReadBuffer buffer)
		{
			int count = buffer.ReadInt();
			if (count > _aspects.Length)
			{
				return _aspects.Length;
			}
			return count;
		}

		internal virtual byte[] ReadName(Transaction a_trans)
		{
			i_reader = a_trans.Container().ReadReaderByID(a_trans, GetID());
			return ReadName1(a_trans, i_reader);
		}

		public byte[] ReadName1(Transaction trans, ByteArrayBuffer reader)
		{
			if (reader == null)
			{
				return null;
			}
			i_reader = reader;
			bool ok = false;
			try
			{
				ClassMarshaller marshaller = MarshallerFamily.Current()._class;
				i_nameBytes = marshaller.ReadName(trans, reader);
				marshaller.ReadMetaClassID(reader);
				// never used ???
				SetStateUnread();
				BitFalse(Const4.CheckedChanges);
				BitFalse(Const4.StaticFieldsStored);
				ok = true;
				return i_nameBytes;
			}
			finally
			{
				if (!ok)
				{
					SetStateDead();
				}
			}
		}

		public virtual void ReadVirtualAttributes(Transaction trans, ObjectReference @ref
			, bool lastCommitted)
		{
			int id = @ref.GetID();
			ObjectContainerBase stream = trans.Container();
			ByteArrayBuffer buffer = stream.ReadReaderByID(trans, id, lastCommitted);
			ObjectHeader oh = new ObjectHeader(stream, this, buffer);
			ObjectReferenceContext context = new ObjectReferenceContext(trans, buffer, oh, @ref
				);
			Handlers4.FieldAwareTypeHandler(CorrectHandlerVersion(context)).ReadVirtualAttributes
				(context);
		}

		public virtual GenericReflector Reflector()
		{
			return _container.Reflector();
		}

		public virtual void Rename(string newName)
		{
			if (_container.IsClient())
			{
				Exceptions4.ThrowRuntimeException(58);
			}
			int tempState = _state;
			SetStateOK();
			SetName(newName);
			i_nameBytes = AsBytes(i_name);
			SetStateDirty();
			Write(_container.SystemTransaction());
			IReflectClass oldReflector = _classReflector;
			ClassReflector(Container().Reflector().ForName(newName));
			Container().ClassCollection().RefreshClassCache(this, oldReflector);
			Refresh();
			_state = tempState;
		}

		//TODO: duplicates ClassMetadataRepository#asBytes
		private byte[] AsBytes(string str)
		{
			return Container().StringIO().Write(str);
		}

		internal void CreateConfigAndConstructor(Hashtable4 a_byteHashTable, IReflectClass
			 claxx)
		{
			SetName(ResolveName(claxx));
			SetConfig(ConfigImpl().ConfigClass(GetName()));
			if (claxx == null)
			{
				ResolveClassReflector(GetName());
			}
			else
			{
				ClassReflector(claxx);
			}
			//            createConstructor(true);
			if (i_nameBytes != null)
			{
				a_byteHashTable.Remove(i_nameBytes);
				i_nameBytes = null;
			}
		}

		internal virtual string ResolveName(IReflectClass claxx)
		{
			if (claxx != null)
			{
				return claxx.GetName();
			}
			if (i_nameBytes != null)
			{
				string name = _container.StringIO().Read(i_nameBytes);
				return ConfigImpl().ResolveAliasStoredName(name);
			}
			throw new InvalidOperationException();
		}

		internal virtual bool ReadThis()
		{
			bool stateUnread = StateUnread();
			if (stateUnread)
			{
				SetStateOK();
				SetStateClean();
			}
			if (stateUnread || StateDead())
			{
				ForceRead();
				return true;
			}
			return false;
		}

		internal void ForceRead()
		{
			if (i_reader == null || BitIsTrue(Const4.Reading))
			{
				return;
			}
			BitTrue(Const4.Reading);
			try
			{
				MarshallerFamily.ForConverterVersion(_container.ConverterVersion())._class.Read(_container
					, this, i_reader);
				i_nameBytes = null;
				i_reader = null;
			}
			finally
			{
				BitFalse(Const4.Reading);
			}
		}

		public override void ReadThis(Transaction a_trans, ByteArrayBuffer a_reader)
		{
			throw Exceptions4.VirtualException();
		}

		public virtual void Refresh()
		{
			if (!StateUnread())
			{
				ResolveClassReflector(i_name);
				BitFalse(Const4.CheckedChanges);
				CheckChanges();
				ForEachDeclaredField(new _IProcedure4_1567());
			}
		}

		private sealed class _IProcedure4_1567 : IProcedure4
		{
			public _IProcedure4_1567()
			{
			}

			public void Apply(object arg)
			{
				((FieldMetadata)arg).Refresh();
			}
		}

		internal virtual void RemoveFromIndex(Transaction ta, int id)
		{
			if (HasClassIndex())
			{
				_index.Remove(ta, id);
			}
			if (i_ancestor != null)
			{
				i_ancestor.RemoveFromIndex(ta, id);
			}
		}

		internal virtual bool RenameField(string oldName, string newName)
		{
			BooleanByRef renamed = new BooleanByRef(false);
			for (int i = 0; i < _aspects.Length; i++)
			{
				if (_aspects[i].GetName().Equals(newName))
				{
					_container.LogMsg(9, "class:" + GetName() + " field:" + newName);
					return false;
				}
			}
			ForEachDeclaredField(new _IProcedure4_1592(oldName, newName, renamed));
			return renamed.value;
		}

		private sealed class _IProcedure4_1592 : IProcedure4
		{
			public _IProcedure4_1592(string oldName, string newName, BooleanByRef renamed)
			{
				this.oldName = oldName;
				this.newName = newName;
				this.renamed = renamed;
			}

			public void Apply(object arg)
			{
				FieldMetadata field = (FieldMetadata)arg;
				if (field.GetName().Equals(oldName))
				{
					field.SetName(newName);
					renamed.value = true;
				}
			}

			private readonly string oldName;

			private readonly string newName;

			private readonly BooleanByRef renamed;
		}

		internal virtual void SetConfig(Config4Class config)
		{
			if (config == null)
			{
				return;
			}
			// The configuration can be set by a ObjectClass#readAs setting
			// from YapClassCollection, right after reading the meta information
			// for the first time. In that case we never change the setting
			if (i_config == null)
			{
				i_config = config;
			}
		}

		internal virtual void SetName(string a_name)
		{
			i_name = a_name;
		}

		internal void SetStateDead()
		{
			BitTrue(Const4.Dead);
			BitFalse(Const4.Continue);
		}

		private void SetStateUnread()
		{
			BitFalse(Const4.Dead);
			BitTrue(Const4.Continue);
		}

		internal void SetStateOK()
		{
			BitFalse(Const4.Dead);
			BitFalse(Const4.Continue);
		}

		internal virtual bool StateDead()
		{
			return BitIsTrue(Const4.Dead);
		}

		private bool StateOK()
		{
			return BitIsFalse(Const4.Continue) && BitIsFalse(Const4.Dead) && BitIsFalse(Const4
				.Reading);
		}

		internal bool StateOKAndAncestors()
		{
			if (!StateOK() || _aspects == null)
			{
				return false;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.StateOKAndAncestors();
			}
			return true;
		}

		internal virtual bool StateUnread()
		{
			return BitIsTrue(Const4.Continue) && BitIsFalse(Const4.Dead) && BitIsFalse(Const4
				.Reading);
		}

		internal virtual bool StoreField(IReflectField field)
		{
			if (field.IsStatic())
			{
				return false;
			}
			if (IsTransient(field))
			{
				if (!ShouldStoreTransientFields())
				{
					return false;
				}
			}
			return Platform4.CanSetAccessible() || field.IsPublic();
		}

		private bool ShouldStoreTransientFields()
		{
			Config4Class config = ConfigOrAncestorConfig();
			if (config == null)
			{
				return false;
			}
			return config.StoreTransientFields();
		}

		private bool IsTransient(IReflectField field)
		{
			return field.IsTransient() || Platform4.IsTransient(field.GetFieldType());
		}

		public virtual IStoredField StoredField(string fieldName, object fieldType)
		{
			lock (Lock())
			{
				Db4objects.Db4o.Internal.ClassMetadata fieldTypeFilter = fieldType == null ? null
					 : _container.ClassMetadataForReflectClass(ReflectorUtils.ReflectClassFor(Reflector
					(), fieldType));
				ByRef foundField = new ByRef();
				ForEachField(new _IProcedure4_1695(foundField, fieldName, fieldTypeFilter));
				// TODO: implement field creation
				return (IStoredField)foundField.value;
			}
		}

		private sealed class _IProcedure4_1695 : IProcedure4
		{
			public _IProcedure4_1695(ByRef foundField, string fieldName, Db4objects.Db4o.Internal.ClassMetadata
				 fieldTypeFilter)
			{
				this.foundField = foundField;
				this.fieldName = fieldName;
				this.fieldTypeFilter = fieldTypeFilter;
			}

			public void Apply(object arg)
			{
				if (foundField.value != null)
				{
					return;
				}
				FieldMetadata field = (FieldMetadata)arg;
				if (field.GetName().Equals(fieldName))
				{
					if (fieldTypeFilter == null || fieldTypeFilter == field.FieldType())
					{
						foundField.value = field;
					}
				}
			}

			private readonly ByRef foundField;

			private readonly string fieldName;

			private readonly Db4objects.Db4o.Internal.ClassMetadata fieldTypeFilter;
		}

		internal virtual void StoreStaticFieldValues(Transaction trans, bool force)
		{
			if (BitIsTrue(Const4.StaticFieldsStored) && !force)
			{
				return;
			}
			BitTrue(Const4.StaticFieldsStored);
			if (!ShouldStoreStaticFields(trans))
			{
				return;
			}
			ObjectContainerBase stream = trans.Container();
			stream.ShowInternalClasses(true);
			try
			{
				StaticClass sc = QueryStaticClass(trans);
				if (sc == null)
				{
					CreateStaticClass(trans);
				}
				else
				{
					UpdateStaticClass(trans, sc);
				}
			}
			finally
			{
				stream.ShowInternalClasses(false);
			}
		}

		private bool ShouldStoreStaticFields(Transaction trans)
		{
			return !IsReadOnlyContainer() && (StaticFieldValuesArePersisted() || Platform4.StoreStaticFieldValues
				(trans.Reflector(), ClassReflector()));
		}

		private bool IsReadOnlyContainer()
		{
			return Container().Config().IsReadOnly();
		}

		private void UpdateStaticClass(Transaction trans, StaticClass sc)
		{
			ObjectContainerBase stream = trans.Container();
			stream.Activate(trans, sc, new FixedActivationDepth(4));
			StaticField[] existingFields = sc.fields;
			IEnumerator staticFields = Iterators.Map(StaticReflectFields(), new _IFunction4_1755
				(this, existingFields, trans));
			sc.fields = ToStaticFieldArray(staticFields);
			if (!stream.IsClient())
			{
				SetStaticClass(trans, sc);
			}
		}

		private sealed class _IFunction4_1755 : IFunction4
		{
			public _IFunction4_1755(ClassMetadata _enclosing, StaticField[] existingFields, Transaction
				 trans)
			{
				this._enclosing = _enclosing;
				this.existingFields = existingFields;
				this.trans = trans;
			}

			public object Apply(object arg)
			{
				IReflectField reflectField = (IReflectField)arg;
				StaticField existingField = this._enclosing.FieldByName(existingFields, reflectField
					.GetName());
				if (existingField != null)
				{
					this._enclosing.UpdateExistingStaticField(trans, existingField, reflectField);
					return existingField;
				}
				return this._enclosing.ToStaticField(reflectField);
			}

			private readonly ClassMetadata _enclosing;

			private readonly StaticField[] existingFields;

			private readonly Transaction trans;
		}

		private void CreateStaticClass(Transaction trans)
		{
			if (trans.Container().IsClient())
			{
				return;
			}
			StaticClass sc = new StaticClass(GetName(), ToStaticFieldArray(StaticReflectFieldsToStaticFields
				()));
			SetStaticClass(trans, sc);
		}

		private IEnumerator StaticReflectFieldsToStaticFields()
		{
			return Iterators.Map(StaticReflectFields(), new _IFunction4_1783(this));
		}

		private sealed class _IFunction4_1783 : IFunction4
		{
			public _IFunction4_1783(ClassMetadata _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object arg)
			{
				return this._enclosing.ToStaticField((IReflectField)arg);
			}

			private readonly ClassMetadata _enclosing;
		}

		protected virtual StaticField ToStaticField(IReflectField reflectField)
		{
			return new StaticField(reflectField.GetName(), StaticReflectFieldValue(reflectField
				));
		}

		private object StaticReflectFieldValue(IReflectField reflectField)
		{
			return _fieldAccessor.Get(reflectField, null);
		}

		private void SetStaticClass(Transaction trans, StaticClass sc)
		{
			// TODO: we should probably use a specific update depth here, 4?
			trans.Container().StoreInternal(trans, sc, true);
		}

		private StaticField[] ToStaticFieldArray(IEnumerator iterator4)
		{
			return ToStaticFieldArray(new Collection4(iterator4));
		}

		private StaticField[] ToStaticFieldArray(Collection4 fields)
		{
			return (StaticField[])fields.ToArray(new StaticField[fields.Size()]);
		}

		private IEnumerator StaticReflectFields()
		{
			return Iterators.Filter(ReflectFields(), new _IPredicate4_1812());
		}

		private sealed class _IPredicate4_1812 : IPredicate4
		{
			public _IPredicate4_1812()
			{
			}

			public bool Match(object candidate)
			{
				return ((IReflectField)candidate).IsStatic();
			}
		}

		private IReflectField[] ReflectFields()
		{
			return ClassReflector().GetDeclaredFields();
		}

		protected virtual void UpdateExistingStaticField(Transaction trans, StaticField existingField
			, IReflectField reflectField)
		{
			ObjectContainerBase stream = trans.Container();
			object newValue = StaticReflectFieldValue(reflectField);
			if (existingField.value != null && newValue != null && existingField.value.GetType
				() == newValue.GetType())
			{
				int id = stream.GetID(trans, existingField.value);
				if (id > 0)
				{
					if (existingField.value != newValue)
					{
						// This is the clue:
						// Bind the current static member to it's old database identity,
						// so constants and enums will work with '=='
						stream.Bind(trans, newValue, id);
						// This may produce unwanted side effects if the static field object
						// was modified in the current session. TODO:Add documentation case.
						stream.Refresh(trans, newValue, int.MaxValue);
						existingField.value = newValue;
					}
					return;
				}
			}
			if (newValue == null)
			{
				try
				{
					_fieldAccessor.Set(reflectField, null, existingField.value);
				}
				catch (Exception)
				{
				}
				// fail silently
				// TODO: why?
				return;
			}
			existingField.value = newValue;
		}

		private bool StaticFieldValuesArePersisted()
		{
			return (i_config != null && i_config.StaticFieldValuesArePersisted());
		}

		protected virtual StaticField FieldByName(StaticField[] fields, string fieldName)
		{
			for (int i = 0; i < fields.Length; i++)
			{
				StaticField field = fields[i];
				if (fieldName.Equals(field.name))
				{
					return field;
				}
			}
			return null;
		}

		private StaticClass QueryStaticClass(Transaction trans)
		{
			IQuery q = trans.Container().Query(trans);
			q.Constrain(Const4.ClassStaticclass);
			q.Descend("name").Constrain(GetName());
			IObjectSet os = q.Execute();
			return os.Count > 0 ? (StaticClass)os.Next() : null;
		}

		public override string ToString()
		{
			if (i_name != null)
			{
				return i_name;
			}
			if (i_nameBytes == null)
			{
				return "*CLASS NAME UNKNOWN*";
			}
			LatinStringIO stringIO = _container == null ? Const4.stringIO : _container.StringIO
				();
			return stringIO.Read(i_nameBytes);
		}

		public override bool WriteObjectBegin()
		{
			if (!StateOK())
			{
				return false;
			}
			return base.WriteObjectBegin();
		}

		public sealed override void WriteThis(Transaction trans, ByteArrayBuffer writer)
		{
			MarshallerFamily.Current()._class.Write(trans, this, writer);
		}

		public virtual IPreparedComparison PrepareComparison(IContext context, object source
			)
		{
			return Handlers4.PrepareComparisonFor(_typeHandler, context, source);
		}

		public static void DefragObject(DefragmentContextImpl context)
		{
			ObjectHeader header = ObjectHeader.Defrag(context);
			DefragmentContextImpl childContext = new DefragmentContextImpl(context, header);
			header.ClassMetadata().Defragment(childContext);
		}

		public virtual void Defragment(IDefragmentContext context)
		{
			CorrectHandlerVersion(context).Defragment(context);
		}

		public virtual void DefragClass(DefragmentContextImpl context, int classIndexID)
		{
			MarshallerFamily mf = MarshallerFamily.ForConverterVersion(Container().ConverterVersion
				());
			mf._class.Defrag(this, _container.StringIO(), context, classIndexID);
		}

		public static Db4objects.Db4o.Internal.ClassMetadata ReadClass(ObjectContainerBase
			 stream, ByteArrayBuffer reader)
		{
			ObjectHeader oh = new ObjectHeader(stream, reader);
			return oh.ClassMetadata();
		}

		public virtual bool IsAssignableFrom(Db4objects.Db4o.Internal.ClassMetadata other
			)
		{
			return ClassReflector().IsAssignableFrom(other.ClassReflector());
		}

		public virtual void SetAncestor(Db4objects.Db4o.Internal.ClassMetadata ancestor)
		{
			if (ancestor == this)
			{
				throw new InvalidOperationException();
			}
			i_ancestor = ancestor;
		}

		public virtual object WrapWithTransactionContext(Transaction transaction, object 
			value)
		{
			if (value is int)
			{
				return value;
			}
			return new TransactionContext(transaction, value);
		}

		public virtual ITypeHandler4 TypeHandler()
		{
			return _typeHandler;
		}

		public virtual ITypeHandler4 DelegateTypeHandler(IContext context)
		{
			if (context is IHandlerVersionContext)
			{
				return CorrectHandlerVersion((IHandlerVersionContext)context);
			}
			return _typeHandler;
		}

		protected virtual ITypeHandler4 CorrectHandlerVersion(IHandlerVersionContext context
			)
		{
			ITypeHandler4 typeHandler = HandlerRegistry.CorrectHandlerVersion(context, _typeHandler
				);
			if (typeHandler != _typeHandler)
			{
				if (typeHandler is StandardReferenceTypeHandler)
				{
					((StandardReferenceTypeHandler)typeHandler).ClassMetadata(this);
				}
			}
			return typeHandler;
		}

		public virtual void ForEachField(IProcedure4 procedure)
		{
			ForEachAspect(new SubTypePredicate(typeof(FieldMetadata)), procedure);
		}

		public virtual void ForEachDeclaredField(IProcedure4 procedure)
		{
			ForEachDeclaredAspect(new SubTypePredicate(typeof(FieldMetadata)), procedure);
		}

		public virtual void ForEachAspect(IPredicate4 predicate, IProcedure4 procedure)
		{
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = this;
			while (classMetadata != null)
			{
				classMetadata.ForEachDeclaredAspect(predicate, procedure);
				classMetadata = classMetadata.i_ancestor;
			}
		}

		public virtual void ForEachAspect(IProcedure4 procedure)
		{
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = this;
			while (classMetadata != null)
			{
				classMetadata.ForEachDeclaredAspect(procedure);
				classMetadata = classMetadata.i_ancestor;
			}
		}

		public virtual void ForEachDeclaredAspect(IPredicate4 predicate, IProcedure4 procedure
			)
		{
			if (_aspects == null)
			{
				return;
			}
			for (int i = 0; i < _aspects.Length; i++)
			{
				if (predicate.Match(_aspects[i]))
				{
					procedure.Apply(_aspects[i]);
				}
			}
		}

		public virtual void ForEachDeclaredAspect(IProcedure4 procedure)
		{
			if (_aspects == null)
			{
				return;
			}
			for (int i = 0; i < _aspects.Length; i++)
			{
				procedure.Apply(_aspects[i]);
			}
		}

		public virtual bool AspectsAreNull()
		{
			return _aspects == null;
		}

		private sealed class AlwaysModified : IModificationAware
		{
			internal static readonly ClassMetadata.AlwaysModified Instance = new ClassMetadata.AlwaysModified
				();

			public bool IsModified(object obj)
			{
				return true;
			}
		}

		public virtual bool IsModified(object obj)
		{
			return _modificationChecker.IsModified(obj);
		}

		public virtual int InstanceCount()
		{
			return InstanceCount(_container.Transaction());
		}

		public virtual int InstanceCount(Transaction trans)
		{
			return _container.InstanceCount(this, trans);
		}

		public virtual bool CanHold(IReflectClass type)
		{
			IReflectClass classReflector = ClassReflector();
			if (classReflector.IsCollection())
			{
				return true;
			}
			return classReflector.IsAssignableFrom(type);
		}

		public virtual bool IsStorable()
		{
			return !StateDead() || IsTransient();
		}

		private object InstantiateWithCustomTypeHandlerIfEnabled(UnmarshallingContext context
			)
		{
			if (!_customTypeHandlerAspect.IsEnabledOn(context))
			{
				return InstantiateForVersionWithoutCustomTypeHandler(context);
			}
			return InstantiateWithCustomTypeHandler(context);
		}

		private object InstantiateForVersionWithoutCustomTypeHandler(UnmarshallingContext
			 context)
		{
			IFunction4 oldVersionConstructor = CreateConstructor(null);
			if (null == oldVersionConstructor)
			{
				throw new InvalidOperationException();
			}
			return oldVersionConstructor.Apply(context);
		}

		private object InstantiateWithCustomTypeHandler(UnmarshallingContext context)
		{
			ContextState contextState = context.SaveState();
			try
			{
				bool fieldHasValue = SeekToField(context, _customTypeHandlerAspect);
				if (!fieldHasValue)
				{
					context.RestoreState(contextState);
					return InstantiateForVersionWithoutCustomTypeHandler(context);
				}
				IInstantiatingTypeHandler customTypeHandler = (IInstantiatingTypeHandler)_customTypeHandlerAspect
					._typeHandler;
				return context.SlotFormat().DoWithSlotIndirection(context, new _IClosure4_2084(customTypeHandler
					, context));
			}
			finally
			{
				context.RestoreState(contextState);
			}
		}

		private sealed class _IClosure4_2084 : IClosure4
		{
			public _IClosure4_2084(IInstantiatingTypeHandler customTypeHandler, UnmarshallingContext
				 context)
			{
				this.customTypeHandler = customTypeHandler;
				this.context = context;
			}

			public object Run()
			{
				return customTypeHandler.Instantiate(context);
			}

			private readonly IInstantiatingTypeHandler customTypeHandler;

			private readonly UnmarshallingContext context;
		}

		public virtual bool IsStruct()
		{
			return Platform4.IsStruct(ClassReflector());
		}
	}
}
