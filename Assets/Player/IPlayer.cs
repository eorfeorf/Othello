using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IPlayer
{
    IReadOnlyReactiveProperty<GameInfo.PlayerId> Id { get; }
    void UpdateInputPut();
    void Put(int x, int y);
}
