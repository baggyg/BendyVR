using BendyVR_5.Assets;
using BendyVR_5.Helpers;
using BendyVR_5.LaserPointer;
using BendyVR_5.Settings;
using BendyVR_5.Stage;
using BendyVR_5.VrCamera;
using BendyVR_5.VrInput.ActionInputs;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMG.Controls;
using UnityEngine;

namespace BendyVR_5.Player;

internal class VRPlayerController : MonoBehaviour
{
    private PlayerController mPlayerController;
    private VrCore coreVR;
	private Transform mDominantHand;
	private Transform mNonDominantHand;
	public VrLaser mLaser;
	bool enabled = false;
	bool hasAxe = false;
	
	private Vector3 velocity;
	private Vector3 lastPos;

	private Quaternion lastRotation;
	public float velocityVectorLength = 0f;
	public float angularVelocityVectorLength = 0f;

	//Snap Turning Things
	private const float smoothRotationBaseSpeed = 50f;
	private bool isSnapTurning = false;

	public static VRPlayerController Create(VrCore vrcore)
    {
        var instance = vrcore.gameObject.AddComponent<VRPlayerController>();
        instance.coreVR = vrcore;
		return instance;
    }
	
	public void SetUp(PlayerController _playerController)
	{
		mPlayerController = _playerController;

		//Add the prefabs to the hands
		mNonDominantHand = Instantiate(VrAssetLoader.LeftHandPrefab).transform;
		mNonDominantHand.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		LayerHelper.SetLayerRecursive(transform.gameObject, GameLayer.VrHands);
		mNonDominantHand.name = "NonDominantVRHand";
		mNonDominantHand.parent = mPlayerController.m_HandContainer;

		mDominantHand = Instantiate(VrAssetLoader.RightHandPrefab).transform;
		mDominantHand.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		LayerHelper.SetLayerRecursive(transform.gameObject, GameLayer.VrHands);
		mDominantHand.name = "DominantVRHand";
		mDominantHand.parent = mPlayerController.m_HandContainer;
		mLaser = VrLaser.Create(mDominantHand.transform);

		//Move the hand below the tracked hand and scale correctly
		Transform hand = mPlayerController.m_HandContainer.Find("Hand");
		hand.parent = mDominantHand;
		hand.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        hand.eulerAngles = new Vector3(90f, 0.0f, 0.0f);
		hand.localScale = new Vector3(0.8f, 0.8f, 0.8f);

		//Removed this as I think I meant Hand, not handcontainer
		//mPlayerController.m_HandContainer.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

		enabled = true;
	}

	public void SetupAxe()
    {
		Logs.WriteInfo("SetupAxe");

		//Turn off animator on hand
		Transform hand = mDominantHand.Find("Hand");
		if (hand == null)
		{
			Logs.WriteError("Hand is Null");
			return;
		}
		Animator _anim = hand.gameObject.GetComponent<Animator>();
		if(_anim == null)
        {
			Logs.WriteError("Animator is Null");
			return;
		}
		_anim.enabled = false;

		//Set Up Axe Local Position
		Transform axe = hand.Find("WeaponAnimator/Weapon_Axe");
		axe.localPosition = new Vector3(0f, -.2f, 0f);

		//Turn Off Glove
		TurnOffDominantHand();
		hasAxe = true;
	}

	public void TurnOffDominantHand()
    {
		Transform gloveRenderMesh = mDominantHand.Find("vr_glove_model/renderMesh0");
		gloveRenderMesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
	}

	private void Update()
	{
		if (!enabled)
			return;

		if (GameManager.Instance.isPaused)
		{
			mPlayerController.m_IsPaused = true;
		}
		else if (mPlayerController.m_IsPaused)
		{
			DOTween.Sequence().InsertCallback(0.1f, delegate
			{
				mPlayerController.m_IsPaused = false;
			});
		}
		else if (!GameManager.Instance.isPaused)
		{
			mPlayerController.m_PlayerLook.GetInput();
			if (mPlayerController.m_CharacterController.isGrounded && !mPlayerController.m_JumpInput && mPlayerController.m_EnableJump && mPlayerController.canJump && !mPlayerController.isLocked && !mPlayerController.isMoveLocked)
			{
				mPlayerController.m_JumpInput = PlayerInput.Jump();
			}
						
			//Intereaction Based on where laser is....
			mPlayerController.m_Interaction.UpdateInteraction(mLaser.transform.position, mLaser.transform.forward);			
			
			if (mPlayerController.m_CanHaveSeeingTool && mPlayerController.m_CharacterController.isGrounded && mPlayerController.CurrentStatus != CombatStatus.Hiding && !mPlayerController.isLocked && mPlayerController.m_IsSeeingToolEnabled && PlayerInput.SeeingTool())
			{
				mPlayerController.isSeeingToolActive = !mPlayerController.isSeeingToolActive;
				mPlayerController.UseSeeingTool(mPlayerController.isSeeingToolActive);
			}

			if (hasAxe)
			{
				//Velocity Delta
				velocity = (mDominantHand.localPosition - lastPos) / Time.deltaTime;
				lastPos = mDominantHand.localPosition;
				velocityVectorLength = velocity.sqrMagnitude;

				//Angular Velocity
				Quaternion deltaRotation = mDominantHand.localRotation * Quaternion.Inverse(lastRotation);
				lastRotation = mDominantHand.localRotation;
				deltaRotation.ToAngleAxis(out var angle, out var axis);
				angle *= Mathf.Deg2Rad;
				Vector3 angularVelocity = (1.0f / Time.deltaTime) * angle * axis;
				angularVelocityVectorLength = angularVelocity.magnitude;				
			}
		}
	}

