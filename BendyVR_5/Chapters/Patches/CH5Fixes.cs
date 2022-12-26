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
public class CH5Fixes : BendyVRPatch
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5OpeningSequenceController), nameof(CH5OpeningSequenceController.InternalActivate))]
	private static bool OpeningInternalActivate(CH5OpeningSequenceController __instance)
	{
		GameManager.Instance.Player.SetCameraSway(active: true);
		GameManager.Instance.Player.SetLock(active: true);
		__instance.m_FreeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		__instance.m_FreeRoamCam.position = __instance.m_S1StartLocation.position;
		//m_FreeRoamCam.eulerAngles = m_S1StartLocation.eulerAngles;
		__instance.ResetSequence();
		float num = 0f;
		__instance.m_Sequence.InsertCallback(num, delegate
		{
			GameManager.Instance.AudioManager.Play(__instance.m_IntroMusic, AudioObjectType.MUSIC);
		});
		num += 4f;
		__instance.m_Sequence.InsertCallback(num, delegate
		{
			GameManager.Instance.ShowChapterTitle("MENU/CH5_LABEL", "MENU/CH5_TITLE");
		});
		num += 10f;
		__instance.m_Sequence.InsertCallback(num, delegate
		{
			GameManager.Instance.HideScreenBlocker(2f);
			GameManager.Instance.UnlockPause();
			__instance.m_OpeningScenes.Scene01Event01 += __instance.HandleScene01Event01;
			__instance.m_OpeningScenes.Scene01OnComplete += __instance.OpeningScene01OnComplete;
			__instance.m_OpeningScenes.ActivateScene01();
		});
		num += 3.5f;
		__instance.m_Sequence.Insert(num, __instance.m_FreeRoamCam.DOMove(__instance.m_S1UpLocation.position, 4f).SetEase(Ease.InOutQuad));
		__instance.m_Sequence.Insert(num, __instance.m_FreeRoamCam.DORotate(__instance.m_S1UpLocation.eulerAngles, 4f).SetEase(Ease.InOutQuad));
		num += 5f;
		__instance.m_Sequence.Insert(num, __instance.m_FreeRoamCam.DOMove(__instance.m_S1DoorwayLocation.position, 10f).SetEase(Ease.InOutQuad));
		__instance.m_Sequence.Insert(num, __instance.m_FreeRoamCam.DORotate(__instance.m_S1DoorwayLocation.eulerAngles, 12f).SetEase(Ease.InOutQuad));
		return false;
	}
	
	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5OpeningSequenceController), nameof(CH5OpeningSequenceController.HandleScene01Event01))]
	private static bool	VRHandleScene01Event01(CH5OpeningSequenceController __instance)
	{
		__instance.m_OpeningScenes.Scene01Event01 -= __instance.HandleScene01Event01;
		float num = 2f;
		//__instance.m_FreeRoamCam.DOMove(__instance.m_S1BackupLocation.position, 3f).SetEase(Ease.OutQuad).SetDelay(num);
		//num += 2.5f;
		//__instance.m_FreeRoamCam.DOLookAt(__instance.m_S1StartLocation.position, 3f).SetEase(Ease.InOutQuad).SetDelay(num);
		//num += 1.5f;
		GameManager.Instance.ShowScreenBlocker(1f, num);
		return false;
	}
	
	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5OpeningSequenceController), nameof(CH5OpeningSequenceController.OpeningScene03OnComplete))]
	private static bool VROpeningScene03OnComplete(CH5OpeningSequenceController __instance)
	{
		__instance.m_OpeningScenes.Scene03OnComplete -= __instance.OpeningScene03OnComplete;
		__instance.m_FreeRoamCam.position = __instance.m_S4KneelLocation.position;
		//m_FreeRoamCam.eulerAngles = m_S4KneelLocation.eulerAngles;
		GameManager.Instance.HideScreenBlocker(1f, 0.1f);
		__instance.m_FreeRoamCam.DOMove(__instance.m_S4DoorwayLocation.position, 2f).SetEase(Ease.InOutQuad).SetDelay(3.5f);
		__instance.m_FreeRoamCam.DORotate(__instance.m_S4DoorwayLocation.eulerAngles, 3f).SetEase(Ease.InOutQuad).SetDelay(3.75f);
		for (int i = 0; i < __instance.m_S4Activate.Length; i++)
		{
			if ((bool)__instance.m_S4Activate[i])
			{
				__instance.m_S4Activate[i].SetActive(value: true);
			}
		}
		__instance.m_OpeningScenes.Scene04OnComplete += __instance.OpeningScene04OnComplete;
		__instance.m_OpeningScenes.ActivateScene04();
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5Tunnels), nameof(CH5Tunnels.HandleHandTriggerOnEnter))]
	private static bool VRHandleHandTriggerOnEnter(CH5Tunnels __instance)
	{
		__instance.m_HandTrigger.OnEnter -= __instance.HandleHandTriggerOnEnter;
		__instance.m_IsClogged = true;
		__instance.m_Boat.Clog();
		Sequence s = DOTween.Sequence();
		s.InsertCallback(1f, delegate
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(__instance.m_HenryClogClip, "DIACH5/DIA_CH5_HENRY_PADDLE_WHEEL"));
		});
		if ((bool)GameManager.Instance.Player.WeaponGameObject)
		{
			s.Insert(4f, GameManager.Instance.Player.WeaponGameObject.transform.DOLocalMoveY(-5f, 0.5f));
			s.Insert(6f, GameManager.Instance.Player.WeaponGameObject.transform.DOLocalMoveY(0f, 0.5f));
		}
		s.InsertCallback(4.5f, delegate
		{
			GameManager.Instance.HideCrosshair();
			__instance.m_MusicObject = GameManager.Instance.AudioManager.Play(__instance.m_TunnelMusic, AudioObjectType.MUSIC, -1);
			Transform transform = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
			transform.SetParent(__instance.m_Boat.Content);
			//transform.DOLookAt(__instance.m_BendyHandGrab.transform.position, 1.5f);
			__instance.m_BendyHandGrab.OnComplete += __instance.HandleBendyHandGrabOnComplete;
			__instance.m_BendyHandGrab.Play();
		});
		s.InsertCallback(6f, delegate
		{
			GameManager.Instance.ShowCrosshair();
			GameManager.Instance.GameCamera.ExitFreeRoamCam();
		});
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5Safehouse), nameof(CH5Safehouse.ForceComplete))]
	private static bool VRSafeHouseForceComplete(CH5Safehouse __instance)
	{
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_ESCAPE_YOUR_PRISON", "OBJECTIVES/CH5_OBJECTIVE_ESCAPE_YOUR_PRISON_TIP"));
		__instance.m_SecretDoor.ForceOpen();
		__instance.m_Toilet.ForceComplete();
		__instance.m_BreakablePlanks.SetActive(value: false);
		GameManager.Instance.Player.WeaponGameObject = __instance.m_GentPipe.gameObject;
		GameManager.Instance.Player.EquipWeapon();
		if ((bool)__instance.m_GentPipe && __instance.m_GentPipe.Interaction != null)
		{
			__instance.m_GentPipe.gameObject.SetActive(value: true);
			__instance.m_GentPipe.Interaction.SetActive(active: false);
		}
		__instance.m_GentPipe.KillInteraction();
		__instance.m_GentPipe.Equip();
		__instance.m_GentPipe.transform.SetParent(GameManager.Instance.Player.WeaponParent);
		//m_GentPipe.transform.localPosition = Vector3.zero;
		//m_GentPipe.transform.localEulerAngles = Vector3.zero;
		GameManager.Instance.Player.EnableSeeingTool(active: true);
		__instance.SendOnComplete();
		return false;
	}
}