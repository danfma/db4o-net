﻿using System;
using System.Collections.Generic;
using System.Collections;

using Db4objects.Db4o.Tests.Common.Handlers;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI2.Handlers
{
    class GenericListVersionUpdateTestCase : HandlerUpdateTestCaseBase
    {
        class Item<T>
        {
            public IList<T> list;
            public object untypedGenericList;
            public IList<SimpleItem> simpleItemList;

            public Item(IList<T> list_, object untypedGenericList_, IList<SimpleItem> simpleItemList_)
            {
                list = list_;
                untypedGenericList = untypedGenericList_;
                simpleItemList = simpleItemList_;
            }
        }

        class ItemArray
        {
            public IList<int>[] arrayOfIntList;
            public IList<SimpleItem>[] arrayOfSimpleItemList;
            public object genericListArrayInObject;
            public object[] genericListArrayInObjectArray;

            public ItemArray(
                        IList<int>[] arrayOfIntList_,
                        IList<SimpleItem>[] arrayOfSimpleItemList_,
                        object genericListArrayInObject_,
                        object[] genericListArrayInObjectArray_)
            {
                arrayOfIntList = arrayOfIntList_;
                arrayOfSimpleItemList = arrayOfSimpleItemList_;
                genericListArrayInObject = genericListArrayInObject_;
                genericListArrayInObjectArray = genericListArrayInObjectArray_;
            }
        }

        sealed class SimpleItem
        {
            public int foo;

            public SimpleItem(int foo_)
            {
                foo = foo_;
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                
                if (obj.GetType() != this.GetType()) return false;

                SimpleItem item = (SimpleItem) obj;
                return item.foo == foo;
            }
        }

        private static IList<int> intList1()
        {
            return new List<int>(new int[] { Int32.MinValue, 0, Int32.MaxValue });
        }

        private static IList<int> intList2()
        {
            return new List<int>(new int[] { 1, 2, 3 });
        }

        private static IList<SimpleItem> simpleItemList1()
        {
            return new List<SimpleItem>(new SimpleItem[] { new SimpleItem(100), new SimpleItem(200)});
        }

        private static IList<SimpleItem> simpleItemList2()
        {
            return new List<SimpleItem>(new SimpleItem[] { new SimpleItem(-1), new SimpleItem(42)}); 
        }

        private static IList<SimpleItem> simpleItemEmptyList()
        {
            return new List<SimpleItem>();
        }

        private static IList<string> stringList1()
        {
            return new List<string>(new string[] {"Adriano", null, "Norberto", String.Empty});
        }

        private static IList<string> stringList2()
        {
            return new List<string>(new string[] {"Foo", "Bar", String.Empty});
        }

        protected override string TypeName()
        {
            return "Generic List Version Update";
        }

        protected override object[] CreateValues()
        {
            return new object[2] {
                                    new Item<int>( intList1(), intList2(), null),
                                    new Item<string>(stringList1(), stringList2(), simpleItemList1())
                        };
        }

        protected override object CreateArrays()
        {
            IList<int>[] intList = new IList<int>[] { intList1() , intList2() };

            IList<SimpleItem>[] simpleItemList = new IList<SimpleItem>[] 
                                                 {
                                                     simpleItemList1(), 
                                                     null,
                                                     simpleItemList2(),
                                                     simpleItemEmptyList(),
                                                 };
            
            return new ItemArray(intList, simpleItemList, simpleItemList, intList);
        }

        protected override void AssertValues(object[] values)
        {
            AssertItem((Item<int>)values[0], intList1(), intList2(), null);
            AssertItem((Item<string>)values[1], stringList1(), stringList2(), simpleItemList1());
        }

        private void AssertItem<T>(Item<T> tba, IList<T> list, IList<T> untypedGenericList, IList<SimpleItem> simpleItemList)
        {
            Assert.IsNotNull(tba);
            AssertList(list, tba.list);
            AssertList(untypedGenericList, tba.untypedGenericList as IList<T>);
            AssertList(simpleItemList, tba.simpleItemList);
        }

        private void AssertList<T,S>(IList<T> expected, IList<S> actual)
        {
            if (expected != null)
            {
                Assert.IsNotNull(actual);
                Iterator4Assert.AreEqual(expected.GetEnumerator(), actual.GetEnumerator());
            }
            else
            {
                Assert.IsNull(actual);
            }
        }

        protected override void AssertArrays(object obj)
        {
            ItemArray itemArray = obj as ItemArray;
            Assert.IsNotNull(itemArray);

            AssertArrayList(
                        new IList<int>[] { intList1(), intList2() }, 
                        itemArray.arrayOfIntList);

            AssertArrayList(
                    new IList<SimpleItem>[] { simpleItemList1(), null,  simpleItemList2(), simpleItemEmptyList()},
                    itemArray.arrayOfSimpleItemList);

            AssertArrayList(
                        new IList<SimpleItem>[] { simpleItemList1(), null, simpleItemList2(), simpleItemEmptyList() },
                        (IList<SimpleItem>[]) itemArray.genericListArrayInObject);

            AssertArrayList(
                        new IList<int>[] { intList1(), intList2() },
                        (IList<int>[]) itemArray.genericListArrayInObjectArray);
        }

        private void AssertArrayList<T>(IList<T>[] expected, IList<T>[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for(int i=0; i < expected.Length; i++)
            {
                AssertList(expected[i], actual[i]);
            }
        }
    }
}