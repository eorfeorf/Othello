using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IInputEventProvider
{
    IReadOnlyReactiveProperty<bool> Up { get; }
    IReadOnlyReactiveProperty<bool> Down { get; }
    IReadOnlyReactiveProperty<bool> Left { get; }
    IReadOnlyReactiveProperty<bool> Right { get; }
}
