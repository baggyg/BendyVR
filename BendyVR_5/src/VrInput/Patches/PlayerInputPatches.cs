using System.Collections.Generic;
using BepInEx.Configuration;
//using BendyVR_5.Settings;
using BendyVR_5.Stage;
using BendyVR_5.VrInput.ActionInputs;
using UnityEngine;
using Valve.VR;
using InControl;
using TMG.Controls;
using HarmonyLib;
using BendyVR_5.src;

namespace BendyVR_5.VrInput.Patches;

[HarmonyPatch]
public class PlayerInputPatches : BendyVRPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.Any))]
    private static bool Any(ref bool __result)
    {
        __result = ActionInputDefinitions.Weapon.ButtonDown ||
            ActionInputDefinitions.SeeingTool.ButtonDown ||
            ActionInputDefinitions.Run.ButtonDown ||
            ActionInputDefinitions.Jump.ButtonDown ||
            ActionInputDefinitions.Pause.ButtonDown ||
            ActionInputDefinitions.Cancel.ButtonDown ||
            ActionInputDefinitions.Interact.ButtonDown;

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.Attack))]
    private static bool PlayerAttack(ref bool __result)
    {
        __result = ActionInputDefinitions.Weapon.ButtonDown;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.SeeingTool))]
    private static bool PlayerSeeingTool(ref bool __result)
    {
        __result = ActionInputDefinitions.SeeingTool.ButtonDown;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.AttackHold))]
    private static bool PlayerAttackHold(ref bool __result)
    {
        __result = ActionInputDefinitions.Weapon.ButtonValue;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.Run))]
    private static bool PlayerRun(ref bool __result)
    {
        __result = ActionInputDefinitions.Run.ButtonValue;
        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.Jump))]
    private static bool PlayerJump(ref bool __result)
    {
        __result = ActionInputDefinitions.Jump.ButtonDown;
        return false;
    }
        
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.Pause))]
    private static bool PlayerPause(ref bool __result)
    {
        __result = ActionInputDefinitions.Pause.ButtonDown;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.MoveX))]
    private static bool PlayerMoveX(ref float __result)
    {
        __result = ActionInputDefinitions.MoveX.AxisValue;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.MoveY))]
    private static bool PlayerMoveY(ref float __result)
    {
        __result = ActionInputDefinitions.MoveY.AxisValue;
        return false;
    }

    //Not sure what to do about LookX / LookY at the moment
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.LookX))]
    private static bool LookX(ref float __result, float TSpeed = 0f)
    {
        //return BasePlayerInput.GetInputFloat(GamepadInput.GetAxis(InputControlType.RightStickX) * (1.25f + TSpeed), Input.GetAxis("Mouse X"));
        __result = ActionInputDefinitions.RotateX.AxisValue * (1.25f + TSpeed);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.BackOnPressed))]
    private static bool PlayerBackOnPressed(ref bool __result)
    {
        __result = ActionInputDefinitions.Cancel.ButtonDown;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.InteractOnPressed))]
    private static bool PlayerInteractOnPressed(ref bool __result)
    {
        __result = ActionInputDefinitions.Interact.ButtonDown;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.InteractOnReleased))]
    private static bool PlayerInteractOnReleased(ref bool __result)
    {
        __result = ActionInputDefinitions.Interact.ButtonUp;
        return false;
    }
}