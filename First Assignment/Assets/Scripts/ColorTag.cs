using UnityEngine;

public class ColorTag : MonoBehaviour
{
    public BallColor color;

    public void Set(BallColor bc) => color = bc;

    public int BitsCount()
    {
        int v = (int)color;
        int n = 0;
        while (v != 0) { v &= v - 1; n++; }
        return n;
    }

    public int ScoreValue()
    {
        int bits = BitsCount();
        if (bits == 1) return 1;   // R, G, B
        if (bits == 2) return 3;   // Y, M, C
        return 0;
    }

    public bool Matches(Color laser, float tol = 0.25f)
    {
        int r = laser.r > 1f - tol ? 1 : (laser.r < tol ? 0 : -1);
        int g = laser.g > 1f - tol ? 1 : (laser.g < tol ? 0 : -1);
        int b = laser.b > 1f - tol ? 1 : (laser.b < tol ? 0 : -1);

        if (r < 0 || g < 0 || b < 0)
            return false;

        BallColor laserBits = BallColor.None;
        if (r == 1) laserBits |= BallColor.R;
        if (g == 1) laserBits |= BallColor.G;
        if (b == 1) laserBits |= BallColor.B;

                Debug.Log($"[ColorTag] Ball: {color} | Laser RGB({laser.r:F2}, {laser.g:F2}, {laser.b:F2}) → LaserBits: {laserBits}");
        return laserBits == color;
    }
}