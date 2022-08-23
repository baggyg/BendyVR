using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BendyVR_5.Stage;


namespace BendyVR_5;

public abstract class BendyVRPatch
{
    protected static VrStage StageInstance;

    public static void SetStage(VrStage vrStage)
    {
        StageInstance = vrStage;
    }
}
