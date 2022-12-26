using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using BendyVR_5.Helpers;
using System;
using TMG.UI;
using InControl;
using TMG.Controls;
using TMG.UI.Controls;
using BendyVR_5.Assets;
using BendyVR_5.Stage;

namespace BendyVR_5.UI.Patches;

[HarmonyPatch]
public class MainMenuPatches : BendyVRPatch
{
    // These are all overlays that only made sense in pancake mode.
    private static readonly string[] objectsToDisable =
    {
        "Particles"        
    };

	//This removes the horrible gaze thing from the menus
	//TODO possibly change this to laser later - not really necessary 
	[HarmonyPostfix]
	/*[HarmonyPatch(typeof(TitleScreenController), nameof(TitleScreenController.InitController))]
	[HarmonyPatch(typeof(OptionsMenuController), nameof(OptionsMenuController.InitController))]*/
	[HarmonyPatch(typeof(BaseUIController), nameof(BaseUIController.InitController))]
	private static void RemoveCustomInput(BaseUIController __instance)
	{
        GraphicRaycaster gr = __instance.gameObject.GetComponent<GraphicRaycaster>();
        if (gr != null)
            gr.enabled = false;
    }

    [HarmonyPostfix]
	[HarmonyPatch(typeof(OptionsMenuController), nameof(OptionsMenuController.InitController))]
	private static void RemoveCustomInput(TitleScreenController __instance)
	{
		__instance.gameObject.GetComponent<GraphicRaycaster>().enabled = false;
	}

	[HarmonyPostfix]
    [HarmonyPatch(typeof(TitleScreenController), nameof(TitleScreenController.InitController))]
    private static void DisableUnnecessaryMainMenuObjects(TitleScreenController __instance)
    {
        foreach (Transform child in __instance.transform)
            if (objectsToDisable.Contains(child.name))
                child.gameObject.SetActive(false);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TitleScreenController), nameof(TitleScreenController.InitController))]
    private static void DebugControllers(TitleScreenController __instance)
    {
        Logs.WriteInfo("Game Manager Has Controller: " + GameManager.Instance.HasController.ToString());
        Logs.WriteInfo("InputManager Device Count: " + InputManager.Devices.Count.ToString());


        //Lets try to change the background
        var uiparent = GameObject.Find("[UI MANAGER]");
        if (uiparent)
        {
            Logs.WriteWarning("Found: " + uiparent.name);
            var background = GameObject.Find("Visuals/BG/BG");
            if (background)
            {
                Logs.WriteWarning("Found: " + background.name);
                GameObject.Find("Visuals/BG/LightCookie").SetActive(false);
                Logs.WriteWarning("Turned off Light Cookie");
                GameObject.Find("Visuals/BG/Frame").SetActive(false);
                Logs.WriteWarning("Turned off Frame");
                //background.gameObject.SetActive(true);
                var image = background.GetComponent<Image>();
                if (image)
                {
                    Logs.WriteWarning("Found Image");
                    image.sprite = VrAssetLoader.MenuBG;
                }
            }
        }

        /*image.texture = null;
        image.color = new Color(0, 0, 0, 0.75f);
        if (image.transform.localPosition.z == 0) image.transform.localPosition += Vector3.forward * 50;*/
    }		

	[HarmonyPostfix]
    [HarmonyPatch(typeof(BaseUIController), nameof(BaseUIController.InitController))]
    private static void CorrectSettingsCanvas(BaseUIController __instance)
    {
        if (__instance is ScreenBlockerController)
        {
            CanvasToWorldSpace.MoveToCameraSpace(__instance,1f);        
        }
        else if (__instance is CH1ConclusionModalController)
        {
            CanvasToWorldSpace.MoveToCameraSpace(__instance, 2f);
        }
        else if (__instance is HurtBordersController)
        {
            CanvasToWorldSpace.MoveToCameraSpace(__instance,4f);
        }
        else
        {
            CanvasToWorldSpace.MoveToWorldSpace(__instance, 0.75f);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TitleScreenController), nameof(TitleScreenController.InitController))]
    private static void MoveStoreButton(TitleScreenController __instance)
    {
        __instance.m_StoreBtn.transform.localPosition = new Vector3(-700f, 405f,0f);
    }
}
