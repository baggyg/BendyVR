using BendyVR_5.Helpers;
using BendyVR_5.Stage;
using UnityEngine;

namespace BendyVR_5.UI;

public class InteractiveUiTarget : UiTarget
{
    protected override float MinAngleDelta => 45f;

    public static InteractiveUiTarget Create(VrCore stage)
    {
        var instance = new GameObject(nameof(InteractiveUiTarget)).AddComponent<InteractiveUiTarget>();
        instance.transform.SetParent(stage.transform, false);
        instance.TargetTransform = new GameObject("InteractiveUiTargetTransform").transform;
        instance.TargetTransform.SetParent(instance.transform, false);
        instance.TargetTransform.localPosition = Vector3.forward * 3f;
        return instance;
    }

    protected override Vector3 GetCameraForward()
    {
        return MathHelper.GetProjectedForward(CameraTransform);
    }
}