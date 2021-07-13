using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerInputEventProvider : MonoBehaviour, IInputEventProvider
{
    public IReadOnlyReactiveProperty<bool> Up => up;
    public IReadOnlyReactiveProperty<bool> Down => down;
    public IReadOnlyReactiveProperty<bool> Left => left;
    public IReadOnlyReactiveProperty<bool> Right => right;

    private readonly ReactiveProperty<bool> up = new ReactiveProperty<bool>();
    private readonly ReactiveProperty<bool> down = new ReactiveProperty<bool>();
    private readonly ReactiveProperty<bool> left = new ReactiveProperty<bool>();
    private readonly ReactiveProperty<bool> right = new ReactiveProperty<bool>();

    private void Update()
    {
        up.Value = Input.GetKeyDown(KeyCode.UpArrow); 
        down.Value = Input.GetKeyDown(KeyCode.DownArrow); 
        left.Value = Input.GetKeyDown(KeyCode.LeftArrow); 
        right.Value = Input.GetKeyDown(KeyCode.RightArrow); 
    }
}