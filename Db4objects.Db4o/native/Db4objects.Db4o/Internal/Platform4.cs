﻿/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Collections;
using System.Globalization;

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Config.Attributes;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Query;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Reflect.Net;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Internal
{
    /// <exclude />
    public class Platform4
    {
        private static String[] oldAssemblyNames;

        private static byte[] assembly;

        private static ArrayList shutDownStreams;

        private static byte[][] oldAssemblies;

		public static object[] CollectionToArray(ObjectContainerBase stream, object obj)
        {
            Collection4 col = FlattenCollection(stream, obj);
            object[] ret = new object[col.Size()];
            col.ToArray(ret);
            return ret;
        }

        static Platform4()
        {
            oldAssemblyNames = new String[] { "db4o", "db4o-4.0-net1", "db4o-4.0-compact1" };
            String fullAssemblyName = typeof(Platform4).Assembly.GetName().ToString();
            String shortAssemblyName = fullAssemblyName;
            int pos = fullAssemblyName.IndexOf(",");
            if (pos > 0)
            {
                shortAssemblyName = fullAssemblyName.Substring(0, pos);
            }
           	LatinStringIO stringIO = new UnicodeStringIO();
            assembly = stringIO.Write(shortAssemblyName);
            oldAssemblies = new byte[oldAssemblyNames.Length][];
            for (int i = 0; i < oldAssemblyNames.Length; i++)
            {
                oldAssemblies[i] = stringIO.Write(oldAssemblyNames[i]);
            }
        }

        internal static bool IsDb4oClass(string className)
        {
            if (className.StartsWith("Db4objects.Db4o.Tests"))
            {
                return false;
            }
            return className.StartsWith("Db4objects.Db4o");
        }

        internal static JDK Jdk()
        {
            throw new NotSupportedException();
        }

        internal static void AddShutDownHook(Object stream, Object streamLock)
        {
            lock (typeof(Platform4))
            {
                if (shutDownStreams == null)
                {
                    shutDownStreams = new ArrayList();
#if !CF_1_0 && !CF_2_0
                    EventHandler handler = new EventHandler(OnShutDown);
                    AppDomain.CurrentDomain.ProcessExit += handler;
                    AppDomain.CurrentDomain.DomainUnload += handler;
#endif
                }
                shutDownStreams.Add(stream);
            }
        }

        internal static byte[] Serialize(Object obj)
        {
            throw new NotSupportedException();
        }

        internal static Object Deserialize(byte[] bytes)
        {
            throw new NotSupportedException();
        }

        internal static bool CanSetAccessible()
        {
            return true;
        }

        internal static IDb4oCollections Collections(Object a_object)
        {
            return new P2Collections(a_object);
        }

        internal static IReflector CreateReflector(Object config)
        {
            return new NetReflector();
        }

        internal static Object CreateReferenceQueue()
        {
            return new WeakReferenceHandlerQueue();
        }

        public static Object CreateWeakReference(Object obj)
        {
            return new WeakReference(obj, false);
        }

        internal static Object CreateYapRef(Object referenceQueue, Object yapObject, Object obj)
        {
            return new WeakReferenceHandler(referenceQueue, yapObject, obj);
        }

        internal static long DoubleToLong(double a_double)
        {
#if CF_1_0 || CF_2_0
            byte[] bytes = BitConverter.GetBytes(a_double);
            return BitConverter.ToInt64(bytes, 0);
#else
            return BitConverter.DoubleToInt64Bits(a_double);
#endif
        }

        internal static QConEvaluation EvaluationCreate(Transaction a_trans, Object example)
        {
            if (example is IEvaluation || example is EvaluationDelegate)
            {
                return new QConEvaluation(a_trans, example);
            }
            return null;
        }

        internal static void EvaluationEvaluate(Object a_evaluation, ICandidate a_candidate)
        {
            IEvaluation eval = a_evaluation as IEvaluation;
            if (eval != null)
            {
                eval.Evaluate(a_candidate);
            }
            else
            {
                // use starting _ for PascalCase conversion purposes
                EvaluationDelegate _ed = a_evaluation as EvaluationDelegate;
                if (_ed != null)
                {
                    _ed(a_candidate);
                }
            }
        }

        internal static Config4Class ExtendConfiguration(IReflectClass clazz, IConfiguration config, Config4Class classConfig)
        {
            Type t = GetNetType(clazz);
            if (t == null)
            {
                return classConfig;
            }
            ConfigurationIntrospector a = new ConfigurationIntrospector(t, classConfig, config);
            a.Apply();
            return a.ClassConfiguration;
        }

		internal static Collection4 FlattenCollection(ObjectContainerBase stream, Object obj)
        {
            Collection4 collection41 = new Collection4();
            FlattenCollection1(stream, obj, collection41);
            return collection41;
        }

		internal static void FlattenCollection1(ObjectContainerBase stream, Object obj, Collection4 collection4)
        {
            Array arr = obj as Array;
            if (arr != null)
            {
                IReflectArray reflectArray = stream.Reflector().Array();

                Object[] flat = new Object[arr.Length];

                reflectArray.Flatten(obj, reflectArray.Dimensions(obj), 0, flat, 0);
                for (int i = 0; i < flat.Length; i++)
                {
                    FlattenCollection1(stream, flat[i], collection4);
                }
            }
            else
            {
                // If obj implements IEnumerable, add all elements to collection4
                IEnumerator enumerator = GetCollectionEnumerator(obj, true);

                // Add elements to collection if conversion was succesful
                if (enumerator != null)
                {
                    if (enumerator is IDictionaryEnumerator)
                    {
                        IDictionaryEnumerator dictEnumerator = enumerator as IDictionaryEnumerator;
                        while (enumerator.MoveNext())
                        {
                            FlattenCollection1(stream, dictEnumerator.Key, collection4);
                        }
                    }
                    else
                    {
                        while (enumerator.MoveNext())
                        {
                            // recursive call to flatten Collections in Collections
                            FlattenCollection1(stream, enumerator.Current, collection4);
                        }
                    }
                }
                else
                {
                    // If obj is not a Collection, it still needs to be collected.
                    collection4.Add(obj);
                }
            }
        }

        internal static void ForEachCollectionElement(Object obj, IVisitor4 visitor)
        {
            IEnumerator enumerator = GetCollectionEnumerator(obj, false);
            if (enumerator != null)
            {
                // If obj is a map (IDictionary in .NET speak) call Visit() with the key
                // otherwise use the element itself
                if (enumerator is IDictionaryEnumerator)
                {
                    IDictionaryEnumerator dictEnumerator = enumerator as IDictionaryEnumerator;
                    while (enumerator.MoveNext())
                    {
                        visitor.Visit(dictEnumerator.Key);
                    }
                }
                else
                {
                    while (enumerator.MoveNext())
                    {
                        visitor.Visit(enumerator.Current);
                    }
                }
            }
        }

        internal static String Format(Sharpen.Util.Date date, bool showSeconds)
        {
            String fmt = "yyyy-MM-dd";
            if (showSeconds)
            {
                fmt += " HH:mm:ss";
            }
            return new DateTime(date.GetTicks()).ToString(fmt);
        }

        public static Object GetClassForType(Object obj)
        {
            Type t = obj as Type;
            if (t != null)
            {
                return t;
            }
            return obj;
        }

        internal static IEnumerator GetCollectionEnumerator(object obj, bool allowArray)
        {
			IEnumerable enumerable = obj as IEnumerable;
			if (enumerable == null) return null;
		    if (obj is string) return null;
            if (!allowArray && obj is Array) return null;
		    return enumerable.GetEnumerator();
		}

        internal static void GetDefaultConfiguration(Config4Impl config)
        {
            if (IsCompact())
            {
                config.SingleThreadedClient(true);
                config.WeakReferenceCollectionInterval(0);
            }

            Translate(config, typeof(Delegate), new TNull());
            Translate(config, typeof(Type), new TType());

            if (IsMono())
            {
                Translate(config, "System.MonoType, mscorlib", new TType());
            }
            else
            {
                Translate(config, "System.RuntimeType, mscorlib", new TType());
            }

            Translate(config, new ArrayList(), new TList());
            Translate(config, new Hashtable(), new TDictionary());
            Translate(config, new Queue(), new TQueue());
            Translate(config, new Stack(), new TStack());
			Translate(config, CultureInfo.InvariantCulture, new TCultureInfo());

            if (!IsCompact())
            {
                Translate(config, "System.Collections.SortedList, mscorlib", new TDictionary());
            }
        }

        public static bool IsCompact()
        {
#if CF_1_0 || CF_2_0
			return true;
#else
            return false;
#endif
        }

        internal static bool IsMono()
        {
            return null != Type.GetType("System.MonoType, mscorlib");
        }

        public static Object GetTypeForClass(Object obj)
        {
            return obj;
        }

        internal static Object GetYapRefObject(Object obj)
        {
			WeakReferenceHandler refHandler = obj as WeakReferenceHandler;
			if (refHandler != null)
            {
				return refHandler.Get();
            }
            return obj;
        }

        internal static bool HasCollections()
        {
            return true;
        }

        internal static bool HasLockFileThread()
        {
            return false;
        }

        internal static bool HasNio()
        {
            return false;
        }

        internal static bool HasWeakReferences()
        {
            return true;
        }

        internal static bool IgnoreAsConstraint(Object obj)
        {
            Type t = obj.GetType();
            if (t.IsEnum)
            {
                if (System.Convert.ToInt32(obj) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsCollectionTranslator(Config4Class config4class)
        {
            if (config4class != null)
            {
                IObjectTranslator ot = config4class.GetTranslator();
                if (ot != null)
                {
                    return ot is TList || ot is TDictionary || ot is TQueue || ot is TStack;
                }
            }
            return false;
        }

        public static bool IsConnected(Sharpen.Net.Socket socket)
        {
            if (socket == null)
            {
                return false;
            }
            return socket.IsConnected();
        }

        internal static bool IsValueType(IReflectClass claxx)
        {
            if (claxx == null)
            {
                return false;
            }
            NetClass netClass = claxx.GetDelegate() as NetClass;
            if (netClass == null)
            {
                return false;
            }
            return netClass.GetNetType().IsValueType;
        }

        internal static void KillYapRef(Object obj)
        {
			WeakReferenceHandler yr = obj as WeakReferenceHandler;
            if (yr != null)
            {
                yr.ObjectReference = null;
            }
        }

        internal static double LongToDouble(long l)
        {
#if CF_1_0 || CF_2_0
            byte[] bytes = BitConverter.GetBytes(l);
            return BitConverter.ToDouble(bytes, 0);
#else
            return BitConverter.Int64BitsToDouble(l);
#endif
        }

        internal static void LockFile(object raf)
        {
            // do nothing. C# RAF is locked automatically upon opening
        }

        internal static void UnlockFile(object randomaccessfile)
        {
            // do nothing. C# RAF is unlocked automatically upon closing
        }

        internal static void MarkTransient(String marker)
        {
            NetField.MarkTransient(marker);
        }

        internal static bool CallConstructor()
        {
            return false;
        }

        internal static void PollReferenceQueue(Object stream, Object referenceQueue)
        {
            ((WeakReferenceHandlerQueue)referenceQueue).Poll((IExtObjectContainer)stream);
        }

        internal static void PostOpen(IObjectContainer objectContainer)
        {
        }

        internal static void PreClose(IObjectContainer objectContainer)
        {
        }

        public static void RegisterCollections(GenericReflector reflector)
        {
            reflector.RegisterCollectionUpdateDepth(
                typeof(IDictionary),
                3);
        }

        internal static void RemoveShutDownHook(Object yapStream, Object streamLock)
        {
            lock (typeof(Platform4))
            {
                if (shutDownStreams != null && shutDownStreams.Contains(yapStream))
                {
                    shutDownStreams.Remove(yapStream);
                }
            }
        }

        public static void SetAccessible(Object obj)
        {
            // do nothing
        }

        private static void OnShutDown(object sender, EventArgs args)
        {
            lock (typeof(Platform4))
            {
				for (int i = 0; i < shutDownStreams.Count; i++)
				{
					Unobfuscated.ShutDownHookCallback(shutDownStreams[i]);
				}
            }
        }

        public static bool StoreStaticFieldValues(IReflector reflector, IReflectClass clazz)
        {
            return false;
        }


        private static void Translate(Config4Impl config, object obj, IObjectTranslator translator)
        {
            try
            {
                config.ObjectClass(obj).Translate(translator);
            }
            catch
            {
                // TODO: why the object is being logged instead of the error?
                Unobfuscated.LogErr(config, 48, obj.ToString(), null);
            }
        }

        internal static byte[] UpdateClassName(byte[] bytes)
        {
            for (int i = 0; i < oldAssemblyNames.Length; i++)
            {
                int j = oldAssemblies[i].Length - 1;
                for (int k = bytes.Length - 1; k >= 0; k--)
                {
                    if (bytes[k] != oldAssemblies[i][j])
                    {
                        break;
                    }
                    j--;
                    if (j < 0)
                    {
                        int keep = bytes.Length - oldAssemblies[i].Length;
                        byte[] result = new byte[keep + assembly.Length];
                        Array.Copy(bytes, 0, result, 0, keep);
                        Array.Copy(assembly, 0, result, keep, assembly.Length);
                        return result;
                    }
                }
            }
            return bytes;
        }

        public static Object WeakReferenceTarget(Object weakRef)
        {
            WeakReference wr = weakRef as WeakReference;
            if (wr != null)
            {
                return wr.Target;
            }
            return weakRef;
        }

        internal static object WrapEvaluation(object evaluation)
        {
#if CF_1_0 || CF_2_0
			// FIXME: How to better support EvaluationDelegate on the CompactFramework?
			return evaluation;
#else
            return (evaluation is EvaluationDelegate)
                ? new EvaluationDelegateWrapper((EvaluationDelegate)evaluation)
                : evaluation;
#endif
        }

        internal static bool IsTransient(IReflectClass clazz)
        {
            Type type = GetNetType(clazz);
            if (null == type) return false;
            return type.IsPointer
                || type.IsSubclassOf(typeof(Delegate));
        }

        private static Type GetNetType(IReflectClass clazz)
        {
            if (null == clazz)
            {
                return null;
            }

            NetClass netClass = clazz as NetClass;
            if (null != netClass)
            {
                return netClass.GetNetType();
            }
            IReflectClass claxx = clazz.GetDelegate();
            if (claxx == clazz)
            {
                return null;
            }
            return GetNetType(claxx);
        }

		internal static NetTypeHandler[] Types(ObjectContainerBase stream)
        {
			return new NetTypeHandler[]
				{
					//new DoubleHandler(stream),
					new SByteHandler(stream),
					new DecimalHandler(stream),
					new UIntHandler(stream),
					new ULongHandler(stream),
					new UShortHandler(stream),
					new DateTimeHandler(stream),
				};
        }

        public static bool IsSimple(Type a_class)
        {
            for (int i1 = 0; i1 < SIMPLE_CLASSES.Length; i1++)
            {
                if (a_class == SIMPLE_CLASSES[i1])
                {
                    return true;
                }
            }
            return false;
        }

        private static Type[] SIMPLE_CLASSES = {
		                                        	typeof(Int32),
		                                        	typeof(Int64),
		                                        	typeof(Single),
		                                        	typeof(Boolean),
		                                        	typeof(Double),
		                                        	typeof(Byte),
		                                        	typeof(Char),
		                                        	typeof(Int16),
		                                        	typeof(String),
		                                        	typeof(Sharpen.Util.Date)
		                                        };
    }
}