using System.Collections.Generic;
using System.IO;
using BendyVR_5.Helpers;
using UnityEngine;

namespace BendyVR_5.Assets;

public static class VrAssetLoader
{
    private const string assetsDir = "/BepInEx/plugins/baggyg-bendyvr/BendyVRAssets/AssetBundles/";
    public static readonly Dictionary<string, Shader> LivShaders = new();
    
    public static Shader TMProShader { get; private set; }
    public static Shader FadeShader { get; private set; }
    public static GameObject VrSettingsMenuPrefab { get; private set; }
    public static GameObject LeftHandPrefab { get; private set; }
    public static GameObject RightHandPrefab { get; private set; }
    public static GameObject TeleportTargetPrefab { get; private set; }
    

    public static void LoadAssets()
    {
        /*var bodyBundle = LoadBundle("body");
        LeftHandPrefab = bodyBundle.LoadAsset<GameObject>("left-hand");
        RightHandPrefab = bodyBundle.LoadAsset<GameObject>("right-hand");        */

        Logs.WriteInfo("Loading VRHands Assets");
        var vrBundle = LoadBundle("vrhands");
        LeftHandPrefab = vrBundle.LoadAsset<GameObject>("vr_glove_left");
        RightHandPrefab = vrBundle.LoadAsset<GameObject>("vr_glove_right");

        var uiBundle = LoadBundle("ui");
        TeleportTargetPrefab = uiBundle.LoadAsset<GameObject>("teleport-target");
        TMProShader = uiBundle.LoadAsset<Shader>("TMP_SDF-Mobile");
        FadeShader = uiBundle.LoadAsset<Shader>("SteamVR_Fade");
    }

    private static AssetBundle LoadBundle(string assetName)
    {
        var bundle = AssetBundle.LoadFromFile($"{Directory.GetCurrentDirectory()}{assetsDir}{assetName}");
        if (bundle != null) return bundle;
        Logs.WriteError($"Failed to load AssetBundle {assetName}");
        return null;
    }
}