	private void FixedUpdate()
	{
		if (!enabled)
			return;


		if (GameManager.Instance.isPaused)
		{
			mPlayerController.m_IsPaused = true;
			return;
		}
		if (mPlayerController.m_IsPaused)
		{
			DOTween.Sequence().InsertCallback(0.1f, delegate
			{
				mPlayerController.m_IsPaused = false;
			});
			return;
		}

		float speed = 0f;
		float num = 10f;

		//Getting Input just gets the joystick values which are already fine. 
		mPlayerController.GetInput(out speed);
		
		if (mPlayerController.m_IsRunning)
		{
			//Think this should be a bit faster.
			speed *= 1.1f;
		}
		mPlayerController.CurrentSpeed = speed;

		

		//Check Grounding is also fine
		mPlayerController.CheckGrounding();
		if (mPlayerController.m_CharacterController.isGrounded)
		{
			num = 60f;
		}

		if (!mPlayerController.isMoveLocked)
		{
			//TODO Instead of moving using this method (which assumes (PlayerController)base.transform.forward), lets use our VRPlayerController Move Function
			//As we already have the current speed

			Move(mPlayerController.CurrentSpeed);
			if (mPlayerController.m_ExternalForce.magnitude > 0f)
			{
				if (mPlayerController.m_CharacterController.enabled)
				{
					mPlayerController.m_CharacterController.Move(mPlayerController.m_ExternalForce * Time.fixedDeltaTime);
				}
				mPlayerController.m_ExternalForce = Vector3.MoveTowards(mPlayerController.m_ExternalForce, Vector3.zero, 3f * Time.fixedDeltaTime * num);
			}
		}
		/*if (isSeeingToolActive)
		{
			m_HeadBob.UpdateCameraPosition(0f, 0f);
		}*/
		//mPlayerController.GetRotations();
		turnController();
		//PlayerLookRotations(mPlayerController.transform, mPlayerController.m_HeadContainer, mPlayerController.m_HandContainer);
		mPlayerController.m_PlayerLook.UpdateCursorLock();

		
	}

	private void turnController()
    {
		if (VrSettings.SnapTurning.Value)
			UpdateSnapTurning();
		else
			UpdateSmoothTurning();
	}

	private void UpdateSnapTurning()
	{
		if (!isSnapTurning && ActionInputDefinitions.SnapTurnLeft.ButtonDown)
		{
			isSnapTurning = true;
			coreVR.FadeToBlack();
			Invoke(nameof(SnapTurnLeft), FadeOverlay.Duration);
		}

		if (!isSnapTurning && ActionInputDefinitions.SnapTurnRight.ButtonDown)
		{
			isSnapTurning = true;
			coreVR.FadeToBlack();
			Invoke(nameof(SnapTurnRight), FadeOverlay.Duration);
		}
	}

	private void UpdateSmoothTurning()
	{
		mPlayerController.transform.Rotate(
			Vector3.up,
			ActionInputDefinitions.RotateX.AxisValue * smoothRotationBaseSpeed *
			(int)VrSettings.SmoothRotationSpeed.Value *
			Time.unscaledDeltaTime);
	}

	private void SnapTurnLeft()
	{
		SnapTurn(-(int)VrSettings.SnapTurnAngle.Value);
	}

	private void SnapTurnRight()
	{
		SnapTurn((int)VrSettings.SnapTurnAngle.Value);
	}

	private void SnapTurn(float angle)
	{
		Logs.WriteInfo("Snap Turning " + angle.ToString());
		mPlayerController.transform.Rotate(Vector3.up, angle);
		Invoke(nameof(EndSnap), FadeOverlay.Duration);
	}

	private void EndSnap()
	{
		coreVR.FadeToClear();
		isSnapTurning = false;
	}
		

