using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeFollowView : AView
{
    [Header("Configs Bottom=0, Middle=0.5, Top=1")]
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
    
    [Header("Collision")]
    public float SphereRadius = 0.3f;
    public LayerMask ObstacleMask;
    public float CollisionAvoidanceOffset = 0.2f;

    private Vector2 _moveInput;
    private bool _canDragCam;

    private void Start()
    {
        SetActive(true);
        _canDragCam = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        SetActive(false);
        _canDragCam = false;
    }
    
    private void OnTriggerExit(Collider other)
    {
        SetActive(true);
        _canDragCam = true;
    }

    public void OnPlayerMoveMouse(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
        if (Input.GetMouseButton(0) && _canDragCam)
            _moveInput = context.ReadValue<Vector2>();
        
    }

    private void Update()
    {
        Yaw += _moveInput.x * YawSpeed * Time.deltaTime;
        Yaw = Mathf.Repeat(Yaw, 360f);

        CurvePosition = Mathf.Clamp01(CurvePosition + -_moveInput.y * CurveSpeed * Time.deltaTime);
    }


    private Matrix4x4 CurveToWorldMatrix()
    {
        return Matrix4x4.TRS(
            Target.position,          
            Quaternion.Euler(0, Yaw, 0), 
            Vector3.one              
        );
    }

    public override CameraConfiguration GetConfiguration()
    {
        Matrix4x4 curveToWorld = CurveToWorldMatrix();
        Vector3 desiredPos = Curve.GetPosition(CurvePosition, curveToWorld);

        Vector3 dir = desiredPos - Target.position;
        float desiredDistance = dir.magnitude;
        Vector3 camPos = desiredPos;
        
        if (Physics.SphereCast(Target.position, SphereRadius, dir.normalized, out RaycastHit hit, desiredDistance, ObstacleMask))
        {
            camPos = Target.position + dir.normalized * (hit.distance - CollisionAvoidanceOffset);
        }

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
            pivot = camPos,
            distance = 0f
        };
    }
    
    private void OnDrawGizmos()
    {
        Matrix4x4 curveToWorld = CurveToWorldMatrix();
        Curve.DrawGizmo(Color.red, curveToWorld);


        Vector3 desiredPos = Curve.GetPosition(CurvePosition, curveToWorld);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Target.position, desiredPos);
        Gizmos.DrawWireSphere(desiredPos, SphereRadius);
        
        
        DrawCamera(Color.red, 0);
        DrawCamera(Color.green, 1);
        DrawCamera(Color.blue, 2);
    }
    
    public void DrawCamera(Color color, int cameraIndex)
    {
        Gizmos.color = color;
        Matrix4x4 curveToWorld = CurveToWorldMatrix();
        Vector3 position = Curve.GetPosition(cameraIndex / 2f, curveToWorld);
        Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.Euler(Pitch[cameraIndex], Yaw, Roll[cameraIndex]), Vector3.one);
        if (Camera.main != null) 
            Gizmos.DrawFrustum(Vector3.zero, Fov[cameraIndex], 0.5f, 0f, Camera.main.aspect);
        Gizmos.matrix = Matrix4x4.identity;
    }


}
