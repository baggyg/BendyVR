using System;
using System.Linq;
using BendyVR_5.Debugging;
using BendyVR_5.Helpers;
using BendyVR_5.Player;
using BendyVR_5.Settings;
using BendyVR_5.src;
using BendyVR_5.UI;
using BendyVR_5.VrCamera;
using BendyVR_5.VrInput;
using BendyVR_5.VrInput.ActionInputs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace BendyVR_5.Stage;

public class VrCore : MonoBehaviour
{
    public static VrCore instance;
    private static GameManager m_gameManager;

    private static readonly string[] fallbackCameraTagSkipScenes = {"Main", "PreLoad"};
    private BindingsManager bindingsManager;
    private VRPlayerController vrPlayerController;

    private VRCameraManager cameraManager;
    private FadeOverlay fadeOverlay;
    private Camera mainCamera;    
    
    private VeryLateUpdateManager veryLateUpdateManager;    
    
    //private LivManager livManager; // GB This is for mixed motion capture

    public static void Create(GameManager gm_parent)
    {
        if (instance) return;
        
        m_gameManager = gm_parent;

        var stageParent = new GameObject("VrCoreParent")
        {
            //transform = {parent = parent}
        };

        DontDestroyOnLoad(stageParent);
        instance = new GameObject("VrCore").AddComponent<VrCore>();        
        instance.transform.SetParent(stageParent.transform, false);
        instance.vrPlayerController = VRPlayerController.Create(instance);
        instance.veryLateUpdateManager = VeryLateUpdateManager.Create(instance);
        instance.bindingsManager = BindingsManager.Create(instance);
        
        BendyVRPatch.SetStage(instance);        
    }

    public float GetAttackVelocity()
    {
        return vrPlayerController.velocityVectorLength;
    }
    public float GetAttackAngularVelocity()
    {
        return vrPlayerController.angularVelocityVectorLength;
    }

    public void SetUp(Camera camera, PlayerController playerController)
    {
        mainCamera = camera;        
        var playerTransform = playerController ? playerController.transform : null;
        vrPlayerController.SetUp(playerController);

        // If the camera starts with an offset position, the tracking will always be incorrect.
        // So I disable VR, reset the camera position, and then enable VR again.
        XRSettings.enabled = false;

        //Create something above both CameraContainer... Then Move CameraContainer and Camera itself to that. Then FakeParent Camera to CameraContainer
        //(Since CameraContainer is used for interactions - it actually isn't anymore so don't even know if I need this)

        

        //playerController.transform.SetParent(mRoomScaleDamper,false);
        //playerController.transform.localPosition = new Vector3(0f, 0f, 0f);
        //playerController.m_HeadContainer.SetParent(mRoomScaleDamper);
        //playerController.m_HandContainer.SetParent(mRoomScaleDamper);
        //playerController.transform.localPosition = new Vector3(0f,0f,0f);
        //playerController.m_HeadContainer.localPosition = new Vector3(0f, 0f, playerController.transform.localPosition.z);
        //playerController.m_HandContainer.localPosition = new Vector3(0f, 0f, playerController.transform.localPosition.z);
        //TODO Feet

        //Room Scale - Create a movement buffer above the PlayerController and DeltaTransform that
        //TODO -> Only do X and Y
        //mRoomScaleDamper = new GameObject("RoomScaleDamper").transform;
        //playerController.transform.parent = mRoomScaleDamper;

        //Also Newly Set the Camera to also be below VrCameraParent
        //GameManager.Instance.GameCamera.transform.SetParent(mRoomScaleDamper);

        //Need to figure out roomscale.... ideally move the playercontroller position to match camera position and headcontainer to be the opposite
        //FakeParenting.Create(playerController.transform, GameManager.Instance.GameCamera.transform, FakeParenting.UpdateType.LateUpdate, FakeParenting.TransformType.All);
        //FakeParenting.Create(mRoomScaleDamper, GameManager.Instance.GameCamera.transform, FakeParenting.UpdateType.LateUpdate, FakeParenting.TransformType.DeltaInverseTransformOnly);
        //FakeParenting.Create(playerController.m_HeadContainer, GameManager.Instance.GameCamera.transform, FakeParenting.UpdateType.LateUpdate, FakeParenting.TransformType.DeltaInverseTransformOnly);


        //Set Gamecamera planes
        GameManager.Instance.GameCamera.Camera.nearClipPlane = 0.01f;        

        //Set Camera World Scale (Move Later)
        UpdateWorldScale();
        ResetHeight();
        //VrSettings.WorldScale.SettingChanged += UpdateWorldScaleRealTime;
        VrSettings.HeightOffset.SettingChanged += UpdateWorldScaleRealTime;

        XRSettings.enabled = true;

        //cameraManager.SetUp(mainCamera, playerTransform);
        //limbManager.SetUp(playerController, GameManager.Instance.GameCamera.Camera);
        //interactiveUiTarget.SetUp(nextCamera);
        //staticUiTarget.SetUp(nextCamera);
        //teleportController.SetUp(playerController);
        //FadeOverlay.Create(GameManager.Instance.GameCamera.Camera);
        veryLateUpdateManager.SetUp(GameManager.Instance.GameCamera.Camera);
        //turningController.SetUp(playerController);
        //roomScaleBodyTransform.SetUp(playerController);
        //bodyRendererManager.SetUp(playerController);
        //livManager.SetUp(nextCamera);
    }

