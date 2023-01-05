using BendyVR_5.Helpers;
using BendyVR_5.src;
using BendyVR_5.VrInput.ActionInputs;
using HarmonyLib;
using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Valve.VR;

namespace BendyVR_5.VrInput.Patches;

[HarmonyPatch]
public class InputManagerPatches : BendyVRPatch
{
	/*[HarmonyPrefix]
	[HarmonyPatch(typeof(InputManager), nameof(InputManager.SetupInternal))]
	internal static bool CustomSetupInternal(ref bool __result)
	{
		if (InputManager.IsSetup)
		{
			return false;
		}
		InputManager.Platform = Utility.GetWindowsVersion().ToUpper();
		InputManager.enabled = true;
		InputManager.initialTime = 0f;
		InputManager.currentTime = 0f;
		InputManager.lastUpdateTime = 0f;
		InputManager.currentTick = 0uL;
		InputManager.applicationIsFocused = true;
		InputManager.deviceManagers.Clear();
		InputManager.deviceManagerTable.Clear();
		InputManager.devices.Clear();
		InputManager.Devices = InputManager.devices.AsReadOnly();
		InputManager.activeDevice = InputDevice.Null;
		InputManager.activeDevices.Clear();
		InputManager.ActiveDevices = InputManager.activeDevices.AsReadOnly();
		InputManager.playerActionSets.Clear();
		InputManager.IsSetup = true;
		bool flag = true;
		if (InputManager.OnSetup != null)
		{
			InputManager.OnSetup();
			InputManager.OnSetup = null;
		}
		if (flag)
		{
			AddDeviceManager<UnityInputDeviceManager>();
		}
		
		__result = true;
		return __result;
	}*/

	//In VR we need GameManager->HasController to report true, but this is an inline function so can't override it. 
	//Therefore we will add a dummy device
	[HarmonyPrefix]
	[HarmonyPatch(typeof(InputManager), nameof(InputManager.UpdateDevices))]
	private static bool AddDummyDevice()
	{
		if(InputManager.Devices.Count == 0)
        {
			InputDevice inputDevice = new InputDevice("VRDummy", true);
			InputManager.devices.Add(inputDevice);
		}
		return false;
	}

	


