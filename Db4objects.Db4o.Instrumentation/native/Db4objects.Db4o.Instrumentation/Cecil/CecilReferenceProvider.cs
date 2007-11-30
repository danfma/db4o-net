using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Db4objects.Db4o.Instrumentation.Api;
using Mono.Cecil;

namespace Db4objects.Db4o.Instrumentation.Cecil
{
	public class CecilReferenceProvider : IReferenceProvider
	{
		private static readonly object AnnotationKey = new object();

		public static CecilReferenceProvider ForModule(ModuleDefinition module)
		{
			System.Collections.IDictionary annotations = Annotations(module);
			CecilReferenceProvider provider = (CecilReferenceProvider) annotations[AnnotationKey];
			if (null == provider)
			{
				provider = new CecilReferenceProvider(module);
				annotations[AnnotationKey] = provider;
			}
			return provider;
		}

		private static IDictionary Annotations(ModuleDefinition module)
		{
			return (module as IAnnotationProvider).Annotations;
		}

		private readonly ModuleDefinition _module;
		private readonly Dictionary<TypeReference, ITypeRef> _typeCache = new Dictionary<TypeReference, ITypeRef>();

		private CecilReferenceProvider(ModuleDefinition module)
		{
			if (null == module) throw new ArgumentNullException();
			_module = module;
		}

		public ITypeRef ForType(Type type)
		{
			return ForCecilType(_module.Import(type));
		}

		public ITypeRef ForCecilType(TypeReference type)
		{
			ITypeRef typeRef;
			if (!_typeCache.TryGetValue(type, out typeRef))
			{
				typeRef = new CecilTypeRef(this, type);
				_typeCache.Add(type, typeRef);
			}
			return typeRef;
		}

		public IMethodRef ForMethod(MethodInfo method)
		{
			return new CecilMethodRef(this, _module.Import(method));
		}

		public IMethodRef ForMethod(ITypeRef declaringType, string methodName, ITypeRef[] parameterTypes, ITypeRef returnType)
		{
			throw new NotImplementedException();
		}

		public IFieldRef ForCecilField(FieldReference field)
		{
			return new CecilFieldRef(this, field);
		}
	}
}