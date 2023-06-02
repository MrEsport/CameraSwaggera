using NaughtyAttributes;
using UnityEngine;

public class TriggeredViewVolume : AViewVolume
{
    [Layer, SerializeField] int targetLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == targetLayer)
            SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == targetLayer)
            SetActive(false);
    }
}
