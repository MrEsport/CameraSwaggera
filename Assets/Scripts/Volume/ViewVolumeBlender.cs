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
        UpdateVolumes();
    }

    public void UpdateVolumes()
    {
        for (int i = 0; i < activeViewVolumes.Count; ++i)
            activeViewVolumes[i].View.Weight = 0f;

        var orderedVolumes = activeViewVolumes
            .OrderByDescending(v => v.Priority)       // Order by Priority
            .ThenByDescending(v => v.Uid)             // If Same Priority, Order by UID
            .ToList();

        for (int i = 0; i < orderedVolumes.Count; ++i)
        {
            float weight = Mathf.Clamp01(orderedVolumes[i].ComputeSelfWeight());
            float remainingWeight = 1f - weight;
            foreach (var view in volumesPerView.Keys)
                view.Weight *= remainingWeight;
            orderedVolumes[i].View.Weight += weight;
        }
    }

    public void AddVolume(AViewVolume volume)
    {
        activeViewVolumes.Add(volume);
        if (!volumesPerView.ContainsKey(volume.View))
        {
            volumesPerView[volume.View] = new List<AViewVolume>() { volume };
            volume.View.SetActive(true);
        }
        else
        {
            volumesPerView[volume.View].Add(volume);
        }
        UpdateVolumes();
    }
    
    public void RemoveVolume(AViewVolume volume)
    {
        activeViewVolumes.Remove(volume);
        volumesPerView[volume.View].Remove(volume);
        if (volumesPerView[volume.View].Count <= 0)
        {
            volumesPerView.Remove(volume.View);
            volume.View.SetActive(false);
        }
        UpdateVolumes();
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
