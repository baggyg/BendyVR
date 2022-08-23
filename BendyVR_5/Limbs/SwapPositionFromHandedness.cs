using BendyVR_5.Settings;
using UnityEngine;

namespace BendyVR_5.Limbs;

public class SwapPositionFromHandedness : HandednessChangeListener
{
    private Vector3 leftHandedPosition;
    private Vector3 rightHandedPosition;

    public static void Create(Transform transform, Vector3 rightHandedPosition, Vector3 leftHandedPosition)
    {
        var instance = transform.gameObject.AddComponent<SwapPositionFromHandedness>();
        instance.rightHandedPosition = rightHandedPosition;
        instance.leftHandedPosition = leftHandedPosition;
    }

    protected override void HandednessChanged()
    {
        transform.localPosition = VrSettings.LeftHandedMode.Value ? leftHandedPosition : rightHandedPosition;
    }
}