using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : ScriptableObject
{
    public const int Length = 8;
    public const int Half = 4;
    public const float HalfAdjust = 0.5f;

    public enum Dir
    {
        U,
        UR,
        R,
        DR,
        D,
        DL,
        L,
        UL,
        Max
    }
    
    public enum PlayerId
    {
        None,
        First,
        Second
    }

    public enum InputDir
    {
        U,R,D,L,Max
    }

    [Serializable]
    public struct InputConfig
    {
        public KeyCode Up;
        public KeyCode Right;
        public KeyCode Left;
        public KeyCode Down;
        public KeyCode Put;
    }
}