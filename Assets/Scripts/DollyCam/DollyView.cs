using System.Collections.Generic;
using UnityEngine;

public class DollyView : AView
{
    [Header("Camera Parameters")]
    public float roll = 0f;
    public float distance = 10f;
    public float fov = 60f;

    [Header("Target Following")]
    public Transform target;

    [Header("Rail Settings")]
    public Rail rail;
    public float distanceOnRail = 0f;
    public float speed = 5f;
    public bool isAuto = true;

    private void Update()
    {
        if (!rail)
            return;

        if (isAuto)
        {
            distanceOnRail = GetNearestDistanceOnRail(target.position);
        }
        else
        {
            float input = Input.GetAxis("Horizontal");
            distanceOnRail += input * speed * Time.deltaTime;

            if (rail.isLoop)
            {
                float length = rail.GetLength();
                distanceOnRail = (distanceOnRail % length + length) % length;
            }
            else
            {
                distanceOnRail = Mathf.Clamp(distanceOnRail, 0f, rail.GetLength());
            }
        }
    }

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration config = new CameraConfiguration();

        Vector3 railPos = rail.GetPosition(distanceOnRail);
        Vector3 lookDir = Vector3.forward;
        
        if (target)
            lookDir = (target.position - railPos).normalized;

        Vector3 flatDir = new Vector3(lookDir.x, 0f, lookDir.z).normalized;

        float yaw = Mathf.Atan2(flatDir.x, flatDir.z) * Mathf.Rad2Deg;
        float pitch = -Mathf.Asin(lookDir.y) * Mathf.Rad2Deg;

        config.yaw = yaw;
        config.pitch = pitch;
        config.roll = roll;
        config.fov = fov;
        config.pivot = railPos;
        config.distance = distance;

        return config;
    }
    
    private float GetNearestDistanceOnRail(Vector3 targetPos)
    {
        if (!rail) 
            return 0f;

        float closestDistance = 0f;
        float minSqrDistance  = float.MaxValue;

        float cumulativeLength = 0f;

        List<Vector3> nodes = new List<Vector3>();
        for (int i = 0; i < rail.transform.childCount; i++)
        {
            nodes.Add(rail.transform.GetChild(i).position);
        }

        int segmentCount = nodes.Count;
        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 a = nodes[i];
            Vector3 b = nodes[(i + 1) % segmentCount];
            
            if (!rail.isLoop && i == segmentCount - 1) 
                break;

            Vector3 projected = MathUtils.GetNearestPointOnSegment(a, b, targetPos);
            float sqrDist = (targetPos - projected).sqrMagnitude;

            if (sqrDist < minSqrDistance)
            {
                minSqrDistance = sqrDist;
                float segmentLength = Vector3.Distance(a, b);
                float t = Vector3.Distance(a, projected) / segmentLength;
                closestDistance = cumulativeLength + t * segmentLength;
            }

            cumulativeLength += Vector3.Distance(a, b);
        }

        return closestDistance;
    }
}
    
    
