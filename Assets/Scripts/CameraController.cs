using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    private Camera _camera;
    
    [SerializeField] private float smoothSpeed = 5f;

    private bool isCutRequested = false;

    private CameraConfiguration _currentConfig;
    private CameraConfiguration _targetConfig;

    private List<AView> _activeViews = new List<AView>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _camera = GetComponent<Camera>();
        
        _targetConfig = ComputeAverage();
        _currentConfig = _targetConfig;
    }

    private void Update()
    {
        _targetConfig = ComputeAverage();

        if (isCutRequested)
        {
            _currentConfig = _targetConfig;
            isCutRequested = false;
        }
        else
        {
            SmoothTowardsTarget();    
        }
        
        ApplyConfiguration(_currentConfig);
    }

    private void ApplyConfiguration(CameraConfiguration config)
    {
        if (!_camera)
            return;

        _camera.transform.rotation = config.GetRotation();
        _camera.transform.position = config.GetPosition();
        _camera.fieldOfView = config.fov;
    }

    private void SmoothTowardsTarget()
    {
        float dt = Time.deltaTime;
        float t = smoothSpeed * dt;

        _currentConfig.pivot    = Vector3.Lerp(_currentConfig.pivot, _targetConfig.pivot, t);
        _currentConfig.distance = Mathf.Lerp(_currentConfig.distance, _targetConfig.distance, t);
        _currentConfig.pitch    = Mathf.Lerp(_currentConfig.pitch, _targetConfig.pitch, t);
        _currentConfig.roll     = Mathf.Lerp(_currentConfig.roll, _targetConfig.roll, t);
        _currentConfig.fov      = Mathf.Lerp(_currentConfig.fov, _targetConfig.fov, t);

        Vector2 currentYawVec = new Vector2(
            Mathf.Cos(_currentConfig.yaw * Mathf.Deg2Rad),
            Mathf.Sin(_currentConfig.yaw * Mathf.Deg2Rad)
        );

        Vector2 targetYawVec = new Vector2(
            Mathf.Cos(_targetConfig.yaw * Mathf.Deg2Rad),
            Mathf.Sin(_targetConfig.yaw * Mathf.Deg2Rad)
        );

        Vector2 smoothedYawVec = Vector2.Lerp(currentYawVec, targetYawVec, t);
        _currentConfig.yaw = Vector2.SignedAngle(Vector2.right, smoothedYawVec);
    }

    public void AddView(AView view)
    {
        if (!_activeViews.Contains(view))
            _activeViews.Add(view);
    }

    public void RemoveView(AView view)
    {
        _activeViews.Remove(view);
    }

    private CameraConfiguration ComputeAverage()
    {
        if (_activeViews.Count == 0) return _currentConfig;

        CameraConfiguration avg = new CameraConfiguration();
        float totalWeight = 0f;

        Vector2 yawVector = Vector2.zero;

        foreach (AView view in _activeViews)
        {
            CameraConfiguration config = view.GetConfiguration();

            avg.pitch += config.pitch * view.Weight;
            avg.roll += config.roll * view.Weight;
            avg.fov += config.fov * view.Weight;
            avg.pivot += config.pivot * view.Weight;
            avg.distance += config.distance * view.Weight;

            yawVector += new Vector2(
                Mathf.Cos(config.yaw * Mathf.Deg2Rad),
                Mathf.Sin(config.yaw * Mathf.Deg2Rad)
            ) * view.Weight;

            totalWeight += view.Weight;
        }

        if (totalWeight > 0f)
        {
            avg.pitch /= totalWeight;
            avg.roll /= totalWeight;
            avg.fov /= totalWeight;
            avg.pivot /= totalWeight;
            avg.distance /= totalWeight;
        }

        avg.yaw = Vector2.SignedAngle(Vector2.right, yawVector);

        return avg;
    }

    public void Cut()
    {
        isCutRequested = true;
    }

    private void OnDrawGizmos()
    {
        if (_camera != null)
        {
            _currentConfig.DrawGizmos(Color.green);
            _targetConfig.DrawGizmos(Color.red); // debug : voir la cible
        }
    }
}
