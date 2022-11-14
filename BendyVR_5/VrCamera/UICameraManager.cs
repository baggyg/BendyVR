using BendyVR_5.Helpers;
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
    private float mRange = 50f;
    private float mLerp = 0f;
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
        //So lets try and lerp the CameraParent to match the opposite rotation
        //Firstly lets get the rotation
        Vector3 rotation = mUICamera.transform.localRotation.eulerAngles;
        //Vector3 newPosition = mUICamera.transform.localPosition;

        //Lets just set it instantly to test        
        //mUICameraParent.transform.RotateAround(mUICamera.transform.localPosition, Vector3.up, -rotation.y);
        
        //Here's an idea... if we hit rotation of 0 just reset everything
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
            mUICameraParentParent.transform.localEulerAngles = new Vector3(0f, mRange - rotation.y, 0f);
        }
        else if (offset < 360f - mRange && offset > 180f)
        {
            mUICentre = (rotation.y + mRange) - 360f;
            mUICameraParentParent.transform.localEulerAngles = new Vector3(0f, (360f - rotation.y) - mRange, 0f);            
        }        

        //mUICameraParentParent.transform.localPosition = new Vector3(-newPosition.x, 0f, 0f);        
    }

}