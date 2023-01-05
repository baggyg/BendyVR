using BendyVR_5.src;
using System;
using UnityEngine;

namespace BendyVR_5.Helpers;

// This component is useful when we need to simulate object parenting,
// without actually changing the hierarchy.
public class FakeParenting : BendyVRBehavior
{
    [Flags]
    public enum UpdateType
    {
        None = 0,
        LateUpdate = 1,
        VeryLateUpdate = 2
    }

    public enum TransformType
    {
        All = 0,        
        Delta = 1,
        DeltaInverse = 2,
        DeltaTransformOnly = 3,
        DeltaInverseTransformOnly = 4,
        TransformOnly = 5,
        RotationOnly = 6
    }

    private TransformType transformType;
    private Transform target;
    private UpdateType updateTypes;
    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    private Quaternion lastRotation;

    public static FakeParenting Create(Transform transform, Transform target = null,
        UpdateType updateType = UpdateType.VeryLateUpdate,
        TransformType transformType = TransformType.All)
    {
        var instance = transform.GetComponent<FakeParenting>();
        if (!instance) instance = transform.gameObject.AddComponent<FakeParenting>();
        instance.target = target;
        instance.updateTypes = updateType;
        instance.transformType = transformType;

        Logs.WriteWarning(transform.gameObject.name + " is now following " + target.gameObject.name +  " (" + transformType.ToString() + ")");

        return instance;
    }

    private void LateUpdate()
    {
        if (!IsUpdateType(UpdateType.LateUpdate)) return;
        UpdateTransform();
    }

    protected override void VeryLateUpdate()
    {
        if (!IsUpdateType(UpdateType.VeryLateUpdate)) return;
        UpdateTransform();
    }

    private bool IsUpdateType(UpdateType type)
    {
        return (updateTypes & type) != UpdateType.None;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void UpdateTransform()
    {
        if (!target) return;

        if (transformType.Equals(TransformType.All) ||
            transformType.Equals(TransformType.RotationOnly) ||
            transformType.Equals(TransformType.TransformOnly))
        {
            if(!transformType.Equals(TransformType.RotationOnly))
                transform.position = target.position;
            if (!transformType.Equals(TransformType.TransformOnly))
                transform.rotation = target.rotation;
        }
        else if (transformType.Equals(TransformType.Delta) || 
            transformType.Equals(TransformType.DeltaInverse) ||
            transformType.Equals(TransformType.DeltaTransformOnly) || 
            transformType.Equals(TransformType.DeltaInverseTransformOnly))
        {
            //Velocity Delta
            if (!lastPosition.Equals(new Vector3(0f, 0f, 0f)))
            {
                Vector3 posChange = target.localPosition - lastPosition;
                lastPosition = target.localPosition;

                //Angular Velocity
                Quaternion deltaRotation = target.localRotation * Quaternion.Inverse(lastRotation);
                lastRotation = target.localRotation;

                if (transformType.Equals(TransformType.DeltaTransformOnly) || transformType.Equals(TransformType.Delta))
                {
                    transform.position += posChange;
                    if (transformType.Equals(TransformType.Delta))
                        transform.rotation *= deltaRotation;
                }
                else
                {
                    transform.position -= posChange;
                    if (transformType.Equals(TransformType.DeltaInverse))
                        transform.rotation *= Quaternion.Inverse(deltaRotation);
                }
            }
            else
            {
                lastPosition = target.localPosition;
                lastRotation = target.localRotation;
            }
        }
    }
}