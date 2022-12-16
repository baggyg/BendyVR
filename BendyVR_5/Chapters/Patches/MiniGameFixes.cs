using BendyVR_5.Helpers;
using BendyVR_5.Player;
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
public class MiniGameFixes : BendyVRPatch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(WinnableMiniGameBaseController), nameof(WinnableMiniGameBaseController.PrepPlayerForGame))]
	private static void VRPrepMiniGame(WinnableMiniGameBaseController __instance)
	{
        VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();

        //Poss Fix - Make sure this is the child of hand (not sure why its not here)
        Logs.WriteWarning("Preparing Mini Game Object " + __instance.HeldObject.name);
        
        //Set Up Axe Local Position
        __instance.HeldObject.SetParent(GameManager.Instance.Player.WeaponParent);

        if (__instance is Minigame_ShootingGallery ||
			__instance is Minigame_BallToss)
        {
            vrPlayerController.SetupGunWeapon(__instance.HeldObject.name);
        }		

		/*LineRenderer lineRenderer = __instance.HeldObject.gameObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
		lineRenderer.SetPositions(new[] { Vector3.zero, Vector3.right * 10f});
		/*lineRenderer.SetPositions(new[] {__instance.HeldObject.position + (__instance.HeldObject.rotation * new Vector3(0f, 0f, 0.7f)),
			(__instance.HeldObject.position + (__instance.HeldObject.rotation * new Vector3(0f, 0f, 0.7f))) + __instance.HeldObject.right * 100f});*/
		/*lineRenderer.startWidth = 0.005f;
		lineRenderer.endWidth = 0.001f;
		lineRenderer.endColor = new Color(1, 1, 1, 0.8f);
		lineRenderer.startColor = Color.clear;
		lineRenderer.material.shader = Shader.Find("Particles/Alpha Blended Premultiply");
		lineRenderer.material.SetColor(ShaderProperty.Color, Color.white);
		lineRenderer.sortingOrder = 10000;
		lineRenderer.enabled = true;*/
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(WinnableMiniGameBaseController), nameof(WinnableMiniGameBaseController.EnablePlayer))]
	private static void FinishMiniGame(WinnableMiniGameBaseController __instance)
	{
		//LineRenderer lineRenderer = __instance.HeldObject.gameObject.GetComponent<LineRenderer>();
		//UnityEngine.Object.Destroy(lineRenderer);
		if (GameManager.Instance.Player.WeaponGameObject == null)
		{
			VrCore.instance.GetVRPlayerController().TurnOnDominantHand();
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Minigame_BallToss), nameof(Minigame_BallToss.Update))]
	public static bool UpdateBallToss(Minigame_BallToss __instance)
	{
		//Can't use base so copy the code from there (instead of doing a Reverse Patch):
		//base.Update();
		/// BASE UPDATE ///
		if (!GameManager.Instance.isPaused && (!GameManager.Instance.Player || GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding) && (__instance.CurrentState == WinnableMiniGameBaseController.MiniGameState.ACTIVE || __instance.CurrentState == WinnableMiniGameBaseController.MiniGameState.PREPPING) && __instance.CurrentState == WinnableMiniGameBaseController.MiniGameState.ACTIVE)
		{
			if (PlayerInput.InteractOnPressed())
			{
				__instance.CurrentState = WinnableMiniGameBaseController.MiniGameState.INACTIVE;
				__instance.ForceExitGame();
			}
			__instance.m_CharacterLook.GetInput();
		}
		/// END BASE UPDATE ///
		if (__instance.CurrentState != WinnableMiniGameBaseController.MiniGameState.ACTIVE || __instance.m_Reloading)
		{
			return false;
		}

		//__instance.HeldObject.localPosition = Vector3.zero;
		//__instance.HeldObject.localEulerAngles = Vector3.zero;

		VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();
		/*if (PlayerInput.AttackHold())
		{
			vrPlayerController.attackedPressedMiniGame = true;
			vrPlayerController.AddVelocity();
		}
		else if(vrPlayerController.attackedPressedMiniGame)
        {*/
		if (PlayerInput.Attack())
		{
			//Reset count
			vrPlayerController.throwVelocityNext = -1;
			vrPlayerController.attackedPressedMiniGame = false;
			__instance.m_Reloading = true;
			__instance.m_AudioSwitch.Play("toss");
			Rigidbody rigidbody = UnityEngine.Object.Instantiate(__instance.m_BallObject);
			rigidbody.transform.position = __instance.HeldObject.position;
			
			//rigidbody.transform.eulerAngles = GameManager.Instance.GameCamera.transform.eulerAngles;
			//rigidbody.transform.eulerAngles = vrPlayerController.mDominantHand.eulerAngles;
			//Could try the ball itself
			rigidbody.transform.eulerAngles = __instance.HeldObject.eulerAngles;
			
			rigidbody.gameObject.SetActive(value: true);

			//This is 25 forward and 8 up (0 to the right) - so do something similar but only take forward force.
			rigidbody.velocity = 25f * 1.6f * rigidbody.transform.forward + rigidbody.transform.up * 8f - rigidbody.transform.right;			
			//Vector3 velocity = vrPlayerController.GetAverageVelocity();
			//velocity.x *= vrPlayerController.miniGameThrowMultiplier;
			//rigidbody.velocity = vrPlayerController.miniGameThrowMultiplier * vrPlayerController.GetAverageVelocity();
			//vrPlayerController.TryGetDominantHandNodeStateVelocity(out Vector3 velocity);
			//rigidbody.velocity = vrPlayerController.miniGameThrowMultiplier * velocity;
			//Logs.WriteWarning("Velocity = " + rigidbody.velocity);


			rigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(500f, 1000f), UnityEngine.Random.Range(500f, 1000f), UnityEngine.Random.Range(500f, 1000f)), ForceMode.Impulse);
			__instance.m_LastBallPosition = __instance.HeldObject.localPosition;
			__instance.HeldObject.gameObject.SetActive(value: false);
			//__instance.HeldObject.localPosition += new Vector3(0f, -5f, 0f);
			__instance.m_Balls.Add(rigidbody);
			__instance.m_BallCount++;
			SimpleOnHit component = rigidbody.GetComponent<SimpleOnHit>();
			if (component != null)
			{
				component.OnHit += __instance.HandleBallOnHit;
				component.OnHitGeneric += __instance.HandleBallOnHitGeneric;
			}
			__instance.ThrowSequence();
		}

		return false;
		/*m_TimeSinceStart += Time.deltaTime;
		m_ThrowForce = 0.8f * (1f + Mathf.Sin(6.28f * m_TimeSinceStart));
		base.HeldObject.localPosition = new Vector3(0f, 0.25f * (0f - m_ThrowForce), 0f);
		if (PlayerInput.Attack())
		{
			m_Reloading = true;
			m_AudioSwitch.Play("toss");
			Rigidbody rigidbody = UnityEngine.Object.Instantiate(m_BallObject);
			rigidbody.transform.position = base.HeldObject.position;
			rigidbody.transform.eulerAngles = GameManager.Instance.GameCamera.transform.eulerAngles;
			rigidbody.gameObject.SetActive(value: true);
			rigidbody.velocity = 25f * m_ThrowForce * rigidbody.transform.forward + rigidbody.transform.up * 8f - rigidbody.transform.right;
			rigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(500f, 1000f), UnityEngine.Random.Range(500f, 1000f), UnityEngine.Random.Range(500f, 1000f)), ForceMode.Impulse);
			m_LastBallPosition = base.HeldObject.localPosition;
			base.HeldObject.gameObject.SetActive(value: false);
			base.HeldObject.localPosition += new Vector3(0f, -5f, 0f);
			m_Balls.Add(rigidbody);
			m_BallCount++;
			SimpleOnHit component = rigidbody.GetComponent<SimpleOnHit>();
			if (component != null)
			{
				component.OnHit += HandleBallOnHit;
				component.OnHitGeneric += HandleBallOnHitGeneric;
			}
			ThrowSequence();
		}*/
	}


	[HarmonyPrefix]
	[HarmonyPatch(typeof(Minigame_ShootingGallery), nameof(Minigame_ShootingGallery.Update))]
	public static bool UpdateShootingGallery(Minigame_ShootingGallery __instance)
	{
		
		//Can't use base so copy the code from there (instead of doing a Reverse Patch):
		//base.Update();
		/// BASE UPDATE ///
		if (!GameManager.Instance.isPaused && (!GameManager.Instance.Player || GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding) && (__instance.CurrentState == WinnableMiniGameBaseController.MiniGameState.ACTIVE || __instance.CurrentState == WinnableMiniGameBaseController.MiniGameState.PREPPING) && __instance.CurrentState == WinnableMiniGameBaseController.MiniGameState.ACTIVE)
		{
			if (PlayerInput.InteractOnPressed())
			{
				__instance.CurrentState = WinnableMiniGameBaseController.MiniGameState.INACTIVE;
				__instance.ForceExitGame();
			}
			__instance.m_CharacterLook.GetInput();
		}
		/// END BASE UPDATE ///

		if (__instance.CurrentState != WinnableMiniGameBaseController.MiniGameState.ACTIVE || __instance.m_IsReloading || !PlayerInput.Attack() || __instance.m_CurrentShotCount <= 0)
		{
			return false;
		}

		//LineRenderer lineRenderer = __instance.HeldObject.gameObject.GetComponent<LineRenderer>();
		//lineRenderer.SetPosition(1, (__instance.HeldObject.position + (__instance.HeldObject.rotation * new Vector3(0f, 0f, 0.7f))) + __instance.HeldObject.right * 100f);
		//lineRenderer.SetPosition(1, Vector3.right * 10f);

		__instance.m_IsReloading = true;
		__instance.m_AudioSwitch.Play("fire");

		int layerMask = ~LayerMask.GetMask("Audio");
		if (Physics.Raycast(__instance.HeldObject.position + (__instance.HeldObject.rotation * new Vector3(0f, 0f, 0.7f)), __instance.HeldObject.right, out __instance.hit, 100f, layerMask))
		{
			Collider collider = __instance.hit.collider;
			if (__instance.m_AllTargets.ContainsKey(collider))
			{
				CH4FairGameTarget cH4FairGameTarget = __instance.m_AllTargets[collider];
				cH4FairGameTarget.Hit();
				if (cH4FairGameTarget.IsBad)
				{
					__instance.m_CurrentScore--;
					if (__instance.m_CurrentScore <= 0)
					{
						__instance.m_CurrentScore = 0;
					}
					__instance.m_AudioSwitch.Play("bad");
					Debug.Log("Weapon Missed and Hit Object: " + __instance.hit.transform.name);
					
					/*GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					sphere.transform.position = __instance.hit.transform.position;
					sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
					UnityEngine.Object.Destroy(sphere.GetComponent<Collider>());
					var renderer = sphere.GetComponent<MeshRenderer>();
					renderer.material.SetColor("_Color", Color.red);
					UnityEngine.Object.Destroy(sphere, 3f);*/
				}
				else
				{
					__instance.m_CurrentScore++;
					__instance.m_AudioSwitch.Play("good");
					/*GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					sphere.transform.position = __instance.hit.transform.position;
					sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
					UnityEngine.Object.Destroy(sphere.GetComponent<Collider>());
					var renderer = sphere.GetComponent<MeshRenderer>();
					renderer.material.SetColor("_Color", Color.green);
					UnityEngine.Object.Destroy(sphere, 3f);*/
				}
				Vector3 endValue = Vector3.Lerp(__instance.m_MeterStart.localPosition, __instance.m_MeterEnd.localPosition, (float)__instance.m_CurrentScore / (float)__instance.m_HighScore);
				__instance.m_Meter.DOKill();
				__instance.m_Meter.DOLocalMove(endValue, 0.35f).SetEase(Ease.InOutQuad);
				Sequence s = DOTween.Sequence();
				int num = 1;
				float num2 = 0f;
				for (int i = 0; i < 4; i++)
				{
					s.Insert(num2, __instance.m_Meter.DOLocalRotate(new Vector3(0f, 0f, 8f * (float)num), 0.125f).SetEase(Ease.Linear));
					num2 += 0.125f;
					num *= -1;
				}
			}
			/*else
            {
				//Hit somethng else
				Debug.Log("Weapon Missed and Hit Object: " + __instance.hit.transform.name);
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = __instance.hit.transform.position;
				sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				UnityEngine.Object.Destroy(sphere.GetComponent<Collider>());
				var renderer = sphere.GetComponent<MeshRenderer>();
				renderer.material.SetColor("_Color", Color.red);
				UnityEngine.Object.Destroy(sphere, 3f);
			}*/
		}
		/*GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.position = __instance.HeldObject.position + (__instance.HeldObject.rotation * new Vector3(0f, 0f, 0.5f));
		sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		UnityEngine.Object.Destroy(sphere.GetComponent<Collider>());

		GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere2.transform.position = ((__instance.HeldObject.position + (__instance.HeldObject.rotation * new Vector3(0f, 0f, 0.5f))) + (__instance.HeldObject.right * 1f));
		sphere2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		UnityEngine.Object.Destroy(sphere2.GetComponent<Collider>());
		var renderer = sphere2.GetComponent<MeshRenderer>();
		renderer.material.SetColor("_Color", Color.red);

		UnityEngine.Object.Destroy(sphere, 3f);
		UnityEngine.Object.Destroy(sphere2, 3f);*/
		


		__instance.m_CurrentShotCount--;
		__instance.DOReload();
		return false;
	}
}