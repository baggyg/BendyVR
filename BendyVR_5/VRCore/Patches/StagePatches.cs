using BendyVR_5.Helpers;
using BendyVR_5.UI;
using DG.Tweening;
using HarmonyLib;
using TMG.Core;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.XR;

namespace BendyVR_5.Stage.Patches;

[HarmonyPatch]
public class StagePatches : BendyVRPatch
{
    private static GameManager gameManager;
    private static Transform originalParent;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(InitializeGame), nameof(InitializeGame.InitPlayerSettings))]
    private static void CreateVRCore(InitializeGame __instance)
    {        
        gameManager = __instance.m_GameManager;
        VrCore.Create(gameManager);        
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.Init))]
    public static void SetUpStagePlayer(PlayerController __instance)
    {
        // Getting camera manually because cameraController.camera isn't set up yet.
        //var camera = __instance.cameraController.GetComponentInChildren<Camera>();

        //First Turn off the tracking of the weapon camera        
        XRDevice.DisableAutoXRCameraTracking(GameManager.Instance.GameCamera.WeaponCamera, true);
        
        //Also Turn Off tracking of the main camera
        //XRDevice.DisableAutoXRCameraTracking(GameManager.Instance.GameCamera.Camera, true);

        var camera = GameManager.Instance.GameCamera.Camera;
        Logs.WriteInfo("Setting Up VR Core (PlayerController Initialised)");
        StageInstance.SetUp(camera, __instance);
    }

    [HarmonyPrefix]
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
    }

	

    [HarmonyPrefix]    
    [HarmonyPatch(typeof(CH3OpeningSequenceController), nameof(CH3OpeningSequenceController.Activate))]
    [HarmonyPatch(typeof(CH4OpeningSequenceController), nameof(CH4OpeningSequenceController.Activate))]
    [HarmonyPatch(typeof(CH5OpeningSequenceController), nameof(CH5OpeningSequenceController.Activate))]
    public static bool EndDemo()
    {
        ErrorPresenter.Show("End of the Early Access 2 Demo. Your game has been saved!");
        
        return false;
    }


    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(vgDestroyAllGameObjects), "OnEnter")]
    private static void PreventStageDestructionStart(object __instance)
    {
        originalParent = StageInstance.transform.parent.parent;
        StageInstance.transform.parent.SetParent(resetObject.transform);
    }*/

    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(vgDestroyAllGameObjects), "OnEnter")]
    private static void PreventStageDestructionEnd()
    {
        StageInstance.transform.parent.SetParent(originalParent);
    }*/


}