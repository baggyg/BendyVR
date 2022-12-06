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
using UnityEngine.XR;

namespace BendyVR_5.Player;

internal class VRPlayerController : MonoBehaviour
{
	private VrCore coreVR;

	public PlayerController mPlayerController;
    
	public Transform mDominantHand;
	public Transform mNonDominantHand;

	public Transform mNewCameraParent;
	private FakeParenting mFollowCameraParent;
	public Transform mNewHandParent;
	private FakeParenting mFollowHandParent;
	Vector3 oldPosition = Vector3.zero;
	float oldRotation = 0f;
	
	bool mInitialHeightSet = false;
	float mInitialHeight = 0f;
	float mInitialHeightOffset = 0f;


	public VrLaser mLaser;
	bool enabled = false;
	bool hasMeleeWeapon = false;
	bool hasGunWeapon = false;
	public bool inFreeRoam = false;

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

		//Turn down the radius of character controller so wall pushback isn't such a problem
		CharacterController cc = mPlayerController.GetComponent<CharacterController>();
		//Some falling through levels. Lets keep it as is and see if people complain
		//cc.radius = 0.3f;

		//Set up a new camera parent
		mNewCameraParent = new GameObject("RoomScaleDamper").transform;
		//Set it to be below the head container
		mNewCameraParent.localPosition = Vector3.zero;
		mNewCameraParent.localRotation = Quaternion.identity;
		mNewCameraParent.SetParent(mPlayerController.HeadContainer, false);

		//Set up a new hand parent
		mNewHandParent = new GameObject("RoomScaleDamperHand").transform;
		//Set it to be below the head container
		mNewHandParent.localPosition = Vector3.zero;
		mNewHandParent.localRotation = Quaternion.identity;
		mNewHandParent.SetParent(mPlayerController.m_HandContainer, false);

		//Move the Audio Collider outside of the main camera
		Transform audioCollider = GameManager.Instance.GameCamera.transform.Find("AudioCollider");
		audioCollider.localPosition = Vector3.zero;
		audioCollider.localRotation = Quaternion.identity;
		audioCollider.SetParent(mPlayerController.HeadContainer, false);

		//Move the Forward Camera Target below the main camera (so is rendered is properly done)
		Transform forwardCamera = mPlayerController.HeadContainer.Find("Forward Camera Target");
		forwardCamera.localPosition = Vector3.zero;
		forwardCamera.localRotation = Quaternion.identity;
		forwardCamera.SetParent(GameManager.Instance.GameCamera.transform, false);


		//RoomScale Mid Tier Dampening
		SetupCameraHierarchy();
		SetupCameraFollow();
		

		//TODO This assume dominant is always right
		//Add the prefabs to the hands
		if (VrSettings.LeftHandedMode.Value == true)
		{
			mNonDominantHand = Instantiate(VrAssetLoader.RightHandPrefab).transform;
			mDominantHand = Instantiate(VrAssetLoader.LeftHandPrefab).transform;
		}
		else
        {
			mNonDominantHand = Instantiate(VrAssetLoader.LeftHandPrefab).transform;
			mDominantHand = Instantiate(VrAssetLoader.RightHandPrefab).transform;
		}
		mNonDominantHand.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		LayerHelper.SetLayerRecursive(transform.gameObject, GameLayer.VrHands);
		mNonDominantHand.name = "NonDominantVRHand";
		
		mDominantHand.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		LayerHelper.SetLayerRecursive(transform.gameObject, GameLayer.VrHands);
		mDominantHand.name = "DominantVRHand";

		ParentHands();


		mLaser = VrLaser.Create(mNonDominantHand.transform);


		//Move the hand below the tracked hand and scale correctly
		Transform hand = mPlayerController.m_HandContainer.Find("Hand");
		hand.parent = mDominantHand;
		hand.localPosition = Vector3.zero;
        hand.eulerAngles = Vector3.zero;
		hand.localScale = new Vector3(0.8f, 0.8f, 0.8f);

