using BendyVR_5.Helpers;
using BendyVR_5.Stage;
using DG.Tweening;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMG.Controls;
using UnityEngine;

namespace BendyVR_5.Player.Patches;

[HarmonyPatch]
public class HoldableItemPatches : BendyVRPatch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(CH1FinaleController), nameof(CH1FinaleController.HandleAxeOnEquipped))]
	//[HarmonyPatch(typeof(CH4BertrumController), nameof(CH4BertrumController.HandleAxeOnEquipped))]
	private static void PrepareAxe(CH1FinaleController __instance)
	{
		//GB - May need to change this to the specific HandleAxeOnEquipped calls whenever they occur
		VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();
		vrPlayerController.SetupAxe();		
	}
}
