﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE file in the project root for full license information.

using System.Windows.Threading;

public class WpfTheoryTests
{
    public static object[][] MemberDataSource => new object[][]
    {
        new object[] { 1, 2 },
        new object[] { 3, 4 },
    };

    [WpfTheory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task WpfTheory_OnSTAThread(int arg)
    {
        Assert.IsType<DispatcherSynchronizationContext>(SynchronizationContext.Current);
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        await Task.Yield();
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState()); // still there
        Assert.IsType<DispatcherSynchronizationContext>(SynchronizationContext.Current);
        Assert.True(arg == 0 || arg == 1);
    }

    [Trait("TestCategory", "FailureExpected")]
    [WpfTheory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task WpfTheoryFails(int arg)
    {
        Assert.IsType<DispatcherSynchronizationContext>(SynchronizationContext.Current);
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        await Task.Yield();
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState()); // still there
        Assert.IsType<DispatcherSynchronizationContext>(SynchronizationContext.Current);
        Assert.False(arg == 0 || arg == 1);
    }

    [WpfTheory]
    [MemberData(nameof(MemberDataSource))]
    public void MemberBasedTheory(int a, int b)
    {
        Assert.Equal(b, a + 1);
    }

    [WpfTheory, Trait("TestCategory", "FailureExpected")]
    [InlineData(1)]
    public async Task OperationCanceledException_Thrown(int a)
    {
        Assert.Equal(1, a);
        await Task.Yield();
        throw new OperationCanceledException();
    }

    [WpfTheory]
    [MemberData(nameof(NonSerializableObject.Data), MemberType = typeof(NonSerializableObject))]
    public void ThreadAffinitizedDataObject(NonSerializableObject o)
    {
        Assert.Equal(System.Diagnostics.Process.GetCurrentProcess().Id, o.ProcessId);
        Assert.Equal(Environment.CurrentManagedThreadId, o.ThreadId);
    }

    [WpfTheory, Trait("TestCategory", "FailureExpected")]
    [InlineData(0)]
    public void JustFailVoid(int a) => throw new InvalidOperationException("Expected failure " + a);
}
