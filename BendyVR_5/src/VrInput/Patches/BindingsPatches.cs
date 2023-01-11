using System.Collections.Generic;
using System.IO;
using BendyVR_5.src;
using BepInEx;
using HarmonyLib;
//using BendyVR_5.Settings;
using UnityEngine;
using Valve.VR;

namespace BendyVR_5.VrInput.Patches;

[HarmonyPatch]
public class BindingsPatches : BendyVRPatch
{
    private const float outerDeadzone = 0.5f;
    private const float innerDeadzone = 0.1f;

    private static readonly HashSet<string> ignoredVirtualKeys = new()
    {
        VirtualKey.ScrollUpDown,
        VirtualKey.Melee
    };

    private static readonly Dictionary<string, Dictionary<string, string>> replacementCommandMap =
        new()
        {
            {
                VirtualKey.DialogDown,
                new Dictionary<string, string>
                {
                    // Fixes UIDown triggering interact.
                    {CommandName.DialogSelectionDownOrUse, CommandName.DialogSelectionDown},

                    // Fixes UIDown triggering lock tumbler right..
                    {CommandName.LockTumblerRight, CommandName.None}
                }
            },
            // Keyboard move keys are needed to interact with locks and UI stuff,
            // but need to prevent them from triggering player movement.
            {
                VirtualKey.MoveBackwardKeyboard,
                new Dictionary<string, string>
                {
                    {CommandName.BackwardKeyDown, CommandName.None},
                    {CommandName.BackwardKeyUp, CommandName.None}
                }
            },
            {
                VirtualKey.MoveForwardKeyboard,
                new Dictionary<string, string>
                {
                    {CommandName.ForwardKeyDown, CommandName.None},
                    {CommandName.ForwardKeyUp, CommandName.None}
                }
            },
            {
                VirtualKey.StrafeLeftKeyboard,
                new Dictionary<string, string>
                {
                    {CommandName.StrafeLeftKeyDown, CommandName.None},
                    {CommandName.StrafeLeftKeyUp, CommandName.None}
                }
            },
            {
                VirtualKey.StrafeRightKeyboard,
                new Dictionary<string, string>
                {
                    {CommandName.StrafeRightKeyDown, CommandName.None},
                    {CommandName.StrafeRightKeyUp, CommandName.None}
                }
            }
        };



    [HarmonyPrefix]
    [HarmonyPatch(typeof(SteamVR_Input), nameof(SteamVR_Input.GetActionsFileFolder))]
    private static bool GetActionsFileFromMod(ref string __result)
    {
        __result = Path.Combine(Paths.PluginPath, Path.Combine("BendyVRAssets", "Bindings"));
        //__result = $"{Directory.GetCurrentDirectory()}/BepInEx/plugins/baggyg-bendyvr/BendyVRAssets/Bindings";
        return false;
    }
    
    private static float ProcessAxisValue(float value)
    {
        var valueSign = Mathf.Sign(value);
        var absoluteValue = Mathf.Abs(value);
        return valueSign * Mathf.InverseLerp(innerDeadzone, 1f - outerDeadzone, absoluteValue);
    }
}