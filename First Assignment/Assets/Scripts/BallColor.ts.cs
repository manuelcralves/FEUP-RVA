using UnityEngine;

[System.Flags]
public enum BallColor
{
    None = 0,
    R = 1 << 0,
    G = 1 << 1,
    B = 1 << 2,
    Y = R | G,
    M = R | B,
    C = G | B
}

