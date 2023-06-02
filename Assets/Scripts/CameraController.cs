using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    public static CameraController Instance { get { return _instance; } }

    [SerializeField] private Camera _camera;

    [SerializeField] private AView currentView = null;
    [SerializeField] private AView targetView = null;
    [SerializeField] private float transitionTime = 1;
    
    [Range(0f, 1f)] public float t = 0;
    
    private List<AView> _activeViews = new List<AView>();
    private CameraConfiguration _currentConfig;
    private Coroutine transitionRoutine = null;

    private void Awake()
    {
        if(_instance != null)
            Destroy(gameObject);
        else
            _instance = this;

        if (currentView == null || targetView == null)
            return;

       TransitionFromTo(currentView, targetView, transitionTime);
    }

    private void Update()
    {
        if (_activeViews.Count <= 0)
            return;

        if (transitionRoutine != null)
            return;

        if (currentView == null || targetView == null)
        {
            ApplyConfiguration(_camera, ComputeWeightedAverageConfiguration());
            return;
        }

        ApplyConfiguration(_camera, ViewLerp(currentView, targetView, t));
    }

    public void ApplyConfiguration(Camera camera, CameraConfiguration configuration)
    {
        _currentConfig = configuration;
        camera.transform.rotation = configuration.GetRotation; 
        camera.transform.position = configuration.GetPosition; 
        camera.fieldOfView = configuration.fov;
    }

    public void AddView(AView view) => _activeViews.Add(view);
    public void RemoveView(AView view) => _activeViews.Remove(view);

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

            pitchSum += viewConfig.pitch * view.weight;
            yawSum += new Vector2(Mathf.Cos(viewConfig.yaw * Mathf.Deg2Rad), Mathf.Sin(viewConfig.yaw * Mathf.Deg2Rad)) * view.weight; // Average Yaw from Vectors
            rollSum += viewConfig.roll * view.weight;

            distanceSum += viewConfig.distance * view.weight;
            fovSum += viewConfig.fov * view.weight;

            pivotSum += viewConfig.pivot * view.weight;

            totalWeight += view.weight;
        }

        config.pitch = pitchSum / totalWeight;
        config.yaw = Vector2.SignedAngle(Vector2.right, yawSum);
        config.roll = rollSum / totalWeight;
        
        config.distance = distanceSum / totalWeight;
        config.fov = fovSum / totalWeight;

        config.pivot = pivotSum / totalWeight;

        return config;
    }

    private CameraConfiguration ViewLerp(AView viewA, AView viewB, float t)
    {
        var config = new CameraConfiguration();

        var viewAConfig = viewA.GetConfiguration();
        var viewBConfig = viewB.GetConfiguration();

        config.pitch = (viewAConfig.pitch * t + viewBConfig.pitch * (1 - t));
        config.yaw = Vector2.SignedAngle(Vector2.right, new Vector2(Mathf.Cos(viewAConfig.yaw * Mathf.Deg2Rad), Mathf.Sin(viewAConfig.yaw * Mathf.Deg2Rad)) * t // Average Yaw from Vectors
                    + new Vector2(Mathf.Cos(viewBConfig.yaw * Mathf.Deg2Rad), Mathf.Sin(viewBConfig.yaw * Mathf.Deg2Rad)) * (1 - t)); // Average Yaw from Vectors
        config.roll = (viewAConfig.roll * t + viewBConfig.roll * (1 - t));

        config.distance = (viewAConfig.distance * t + viewBConfig.distance * (1 - t));
        config.fov = (viewAConfig.fov * t + viewBConfig.fov * (1 - t));
        
        config.pivot = (viewAConfig.pivot * t + viewBConfig.pivot * (1 - t));

        return config;
    }

    public void TransitionFromTo(AView viewA, AView viewB, float time)
    {
        if(transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(TransitionCoroutine());

        IEnumerator TransitionCoroutine()
        {
            t = 0;
            while (1 - t >= .025f)
            {
                t += (1 - t) * (1 / time) * Time.deltaTime;
                t = 1 - t >= .025f ? t : 1;
                ApplyConfiguration(_camera, ViewLerp(viewA, viewB, t));
                yield return null;
            }
        }

        transitionRoutine = null;
    }


    private void OnDrawGizmos()
    {
        if(_activeViews.Count <= 0)
            return;
        
        _currentConfig?.DrawGizmos(Color.cyan);
    }
}
