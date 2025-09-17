using UnityEngine;
using UnityEngine.InputSystem;

public class FreeFollowView : AView
{
    [Header("Configs (bottom=0, middle=0.5, top=1)")]
    public float[] Pitch = new float[3];
    public float[] Roll  = new float[3];
    public float[] Fov   = new float[3];

    [Header("Control")]
    public float Yaw;
    public float YawSpeed = 90f;
    public Transform Target;
    public Curve Curve;
    [Range(0f, 1f)] public float CurvePosition;
    public float CurveSpeed = 1f;

    private Vector2 moveInput;

    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // stocker l’input courant
    }

    private void Update()
    {
        // appliquer en continu
        Yaw += moveInput.x * YawSpeed * Time.deltaTime;
        CurvePosition = Mathf.Clamp01(CurvePosition + moveInput.y * CurveSpeed * Time.deltaTime);
    }


    private Matrix4x4 CurveToWorldMatrix()
    {
        return Matrix4x4.TRS(
            Target.position,          // translation
            Quaternion.Euler(0, Yaw, 0), // rotation autour de Y
            Vector3.one               // pas d’échelle
        );
    }

    public override CameraConfiguration GetConfiguration()
    {
        Matrix4x4 curveToWorld = CurveToWorldMatrix();
        Vector3 cameraWorldPos = Curve.GetPosition(CurvePosition, curveToWorld);

        // Interpolation entre bottom (0), middle (0.5) et top (1)
        float pitchValue = Mathf.Lerp(
            Mathf.Lerp(Pitch[0], Pitch[1], CurvePosition * 2f),
            Mathf.Lerp(Pitch[1], Pitch[2], (CurvePosition - 0.5f) * 2f),
            CurvePosition > 0.5f ? 1 : 0
        );

        float rollValue = Mathf.Lerp(
            Mathf.Lerp(Roll[0], Roll[1], CurvePosition * 2f),
            Mathf.Lerp(Roll[1], Roll[2], (CurvePosition - 0.5f) * 2f),
            CurvePosition > 0.5f ? 1 : 0
        );

        float fovValue = Mathf.Lerp(
            Mathf.Lerp(Fov[0], Fov[1], CurvePosition * 2f),
            Mathf.Lerp(Fov[1], Fov[2], (CurvePosition - 0.5f) * 2f),
            CurvePosition > 0.5f ? 1 : 0
        );

        return new CameraConfiguration
        {
            yaw = Yaw,
            pitch = pitchValue,
            roll = rollValue,
            fov = fovValue,
            pivot = cameraWorldPos,
            distance = 0f
        };
    }
    
    public CameraConfiguration GetConfigurationAtCurvePosition(float curvePosition)
    {
        Matrix4x4 curveToWorld = CurveToWorldMatrix();

        Vector3 cameraWorldPos = Curve.GetPosition(CurvePosition, curveToWorld);

        CameraConfiguration config = new CameraConfiguration
        {
            yaw = Yaw,
            pitch = Mathf.Lerp(Pitch[0], Pitch[2], curvePosition),
            roll  = Mathf.Lerp(Roll[0],  Roll[2],  curvePosition),
            fov   = Mathf.Lerp(Fov[0],   Fov[2],   curvePosition),
            pivot = cameraWorldPos,
            distance = 0f
        };

        return config;
    }
    
    private void OnDrawGizmos()
    {
        Curve.DrawGizmo(Color.red, CurveToWorldMatrix());
        DrawCamera(Color.red, 0);
        DrawCamera(Color.green, 1);
        DrawCamera(Color.blue, 2);
    }
    
    public void DrawCamera(Color color, int cameraIndex)
    {
        Gizmos.color = color;

        Matrix4x4 curveToWorld = CurveToWorldMatrix();
        float t = cameraIndex / 2f; // 0, 0.5, 1

        // Position caméra correctement transformée
        Vector3 position = Curve.GetPosition(t, curveToWorld);

        Gizmos.matrix = Matrix4x4.TRS(
            position,
            Quaternion.Euler(Pitch[cameraIndex], Yaw, Roll[cameraIndex]),
            Vector3.one
        );

        if (Camera.main != null)
            Gizmos.DrawFrustum(Vector3.zero, Fov[cameraIndex], 0.5f, 0f, Camera.main.aspect);

        Gizmos.matrix = Matrix4x4.identity;
    }


}
