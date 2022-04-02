using System.Collections.Concurrent;

namespace Earthworm;

internal static class Memoization
{
    public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> f)
        where T : notnull
    {
        var cache = new ConcurrentDictionary<T, TResult>();
        return x => cache.GetOrAdd(x, _ => f(x));
    }

    private static Func<Tuple<T1, T2>, TResult> Tuplify<T1, T2, TResult>(this Func<T1, T2, TResult> f)
    {
        return t => f(t.Item1, t.Item2);
    }

    private static Func<T1, T2, TResult> UnTuplify<T1, T2, TResult>(this Func<Tuple<T1, T2>, TResult> f)
    {
        return (x1, x2) => f(Tuple.Create(x1, x2));
    }

    public static Func<T1, T2, TResult> Memoize<T1, T2, TResult>(this Func<T1, T2, TResult> f)
    {
        return f.Tuplify().Memoize().UnTuplify();
    }

    private static Func<Tuple<T1, T2, T3>, TResult> Tuplify<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> f)
    {
        return t => f(t.Item1, t.Item2, t.Item3);
    }

    private static Func<T1, T2, T3, TResult> UnTuplify<T1, T2, T3, TResult>(this Func<Tuple<T1, T2, T3>, TResult> f)
    {
        return (x1, x2, x3) => f(Tuple.Create(x1, x2, x3));
    }

    public static Func<T1, T2, T3, TResult> Memoize<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> f)
    {
        return f.Tuplify().Memoize().UnTuplify();
    }
}
