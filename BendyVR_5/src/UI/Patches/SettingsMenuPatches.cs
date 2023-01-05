using BendyVR_5.Helpers;
using HarmonyLib;
using UnityEngine;

// Some of the available game settings don't go well with VR.
// These patches force some settings to certain values to prevent VR funkyness.
namespace BendyVR_5.UI.Patches;

[HarmonyPatch]
public class SettingsMenuPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(OptionsMenuController), nameof(OptionsMenuController.ShowAdvancedMenu))]
    private static void RepositionAdvanced(OptionsMenuController __instance)
    {
        var advanced = GameObject.Find("Visuals/AdvancedMenu");
        Logs.WriteWarning("Found: " + advanced.name);
        RectTransform rt = advanced.GetComponent<RectTransform>();
        rt.position = new Vector3(0f, 150f, -260f);
    }
}