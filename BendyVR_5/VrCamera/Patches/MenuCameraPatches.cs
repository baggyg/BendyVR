using System;
using HarmonyLib;
using TMG.Core;
using TMG.UI;
using UnityEngine;

namespace BendyVR_5.VrCamera.Patches;

[HarmonyPatch]
public class MenuCameraPatches : BendyVRPatch
{
    //Camera.farClipPlane = 5000;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIManager), nameof(UIManager.Init))]
    private static void SetupMenuCamera(UIManager __instance)
    {
        __instance.Camera.farClipPlane = 5000;
        __instance.Camera.nearClipPlane = 0.01f;
    }

 }