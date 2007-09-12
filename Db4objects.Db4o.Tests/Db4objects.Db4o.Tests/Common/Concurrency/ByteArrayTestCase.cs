/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Concurrency;
using Db4objects.Db4o.Tests.Common.Persistent;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class ByteArrayTestCase : Db4oClientServerTestCase
	{
		internal const int ITERATIONS = 15;

		internal const int INSTANCES = 2;

		internal const int ARRAY_LENGTH = 1024 * 512;

		public static void Main(string[] args)
		{
			new ByteArrayTestCase().RunConcurrency();
		}

		#if !CF_1_0 && !CF_2_0
		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(SerializableByteArrayHolder)).Translate(new TSerializable
				());
		}
		#endif // !CF_1_0 && !CF_2_0

		protected override void Store()
		{
			for (int i = 0; i < INSTANCES; ++i)
			{
				Store(new ByteArrayHolder(CreateByteArray()));
				Store(new SerializableByteArrayHolder(CreateByteArray()));
			}
		}

		public virtual void ConcByteArrayHolder(IExtObjectContainer oc)
		{
			TimeQueryLoop(oc, "raw byte array", typeof(ByteArrayHolder));
		}

		public virtual void ConcSerializableByteArrayHolder(IExtObjectContainer oc)
		{
			TimeQueryLoop(oc, "TSerializable", typeof(SerializableByteArrayHolder));
		}

		private void TimeQueryLoop(IExtObjectContainer oc, string label, Type clazz)
		{
			for (int i = 0; i < ITERATIONS; ++i)
			{
				IQuery query = oc.Query();
				query.Constrain(clazz);
				IObjectSet os = query.Execute();
				Assert.AreEqual(INSTANCES, os.Size());
				while (os.HasNext())
				{
					Assert.AreEqual(ARRAY_LENGTH, ((IIByteArrayHolder)os.Next()).GetBytes().Length);
				}
			}
		}

		internal virtual byte[] CreateByteArray()
		{
			byte[] bytes = new byte[ARRAY_LENGTH];
			for (int i = 0; i < bytes.Length; ++i)
			{
				bytes[i] = (byte)(i % 256);
			}
			return bytes;
		}
	}
}