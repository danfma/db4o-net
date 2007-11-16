/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System;
using System.Runtime.CompilerServices;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Db4oAdmin.Tests.TA; // MockActivator
using Db4oUnit;

public class Task : ProjectItem
{
	public bool Finished;

    public Task(string name)
        : base(name)
    {
    }

	override public bool Equals(object o)
    {
        Task other = o as Task;
        if (other == null) return false;

        // foreign field access
        return _name == other._name;
    }

    override public int GetHashCode()
    {
        return _name.GetHashCode();
    }
}

// This class is filtered out from the instrumented set
// by a custom rule.
//
// This shouldn't however affect its ability
// to activate instrumented classes.
class FilteredOutByName
{
	public static bool CheckTask(Task task)
	{
		return task.Finished;
	}
}

class TAAssemblyReferenceSubject : ITestCase
{
	public void TestFilteredOutClassesStillActivateForeignFields()
	{
		Assert.IsFalse(IsActivatable(typeof(FilteredOutByName)));

		Task t = new Task("test");
		MockActivator activator = ActivatorFor(t);

		FilteredOutByName.CheckTask(t);

		Assert.AreEqual(1, activator.Count);
	}

    public void TestIsActivatable()
    {
        Assert.IsTrue(IsActivatable(typeof(Task)));
    }

    public void TestPropertyGetter()
    {
        Task p = new Task("test");
        MockActivator activator = ActivatorFor(p);

        Assert.AreEqual(0, activator.Count);
        Assert.AreEqual("test", p.Name);
        Assert.AreEqual(1, activator.Count);
    }

    public void TestForeignFieldAccess()
    {
        Task p1 = new Task("test");
        Task p2 = new Task("test");

        MockActivator a1 = ActivatorFor(p1);
        MockActivator a2 = ActivatorFor(p2);

        Assert.IsTrue(p1.Equals(p2));

        Assert.AreEqual(1, a1.Count, "a1");
        Assert.AreEqual(1, a2.Count, "a2");
    }

    private MockActivator ActivatorFor(Task p)
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