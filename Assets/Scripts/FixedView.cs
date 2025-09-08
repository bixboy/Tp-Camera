using UnityEngine;


public class FixedView : AView
{
    public float Yaw;
    public float Pitch;
    public float Roll;
    public float Fov;

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
}
