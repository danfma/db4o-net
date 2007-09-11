﻿/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System;
using System.Runtime.CompilerServices;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Db4oUnit;

public abstract class Named
{
	protected string _name;

	public Named(string name)
	{
		_name = name;
	}

	public string Name
	{
		get { return _name;  }
	}
}

public class ProjectItem : Named
{
	public ProjectItem(string name) : base(name)
	{
	}

	override public bool Equals(object o)
	{
		ProjectItem other = o as ProjectItem;
		if (other == null) return false;

		// foreign field access
		return _name == other._name;
	}

	override public int GetHashCode()
	{
		return _name.GetHashCode();
	}
}

class Project : ProjectItem
{
	public Project(string name) : base(name)
	{
	}

	public void UseByRef()
	{
		ByRef(ref _name);
	}

	private void ByRef(ref string name)
	{
	}
}

struct AValueType
{	
}

[CompilerGenerated]
class CompilerGeneratedType
{
}

class MockActivator : IActivator
{
	private int _count;

	public int Count
	{
		get { return _count;  }
	}

	public void Activate()
	{
		++_count;
	}
}

class TAInstrumentationSubject : ITestCase
{
	public void TestIsActivatable()
	{
        Assert.IsTrue(IsActivatable(typeof(Named)));
        Assert.IsTrue(IsActivatable(typeof(ProjectItem)));
		Assert.IsTrue(IsActivatable(typeof(Project)));
	}

	public void TestPropertyGetter()
	{
		Project p = new Project("test");
		MockActivator activator = ActivatorFor(p);

		Assert.AreEqual(0, activator.Count);
		Assert.AreEqual("test", p.Name);
		Assert.AreEqual(1, activator.Count);
	}

	public void TestForeignFieldAccess()
	{
		Project p1 = new Project("test");
		Project p2 = new Project("test");

		MockActivator a1 = ActivatorFor(p1);
		MockActivator a2 = ActivatorFor(p2);

		Assert.IsTrue(p1.Equals(p2));

		Assert.AreEqual(1, a1.Count);
		Assert.AreEqual(1, a2.Count);
	}

	public void TestFieldByRef()
	{
		Project p1 = new Project("test");
		MockActivator a1 = ActivatorFor(p1);

		p1.UseByRef();

		Assert.AreEqual(1, a1.Count);
	}

	public void TestValueTypesAreNotInstrumented()
	{
		Assert.IsFalse(IsActivatable(typeof(AValueType)));
	}

	public void TestCompilerGeneratedClassesAreNotInstrumented()
	{
		Assert.IsFalse(IsActivatable(typeof(CompilerGeneratedType)));
	}

	private MockActivator ActivatorFor(Project p)
	{
		MockActivator activator = new MockActivator();
		((IActivatable)p).Bind(activator);
		return activator;
	}

	private static bool IsActivatable(Type type)
	{
		return typeof(IActivatable).IsAssignableFrom(type);
	}
}