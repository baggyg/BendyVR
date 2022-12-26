using BendyVR_5.Helpers;
using BendyVR_5.UI;
using DG.Tweening;
using HarmonyLib;
using TMG.Controls;
using TMG.Core;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.XR;


namespace BendyVR_5.Chapters.Patches;

[HarmonyPatch]
public class CH1Fixes : BendyVRPatch
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH1BendyFinaleController), nameof(CH1BendyFinaleController.Update))]
	private static bool FixBendyEncounter(CH1BendyFinaleController __instance)
	{
		//if (__instance.m_CanScare && !__instance.m_InkMachineMeshRenderer.isVisible)
		//	Logs.WriteInfo("Can't See the Ink Machine!!!");

		if (__instance.m_ShowRunTutorial && PlayerInput.Run())
		{
			GameManager.Instance.HideTutorial();
			__instance.m_ShowRunTutorial = false;
		}
		//if (__instance.m_CanScare && m_InkMachineMeshRenderer.isVisible)
		if (__instance.m_CanScare)
		{
			__instance.m_BnedyEventTrigger.OnEnter -= __instance.HandleBendyEventTriggerOnEnter;
			__instance.m_BnedyEventTrigger.OnExit -= __instance.HandleBendyEventTriggerOnExit;
			__instance.m_BnedyEventTrigger.Dispose();
			__instance.m_InkMachineMeshRenderer.enabled = false;
			__instance.m_CanScare = false;
			__instance.m_HasEffects = true;
			Sequence s = DOTween.Sequence();
			float num = 0f;
			s.InsertCallback(num, __instance.ActualActivate);
			s.InsertCallback(num + 2f, delegate
			{
				__instance.m_Music = GameManager.Instance.AudioManager.Play(__instance.m_MusicClip, AudioObjectType.MUSIC, -1);
			});
			Transform target = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
			GameManager.Instance.Player.GoToAndLookAt(__instance.m_HenryFallLocation);
			s.InsertCallback(num, delegate
			{
				GameManager.Instance.GameCamera.transform.DOShakePosition(3f, 0.6f, 15);
			});
			s.Insert(num, target.DOLookAt(__instance.m_BendyLookAt.position, 0.2f).SetEase(Ease.Linear));
			num += 0.4f;
			s.Insert(num, target.DOMoveX(__instance.m_HenryFallLocation.position.x, 0.5f).SetEase(Ease.Linear));
			s.Insert(num, target.DOMoveZ(__instance.m_HenryFallLocation.position.z, 0.5f).SetEase(Ease.Linear));
			num += 0.35f;
			s.Insert(num, target.DOMoveY(__instance.m_HenryFallLocation.position.y - 2f, 0.15f).SetEase(Ease.Linear));
			s.InsertCallback(num + 0.15f, delegate
			{
				GameManager.Instance.AudioManager.Play(__instance.m_HenryBodyFallClip);
			});
			s.Insert(num, target.DORotate(new Vector3(-70f, -15f, 20f), 0.4f).SetEase(Ease.Linear));
			num += 0.75f;
			s.InsertCallback(num, delegate
			{
				__instance.m_BENDY.SetActive(value: false);
			});
			s.Insert(num, target.DORotate(__instance.m_HenryFallLocation.eulerAngles, 1f).SetEase(Ease.InOutQuad));
			s.Insert(num, target.DOMove(GameManager.Instance.Player.HeadContainer.position, 1f).SetEase(Ease.InOutQuad));
			s.InsertCallback(num + 1f, delegate
			{
				GameManager.Instance.GameCamera.ExitFreeRoamCam();
				GameManager.Instance.Player.SetLock(active: false);
				GameManager.Instance.ShowTutorial(new TutorialDataVO("Tutorial/TUTORIAL_RUN"));
				__instance.m_ShowRunTutorial = true;
			});
			num += 1.5f;
			s.InsertCallback(num, delegate
			{
				GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_06", string.Empty, 2f, isCurrentObjective: false, 1.75f));
			});
		}
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH1BendyFinaleController), nameof(CH1BendyFinaleController.ShakeCamera))]
	private static bool DontShakeCamera()
	{
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH1FinaleController), nameof(CH1FinaleController.HandleAxeOnEquipped))]
	private static bool DontShowTutorial(CH1FinaleController __instance)
	{
		//GameManager.Instance.ShowTutorial(new TutorialDataVO("Tutorial/TUTORIAL_ATTACK"));
		//m_ShowAttackTutorial = true;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(__instance.m_HenryClip07, "DIACH1/DIA_CH1_HENRY_07"));
		for (int i = 0; i < __instance.m_Breakables.Count; i++)
		{
			__instance.m_Breakables[i].OnBroken += __instance.HandleOnBroken;
		}
		return false;
	}
}

