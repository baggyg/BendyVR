using System.Globalization;
using BendyVR_5.Helpers;
using BendyVR_5.src;
using BendyVR_5.VrInput.ActionInputs;
using HarmonyLib;
using UnityEngine;
using Valve.VR;

namespace BendyVR_5.VrInput.Patches;

[HarmonyPatch]
public class InputPromptsPatches : BendyVRPatch
{
    private static readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TutorialPopupController), nameof(TutorialPopupController.InitController))]
	private static void ReplacePromptIconsWithVrButtonText(TutorialPopupController __instance)	
	{
		Logs.WriteInfo("Repositioning Tutorial");
		__instance.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
		__instance.transform.localPosition = new Vector3(0f, 1000f, 0f);
		__instance.m_ButtonImage.SetActive(value: false);
		__instance.m_KeyboardImage.SetActive(value: true);
		__instance.m_MouseImageLeft.SetActive(value: false);
		__instance.m_MouseImageRight.SetActive(value: false);
		__instance.m_MouseImageMiddle.SetActive(value: false);
		IActionInput inputAction = ActionInputs.ActionInputDefinitions.Jump;
		if (__instance.m_DataVO.ActionRaw == "Tutorial/TUTORIAL_JUMP")
		{
			inputAction = ActionInputs.ActionInputDefinitions.Jump;
		}
		else if (__instance.m_DataVO.ActionRaw == "Tutorial/TUTORIAL_RUN")
		{
			inputAction = ActionInputs.ActionInputDefinitions.Run;
		}
		else if (__instance.m_DataVO.ActionRaw == "Tutorial/TUTORIAL_SEEING_TOOL")
		{
			inputAction = ActionInputs.ActionInputDefinitions.SeeingTool;
		}
		var source = inputAction.ActiveSource;
		var hand = "";
		if (source == SteamVR_Input_Sources.RightHand) hand = "right ";
		if (source == SteamVR_Input_Sources.LeftHand) hand = "left ";

		var name = inputAction.Action.GetRenderModelComponentName(inputAction.ActiveSource);
		name = name.Replace("button_", "");
		name = name.Replace("thumb", "");
		__instance.m_KeyboardLbl.text = textInfo.ToTitleCase($"{hand}{name}");
	}

	/*[HarmonyPrefix]
    [HarmonyPatch(typeof(vgButtonIconMap), nameof(vgButtonIconMap.GetIconName))]
    private static bool ReplacePromptIconsWithVrButtonText(ref string __result, string id)
    {
        var inputAction = StageInstance.GetInputAction(id);
        if (inputAction?.Action == null || !inputAction.Action.active)
        {
            __result = "n/a";
            return false;
        }

        var source = inputAction.ActiveSource;
        var hand = "";
        if (source == SteamVR_Input_Sources.RightHand) hand = "right ";
        if (source == SteamVR_Input_Sources.LeftHand) hand = "left ";

        var name = inputAction.Action.GetRenderModelComponentName(inputAction.ActiveSource);
        name = name.Replace("button_", "");
        name = name.Replace("thumb", "");
        __result = textInfo.ToTitleCase($"{hand}{name}");

        return false;
    }*/

	/*[HarmonyPrefix]
    [HarmonyPatch(typeof(vgButtonIconMap), nameof(vgButtonIconMap.HasIcon))]
    private static bool CheckHasIconFromVrInputs(ref bool __result, string id)
    {
        __result = true;
        return false;
    }*/
}