using BepInEx.Configuration;
using BendyVR_5.Settings;
using BendyVR_5.Stage;
using UnityEngine;

namespace BendyVR_5.Player;

public class MovementDirection : MonoBehaviour
{
    private Transform cameraDirectionTransform;
    private VrCore stage;

    public static void Create(PlayerController navigationController, VrCore stage)
    {
        var instance = new GameObject("VrMovementDirection").AddComponent<MovementDirection>();

        instance.cameraDirectionTransform = navigationController.transform.Find("henry");
        instance.stage = stage;

        // Usually NavigationController uses player camera forward as a basis for movement direction.
        // This dummy camera is used instead, so that movement direction can be independent of the camera rotation.
        //var dummyCamera = instance.gameObject.AddComponent<Camera>();
        //dummyCamera.enabled = false;
        //navigationController.playerCamera = dummyCamera;
    }

    private void Start()
    {
        UpdateParent();
    }

    private void LateUpdate()
    {
        var forward = transform.parent.forward;
        forward.y = 0;
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    private void OnEnable()
    {
        VrSettings.Config.SettingChanged += HandleSettingChanged;
    }

    private void OnDisable()
    {
        VrSettings.Config.SettingChanged -= HandleSettingChanged;
    }

    private void HandleSettingChanged(object sender, SettingChangedEventArgs e)
    {
        UpdateParent();
    }

    private void UpdateParent()
    {
        transform.SetParent(GetMovementDirectionTransform(), false);
    }

    private Transform GetMovementDirectionTransform()
    {
        return VrSettings.ControllerBasedMovementDirection.Value
            ? stage.GetMovementStickHand()
            : cameraDirectionTransform;
    }
}