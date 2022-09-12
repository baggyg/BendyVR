using System;
using BendyVR_5.LaserPointer;
using BendyVR_5.Settings;
using BendyVR_5.Stage;
using UnityEngine;

namespace BendyVR_5.Limbs;

public class VrLimbManager : MonoBehaviour
{
    public VrLaser Laser;
    private Transform playerTransform;
    public PlayerController m_playerController;
    //private vgPlayerNavigationController navigationController;
    //private ToolPicker toolPicker;
    public VrHand NonDominantHand { get; private set; }
    public VrHand DominantHand { get; private set; }
    
    public static VrLimbManager Create(VrCore stage)
    {
        var instance = new GameObject("VrLimbManager").AddComponent<VrLimbManager>();
        var instanceTransform = instance.transform;
        instanceTransform.SetParent(stage.transform, false);

        instance.DominantHand = VrHand.Create(instanceTransform);
        instance.NonDominantHand = VrHand.Create(instanceTransform, true);
        //instance.toolPicker = ToolPicker.Create(instance, instance.DominantHand);
        instance.Laser = VrLaser.Create(instance.DominantHand.transform);

        return instance;
    }

    public void SetUp(PlayerController playerController, Camera camera)
    {
        m_playerController = playerController;
        var playerTransform = playerController ? playerController.transform : null;
        //navigationController = playerController ? playerController.navController : null;
        //var skeletonRoot = GetSkeletonRoot(playerTransform);
        //var armsMaterial = GetArmsMaterial(playerTransform);
        DominantHand.SetUp(playerController);
        NonDominantHand.SetUp(playerController);
        Laser.SetUp(camera);
        UpdateHandedness();
    }

    private void Update()
    {
        UpdateHandedness();
    }

    private void OnEnable()
    {
        VrSettings.Config.SettingChanged += HandleLeftHandedModeSettingChanged;
    }

    private void OnDisable()
    {
        VrSettings.Config.SettingChanged -= HandleLeftHandedModeSettingChanged;
    }

    private VrHand GetRightHand()
    {
        return VrSettings.LeftHandedMode.Value ? NonDominantHand : DominantHand;
    }

    private VrHand GetLeftHand()
    {
        return VrSettings.LeftHandedMode.Value ? DominantHand : NonDominantHand;
    }

    public VrHand GetMovementStickHand()
    {
        return VrSettings.SwapSticks.Value ? GetRightHand() : GetLeftHand();
    }

    private void HandleLeftHandedModeSettingChanged(object sender, EventArgs e)
    {
        UpdateHandedness();
    }

    private void UpdateHandedness()
    {
        if (!playerTransform || !m_playerController) return;

        var scale = new Vector3(VrSettings.LeftHandedMode.Value && m_playerController.enabled ? -1 : 1, 1, 1);

        //See what needs doing later
        /*henryTransform.localScale = scale;

        var playerController = navigationController.playerController;

        if (playerController && playerController.inventory && playerController.inventory.heldObject)
            playerController.inventory.heldObject.transform.localScale = scale;*/
    }
    
    public void StopTrackingOriginalHands()
    {
        NonDominantHand.StopTrackingOriginalHands();
        DominantHand.StopTrackingOriginalHands();
    }

    public void StartTrackingOriginalHands()
    {
        NonDominantHand.StartTrackingOriginalHands();
        DominantHand.StartTrackingOriginalHands();
    }
}