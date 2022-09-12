using System;
using System.Linq;
using BendyVR_5.Debugging;
using BendyVR_5.Helpers;
using BendyVR_5.Player;
using BendyVR_5.Limbs;
//using BendyVR_5.PlayerBody;
using BendyVR_5.Settings;
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
    private static VrCore instance;
    private static GameManager m_gameManager;

    private static readonly string[] fallbackCameraTagSkipScenes = {"Main", "PreLoad"};
    private BindingsManager bindingsManager;
    private VRPlayerController vrPlayerController;

    //private BodyRendererManager bodyRendererManager;
    private Transform mNewCameraParent;
    private VRCameraManager cameraManager;

    private FadeOverlay fadeOverlay;
    //private Camera fallbackCamera;
    //private FakeParenting follow;
    //private InteractiveUiTarget interactiveUiTarget;
    //private IntroFix introFix;
    private VrLimbManager limbManager;
    private Camera mainCamera;
    //private RoomScaleBodyTransform roomScaleBodyTransform;
    //private StaticUiTarget staticUiTarget;
    private TeleportController teleportController;
    
    private VeryLateUpdateManager veryLateUpdateManager;
    //private VrSettingsMenu vrSettingsMenu;
    
    //private LivManager livManager; // GB This is for mixed motion capture

    public static void Create(GameManager gm_parent)
    {
        if (instance) return;
        
        m_gameManager = gm_parent;

        var stageParent = new GameObject("VrCoreParent")
        {
            //transform = {parent = parent}
        };

        //stageParent.AddComponent<vgOnlyLoadOnce>().dontDestroyOnLoad = true;

        DontDestroyOnLoad(stageParent);
        instance = new GameObject("VrCore").AddComponent<VrCore>();        
        instance.transform.SetParent(stageParent.transform, false);
        instance.vrPlayerController = VRPlayerController.Create(instance);
        
        //instance.cameraManager = VRCameraManager.Create(instance);
        //instance.limbManager = VrLimbManager.Create(instance);
        //instance.follow = stageParent.AddComponent<FakeParenting>();
        //instance.follow = FakeParenting.Create(stageParent.transform);
        //instance.interactiveUiTarget = InteractiveUiTarget.Create(instance);
        //instance.staticUiTarget = StaticUiTarget.Create(instance);
        //instance.teleportController = TeleportController.Create(instance, instance.limbManager);
        instance.veryLateUpdateManager = VeryLateUpdateManager.Create(instance);
        //instance.roomScaleBodyTransform = RoomScaleBodyTransform.Create(instance, instance.teleportController);
        //instance.bodyRendererManager = BodyRendererManager.Create(instance, instance.teleportController, instance.limbManager);
        instance.bindingsManager = BindingsManager.Create(instance);
        //instance.livManager = LivManager.Create(instance);
        //instance.fallbackCamera = new GameObject("VrFallbackCamera").AddComponent<Camera>();
        //instance.fallbackCamera.enabled = false;
        //instance.fallbackCamera.clearFlags = CameraClearFlags.Color;
        //instance.fallbackCamera.backgroundColor = Color.black;
        //instance.fallbackCamera.transform.SetParent(instance.transform, false);

        //instance.gameObject.AddComponent<GeneralDebugger>();

        BendyVRPatch.SetStage(instance);        
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
        //(Since CameraContainer is used for interactions)        
        //Set up a new camera parent
        mNewCameraParent = new GameObject("VrCameraParent").transform;
        //Set it to be below the head container
        mNewCameraParent.SetParent(playerController.HeadContainer, false);
        //Set Camera Container to be below new VrCameraParent
        playerController.CameraParent.SetParent(mNewCameraParent);
        //Set the Camera to also be below VrCameraParent
        GameManager.Instance.GameCamera.transform.SetParent(mNewCameraParent);
        //Move the Camera Container to match the VR Camera (which is auto tracked)
        FakeParenting.Create(playerController.CameraParent, GameManager.Instance.GameCamera.transform, FakeParenting.UpdateType.LateUpdate);

        //Set Gamecamera planes
        GameManager.Instance.GameCamera.Camera.nearClipPlane = 0.01f;        

        //Set Camera World Scale (Move Later)
        UpdateWorldScale();
        ResetHeight();
        VrSettings.WorldScale.SettingChanged += UpdateWorldScaleRealTime;
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

    private void UpdateWorldScaleRealTime(object sender, EventArgs e)
    {
        UpdateWorldScale();
        ResetHeight();
    }

    public void UpdateWorldScale()
    {
        float scale = VrSettings.WorldScale.Value;
        //GameManager.Instance.GameCamera.transform.localScale = new Vector3(scale, scale, scale);
        mNewCameraParent.localScale = new Vector3(scale, scale, scale);
        GameManager.Instance.Player.m_HandContainer.localScale = new Vector3(scale, scale, scale);
    }

    private void ResetHeight()
    {
        float scale = VrSettings.WorldScale.Value;
        float offset = (-1.5f * scale) + 0.7f;
        offset += VrSettings.HeightOffset.Value;
        mNewCameraParent.localPosition = new Vector3(0, offset, 0);
        GameManager.Instance.Player.m_HandContainer.localPosition = new Vector3(0, offset, 0);
        Logs.WriteInfo("Scale = " + scale);
        Logs.WriteInfo("VrCameraParent Y = " + mNewCameraParent.localPosition.y);
    }

    private void Update()
    {
        //if (!fallbackCamera.enabled && !(mainCamera && mainCamera.enabled)) SetUp(null, null);

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

    public void OpenVrSettings()
    {
        /*if (!vrSettingsMenu) return;
        vrSettingsMenu.Open();*/
    }

    /*public Transform GetInteractiveUiTarget()
    {
        return interactiveUiTarget ? interactiveUiTarget.TargetTransform : null;
    }

    public Transform GetStaticUiTarget()
    {
        return staticUiTarget ? staticUiTarget.TargetTransform : null;
    }*/

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

    public Transform GetLaserTransform()
    {
        if (limbManager == null || limbManager.Laser == null) return null;
        return limbManager.Laser.transform;
    }

    public Transform GetDominantHand()
    {
        return limbManager == null ? null : limbManager.DominantHand.transform;
    }


    public Transform GetMovementStickHand()
    {
        return limbManager == null ? null : limbManager.GetMovementStickHand().transform;
    }
}