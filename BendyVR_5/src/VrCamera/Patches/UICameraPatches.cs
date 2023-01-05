using System;
using BendyVR_5.src;
using HarmonyLib;
using TMG.Core;
using TMG.UI;
using UnityEngine;
using UnityEngine.XR;

namespace BendyVR_5.VrCamera.Patches;

[HarmonyPatch]
public class UICameraPatches : BendyVRPatch
{
    //Camera.farClipPlane = 5000;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIManager), nameof(UIManager.Init))]
    private static void SetupMenuCamera(UIManager __instance)
    {
        __instance.Camera.farClipPlane = 5000;
        __instance.Camera.nearClipPlane = 0.01f;

        //Lets give the Camera a Parent and a Dampener
        UICameraManager.Create(__instance.Camera);

        //Set the camera to not be auto tracked
        //XRDevice.DisableAutoXRCameraTracking(__instance.Camera, true);
    }
}