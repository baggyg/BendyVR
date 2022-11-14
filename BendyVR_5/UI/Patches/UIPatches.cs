using System;
using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using BendyVR_5.Assets;
using BendyVR_5.Helpers;
using UnityEngine;
using UnityEngine.UI;
using TMG.UI;

namespace BendyVR_5.UI.Patches;

[HarmonyPatch]
public class UIPatches : BendyVRPatch
{
    private static readonly Dictionary<string, Material> materialMap = new();
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BaseUIController), nameof(BaseUIController.InitController))]
    private static void UniformResizeHUD(BaseUIController __instance)
    {
        //__instance.gameObject.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        __instance.gameObject.GetComponent<CanvasScaler>().enabled = false;
        __instance.rectTransform.sizeDelta = new Vector2(2408, 2428);
    }

    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(CanvasScaler), nameof(CanvasScaler.Start))]
    private static void SetConstantSize(CanvasScaler __instance)
    {
        __instance.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
    }*/

    //On higher World Scales the projections weren't showing. God knows why but this fixes it. 
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Decal), nameof(Decal.UpdateProjectionClipping))]    
    private static bool IncreaseDecalProjections(Decal __instance)
    {
        float value = Mathf.Cos((float)Math.PI / 180f * 180f);
        __instance.materialProperties.SetFloat(__instance._NormalCutoff, value);
        return false;
    }

    //Reposition the Audio Logs to be readable
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AudioLogModalController), nameof(AudioLogModalController.InitController))]
    private static void RepositionAudioLogs(AudioLogModalController __instance)
    {
        Logs.WriteInfo("Repositioning Audio Logs");
        __instance.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        __instance.transform.localPosition = new Vector3(500f, -800f, 0f);
    }

    //Reposition the Objectives to be readable
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ObjectivesController), nameof(ObjectivesController.InitController))]
    private static void RepositionObjectives(ObjectivesController __instance)
    {
        Logs.WriteInfo("Repositioning Objectives");
        __instance.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        __instance.transform.localPosition = new Vector3(1120f, 900f, 0f);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HurtBordersController), nameof(HurtBordersController.InitController))]
    private static void RepositionHurt(HurtBordersController __instance)
    {
        Logs.WriteInfo("Repositioning Hurt");
        __instance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainSubtitlesController), nameof(MainSubtitlesController.InitController))]
    private static void RepositionSubtitles(MainSubtitlesController __instance)
    {
        Logs.WriteInfo("Repositioning Subtitles");
        __instance.transform.localPosition = new Vector3(__instance.transform.localPosition.x, 300f, __instance.transform.localPosition.z);
    }

    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.ShowAbilityIcon))]
    private static bool PreventShowingAbilityIcon()
    {
        return false;
    }*/

    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.InitializeAbilityIcon))]
    private static bool DestroyAbilityIcon(vgHudManager __instance)
    {
        Object.Destroy(__instance.abilityIcon);
        return false;
    }*/

    // For some reason, the default text shader draws on top of everything.
    // I'm importing the TMPro shader from a more recent version and replacing it in the font materials.
    // This way, I can decide which ones I actually want to draw on top.
    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.Awake))]
    [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.OnEnable))]
    private static void PreventTextFromDrawingOnTop(TextMeshProUGUI __instance)
    {
        try
        {
            var isInteractive = __instance.canvas && __instance.canvas.GetComponent<GraphicRaycaster>();
            var key = $"{__instance.font.name}{(isInteractive ? "interactive" : "non-interactive")}";

            materialMap.TryGetValue(key, out var material);

            if (material == null)
            {
                material = new Material(__instance.font.material);
                if (__instance.canvas && __instance.canvas.GetComponent<GraphicRaycaster>())
                    material.shader = VrAssetLoader.TMProShader;

                materialMap[key] = material;
            }

            //__instance.SetFontMaterial(material);
            //__instance.SetSharedFontMaterial(material);
            __instance.SetFontBaseMaterial(material);

            // Problem: setting fontSharedMaterial is needed to prevent errors and the empty settings dropdowns,
            // but it also makes the dialog choices stop rendering on top.
            // __instance.fontSharedMaterial = material;
        }
        catch (Exception exception)
        {
            Logs.WriteWarning($"Error in TMPro Patch ({__instance.name}): {exception}");
        }
    }*/

    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.Awake))]
    private static void HideHudElements(vgHudManager __instance)
    {
        __instance.readObjectButtonGroup.transform.parent.Find("ExamineItem").gameObject.SetActive(false);
        __instance.readObjectButtonGroup.SetActive(false);

        // Dummy object is just so the hud manager still has a valid reference after we destroy the object.
        __instance.readObjectButtonGroup = new GameObject("Dummy");
        __instance.readObjectButtonGroup.transform.SetParent(__instance.transform, false);

        var safeZoner = __instance.transform.Find("uGUI Root/HUD/SafeZoner");
        var reticuleCanvasGroup = safeZoner.Find("ReticuleGroup/ReticuleParent/ReticuleCanvasGroup");
        var reticule = reticuleCanvasGroup.Find("Reticule");
        reticule.GetComponent<Image>().enabled = false;
        reticuleCanvasGroup.Find("ReticuleDisabled").GetComponent<Image>().enabled = false;
        reticule.Find("ReticuleLarge").GetComponent<Image>().enabled = false;

        var bottomLeftObjects = safeZoner.Find("BottomLeftObjects");
        bottomLeftObjects.Find("CompassOnScreenTooltip").gameObject.SetActive(false);
        bottomLeftObjects.Find("MapOnScreenTooltip").gameObject.SetActive(false);
    }*/
}