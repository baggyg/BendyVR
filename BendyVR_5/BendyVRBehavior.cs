using System;
using System.Collections.Generic;
using UnityEngine;

namespace BendyVR_5;

public abstract class BendyVRBehavior : MonoBehaviour
{
    private static readonly Dictionary<Type, List<BendyVRBehavior>> typeInstanceMap = new();

    protected virtual void Awake()
    {
        if (typeInstanceMap.ContainsKey(GetType()))
            typeInstanceMap[GetType()].Add(this);
        else
            typeInstanceMap[GetType()] = new List<BendyVRBehavior> {this};
    }

    protected virtual void OnDestroy()
    {
        typeInstanceMap.TryGetValue(GetType(), out var instance);

        instance?.Remove(this);
    }

    protected abstract void VeryLateUpdate();

    public static void InvokeVeryLateUpdate<TBehavior>() where TBehavior : BendyVRBehavior
    {
        typeInstanceMap.TryGetValue(typeof(TBehavior), out var instances);
        if (instances == null) return;
        foreach (var instance in instances) instance.InvokeVeryLateUpdateIfEnabled();
    }

    private void InvokeVeryLateUpdateIfEnabled()
    {
        if (!enabled) return;
        VeryLateUpdate();
    }
}