/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using Sharpen.Lang;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o {

   internal class P2Collections : IDb4oCollections {

      internal Transaction _transaction;
      
      internal P2Collections(Transaction transaction) : base() {
          _transaction = transaction;
      }
      
      public IDb4oList NewLinkedList() {
          lock(Lock()){
              if (Unobfuscated.CreateDb4oList(Container())){
                  IDb4oList l = new P2LinkedList();
                  Container().Set(_transaction, l);
                  return l;
              }
              return null;
          }
      }
      
      public IDb4oMap NewHashMap(int size) {
          lock(Lock()){
              if (Unobfuscated.CreateDb4oList(Container())){
                  return new P2HashMap(size);
              }
              return null;
          }
      }

       public IDb4oMap NewIdentityHashMap(int size) {
           lock(Lock()){
               if(Unobfuscated.CreateDb4oList(Container())){
                   P2HashMap m = new P2HashMap(size);
                   m.i_type = 1;
                   Container().Set(_transaction, m);
                   return m;
               }
               return null;
           }
       }

    private Object Lock(){
        return Container().Lock();
    }
    
    private ObjectContainerBase Container(){
        return _transaction.Stream();
    }


   }
}