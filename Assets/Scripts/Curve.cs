using UnityEngine;


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

    void DrawGizmo(Color c, Matrix4x4 localToWorldMatrix)
    {
        Gizmos.color = c;
         
        
    }
}
