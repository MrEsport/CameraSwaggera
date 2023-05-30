using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    public static CameraController Instance { get { return instance; } }

    [SerializeField] private Camera _camera;
    [SerializeField] private CameraConfiguration _cameraConfiguration;

    private void Awake()
    {
        if(instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        ApplyConfiguration();
    }

    public void ApplyConfiguration()
    {
        ApplyConfiguration(_camera, _cameraConfiguration);
    }
    
    public void ApplyConfiguration(Camera camera, CameraConfiguration configuration)
    {
        _camera = camera;
        _cameraConfiguration = configuration;
        camera.transform.rotation = configuration.GetRotation; 
        camera.transform.position = configuration.GetPosition; 
        camera.fieldOfView = configuration.fov;
    }
    private void OnDrawGizmos()
    {
        if(_cameraConfiguration != null)
            _cameraConfiguration.DrawGizmos(Color.cyan);
    }
}
