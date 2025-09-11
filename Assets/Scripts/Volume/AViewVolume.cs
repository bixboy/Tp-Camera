using System;
using UnityEngine;

public abstract class AViewVolume : MonoBehaviour
{
    public int priority = 0;

    public AView view;

    public int uid;

    public static int NextUid = 0;

    public bool isCutOnSwitch = false;
    
    protected bool IsActive { get; private set; }

    private void Awake()
    {
        uid = NextUid;
        NextUid++;
    }

    public virtual float ComputeSelfWeight()
    {
        return 1.0f;
    }
    
    protected void SetActive(bool active)
    {
        if (IsActive == active)
            return;

        IsActive = active;

        if (IsActive)
            ViewVolumeBlender.Instance.AddVolume(this);
        else
            ViewVolumeBlender.Instance.RemoveVolume(this);

        if (isCutOnSwitch)
        {
            ViewVolumeBlender.Instance.ForceUpdate();
            CameraController controller = FindFirstObjectByType<CameraController>();
            
            if (controller)
                controller.Cut();
        }
    }
}