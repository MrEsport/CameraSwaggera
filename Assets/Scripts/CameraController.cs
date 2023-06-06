using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    public static CameraController Instance { get { return _instance; } }

    [SerializeField] private Camera _camera;

    [SerializeField] private float transitionSpeed = 1;
    [CurveRange(0, 0, 1, 1, EColor.Green), SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float maxTargetDistance;

    [Range(0f, 1f), SerializeField] private float t = 0;

    private List<AView> _activeViews = new List<AView>();
    private CameraConfiguration _currentConfig;
    private CameraConfiguration _targetConfig;
    [SerializeField] private bool isCutRequested;

    private float distanceSpeedFactor;

    private void Awake()
    {
        if(_instance != null)
            Destroy(gameObject);
        else
            _instance = this;
    }

    private void Update()
    {
        if (_currentConfig == null && !InitializeConfig())
            return;

        if (_activeViews.Count <= 0)
            return;

        // Compute Target config for this frame from Active Views
        _targetConfig = ComputeWeightedAverageConfiguration();

        if(isCutRequested)
        {
            // Force Cut to Target config
            ApplyConfiguration(_camera, _targetConfig);
            isCutRequested = false;
            return;
        }

        // Interpolate toward desired config
        ApplyConfiguration(_camera, LerpConfig());
    }

    public void ApplyConfiguration(Camera camera, CameraConfiguration configuration)
    {
        _currentConfig = configuration;
        camera.transform.rotation = configuration.GetRotation; 
        camera.transform.position = configuration.GetPosition; 
        camera.fieldOfView = configuration.fov;
    }

    public void AddView(AView view)
    {
        if (_activeViews.Contains(view))
            return;

        _activeViews.Add(view);
    }

    public void RemoveView(AView view)
    {
        if (!_activeViews.Contains(view))
            return;

        _activeViews.Remove(view);
    }

    public void Cut()
    {
        isCutRequested = true;
    }

    private bool InitializeConfig()
    {
        if (_activeViews.Count <= 0)
            return false;

        _currentConfig = ComputeWeightedAverageConfiguration();
        return true;
    }

    private CameraConfiguration ComputeWeightedAverageConfiguration()
    {
        var config = new CameraConfiguration();

        float totalWeight = 0f;

        float pitchSum = 0f;
        Vector2 yawSum = Vector2.zero;
        float rollSum = 0f;

        float distanceSum = 0f;
        float fovSum = 0f;

        Vector3 pivotSum = Vector3.zero;

        foreach (AView view in _activeViews)
        {
            var viewConfig = view.GetConfiguration();

            pitchSum += viewConfig.pitch * view.Weight;
            yawSum += new Vector2(Mathf.Cos(viewConfig.yaw * Mathf.Deg2Rad), Mathf.Sin(viewConfig.yaw * Mathf.Deg2Rad)) * view.Weight; // Average Yaw from Vectors
            rollSum += viewConfig.roll * view.Weight;

            distanceSum += viewConfig.distance * view.Weight;
            fovSum += viewConfig.fov * view.Weight;

            pivotSum += viewConfig.pivot * view.Weight;

            totalWeight += view.Weight;
        }

        config.pitch = pitchSum / totalWeight;
        config.yaw = Vector2.SignedAngle(Vector2.right, yawSum);
        config.roll = rollSum / totalWeight;
        
        config.distance = distanceSum / totalWeight;
        config.fov = fovSum / totalWeight;

        config.pivot = pivotSum / totalWeight;

        return config;
    }

    private CameraConfiguration LerpConfig()
    {
        distanceSpeedFactor = Mathf.InverseLerp(0.05f, maxTargetDistance, Vector3.Distance(_currentConfig.GetPosition, _targetConfig.GetPosition));
        if (distanceSpeedFactor <= 0f)
        {
            t = 1f;
            return _targetConfig;
        }
        
        float lerpSpeed = transitionSpeed * speedCurve.Evaluate(distanceSpeedFactor);;
        t = lerpSpeed * Time.deltaTime;

        var config = new CameraConfiguration();

        config.pitch = _targetConfig.pitch * t + _currentConfig.pitch * (1 - t);
        config.yaw = Vector2.SignedAngle(Vector2.right,
            new Vector2(Mathf.Cos(_targetConfig.yaw * Mathf.Deg2Rad), Mathf.Sin(_targetConfig.yaw * Mathf.Deg2Rad)) * t // Average Yaw from Vectors
            + new Vector2(Mathf.Cos(_currentConfig.yaw * Mathf.Deg2Rad), Mathf.Sin(_currentConfig.yaw * Mathf.Deg2Rad)) * (1 - t)); // Average Yaw from Vectors
        config.roll = _targetConfig.roll * t + _currentConfig.roll * (1 - t);

        config.distance = _targetConfig.distance * t + _currentConfig.distance * (1 - t);
        config.fov = _targetConfig.fov * t + _currentConfig.fov * (1 - t);
        
        config.pivot = _targetConfig.pivot * t + _currentConfig.pivot * (1 - t);

        return config;
    }

    private void OnDrawGizmos()
    {
        if(_activeViews.Count <= 0)
            return;
        
        _currentConfig?.DrawGizmos(Color.cyan);
        _targetConfig?.DrawGizmos(Color.red);

        if (_currentConfig == null || _targetConfig == null)
            return;

        Gizmos.color = Color.Lerp(Color.cyan, Color.red, distanceSpeedFactor);
        Gizmos.DrawLine(_currentConfig.GetPosition, _targetConfig.GetPosition);
    }
}
