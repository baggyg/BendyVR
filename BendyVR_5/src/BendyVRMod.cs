using BendyVR_5.Assets;
using BendyVR_5.Settings;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Valve.VR;

namespace BendyVR_5.src;

[BepInPlugin("org.baggyg.vrplugins.bendyvr", "BendyVR", "1.1.0")]
[BepInProcess("Bendy and the Ink Machine.exe")]

// A soft dependency. Loading won't be skipped if it's missing.
//[BepInDependency("com.bepinex.plugin.somedependency", BepInDependency.DependencyFlags.SoftDependency)]
// A hard dependency. Loading will be skipped (and an error shown) if the dependency is missing.
//[BepInDependency("com.bepinex.plugin.importantdependency", BepInDependency.DependencyFlags.HardDependency)]
// If flags are not specified, the dependency is **hard** by default
//[BepInDependency("com.bepinex.plugin.anotherimportantone")]
// Depends on com.bepinex.plugin.versioned version 1.2.x
//[BepInDependency("com.bepinex.plugin.versioned", "~1.2")]

// If some.undesirable.plugin is installed, this plugin is skipped
//[BepInIncompatibility("some.undesirable.plugin")]
public class BendyVRPlugin : BaseUnityPlugin
{
    internal static ManualLogSource logBendyVR;
    private ConfigEntry<bool> configEnableMod;

    private void Awake()
    {
        // Plugin startup logic

        //LOGGING
        logBendyVR = new ManualLogSource("BendyVRLog"); // The source name is shown in BepInEx log
        BepInEx.Logging.Logger.Sources.Add(logBendyVR);
        logBendyVR.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

        //logBendyVR.LogWarning("This is a warning - Auto Installed");
        //logBendyVR.LogError("This is an error - No Time to Do it!");

        //CONFIG SETTINGS
        //HARMONY PATCHING OF METHODS
        VrSettings.SetUp(Config);
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        VrAssetLoader.LoadAssets();
        InitSteamVR();        

        // Remove the source to free resources
        //BepInEx.Logging.Logger.Sources.Remove(logBendyVR);
    }

    private static void InitSteamVR()
    {
        logBendyVR.LogInfo("Initalising SteamVR");
        SteamVR_Actions.PreInitialize();
        SteamVR.Initialize(true); //Force Unity VR Mode via true
        SteamVR_Settings.instance.pauseGameWhenDashboardVisible = true;
        logBendyVR.LogInfo("SteamVR Initialised");
    }

    /*private void SetupUI()
    {
        var canvases = GameObject.FindObjectsOfType<Canvas>().Where(canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay);
        canvases.Do(canvas =>
        {
            canvas.worldCamera = Camera.main;
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.SetParent(Camera.main.transform, false);
            canvas.transform.localPosition = Vector3.forward * 0.5f;
            canvas.transform.localScale = Vector3.one * 0.0004f;

        });
    }*/
}
