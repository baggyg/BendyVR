using BendyVR_5.Helpers;
using BendyVR_5.Settings;
using BendyVR_5.Stage;
using BendyVR_5.VrInput.ActionInputs;
using UnityEngine;


namespace BendyVR_5.LaserPointer;

public class VrLaser : MonoBehaviour
{
    public float laserLength = 1.5f;
    private readonly IActionInput actionInput = ActionInputDefinitions.Interact;
    
    private LineRenderer lineRenderer;    

    public static VrLaser Create(Transform dominantHand)
    {
        var instance = new GameObject("VrHandLaser").AddComponent<VrLaser>();
        var instanceTransform = instance.transform;
        instanceTransform.SetParent(dominantHand, false);
        instanceTransform.localEulerAngles = new Vector3(39.132f, 356.9302f, 0.3666f);
        return instance;
    }

    private void Start()
    {
        //Trying to match length to shimmer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.SetPositions(new[] {Vector3.zero, Vector3.forward * laserLength});
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.001f;
        lineRenderer.endColor = new Color(1, 1, 1, 0.8f);
        lineRenderer.startColor = Color.clear;
        lineRenderer.material.shader = Shader.Find("Particles/Alpha Blended Premultiply");
        //lineRenderer.material.shader = Shader.Find("Standard");
        lineRenderer.material.SetColor(ShaderProperty.Color, Color.white);
        lineRenderer.sortingOrder = 10000;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        UpdateLaserVisibility();        
    }    
    
    private void UpdateLaserVisibility()
    {
        if (VrSettings.ShowLaserPointer.Value && !VrCore.instance.GetVRPlayerController().mPlayerController.isSeeingToolActive)
        {
            lineRenderer.enabled = ActionInputDefinitions.Interact.AxisValue != 0;
        }
    }

    
}