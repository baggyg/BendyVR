using BendyVR_5.Helpers;
using BendyVR_5.Stage;
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
public class CH2Fixes : BendyVRPatch
{
	/*[HarmonyPrefix]
	[HarmonyPatch(typeof(CH2OpeningSequenceController), nameof(CH2OpeningSequenceController.DOOpeningSequence))]
	public static bool CH2OpeningFix(CH2OpeningSequenceController __instance, ref Sequence __result)
	{
		__result = DOTween.Sequence();
		Logs.WriteWarning("Playing Hurt Border");

		float num = 1f;
		float duration = 1f;
		__result.InsertCallback(num, delegate
		{
			GameManager.Instance.AudioManager.Play(__instance.m_TitleMusicClip, AudioObjectType.MUSIC);
			GameManager.Instance.ShowHurtBorder(isSilent: true);
		});
		num += 5f;

		Logs.WriteWarning("Show Chapter Title");

		__result.InsertCallback(num, delegate
		{
			GameManager.Instance.ShowChapterTitle("MENU/CH2_LABEL", "MENU/CH2_TITLE", showBlocker: false);
		});
		num += 2f;

		Logs.WriteWarning("Show Hurt Borders");

		for (int i = 0; i < 4; i++)
		{
			__result.InsertCallback(num, delegate
			{
				GameManager.Instance.ShowHurtBorder(isSilent: true);
			});
		}
		__result.InsertCallback(num, __instance.PlayIntroDialogue_01);
		__result.InsertCallback(num, delegate
		{
			GameManager.Instance.HideScreenBlocker(duration);
		});
		num += duration;
		duration = 0.5f;

		Logs.WriteWarning("Show Screen Blocker");

		__result.InsertCallback(num, delegate
		{
			GameManager.Instance.ShowScreenBlocker(duration);
		});
		num += duration + 0.75f;

		Logs.WriteWarning("Hide Screen Blocker");

		__result.InsertCallback(num, delegate
		{
			GameManager.Instance.HideScreenBlocker(duration);
		});
		num += duration + 0.25f;

		Logs.WriteWarning("Setting Focal Distance");

		if (__instance.m_GameCam.DoF)
		{
			__instance.m_GameCam.UnityDOF.manualDOF = true;
			float distance = __instance.m_GameCam.UnityDOF.focalDistance;
			__result.Insert(num, DOTween.To(() => distance, delegate (float value)
			{
				distance = value;
			}, 5f, 2f).SetEase(Ease.Linear).OnUpdate(delegate
			{
				__instance.m_GameCam.UnityDOF.focalDistance = distance;
			}));
		}

		Logs.WriteWarning("Show Screen Blocker");

		num += 1.5f;
		__result.InsertCallback(num, delegate
		{
			GameManager.Instance.ShowScreenBlocker();
		});
		num += 0.6f;

		Logs.WriteWarning("Unset DOF");

		if (__instance.m_GameCam.DoF)
		{
			__result.InsertCallback(num, delegate
			{
				__instance.m_GameCam.UnityDOF.manualDOF = false;
			});
		}

		Logs.WriteWarning("Hide Screen Blocker");

		__result.InsertCallback(num, delegate
		{
			GameManager.Instance.HideScreenBlocker();
		});

		return false;
	}*/

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH2OpeningSequenceController), nameof(CH2OpeningSequenceController.Activate))]
	public static bool VRActivateOpening(CH2OpeningSequenceController __instance)
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.RitualObjective.IsComplete)
		{
			__instance.ForceComplete();
			return false;
		}
		__instance.m_GameCam = GameManager.Instance.GameCamera;
		/*m_GameCamTransform = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		m_GameCamTransform.position = m_StartPosition.position;
		m_GameCamTransform.eulerAngles = m_StartPosition.eulerAngles;*/
		if (__instance.m_GameCam.DoF)
		{
			__instance.m_GameCam.UnityDOF.manualDOF = true;
			__instance.m_GameCam.UnityDOF.focalDistance = 0f;
		}
		__instance.DOOpeningSequence();
		return false;
	}

	//Get rid of VR nausea simulator (CH2 Opening)
	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH2OpeningSequenceController), nameof(CH2OpeningSequenceController.DOGetUpSequence))]
	private static bool DontGetUp(CH2OpeningSequenceController __instance, ref Sequence __result)
	{
		Sequence sequence = DOTween.Sequence();
		float num = 1f;
		/*sequence.InsertCallback(num, delegate
		{
			__instance.m_GameCam.Camera.transform.SetParent(GameManager.Instance.Player.CameraParent);
		});*/
		sequence.InsertCallback(num += 0.5f, __instance.PlayIntroDialogue_02);
		__result = sequence;
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH2OpeningSequenceController), nameof(CH2OpeningSequenceController.GetUpSequenceOnComplete))]
	public static bool DontGetUpOnComplete(CH2OpeningSequenceController __instance)
	{
		//GameManager.Instance.GameCamera.ExitFreeRoamCam();
		__instance.PlayIntroDialogue_03();
		GameManager.Instance.Player.SetLock(active: false);
		GameManager.Instance.UnlockPause();
		GameManager.Instance.Player.SetInteraction(active: true);
		GameManager.Instance.Player.SetCameraSway(active: true);
		return false;
	}

	[HarmonyPrefix]
    [HarmonyPatch(typeof(CH2RitualRoomController), nameof(CH2RitualRoomController.ForceComplete))]
    private static bool VRCH2RitualRoomControllerComplete(CH2RitualRoomController __instance)
    {
        GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
        GameManager.Instance.Player.WeaponGameObject = __instance.m_Axe.gameObject;
        GameManager.Instance.Player.EquipWeapon();
        if ((bool)__instance.m_Axe && __instance.m_Axe.Interaction != null)
        {
            __instance.m_Axe.Interaction.SetActive(active: false);
        }
        __instance.m_Axe.KillInteraction();
        __instance.m_Axe.Equip();

        __instance.m_Plank.gameObject.SetActive(value: false);
        __instance.m_ScarePlank.gameObject.SetActive(value: false);
        __instance.m_Door.ForceOpen(145f);
        __instance.m_Door.Lock();
        __instance.SendOnComplete();

        return false;
    }

    

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CH2ClosingSequenceController), nameof(CH2ClosingSequenceController.HandleFinalTriggerOnEnter))]
    private static bool VRHandleFinalTriggerOnEnter(CH2ClosingSequenceController __instance)
    {
        __instance.m_FinalTrigger.OnEnter -= __instance.HandleFinalTriggerOnEnter;
        GameManager.Instance.HideCrosshair();
        GameManager.Instance.LockPause();
        GameManager.Instance.Player.SetCameraSway(active: true);
        GameManager.Instance.Player.SetLock(active: true);
        //GameManager.Instance.Player.HeadContainer.DOLookAt(__instance.m_FinalLookAt.position, 2f).SetEase(Ease.InOutQuad);
        //VrCore.instance.GetVRPlayerController().mNewCameraParent.DOLookAt(__instance.m_FinalLookAt.position, 2f).SetEase(Ease.InOutQuad);        

        //TODO Might remove this completely - since if they turn around physically it will be off. 
        VrCore.instance.GetVRPlayerController().mPlayerController.transform.DOLookAt(__instance.m_FinalLookAt.position, 2f).SetEase(Ease.InOutQuad);
        GameManager.Instance.AudioManager.Play(__instance.m_CanKickClip);
        __instance.DOSequence().OnComplete(__instance.SequenceOnComplete);
        return false;
    }

	

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CH2BendyChaseController), nameof(CH2BendyChaseController.HandleBendyOnEntered))]
	private static bool FixBendyEncounter(CH2BendyChaseController __instance)
	{
		__instance.m_BendyTrigger.OnEnter -= __instance.HandleBendyOnEntered;
		__instance.m_BendyTrigger.Dispose();
		for (int i = 0; i < __instance.m_DisableGameObjects.Count; i++)
		{
			__instance.m_DisableGameObjects[i].SetActive(value: true);
		}
		for (int j = 0; j < __instance.m_ActiveGameObjects.Count; j++)
		{
			__instance.m_ActiveGameObjects[j].SetActive(value: false);
		}
		for (int k = 0; k < __instance.m_DisableSpawners.Length; k++)
		{
			__instance.m_DisableSpawners[k].SetActive(value: false);
		}
		for (int l = 0; l < __instance.m_EnableSpawners.Length; l++)
		{
			__instance.m_EnableSpawners[l].SetActive(value: true);
		}
		GameManager.Instance.AudioManager.Play(__instance.m_CeilingCollapseClip);
		GameManager.Instance.AudioManager.Play(__instance.m_CelingSettleClip);
		__instance.BendyReveal();
		__instance.m_DoorController.Open(0f, Ease.Linear, -125f);
		__instance.m_BarricadeTrigger.SetActive(active: true);
		__instance.m_BarricadeTrigger.OnEnter += __instance.HandleBarricadeTriggerOnEnter;
		GameManager.Instance.CurrentChapter.DeathController.OnDeath += __instance.HandlePlayerOnDeath;
		
		return false;
	}
}

