using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewVolumeBlender : MonoBehaviour
{

    private static ViewVolumeBlender instance;
    public static ViewVolumeBlender Instance { get => instance; } 

    [SerializeField] private List<AViewVolume> activeViewVolumes = new List<AViewVolume>();
    [SerializeField] private Dictionary<AView, List<AViewVolume>> volumesPerView = new Dictionary<AView, List<AViewVolume>>();

    [SerializeField] private bool drawDebug;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        for (int i = 0; i < activeViewVolumes.Count; ++i)
            activeViewVolumes[i].view.Weight = 0f;

        var orderedVolumes = activeViewVolumes
            .OrderByDescending(v => v.priority)       // Order by Priority
            .ThenByDescending(v => v.Uid)             // If Same Priority, Order by UID
            .ToList();

        for (int i = 0; i < orderedVolumes.Count; ++i)
        {
            float weight = Mathf.Clamp01(orderedVolumes[i].ComputeSelfWeight());
            float remainingWeight = 1f - weight;
            foreach (var view in volumesPerView.Keys)
                view.Weight *= remainingWeight;
            orderedVolumes[i].view.Weight += weight;
        }
    }

    public void AddVolume(AViewVolume volume)
    {
        activeViewVolumes.Add(volume);
        if (!volumesPerView.ContainsKey(volume.view))
        {
            volumesPerView[volume.view] = new List<AViewVolume>() { volume };
            volume.view.SetActive(true);
        }
        else
        {
            volumesPerView[volume.view].Add(volume);
        }
    }
    
    public void RemoveVolume(AViewVolume volume)
    {
        activeViewVolumes.Remove(volume);
        volumesPerView[volume.view].Remove(volume);
        if (volumesPerView[volume.view].Count <= 0)
        {
            volumesPerView.Remove(volume.view);
            volume.view.SetActive(false);
        }
    }

    private void OnGUI()
    {
        if (!drawDebug)
            return;

        GUI.Label(new Rect(5, 0, 400, 20), "  Active Volumes :");
        for (int i = 0;  i < activeViewVolumes.Count; ++i) 
        {
            GUI.Label(new Rect(5, 20 + i * 20, 400, 20), $"{activeViewVolumes[i].name}");
        }
    }
}
