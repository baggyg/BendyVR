using System.Collections.Generic;
using System.IO;
using BendyVR_5.Helpers;
using BendyVR_5.Settings;
using BepInEx;
using UnityEngine;

namespace BendyVR_5.Assets;

public static class VrAssetLoader
{
    private const string assetsDir = "BendyVRAssets/AssetBundles/";
    public static readonly Dictionary<string, Shader> LivShaders = new();
    
    public static Shader TMProShader { get; private set; }
    public static Shader FadeShader { get; private set; }
    public static GameObject VrSettingsMenuPrefab { get; private set; }
    public static GameObject LeftHandPrefab { get; private set; }
    public static GameObject RightHandPrefab { get; private set; }
    public static GameObject TeleportTargetPrefab { get; private set; }
    public static Sprite MenuBG { get; private set; }
    

    public static void LoadAssets()
    {
        Logs.WriteInfo("Loading VRHands Assets");
        var vrBundle = LoadBundle("vrhands");
        if (VrSettings.ShowLegacySteamVRHands.Value == true)
        {
            LeftHandPrefab = vrBundle.LoadAsset<GameObject>("vr_glove_left_bkp");
            RightHandPrefab = vrBundle.LoadAsset<GameObject>("vr_glove_right_bkp");
        }
        else
        {
            LeftHandPrefab = vrBundle.LoadAsset<GameObject>("vr_glove_left");
            RightHandPrefab = vrBundle.LoadAsset<GameObject>("vr_glove_right");
        }
        
        MenuBG = vrBundle.LoadAsset<Sprite>("menu_bg");
    }

    private static AssetBundle LoadBundle(string assetName)
    {
        var bundle = AssetBundle.LoadFromFile(Path.Combine(Paths.PluginPath, Path.Combine(assetsDir, assetName)));
        if (bundle != null) return bundle;
        Logs.WriteError($"Failed to load AssetBundle {assetName}");
        return null;
    }
}