	private void Move(float speed)
	{
		//TODO Change to be the dominant hand if that setting is set
		Vector3 vector;
		if (VrSettings.ControllerBasedMovementDirection.Value == true)
			vector = mNonDominantHand.forward * mPlayerController.m_Input.y + mPlayerController.transform.right * mPlayerController.m_Input.x;
		else
			vector = GameManager.Instance.GameCamera.transform.forward * mPlayerController.m_Input.y + mPlayerController.transform.right * mPlayerController.m_Input.x;

		//vector = mPlayerController.transform.forward * mPlayerController.m_Input.y + mPlayerController.transform.right * mPlayerController.m_Input.x;
		float num = Vector3.Angle(Vector3.up, mPlayerController.m_GroundNormal);
		bool flag = num < mPlayerController.m_CharacterController.slopeLimit || num > 85f;
		
		mPlayerController.m_MoveDir.x = vector.x * speed;
		mPlayerController.m_MoveDir.z = vector.z * speed;
		
		//Logs.WriteInfo(mPlayerController.m_MoveDir.x + " " + mPlayerController.m_MoveDir.z);
		if (!flag)
		{
			mPlayerController.m_MoveDir.x += (1f - mPlayerController.m_GroundNormal.y) * mPlayerController.m_GroundNormal.x * (speed / 2f);
			mPlayerController.m_MoveDir.z += (1f - mPlayerController.m_GroundNormal.y) * mPlayerController.m_GroundNormal.z * (speed / 2f);
		}
		mPlayerController.m_GroundNormal = Vector3.up;
		if (mPlayerController.m_CharacterController.isGrounded)
		{
			if (mPlayerController.m_JumpInput && flag)
			{
				mPlayerController.m_GravityPower = mPlayerController.m_JumpSpeed;
				mPlayerController.m_PlayerFootsteps.PlayJumpAudio();
			}
			mPlayerController.m_JumpInput = false;
		}
		else if (mPlayerController.m_EnableGravity)
		{
			mPlayerController.m_GravityPower += Physics.gravity.y * mPlayerController.m_GravityMultiplier * Time.fixedDeltaTime;
		}
		if (mPlayerController.m_CharacterController.enabled)
		{
			mPlayerController.m_CharacterController.Move(mPlayerController.m_MoveDir * Time.fixedDeltaTime + Vector3.up * mPlayerController.m_GravityPower);
		}
		if (!mPlayerController.m_CharacterController.isGrounded)
		{
			return;
		}
		float magnitude = mPlayerController.m_CharacterController.velocity.magnitude;
		FootstepTypes footstepType = FootstepTypes.WOOD;
		
		//Not sure this is right. It used to be base but I think that will be the VRCore or something
		Vector3 position = mPlayerController.transform.position;
		position.y += mPlayerController.m_CharacterController.bounds.extents.y - 0.1f;
		if (Physics.Raycast(position, Vector3.down, out var hitInfo, mPlayerController.m_CharacterController.height + 1f, ~(1 << LayerMask.NameToLayer("Player"))))
		{
			if (hitInfo.collider.CompareTag("Ink"))
			{
				footstepType = FootstepTypes.INK;
			}
			else if (hitInfo.collider.CompareTag("DeepInk"))
			{
				footstepType = FootstepTypes.INK_DEEP;
			}
			else if (hitInfo.collider.CompareTag("Stairs"))
			{
				footstepType = FootstepTypes.WOOD_STAIRS;
			}
			else if (hitInfo.collider.CompareTag("StairsInk"))
			{
				footstepType = FootstepTypes.INK_STAIRS;
			}
			else if (hitInfo.collider.CompareTag("Vent"))
			{
				footstepType = FootstepTypes.VENT;
			}
			else if (hitInfo.collider.CompareTag("Dirt"))
			{
				footstepType = FootstepTypes.DIRT;
			}
			else if (hitInfo.collider.CompareTag("Metal"))
			{
				footstepType = FootstepTypes.METAL;
			}
			else if (hitInfo.collider.CompareTag("Tile"))
			{
				footstepType = FootstepTypes.TILE;
			}
		}
		mPlayerController.m_PlayerFootsteps.SetFootstepType(footstepType);
		mPlayerController.m_PlayerFootsteps.ProgressStepCycle(magnitude, speed);
		//mPlayerController.m_HeadBob.UpdateCameraPosition(magnitude, speed);
		//mPlayerController.m_CameraFOV.UpdateVOD(magnitude, speed, m_IsRunning);
	}

	/*private Transform GetMovementDirectionTransform()
	{
		return VrSettings.ControllerBasedMovementDirection.Value
			? stage.GetMovementStickHand()
			: cameraDirectionTransform;
	}*/

	
}
