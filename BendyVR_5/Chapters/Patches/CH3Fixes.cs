using BendyVR_5.Helpers;
using BendyVR_5.Stage;
using BendyVR_5.UI;
using DG.Tweening;
using HarmonyLib;
using System;
using System.Collections;
using TMG.Controls;
using TMG.Core;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.XR;

namespace BendyVR_5.Chapters.Patches;

[HarmonyPatch]
public class CH3Fixes : BendyVRPatch
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH3OpeningSequenceController), nameof(CH3OpeningSequenceController.InternalActivate))]
	private static bool InternalActivate(CH3OpeningSequenceController __instance)
	{
		//__instance.m_GameCamTransform = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		//__instance.m_GameCamTransform.position = __instance.m_BedPosition.position;
		//__instance.m_GameCamTransform.eulerAngles = __instance.m_BedPosition.eulerAngles;
		GameManager.Instance.ShowChapterTitle("MENU/CH3_LABEL", "MENU/CH3_TITLE");
		GameManager.Instance.HideScreenBlocker(1f);
		//__instance.DOSequence();
		//GameManager.Instance.GameCamera.ExitFreeRoamCam();
		GameManager.Instance.Player.SetLock(active: false);
		GameManager.Instance.UnlockPause();
		//GameManager.Instance.Player.SetCameraSway(active: true);
		//GameManager.Instance.ShowCrosshair();
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_01", string.Empty, 4f));
		__instance.SendOnComplete();
		return false;
	}	
	

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH3WeaponStationController), nameof(CH3WeaponStationController.HandleDropboxOnInteracted))]
	private static bool FixWeaponSwitch(CH3WeaponStationController __instance)
	{
		__instance.m_Dropbox.OnInteracted -= __instance.HandleDropboxOnInteracted;
		__instance.m_Dropbox.SetActive(active: false);
		GameManager.Instance.AudioManager.PlayAtPosition(__instance.m_DropboxClip, __instance.m_Dropbox.transform.position);
		PlayerController player = GameManager.Instance.Player;
		if ((bool)player.InactiveWeapon)
		{
			UnityEngine.Object.Destroy(player.WeaponGameObject);
			player.WeaponGameObject = player.InactiveWeapon;
			player.WeaponGameObject.SetActive(value: true);
			player.WeaponGameObject.transform.localPosition = Vector3.zero;
			player.WeaponGameObject.transform.localEulerAngles = Vector3.zero;
			player.InactiveWeapon = null;
			VrCore.instance.GetVRPlayerController().SetupMeleeWeapon(player.WeaponGameObject.name);
		}
		return false;
	}
	
	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH3ProjectionistTaskController), nameof(CH3ProjectionistTaskController.HandleGunOnInteracted))]
	private static bool FixElevator(CH3ProjectionistTaskController __instance, object sender, EventArgs e)
	{
		(sender as Interactable).OnInteracted -= __instance.HandleGunOnInteracted;
		if (!__instance.m_CanHaveTommyGun)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(__instance.m_AliceTommyGunFakeClip, "DIACH3/DIA_CH3_ALICE_22"));
		}
		else
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.BLAZING_METAL);
		}
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_PROJECTIONIST", "OBJECTIVES/CH3_OBJECTIVE_TASK_PROJECTIONIST_TIP", 4f);
		objectiveDataVO.AddItemCounter(__instance.m_ObjectiveSprite, 0);
		GameManager.Instance.ShowObjective(objectiveDataVO);
		__instance.m_WeaponStationController.Unblock();
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Status.IsStarted = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = __instance.m_LiftController.CurrentFloor.ID;
		//This was missing!
		__instance.m_LiftController.EnableLift();
		GameManager.Instance.GameDataManager.Save();
		__instance.EnableTask();
		return false;
	}

	/*[HarmonyPrefix]
	[HarmonyPatch(typeof(CH3ProjectionistTaskController), nameof(CH3ProjectionistTaskController.CheckTommyGun))]
	private static bool TommyGunCheat(ref bool __result)
	{
		__result = true; //Force Tommy Gun On
		return false;
	}*/

}