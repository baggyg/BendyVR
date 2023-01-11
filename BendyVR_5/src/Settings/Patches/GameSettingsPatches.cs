using BendyVR_5.Helpers;
using BendyVR_5.Settings;
using HarmonyLib;
using I2.Loc;
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
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.Initialize))]
    public static bool VRInitializeSettings(PlayerSettings __instance)
    {
        //GB Updated one so I could reset prefs
        if (!PlayerPrefs.HasKey("v1.5.0.1"))
        {
            Debug.Log("Resetting Player Prefs");
            PlayerPrefs.DeleteAll();
            PlayerPrefsManager.Save("v1.5.0.1", 1);
        }
        if (!PlayerPrefs.HasKey("BRIGHTNESS"))
        {
            Debug.Log("Initializing Brightness");
            PlayerPrefsManager.Save("BRIGHTNESS", 0.5f);
        }
        if (!PlayerPrefs.HasKey("QUALITY"))
        {
            Debug.Log("Initializing Quality");
            PlayerPrefsManager.Save("QUALITY", 3);
        }
        if (!PlayerPrefs.HasKey("VOLUME"))
        {
            Debug.Log("Initializing Volume");
            PlayerPrefsManager.Save("VOLUME", 1f);
        }
        if (!PlayerPrefs.HasKey("MUSIC_VOLUME"))
        {
            Debug.Log("Initializing Music Volume");
            PlayerPrefsManager.Save("MUSIC_VOLUME", 0.95f);
        }
        if (!PlayerPrefs.HasKey("SFX_VOLUME"))
        {
            Debug.Log("Initializing SFX Volume");
            PlayerPrefsManager.Save("SFX_VOLUME", 0.95f);
        }
        if (!PlayerPrefs.HasKey("DIALOGUE_VOLUME"))
        {
            Debug.Log("Initializing Dialogue Volume");
            PlayerPrefsManager.Save("DIALOGUE_VOLUME", 0.95f);
        }
        if (!PlayerPrefs.HasKey("SUBTITLES"))
        {
            Debug.Log("Initializing Subtitles");
            PlayerPrefsManager.Save("SUBTITLES", 0);
        }
        if (!PlayerPrefs.HasKey("LANGUAGE"))
        {
            Debug.Log("Initializing Language");
            string value = (LocalizationManager.CurrentLanguageCode = LocalizationManager.GetLanguageCode(LocalizationManager.GetCurrentDeviceLanguage()));
            PlayerPrefsManager.Save("LANGUAGE", value);
        }
        if (!PlayerPrefs.HasKey("INVERTED"))
        {
            Debug.Log("Initializing Inverted");
            PlayerPrefsManager.Save("INVERTED", 0);
        }
        if (!PlayerPrefs.HasKey("CROSSHAIR"))
        {
            Debug.Log("Initializing Crosshair");
            PlayerPrefsManager.Save("CROSSHAIR", 0);
        }
        if (!PlayerPrefs.HasKey("SENSITIVITY"))
        {
            Debug.Log("Initializing Look Sensitivity");
            PlayerPrefsManager.Save("SENSITIVITY", 0.25f);
        }
        if (!PlayerPrefs.HasKey("BLOOM"))
        {
            Debug.Log("Initializing Bloom Lighting");
            PlayerPrefsManager.Save("BLOOM", 1);
        }
        if (!PlayerPrefs.HasKey("DOF"))
        {
            Debug.Log("Initializing Depth of Field");
            PlayerPrefsManager.Save("DOF", 0);
        }
        if (!PlayerPrefs.HasKey("VOLUMETRIC_LIGHT"))
        {
            Debug.Log("Initializing Volumetric Lighting");
            PlayerPrefsManager.Save("VOLUMETRIC_LIGHT", 0);
        }
        if (!PlayerPrefs.HasKey("AMBIENT_OCCLUSION"))
        {
            Debug.Log("Initializing Ambient Occlusion");
            PlayerPrefsManager.Save("AMBIENT_OCCLUSION", 0);
        }
        if (!PlayerPrefs.HasKey("GRAIN"))
        {
            Debug.Log("Initializing Grain");
            PlayerPrefsManager.Save("GRAIN", 0);
        }
        if (!PlayerPrefs.HasKey("MOTION_BLUR"))
        {
            Debug.Log("Initializing Motion Blur");
            PlayerPrefsManager.Save("MOTION_BLUR", 0);
        }
        if (!PlayerPrefs.HasKey("ANTI_ALIASING"))
        {
            Debug.Log("Initializing Anti Aliasing");
            PlayerPrefsManager.Save("ANTI_ALIASING", 1);
        }
        if (!PlayerPrefs.HasKey("V_SYNC"))
        {
            Debug.Log("Initializing V-Sync");
            PlayerPrefsManager.Save("V_SYNC", 1);
        }
        if (!PlayerPrefs.HasKey("VIEW_BOBBING"))
        {
            Debug.Log("Initializing View Bobbing");
            PlayerPrefsManager.Save("VIEW_BOBBING", 0);
        }
        if (!PlayerPrefs.HasKey("VIEW_SWAYING"))
        {
            Debug.Log("Initializing View Swaying");
            PlayerPrefsManager.Save("VIEW_SWAYING", 0);
        }
        if (!PlayerPrefs.HasKey("SHADOWS"))
        {
            Debug.Log("Initializing Shadow");
            PlayerPrefsManager.Save("SHADOWS", 1);
        }
        if (!PlayerPrefs.HasKey("FOG"))
        {
            Debug.Log("Initializing Fog");
            PlayerPrefsManager.Save("FOG", 1);
        }
        if (!PlayerPrefs.HasKey("DUST_PARTICLES"))
        {
            Debug.Log("Initializing Dust Particles");
            PlayerPrefsManager.Save("DUST_PARTICLES", 1);
        }
        if (!PlayerPrefs.HasKey("TIPS"))
        {
            Debug.Log("Initializing TIPS");
            PlayerPrefsManager.Save("TIPS", 1);
        }

        //Overside according the config
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
        Debug.Log("Forcing View Swaying");
        PlayerPrefsManager.Save("VIEW_SWAYING", 0);
        PlayerPrefsManager.Save("FOG", VrSettings.EnableParticles.Value ? 1 : 0);
        PlayerPrefsManager.Save("DUST_PARTICLES", VrSettings.EnableParticles.Value ? 1 : 0);
        PlayerPrefsManager.Save("VIEW_BOBBING", VrSettings.EnableHeadBob.Value ? 1 : 0);

        LocalizationManager.CurrentLanguageCode = GameManager.Instance.PlayerSettings.Language;
        __instance.InitializeQuality();
        __instance.InitializeResolution();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(InputManager), nameof(InputManager.InvertYAxis), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.MotionBlur), MethodType.Setter)]
    //[HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.ViewBobbing), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.ViewSwaying), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.Inverted), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.Crosshair), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.VolumetricLighting), MethodType.Setter)]
    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.Grain), MethodType.Setter)]
    private static bool ForceDisableBoolSetting(ref bool value)
    {
        value = false;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(OptionsControls), nameof(OptionsControls.HandleViewBobbingValueChange))]
    public static bool HandleViewBobbingValueChange()
    {        
        GameManager.Instance.PlayerSettings.ViewBobbing = !GameManager.Instance.PlayerSettings.ViewBobbing;
        VrSettings.EnableHeadBob.Value = GameManager.Instance.PlayerSettings.ViewBobbing;
        return false;
    }

    /*[HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.ViewBobbing), MethodType.Setter)]
    private static bool AlignViewBobbing(ref bool value)
    {
        value = VrSettings.EnableHeadBob.Value;
        return false;
    }

    [HarmonyPatch(typeof(PlayerSettings), nameof(PlayerSettings.ViewBobbing), MethodType.Getter)]
    private static bool GetViewBobbing(ref bool value)
    {
        value = VrSettings.EnableHeadBob.Value;
        return false;
    }*/

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
        //__instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "World Scale", VrSettings.WorldScale.Value.ToString());
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "Show Laser Pointer", !VrSettings.ShowLaserPointer.Value ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "HeightOffset", (Mathf.Round(VrSettings.HeightOffset.Value * 10.0f) * 0.1f).ToString());
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "velocityTrigger", (Mathf.Round(VrSettings.VelocityTrigger.Value * 10.0f) * 0.1f).ToString());
        __instance.CreateOption(ref __instance.m_AdvancedMenuOptions, __instance.m_AdvancedMenuParent, "angularVelocityTrigger", (Mathf.Round(VrSettings.AngularVelocityTrigger.Value * 10.0f) * 0.1f).ToString());


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
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((!VrSettings.SnapTurning.Value) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 1:
                Logs.WriteInfo("Snap Turn Angle is " + VrSettings.SnapTurnAngle.Value.ToString());
                VrSettings.UpdateSnapTurnAngle(isRight);
                Logs.WriteInfo("SnapTurning is now " + VrSettings.SnapTurnAngle.Value.ToString());
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue(VrSettings.GetSnapTurnAngle());
                break;
            case 2:
                Logs.WriteInfo("Smooth Rotation Speed is " + VrSettings.SmoothRotationSpeed.Value.ToString());
                VrSettings.UpdateSmoothTurnSpeed(isRight);
                Logs.WriteInfo("Smooth Rotation Speed is now " + VrSettings.SmoothRotationSpeed.Value.ToString());
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue(VrSettings.SmoothRotationSpeed.Value.ToString());
                break;
            case 3:
                Logs.WriteInfo("Controller Movement is " + VrSettings.ControllerBasedMovementDirection.Value.ToString());
                VrSettings.ControllerBasedMovementDirection.Value = !VrSettings.ControllerBasedMovementDirection.Value;
                Logs.WriteInfo("Controller Movement is now " + VrSettings.ControllerBasedMovementDirection.Value.ToString());
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((!VrSettings.ControllerBasedMovementDirection.Value) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 4:
                Logs.WriteInfo("LeftHandedMode is " + VrSettings.LeftHandedMode.Value.ToString());
                VrSettings.LeftHandedMode.Value = !VrSettings.LeftHandedMode.Value;
                VrSettings.UpdateLeftHandedMode();
                Logs.WriteInfo("LeftHandedMode is now " + VrSettings.LeftHandedMode.Value.ToString());
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((!VrSettings.LeftHandedMode.Value) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 5:
                Logs.WriteInfo("SwapSticks is " + VrSettings.SwapSticks.Value.ToString());
                VrSettings.SwapSticks.Value = !VrSettings.SwapSticks.Value;
                Logs.WriteInfo("SwapSticks is now " + VrSettings.SwapSticks.Value.ToString());
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((!VrSettings.SwapSticks.Value) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 6:
                /*Logs.WriteInfo("WorldScale is " + VrSettings.WorldScale.Value.ToString());
                VrSettings.UpdateWorldScale(isRight);
                Logs.WriteInfo("WorldScale is now " + VrSettings.WorldScale.Value.ToString());
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue(VrSettings.WorldScale.Value.ToString());*/
                Logs.WriteInfo("ShowLaserPointer is " + VrSettings.ShowLaserPointer.Value.ToString());
                VrSettings.ShowLaserPointer.Value = !VrSettings.ShowLaserPointer.Value;
                Logs.WriteInfo("ShowLaserPointer is now " + VrSettings.ShowLaserPointer.Value.ToString());
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((!VrSettings.ShowLaserPointer.Value) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 7:
                Logs.WriteInfo("HeightOffset is now " + VrSettings.HeightOffset.Value.ToString());
                VrSettings.UpdateHeightOffset(isRight);
                Logs.WriteInfo("HeightOffset is now " + VrSettings.HeightOffset.Value.ToString());
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((Mathf.Round(VrSettings.HeightOffset.Value * 10.0f) * 0.1f).ToString());
                break;
            case 8:
                Logs.WriteInfo("VelocityTrigger is now " + VrSettings.VelocityTrigger.Value.ToString());
                VrSettings.UpdateVelocityTrigger(isRight);
                Logs.WriteInfo("VelocityTrigger is now " + VrSettings.VelocityTrigger.Value.ToString());
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((Mathf.Round(VrSettings.VelocityTrigger.Value * 10.0f) * 0.1f).ToString());
                break;
            case 9:
                Logs.WriteInfo("AngularVelocityTrigger is now " + VrSettings.AngularVelocityTrigger.Value.ToString());
                VrSettings.UpdateAngularVelocityTrigger(isRight);
                Logs.WriteInfo("AngularVelocityTrigger is now " + VrSettings.AngularVelocityTrigger.Value.ToString());
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((Mathf.Round(VrSettings.AngularVelocityTrigger.Value * 10.0f) * 0.1f).ToString());
                break;
            case 10:
                __instance.m_OptionControls.HandleFullscreenValueChange();
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((!GameManager.Instance.PlayerSettings.Fullscreen) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
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
            case 11:
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
                    __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue(__instance.m_OptionControls.UpdateQuality(num3));
                    break;
                }
            case 12:
                __instance.m_OptionControls.HandleAAValueChange();
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((!GameManager.Instance.PlayerSettings.AA) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 13:
                __instance.m_OptionControls.HandleVSyncValueChange();
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((!GameManager.Instance.PlayerSettings.VSync) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 14:
                __instance.m_OptionControls.HandleDOFValueChange();
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((!GameManager.Instance.PlayerSettings.DoF) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 15:
                __instance.m_OptionControls.HandleBloomValueChange();
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((!GameManager.Instance.PlayerSettings.Bloom) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
                break;
            case 16:
                __instance.m_OptionControls.HandleAmbientOcclusionValueChange();
                __instance.m_AdvancedMenuOptions[__instance.m_SelectedIndex].UpdateValue((!GameManager.Instance.PlayerSettings.AmbientOcclusion) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
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
          

    
}