﻿using BendyVR_5.Helpers;
using BendyVR_5.Settings;
using HarmonyLib;
using InControl;
using TMG.Data;
using UnityEngine;

// Some of the available game settings don't go well with VR.
// These patches force some settings to certain values to prevent VR funkyness.
namespace TwoForksVr.Settings.Patches;

[HarmonyPatch]
public class GameSettingsPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(InputManager), nameof(InputManager.InvertYAxis), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.MotionBlur), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.ViewBobbing), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.ViewSwaying), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.AA), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.Inverted), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.Crosshair), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.VolumetricLighting), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.Grain), MethodType.Setter)]
    private static void ForceDisableBoolSetting(ref bool value)
    {
        value = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(OptionsMenuController), nameof(OptionsMenuController.GenerateAdvancedMenu))]
    private static bool GenerateNewAdvancedMenu(OptionsMenuController __instance)
    {
        __instance.GenerateCategory(__instance.m_AdvancedMenuParent, "VR Options");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "Snap Turn", !VrSettings.SnapTurning.Value ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "Turn Angle", VrSettings.GetSnapTurnAngle());
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "Smooth Rotation Speed", VrSettings.SmoothRotationSpeed.Value.ToString());
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "Controller Direction Movement", !VrSettings.ControllerBasedMovementDirection.Value ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "Left Handed Mode", !VrSettings.LeftHandedMode.Value ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "Swap Sticks", !VrSettings.SwapSticks.Value ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "World Scale", VrSettings.WorldScale.Value.ToString());


        __instance.GenerateCategory(__instance.m_AdvancedMenuParent, "Advanced Options");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "MENU/ADVANCED_FULLSCREEN", (!GameManager.Instance.PlayerSettings.Fullscreen) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        //__instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "MENU/ADVANCED_RESOLUTION", __instance.m_OptionControls.HandleResolutionValeChange(GameManager.Instance.PlayerSettings.CurrentResolution), hasTranslation: false);
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "MENU/ADVNACED_QUALITY", __instance.GetQuality());
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "MENU/ADVANCED_AA", (!GameManager.Instance.PlayerSettings.AA) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "MENU/ADVANCED_VSYNC", (!GameManager.Instance.PlayerSettings.VSync) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "MENU/ADVANCED_DOF", (!GameManager.Instance.PlayerSettings.DoF) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "MENU/ADVANCED_BLOOM", (!GameManager.Instance.PlayerSettings.Bloom) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "MENU/ADVANCED_AO", (!GameManager.Instance.PlayerSettings.AmbientOcclusion) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        /*__instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "MENU/ADVANCED_MOTION_BLUR", (!GameManager.Instance.PlayerSettings.MotionBlur) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "MENU/ADVANCED_FILM_GRAIN", (!GameManager.Instance.PlayerSettings.Grain) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");*/
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(OptionsMenuController), nameof(OptionsMenuController.CheckAdvancedMenuItem))]
    private static bool CheckNewAdvancedMenuItem(OptionsMenuController __instance, bool isRight)
    {
        int num = (isRight ? 1 : (-1));
        switch (__instance.m_SelectedIndex)
        {
            //Snap Turn
            //Snap Turn Value
            //SmoothRotationSpeed
            //ControllerBasedMovementDirection
            //LeftHandedMode
            //SwapSticks
            //WorldScale
            case 0:
                Logs.WriteInfo("SnapTurning is " + VrSettings.SnapTurning.Value.ToString());
                VrSettings.SnapTurning.Value = !VrSettings.SnapTurning.Value;
                Logs.WriteInfo("SnapTurning is now " + VrSettings.SnapTurning.Value.ToString());
                __instance.m_AdvancedMenuOptions[0].UpdateValue((!VrSettings.SnapTurning.Value) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 1:
                Logs.WriteInfo("Snap Turn Angle is " + VrSettings.SnapTurnAngle.Value.ToString());
                VrSettings.UpdateSnapTurnAngle(isRight);
                Logs.WriteInfo("SnapTurning is now " + VrSettings.SnapTurnAngle.Value.ToString());
                __instance.m_AdvancedMenuOptions[0].UpdateValue(VrSettings.GetSnapTurnAngle());
                break;
            case 2:
                Logs.WriteInfo("Smooth Rotation Speed is " + VrSettings.SmoothRotationSpeed.Value.ToString());
                VrSettings.UpdateSnapTurnAngle(isRight);
                Logs.WriteInfo("Smooth Rotation Speed is now " + VrSettings.SmoothRotationSpeed.Value.ToString());
                __instance.m_AdvancedMenuOptions[0].UpdateValue(VrSettings.SmoothRotationSpeed.Value.ToString());
                break;
            case 3:
                Logs.WriteInfo("Controller Movement is " + VrSettings.ControllerBasedMovementDirection.Value.ToString());
                VrSettings.ControllerBasedMovementDirection.Value = !VrSettings.ControllerBasedMovementDirection.Value;
                Logs.WriteInfo("Controller Movement is now " + VrSettings.ControllerBasedMovementDirection.Value.ToString());
                __instance.m_AdvancedMenuOptions[0].UpdateValue((!VrSettings.ControllerBasedMovementDirection.Value) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 4:
                Logs.WriteInfo("LeftHandedMode is " + VrSettings.LeftHandedMode.Value.ToString());
                VrSettings.LeftHandedMode.Value = !VrSettings.LeftHandedMode.Value;
                Logs.WriteInfo("LeftHandedMode is now " + VrSettings.LeftHandedMode.Value.ToString());
                __instance.m_AdvancedMenuOptions[0].UpdateValue((!VrSettings.LeftHandedMode.Value) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 5:
                Logs.WriteInfo("SwapSticks is " + VrSettings.SwapSticks.Value.ToString());
                VrSettings.SwapSticks.Value = !VrSettings.SwapSticks.Value;
                Logs.WriteInfo("SwapSticks is now " + VrSettings.SwapSticks.Value.ToString());
                __instance.m_AdvancedMenuOptions[0].UpdateValue((!VrSettings.SwapSticks.Value) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 6:
                Logs.WriteInfo("WorldScale is " + VrSettings.WorldScale.Value.ToString());
                VrSettings.UpdateWorldScale(isRight);
                Logs.WriteInfo("WorldScale is now " + VrSettings.WorldScale.Value.ToString());
                __instance.m_AdvancedMenuOptions[0].UpdateValue(VrSettings.WorldScale.Value.ToString());
                break;
            case 7:
                __instance.m_OptionControls.HandleFullscreenValueChange();
                __instance.m_AdvancedMenuOptions[0].UpdateValue((!GameManager.Instance.PlayerSettings.Fullscreen) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            /*case 1:
                {
                    int currentResolution = GameManager.Instance.PlayerSettings.CurrentResolution;
                    int num2 = currentResolution + num;
                    if (num2 < 0)
                    {
                        num2 = GameManager.Instance.PlayerSettings.Resolutions.Count - 1;
                    }
                    else if (num2 >= GameManager.Instance.PlayerSettings.Resolutions.Count)
                    {
                        num2 = 0;
                    }
                    m_AdvancedMenuOptions[1].UpdateValue(m_OptionControls.HandleResolutionValeChange(num2));
                    break;
                }*/
            case 8:
                {
                    int currentQuality = GameManager.Instance.PlayerSettings.currentQuality;
                    int num3 = currentQuality + num;
                    if (num3 < 0)
                    {
                        num3 = 3;
                    }
                    else if (num3 > 3)
                    {
                        num3 = 0;
                    }
                    __instance.m_AdvancedMenuOptions[2].UpdateValue(__instance.m_OptionControls.UpdateQuality(num3));
                    break;
                }
            case 9:
                __instance.m_OptionControls.HandleAAValueChange();
                __instance.m_AdvancedMenuOptions[3].UpdateValue((!GameManager.Instance.PlayerSettings.AA) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 10:
                __instance.m_OptionControls.HandleVSyncValueChange();
                __instance.m_AdvancedMenuOptions[4].UpdateValue((!GameManager.Instance.PlayerSettings.VSync) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 11:
                __instance.m_OptionControls.HandleDOFValueChange();
                __instance.m_AdvancedMenuOptions[5].UpdateValue((!GameManager.Instance.PlayerSettings.DoF) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 12:
                __instance.m_OptionControls.HandleBloomValueChange();
                __instance.m_AdvancedMenuOptions[6].UpdateValue((!GameManager.Instance.PlayerSettings.Bloom) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 13:
                __instance.m_OptionControls.HandleAmbientOcclusionValueChange();
                __instance.m_AdvancedMenuOptions[7].UpdateValue((!GameManager.Instance.PlayerSettings.AmbientOcclusion) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            /*case 8:
                m_OptionControls.HandleMotionBlurValueChange();
                m_AdvancedMenuOptions[8].UpdateValue((!GameManager.Instance.PlayerSettings.MotionBlur) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 9:
                m_OptionControls.HandleGrainValueChange();
                m_AdvancedMenuOptions[9].UpdateValue((!GameManager.Instance.PlayerSettings.Grain) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;*/
        }
        return false;
    }


    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(OptionsMenuController), nameof(OptionsMenuController.GenerateOptionsMenu))]
    private static void AddVRMenu(OptionsMenuController __instance)
    {
        __instance.GetButtonData(ref __instance.m_AdvancedBtn, ref __instance.m_OptionsMenuItemButtons, "MENU/MENU_VRSETTINGS", __instance.CheckSelectedItem);
    }*/
          

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.Initialize))]
    private static void OverrideDefaultSettings(PlayerSettings __instance)
    {
        Debug.Log("Forcing Inverted");
        PlayerPrefsManager.Save("INVERTED", 0);
        Debug.Log("Forcing Crosshair");
        PlayerPrefsManager.Save("CROSSHAIR", 0);
        Debug.Log("Forcing Volumetric Lighting");
        PlayerPrefsManager.Save("VOLUMETRIC_LIGHT", 0);
        Debug.Log("Forcing Volumetric Lighting");
        PlayerPrefsManager.Save("VOLUMETRIC_LIGHT", 0);
        Debug.Log("Forcing Grain");
        PlayerPrefsManager.Save("GRAIN", 0);
        Debug.Log("Forcing Motion Blur");
        PlayerPrefsManager.Save("MOTION_BLUR", 0);
        Debug.Log("Forcing View Bobbing");
        PlayerPrefsManager.Save("VIEW_BOBBING", 0);
        Debug.Log("Forcing View Swaying");
        PlayerPrefsManager.Save("VIEW_SWAYING", 0);
    }
}