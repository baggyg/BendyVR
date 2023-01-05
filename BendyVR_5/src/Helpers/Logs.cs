using BendyVR_5.src;
using UnityEngine;

namespace BendyVR_5.Helpers;

public static class Logs
{
    // ReSharper disable Unity.PerformanceAnalysis
    public static void WriteInfo(object data)
    {
#if DEBUG
        BendyVRPlugin.logBendyVR.LogInfo(data);
#endif
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void WriteWarning(object data)
    {
#if DEBUG
        BendyVRPlugin.logBendyVR.LogWarning(data);        
#endif
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void WriteError(object data)
    {
#if DEBUG        
        BendyVRPlugin.logBendyVR.LogError(data);
#endif
    }
}