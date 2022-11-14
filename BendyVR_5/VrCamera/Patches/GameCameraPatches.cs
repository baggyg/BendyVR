using BendyVR_5.Helpers;
using BendyVR_5.Stage;
using DG.Tweening;
using HarmonyLib;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace BendyVR_5.VrCamera.Patches;

[HarmonyPatch]
public class GameCameraPatches : BendyVRPatch
{
    private static bool isDone;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameCamera), nameof(GameCamera.ExitFreeRoamCam))]
    public static bool VRExitFreeRoamCam(GameCamera __instance)
    {
        Vector3 zero = Vector3.zero;
        zero.x = __instance.FreeRoamCam.eulerAngles.x;
        Vector3 zero2 = Vector3.zero;
        zero2.y = __instance.FreeRoamCam.localEulerAngles.y;
        GameManager.Instance.GameCamera.CameraContainer.SetParent(GameManager.Instance.GameCamera.HeadContainer);

        //Set Camera Container to be below new VrCameraParent
        VrCore vrCore = VrCore.instance;
        vrCore.GetVRPlayerController().SetupCameraHierarchy();

        Logs.WriteWarning("Exiting Free Roam. '" + GameManager.Instance.GameCamera.CameraContainer.gameObject.name + "' parent is '" + GameManager.Instance.GameCamera.CameraContainer.parent.name + "'");
        
        GameManager.Instance.GameCamera.CameraContainer.localPosition = Vector3.zero;
        GameManager.Instance.GameCamera.CameraContainer.localEulerAngles = Vector3.zero;
        GameManager.Instance.GameCamera.CameraContainer.localScale = Vector3.one;
        GameManager.Instance.Player.LookRotation(Quaternion.Euler(zero2), Quaternion.Euler(zero));
        GameManager.Instance.Player.SetLockedMovement(active: false);
        GameManager.Instance.GameCamera.FreeRoamCam.SetParent(null);
        GameManager.Instance.Player.EnableSeeingTool(active: true);

        vrCore.GetVRPlayerController().SetupCameraFollow();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameCamera), nameof(GameCamera.InitializeFreeRoamCam))]
    public static bool VRInitializeFreeRoamCam(ref Transform __result)
    {
        Logs.WriteWarning("INITIALISING FREE ROAM");
        GameManager.Instance.Player.EnableSeeingTool(active: false);
        GameManager.Instance.Player.SetLockedMovement(active: true);

        //Removing Follow
        VrCore.instance.GetVRPlayerController().RemoveCameraFollow();
        
        Transform freeRoamCam = GameManager.Instance.GameCamera.FreeRoamCam;
        Logs.WriteWarning("FreeRoam Tranform = " + freeRoamCam.name);
        freeRoamCam.position = GameManager.Instance.GameCamera.CameraContainer.position;
        freeRoamCam.rotation = GameManager.Instance.GameCamera.CameraContainer.rotation;
        
        GameManager.Instance.GameCamera.CameraContainer.SetParent(freeRoamCam);
        GameManager.Instance.GameCamera.CameraContainer.localScale = Vector3.one;
        
        __result = freeRoamCam;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CameraFOV), nameof(CameraFOV.SetFOV))]
    [HarmonyPatch(typeof(CameraFOV), nameof(CameraFOV.DOFov))]
    [HarmonyPatch(typeof(CameraFOV), nameof(CameraFOV.UpdateVOD))]    
    private static bool OverrideFOV()
    {
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FirstPersonHeadBob), nameof(FirstPersonHeadBob.UpdateCameraPosition))]
    [HarmonyPatch(typeof(CameraMovements), nameof(CameraMovements.Sway))]
    private static bool RemoveHeadBobAndSway()
    {
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FirstPersonHeadBob), nameof(FirstPersonHeadBob.SetActive))]
    private static bool DeactivateHeadBob(FirstPersonHeadBob __instance)
    {
        __instance.m_Active = false;
        __instance.m_EnableJumpBob = false;
        return false;
    }

    //Effect glitches
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LightFlicker), nameof(LightFlicker.Update))]    
    private static bool TurnOffLightFlicker(LightFlicker __instance)
    {
        if(!__instance.m_IsOff)
        {
            __instance.TurnOff();
        }
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(VisionEffectController), nameof(VisionEffectController.SetVisionEffect))]
    private static void TurnOffAmplifyPostProcessing(VisionEffectController __instance)
    {
        __instance.PostProcessScript.enabled = false;        
    }

    //Get rid of VR nausea simulator (CH2 Opening)
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CH2OpeningSequenceController), nameof(CH2OpeningSequenceController.DOGetUpSequence))]
    private static bool DontGetUp(CH2OpeningSequenceController __instance, ref Sequence __result)
    {
        Sequence sequence = DOTween.Sequence();
        float num = 1f;
        sequence.InsertCallback(num, delegate
        {
            __instance.m_GameCam.Camera.transform.SetParent(GameManager.Instance.Player.CameraParent);
        });
        sequence.InsertCallback(num += 0.5f, __instance.PlayIntroDialogue_02);
        __result = sequence;        
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CH2ClosingSequenceController), nameof(CH2ClosingSequenceController.HandleFinalTriggerOnEnter))]
    private static bool VRHandleFinalTriggerOnEnter(CH2ClosingSequenceController __instance)
    {
        __instance.m_FinalTrigger.OnEnter -= __instance.HandleFinalTriggerOnEnter;
        GameManager.Instance.HideCrosshair();
        GameManager.Instance.LockPause();
        GameManager.Instance.Player.SetCameraSway(active: true);
        GameManager.Instance.Player.SetLock(active: true);
        //GameManager.Instance.Player.HeadContainer.DOLookAt(__instance.m_FinalLookAt.position, 2f).SetEase(Ease.InOutQuad);
        //VrCore.instance.GetVRPlayerController().mNewCameraParent.DOLookAt(__instance.m_FinalLookAt.position, 2f).SetEase(Ease.InOutQuad);        
        
        //TODO Might remove this completely - since if they turn around physically it will be off. 
        VrCore.instance.GetVRPlayerController().mPlayerController.transform.DOLookAt(__instance.m_FinalLookAt.position, 2f).SetEase(Ease.InOutQuad);
        GameManager.Instance.AudioManager.Play(__instance.m_CanKickClip);
        __instance.DOSequence().OnComplete(__instance.SequenceOnComplete);
        return false;
    }


    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.LateUpdate))]
    private static void RecenterCamera()
    {
        if (isDone) return;
        StageInstance.RecenterPosition(true);
        isDone = true;
    }*/

    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.Start))]
    private static void ResetIsDoneOnCameraStart(vgCameraController __instance)
    {
        isDone = false;
    }*/

    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(vgPlayerNavigationController), nameof(vgPlayerNavigationController.Start))]
    private static void FixFog(vgPlayerNavigationController __instance)
    {
        var stylisticFog = __instance.cameraController.camera.GetComponent<vgStylisticFog>();
        if (!stylisticFog || !stylisticFog.enabled) return;
        stylisticFog.enabled = false;
        stylisticFog.enabled = true;
    }*/

    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnEnable))]
    private static void ResetIsDoneOnLoading(vgLoadingCamera __instance)
    {
        isDone = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgCameraLimit), nameof(vgCameraLimit.SetLimits))]
    private static void PreventCameraVerticalRotation(ref float minVerticalAngle, ref float maxVerticalAngle)
    {
        minVerticalAngle = 0;
        maxVerticalAngle = 0;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgUtils), nameof(vgUtils.GetGameCamera))]
    private static bool ReplaceGetCameraResult(ref Camera __result)
    {
        var camera = StageInstance.GetMainCamera();
        if (!camera) return true;

        __result = camera;
        return false;
    }*/
}