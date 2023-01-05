using BendyVR_5.Helpers;
//using TwoForksVr.PlayerBody;
using BendyVR_5.Stage;
using UnityEngine;

namespace BendyVR_5.src;

// This manager handles some methods that need to run as late as possible, after every other update.
// They often need to run in a specific order, which is also defined here.
public class VeryLateUpdateManager : MonoBehaviour
{
    private Camera camera;

    public static VeryLateUpdateManager Create(VrCore stage)
    {
        return stage.gameObject.AddComponent<VeryLateUpdateManager>();
    }

    public void SetUp(Camera activeCamera)
    {
        camera = activeCamera;
    }

    private void Awake()
    {
        Camera.onPreCull += HandlePreCull;
    }

    private void OnDestroy()
    {
        Camera.onPreCull -= HandlePreCull;
    }

    private void HandlePreCull(Camera preCullCamera)
    {
        if (preCullCamera != camera) return;

        //BendyVRBehavior.InvokeVeryLateUpdate<RoomScaleBodyTransform>();
        BendyVRBehavior.InvokeVeryLateUpdate<FakeParenting>();
        BendyVRBehavior.InvokeVeryLateUpdate<CopyLocalTransformValues>();
    }
}