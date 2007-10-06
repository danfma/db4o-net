using System;
using Db4objects.Db4o.Tests.Common.Handlers;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI1.Handlers
{
    class DateTimeHandlerUpdateTestCase : HandlerUpdateTestCaseBase
    {
        public class Item
        {
            public DateTime _dateTime;

            public Object _untyped;

#if NET_2_0 || CF_2_0
			public DateTime? _nullableDateTime;
#endif
        }

        public class ItemArrays
        {
            public DateTime[] _dateTimeArray;

            public object[] _untypedObjectArray;

            public object _arrayInObject;

#if NET_2_0 || CF_2_0
			public DateTime?[] _nullableDateTimeArray;
#endif
        }

        private static readonly DateTime[] data = new DateTime[] {new DateTime(DateTime.MinValue.Ticks),
            new DateTime(DateTime.MinValue.Ticks + 1), new DateTime(5), 
            new DateTime(DateTime.MaxValue.Ticks - 1), new DateTime(DateTime.MaxValue.Ticks),
        };

        public static void Main(string[] args)
        {
            new TestRunner(typeof(DateHandlerUpdateTestCase)).Run();
        }

        protected override void AssertArrays(object obj)
        {
            ItemArrays itemArrays = (ItemArrays)obj;
            DateTime[] dateTimeArray = (DateTime[])itemArrays._arrayInObject;
            for (int i = 0; i < data.Length; i++)
            {
                AssertAreEqual(data[i], itemArrays._dateTimeArray[i]);
                AssertAreEqual(data[i], (DateTime) itemArrays._untypedObjectArray[i]);
                AssertAreEqual(data[i], dateTimeArray[i]);
                //FIXME: Cannot retrieve nullable struct array.
                //AssertAreEqual(data[i], (DateTime)itemArrays._nullableDateTimeArray[i]);
            }

            Assert.IsNull(itemArrays._untypedObjectArray[data.Length]);
            AssertAreEqual(new DateTime(0), itemArrays._dateTimeArray[data.Length]);
            AssertAreEqual(new DateTime(0), dateTimeArray[data.Length]);
            //FIXME: Cannot retrieve nullable struct array.
            //Assert.IsNull(itemArrays._nullableDateTimeArray[data.Length]);
        }

        protected override void AssertValues(object[] values)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Item item = (Item)values[i];
                AssertAreEqual(data[i], item._dateTime);
                AssertAreEqual(data[i], (DateTime) item._untyped);
#if NET_2_0 || CF_2_0
                AssertAreEqual(data[i], (DateTime) item._nullableDateTime);
#endif
				}

            Item nullItem = (Item) values[values.Length - 1];

            AssertAreEqual(new DateTime(0), nullItem._dateTime);
            Assert.IsNull(nullItem._untyped);
#if NET_2_0 || CF_2_0
            Assert.IsNull(nullItem._nullableDateTime);
#endif
			}

        private void AssertAreEqual(DateTime expected, DateTime actual)
        {
            Assert.AreEqual(expected, actual);
        }

        protected override object CreateArrays()
        {
            ItemArrays itemArrays = new ItemArrays();
            itemArrays._dateTimeArray = new DateTime[data.Length + 1];
            System.Array.Copy(data, 0, itemArrays._dateTimeArray, 0, data.Length);

            itemArrays._untypedObjectArray = new object[data.Length + 1];
            System.Array.Copy(data, 0, itemArrays._untypedObjectArray, 0, data.Length);

            DateTime[] dateTimeArray = new DateTime[data.Length + 1];
            System.Array.Copy(data, 0, dateTimeArray, 0, data.Length);
            itemArrays._arrayInObject = dateTimeArray;

#if NET_2_0 || CF_2_0
            itemArrays._nullableDateTimeArray = new DateTime?[data.Length + 1];
            for (int i = 0; i < data.Length; i++)
            {
                itemArrays._nullableDateTimeArray[i] = data[i];
            }
#endif
			return itemArrays;
        }

        protected override object[] CreateValues()
        {
            Item[] values = new Item[data.Length + 1];
            for (int i = 0; i < data.Length; i++)
            {
                Item item = new Item();
                item._dateTime = data[i];
                item._untyped = data[i];
#if NET_2_0 || CF_2_0
                item._nullableDateTime = data[i];
#endif
				values[i] = item;
            }
            values[values.Length - 1] = new Item();
            return values;
        }

        protected override string TypeName()
        {
            return "datetime";
        }
    }
}
