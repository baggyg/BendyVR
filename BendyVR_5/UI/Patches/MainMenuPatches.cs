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
		__instance.gameObject.GetComponent<GraphicRaycaster>().enabled = false;		
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
    }

		/*[HarmonyPrefix]
		[HarmonyPatch(typeof(TitleScreenController), nameof(TitleScreenController.Update))]
		private static bool Update(TitleScreenController __instance)
		{
			if (__instance.m_CurrentMenuItem == TitleScreenController.MenuItem.OPTIONS)
			{
				return false;
			}

			__instance.m_NavigationInput.SetActive(value: true);
			__instance.m_NavicationKB.SetActive(value: false);
			__instance.m_WarningControllerNav.SetActive(value: true);
			__instance.m_WarningKBNav.SetActive(value: false);

			if (InputUtil.GetInputY(out __instance.m_ActiveAxisY) && Time.unscaledTime > __instance.m_SelectYTimeNext)
			{
				Logs.WriteInfo("TitleScreen_Update: Move Menu -> " + __instance.m_ActiveAxisY);
				__instance.m_SelectYTimeNext = Time.unscaledTime + __instance.m_SelectYTimeRate;
				if (__instance.m_ActiveAxisY > __instance.m_AxisThreshold)
				{
					__instance.m_SelectedIndex--;
				}
				else if (__instance.m_ActiveAxisY < 0f - __instance.m_AxisThreshold)
				{
					__instance.m_SelectedIndex++;
				}
				if (__instance.m_SelectedIndex >= __instance.m_IndexMax)
				{
					__instance.m_SelectedIndex = 0;
				}
				else if (__instance.m_SelectedIndex < 0)
				{
					__instance.m_SelectedIndex = __instance.m_IndexMax - 1;
				}
				__instance.CheckActiveMenuItemButtons();
			}
			if (__instance.m_CurrentMenuItem == TitleScreenController.MenuItem.CHAPTERS)
			{
				if (InputUtil.GetInputX(out __instance.m_ActiveAxisX) && Time.unscaledTime > __instance.m_SelectXTimeNext)
				{
					__instance.m_SelectXTimeNext = Time.unscaledTime + __instance.m_SelectXTimeRate;
					if (__instance.m_ActiveAxisX > __instance.m_AxisThreshold)
					{
						__instance.m_ChapterArrows.TriggerRight();
					}
					else if (__instance.m_ActiveAxisX < 0f - __instance.m_AxisThreshold)
					{
						__instance.m_ChapterArrows.TriggerLeft();
					}
				}
				if (__instance.m_ActiveAxisX > 0f - __instance.m_AxisThreshold && __instance.m_ActiveAxisX < __instance.m_AxisThreshold)
				{
					__instance.m_SelectXTimeNext = 0f;
				}
			}


			if (__instance.m_ActiveAxisY > 0f - __instance.m_AxisThreshold && __instance.m_ActiveAxisY < __instance.m_AxisThreshold)
			{
				__instance.m_SelectYTimeNext = 0f;
			}
			if (PlayerInput.Jump())
			{
				Logs.WriteInfo("TitleScreen_Update: Player JUMPED");
				if (__instance.m_CurrentMenuItem == TitleScreenController.MenuItem.WARNING)
				{
					__instance.HideWarningMenu();
					__instance.NewGame();
				}
				else
				{
					__instance.CheckSelectedItem();
				}
			}
			else if (PlayerInput.BackOnPressed())
			{
				Logs.WriteInfo("TitleScreen_Update: Back Button");
				if (__instance.m_CurrentMenuItem == TitleScreenController.MenuItem.WARNING)
				{
					__instance.HideWarningMenu();
				}
				else
				{
					__instance.GetBackFromCurrentMenuItem();
				}
			}
			return false;
		}*/


		/*[HarmonyPostfix]
		[HarmonyPatch(typeof(TitleScreenController), nameof(TitleScreenController.InitController))]
		private static void CorrectMainMenuCanvas(TitleScreenController __instance)
		{
			CanvasToWorldSpace.MoveToWorldSpace(__instance,0.75f);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(OptionsMenuController), nameof(OptionsMenuController.InitController))]
		private static void CorrectSettingsCanvas(OptionsMenuController __instance)
		{
			CanvasToWorldSpace.MoveToWorldSpace(__instance, 0.5f);
		}*/

		[HarmonyPostfix]
    [HarmonyPatch(typeof(BaseUIController), nameof(BaseUIController.InitController))]
    private static void CorrectSettingsCanvas(BaseUIController __instance)
    {
        CanvasToWorldSpace.MoveToWorldSpace(__instance, 0.75f);
    }

    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(TitleScreenController), nameof(TitleScreenController.InitController))]
    private static void AddMainMenuBackground(TitleScreenController __instance)
    {
        var background = __instance.transform.Find("Background Layout");
        background.gameObject.SetActive(true);
        var image = background.GetComponentInChildren<RawImage>();
        image.texture = null;
        image.color = new Color(0, 0, 0, 0.75f);
        if (image.transform.localPosition.z == 0) image.transform.localPosition += Vector3.forward * 50;
    }*/
}
