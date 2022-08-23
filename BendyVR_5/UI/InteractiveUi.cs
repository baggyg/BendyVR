using HarmonyLib;
using UnityEngine;
using UnityEngine.UI.Collections;
using UnityEngine.EventSystems;
using InControl;

namespace BendyVR_5.UI;

public class InteractiveUi : AttachedUi
{
    private BoxCollider collider;
    private InControlInputModule[] inputModules = { };

    private void Start()
    {
        SetUpCollider();
        SetUpInputModules();
    }

    protected override void Update()
    {
        base.Update();

        var active = IsAnyInputModuleActive();
        if (active && !collider.enabled)
            collider.enabled = true;
        else if (!active && collider.enabled) collider.enabled = false;
    }

    private bool IsAnyInputModuleActive()
    {
        if (inputModules.Length == 0) return false;
        foreach (var inputModule in inputModules)
            if (inputModule && inputModule.gameObject.activeInHierarchy && inputModule.enabled)
                return true;

        return false;
    }

    private void SetUpInputModules()
    {
        var rootObjects = gameObject.scene.GetRootGameObjects();
        foreach (var rootObject in rootObjects)
        {
            var modules = rootObject.GetComponentsInChildren<InControlInputModule>(true);
            inputModules = inputModules.AddRangeToArray(modules);
        }
    }

    private void SetUpCollider()
    {
        collider = gameObject.GetComponent<BoxCollider>();
        if (collider != null) return;

        var rectTransform = gameObject.GetComponent<RectTransform>();
        collider = gameObject.gameObject.AddComponent<BoxCollider>();
        var rectSize = rectTransform.sizeDelta;
        collider.size = new Vector3(rectSize.x, rectSize.y, 0.1f);
        gameObject.layer = LayerMask.NameToLayer("UI");
    }
}