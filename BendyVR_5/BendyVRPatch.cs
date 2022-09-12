using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BendyVR_5.Stage;


namespace BendyVR_5;

public abstract class BendyVRPatch
{
    protected static VrCore StageInstance;

    public static void SetStage(VrCore vrStage)
    {
        StageInstance = vrStage;
    }
}
