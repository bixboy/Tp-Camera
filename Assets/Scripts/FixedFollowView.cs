using System;
using UnityEngine;


public class FixedFollowView : AView
{
    public float Roll;
    public float Yaw
    {
        get
        {
            if (!Target) return 0f;
            Vector3 dirToTarget = (Target.position - transform.position).normalized;
            float angleToTarget = Mathf.Atan2(dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;
            if (!CentralPoint) return angleToTarget;
            
            Vector3 dirToCentral = (CentralPoint.transform.position - transform.position).normalized;
            float angleToCentral = Mathf.Atan2(dirToCentral.x, dirToCentral.z) * Mathf.Rad2Deg;
            float angleDiff = Mathf.DeltaAngle(angleToCentral, angleToTarget);
            if (Mathf.Abs(angleDiff) > YawOffsetMax)
            {
                return angleToCentral + YawOffsetMax * Mathf.Sign(angleDiff);
            }
            return angleToTarget;
        }
    }

    public float Pitch
    {
        get
        {
            if (!Target) return 0f;
            Vector3 dirToTarget = (Target.position - transform.position).normalized;
            float angleToTarget = -Mathf.Asin(dirToTarget.y) * Mathf.Rad2Deg;
            if (!CentralPoint) return angleToTarget;
            
            Vector3 dirToCentral = (CentralPoint.transform.position - transform.position).normalized;
            float angleToCentral = -Mathf.Asin(dirToCentral.y) * Mathf.Rad2Deg;
            float angleDiff = Mathf.DeltaAngle(angleToCentral, angleToTarget);
            if (Mathf.Abs(angleDiff) > PitchOffsetMax)
            {
                return angleToCentral +PitchOffsetMax * Mathf.Sign(angleDiff);
            }

            return angleToTarget;
        }
    }

    public float Fov;
    public Transform Target;
    public GameObject CentralPoint;
    public float YawOffsetMax;
    public float PitchOffsetMax;
    
    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration config = new CameraConfiguration
        {
            yaw = Yaw,
            pitch = Pitch,
            roll = Roll,
            fov = Fov,
            pivot = transform.position,
            distance = 0f
        };
        return config;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;

        Vector3 CamToCenter = CentralPoint.transform.position - transform.position;
        Quaternion yawOffsetRot = Quaternion.AngleAxis(YawOffsetMax, Vector3.up);
        Vector3 dirYawMax = yawOffsetRot * CamToCenter;
        Gizmos.DrawRay(transform.position, dirYawMax);
        Vector3 dirYawMin = Quaternion.Inverse(yawOffsetRot) * CamToCenter;
        Gizmos.DrawRay(transform.position, dirYawMin);
        
        Vector3 dirPitchMax = Vector3.RotateTowards(CamToCenter, Vector3.up, PitchOffsetMax * Mathf.Deg2Rad, 0f);
        Gizmos.DrawRay(transform.position, dirPitchMax);
        Vector3 dirPitchMin = Vector3.RotateTowards(CamToCenter, Vector3.down, PitchOffsetMax * Mathf.Deg2Rad, 0f);
        Gizmos.DrawRay(transform.position, dirPitchMin);
    }
}
