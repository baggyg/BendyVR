using BendyVR_5.Helpers;
using BendyVR_5.UI;
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
        
        //Also Turn Off tracking of the main camera
        //XRDevice.DisableAutoXRCameraTracking(GameManager.Instance.GameCamera.WeaponCamera, true);

        var camera = GameManager.Instance.GameCamera.Camera;
        Logs.WriteInfo("Setting Up VR Core (PlayerController Initialised)");
        StageInstance.SetUp(camera, __instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CH2OpeningSequenceController), nameof(CH2OpeningSequenceController.Activate))]
    [HarmonyPatch(typeof(CH3OpeningSequenceController), nameof(CH3OpeningSequenceController.Activate))]
    [HarmonyPatch(typeof(CH4OpeningSequenceController), nameof(CH4OpeningSequenceController.Activate))]
    [HarmonyPatch(typeof(CH5OpeningSequenceController), nameof(CH5OpeningSequenceController.Activate))]
    public static bool EndDemo()
    {
        ErrorPresenter.Show("End of the Early Access 1 Demo. Your game has been saved!");
        
        return false;
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