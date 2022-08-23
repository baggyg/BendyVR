using HarmonyLib;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace BendyVR_5.VrCamera.Patches;

[HarmonyPatch]
public class GameCameraPatches : BendyVRPatch
{
    private static bool isDone;

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