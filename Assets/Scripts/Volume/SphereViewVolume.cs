using System;
using UnityEngine;

public class SphereViewVolume : AViewVolume
{
    public GameObject target;
    
    public float outerRadius;
    
    public float innerRadius;


    private float _distance;

    private void Update()
    {
        if (!target) return;

        _distance = Vector3.Distance(target.transform.position, transform.position);

        if (_distance <= outerRadius && !IsActive)
            SetActive(true);

        if (_distance > outerRadius && IsActive)
            SetActive(false);
    }

    public override float ComputeSelfWeight()
    {
        if (!target) 
            return 0f;

        float d = _distance;

        float inner = Mathf.Max(0f, innerRadius);
        float outer = Mathf.Max(inner + 0.01f, outerRadius);

        if (d <= inner)
            return 1.0f;

        if (d >= outer)
            return 0.0f;

        float t = (d - inner) / (outer - inner);
        return 1.0f - t;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
    }
}
