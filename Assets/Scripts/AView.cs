using System;
using UnityEngine;


public abstract class AView : MonoBehaviour
{
    [Range(0f, 1f)]
    public float Weight;
    
    public void Start()
    {
        SetActive(true);
    }

    public virtual CameraConfiguration GetConfiguration()
    {
        return new CameraConfiguration();
    }

    public void SetActive(bool isActive)
    {
        if (isActive)
            CameraController.Instance.AddView(this);
        else
            CameraController.Instance.RemoveView(this);
    }

    public virtual void OnDrawGizmos()
    {
        GetConfiguration().DrawGizmos(Color.green);
    }
}
