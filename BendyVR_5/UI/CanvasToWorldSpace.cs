using BendyVR_5.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMG.UI;
using UnityEngine;

namespace BendyVR_5.UI
{
    public static class CanvasToWorldSpace
    {
        public static void MoveToCameraSpace(BaseUIController __instance, float planeDistance = 1.0f)
        {
            Logs.WriteInfo($"Working with object ({__instance.name})");
            var canvas = __instance.GetComponent<Canvas>();            
            if (!canvas)
            {
                Logs.WriteError($"No Canvas Found in Object ({__instance.name})");
                return;
            }
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Logs.WriteWarning($"Canvas already has render mode ({canvas.renderMode.ToString()})");
                canvas.planeDistance = planeDistance;
                return;
            }
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.planeDistance = planeDistance;
            Logs.WriteInfo($"Canvas Should be set to Screen Space Camera ({__instance.name})");
        }

        public static void MoveToWorldSpace(BaseUIController __instance, float scale = 1.0f)
        {
            try
            {
                Logs.WriteInfo($"Working with object ({__instance.name})");
                var canvas = __instance.GetComponent<Canvas>();
                if (!canvas)
                {
                    Logs.WriteError($"No Canvas Found in Object ({__instance.name})");
                    return;
                }
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    Logs.WriteWarning($"Canvas already has render mode ({canvas.renderMode.ToString()})");
                    return;
                }
                canvas.renderMode = RenderMode.WorldSpace;
                Logs.WriteInfo($"Canvas Should be set to World Space ({__instance.name})");

                //Ensure Scale is Correct
                __instance.transform.position = new Vector3(0, 0, 0);
                __instance.transform.localScale = new Vector3(scale, scale, scale);
                __instance.transform.localRotation = Quaternion.identity;
                __instance.transform.localPosition = Vector3.zero;

                // Canvases with graphic raycasters are the ones that receive click events.
                // Those need to be handled differently, with colliders for the laser ray.
                /*
                 * if (canvas.GetComponent<GraphicRaycaster>())
                    AttachedUi.Create<InteractiveUi>(canvas, StageInstance.GetInteractiveUiTarget(), 0.002f);
                else
                    AttachedUi.Create<StaticUi>(canvas, StageInstance.GetStaticUiTarget(), 0.00045f);
                */
            }
            catch (Exception exception)
            {
                Logs.WriteWarning($"Failed to move canvas to world space ({__instance.name}): {exception}");
            }
        }
        

    }
}
