using UnityEngine;

public static class BallColorUtils
{
    public static Color ToUnityColor(this BallColor bc)
    {
        return new Color(
            (bc & BallColor.R) != 0 ? 1f : 0f,
            (bc & BallColor.G) != 0 ? 1f : 0f,
            (bc & BallColor.B) != 0 ? 1f : 0f
        );
    }

    public static int BitsCount(this BallColor bc)
    {
        int v = ((int)bc);
        int count = 0;
        while (v != 0) { v &= v - 1; count++; }
        return count;
    }
}
