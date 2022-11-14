using HarmonyLib;
using BendyVR_5.Helpers;
using UnityEngine;
using TMG.Controls;
using BendyVR_5.Stage;

namespace BendyVR_5.UI.Patches;

[HarmonyPatch]
public static class PauseMenuPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameMenuController), nameof(GameMenuController.Quit))]
    private static void NotifyQuit(GameMenuController __instance)
    {
        //VrCore.instance.quitTriggered = true;
        //TODO - What the hell is this doing? Need to debug the whole thing
    }

		
}