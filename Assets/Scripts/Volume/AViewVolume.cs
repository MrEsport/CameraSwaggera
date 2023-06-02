using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AViewVolume : MonoBehaviour
{
    public int priority;
    public AView view;

    protected int Uid;
    public static int NextUid;

    private void Awake()
    {
        Uid = NextUid;
        NextUid++;
    }


    public virtual float ComputeSelfWeight()
    {
        return 1.0f;
    }
}
