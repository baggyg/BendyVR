using BendyVR_5.Helpers;
using BendyVR_5.Stage;
using DG.Tweening;
using HarmonyLib;
using S13Audio;
using UnityEngine;
using UnityEngine.PostProcessing;
using TMG.Core;
using System;
using BendyVR_5.src;
using BendyVR_5.Settings;

namespace BendyVR_5.VrCamera.Patches;

[HarmonyPatch]
public class GameCameraPatches : BendyVRPatch
{
    private static bool isDone;
    
    [HarmonyPostfix]    
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.InitChapter1))]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.InitChapter2))]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.InitChapter3))]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.InitChapter4))]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.InitChapter5))]
    private static void TurnOffOcclusionMeshes(GameManager __instance)
    {
        VrCore.instance.GetVRPlayerController().inLevel = true;
        Logs.WriteInfo("Chapter " + __instance.CurrentChapter.name + " loaded");
        //Loop through the whole scene and find mesh renderers
        var foundRenderers = Resources.FindObjectsOfTypeAll(typeof(MeshRenderer));
        foreach (MeshRenderer meshRenderer in foundRenderers)        
        {
            if (meshRenderer.name.ToLower().Contains("ink"))
            {
                meshRenderer.allowOcclusionWhenDynamic = false;
                Logs.WriteInfo("Ink Fix: " + meshRenderer.name);
            }
        }
    }

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
    private static bool RemoveHeadBob()
    {
        return VrSettings.EnableHeadBob.Value;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CameraMovements), nameof(CameraMovements.Sway))]
    private static bool RemoveSway()
    {
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FirstPersonHeadBob), nameof(FirstPersonHeadBob.SetActive))]
    private static bool DeactivateHeadBob(FirstPersonHeadBob __instance)
    {
        if (!VrSettings.EnableHeadBob.Value)
        {
            __instance.m_Active = false;
            __instance.m_EnableJumpBob = false;
            return false;
        }
        return true;
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
}