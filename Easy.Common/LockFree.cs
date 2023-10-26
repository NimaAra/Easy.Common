namespace Easy.Common;

using System;
using System.Diagnostics;
using System.Threading;

/// <summary>
/// This is a helper class providing lock-free updates using <c>CAS</c>.
/// </summary>
public static class LockFree
{
    /// <summary>
    /// Atomically updates the variable at the given <paramref name="location"/>.
    /// </summary>
    /// <param name="location">The location of the variable to be updated</param>
    /// <param name="generator">
    /// The delegate providing the updated value.
    /// <remarks>The <paramref name="generator"/> may run more than once</remarks>
    /// </param>
    [DebuggerStepThrough]
    public static void Update(ref object location, Func<object, object> generator)
    {
        var spinner = new SpinWait();
        while (true)
        {
            var snapshot1 = location;
            var value = generator(snapshot1);
            var snapshot2 = Interlocked.CompareExchange(ref location, value, snapshot1);
            if (snapshot1.Equals(snapshot2)) { return; }
            spinner.SpinOnce();
        }
    }

    /// <summary>
    /// Atomically updates the variable at the given <paramref name="location"/>.
    /// </summary>
    /// <param name="location">The location of the variable to be updated</param>
    /// <param name="generator">
    /// The delegate providing the updated value.
    /// <remarks>The <paramref name="generator"/> may run more than once</remarks>
    /// </param>
    [DebuggerStepThrough]
    public static void Update<T>(ref T location, Func<T, T> generator) where T : class
    {
        var spinner = new SpinWait();
        while (true)
        {
            var snapshot1 = location;
            var value = generator(snapshot1);
            var snapshot2 = Interlocked.CompareExchange(ref location, value, snapshot1);
            if (snapshot1.Equals(snapshot2)) { return; }
            spinner.SpinOnce();
        }
    }

    /// <summary>
    /// Atomically updates the variable at the given <paramref name="location"/>.
    /// </summary>
    /// <param name="location">The location of the variable to be updated</param>
    /// <param name="generator">
    /// The delegate providing the updated value.
    /// <remarks>The <paramref name="generator"/> may run more than once</remarks>
    /// </param>
    [DebuggerStepThrough]
    public static void Update(ref int location, Func<int, int> generator)
    {
        var spinner = new SpinWait();
        while (true)
        {
            var snapshot1 = location;
            var value = generator(snapshot1);
            var snapshot2 = Interlocked.CompareExchange(ref location, value, snapshot1);
            if (snapshot1.Equals(snapshot2)) { return; }
            spinner.SpinOnce();
        }
    }

    /// <summary>
    /// Atomically updates the variable at the given <paramref name="location"/>.
    /// </summary>
    /// <param name="location">The location of the variable to be updated</param>
    /// <param name="generator">
    /// The delegate providing the updated value.
    /// <remarks>The <paramref name="generator"/> may run more than once</remarks>
    /// </param>
    [DebuggerStepThrough]
    public static void Update(ref long location, Func<long, long> generator)
    {
        var spinner = new SpinWait();
        while (true)
        {
            var snapshot1 = location;
            var value = generator(snapshot1);
            var snapshot2 = Interlocked.CompareExchange(ref location, value, snapshot1);
            if (snapshot1.Equals(snapshot2)) { return; }
            spinner.SpinOnce();

        }
    }

    /// <summary>
    /// Atomically updates the variable at the given <paramref name="location"/>.
    /// </summary>
    /// <param name="location">The location of the variable to be updated</param>
    /// <param name="generator">
    /// The delegate providing the updated value.
    /// <remarks>The <paramref name="generator"/> may run more than once</remarks>
    /// </param>
    [DebuggerStepThrough]
    public static void Update(ref float location, Func<float, float> generator)
    {
        var spinner = new SpinWait();
        while (true)
        {
            var snapshot1 = location;
            var value = generator(snapshot1);
            var snapshot2 = Interlocked.CompareExchange(ref location, value, snapshot1);
            if (snapshot1.Equals(snapshot2)) { return; }
            spinner.SpinOnce();

        }
    }

    /// <summary>
    /// Atomically updates the variable at the given <paramref name="location"/>.
    /// </summary>
    /// <param name="location">The location of the variable to be updated</param>
    /// <param name="generator">
    /// The delegate providing the updated value.
    /// <remarks>The <paramref name="generator"/> may run more than once</remarks>
    /// </param>
    [DebuggerStepThrough]
    public static void Update(ref double location, Func<double, double> generator)
    {
        var spinner = new SpinWait();
        while (true)
        {
            var snapshot1 = location;
            var value = generator(snapshot1);
            var snapshot2 = Interlocked.CompareExchange(ref location, value, snapshot1);
            if (snapshot1.Equals(snapshot2)) { return; }
            spinner.SpinOnce();

        }
    }
}