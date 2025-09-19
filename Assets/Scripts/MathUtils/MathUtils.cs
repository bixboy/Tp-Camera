using UnityEngine;

public static class MathUtils
{
    public static Vector3 GetNearestPointOnSegment(Vector3 a, Vector3 b, Vector3 target)
    {
        Vector3 ab = b - a;
        Vector3 ac = target - a;

        float abSqrLength = ab.sqrMagnitude;

        if (abSqrLength < Mathf.Epsilon)
            return a;

        float t = Vector3.Dot(ac, ab) / abSqrLength;
        t = Mathf.Clamp01(t);

        return a + ab * t;
    }
    
    public static Vector3 LinearBezier(Vector3 A, Vector3 B, float t)
    {
        return Vector3.Lerp(A, B, t);
    }
    
    public static Vector3 QuadraticBezier(Vector3 A, Vector3 B, Vector3 C, float t)
    {
        Vector3 AB = LinearBezier(A, B, t);
        Vector3 BC = LinearBezier(B, C, t);
        return LinearBezier(AB, BC, t);
    }
    
    public static Vector3 CubicBezier(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
    {
        Vector3 AB_BC = QuadraticBezier(A, B, C, t);
        Vector3 BC_CD = QuadraticBezier(B, C, D, t);
        return LinearBezier(AB_BC, BC_CD, t);
    }
}
