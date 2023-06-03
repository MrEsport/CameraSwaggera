using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AView : MonoBehaviour
{
    [SerializeField] protected bool isActiveOnStart;

    [Min(0f), SerializeField] private float weight;
    public float Weight { get => weight; set => weight = value; }

    private CameraConfiguration _config;

    public virtual CameraConfiguration GetConfiguration()
    {
        if (_config == null)
            _config = new CameraConfiguration();
        return _config;
    }

    public void SetActive(bool isActive)
    {
        if(isActive)
            CameraController.Instance.AddView(this);
        else
            CameraController.Instance.RemoveView(this);
    }

    protected virtual void Reset()
    {
        isActiveOnStart = true;
        Weight = 1f;
    }

    protected virtual void Start()
    {
        if (isActiveOnStart)
            SetActive(true);
    }
}
