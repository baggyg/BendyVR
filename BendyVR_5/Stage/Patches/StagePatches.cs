using HarmonyLib;
using TMG.Core;
using UnityEngine;
using UnityEngine.VR;

namespace BendyVR_5.Stage.Patches;

[HarmonyPatch]
public class StagePatches : BendyVRPatch
{
    private static GameManager gameManager;
    private static Transform originalParent;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(InitializeGame), nameof(InitializeGame.InitPlayerSettings))]
    private static void CreateStage(InitializeGame __instance)
    {
        gameManager = __instance.m_GameManager;
        // All objects are eventually destroyed, unless they are children of this "reset object".
        // So we make the reset object the parent of the VR Stage, to make sure we keep it alive.
        VrStage.Create(gameManager);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.Init))]
    public static void SetUpStagePlayer(PlayerController __instance)
    {
        // Getting camera manually because cameraController.camera isn't set up yet.
        //var camera = __instance.cameraController.GetComponentInChildren<Camera>();
        var camera = GameManager.Instance.GameCamera.Camera;
        StageInstance.SetUp(camera, __instance);
    }

    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(vgDestroyAllGameObjects), "OnEnter")]
    private static void PreventStageDestructionStart(object __instance)
    {
        originalParent = StageInstance.transform.parent.parent;
        StageInstance.transform.parent.SetParent(resetObject.transform);
    }*/

    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(vgDestroyAllGameObjects), "OnEnter")]
    private static void PreventStageDestructionEnd()
    {
        StageInstance.transform.parent.SetParent(originalParent);
    }*/


}