using BepInEx.Configuration;
using System;
using System.Linq;
using UnityEngine;

namespace BendyVR_5.Settings;

public static class VrSettings
{
    public enum SmoothRotationSpeedOption
    {
        VerySlow = 1,
        Slow = 2,
        Default = 3,
        Fast = 4,
        VeryFast = 5
    }

    public enum SnapTurnAngleOption
    {
        Angle23 = 23,
        Angle30 = 30,
        Angle45 = 45,
        Angle60 = 60,
        Angle90 = 90
    }

    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }

    public static T Previous<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) - 1;
        return (j < 0) ? Arr.Last() : Arr[j];
    }

    private const string controlsCategory = "Controls";
    private const string comfortCategory = "Comfort";
    private const string playerBodyCategory = "Player Body";
    private const string renderingCategory = "Rendering";

    public static ConfigFile Config { get; private set; }
    public static ConfigEntry<bool> SnapTurning { get; private set; }
    public static ConfigEntry<bool> ShowLegs { get; private set; }
    public static ConfigEntry<bool> RoomScaleBodyPosition { get; private set; }
    public static ConfigEntry<bool> Teleport { get; private set; }
    public static ConfigEntry<bool> FixedCameraDuringAnimations { get; private set; }
    public static ConfigEntry<bool> LeftHandedMode { get; private set; }
    public static ConfigEntry<bool> SwapSticks { get; private set; }
    public static ConfigEntry<bool> ControllerBasedMovementDirection { get; private set; }
    public static ConfigEntry<float> WorldScale { get; private set; }
    public static ConfigEntry<SnapTurnAngleOption> SnapTurnAngle { get; private set; }
    public static ConfigEntry<SmoothRotationSpeedOption> SmoothRotationSpeed { get; private set; }

    public static void SetUp(ConfigFile config)
    {
        SetUpResolution();

        Config = config;
        SnapTurning = config.Bind(comfortCategory, "SnapTurning", true,
            "Snap turning|Enabled: snap turning. Disabled: smooth turning.");
        SnapTurnAngle = config.Bind(comfortCategory, "SnapTurnAngle", SnapTurnAngleOption.Angle45,
            "Snap turn angle|How much to turn when snap turning is enabled.");
        SmoothRotationSpeed = config.Bind(comfortCategory, "SmoothRotationSpeed", SmoothRotationSpeedOption.Default,
            "Smooth rotation speed|How fast to turn when snap turning is disabled.");
        Teleport = config.Bind(comfortCategory, "Teleport", false,
            "Fixed camera while moving|\"Teleport\" locomotion. Camera stays still while player moves.");
        FixedCameraDuringAnimations = config.Bind(comfortCategory, "FixedCameraDuringAnimations", false,
            "Fixed camera during animations|Camera stays still during some larger animations.");
        RoomScaleBodyPosition = config.Bind(playerBodyCategory, "RoomScaleBodyPosition", true,
            "Make player body follow headset position|Disabling prevents drifting, but you'll need to occasionally recenter manually in the pause menu.");
        ControllerBasedMovementDirection = config.Bind(controlsCategory, "ControllerBasedMovementDirection", false,
            "Controller-based movement direction|Enabled: controller-based direction. Disabled: head-based direction.");
        LeftHandedMode = config.Bind(controlsCategory, "LeftHandedMode", false,
            "Left handed mode.");
        SwapSticks = config.Bind(controlsCategory, "SwapSticks", false,
            "Swap movement / rotation sticks|Swaps controller sticks, independently of handedness.");
        WorldScale = config.Bind(renderingCategory, "WorldScale", 3.0f,
            "World Scale. Increase for World/Objects to look smaller|Decrease for World/Objects to look bigger");
    }

    public static string GetSnapTurnAngle()
    {
        string angle = SnapTurnAngle.Value.ToString();
        angle = angle.Replace("Angle", "");
        return angle;
    }

    public static void UpdateSnapTurnAngle(bool higher)
    {        
        if(higher)
            SnapTurnAngle.Value = SnapTurnAngle.Value.Next();
        else
            SnapTurnAngle.Value = SnapTurnAngle.Value.Previous();        
    }

    public static void UpdateWorldScale(bool higher)
    {
        if (higher)
            WorldScale.Value += 0.5f;            
        else
            WorldScale.Value -= 0.5f;
    }

    public static void UpdateSmoothTurnSpeed(bool higher)
    {
        if (higher)
            SmoothRotationSpeed.Value = SmoothRotationSpeed.Value.Next();
        else
            SmoothRotationSpeed.Value = SmoothRotationSpeed.Value.Previous();
    }

    private static void SetUpResolution()
    {
        Screen.SetResolution(1920, 1080, false);
    }
}