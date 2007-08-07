﻿/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4oAdmin.Core;
using Db4oAdmin.Tests.Core;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Db4oAdmin.Tests.Core
{
	/// <summary>
	/// Prepends Console.WriteLine("TRACE: " + method) to every method
	/// in the assembly.
	/// </summary>
	public class TraceInstrumentation : AbstractAssemblyInstrumentation
	{
		override protected void ProcessMethod(MethodDefinition method)
		{
			if (!method.HasBody) return;
			
			MethodBody body = method.Body;
			Instruction firstInstruction = body.Instructions[0];
			CilWorker worker = body.CilWorker;
			
			// ldstr "TRACE: " + method
			worker.InsertBefore(firstInstruction,
			                    worker.Create(OpCodes.Ldstr, "TRACE: " + method));
			
			// call Console.WriteLine(string)
			MethodReference Console_WriteLine = Import(typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
			worker.InsertBefore(firstInstruction,
			                    worker.Create(OpCodes.Call, Console_WriteLine));
		}
	}

	class CustomInstrumentationTestCase : AbstractCommandLineInstrumentationTestCase
	{
		protected override string ResourceName
		{
			get { return "CustomInstrumentationSubject"; }
		}

		protected override string CommandLine
		{
			get { return "-instrumentation:Db4oAdmin.Tests.Core.TraceInstrumentation,Db4oAdmin.Tests"; }
		}
	}
}
