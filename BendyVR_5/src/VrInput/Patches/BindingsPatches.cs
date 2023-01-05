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
    

    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.ForwardMovement))]
    private static bool FixForwardMovement(PlayerController __instance, float axisValue)
    {
        if (VrSettings.Teleport.Value && __instance.navController && __instance.navController.enabled) return false;

        __instance.forwardInput = ProcessAxisValue(axisValue);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.StrafeMovement))]
    private static bool FixStrafeMovement(PlayerController __instance, float axisValue)
    {
        if (VrSettings.Teleport.Value) return false;

        __instance.strafeInput = ProcessAxisValue(axisValue);
        return false;
    }*/

    private static float ProcessAxisValue(float value)
    {
        var valueSign = Mathf.Sign(value);
        var absoluteValue = Mathf.Abs(value);
        return valueSign * Mathf.InverseLerp(innerDeadzone, 1f - outerDeadzone, absoluteValue);
    }

    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.HasController))]
    private static void ForceGamepadEnabled(ref bool __result)    
    {
        __result = true;
    }*/

    /*
     * [HarmonyPrefix]
    [HarmonyPatch(typeof(vgRewiredInput), nameof(vgRewiredInput.GetButtonUp))]
    private static bool ReadVrButtonInputUp(ref bool __result, string id)
    {
        __result = StageInstance.GetInputUp(id);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgRewiredInput), nameof(vgRewiredInput.GetAxis))]
    private static bool ReadVrAxisInput(ref float __result, string id)
    {
        __result = StageInstance.GetInputValue(id);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgRewiredInput), nameof(vgRewiredInput.GetButtonDown))]
    private static bool ReadVrButtonInputDown(ref bool __result, string id)
    {
        __result = StageInstance.GetInputDown(id);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgRewiredInput), nameof(vgRewiredInput.GetButton))]
    private static bool ReadVrButtonInputValue(ref bool __result, string id)
    {
        __result = StageInstance.GetInputValue(id) != 0;
        return false;
    }

    // Forcing Rewired to always handle input makes it easier to patch button input values,
    // and also helps with patching the input prompt icons.
    [HarmonyPostfix]
    [HarmonyPatch(typeof(vgRewiredInput), nameof(vgRewiredInput.IsHandlingInput))]
    private static void ForceRewiredHandlingInput(ref bool __result)
    {
        __result = true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(InputManager), nameof(InputManager.ProcessContextStack))]
    private static void FixCommandsAhhhhhhh(LinkedList<vgInputContext> stack)
    {
        foreach (var inputContext in stack)
        foreach (var commandMapping in inputContext.commandMap)
        {
            if (ignoredVirtualKeys.Contains(commandMapping.virtualKey))
            {
                commandMapping.commands.Clear();
                continue;
            }

            replacementCommandMap.TryGetValue(commandMapping.virtualKey, out var commandReplacements);
            if (commandReplacements == null) continue;

            for (var i = 0; i < commandMapping.commands.Count; i++)
            {
                var command = commandMapping.commands[i];
                commandReplacements.TryGetValue(command.command, out var replacementCommand);
                switch (replacementCommand)
                {
                    case null:
                        continue;
                    case CommandName.None:
                        commandMapping.commands.RemoveAt(i);
                        continue;
                    default:
                        commandMapping.commands[i].command = replacementCommand;
                        break;
                }
            }
        }
    }*/
}