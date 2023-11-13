using System;

namespace Bravasoft.Functional
{
    public static partial class Prelude
    {
        public static Core.NoneType None => Core.None;

        public static Option<T> Some<T>(T value) => Option<T>.Some(value);
        public static AsyncOption<T> AsyncSome<T>(T value) => AsyncOption<T>.Some(value);
        public static Option<T> Optional<T>(T value) =>
            value == null ? Option<T>.None : Option<T>.Some(value);
    }
}