		[HarmonyPrefix]
	[HarmonyPatch(typeof(InputUtil), nameof(InputUtil.GetInputY),new Type[] { typeof(float), typeof(bool) },new ArgumentType[] { ArgumentType.Out, ArgumentType.Out})]
	private static bool ReplaceInputY(ref bool __result, out float axis, out bool hasMouse)
	{
		float valueUp = ActionInputDefinitions.UiUp.AxisValue;
		float valueDown = ActionInputDefinitions.UiDown.AxisValue;
		float stickValue = ActionInputDefinitions.MoveY.AxisValue;
		/*if (ActionInputDefinitions.MoveY.AxisValue != 0f)		
			Logs.WriteInfo("MoveY: " + ActionInputDefinitions.MoveY.ActiveSource + ActionInputDefinitions.MoveY.AxisValue.ToString());
		if (ActionInputDefinitions.MoveX.AxisValue != 0f)
			Logs.WriteInfo("MoveX: " + ActionInputDefinitions.MoveX.ActiveSource + ActionInputDefinitions.MoveX.AxisValue.ToString());
		if (ActionInputDefinitions.UiUp.AxisValue != 0)
			Logs.WriteInfo("UiUp: " + ActionInputDefinitions.UiUp.ActiveSource + ActionInputDefinitions.UiUp.AxisValue.ToString());
		if (ActionInputDefinitions.UiDown.AxisValue != 0)
			Logs.WriteInfo("UiDown: " + ActionInputDefinitions.UiDown.ActiveSource + ActionInputDefinitions.UiDown.AxisValue.ToString());
		if (ActionInputDefinitions.Jump.ButtonDown)
			Logs.WriteInfo("Jump:" + ActionInputDefinitions.Jump.ActiveSource + ActionInputDefinitions.Jump.ButtonDown.ToString());
		if (ActionInputDefinitions.Weapon.ButtonDown)
			Logs.WriteInfo("Weapon:" + ActionInputDefinitions.Weapon.ActiveSource + ActionInputDefinitions.Weapon.ButtonDown.ToString());
		if (ActionInputDefinitions.Interact.ButtonDown)
			Logs.WriteInfo("Interact:" + ActionInputDefinitions.Interact.ActiveSource + ActionInputDefinitions.Interact.ButtonDown.ToString());*/

		hasMouse = false;
		axis = 0f;
		__result = false;
		if (valueUp != 0f)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			axis = valueUp;
			__result = true;
			//Logs.WriteInfo("Axis UP: " + axis);
		}
		else if (valueDown != 0f)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			axis = -valueDown;
			__result = true;
			//Logs.WriteInfo("Axis DOWN: " + axis);
		}
		else if(stickValue != 0f)
        {
			axis = stickValue;
			__result = true;
			//Logs.WriteInfo("Stick Movement: " + axis);
		}
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(InputUtil), nameof(InputUtil.GetInputX), new Type[] { typeof(float), typeof(bool) }, new ArgumentType[] { ArgumentType.Out, ArgumentType.Out })]
	private static bool ReplaceInputX(ref bool __result, out float axis, out bool hasMouse)
	{
		float valueLeft = ActionInputDefinitions.Previous.AxisValue;
		float valueRight = ActionInputDefinitions.Next.AxisValue;
		float stickValue = ActionInputDefinitions.MoveX.AxisValue;
		/*if (ActionInputDefinitions.MoveY.AxisValue != 0f)		
			Logs.WriteInfo("MoveY: " + ActionInputDefinitions.MoveY.ActiveSource + ActionInputDefinitions.MoveY.AxisValue.ToString());
		if (ActionInputDefinitions.MoveX.AxisValue != 0f)
			Logs.WriteInfo("MoveX: " + ActionInputDefinitions.MoveX.ActiveSource + ActionInputDefinitions.MoveX.AxisValue.ToString());
		if (ActionInputDefinitions.UiUp.AxisValue != 0)
			Logs.WriteInfo("UiUp: " + ActionInputDefinitions.UiUp.ActiveSource + ActionInputDefinitions.UiUp.AxisValue.ToString());
		if (ActionInputDefinitions.UiDown.AxisValue != 0)
			Logs.WriteInfo("UiDown: " + ActionInputDefinitions.UiDown.ActiveSource + ActionInputDefinitions.UiDown.AxisValue.ToString());
		if (ActionInputDefinitions.Jump.ButtonDown)
			Logs.WriteInfo("Jump:" + ActionInputDefinitions.Jump.ActiveSource + ActionInputDefinitions.Jump.ButtonDown.ToString());
		if (ActionInputDefinitions.Weapon.ButtonDown)
			Logs.WriteInfo("Weapon:" + ActionInputDefinitions.Weapon.ActiveSource + ActionInputDefinitions.Weapon.ButtonDown.ToString());
		if (ActionInputDefinitions.Interact.ButtonDown)
			Logs.WriteInfo("Interact:" + ActionInputDefinitions.Interact.ActiveSource + ActionInputDefinitions.Interact.ButtonDown.ToString());*/

		hasMouse = false;
		axis = 0f;
		__result = false;
		if (valueRight != 0f)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			axis = valueRight;
			__result = true;
			//Logs.WriteInfo("Axis UP: " + axis);
		}
		else if (valueLeft != 0f)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			axis = -valueLeft;
			__result = true;
			//Logs.WriteInfo("Axis DOWN: " + axis);
		}
		else if (stickValue != 0f)
		{
			axis = stickValue;
			__result = true;
			//Logs.WriteInfo("Stick Movement: " + axis);
		}
		return false;
	}
}

