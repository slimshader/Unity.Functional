using Bravasoft.Functional;
using System;
using UnityEngine;

namespace Bravasoft.Functional.Unity
{
    [Serializable]
    public class SerializableOption<T>
    {
        [SerializeField]
        private bool _hasValue;
        [SerializeField]
        private T _value;
        public Option<T> Option => _hasValue ? Prelude.Optional(_value) : Option<T>.None;

        public static implicit operator Option<T>(SerializableOption<T> serializedOption) => serializedOption.Option;
    }
}