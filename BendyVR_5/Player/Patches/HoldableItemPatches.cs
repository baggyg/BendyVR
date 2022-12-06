using BendyVR_5.Helpers;
using BendyVR_5.Settings;
using BendyVR_5.Stage;
using DG.Tweening;
using HarmonyLib;
using S13Audio;
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
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Holdable), nameof(Holdable.Remove))]
    public static bool RemoveHoldableItem(Holdable __instance)
    {
        Logs.WriteWarning("Removing Holdable " + __instance.transform.name);
        if (__instance.m_HoldableType == Holdable.HoldableType.INK)
        {
            S13AudioManager.Instance.InvokeEvent("evt_ink_deposited");
        }
        if ((bool)GameManager.Instance.Player.InactiveWeapon)
        {
            GameManager.Instance.Player.WeaponGameObject = GameManager.Instance.Player.InactiveWeapon;
            GameManager.Instance.Player.WeaponGameObject.SetActive(value: true);
            GameManager.Instance.Player.EquipWeapon();
            GameManager.Instance.Player.InactiveWeapon = null;
        }
        else
        {
            VrCore.instance.GetVRPlayerController().TurnOnDominantHand();
        }
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Holdable), nameof(Holdable.OnInteract))]
    private static void PrepareHoldableItem(Holdable __instance)
    {
        //GB - May need to change this to the specific HandleAxeOnEquipped calls whenever they occur
        VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();

        //Poss Fix - Make sure this is the child of hand (not sure why its not here)
        Logs.WriteWarning("Preparing Holdable " + __instance.transform.name);
        Logs.WriteWarning("Parent is " + __instance.transform.parent.name);

        //Set Up Local Position
        __instance.transform.SetParent(GameManager.Instance.Player.WeaponParent);

        vrPlayerController.SetupMeleeWeapon(__instance.transform.name);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GunWeapon), nameof(GunWeapon.OnEquip))]
    private static void PrepareAxe(GunWeapon __instance)
    {
        //GB - May need to change this to the specific HandleAxeOnEquipped calls whenever they occur
        VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();

        //Poss Fix - Make sure this is the child of hand (not sure why its not here)
        Logs.WriteWarning("Preparing Weapon " + __instance.transform.name);
        Logs.WriteWarning("Parent is " + __instance.transform.parent.name);

        //Set Up Local Position
        __instance.transform.SetParent(GameManager.Instance.Player.WeaponParent);

        vrPlayerController.SetupGunWeapon(__instance.transform.name);
    }

    [HarmonyPostfix]
	[HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.OnEquip))]    
    private static void PrepareAxe(MeleeWeapon __instance)
	{
        //GB - May need to change this to the specific HandleAxeOnEquipped calls whenever they occur
        VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();

        //Poss Fix - Make sure this is the child of hand (not sure why its not here)
        Logs.WriteWarning("Preparing Weapon " + __instance.transform.name);
        Logs.WriteWarning("Parent is " + __instance.transform.parent.name);
        
        //Set Up Axe Local Position
        __instance.transform.SetParent(GameManager.Instance.Player.WeaponParent);
        
		vrPlayerController.SetupMeleeWeapon(__instance.transform.name);		
	}

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BaseWeapon), nameof(BaseWeapon.CleanEquip))]
    private static void PrepareWeapon(BaseWeapon __instance)
    {
        if (__instance is MeleeWeapon)
        {
            //GB - May need to change this to the specific HandleAxeOnEquipped calls whenever they occur
            VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();

            //Poss Fix - Make sure this is the child of hand (not sure why its not here)
            Logs.WriteWarning("(CleanEquip) Preparing Weapon " + __instance.transform.name);
            if(__instance.transform.parent != null)
                Logs.WriteWarning("Parent is " + __instance.transform.parent.name);

            //Set Up Axe Local Position
            __instance.transform.SetParent(GameManager.Instance.Player.WeaponParent);

            vrPlayerController.SetupMeleeWeapon(__instance.transform.name);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.UnEquipWeapon))]    
    private static void LoseAxe(PlayerController __instance)
    {
        VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();
        vrPlayerController.LoseMelee();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CH3Flashlight), nameof(CH3Flashlight.Equip))]
    private static void PrepareFlashlight(CH3Flashlight __instance)
    {
        //GB - May need to change this to the specific HandleAxeOnEquipped calls whenever they occur
        VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();

        //Poss Fix - Make sure this is the child of hand (not sure why its not here)
        Logs.WriteWarning("Preparing Flashlight " + __instance.transform.name);
        Logs.WriteWarning("Parent is " + __instance.transform.parent.name);

        //Set Up Axe Local Position
        __instance.transform.SetParent(GameManager.Instance.Player.WeaponParent);

        vrPlayerController.SetupMeleeWeapon(__instance.transform.name);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CH3DarkHallwayController), nameof(CH3DarkHallwayController.HandleBorisOnInteracted))]
    [HarmonyPatch(typeof(CH4VentController), nameof(CH4VentController.HandleExitVentOnComplete))]
    private static void LoseFlashlight()
    {
        Logs.WriteWarning("Turning On Dominant Hand");
        VrCore.instance.GetVRPlayerController().TurnOnDominantHand();
    }    

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CH3SafehouseController), nameof(CH3SafehouseController.HandleCookedSoupOnInteracted))]
    private static void PickupSoup(CH3SafehouseController __instance)
    {
        //GB - May need to change this to the specific HandleAxeOnEquipped calls whenever they occur
        VRPlayerController vrPlayerController = VrCore.instance.GetVRPlayerController();

        //Poss Fix - Make sure this is the child of hand (not sure why its not here)
        Logs.WriteWarning("Preparing Soup " + __instance.m_CarryBowl.transform.name);
        Logs.WriteWarning("Parent is " + __instance.m_CarryBowl.transform.parent.name);
                
        vrPlayerController.SetupMeleeWeapon(__instance.m_CarryBowl.transform.name);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CH3SafehouseController), nameof(CH3SafehouseController.HandleBorisBowlOnInteracted))]
    private static void GiveSoup(CH3SafehouseController __instance)
    {
        VrCore.instance.GetVRPlayerController().TurnOnDominantHand();
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(BaseWeapon), nameof(BaseWeapon.CheckHitRaycast))]
    private static bool AmendHitRaycast(BaseWeapon __instance)
    {
        //Change this to be the weapon 
        float weaponYOffset = 1.0f;

        //Logs.WriteInfo(__instance.m_WeaponModel.name + " (AmendHitRayCast)");

        //If its the axe set the range to very small so they physically have to hit it
        /*if(__instance.m_WeaponModel.name.Equals("Axe"))
        {
            __instance.m_WeaponRange = 1.0f;            
        }*/
        //GB Set the range to 1 anyway and correct if not right
        __instance.m_WeaponRange = 1.0f;

        //Inst4ead of the camera set the transform to be the axe model
        Transform transform = __instance.m_WeaponModel.transform;
                
        if (__instance.m_WeaponModel.name.ToLower().Equals("model_gent_pipe"))
        {            
            weaponYOffset = 1.8f;
        }
        else if (__instance.m_WeaponModel.name.ToLower().Equals("ch3_wrench_weapon"))
        {
            weaponYOffset = 0.7f;
        }
        else if (__instance.m_WeaponModel.name.ToLower().Equals("weapon_inktool") ||
            __instance.m_WeaponModel.name.ToLower().Equals("weapon_inktool(clone)") ||
            __instance.m_WeaponModel.name.ToLower().Equals("pump tool model"))
        {
            weaponYOffset = 0.0f;
        }


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
        if (__instance is GunWeapon)
        {
            if (__instance .m_CanAttack && __instance.m_IsEquipped && 
                !GameManager.Instance.Player.isLocked && 
                !GameManager.Instance.isPaused && 
                ((!__instance.m_IsHoldToAttack) ? PlayerInput.Attack() : PlayerInput.AttackHold()))
            {
                __instance.m_CanAttack = false;
                __instance.OnAttack();
            }
        }
        else if (__instance is MeleeWeapon)
        {
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
    [HarmonyPatch(typeof(GunWeapon), nameof(GunWeapon.OnAttack))]
    public static bool GunAttack(GunWeapon __instance)
    {
        __instance.m_CanAttack = true;
        if (__instance.m_IsReloading || !(Time.time > __instance.m_FireRateNext))
        {
            return false;
        }
        __instance.m_FireRateNext = Time.time + __instance.m_FireRate;
        if (__instance.m_ClipCount <= 0)
        {
            __instance.m_ReloadCount++;
            if (__instance.m_ReloadCount > __instance.m_ReloadMax)
            {
                __instance.Reload();
            }
            else
            {
                GameManager.Instance.AudioManager.Play("Audio/SFX/Gun/SFX_Ink_Gun_Empty");
            }
        }
        else
        {
            __instance.m_Bullets.Emit(1);
            __instance.m_Sparks.Emit(5);
            for (int i = 0; i < __instance.m_Lights.Count; i++)
            {
                __instance.m_Lights[i].enabled = true;
            }
            GameManager.Instance.AudioManager.Play("Audio/SFX/Gun/SFX_Ink_Shoot");
            //GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
            GameManager.Instance.GameCamera.transform.DOShakePosition(0.15f, 0.0f, 0, 90f, snapping: false, fadeOut: false).OnComplete(delegate
            {
                for (int j = 0; j < __instance.m_Lights.Count; j++)
                {
                    __instance.m_Lights[j].enabled = false;
                }
                //GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
            });
        }
        __instance.m_ClipCount--;
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GunWeapon), nameof(GunWeapon.Reload))]
    private static bool VRReload(GunWeapon __instance)
    {
        __instance.m_IsReloading = true;
        GameManager.Instance.AudioManager.Play("Audio/SFX/Gun/SFX_Ink_Gun_Reload");
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0f, __instance.transform.DOLocalMoveY(0f, 0.4f));
        sequence.Insert(0f, __instance.transform.DOLocalMoveZ(-0.25f, 0.4f));
        sequence.Insert(0f, __instance.transform.DOLocalRotate(new Vector3(25f, 0f, 0f), 0.4f));
        sequence.Insert(0.4f, __instance.transform.DOLocalMoveY(-0.2f, 0.4f));
        sequence.Insert(0.4f, __instance.transform.DOLocalMoveZ(-0.1f, 0.4f));
        sequence.Insert(0.4f, __instance.transform.DOLocalRotate(new Vector3(350f, 350f, 0f), 0.4f));
        sequence.OnComplete(__instance.ReloadOnComplete);
        return false;
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.OnAttack))]
    private static bool RevisedMeleeAttack(BaseWeapon __instance)
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
    private static bool RevisedMeleeAttack(BaseWeapon __instance, ref Sequence __result)
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
