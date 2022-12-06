using BendyVR_5.Helpers;
using DG.Tweening;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BendyVR_5.Player.Patches;

[HarmonyPatch]
public class PlayerControllerPatches : BendyVRPatch
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(PlayerController), nameof(PlayerController.FixedUpdate))]
	[HarmonyPatch(typeof(PlayerController), nameof(PlayerController.Update))]
	private static bool RemoveCorePlayerUpdates(PlayerController __instance)
	{
		return false; 
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(EventTrigger), nameof(EventTrigger.OnTriggerEnter))]
	private static void WhatAmICollidingWith(EventTrigger __instance, Collider col)
	{
		Logs.WriteWarning(__instance.transform.name + " Collided with " + col.transform.name);
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(HurtBordersController), nameof(HurtBordersController.ShowBorder))]
	public static bool VRShowBorder(HurtBordersController __instance, bool isSilent = false)
	{
		if (__instance.m_HitCount >= __instance.m_HitMax)
		{
			/*if (!__instance.m_IsHitMax)
			{
				GameManager.Instance.isDead = true;
				__instance.m_HitCount--;
				__instance.m_IsHitMax = true;
				__instance.m_BlackImage.DOKill();
				__instance.m_BlackImage.DOFade(1f, 0.2f).SetEase(Ease.Linear);
				GameManager.Instance.ShowScreenBlocker(0f);
				GameManager.Instance.HideCrosshair();
				__instance.PlayAudio(ref __instance.m_DeathClips);
				//How do I do this?
				//__instance.OnMaxHit.Send(__instance);				
				__instance.OnMaxHit += __instance;
			}*/
			return true; //If we are dying send through the original code
		}
		if (!isSilent)
		{
			__instance.PlayAudio(ref __instance.m_HurtClips);
		}
		__instance.m_IsHit = true;
		__instance.m_Timer = 0f;
		//Don't do any camera stuff
		/*Transform camera = GameManager.Instance.GameCamera.transform;
		camera.localPosition = Vector3.zero;
		camera.DOShakePosition(0.75f, 0.35f, 15).OnComplete(delegate
		{
			camera.localPosition = Vector3.zero;
		});*/
		for (int i = 0; i < __instance.m_HitMax && i <= __instance.m_HitCount; i++)
		{
			Image image = __instance.m_Borders[i];
			image.DOKill();
			image.rectTransform.DOKill();
			image.enabled = true;
			image.color = Color.white;
			image.rectTransform.localScale = Vector3.one;
			image.rectTransform.DOScale(1.005f, 0.25f + (float)i * 0.005f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
		}
		__instance.m_HitCount++;
		return false;
	}

	/*[HarmonyPrefix]
	[HarmonyPatch(typeof(PlayerController), nameof(PlayerController.GetRotations))]
	private static bool OverrideRotations(PlayerController __instance)
	{
		if (!__instance.isLocked)
		{
			//Save the camera absolute rotation
			Vector3 savedRotation = GameManager.Instance.GameCamera.transform.eulerAngles;
			__instance.m_PlayerLook.Rotation(__instance.transform, __instance.m_HeadContainer, __instance.m_HandContainer);
			savedRotation.z = 0f;
			GameManager.Instance.GameCamera.transform.eulerAngles = savedRotation;			
		}
		if (__instance.m_CharacterController.isGrounded && !__instance.m_PreviouslyGrounded && __instance.m_LandingAudioBS)
		{
			__instance.StartCoroutine(__instance.m_HeadBob.DoJumpBob());
			__instance.m_PlayerFootsteps.PlayLandAudio();
			__instance.m_ExternalForce = Vector3.zero;
		}
		
		

		return false;
	}

	/*[HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.GetRotations))]
    private static void OverrideRotations(PlayerController __instance)
    {
		//offset camera movement
		Transform cameraParent = __instance.CameraParent.Find("VrCameraParent");
		//Transform cameraParent = __instance.m_CameraContainer;
		Logs.WriteInfo("Camera's Parent: " + cameraParent.name);

		//Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
		Quaternion targetRotation = Quaternion.Euler(-__instance.m_PlayerLook.m_InputY, 0f, 0f);
		//Logs.WriteInfo("Reverse Y Received as " + (-__instance.m_PlayerLook.m_InputY).ToString());
		cameraParent.rotation = targetRotation;
		Vector3 eulerAngles = cameraParent.eulerAngles;
		eulerAngles.z = 0f;
		cameraParent.eulerAngles = eulerAngles;
				
	}*/

	/*[HarmonyPrefix]
	[HarmonyPatch(typeof(CharacterLook), nameof(CharacterLook.GetInput))]
	private static bool GetCorrectYInput(CharacterLook __instance)
    {
		float tSpeed = -0.4f;
		if (__instance.turnSpeedBoostTimer > 0.3f)
		{
			tSpeed = 0f;
		}
		float num = PlayerInput.LookX(tSpeed);
		if (Mathf.Abs(num) > 0.1f)
		{
			__instance.turnSpeedBoostTimer += Time.deltaTime;
		}
		else
		{
			__instance.turnSpeedBoostTimer = 0f;
		}
		__instance.m_InputX = num * __instance.m_Sensitivity;

		//__instance.m_InputY = (0f - PlayerInput.LookY()) * __instance.m_Sensitivity;
		//Instead of above lets just take the exact Y angle from the camera
		__instance.m_InputY = GameManager.Instance.GameCamera.transform.eulerAngles.x;
		//Logs.WriteInfo(GameManager.Instance.GameCamera.name + ": " + GameManager.Instance.GameCamera.transform.localEulerAngles.x.ToString() + " : " + GameManager.Instance.GameCamera.transform.eulerAngles.x.ToString());
		return false;
	}
	
	
	[HarmonyPrefix]
	[HarmonyPatch(typeof(CharacterLook), nameof(CharacterLook.Rotation), new Type[] { typeof(Transform), typeof(Transform), typeof(bool) })]
	public static bool ViewRotation(CharacterLook __instance, Transform character, Transform camera, bool hasGravity)
	{
		if (!__instance.IsNullRotation(__instance.m_InputX, __instance.m_InputY))
		{
			if (hasGravity)
			{
				__instance.m_CharacterTargetRotation *= Quaternion.Euler(0f, __instance.m_InputX, 0f);
				if (__instance.hasHorizontalLock)
				{
					__instance.m_CharacterTargetRotation = __instance.ClampRotationYAxis(__instance.m_CharacterTargetRotation, 0f - __instance.m_HorizontalClamp, __instance.m_HorizontalClamp);
				}
				character.localRotation = __instance.m_CharacterTargetRotation;
				if ((bool)camera)
				{
					//Not times by
					//__instance.m_CameraTargetRotation *= Quaternion.Euler(m_InputY, 0f, 0f);
					__instance.m_CameraTargetRotation = Quaternion.Euler(__instance.m_InputY, 0f, 0f);
					if (__instance.m_ClampVerticalRotation)
					{
						__instance.m_CameraTargetRotation = __instance.ClampRotationXAxis(__instance.m_CameraTargetRotation, 0f - __instance.m_VerticalClamp, __instance.m_VerticalClamp);
					}
					camera.rotation = __instance.m_CameraTargetRotation;
					Vector3 localEulerAngles = camera.eulerAngles;
					localEulerAngles.z = 0f;
					camera.eulerAngles = localEulerAngles;
					//character.localRotation = __instance.m_CameraTargetRotation;
				}
			}
			else
			{
				__instance.m_CharacterTargetRotation *= Quaternion.Euler(__instance.m_InputY, __instance.m_InputX, 0f);
				//character.localRotation = __instance.m_CharacterTargetRotation;
			}
		}
		else
		{
			//character.localRotation = __instance.m_CharacterTargetRotation;
			if ((bool)camera)
			{
				camera.localRotation = __instance.m_CameraTargetRotation;
			}
		}
		__instance.UpdateCursorLock();
		return false;
	}*/
}
