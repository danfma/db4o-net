/* Copyright (C) 2004	db4objects Inc.	  http://www.db4o.com */

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Sharpen.Lang;

namespace Sharpen
{
	public class Runtime 
	{
		public static TextWriter Out
		{
			get
			{
#if CF_1_0
				return CompactFramework1Console.Out;
#else
				return Console.Out;
#endif
			}
		}

		public static TextWriter Err
		{
			get
			{
#if CF_1_0
				return CompactFramework1Console.Error;
#else
				return Console.Error;
#endif
			}
		}

		public static object GetArrayValue(object array, int i)
	    {
	        return ((Array)array).GetValue(i);
	    }
	    
	    public static int GetArrayLength(object array)
	    {
            return ((Array) array).Length;
	    }

	    public static void SetArrayValue(object array, int index, object value)
	    {
	        ((Array)array).SetValue(value, index);
	    }

        private const BindingFlags AllMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private const BindingFlags DeclaredMembers = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private const BindingFlags DeclaredMembersIncludingStatic = DeclaredMembers | BindingFlags.Static;
		
		public static FieldInfo GetDeclaredField(Type type, string name)
		{
            return type.GetField(name, DeclaredMembersIncludingStatic);
		}

		public static FieldInfo[] GetDeclaredFields(Type type)
		{
            return type.GetFields(DeclaredMembersIncludingStatic);
		}
		
		public static MethodInfo GetDeclaredMethod(Type type, string name, Type[] parameterTypes)
		{
			return type.GetMethod(name, DeclaredMembers, null, parameterTypes, null);
		}

        public static MethodInfo GetMethod(Type type, string name, Type[] parameterTypes)
        {
            return type.GetMethod(name, AllMembers, null, parameterTypes, null);
        }

		public static Type[] GetParameterTypes(MethodBase method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			Type[] types = new Type[parameters.Length];
			for (int i=0; i<types.Length; ++i)
			{
				types[i] = parameters[i].ParameterType;
			}
			return types;
		}

		public static long CurrentTimeMillis() 
		{
			return Sharpen.Util.Date.ToJavaMilliseconds(DateTime.Now.ToUniversalTime());
		}

		public static int FloatToIntBits(float value) 
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
		}

		public static void Gc() 
		{
			System.GC.Collect();
		}
		
		public static bool EqualsIgnoreCase(string lhs, string rhs) 
		{
			return 0 == string.Compare(lhs, rhs, true);
		}

		public static string Substring(String s, int startIndex)
		{
			return s.Substring(startIndex);
		}

		public static string Substring(String s, int startIndex, int endIndex)
		{
			return s.Substring(startIndex, endIndex-startIndex);
		}

		public static char GetCharAt(string str, int index) 
		{
			return str[index];
		}

		public static void GetCharsForString(string str, int start, int end, char[] destination, int destinationStart) 
		{
			str.CopyTo(start, destination, 0, end-start);
		}

		public static byte[] GetBytesForString(string str)
		{
			return System.Text.Encoding.Default.GetBytes(str);
		}

		public static string GetStringForBytes(byte[] bytes, int index, int length)
		{
			return System.Text.Encoding.Default.GetString(bytes, index, length);
		}

		public static string GetStringValueOf(object value) 
		{
			return null == value
				? "null"
				: value.ToString();
		}

		public static String GetProperty(String key) 
		{
#if CF_1_0 || CF_2_0
			return key.Equals("line.separator") ? "\n" : null;
#else
			return key.Equals("line.separator")
				? Environment.NewLine
				: null;
#endif
		}

		public static object GetReferenceTarget(WeakReference reference) 
		{
			return reference.Target;
		}

		public static long GetTimeForDate(DateTime dateTime) 
		{
			return Sharpen.Util.Date.ToJavaMilliseconds(dateTime);
		}

		public static int IdentityHashCode(object obj) 
		{
			return IdentityHashCodeProvider.IdentityHashCode(obj);
		}

		public static float IntBitsToFloat(int value) 
		{
			return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
		}

		public static void Wait(object obj, long timeout) 
		{
#if !CF_1_0 && !CF_2_0
			Monitor.Wait(obj, (int) timeout);
#endif
		}

		public static void Notify(object obj) 
		{
#if !CF_1_0 && !CF_2_0
			Monitor.Pulse(obj);
#endif
		}

		public static void NotifyAll(object obj) 
		{
#if !CF_1_0 && !CF_2_0
			Monitor.PulseAll(obj);
#endif
		}

		public static void PrintStackTrace(Exception exception) 
		{
			PrintStackTrace(exception, Sharpen.Runtime.Out);
		}

		public static void PrintStackTrace(Exception exception, System.IO.TextWriter writer) 
		{
			writer.WriteLine(exception);
		}

		public static void RunFinalization() 
		{
			System.GC.WaitForPendingFinalizers();
		}

		public static void RunFinalizersOnExit(bool flag) 
		{
			// do nothing
		}

        internal static System.Type GetType(string typeName)
        {
            return TypeReference.FromString(typeName).Resolve();
        }
	}
}