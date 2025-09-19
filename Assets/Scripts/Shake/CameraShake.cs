using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CameraShake
{
    public float duration = 0.5f;
    public float amplitude = 1f;
    public float frequency = 25f;

    public bool affectPosition = true;
    public bool affectRotation = true;
    public bool affectFov = false;

    public float fovAmplitude = 5f;

    public AnimationCurve blendCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    public Vector3 bezierP0 = Vector3.zero;
    public Vector3 bezierP1 = new Vector3(0.5f, 1f, 0);
    public Vector3 bezierP2 = new Vector3(1f, -1f, 0);
    public bool useBezier = false;

    private float elapsed = 0f;
    private Vector3 seed;

    public bool IsFinished => elapsed >= duration;

    public CameraShake()
    {
        seed = new Vector3(
            UnityEngine.Random.value * 100f,
            UnityEngine.Random.value * 100f,
            UnityEngine.Random.value * 100f
        );
    }

    public void Reset()
    {
        elapsed = 0f;
    }

    public void Update(float dt)
    {
        elapsed += dt;
    }

    public Vector3 GetPositionOffset()
    {
        if (!affectPosition) return Vector3.zero;
        float t = Mathf.Clamp01(elapsed / duration);

        float strength = amplitude * blendCurve.Evaluate(t);
        float x = Mathf.PerlinNoise(seed.x, elapsed * frequency) * 2f - 1f;
        float y = Mathf.PerlinNoise(seed.y, elapsed * frequency) * 2f - 1f;
        float z = Mathf.PerlinNoise(seed.z, elapsed * frequency) * 2f - 1f;

        Vector3 offset = new Vector3(x, y, z) * strength;

        if (useBezier)
        {
            offset = EvaluateBezier(t) * amplitude;
        }

        return offset;
    }

    public Vector3 GetRotationOffset()
    {
        if (!affectRotation) return Vector3.zero;
        float t = Mathf.Clamp01(elapsed / duration);

        float strength = amplitude * blendCurve.Evaluate(t);
        float x = (Mathf.PerlinNoise(seed.x, elapsed * frequency) * 2f - 1f) * strength * 2f;
        float y = (Mathf.PerlinNoise(seed.y, elapsed * frequency) * 2f - 1f) * strength * 2f;
        float z = (Mathf.PerlinNoise(seed.z, elapsed * frequency) * 2f - 1f) * strength * 2f;

        return new Vector3(x, y, z);
    }

    public float GetFovOffset()
    {
        if (!affectFov) return 0f;
        float t = Mathf.Clamp01(elapsed / duration);
        return Mathf.Sin(elapsed * frequency) * fovAmplitude * blendCurve.Evaluate(t);
    }

    private Vector3 EvaluateBezier(float t)
    {
        float u = 1 - t;
        return (u * u * bezierP0) + (2 * u * t * bezierP1) + (t * t * bezierP2);
    }
}