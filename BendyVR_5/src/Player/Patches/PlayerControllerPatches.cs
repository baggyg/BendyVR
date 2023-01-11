using BendyVR_5.Helpers;
using BendyVR_5.src;
using DG.Tweening;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TMG.Controls;
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
	[HarmonyPatch(typeof(InteractableInputController), nameof(InteractableInputController.UpdateInteraction), new Type[] { typeof(Vector3), typeof(Vector3), typeof(float) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
	public static bool VRUpdateInteraction(InteractableInputController __instance, Vector3 origin, Vector3 direction, float distance)
	{
		if (!__instance.m_Active)
		{
			return false;
		}
		//if (Physics.SphereCast(origin, __instance.m_SphereCastThickness, direction, out var hitInfo, distance, ~(int)__instance.m_IgnoreLayers))
		if (Physics.SphereCast(origin, 0.3f, direction, out var hitInfo, distance, ~(int)__instance.m_IgnoreLayers))
		{
			Interactable component = hitInfo.transform.GetComponent<Interactable>();
			if ((bool)component)
			{
				__instance.DrawDebugLine(origin, hitInfo.point, Color.yellow);
				if (__instance.Interactable != component)
				{
					__instance.ExitInteraction();
				}
				__instance.Interactable = component;
				if (PlayerInput.InteractOnPressed())
				{
					__instance.Interact();
				}
				else
				{
					__instance.EnterInteraction();
				}
			}
			else
			{
				__instance.ExitInteraction();
			}
		}
		else
		{
			__instance.ExitInteraction();
		}
		return false;
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
}
