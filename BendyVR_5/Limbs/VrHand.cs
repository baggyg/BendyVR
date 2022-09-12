using System;
using BendyVR_5.Assets;
using BendyVR_5.Helpers;
using BendyVR_5.Settings;
using UnityEngine;
using Valve.VR;

namespace BendyVR_5.Limbs;

public class VrHand : MonoBehaviour
{
    private FakeParenting handRootFakeParenting;
    private bool isDominant;
    public PlayerController m_playerController;

    public static VrHand Create(Transform parent, bool isNonDominant = false)
    {
        var transform = Instantiate(isNonDominant ? VrAssetLoader.LeftHandPrefab : VrAssetLoader.RightHandPrefab,
            parent,
            false).transform;
        
        LayerHelper.SetLayerRecursive(transform.gameObject, GameLayer.VrHands);
        transform.name = $"{(isNonDominant ? "Dominand" : "NonDominant")}VrHand";
        var instance = transform.gameObject.AddComponent<VrHand>();
        instance.isDominant = isNonDominant;

        return instance;
    }

    public void SetUp(PlayerController playerController)
    {
        // Need to deactive and reactivate the object to make SteamVR_Behaviour_Pose work properly.
        m_playerController = playerController;
        gameObject.SetActive(false);
        /*if (armsMaterial)
        {
            var material = GetComponentInChildren<SkinnedMeshRenderer>().material;
            material.shader = armsMaterial.shader;
            material.CopyPropertiesFromMaterial(armsMaterial);
        }*/

        //rootBone = playerRootBone;

        SetUpSettings();
        AttachOriginalHand();
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        VrSettings.Config.SettingChanged += HandleSettingChanged;
    }

    private void OnDisable()
    {
        VrSettings.Config.SettingChanged -= HandleSettingChanged;
    }

    private void HandleSettingChanged(object sender, EventArgs e)
    {
        SetUpSettings();
    }

    private void SetUpSettings()
    {
        SetUpHandedness();
    }

    private void SetUpHandedness()
    {
        var isLeft = VrSettings.LeftHandedMode.Value ? !isDominant : isDominant;

        // In Firewatch, Henry is right-handed. He holds the radio with his left hand,
        // but he picks up and throws objects with his right hand. To allow for left handedness in VR, we need to
        // swap the original hands. We can achieve this by targetting different hand and arm bones.
        // So, when playing in left handed mode, the right hand will have armBoneName = "Left",
        // because the player is controlling Henry's left hand with their right VR controller.
        var armBoneName = isDominant ? "Left" : "Right";
        transform.localScale = new Vector3(VrSettings.LeftHandedMode.Value ? -1 : 1, 1, 1);
        //EnableAnimatedHand(armBoneName);
        SetUpPose(isLeft);
    }

    private void SetUpPose(bool isLeft)
    {
        var pose = gameObject.GetComponent<SteamVR_Behaviour_Pose>();
        if (isLeft)
        {
            pose.inputSource = SteamVR_Input_Sources.LeftHand;
            pose.poseAction = SteamVR_Actions.default_PoseLeftHand;
        }
        else
        {
            pose.inputSource = SteamVR_Input_Sources.RightHand;
            pose.poseAction = SteamVR_Actions.default_PoseRightHand;
        }
    }

    private void AttachOriginalHand()
    {
        if (!isDominant)
            return;
                
        handRootFakeParenting = FakeParenting.Create(m_playerController.m_HandContainer, transform, FakeParenting.UpdateType.LateUpdate | FakeParenting.UpdateType.VeryLateUpdate);
        
        // Clone hand bones will follow the original bones, to mimick the same animations.
        //CopyLocalTransformValues.Create(cloneChild.gameObject, targetChild);
    }

    public void StopTrackingOriginalHands()
    {
        if (!handRootFakeParenting) return;

        handRootFakeParenting.enabled = false;
    }

    public void StartTrackingOriginalHands()
    {
        if (!handRootFakeParenting) return;

        handRootFakeParenting.enabled = true;
    }
}