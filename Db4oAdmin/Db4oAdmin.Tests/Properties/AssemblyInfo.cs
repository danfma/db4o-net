﻿/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Db4oAdmin.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("db4objects Inc., San Mateo, CA, USA")]
[assembly: AssemblyProduct("db4o - database for objects")]
[assembly: AssemblyCopyright("db4o 2005")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.7.001")]

#if !CF_1_0 && !CF_2_0
[assembly: AllowPartiallyTrustedCallers]
#endif