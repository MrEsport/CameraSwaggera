using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    public static CameraController Instance { get { return _instance; } }

    [SerializeField] private Camera _camera;

    private List<AView> _activeViews = new List<AView>();

    private void Awake()
    {
        if(_instance != null)
            Destroy(gameObject);
        else
            _instance = this;
    }

    private void Update()
    {
        if (_activeViews.Count <= 0)
            return;

        ApplyConfiguration(_camera, ComputeWeightedAverageConfiguration());
    }

    public void ApplyConfiguration(Camera camera, CameraConfiguration configuration)
    {
        //_camera = camera;
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

    private void OnDrawGizmos()
    {
        if(_activeViews.Count <= 0)
            return;
        ComputeWeightedAverageConfiguration().DrawGizmos(Color.cyan);
    }
}
