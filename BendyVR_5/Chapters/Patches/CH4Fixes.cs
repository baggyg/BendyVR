using BendyVR_5.Helpers;
using BendyVR_5.Stage;
using BendyVR_5.UI;
using DG.Tweening;
using HarmonyLib;
using S13Audio;
using System;
using System.Collections;
using TMG.Controls;
using TMG.Core;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.XR;

namespace BendyVR_5.Chapters.Patches;

[HarmonyPatch]
public class CH4Fixes : BendyVRPatch
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(BruteBorisAi), nameof(BruteBorisAi.ApplyShake))]
	private static bool DontShakeCamera()
	{
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH4BallroomController), nameof(CH4BallroomController.HandleBorisOnDeath))]
	private static bool VRHandleBorisOnDeath(CH4BallroomController __instance)
	{
		__instance.m_Boris.OnDeath -= __instance.HandleBorisOnDeath;
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.HasPlunger)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.UNLIKELY_VICTORY);
		}
		if ((bool)__instance.m_MusicLoop)
		{
			__instance.m_MusicLoop.Stop();
			__instance.m_MusicLoop = null;
		}
		if ((bool)GameManager.Instance.Player.WeaponGameObject)
		{
			GameManager.Instance.Player.WeaponGameObject.SetActive(value: false);
		}
		__instance.ClearThickInk();
		GameManager.Instance.AudioManager.Play(__instance.m_MusicDeathOfAFriendFinisher, AudioObjectType.MUSIC);
		for (int i = 0; i < __instance.m_AliceFinaleClips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(__instance.m_AliceFinaleClips[i], SubtitleConstants.DIA_CH4_ALICE_FINALE[i], isTrimmed: true));
		}
		GameManager.Instance.LockPause();
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.SetCameraSway(active: true);
		Transform target = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		Vector3 position = __instance.m_Boris.transform.position;
		position -= __instance.m_Boris.transform.forward * 5f;
		Sequence sequence = DOTween.Sequence();
		float num = 0.5f;
		//Don't move
		//sequence.Insert(num, target.DOMove(m_PlayerEndLocation.position, 2f).SetEase(Ease.InOutQuad));
		//sequence.Insert(num, target.DORotate(m_PlayerEndLocation.eulerAngles, 2f).SetEase(Ease.InOutQuad));
		num += 2f;
		//sequence.Insert(num, target.DOLookAt(position, 2f).SetEase(Ease.InOutQuad));
		sequence.OnComplete(__instance.SendOnComplete);
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH4ClosingSequenceController), nameof(CH4ClosingSequenceController.Activate))]
	private static bool VRActivate(CH4ClosingSequenceController __instance)
	{
		GameManager.Instance.AudioManager.Play(__instance.m_BorisMusicClip, AudioObjectType.MUSIC).OnComplete += __instance.HandleBorisMusicOnComplete;
		__instance.m_FreeRoamCamera = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		GameManager.Instance.LockPause();
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.Player.SetCameraSway(active: true);
		//__instance.m_FreeRoamCamera.DOMove(__instance.m_BorisLookLocation.position, 12f).SetEase(Ease.InOutQuad);
		//__instance.m_FreeRoamCamera.DORotate(__instance.m_BorisLookLocation.eulerAngles, 12f).SetEase(Ease.InOutQuad);
		Sequence s = DOTween.Sequence();
		float num = 2f;
		float num2 = 11f;
		s.InsertCallback(num, delegate
		{
			S13AudioManager.Instance.InvokeEvent("evt_boris_death_melt");
		});
		s.Insert(num, __instance.m_BorisBody.materials[0].DOFloat(1f, "_MeltPercentage", num2).SetEase(Ease.Linear));
		s.Insert(num, __instance.m_BorisBody.materials[1].DOFloat(1f, "_MeltPercentage", num2).SetEase(Ease.Linear));
		num += num2;
		s.Insert(num, __instance.m_BorisBody.materials[0].DOFloat(1f, "_Dissolve", 4f).SetEase(Ease.Linear));
		s.Insert(num, __instance.m_BorisBody.materials[1].DOFloat(1f, "_Dissolve", 4f).SetEase(Ease.Linear));
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH4ClosingSequenceController), nameof(CH4ClosingSequenceController.ActualActivate))]
	private static bool VRActualActivate(CH4ClosingSequenceController __instance)
	{
		__instance.m_Alice.SetActive(value: true);
		__instance.m_Allison.SetActive(value: true);
		//__instance.m_FreeRoamCamera.DOMove(__instance.m_PlayerCamLocation.position, 1f).SetEase(Ease.Linear);
		//__instance.m_FreeRoamCamera.DORotate(__instance.m_PlayerCamLocation.eulerAngles, 1f).SetEase(Ease.Linear);
		//GameManager.Instance.Player.SetFOVValue(45f, 2.5f);
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH4ClosingSequenceController), nameof(CH4ClosingSequenceController.AliceFall))]
	public static bool AliceFall(CH4ClosingSequenceController __instance)
	{
		__instance.m_Tom.transform.position = __instance.m_BorisLocation.position;
		__instance.m_Tom.transform.eulerAngles = __instance.m_BorisLocation.eulerAngles;
		__instance.m_Tom.SetActive(value: true);
		//m_FreeRoamCamera.DOLookAt(m_FinalLookLocation.position, 4f).SetDelay(1f).SetEase(Ease.InOutQuad);
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH4ClosingSequenceController), nameof(CH4ClosingSequenceController.LookAtAlice))]
	public static bool LookAtAlice(CH4ClosingSequenceController __instance)
	{
		//__instance.m_FreeRoamCamera.DOLookAt(__instance.m_LookAtLocation.position, 1f).SetEase(Ease.InOutQuad);
		__instance.m_AudioSwitch.Play("fall");
		return false;
	}
}