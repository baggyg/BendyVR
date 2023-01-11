using BendyVR_5.Helpers;
using BendyVR_5.Player;
using BendyVR_5.src;
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
	[HarmonyPrefix]
	[HarmonyPatch(typeof(WinnableMiniGameBaseController), nameof(WinnableMiniGameBaseController.PrepPlayerForGame))]
	private static bool VRPrepPlayerForGame(WinnableMiniGameBaseController __instance)
	{		
		GameManager.Instance.ShowScreenBlocker();
		Vector3 endValue = GameManager.Instance.Player.transform.position - GameManager.Instance.Player.transform.forward * 5f;
		GameManager.Instance.Player.SetCollision(active: false);
		__instance.m_FreeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		__instance.m_LocalLookTransform.localRotation = Quaternion.identity;
		Sequence sequence = DOTween.Sequence();
		float num = 0f;
		if (__instance.m_HeldItemPositionTransform != null)
		{
			sequence.Insert(num, __instance.m_HeldItemPositionTransform.DOLocalMoveY(-5f, 0.65f).SetEase(Ease.InOutQuad));
		}
		if (__instance.m_PickUpInteractionObject)
		{
			sequence.InsertCallback(num, __instance.OnPickupObject);
			sequence.Insert(num, __instance.m_InteractGameStart.transform.DOMove(endValue, 0.65f).SetEase(Ease.InOutQuad));
			num += 0.3f;
			sequence.InsertCallback(num, delegate
			{
				__instance.m_InteractGameStart.gameObject.SetActive(value: false);
			});
		}
		if (GameManager.Instance.Player.WeaponGameObject != null)
		{
			sequence.Insert(num, GameManager.Instance.Player.WeaponGameObject.transform.DOLocalMoveY(-5f, 0.25f).SetEase(Ease.InQuad));
			sequence.Insert(num, GameManager.Instance.Player.WeaponGameObject.transform.DOLocalRotate(new Vector3(180f, 0f, 0f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.InQuad));
			sequence.InsertCallback(num + 0.1f, delegate
			{
				GameManager.Instance.Player.WeaponGameObject.SetActive(value: false);
				GameManager.Instance.Player.UnEquipWeapon();
			});
		}
		
		num += 0.15f;
		sequence.Insert(num, __instance.m_FreeRoamCam.DOMove(__instance.m_LocalLookTransform.position, 0.65f).SetEase(Ease.InOutQuad));
		sequence.Insert(num, __instance.m_FreeRoamCam.DOMove(__instance.m_LocalLookTransform.position, 0.65f).SetEase(Ease.InOutQuad));
		sequence.Insert(num, __instance.m_FreeRoamCam.DORotate(__instance.m_LocalLookTransform.eulerAngles, 0.7f).SetEase(Ease.InOutQuad));
		num += 0.5f;
		sequence.OnComplete(__instance.HandleOnPlayerPrepped);
		
		return false;
	}
	
	[HarmonyPostfix]
	[HarmonyPatch(typeof(WinnableMiniGameBaseController), nameof(WinnableMiniGameBaseController.HandleOnPlayerPrepped))]
	private static void HandleOnPlayerPrepped()
	{
		GameManager.Instance.HideScreenBlocker(0.5f);		
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(WinnableMiniGameBaseController), nameof(WinnableMiniGameBaseController.HandleOnQuitGame))]
	private static void VRHandleOnQuitGame()
	{
		GameManager.Instance.ShowScreenBlocker();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(WinnableMiniGameBaseController), nameof(WinnableMiniGameBaseController.EnablePlayer))]
	private static void VREnablePlayer()
	{
		GameManager.Instance.HideScreenBlocker(0.5f);
	}

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
			__instance is Minigame_BallToss ||
			__instance is Minigame_Darts)
        {
            vrPlayerController.SetupGunWeapon(__instance.HeldObject.name);
        }		
		else if(__instance is Minigame_Hammer)
        {
			vrPlayerController.SetupMeleeWeapon(__instance.HeldObject.name);
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
	[HarmonyPatch(typeof(WinnableMiniGameBaseController), nameof(WinnableMiniGameBaseController.HandleBeginGame))]
	public static void PrepareColliders(WinnableMiniGameBaseController __instance)
	{
		if(__instance is Minigame_Hammer)
        {
			//Find the something in the scene
			Transform bell_bottom = GameObject.Find("Bell_Bottom").transform;
			Transform elementParent = bell_bottom.GetParent();
			
			//Remove the collision 
			Transform collision = elementParent.Find("Collision");
			collision.gameObject.SetActive(false);
			Logs.WriteWarning("Collision Turned Off");

			//Add a collision on the button
			Transform button1 = elementParent.Find("Button");
			Transform button = button1.Find("FairGameHammerButton");
			button.gameObject.AddComponent<BoxCollider>();
			Logs.WriteWarning("BoxCollider Added");
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Minigame_Hammer), nameof(Minigame_Hammer.Update))]
	public static bool HammerHit(Minigame_Hammer __instance)
	{
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
		if (__instance.CurrentState == WinnableMiniGameBaseController.MiniGameState.ACTIVE && __instance.m_GameReset)
		{
			
			//UnityEngine.Object.Destroy(__instance.HeldObject.GetComponent<Collider>());
			/*
			m_TimeSinceStart += Time.deltaTime;
			m_HammerStrength = 0.5f * (1f + Mathf.Sin(6.28f * m_TimeSinceStart));
			base.HeldObject.localRotation = Quaternion.Euler(-35f * m_HammerStrength, 0f, 0f);
			if (PlayerInput.Attack())
			{
				m_CurrentTry++;
				m_ClickValue = m_HammerStrength;
				m_GameReset = false;
				DOHammerHit();
			}*/
			Transform transform =__instance.HeldObject;
			
			int layerMask = ~LayerMask.GetMask("Audio");
			if (Physics.SphereCast(transform.position + (transform.rotation * new Vector3(0f, 0f, 2.5f)), 0.5f, -transform.up, out var hitInfo, 0.5f, layerMask))
			{
				Logs.WriteInfo("HIT " + hitInfo.transform.name + " " + hitInfo.transform.GetParent().name);

				/*GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = transform.position + (transform.rotation * new Vector3(0f, 0f, 2.5f));
				sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				UnityEngine.Object.Destroy(sphere.GetComponent<Collider>());
            
				GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere2.transform.position = hitInfo.point;
				sphere2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				UnityEngine.Object.Destroy(sphere2.GetComponent<Collider>());
				var renderer = sphere2.GetComponent<MeshRenderer>();
				renderer.material.SetColor("_Color", Color.green);
            
				UnityEngine.Object.Destroy(sphere, 3f);
				UnityEngine.Object.Destroy(sphere2, 3f);*/

				if(hitInfo.transform.name.Equals("FairGameHammerButton"))
                {
					__instance.m_CurrentTry++;
					VrCore.instance.GetVRPlayerController().TryGetDominantHandNodeStateAngularVelocity(out Vector3 angularVelocity);
					//__instance.m_ClickValue = __instance.m_HammerStrength;
					__instance.m_ClickValue = angularVelocity.magnitude + 0.2f;
					Logs.WriteWarning("Velocity = " + angularVelocity.magnitude);

					//Theoretical Maximum = 1f
					//0.5f * (1f + Mathf.Sin(6.28f * m_TimeSinceStart))
					if (__instance.m_ClickValue > 1)
						__instance.m_ClickValue = 1;
					
					__instance.m_GameReset = false;
					__instance.DOHammerHit();
				}
				/*if (hitInfo.collider.gameObject.Equals(base.gameObject))
				{
					m_IsFlashing = true;
					DOFlash().OnComplete(DisableFlash);
				}*/
			}
		}
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Minigame_Hammer), nameof(Minigame_Hammer.DOHammerHit))]
	public static bool HammerHitVR(Minigame_Hammer __instance)
	{
		if (__instance.CurrentState == WinnableMiniGameBaseController.MiniGameState.ACTIVE)
		{
			float num = 0.5f * __instance.m_ClickValue + 0.1f;
			Vector3 endValue = Vector3.Lerp(__instance.m_BellBottom.localPosition, __instance.m_BellTop.localPosition, __instance.m_ClickValue);
			Sequence sequence = DOTween.Sequence();
			float num2 = 0f;
			/*sequence.Insert(num2, base.HeldObject.DOLocalRotate(new Vector3(90f, 0f, 0f), 0.2f).SetEase(Ease.InQuad));
			sequence.InsertCallback(num2, delegate
			{
				__instance.m_AudioSwitch.Play("swing");
			});
			num2 += 0.2f;
			//sequence.Insert(num2, base.HeldObject.DOLocalRotate(new Vector3(-35f * m_ClickValue, 0f, 0f), 0.3f).SetEase(Ease.InOutQuad));*/
			sequence.InsertCallback(num2, delegate
			{
				__instance.m_AudioSwitch.Play("hit");
			});
			sequence.Insert(num2, __instance.m_Bell.DOLocalMove(endValue, num).SetEase(Ease.OutQuad));
			sequence.InsertCallback(num2, delegate
			{
				__instance.m_AudioSwitch.Play("slide");
			});
			sequence.Insert(num2, __instance.m_Button.DOLocalMoveY(0f - __instance.m_ClickValue, 0.15f).SetEase(Ease.Linear));
			sequence.Insert(num2 + 0.15f, __instance.m_Button.DOLocalMoveY(0f, 0.5f).SetEase(Ease.InOutQuad));
			num2 += num;
			sequence.InsertCallback(num2, __instance.CheckBellHit);
			sequence.Insert(num2, __instance.m_Bell.DOLocalMove(__instance.m_BellBottom.localPosition, num).SetEase(Ease.InQuad));
			num2 += num;
			sequence.InsertCallback(num2 - 0.1f, delegate
			{
				__instance.m_AudioSwitch.Play("reset");
			});
			sequence.OnComplete(__instance.CheckBellWinAndReset);
		}
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Minigame_Darts), nameof(Minigame_Darts.Update))]
	public static bool UpdateDartUpdate(Minigame_Darts __instance)
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

		if (__instance.CurrentState == WinnableMiniGameBaseController.MiniGameState.ACTIVE && !__instance.m_Reloading)
		{
			//m_TimeSinceStart += Time.deltaTime;
			//m_ThrowForce = 0.75f * (1f + Mathf.Sin(6.28f * m_TimeSinceStart));
			//base.HeldObject.localPosition = new Vector3(0f, 0f, 0.5f * (0f - m_ThrowForce));
			if (PlayerInput.Attack())
			{
				__instance.m_DartCount++;
				__instance.m_Reloading = true;
				ThrowableObject throwableObject = UnityEngine.Object.Instantiate(__instance.m_ThrowableDart);
				throwableObject.transform.position = __instance.HeldObject.position;
				throwableObject.transform.eulerAngles = GameManager.Instance.GameCamera.FreeRoamCam.eulerAngles;
				//throwableObject.transform.eulerAngles = __instance.HeldObject.eulerAngles;
				throwableObject.gameObject.SetActive(value: true);
				throwableObject.Initialize(throwableObject.WeaponInfo, -__instance.HeldObject.transform.right * throwableObject.Force);
				throwableObject.OnHit += __instance.HandleDartOnHit;
				throwableObject.Throw();
				__instance.ActiveDarts.Add(throwableObject);
				__instance.HeldObject.gameObject.SetActive(value: false);
				__instance.HeldObject.localPosition += new Vector3(0f, -5f, 0f);
				__instance.Reload();
				__instance.m_AudioSwitch.Play("fire");
			}
		}

		return false;
	}

	/*[HarmonyPrefix]
	[HarmonyPatch(typeof(Minigame_Darts), nameof(Minigame_Darts.Update))]
	private static bool Reload()
	{
		if (base.CurrentState != MiniGameState.ACTIVE)
		{
			return;
		}
		if (m_DartCount >= m_DartMax)
		{
			m_DartCount = 0;
			ExitGameSequence = DOTween.Sequence();
			ExitGameSequence.InsertCallback(2f, delegate
			{
				SetState(MiniGameState.INACTIVE);
				ExitGame();
			});
			return;
		}
		Sequence s = DOTween.Sequence();
		float num = 1f;
		s.InsertCallback(num, delegate
		{
			base.HeldObject.gameObject.SetActive(value: true);
		});
		s.Insert(num, base.HeldObject.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuad));
		num += 0.5f;
		s.InsertCallback(num, delegate
		{
			m_Reloading = false;
		});
		return false;
	}*/

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