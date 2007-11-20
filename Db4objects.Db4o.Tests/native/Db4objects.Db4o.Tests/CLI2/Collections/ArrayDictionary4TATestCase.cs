using System;
using System.Collections.Generic;
using System.Text;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Collections;
using Db4objects.Db4o.Reflect;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI2.Collections
{
    class ArrayDictionary4TATestCase : TransparentActivationTestCaseBase
    {
        protected override void Store()
        {
            IDictionary<string, int> dict = new ArrayDictionary4<string, int>();
            ArrayDictionary4Asserter.PutData(dict);
            Store(dict);
        }

        private IDictionary<string, int> RetrieveOnlyInstance()
        {
            IDictionary<string, int> dict = (IDictionary<string, int>)RetrieveOnlyInstance(typeof(ArrayDictionary4<string, int>));
            AssertRetrievedItem(dict);
            return dict;
        }

        private object GetField(IReflector reflector, object obj, string fieldName)
        {
            IReflectClass clazz = reflector.ForObject(obj);
            IReflectField field = clazz.GetDeclaredField(fieldName);
            field.SetAccessible();

            return field.Get(obj);
        }

        private void AssertRetrievedItem(IDictionary<string, int> dict)
        {
            Assert.IsNull(GetField(Reflector(), dict, "_keys"));
            Assert.IsNull(GetField(Reflector(), dict, "_values"));
            Assert.AreEqual(default(int), GetField(Reflector(), dict, "_startIndex"));
            Assert.AreEqual(default(int), GetField(Reflector(), dict, "_endIndex"));
        }

        public void TestItemGet()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertItemGet(dict);
        }

        public void TestItemSet()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertItemSet(dict);
        }

        public void TestKeys()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertKeys(dict);
        }

        public void TestValues()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertValues(dict);
        }

        public void TestAdd()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertAdd(dict);
        }

        public void TestContainsKey()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.TestContainsKey(dict);
        }

        public void TestRemove()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertRemove(dict);
        }

        public void TestTryGetValue()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertTryGetValue(dict);
        }

        public void TestCount()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertCount(dict);
        }

        public void TestIsReadOnly()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertIsReadOnly(dict);
        }

        public void TestAddKeyValuePair()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertAddKeyValuePair(dict);
        }

        public void TestContains()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertContains(dict);
        }

        public void TestCopyTo()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertCopyTo(dict);
        }

        public void TestRemoveKeyValuePair()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertRemoveKeyValuePair(dict);
        }

        public void TestClear()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertClear(dict);
        }

        public void TestGetEnumerator()
        {
            IDictionary<string, int> dict = RetrieveOnlyInstance();
            ArrayDictionary4Asserter.AssertGetEnumerator(dict);
        }
    }
}
