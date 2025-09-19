using UnityEngine;

public class ShakeTest : MonoBehaviour
{
    public void PlayShake1()
    {
        CameraController.Instance.PlayShake(new CameraShake
        {
            duration = 1f,
            amplitude = 0.5f,
            frequency = 20f,
            affectPosition = true,
            affectRotation = true,
            blendCurve = AnimationCurve.EaseInOut(0, 1, 1, 0)
        });
    }

    public void PlayShake2()
    {
        var bezierShake = new CameraShake
        {
            duration = 2f,
            amplitude = 1f,
            useBezier = true,
            bezierP0 = Vector3.zero,
            bezierP1 = new Vector3(0.5f, 2f, 0f),
            bezierP2 = new Vector3(-1f, -1f, 0f)
        };

        CameraController.Instance.PlayShake(bezierShake);
    }
}
