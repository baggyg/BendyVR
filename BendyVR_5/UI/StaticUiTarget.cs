using BendyVR_5.Stage;
using UnityEngine;

namespace BendyVR_5.UI;

public class StaticUiTarget : UiTarget
{
    protected override float MinAngleDelta => 20f;

    public static StaticUiTarget Create(VrCore stage)
    {
        var instance = new GameObject(nameof(StaticUiTarget)).AddComponent<StaticUiTarget>();
        instance.transform.SetParent(stage.transform, false);
        instance.TargetTransform = new GameObject("InteractiveUiTargetTransform").transform;
        instance.TargetTransform.SetParent(instance.transform, false);
        instance.TargetTransform.localPosition = Vector3.forward;
        return instance;
    }

    protected override Vector3 GetCameraForward()
    {
        return CameraTransform.forward;
    }
}