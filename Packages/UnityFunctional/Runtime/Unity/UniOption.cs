using System;
using UnityEngine;

namespace Bravasoft.Functional.Unity
{
    [Serializable]
    public sealed class UniOption<T>
    {
        [SerializeField]
        private bool _hasValue;
        [SerializeField]
        private T _value;
        public Option<T> Option => _hasValue ? Prelude.Optional(_value) : Option<T>.None;

        public static implicit operator Option<T>(UniOption<T> uniOption) => uniOption.Option;
    }
}