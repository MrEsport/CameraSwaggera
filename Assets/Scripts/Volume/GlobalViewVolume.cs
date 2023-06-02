using UnityEngine;

public class GlobalViewVolume : AViewVolume
{
    [SerializeField] private bool isActiveOnStart;

    private void Start()
    {
        if (isActiveOnStart)
            SetActive(true);
    }
}
