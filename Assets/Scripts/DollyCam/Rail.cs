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
            Gizmos.DrawLine(position, next);
            position = next;
        }
    }
}
