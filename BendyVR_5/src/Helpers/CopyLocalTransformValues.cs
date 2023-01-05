using BendyVR_5.src;
using UnityEngine;

namespace BendyVR_5.Helpers;

public class CopyLocalTransformValues : BendyVRBehavior
{
    private Transform target;

    public static void Create(GameObject gameObject, Transform target)
    {
        var instance = gameObject.GetComponent<CopyLocalTransformValues>();
        if (!instance) instance = gameObject.AddComponent<CopyLocalTransformValues>();

        instance.target = target;
    }

    protected override void VeryLateUpdate()
    {
        if (!target)
        {
            Destroy(this);
            return;
        }
        Logs.WriteInfo("CLTV Setting " + transform.name + " to " + target.name);
        transform.localRotation = target.localRotation;
        transform.localPosition = target.localPosition;
        transform.localScale = target.localScale;
    }
}