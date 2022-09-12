using BendyVR_5.Helpers;
using HarmonyLib;
using TMG.Core;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.XR;

namespace BendyVR_5.Stage.Patches;

[HarmonyPatch]
public class StagePatches : BendyVRPatch
{
    private static GameManager gameManager;
    private static Transform originalParent;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(InitializeGame), nameof(InitializeGame.InitPlayerSettings))]
    private static void CreateVRCore(InitializeGame __instance)
    {        
        gameManager = __instance.m_GameManager;
        VrCore.Create(gameManager);        
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.Init))]
    public static void SetUpStagePlayer(PlayerController __instance)
    {
        // Getting camera manually because cameraController.camera isn't set up yet.
        //var camera = __instance.cameraController.GetComponentInChildren<Camera>();

        //First Turn off the tracking of the weapon camera        
        XRDevice.DisableAutoXRCameraTracking(GameManager.Instance.GameCamera.WeaponCamera, true);
        
        var camera = GameManager.Instance.GameCamera.Camera;
        Logs.WriteInfo("Setting Up VR Core (PlayerController Initialised)");
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