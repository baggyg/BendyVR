using BendyVR_5.Helpers;

namespace BendyVR_5.UI;

public class StaticUi : AttachedUi
{
    private void Awake()
    {
        MaterialHelper.MakeGraphicChildrenDrawOnTop(gameObject);
    }
}