﻿/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Threading;

#if !CF_1_0 && !CF_2_0
namespace Db4objects.Db4o.Foundation
{
    public class Lock4
    {    
        public void Awake()
        {
            Monitor.Pulse(this);
        }

        public Object Run(IClosure4 closure)
        {
            lock (this)
            {
                return closure.Run();
            }
        }
        
        public Object Run(ISafeClosure4 closure)
        {
            lock (this)
            {
                return closure.Run();
            }
        }
    
        public void Snooze(long timeout)
        {
            Monitor.Wait(this, (int)timeout);
        }
    }
}
#endif
