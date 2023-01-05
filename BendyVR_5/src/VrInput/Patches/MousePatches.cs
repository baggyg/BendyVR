using BendyVR_5.src;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BendyVR_5.VrInput.Patches;

[HarmonyPatch]
public class MousePatches : BendyVRPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TitleScreenController), nameof(TitleScreenController.CheckInitialInput))]
    private static void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;        
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Input), nameof(Input.GetAxis))]
    private static bool RemoveMouseInput(ref float __result, string axisName)
    {
        if (axisName.ToLower().Equals("mouse x") || axisName.ToLower().Equals("mouse y"))
        {
            __result = 0f;
            return false;
        }
        return true;
    }


    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(vgCursorManager), nameof(vgCursorManager.Awake))]
    private static bool DestroyCursorManager(vgCursorManager __instance)
    {
        Object.Destroy(__instance);
        return false;
    }*/

    
    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(vgUIInputModule), nameof(vgUIInputModule.ProcessMouseEvent))]
    [HarmonyPatch(typeof(vgUIInputModule), nameof(vgUIInputModule.GetDefaultSelectedGameObject))]
    private static bool DisableMouse(vgUIInputModule __instance)
    {
        return false;
    }*/
}