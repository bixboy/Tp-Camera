using UnityEngine;

[System.Serializable]
public class Curve
{
    public Vector3 A,B,C,D;
    
    public Vector3 GetPosition(float t)
    {
        return MathUtils.CubicBezier(A, B, C, D, t);
    }

    public Vector3 GetPosition(float t, Matrix4x4 localToWorldMatrix)
    {
        return localToWorldMatrix.MultiplyPoint(GetPosition(t));
    }

    public void DrawGizmo(Color c, Matrix4x4 localToWorldMatrix)
    {
        Gizmos.color = c;


        Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(A), 0.05f);
        Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(B), 0.05f);
        Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(C), 0.05f);
        Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(D), 0.05f);


        int steps = 20;
        Vector3 prev = GetPosition(0f, localToWorldMatrix);
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 curr = GetPosition(t, localToWorldMatrix);
            Gizmos.DrawLine(prev, curr);
            prev = curr;
        }
    }
}
