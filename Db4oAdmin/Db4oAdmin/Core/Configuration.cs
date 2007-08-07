﻿/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System.Diagnostics;

namespace Db4oAdmin.Core
{
	public class Configuration
	{
		private bool _caseSensitive;
		private readonly string _assemblyLocation;
		private readonly TraceSwitch _traceSwitch = new TraceSwitch("Db4oAdmin", "Db4oAdmin tracing level");
		
		public Configuration(string assemblyLocation)
		{
			_assemblyLocation = assemblyLocation;
			_traceSwitch.Level = TraceLevel.Warning;
		}

		public bool CaseSensitive
		{
			get { return _caseSensitive; }
			set { _caseSensitive = value; }
		}

		public string AssemblyLocation
		{
			get { return _assemblyLocation; }
		}

		public TraceSwitch TraceSwitch
		{
			get { return _traceSwitch; }
		}
	}
}
