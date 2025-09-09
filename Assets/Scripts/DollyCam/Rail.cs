using System;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour
{
    public bool isLoop = true;

    private float _length;

    private List<Vector3> _nodes;

    private void Start()
    {
        
    }

    float GetLength()
    {
        return _length;
    }

    Vector3 GetPosition(float distance)
    {
        if (_nodes.Count == 0)
            return Vector3.zero;

        Vector3 firstNode = _nodes[0];
        return transform.position;
    }
}
