using BendyVR_5.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BendyVR_5.Player;

internal class VRPlayerController : MonoBehaviour
{
    private PlayerController mPlayerController;
    private VrStage stage;
    bool enabled = false;

    public static VRPlayerController Create(VrStage stage)
    {
        var instance = stage.gameObject.AddComponent<VRPlayerController>();
        instance.stage = stage;
        return instance;
    }

    public void SetUp(PlayerController _playerController)
    {
        mPlayerController = _playerController;
        RemoveAutomaticTracking();
        enabled = true;
    }

    private void RemoveAutomaticTracking()
    {
        //Set the camera to not automatically be updated by tracking:
        //var camera = GameManager.Instance.GameCamera.Camera;
        //UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(camera, true);
    }

    public void LateUpdate()
    {
        if (enabled)
        {
            /*var camera = GameManager.Instance.GameCamera.Camera;
            Quaternion rotation = camera.transform.localRotation;
            mPlayerController.m_CameraContainer.transform.localRotation = rotation;
            Quaternion headRotation = mPlayerController.HeadContainer.localRotation;
            headRotation.x -= rotation.x;
            mPlayerController.HeadContainer.localRotation = headRotation;*/

            /*Quaternion headsetRotation = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head);
            Vector3 headsetPosition = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.Head);
            
            camera.transform.localRotation = headsetRotation;
            camera.transform.localPosition = headsetPosition;*/
            //mPlayerController.m_CameraContainer.localRotation = headsetRotation;
            /*Vector3 eulerAngles = mPlayerController.HeadContainer.localEulerAngles;
            eulerAngles.z = 0f;
            mPlayerController.HeadContainer.localEulerAngles = eulerAngles;*/

            //mPlayerController.m_CameraContainer.localPosition = headsetPosition;
            //Not sure if I need this
            //mPlayerController.m_HandContainer.localRotation = headsetRotation;
        }
    }
}
