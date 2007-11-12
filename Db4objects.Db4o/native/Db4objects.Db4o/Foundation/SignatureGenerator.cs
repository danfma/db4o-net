/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Foundation
{
    public sealed class SignatureGenerator
    {
        private static Random _random = new Random();
	
	    private static int _counter;
	
	    public static string GenerateSignature() {
            string signature = ToHexString(Environment.TickCount);
            signature += ToHexString(_random.Next());
            signature += Guid.NewGuid();
            signature += ToHexString(_counter++);
            return signature;
	    }

        private static string ToHexString(int i)
        {
            return i.ToString("X");
        }

        private static string ToHexString(long l)
        {
            return l.ToString("X");
        }
    }
}
