using BendyVR_5.Helpers;
using BendyVR_5.Settings;
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
	//[HarmonyPatch(typeof(CH1FinaleController), nameof(CH1FinaleController.HandleAxeOnEquipped))]
    [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.OnEquip))]
    //[HarmonyPatch(typeof(CH4BertrumController), nameof(CH4BertrumController.HandleAxeOnEquipped))]
	private static void PrepareAxe(MeleeWeapon __instance)
	{
        //GB - May need to change this to the specific HandleAxeOnEquipped calls whenever they occur
        VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();

        //Poss Fix - Make sure this is the child of hand (not sure why its not here)
        Logs.WriteWarning("Preparing Axe " + __instance.transform.name);
        Logs.WriteWarning("Parent is " + __instance.transform.parent.name);
        
        //Set Up Axe Local Position
        __instance.transform.SetParent(GameManager.Instance.Player.WeaponParent);
        
		vrPlayerController.SetupAxe();		
	}

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.UnEquipWeapon))]    
    private static void LoseAxe(PlayerController __instance)
    {
        VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();
        vrPlayerController.LoseAxe();
    }




    [HarmonyPrefix]
    [HarmonyPatch(typeof(CH2RitualRoomController), nameof(CH2RitualRoomController.ForceComplete))]
    private static bool VRCH2RitualRoomControllerComplete(CH2RitualRoomController __instance)
    {
        GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
        GameManager.Instance.Player.WeaponGameObject = __instance.m_Axe.gameObject;
        GameManager.Instance.Player.EquipWeapon();
        if ((bool)__instance.m_Axe && __instance.m_Axe.Interaction != null)
        {
            __instance.m_Axe.Interaction.SetActive(active: false);
        }
        __instance.m_Axe.KillInteraction();
        __instance.m_Axe.Equip();

        __instance.m_Plank.gameObject.SetActive(value: false);
        __instance.m_ScarePlank.gameObject.SetActive(value: false);
        __instance.m_Door.ForceOpen(145f);
        __instance.m_Door.Lock();
        __instance.SendOnComplete();

        return false;
    }



    [HarmonyPrefix]
    [HarmonyPatch(typeof(BaseWeapon), nameof(BaseWeapon.CheckHitRaycast))]
    private static bool AmendHitRaycast(BaseWeapon __instance)
    {
        //Change this to be the weapon 
        float weaponYOffset = 1.0f;

        //Logs.WriteInfo(__instance.m_WeaponModel.name + " (AmendHitRayCast)");

        //If its the axe set the range to very small so they physically have to hit it
        if(__instance.m_WeaponModel.name.Equals("Axe"))
        {
            __instance.m_WeaponRange = 1.0f;            
        }
        
        //Inst4ead of the camera set the transform to be the axe model
        Transform transform = __instance.m_WeaponModel.transform;        

        //Try a cast
        if (Physics.SphereCast(transform.position + (transform.rotation * new Vector3(0f, 0f, weaponYOffset)), 0.5f, -transform.up, out var hitInfo, __instance.m_WeaponRange, ~(int)__instance.m_IgnoreLayers))
        {
            //If Hit Something
            __instance.OnRaycastHit(hitInfo);
            //if (__instance.m_debugHitObject)
            //{

            /*GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = transform.position + (transform.rotation * new Vector3(0f, 0f, weaponYOffset));
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            UnityEngine.Object.Destroy(sphere.GetComponent<Collider>());
            
            GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere2.transform.position = hitInfo.point;
            sphere2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            UnityEngine.Object.Destroy(sphere2.GetComponent<Collider>());
            var renderer = sphere2.GetComponent<MeshRenderer>();
            renderer.material.SetColor("_Color", Color.green);
            
            UnityEngine.Object.Destroy(sphere, 3f);
            UnityEngine.Object.Destroy(sphere2, 3f);*/


            //if (__instance.m_debugHitObject)
            //{
            //Debug.Log("Weapon Range: " + __instance.m_WeaponRange);
            //Debug.Log("Weapon Hit Object: " + hitInfo.transform.name);
            //}
        }
        /*else
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = transform.position + (transform.rotation * new Vector3(0f, 0f, weaponYOffset));
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            UnityEngine.Object.Destroy(sphere.GetComponent<Collider>());

            GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere2.transform.position = (transform.position + (transform.rotation * new Vector3(0f, 0f, weaponYOffset))) + (-transform.up * (__instance.m_WeaponRange - 0.5f));
            sphere2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            UnityEngine.Object.Destroy(sphere2.GetComponent<Collider>());

            var renderer = sphere2.GetComponent<MeshRenderer>();
            renderer.material.SetColor("_Color", Color.red);

            UnityEngine.Object.Destroy(sphere, 3f);
            UnityEngine.Object.Destroy(sphere2, 3f);            
        }*/
        
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BaseWeapon), nameof(BaseWeapon.Attack))]
    private static bool NewAttack(BaseWeapon __instance)
    {
        //Attack is always called
        //((!m_IsHoldToAttack) ? PlayerInput.Attack() : PlayerInput.AttackHold())
        if (__instance.m_CanAttack && __instance.m_IsEquipped && !GameManager.Instance.Player.isLocked && !GameManager.Instance.isPaused && 
            (VrCore.instance.GetAttackVelocity() >= VrSettings.VelocityTrigger.Value || VrCore.instance.GetAttackAngularVelocity() >= VrSettings.AngularVelocityTrigger.Value))
        {
            /*if(VrCore.instance.GetAttackVelocity() >= VrSettings.VelocityTrigger.Value)
            {
                Logs.WriteInfo("Velocity Attack: " + VrCore.instance.GetAttackVelocity());
            }
            else if(VrCore.instance.GetAttackAngularVelocity() >= VrSettings.AngularVelocityTrigger.Value)
            {
                Logs.WriteInfo("Angular Attack: " + VrCore.instance.GetAttackAngularVelocity());
            } */              
            __instance.m_CanAttack = false;
            __instance.OnAttack();
        }
        /*else if(!__instance.m_CanAttack && (VrCore.instance.GetAttackVelocity() >= VrSettings.VelocityTrigger.Value || VrCore.instance.GetAttackAngularVelocity() >= VrSettings.AngularVelocityTrigger.Value))
        {
            Logs.WriteInfo("Can't Attack!");
        }*/
        /*else if (__instance.m_IsEquipped && !GameManager.Instance.Player.isLocked && !GameManager.Instance.isPaused &&
            VrCore.instance.GetAttackVelocity() < VrSettings.VelocityTrigger.Value && 
            VrCore.instance.GetAttackAngularVelocity() < VrSettings.AngularVelocityTrigger.Value)
        {
            if(VrCore.instance.GetAttackVelocity() > 0f)
                Logs.WriteInfo("Velocity too low: " + VrCore.instance.GetAttackVelocity());

            if (VrCore.instance.GetAttackAngularVelocity() > 0f)
                Logs.WriteInfo("Angular Velocity too low: " + VrCore.instance.GetAttackAngularVelocity());
        }*/
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.OnAttack))]
    private static bool RevisedAxeAttack(BaseWeapon __instance)
    {
        //__instance.ResetAttackSequence();
        //GetAnimationIndex();
        //GameManager.Instance.Player.WeaponParent.GetComponentInParent<Animator>().Play(m_AnimationClips[m_AnimationIndex].name);

        //TODO Poss Overwrite HandleSwingBegin and only play a sound when I haven't played it in the time it takes to play it (wonderful sentence)
        __instance.HandleSwingBegin();
        __instance.HandleSwingHit();
        __instance.HandleSwingEnd();
        __instance.HandleSwingComplete();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.DOAttack))]
    private static bool RevisedAxeAttack(BaseWeapon __instance, ref Sequence __result)
    {
        __instance.ResetAttackSequence();
        //GetAnimationIndex();
        //GameManager.Instance.Player.WeaponParent.GetComponentInParent<Animator>().Play(m_AnimationClips[m_AnimationIndex].name);

        //TODO Poss Overwrite HandleSwingBegin and only play a sound when I haven't played it in the time it takes to play it (wonderful sentence)
        __instance.m_AttackSequence.InsertCallback(0f, __instance.HandleSwingBegin);
        __instance.m_AttackSequence.InsertCallback(0.01f, __instance.HandleSwingHit);
        __instance.m_AttackSequence.InsertCallback(0.05f, __instance.HandleSwingEnd);
        __result = __instance.m_AttackSequence;
        return false; 
    }
}
