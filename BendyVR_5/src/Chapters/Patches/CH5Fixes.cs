using BendyVR_5.Helpers;
using BendyVR_5.src;
using BendyVR_5.Stage;
using BendyVR_5.UI;
using DG.Tweening;
using HarmonyLib;
using S13Audio;
using System;
using System.Collections;
using System.Reflection;
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
	private static bool VRHandleScene01Event01(CH5OpeningSequenceController __instance)
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
	[HarmonyPatch(typeof(CH5OpeningSequenceController), nameof(CH5OpeningSequenceController.HandleScene05Event02))]
	private static bool VRHandleScene05Event02(CH5OpeningSequenceController __instance)
	{
		__instance.m_OpeningScenes.Scene05Event02 -= __instance.HandleScene05Event02;
		//GameManager.Instance.GameCamera.ExitFreeRoamCam();
		//GameManager.Instance.Player.transform.SetParent(__instance.m_S5EndLocation);
		//GameManager.Instance.Player.LockRotation(25f, 20f);		
		//GameManager.Instance.Player.UseSeeingTool(active: true);
		//GameManager.Instance.Player.SetLock(active: false);
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5OpeningSequenceController), nameof(CH5OpeningSequenceController.HandleScene05Event04))]
	private static bool VRHandleScene05Event04(CH5OpeningSequenceController __instance)
	{
		Logs.WriteWarning("Event 4");
		__instance.m_OpeningScenes.Scene05Event04 -= __instance.HandleScene05Event04;
		//GameManager.Instance.Player.UnlockRotation();
		//GameManager.Instance.Player.SetLock(active: true);
		//GameManager.Instance.Player.UseSeeingTool(active: false);
		//__instance.m_FreeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		//__instance.m_FreeRoamCam.SetParent(__instance.m_S5EndLocation);
		//GameManager.Instance.Player.transform.SetParent(null);
		//Vector3 position = GameManager.Instance.Player.transform.position;
		//GameManager.Instance.Player.GoToAndLookAt(__instance.m_S5EndLocation);
		//GameManager.Instance.Player.transform.position = position;
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5OpeningSequenceController), nameof(CH5OpeningSequenceController.OpeningScene06OnComplete))]
	private static bool VROpeningScene06OnComplete(CH5OpeningSequenceController __instance)
	{
		__instance.m_OpeningScenes.Scene06OnComplete -= __instance.OpeningScene06OnComplete;
		__instance.m_FreeRoamCam.position = __instance.m_S7StartLocation.position;
		//__instance.m_FreeRoamCam.eulerAngles = __instance.m_S7StartLocation.eulerAngles;
		Sequence s = DOTween.Sequence();
		float num = 2f;
		s.InsertCallback(num, delegate
		{
			__instance.m_FreeRoamCam.position = GameManager.Instance.Player.HeadContainer.position;
			GameManager.Instance.HideScreenBlocker(0.5f, 0.1f);
			GameManager.Instance.AudioManager.Play(__instance.m_SceneSevenClip);
			__instance.m_OpeningScenes.Scene07OnComplete += __instance.OpeningScene07OnComplete;
			__instance.m_OpeningScenes.ActivateScene07();
		});
		//s.Insert(num, __instance.m_FreeRoamCam.DOMove(__instance.m_S7StandLocation.position, 1f).SetEase(Ease.InOutQuad));
		//s.Insert(num, __instance.m_FreeRoamCam.DORotate(__instance.m_S7StandLocation.eulerAngles, 1.1f).SetEase(Ease.InOutQuad));
		//num += 1.2f;
		//s.Insert(num, __instance.m_FreeRoamCam.DOMove(GameManager.Instance.Player.HeadContainer.position, 1.5f).SetEase(Ease.InOutQuad));
		//s.Insert(num, __instance.m_FreeRoamCam.DORotate(__instance.m_S5EndLocation.parent.eulerAngles, 1.6f).SetEase(Ease.InOutQuad));
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5DemonWorthy), nameof(CH5DemonWorthy.HandleLeverOnInteracted))]
	private static bool VRHandleLeverOnInteracted(CH5DemonWorthy __instance)
	{
		if (!__instance.m_IsClogged)
		{
			__instance.m_Lever.OnInteracted -= __instance.HandleLeverOnInteracted;
			__instance.m_Lever.AllowShimmer(_active: false);
			__instance.m_Lever.ForceRemoveEffects();
			__instance.m_InteriorCollider.SetActive(value: true);
			__instance.m_IsActive = true;
			__instance.m_Lever.transform.DOKill();
			__instance.m_Lever.transform.DOLocalMove(__instance.m_LeverOnPosition, 0.25f).SetEase(Ease.InOutQuad);
			__instance.m_LeverLight.SetActive(value: true);

			GameManager.Instance.Player.transform.SetParent(__instance.m_Content);
			VrCore.instance.GetVRPlayerController().ResetRoomDampener();
			GameManager.Instance.Player.SetLockedMovement(active: true);
			GameManager.Instance.Player.LockRotation(40f, 35f);
			__instance.ThrottleSequence();
			__instance.m_BoatAudioControl.ThrottleOn();

			//this.OnThrottled.Send(this);
			var handler = typeof(CH5DemonWorthy).GetField("OnThrottled", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) as Delegate;
			if (handler != null)
			{
				var subscribers = handler.GetInvocationList();
				foreach (var subscriber in subscribers)
				{
					subscriber.DynamicInvoke(__instance, EventArgs.Empty);
				}
			}
		}
		return false;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(CH5DemonWorthy), nameof(CH5DemonWorthy.ForceUnclog))]
	[HarmonyPatch(typeof(CH5DemonWorthy), nameof(CH5DemonWorthy.HandleCloggerOnHit))]
	[HarmonyPatch(typeof(CH5DemonWorthy), nameof(CH5DemonWorthy.Clog))]
	private static void Unclog(CH5DemonWorthy __instance)
	{
		VrCore.instance.GetVRPlayerController().mCloggedBoat = __instance.m_IsClogged;
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
		/*if ((bool)GameManager.Instance.Player.WeaponGameObject)
		{
			s.Insert(4f, GameManager.Instance.Player.WeaponGameObject.transform.DOLocalMoveY(-5f, 0.5f));
			s.Insert(6f, GameManager.Instance.Player.WeaponGameObject.transform.DOLocalMoveY(0f, 0.5f));
		}*/
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

	[HarmonyPrefix]
	[HarmonyPatch(typeof(SammyLawrence_Ai), nameof(SammyLawrence_Ai.TriggerDeath))]
	private static bool TriggerDeath(SammyLawrence_Ai __instance)
	{
		__instance.m_AnimationController.SetTrigger("Dead");
		__instance.m_AnimationController.transform.localScale = Vector3.one;
		__instance.m_CharacterController.enabled = false;
		CapsuleCollider component = __instance.GetComponent<CapsuleCollider>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		Transform transform = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		transform.position = __instance.m_CameraParent.position;
		//transform.position = new Vector3(-222.3222f, -4.6095f, -124.5158f);
		transform.DOMove(new Vector3(-222.2934f, -9.664f, -125.0302f), 6f).SetEase(Ease.Linear);
		//transform.SetParent(__instance.m_CameraParent);
		//transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.Linear);
		//transform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.Linear);
		return false;
	}		

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5ThroneRoom), nameof(CH5ThroneRoom.HandleAudioLogOnInteracted))]
	private static bool VRHandleAudioLogOnInteracted(CH5ThroneRoom __instance)
	{
		__instance.m_AudioLog.OnInteracted -= __instance.HandleAudioLogOnInteracted;
		__instance.m_AudioLog.SetActive(active: false);
		__instance.m_AudioLog.ForceRemoveEffects();
		__instance.m_EntranceDoor.ForceClose();
		__instance.m_SideDoor.ForceOpen();
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.AudioManager.Play(__instance.m_MusicClip, AudioObjectType.MUSIC);
		GameManager.Instance.AudioManager.Play(__instance.m_OnClip);
		AudioObject runningClip = GameManager.Instance.AudioManager.Play(__instance.m_RunningClip, AudioObjectType.DIALOGUE, -1);
		float num = 0f;
		int num2 = __instance.m_JoeyClips.Length - 1;
		for (int i = 0; i < __instance.m_JoeyClips.Length; i++)
		{
			AudioClip audioClip = __instance.m_JoeyClips[i];
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(audioClip, SubtitleConstants.DIA_CH5_JOEY_AUDIO_LOG[i], isTrimmed: true));
			if (i == num2 - 1)
			{
				audioObject.OnComplete += delegate
				{
					GameManager.Instance.AudioManager.Play(__instance.m_HenryClip, AudioObjectType.DIALOGUE);
				};
			}
			if (i != 0 && i != num2 && i != num2 - 1 && i != num2 - 2 && i != num2 - 3)
			{
				num += audioClip.length;
			}
		}

		Transform freeRoamCamera = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		GameManager.Instance.Player.SetCameraSway(active: true);
		GameManager.Instance.Player.GoToAndLookAt(__instance.m_PlayerEndLocation);
		GameManager.Instance.Player.SetLock(active: true);
		Sequence s = DOTween.Sequence();
		float num3 = 0f;
		float length = __instance.m_JoeyClips[0].length;
		//Vector3 initialFreeRoamPosition = freeRoamCamera.localPosition;
		Vector3 initialFreeRoamPosition = new Vector3(359.7228f, -91.8852f, -299.3867f);


		s.Insert(num3, freeRoamCamera.DOMove(__instance.m_LookLocation.position, length).SetEase(Ease.InOutQuad));
		//s.Insert(num3, freeRoamCamera.DORotate(__instance.m_LookLocation.eulerAngles, length).SetEase(Ease.InOutQuad));
		num3 += length;
		//s.Insert(num3, freeRoamCamera.DORotate(new Vector3(0f, 360f, 0f), num, RotateMode.WorldAxisAdd).SetEase(Ease.InOutQuad));
		num3 += num;
		//s.Insert(num3, freeRoamCamera.DOLookAt(m_Reel.position, m_JoeyClips[num2 - 3].length).SetEase(Ease.InOutQuad));
		num3 += __instance.m_JoeyClips[num2 - 3].length;
		s.InsertCallback(num3 - 0.01f, delegate
		{
			__instance.m_Reel.SetParent(freeRoamCamera);
		});
		s.Insert(num3, __instance.m_Reel.DOLocalMove(new Vector3(0f, -0.2f, 2f), __instance.m_JoeyClips[num2 - 2].length).SetEase(Ease.InOutQuad));
		s.Insert(num3, __instance.m_Reel.DOLocalRotate(new Vector3(0f, -90f, -15f), __instance.m_JoeyClips[num2 - 2].length).SetEase(Ease.InOutQuad));
		num3 += __instance.m_JoeyClips[num2 - 2].length;
		s.Insert(num3, __instance.m_Reel.DOLocalRotate(new Vector3(0f, 90f, 15f), __instance.m_JoeyClips[num2 - 1].length).SetEase(Ease.InOutQuad));
		num3 += __instance.m_JoeyClips[num2 - 1].length + __instance.m_JoeyClips[num2].length;
		s.Insert(num3, __instance.m_Reel.DOLocalMove(new Vector3(0f, -3f, 0f), 0.5f).SetEase(Ease.InOutQuad));
		s.InsertCallback(num3, delegate
		{
			runningClip.Stop();
			__instance.m_Bendy.SetActive(value: true);
			GameManager.Instance.AudioManager.Play(__instance.m_OffClip);
			GameManager.Instance.AudioManager.Play(__instance.m_BendyMusicClip, AudioObjectType.MUSIC);
			S13AudioManager.Instance.PlayAudio("sfx_beast_reveal_anim");
		});
		num3 += 0.25f;
		s.InsertCallback(num3, delegate
		{
			//freeRoamCamera.SetParent(__instance.m_CameraParent);
			//freeRoamCamera.DOLocalMove(initialFreeRoamPosition, 4.0f);
			freeRoamCamera.DOMove(initialFreeRoamPosition, 4.0f);
			//freeRoamCamera.DOLocalRotate(new Vector3(0f, 90f, 90f), 0.5f);
		});

		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5TheEnd), nameof(CH5TheEnd.HandleReelStationOnComplete))]
	private static bool VRHandleReelStationOnComplete(CH5TheEnd __instance)
	{
		__instance.m_ReelStation.OnComplete -= __instance.HandleReelStationOnComplete;
		GameManager.Instance.HideCrosshair();
		__instance.m_Cartoons.SetActive(value: false);
		Transform freeRoamCamera = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		__instance.m_Bendy.SetActive(value: true);
		DOTween.Sequence().InsertCallback(0.1f, delegate
		{
			freeRoamCamera.SetParent(null);
			freeRoamCamera.position = __instance.m_CameraLook.position;
			freeRoamCamera.DOLocalRotate(new Vector3(0f, 270f, 0f), 2f).SetEase(Ease.InOutQuad);
			//freeRoamCamera.SetParent(__instance.m_CameraLook);
			//freeRoamCamera.DOLocalMove(Vector3.zero, 0.75f).SetEase(Ease.InOutQuad);
			//freeRoamCamera.DOLocalRotate(new Vector3(0f, 90f, 90f), 2f).SetEase(Ease.InOutQuad);
			//freeRoamCamera.SetParent(null);
		});
		GameManager.Instance.AudioManager.Play(__instance.m_EndMusic, AudioObjectType.MUSIC);
		S13AudioManager.Instance.PlayAudio("sfx_beast_death_anim");
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(BeastBendy_Ai), nameof(BeastBendy_Ai.ApplyShake))]
	[HarmonyPatch(typeof(Ch5BeastBendyChargeController), nameof(Ch5BeastBendyChargeController.ApplyShake))]
	public static bool DontApplyShake(float shakePower = 0.5f)
	{
		return false;
		/*float value = Vector3.Distance(__instance.transform.position, GameManager.Instance.Player.transform.position) / __instance.m_CameraShakeDistance;
		value = 1f - Mathf.Clamp01(value);
		GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		GameManager.Instance.GameCamera.transform.DOKill();
		GameManager.Instance.GameCamera.transform.DOShakePosition(0.5f, shakePower * value, 15).OnComplete(delegate
		{
			GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		})*/;		
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5PowerStation), nameof(CH5PowerStation.ValveOnComplete))]
	private static bool ValveOnComplete(CH5PowerStation __instance)
	{
		Transform target = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < __instance.m_Lights.Length; i++)
		{
			LightFlicker light = __instance.m_Lights[i];
			sequence.InsertCallback(0.5f + 0.25f * (float)i, delegate
			{
				light.gameObject.SetActive(value: true);
			});
		}
		//sequence.Insert(0.1f, target.DOLookAt(m_Lights[1].transform.position, 1f).SetEase(Ease.InOutQuad));
		sequence.OnComplete(__instance.StartUpOnComplete);
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH5BendyArena), nameof(CH5BendyArena.HandleOnStartUpComplete))]
	public static bool ArenaStartup(CH5BendyArena __instance)
	{
		__instance.m_PowerStation.OnStartUpComplete -= __instance.HandleOnStartUpComplete;
		__instance.m_PillarRoomEntranceDoor.Close();
		for (int i = 0; i < __instance.m_Pillars.Length; i++)
		{
			__instance.m_Pillars[i].OnHit += __instance.HandlePillarOnHit;
			__instance.m_Pillars[i].TurnOn();
		}
		__instance.m_MusicObject = GameManager.Instance.AudioManager.Play(__instance.m_InkDemonMusic, AudioObjectType.MUSIC, -1);
		__instance.m_MusicObject.AudioSource.volume = 0f;
		__instance.m_MusicObject.AudioSource.DOFade(1f, 0.5f).SetEase(Ease.Linear);
		__instance.m_Bendy.gameObject.SetActive(value: true);
		Vector3 towards = __instance.m_Bendy.transform.position + Vector3.up * 3f;
		//GameManager.Instance.GameCamera.FreeRoamCam.DOLookAt(towards, 0.5f).SetEase(Ease.InOutQuad).OnComplete(GameManager.Instance.GameCamera.ExitFreeRoamCam);
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		return false;
	}
}