    internal VRPlayerController GetVRPlayerController()
    {
        return vrPlayerController;
    }

    private void UpdateWorldScaleRealTime(object sender, EventArgs e)
    {
        UpdateWorldScale();
        ResetHeight();
    }

    public void UpdateWorldScale()
    {
        float scale = VrSettings.WorldScale;
        //GameManager.Instance.GameCamera.transform.localScale = new Vector3(scale, scale, scale);
        if (vrPlayerController.mNewCameraParent)
            vrPlayerController.mNewCameraParent.localScale = new Vector3(scale, scale, scale);
        else
            Logs.WriteError("mNewCameraParent does not exist! (UpdateWorldScale)");

        if(GameManager.Instance.Player.m_HandContainer)
            GameManager.Instance.Player.m_HandContainer.localScale = new Vector3(scale, scale, scale);
        else
            Logs.WriteError("m_HandContainer does not exist! (UpdateWorldScale)");
    }

    private void ResetHeight()
    {
        /*float scale = VrSettings.WorldScale.Value;
        float offset = (-1.5f * scale) + 0.7f;
        offset += VrSettings.HeightOffset.Value;
        
        vrPlayerController.mNewCameraParent.localPosition = new Vector3(0, offset, 0);

        GameManager.Instance.Player.m_HandContainer.localPosition = new Vector3(0, 0, 0);
        //Measure the difference between VRCameraParent and Hand Container (Absolute) -> Thats the new Hand Container offset - Don't ask me to explain it
        float handContainerOffset = vrPlayerController.mNewCameraParent.position.y - GameManager.Instance.Player.m_HandContainer.position.y;
        GameManager.Instance.Player.m_HandContainer.localPosition = new Vector3(0, handContainerOffset, 0);
        
        Logs.WriteInfo("Scale = " + scale);
        Logs.WriteInfo("Hand Offset = " + handContainerOffset);
        Logs.WriteInfo("VrCameraParent Y = " + vrPlayerController.mNewCameraParent.localPosition.y);*/
    }
       

    private void OnDisable()
    {
        throw new Exception(
            "The VR Stage is being disabled. This should never happen. Check the call stack of this error to find the culprit.");
    }

    public Camera GetMainCamera()
    {
        return mainCamera;
    }

    public void RecenterPosition(bool recenterVertically = false)
    {
        cameraManager.RecenterPosition(recenterVertically);
    }

    public void RecenterRotation()
    {
        cameraManager.RecenterRotation();
    }

    public void Recenter()
    {
        RecenterPosition(true);
        RecenterRotation();
    }

    public void FadeToBlack()
    {
        FadeOverlay.StartFade(Color.black, FadeOverlay.Duration);
    }

    public void FadeToClear()
    {
        FadeOverlay.StartFade(Color.clear, FadeOverlay.Duration);
    }

    public IActionInput GetInputAction(string virtualKey)
    {
        if (!bindingsManager) return null;
        bindingsManager.ActionMap.TryGetValue(virtualKey, out var value);
        return value;
    }

    public float GetInputValue(string virtualKey)
    {
        if (!bindingsManager) return 0;
        return bindingsManager.GetValue(virtualKey);
    }

    public bool GetInputUp(string virtualKey)
    {
        return bindingsManager && bindingsManager.GetUp(virtualKey);
    }

    public bool GetInputDown(string virtualKey)
    {
        return bindingsManager && bindingsManager.GetDown(virtualKey);
    }
}