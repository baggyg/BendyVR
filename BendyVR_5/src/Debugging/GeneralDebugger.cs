using BendyVR_5.Helpers;
using InControl;
using UnityEngine;

namespace BendyVR_5.Debugging;

public class GeneralDebugger : MonoBehaviour
{
    /*
    private static void UpdateInputsDebug()
    {
        var inputManager = InputManager.Instance;
        if (!Input.GetKeyDown(KeyCode.Alpha9)) return;
        InputManager.
        Logs.WriteInfo("## Starting key bind logs##");
        foreach (var bind in inputManager.virtualKeyKeyBindMap.Values)
        {
            Logs.WriteInfo("bind");
            foreach (var command in bind.commands) Logs.WriteInfo($"command: {command.command}");
        }

        foreach (var context in inputManager.contextStack)
        {
            Logs.WriteInfo($"## Context {context.name}:");
            foreach (var mapping in context.commandMap)
            {
                Logs.WriteInfo($"# mapping: {mapping.virtualKey}");
                foreach (var command in mapping.commands) Logs.WriteInfo($"command: {command.command}");
            }
        }

        Logs.WriteInfo("## Virtual keys: ##");
        foreach (var item in inputManager.customLayout.mapping) Logs.WriteInfo($"virtual key: {item.virtualKey}");

        Logs.WriteInfo("## Ended key bind logs ##");
    }*/
}