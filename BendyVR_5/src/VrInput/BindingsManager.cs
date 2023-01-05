using System.Collections.Generic;
using BepInEx.Configuration;
//using BendyVR_5.Settings;
using BendyVR_5.Stage;
using BendyVR_5.VrInput.ActionInputs;
using UnityEngine;
using Valve.VR;
using InControl;

namespace BendyVR_5.VrInput;

public class BindingsManager : MonoBehaviour
{
    public readonly Dictionary<string, IActionInput> ActionMap = new()
    {
        {VirtualKey.Run, ActionInputDefinitions.Run},
        {VirtualKey.Use, ActionInputDefinitions.Interact},
        {VirtualKey.Confirm, ActionInputDefinitions.Interact},
        {VirtualKey.Jump, ActionInputDefinitions.Jump},
        {VirtualKey.Cancel, ActionInputDefinitions.Cancel},
        {VirtualKey.DialogUp, ActionInputDefinitions.UiUp},
        {VirtualKey.DialogDown, ActionInputDefinitions.UiDown},
        {VirtualKey.Pause, ActionInputDefinitions.Cancel},
        {VirtualKey.NextMenu, ActionInputDefinitions.Next},
        {VirtualKey.PreviousMenu, ActionInputDefinitions.Previous},
        {VirtualKey.MoveXAxis, ActionInputDefinitions.MoveX},
        {VirtualKey.LookXAxisStick, ActionInputDefinitions.RotateX},
        {VirtualKey.MoveYAxis, ActionInputDefinitions.MoveY},
        {VirtualKey.MoveForwardKeyboard, ActionInputDefinitions.UiUp},
        {VirtualKey.MoveBackwardKeyboard, ActionInputDefinitions.UiDown},
        {VirtualKey.StrafeRightKeyboard, ActionInputDefinitions.Next},
        {VirtualKey.StrafeLeftKeyboard, ActionInputDefinitions.Previous},
        // Unused for actually controlling stuff, but used for the input prompts.
        {VirtualKey.ScrollUpDown, ActionInputDefinitions.UiUp},
        {VirtualKey.Melee, ActionInputDefinitions.Weapon}
    };

    public static BindingsManager Create(VrCore stage)
    {
        var instance = stage.gameObject.AddComponent<BindingsManager>();
        return instance;
    }

    private void Awake()
    {
        ActivateSteamVrActionSets();
    }

    private void OnEnable()
    {
        SteamVR_Events.System(EVREventType.VREvent_Input_BindingsUpdated).Listen(HandleVrBindingsUpdated);
        //VrSettings.Config.SettingChanged += HandleSettingChanged;
    }

    private void OnDisable()
    {
        SteamVR_Events.System(EVREventType.VREvent_Input_BindingsUpdated).Remove(HandleVrBindingsUpdated);
        //VrSettings.Config.SettingChanged -= HandleSettingChanged;
    }

    private void ActivateSteamVrActionSets()
    {
        foreach (var actionSet in SteamVR_Input.actionSets) actionSet.Activate();
    }

    private static void HandleSettingChanged(object sender, SettingChangedEventArgs e)
    {
        UpdatePrompts();
    }

    private static void HandleVrBindingsUpdated(VREvent_t arg0)
    {
        UpdatePrompts();
    }

    private static void UpdatePrompts()
    {
        if (!InputManager.IsSetup) return;

        // This resets the input prompts. The layout choice argument isn't actually used.
        //InputManager.Instance.SetControllerLayout(vgControllerLayoutChoice.KeyboardMouse);
    }

    public float GetValue(string virtualKey)
    {
        ActionMap.TryGetValue(virtualKey, out var actionInput);
        if (actionInput == null) return 0;

        return actionInput.AxisValue;
    }

    public bool GetUp(string virtualKey)
    {
        ActionMap.TryGetValue(virtualKey, out var actionInput);
        if (actionInput == null) return false;

        return actionInput.ButtonUp;
    }

    public bool GetDown(string virtualKey)
    {
        ActionMap.TryGetValue(virtualKey, out var actionInput);
        if (actionInput == null) return false;

        return actionInput.ButtonDown;
    }
}