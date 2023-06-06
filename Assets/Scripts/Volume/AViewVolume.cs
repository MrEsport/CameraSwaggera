using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AViewVolume : MonoBehaviour
{
    public int Priority;
    public AView View;

    public int Uid { get; private set; }
    public static int NextUid;

    protected bool IsActive { get; private set; }

    [SerializeField] protected bool isCutOnSwitch;

    private void Awake()
    {
        Uid = NextUid;
        NextUid++;
    }

    public virtual float ComputeSelfWeight() => 1f;

    protected void SetActive(bool isActive)
    {
        IsActive = isActive;
        if (IsActive)
        {
            ViewVolumeBlender.Instance.AddVolume(this);
        }
        else
        {
            ViewVolumeBlender.Instance.RemoveVolume(this);
        }

        if(isCutOnSwitch)
        {
            ViewVolumeBlender.Instance.UpdateVolumes();
            CameraController.Instance.Cut();
        }
    }
}
