using BendyVR_5.Helpers;
using BendyVR_5.Stage;
using DG.Tweening;
using HarmonyLib;
using S13Audio;
using UnityEngine;
using UnityEngine.PostProcessing;
using TMG.Core;
using System;

namespace BendyVR_5.VrCamera.Patches;

[HarmonyPatch]
public class GameCameraPatches : BendyVRPatch
{
    private static bool isDone;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameCamera), nameof(GameCamera.ExitFreeRoamCam))]
    public static bool VRExitFreeRoamCam(GameCamera __instance)
    {
        VrCore.instance.GetVRPlayerController().ExitFreeRoam();        
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameCamera), nameof(GameCamera.InitializeFreeRoamCam))]
    public static bool VRInitializeFreeRoamCam(ref Transform __result)
    {
        __result = VrCore.instance.GetVRPlayerController().InitializeFreeRoam();        
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LittleMiracleStationController), nameof(LittleMiracleStationController.HandleDoorInteractableOnInteracted))]
    private static bool MiracleStationEntry(LittleMiracleStationController __instance)
    {
        GameManager.Instance.ShowScreenBlocker(0.3f);
        return true;
    }

    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(LittleMiracleStationController), nameof(LittleMiracleStationController.HandleDoorInteractableOnInteracted))]
    private static bool MiracleStationEntry(LittleMiracleStationController __instance)
    {
        Logs.WriteWarning("INITIALISING MIRACLE STATION");
        __instance.m_DoorInteractable.OnInteracted -= __instance.HandleDoorInteractableOnInteracted;
        GameManager.Instance.CurrentChapter.DeathController.OnDeath += __instance.HandlePlayerOnDeath;
        GameManager.Instance.CurrentChapter.DeathController.OnSpawned += __instance.HandlePlayerOnSpawned;
        __instance.m_Player = GameManager.Instance.Player;
        __instance.m_Player.SetLockedMovement(active: true);
        __instance.m_Player.SetCollision(active: false);
        GameManager.Instance.HideCrosshair();

        Transform freeRoamCam = GameManager.Instance.GameCamera.FreeRoamCam;
        freeRoamCam.position = GameManager.Instance.GameCamera.CameraContainer.position;
        freeRoamCam.rotation = GameManager.Instance.GameCamera.CameraContainer.rotation;

        //Check we don't already have the offset (so we don't create one million 543 thousand game objects)
        Transform cameraOffset = freeRoamCam.Find("MiracleCameraOffset");
        if(cameraOffset == null)
            cameraOffset = new GameObject("MiracleCameraOffset").transform;
        cameraOffset.SetParent(freeRoamCam);
        cameraOffset.localRotation = Quaternion.identity;
        //cameraOffset.localPosition = new Vector3(0f,-GameManager.Instance.GameCamera.Camera.transform.localPosition.y, 0f);
        cameraOffset.localPosition = -GameManager.Instance.GameCamera.Camera.transform.localPosition;

        GameManager.Instance.GameCamera.CameraContainer.SetParent(freeRoamCam);
        GameManager.Instance.GameCamera.CameraContainer.localScale = Vector3.one;
        GameManager.Instance.GameCamera.Camera.transform.SetParent(cameraOffset);
        VrCore.instance.GetVRPlayerController().inFreeRoam = true;
        VrCore.instance.GetVRPlayerController().mDominantHand.SetParent(cameraOffset);
        VrCore.instance.GetVRPlayerController().mNonDominantHand.SetParent(cameraOffset);

        //Removing Follow
        VrCore.instance.GetVRPlayerController().RemoveCameraFollow();
        GameManager.Instance.ShowScreenBlocker(0.3f);
        Sequence sequence = DOTween.Sequence();
        float num = 0f;        
        if (GameManager.Instance.Player.WeaponGameObject != null)
        {
            sequence.Insert(num, GameManager.Instance.Player.WeaponGameObject.transform.DOLocalMoveY(-5f, 0.25f).SetEase(Ease.InQuad));
            sequence.Insert(num, GameManager.Instance.Player.WeaponGameObject.transform.DOLocalRotate(new Vector3(180f, 0f, 0f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.InQuad));
            sequence.InsertCallback(num + 0.25f, delegate
            {
                GameManager.Instance.Player.WeaponGameObject.SetActive(value: false);
                GameManager.Instance.Player.UnEquipWeapon();
            });
        }
        num += 0.25f;
        sequence.Insert(num, freeRoamCam.DOMove(__instance.m_BackupPosition.position, 0.4f).SetEase(Ease.InOutQuad));
        sequence.Insert(num, freeRoamCam.DORotate(__instance.m_BackupPosition.eulerAngles, 0.4f).SetEase(Ease.InOutQuad));
        num += 0.4f;
        sequence.Insert(num, __instance.m_Door.DOLocalRotate(new Vector3(0f, 90f, 0f), 0.8f).SetEase(Ease.InOutQuad));
        S13AudioManager.Instance.InvokeEvent("evt_miracle_station_enter");
        num += 0.5f;
        sequence.Insert(num, freeRoamCam.DOMove(__instance.m_CameraLook.position, 0.65f).SetEase(Ease.InOutQuad));
        sequence.Insert(num, freeRoamCam.DORotate(__instance.m_CameraLook.eulerAngles, 0.7f).SetEase(Ease.InOutQuad));
        num += 0.25f;
        sequence.Insert(num, __instance.m_Door.DOLocalRotate(Vector3.zero, 0.7f).SetEase(Ease.InOutQuad));
        sequence.OnComplete(__instance.HandleEntranceOnComplete);
        TMGExtensions.Send(__instance.OnInteract, __instance);
        __instance.OnInteract.Send(__instance);
        __instance.OnInteract?.Invoke(__instance, EventArgs.Empty);


        return false;
    }*/

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LittleMiracleStationController), nameof(LittleMiracleStationController.HandleEntranceOnComplete))]
    private static void TurnOffHands()
    {
        Transform freeRoamCam = GameManager.Instance.GameCamera.FreeRoamCam;
        //Check we don't already have the offset (so we don't create one million 543 thousand game objects)
        Transform cameraOffset = freeRoamCam.Find("MiracleCameraOffset");
        if (cameraOffset == null)
            cameraOffset = new GameObject("MiracleCameraOffset").transform;
        cameraOffset.SetParent(freeRoamCam);
        cameraOffset.localRotation = Quaternion.identity;
        //cameraOffset.localPosition = new Vector3(0f,-GameManager.Instance.GameCamera.Camera.transform.localPosition.y, 0f);
        cameraOffset.localPosition = -GameManager.Instance.GameCamera.Camera.transform.localPosition;

        GameManager.Instance.GameCamera.CameraContainer.SetParent(freeRoamCam);
        GameManager.Instance.GameCamera.CameraContainer.localScale = Vector3.one;
        GameManager.Instance.GameCamera.Camera.transform.SetParent(cameraOffset);
        VrCore.instance.GetVRPlayerController().inFreeRoam = true;
        VrCore.instance.GetVRPlayerController().mDominantHand.SetParent(cameraOffset);
        VrCore.instance.GetVRPlayerController().mNonDominantHand.SetParent(cameraOffset);

        //Removing Follow
        VrCore.instance.GetVRPlayerController().RemoveCameraFollow();        

        //GB New also need to move the camera there (and the hands)        
        Logs.WriteWarning("Turning Off Both hands");
        VrCore.instance.GetVRPlayerController().TurnOffBothHands();
        GameManager.Instance.HideScreenBlocker(0f);
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(LittleMiracleStationController), nameof(LittleMiracleStationController.HandleExitOnComplete))]
    private static void MiracleStationExit(LittleMiracleStationController __instance)
    {
        Logs.WriteWarning("EXITING MIRACLE STATION");
        VrCore vrCore = VrCore.instance;
        vrCore.GetVRPlayerController().SetupCameraHierarchy();
        vrCore.GetVRPlayerController().SetupCameraFollow();
        vrCore.GetVRPlayerController().ParentHands();
        if (GameManager.Instance.Player.WeaponGameObject == null)
            VrCore.instance.GetVRPlayerController().TurnOnBothHands();
        else
            VrCore.instance.GetVRPlayerController().TurnOnNonDominantHand();
        VrCore.instance.GetVRPlayerController().inFreeRoam = false;
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