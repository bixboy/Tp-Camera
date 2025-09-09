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
}
