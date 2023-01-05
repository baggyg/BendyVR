using System;
using HarmonyLib;
using BendyVR_5.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BendyVR_5.src;

namespace BendyVR_5.UI.Patches;

[HarmonyPatch]
public class CanvasToWorldSpacePatches : BendyVRPatch
{
    private static readonly string[] canvasesToDisable =
    {
        "BlackBars", // Cinematic black bars.
        "Camera" // Disposable camera.
    };

    private static readonly string[] canvasesToIgnore =
    {
        "com.sinai.unityexplorer_Root", // UnityExplorer.
        "com.sinai.unityexplorer.MouseInspector_Root", // UnityExplorer.
        "ExplorerCanvas"
    };

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UIBehaviour), "Awake")]
    private static void UIBehaviourAwake(UIBehaviour __instance)
    {
        LayerHelper.SetLayer(__instance, GameLayer.UI);
    }

    private static bool IsCanvasToIgnore(string canvasName)
    {
        foreach (var s in canvasesToIgnore)
            if (Equals(s, canvasName))
                return true;
        return false;
    }

    private static bool IsCanvasToDisable(string canvasName)
    {
        foreach (var s in canvasesToDisable)
            if (Equals(s, canvasName))
                return true;
        return false;
    }
}