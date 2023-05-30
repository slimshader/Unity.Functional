using System;

namespace Bravasoft.Functional
{
    public readonly struct Behavior<T>
    {
        private readonly Func<float, T> _f;

        public T Run(float t) => _f(t);

        public Behavior<U> Map<U>(Func<T, U> map)
        {
            var f = _f;
            return Behavior.Create(t => map(f(t)));
        }

        public Behavior<U> Bind<U>(Func<T, Behavior<U>> bind)
        {
            var f = _f;
            return Behavior.Create(t => bind(f(t)).Run(t));
        }

        public Behavior(Func<float, T> f)
        {
            _f = f ?? throw new ArgumentNullException(nameof(f));
        }
    }

    public static class Behavior
    {
        public static Behavior<float> Time() => new Behavior<float>(x => x);
        public static Behavior<T> Const<T>(T value) => new Behavior<T>(_ => value);
        public static Behavior<T> Create<T>(Func<float, T> f) => new Behavior<T>(f);
        public static Behavior<U> Select<T, U>(this Behavior<T> behavior, Func<T, U> f) => behavior.Map(f);
        public static Behavior<U> SelectMany<T, T1, U>(this Behavior<T> behavior, Func<T, Behavior<T1>> binder, Func<T, T1, U> selector) =>
            behavior.Bind(t => binder(t).Map(t1 => selector(t, t1)));
    }
}
