using System;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour
{
 public bool isLoop = true;

    private float _length;
    private List<Vector3> _nodes = new List<Vector3>();

    private void Awake()
    {
        _nodes.Clear();
        foreach (Transform child in transform)
        {
            _nodes.Add(child.position);
        }

        _length = ComputeLength();
    }
    
    private float ComputeLength()
    {
        float total = 0f;

        if (_nodes.Count < 2) return 0f;

        for (int i = 0; i < _nodes.Count - 1; i++)
        {
            total += Vector3.Distance(_nodes[i], _nodes[i + 1]);
        }

        if (isLoop)
            total += Vector3.Distance(_nodes[_nodes.Count - 1], _nodes[0]);

        return total;
    }
    
    public float GetLength()
    {
        return _length;
    }
    public Vector3 GetPosition(float distance)
    {
        if (_nodes.Count == 0)
            return transform.position;

        if (_nodes.Count == 1)
            return _nodes[0];

        if (isLoop)
            distance = distance % _length;
        else
            distance = Mathf.Clamp(distance, 0f, _length);

        for (int i = 0; i < _nodes.Count - 1; i++)
        {
            float segmentLength = Vector3.Distance(_nodes[i], _nodes[i + 1]);

            if (distance <= segmentLength)
            {
                float t = distance / segmentLength;
                return Vector3.Lerp(_nodes[i], _nodes[i + 1], t);
            }

            distance -= segmentLength;
        }

        if (isLoop)
        {
            float lastSegmentLength = Vector3.Distance(_nodes[_nodes.Count - 1], _nodes[0]);
            float t = distance / lastSegmentLength;
            return Vector3.Lerp(_nodes[_nodes.Count - 1], _nodes[0], t);
        }

        return _nodes[_nodes.Count - 1];
    }

 private void OnDrawGizmos()
    {
        if (transform.childCount == 0)
            return;
        
        Vector3 position = transform.GetChild(0).position;
        for (int i = 1; i < transform.childCount; i++)
        {
            Vector3 next = transform.GetChild(i).position;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(position, next);
            position = next;
        }
        if (isLoop && transform.childCount > 2)
        {
            Gizmos.DrawLine(position, transform.GetChild(0).position);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Vector3 nodePos = transform.GetChild(i).position;

            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(nodePos, Vector3.one * 0.2f);

            Vector3 forward;
            if (i < transform.childCount - 1)
                forward = (transform.GetChild(i + 1).position - nodePos).normalized;
            else if (isLoop)
                forward = (transform.GetChild(0).position - nodePos).normalized;
            else
                forward = Vector3.forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(nodePos, nodePos + forward * 0.5f);

            float fov = 60f;
            float aspect = 16f / 9f;
            float near = 0.1f;
            float far = 0.5f;

            Quaternion rot = Quaternion.LookRotation(forward, Vector3.up);

            Vector3 nearCenter = nodePos + forward * near;
            Vector3 farCenter = nodePos + forward * far;

            float nearHeight = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * near;
            float nearWidth = nearHeight * aspect;

            float farHeight = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * far;
            float farWidth = farHeight * aspect;

            Vector3[] nearCorners = new Vector3[4];
            Vector3[] farCorners = new Vector3[4];

            nearCorners[0] = nearCenter + rot * new Vector3(-nearWidth, -nearHeight, 0);
            nearCorners[1] = nearCenter + rot * new Vector3( nearWidth, -nearHeight, 0);
            nearCorners[2] = nearCenter + rot * new Vector3( nearWidth,  nearHeight, 0);
            nearCorners[3] = nearCenter + rot * new Vector3(-nearWidth,  nearHeight, 0);

            farCorners[0] = farCenter + rot * new Vector3(-farWidth, -farHeight, 0);
            farCorners[1] = farCenter + rot * new Vector3( farWidth, -farHeight, 0);
            farCorners[2] = farCenter + rot * new Vector3( farWidth,  farHeight, 0);
            farCorners[3] = farCenter + rot * new Vector3(-farWidth,  farHeight, 0);

            Gizmos.color = Color.cyan;
            for (int j = 0; j < 4; j++)
            {
                Gizmos.DrawLine(nearCorners[j], nearCorners[(j + 1) % 4]);
                Gizmos.DrawLine(farCorners[j], farCorners[(j + 1) % 4]);
                Gizmos.DrawLine(nearCorners[j], farCorners[j]);
            }
        }
    }
}
