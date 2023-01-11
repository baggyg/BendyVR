using BendyVR_5.Helpers;
using BendyVR_5.Stage;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace BendyVR_5.VrCamera;

public class UICameraManager : MonoBehaviour
{
    private Camera mUICamera;
    private Transform mUICameraParentParent;
    private Transform mUICameraParent;
    private float mUICentre = 0f;
    private float mRange = 40f;
    private float mLerp = 0f;
    private float mLerpSpeed = 0.01f;
    private Vector3 mTargetLerp;

    public static void Create(Camera UICamera)
    {
        var gameObject = GameObject.FindGameObjectWithTag("UIVisualControls");
        var instance = gameObject.AddComponent<UICameraManager>();        
        instance.mUICamera = UICamera;

        //RoomScale Mid Tier Dampening
        instance.mUICameraParentParent = new GameObject("UICameraParentParent").transform;
        instance.mUICameraParentParent.localPosition = new Vector3(0f, 0f, -900f);
        instance.mUICameraParentParent.localRotation = Quaternion.identity;
        instance.mUICameraParentParent.transform.SetParent(gameObject.transform);

        instance.mUICameraParent = new GameObject("UICameraParent").transform;        
        instance.mUICameraParent.transform.SetParent(instance.mUICameraParentParent.transform);
        instance.mUICameraParent.localPosition = new Vector3(0f, 0f, 900f);
        instance.mUICameraParent.localRotation = Quaternion.identity;
        instance.mUICamera.transform.SetParent(instance.mUICameraParent, false);        
    }

    private void Start()
    {
        
    }
    
    private void Update()
    {
        /*if (VrCore.instance.GetVRPlayerController().inLevel)
        {
            //So lets try and lerp the CameraParent to match the opposite rotation
            //Firstly lets get the rotation
            Vector3 rotation = mUICamera.transform.localRotation.eulerAngles;
            //Vector3 newPosition = mUICamera.transform.localPosition;
            if ((mUICameraParentParent.transform.rotation.y + 10 > -rotation.y ||
               mUICameraParentParent.transform.rotation.y - 10 < -rotation.y) &&
               mTargetLerp != new Vector3(0f, -rotation.y, 0f))
            {
                mTargetLerp = new Vector3(0f, -rotation.y, 0f);
                mLerp = 0.00f;
            }
            if (mLerp <= 1.0f)
            {
                Quaternion qr = Quaternion.Euler(mTargetLerp);
                mUICameraParentParent.transform.rotation = Quaternion.Lerp(mUICameraParentParent.transform.rotation, qr, mLerp);
                mLerp += 0.01f;
            }
        }*/


        //Lets just set it instantly to test        
        //mUICameraParent.transform.RotateAround(mUICamera.transform.localPosition, Vector3.up, -rotation.y);

        //Here's an idea... if we hit rotation of 0 just reset everything
        Vector3 rotation = mUICamera.transform.localRotation.eulerAngles;
        float offset = rotation.y - mUICentre;
        if (offset >= 360f)
        {
            offset -= 360f;
        }
        if (offset < 0f)
        {
            offset += 360f;
        }
        //Logs.WriteInfo(string.Format("Y: {0} Center: {1} Offset: {2}", rotation.y, mUICentre, offset));
        if (offset > mRange && offset < 180f)
        {
            mUICentre = rotation.y - mRange;            
            if (mTargetLerp != new Vector3(0f, mRange - rotation.y, 0f))
            {
                mTargetLerp = new Vector3(0f, mRange - rotation.y, 0f);
                //mTargetLerp = new Vector3(0f, rotation.y, 0f);
                //Quaternion qr = Quaternion.Euler(mTargetLerp);
                if (mLerp >= 1f)
                    mLerp = 0.01f;
            }
            //mUICameraParentParent.transform.rotation = Quaternion.Lerp(mUICameraParentParent.transform.rotation, qr, mLerp);

            //mUICameraParentParent.transform.localEulerAngles = mTargetLerp;
        }
        else if (offset < 360f - mRange && offset > 180f)
        {
            mUICentre = (rotation.y + mRange) - 360f;
            if (mTargetLerp != new Vector3(0f, (360f - rotation.y) - mRange, 0f))
            {
                mTargetLerp = new Vector3(0f, (360f - rotation.y) - mRange, 0f);
                if(mLerp >= 1f)
                    mLerp = 0.01f;
            }
            //Quaternion qr = Quaternion.Euler(mTargetLerp);
            //mUICameraParentParent.transform.rotation = Quaternion.Lerp(mUICameraParentParent.transform.rotation, qr, mLerp);
            //mUICameraParentParent.transform.localEulerAngles = mTargetLerp;            
        }

        if (mLerp < 1.0f)
        {
            Quaternion qr = Quaternion.Euler(mTargetLerp);
            mUICameraParentParent.transform.rotation = Quaternion.Lerp(mUICameraParentParent.transform.rotation, qr, mLerp * mLerpSpeed);
            mLerp += Time.deltaTime;
        }

        //mUICameraParentParent.transform.localPosition = new Vector3(-newPosition.x, 0f, 0f);        
    }

}