using Bravasoft.Functional;
using System;
using UnityEngine;

[Serializable]
public class SerializedOption<T>
{
    [SerializeField]
    private bool _hasValue;
    [SerializeField]
    private T _value;
    public Option<T> Option => _hasValue ? Prelude.Optional(_value) : Option<T>.None;
}
