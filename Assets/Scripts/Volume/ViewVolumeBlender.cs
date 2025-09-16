using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewVolumeBlender : MonoBehaviour
{
    
    public static ViewVolumeBlender Instance { get; private set; }
    
    private List<AViewVolume> _activeViewVolumes = new List<AViewVolume>();
    private Dictionary<AView, List<AViewVolume>> _volumesPerViews = new Dictionary<AView, List<AViewVolume>>();


    public void AddVolume(AViewVolume volume)
    {
        if (!_activeViewVolumes.Contains(volume))
            _activeViewVolumes.Add(volume);

        AView view = volume.view;
        if (view == null) return;

        if (!_volumesPerViews.ContainsKey(view))
        {
            _volumesPerViews[view] = new List<AViewVolume>();
            view.SetActive(true);
        }

        if (!_volumesPerViews[view].Contains(volume))
            _volumesPerViews[view].Add(volume);
    }

    public void RemoveVolume(AViewVolume volume)
    {
        if (_activeViewVolumes.Contains(volume))
            _activeViewVolumes.Remove(volume);

        AView view = volume.view;
        if (view == null) return;

        if (_volumesPerViews.ContainsKey(view))
        {
            _volumesPerViews[view].Remove(volume);

            if (_volumesPerViews[view].Count == 0)
            {
                _volumesPerViews.Remove(view);
                view.SetActive(false);
            }
        }  
    }
    
    
    public void ForceUpdate()
    {
        UpdateVolumes();
    }
    
    private void Update()
    {
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        foreach (var view in _volumesPerViews.Keys)
        {
            view.Weight = 0f;
        }

        var sortedVolumes = _activeViewVolumes
            .OrderBy(v => v.priority)
            .ThenBy(v => v.uid)
            .ToList();

        foreach (var volume in sortedVolumes)
        {
            float weight = Mathf.Clamp01(volume.ComputeSelfWeight());
            float remainingWeight = 1.0f - weight;

            foreach (var view in _volumesPerViews.Keys)
            {
                view.Weight *= remainingWeight;
            }

            if (volume.view)
            {
                volume.view.Weight += weight;
            }
        }
    }
}