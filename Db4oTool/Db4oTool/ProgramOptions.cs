﻿/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Collections.Generic;
using Db4oTool.Core;
using Mono.GetOptions;

namespace Db4oTool
{
	public delegate ITypeFilter TypeFilterFactory();

	public class ProgramOptions : Options
	{
		private bool _prettyVerbose;

		[Option("Preserve debugging information", "debug")]
		public bool Debug;

		[Option("Optimize predicate subclasses", "optimize-predicates")]
		public bool OptimizePredicates;

		[Option("Implement Transparent Activation Support", "ta")]
		public bool TransparentActivation;

		[Option("Enable delegate style queries for CompactFramework 2", "cf2-delegates")]
		public bool EnableCF2DelegateQueries;

		[Option("Case sensitive queries", "case-sensitive")]
		public bool CaseSensitive;

		[Option("Verbose operation mode", 'v', "verbose")]
		public bool Verbose;

		[Option("Pretty verbose operation mode", "vv")]
		public bool PrettyVerbose
		{
			get { return _prettyVerbose; }

			set { Verbose = _prettyVerbose = value; }
		}

		[Option("Fake operation mode, assembly won't be written", "fake")]
		public bool Fake;

		public List<string> CustomInstrumentations = new List<string>();

		[Option("Custom instrumentation type", "instrumentation", MaxOccurs = -1)]
		public WhatToDoNext CustomInstrumentation(string instrumentation)
		{
			CustomInstrumentations.Add(instrumentation);
			return WhatToDoNext.GoAhead;
		}

		public readonly List<TypeFilterFactory> Filters = new List<TypeFilterFactory>();

		[Option("Filter types to be instrumented by attribute", "by-attribute", MaxOccurs = -1)]
		public WhatToDoNext ByAttribute(string attribute)
		{
			Filters.Add(delegate { return new ByAttributeFilter(attribute); });
			return WhatToDoNext.GoAhead;
		}

		[Option("Custom type filter", "by-filter", MaxOccurs = -1)]
		public WhatToDoNext ByFilter(string filterType)
		{
			Filters.Add(delegate { return Factory.Instantiate<ITypeFilter>(filterType); });
			return WhatToDoNext.GoAhead;
		}

		[Option("Filter types by name (with regular expression syntax)", "by-name", MaxOccurs = -1)]
		public WhatToDoNext ByName(string name)
		{
			Filters.Add(delegate { return new ByNameFilter(name); });
			return WhatToDoNext.GoAhead;
		}

		[Option("Negates the last filter.", "not", MaxOccurs = -1)]
		public WhatToDoNext Not()
		{
			if (Filters.Count == 0) throw new InvalidOperationException("'not' must be specified after a filter");

			int lastIndex = Filters.Count - 1;
			TypeFilterFactory lastFilter = Filters[lastIndex];
			Filters[lastIndex] = delegate { return new NotFilter(lastFilter()); };
			return WhatToDoNext.GoAhead;
		}

		public string Assembly
		{
			get
			{
				if (RemainingArguments.Length != 1) return null;
				return RemainingArguments[0];
			}
		}

		public bool IsValid
		{
			get
			{
				return Assembly != null
					   && (OptimizePredicates
						   || EnableCF2DelegateQueries
						   || TransparentActivation
						   || CustomInstrumentations.Count > 0);
			}
		}

		public ProgramOptions()
		{
			this.DontSplitOnCommas = true;
		}
	}
}