		//GB This didn't quite work as the HeadContainer was well off from the rest, which is used for events etc
		//RoomScale Mid Tier Dampening
		/*mRoomScaleDamper = new GameObject("RoomScaleDamper").transform;
		mRoomScaleDamper.localPosition = Vector3.zero;
		mRoomScaleDamper.localRotation = Quaternion.identity;
		mRoomScaleDamper.SetParent(mPlayerController.transform, false);
		mPlayerController.m_HandContainer.SetParent(mRoomScaleDamper);
		mPlayerController.m_HeadContainer.SetParent(mRoomScaleDamper);*/

		enabled = true;
	}

	public void ParentHands()
    {
		mNonDominantHand.parent = mNewHandParent;
		mDominantHand.parent = mNewHandParent;
	}

	//Generally this needs to happen when something else has taken control
	public void SetupCameraHierarchy()
	{
		//Set Camera Container to be below new VrCameraParent
		mPlayerController.CameraParent.SetParent(mNewCameraParent);

		//Set the Camera to also be below VrCameraParent - GB Why??? This screws up Miracle Stations
		GameManager.Instance.GameCamera.transform.SetParent(mNewCameraParent);
	}

	public void SetupCameraFollow()
	{
		//Move the Camera Container to match the VR Camera (which is auto tracked)
		mFollowCameraParent = FakeParenting.Create(mPlayerController.CameraParent, GameManager.Instance.GameCamera.transform, FakeParenting.UpdateType.LateUpdate);
		mFollowHandParent = FakeParenting.Create(mNewHandParent, mNewCameraParent, FakeParenting.UpdateType.LateUpdate);
	}

	public void RemoveCameraFollow()
	{
		//Move the Camera Container to match the VR Camera (which is auto tracked)
		UnityEngine.Object.Destroy(mFollowCameraParent);
		UnityEngine.Object.Destroy(mFollowHandParent);
	}

	public void SetupGunWeapon(string weapon_name)
	{
		Logs.WriteInfo("SetupGunWeapon");

		//Turn off animator on hand
		Transform hand = mDominantHand.Find("Hand");
		if (hand == null)
		{
			Logs.WriteError("Hand is Null");
			return;
		}
		Animator _anim = hand.gameObject.GetComponent<Animator>();
		if (_anim == null)
		{
			Logs.WriteError("Animator is Null");
			return;
		}
		_anim.enabled = false;

		//Set Up Local Position
		Transform animator = hand.Find("WeaponAnimator");
		animator.localPosition = Vector3.zero;
		animator.localRotation = Quaternion.identity;

		Transform weapon = animator.Find(weapon_name);
		weapon.localPosition = new Vector3(0f, -0.2f, -0.1f);
		weapon.localEulerAngles = new Vector3(350f, 350f, 0f);		

		//Loop through all children (apart from interaction)
		for (int i = 0; i < weapon.childCount; i++)
		{
			Transform child = weapon.GetChild(i);
			if (!child.name.ToLower().Equals("interaction"))
			{
				child.localPosition = Vector3.zero;
				child.localRotation = Quaternion.identity;
			}
		}

		//Move the Bullets / Sparks Etc
		Transform sparks = weapon.Find("Sparks_TriggerOnly");
		sparks.localPosition = new Vector3(0f,-2.2f,0.2f);
		sparks.localEulerAngles = new Vector3(90f, 0f, 0f);

		//Move the Bullets / Sparks Etc
		Transform bullets = weapon.Find("InkBullet");
		bullets.localPosition = new Vector3(0f, -2.2f, 0.2f);
		bullets.localEulerAngles = new Vector3(90f, 0f, 0f);

		//Turn Off Glove
		TurnOffDominantHand();
		hasGunWeapon = true;
	}

	public void SetupMeleeWeapon(string weapon_name)
    {
		Logs.WriteInfo("SetupMeleeWeapon");

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
		Transform animator = hand.Find("WeaponAnimator");
		animator.localPosition = Vector3.zero;
		animator.localRotation = Quaternion.identity;
		
		Transform weapon = animator.Find(weapon_name);
		weapon.localPosition = Vector3.zero;
		weapon.localEulerAngles = new Vector3(0f, 350f, 0f);
		if(weapon_name.ToLower().Equals("weapon_gent") ||
			weapon_name.ToLower().Equals("weapon_gent(clone)"))
        {
			weapon.localPosition = new Vector3(0f, 0f, -0.2f);
		}
		else if (weapon_name.ToLower().Equals("prop_flashlight") ||
			weapon_name.ToLower().Equals("prop_flashlight_vent"))
		{
			weapon.localPosition = new Vector3(0f, -0.2f, -0.15f);
		}
		else if (weapon_name.ToLower().Equals("weapon_inktool") ||
			weapon_name.ToLower().Equals("weapon_inktool(clone)"))
		{
			weapon.localEulerAngles = new Vector3(270f, 350f, 0f);
		}
		else if (weapon_name.ToLower().Equals("weapon_plunger") ||
			weapon_name.ToLower().Equals("weapon_plunger(clone)"))
		{
			weapon.localPosition = new Vector3(0f, 0f, -0.2f);
		}
		else if (weapon_name.ToLower().Equals("carrybowl") ||
			weapon_name.ToLower().Equals("carrybowl(clone)"))
		{
			weapon.localEulerAngles = new Vector3(270f, 350f, 0f);
			weapon.localPosition = new Vector3(0f, 0f, -0.1f);
		}
		else if (weapon_name.ToLower().Equals("holdable_inkblob") ||
			weapon_name.ToLower().Equals("holdable_inkblob(clone)"))
		{
			weapon.localEulerAngles = new Vector3(90f, 350f, 0f);
			weapon.localScale = new Vector3(0.33f, 0.33f, 0.33f);
			weapon.localPosition = new Vector3(0f, 0f, -0.2f);
		}

		//Loop through all children (apart from interaction)
		for (int i = 0; i < weapon.childCount; i++)
		{
			Transform child = weapon.GetChild(i);
			if(!child.name.ToLower().Equals("interaction"))
            {				
				child.localPosition = Vector3.zero;
				child.localRotation = Quaternion.identity;
			}
		}

		if (weapon_name.ToLower().Equals("prop_flashlight") ||
			weapon_name.ToLower().Equals("prop_flashlight_vent"))
		{
			Transform spotlight = weapon.Find("SpotLight");
			spotlight.localEulerAngles = new Vector3(85f, 0f, 0f);
			spotlight.localPosition = new Vector3(0f, 0f, 1f);
		}

		//Transform axe = weapon.Find("Axe");
		//axe.localPosition = new Vector3(0f, -.2f, 0f);


		//Turn Off Glove
		TurnOffDominantHand();
		hasMeleeWeapon = true;
	}

	public void LoseMelee()
	{
		Logs.WriteInfo("LoseMelee");

		//Turn On Glove
		TurnOnDominantHand();
		hasMeleeWeapon = false;
	}

	public void TurnOffBothHands()
	{
		TurnOffDominantHand();
		TurnOffNonDominantHand();
	}

	public void TurnOnBothHands()
	{
		TurnOnDominantHand();
		TurnOnNonDominantHand();
	}

	public void TurnOffDominantHand()
    {
		Transform gloveRenderMesh = mDominantHand.Find("vr_glove_model/renderMesh0");
		gloveRenderMesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
	}

	public void TurnOffNonDominantHand()
	{
		Transform gloveRenderMesh = mNonDominantHand.Find("vr_glove_model/renderMesh0");
		gloveRenderMesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
	}

	public void TurnOnDominantHand()
	{
		Transform gloveRenderMesh = mDominantHand.Find("vr_glove_model/renderMesh0");
		gloveRenderMesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
	}

	public void TurnOnNonDominantHand()
	{
		Transform gloveRenderMesh = mNonDominantHand.Find("vr_glove_model/renderMesh0");
		gloveRenderMesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
	}

	List<XRNodeState> nodeStatesCache = new List<XRNodeState>();
	bool TryGetCenterEyeNodeStateRotation(out Quaternion rotation)
	{
		InputTracking.GetNodeStates(nodeStatesCache);
		for (int i = 0; i < nodeStatesCache.Count; i++)
		{
			XRNodeState nodeState = nodeStatesCache[i];
			if (nodeState.nodeType == XRNode.CenterEye)
			{
				if (nodeState.TryGetRotation(out rotation))
					return true;
			}
		}
		// This is the fail case, where there was no center eye was available.
		rotation = Quaternion.identity;
		return false;
	}
	bool TryGetCenterEyeNodeStatePosition(out Vector3 position)
	{
		InputTracking.GetNodeStates(nodeStatesCache);
		for (int i = 0; i < nodeStatesCache.Count; i++)
		{
			XRNodeState nodeState = nodeStatesCache[i];
			if (nodeState.nodeType == XRNode.CenterEye)
			{
				if (nodeState.TryGetPosition(out position))
					return true;
			}
		}
		// This is the fail case, where there was no center eye was available.
		position = new Vector3();
		return false;
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
			//Jump
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

			//Velocity Based Attacks
			if (hasMeleeWeapon)
			{
				//Velocity Delta
				velocity = (mDominantHand.localPosition - lastPos) / Time.deltaTime;
				lastPos = mDominantHand.localPosition;
				velocityVectorLength = velocity.sqrMagnitude;

				//Angular Velocity
				Quaternion deltaAxeRotation = mDominantHand.localRotation * Quaternion.Inverse(lastRotation);
				lastRotation = mDominantHand.localRotation;
				deltaAxeRotation.ToAngleAxis(out var angle, out var axis);
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
					Logs.WriteWarning("External Force Applied");
					//Experimentally remove force from player (to stop falling through walls etc)
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

	private void AdjustRoomDampener()
    {
		//Move Room Dampener (Defi X and Z)
		Vector3 newPosition = GameManager.Instance.GameCamera.transform.localPosition;
		Vector3 deltaPosition = newPosition - oldPosition;
		float newRotation = GameManager.Instance.GameCamera.transform.localRotation.eulerAngles.y;
		float deltaRotation = newRotation - oldRotation;

		if (!mPlayerController.isMoveLocked)
		{
			//float scale = VrSettings.WorldScale.Value;
			//float offset = (-1.5f * scale) + 0.7f;
			float offset = VrSettings.HeightOffset.Value;

			if ((!mInitialHeightSet && newPosition.y != 0f))
			{
				mInitialHeight = (-newPosition.y) + offset;// / VrSettings.WorldScale.Value;
				mInitialHeightOffset = offset;
				mInitialHeightSet = true;
				Logs.WriteWarning("mInitialHeight = " + mInitialHeight);
				Logs.WriteWarning("mInitialHeightOffset = " + mInitialHeightOffset);
			}

			if (offset != mInitialHeightOffset)
			{
				mInitialHeight = (mInitialHeight - mInitialHeightOffset) + offset;// / VrSettings.WorldScale.Value;
				mInitialHeightOffset = offset;
				Logs.WriteWarning("mInitialHeight = " + mInitialHeight);
				Logs.WriteWarning("mInitialHeightOffset = " + mInitialHeightOffset);
			}

			//Added 0.2 offset
			mNewCameraParent.localPosition = new Vector3(-newPosition.x, (mInitialHeight + 0.2f), -newPosition.z) * VrSettings.WorldScale.Value;

			//mRoomScaleDamper.transform.localPosition = new Vector3(-newPosition.x, 0f, -newPosition.z) * VrSettings.WorldScale.Value;
			//if (!oldPosition.Equals(Vector3.zero))
			if (!deltaPosition.Equals(Vector3.zero))
			{
				//TODO - see if moving the playercontrollers rotation helps with anything. 
				//mPlayerController.transform.eulerAngles = new Vector3(mPlayerController.transform.eulerAngles.x, mPlayerController.transform.eulerAngles.y + deltaRotation, mPlayerController.transform.eulerAngles.z);

				//Vector3 movePlayer = new Vector3(mPlayerController.transform.localPosition.x - deltaPosition.x, mPlayerController.transform.localPosition.y, mPlayerController.transform.localPosition.z - deltaPosition.z);
				Vector3 movePlayer = new Vector3(-deltaPosition.x, 0f, -deltaPosition.z) * VrSettings.WorldScale.Value;

				//Don't do this now I am realtive to playercontroller
				movePlayer = mPlayerController.transform.rotation * movePlayer;

				mPlayerController.m_CharacterController.Move(-movePlayer);
				//mPlayerController.transform.localPosition = new Vector3(mPlayerController.transform.localPosition.x - movePlayer.x, mPlayerController.transform.localPosition.y, mPlayerController.transform.localPosition.z - movePlayer.z); ;
			}
		}

		Vector3 camDiff = mPlayerController.transform.position - GameManager.Instance.GameCamera.transform.position;
		Vector3 headDiff = mPlayerController.HeadContainer.transform.position - GameManager.Instance.GameCamera.transform.position;

		if (!inFreeRoam)
		{
			if (Mathf.Abs(camDiff.x) > 0.01 || Mathf.Abs(camDiff.z) > 0.01 || Mathf.Abs(headDiff.x) > 0.01 || Mathf.Abs(headDiff.z) > 0.01)
				Logs.WriteInfo("PC -> Cam : " + camDiff.x + " " + camDiff.z + "Head -> Cam : " + headDiff.x + " " + headDiff.z);
		}

		oldPosition = newPosition;
		oldRotation = newRotation;
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
		//Changed to Hand / head Controller to match original game
		/*mPlayerController.m_HeadContainer.Rotate(
			Vector3.up,
			ActionInputDefinitions.RotateX.AxisValue * smoothRotationBaseSpeed *
			(int)VrSettings.SmoothRotationSpeed.Value *
			Time.unscaledDeltaTime);
		mPlayerController.m_HandContainer.Rotate(
			Vector3.up,
			ActionInputDefinitions.RotateX.AxisValue * smoothRotationBaseSpeed *
			(int)VrSettings.SmoothRotationSpeed.Value *
			Time.unscaledDeltaTime);*/


		mPlayerController.transform.Rotate(
			Vector3.up,
			ActionInputDefinitions.RotateX.AxisValue * smoothRotationBaseSpeed *
			(int)VrSettings.SmoothRotationSpeed.Value *
			Time.unscaledDeltaTime);
		/*mPlayerController.transform.RotateAround(
			GameManager.Instance.GameCamera.transform.position, 
			Vector3.up,
			ActionInputDefinitions.RotateX.AxisValue * smoothRotationBaseSpeed * 
			(int)VrSettings.SmoothRotationSpeed.Value *
			Time.unscaledDeltaTime);*/
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
		//Changed to Hand / head Controller to match original game
		//Logs.WriteInfo("Snap Turning " + angle.ToString());
		mPlayerController.transform.Rotate(Vector3.up, angle);
		//mPlayerController.HeadContainer.Rotate(Vector3.up, angle);
		//mPlayerController.m_HandContainer.Rotate(Vector3.up, angle);
		//mPlayerController.transform.RotateAround(GameManager.Instance.GameCamera.transform.position, Vector3.up, angle);
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
			vector = mNonDominantHand.forward * mPlayerController.m_Input.y + mNonDominantHand.right * mPlayerController.m_Input.x;
		else
			vector = GameManager.Instance.GameCamera.transform.forward * mPlayerController.m_Input.y + GameManager.Instance.GameCamera.transform.right * mPlayerController.m_Input.x;

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

			//Move room dampenener here.
			AdjustRoomDampener();
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
