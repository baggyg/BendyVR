using BendyVR_5.src;
using UnityEngine;

namespace BendyVR_5.Helpers;

public static class Logs
{
    // ReSharper disable Unity.PerformanceAnalysis
    public static void WriteInfo(object data)
    {
        BendyVRPlugin.logBendyVR.LogInfo(data);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void WriteWarning(object data)
    {
        BendyVRPlugin.logBendyVR.LogWarning(data);        
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void WriteError(object data)
    {
        BendyVRPlugin.logBendyVR.LogError(data);
    }
}