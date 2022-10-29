using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BendyVR_5.UI
{
    using UnityEngine;

    public class ErrorPresenter : MonoBehaviour
    {
        private static ErrorPresenter _instance;
        private string _message;

        private static ErrorPresenter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject(nameof(ErrorPresenter)).AddComponent<ErrorPresenter>();
                }

                return _instance;
            }
        }

        public static void Show(string message)
        {
            Instance._message = message;
        }

        private void OnGUI()
        {
            if (!_message.Equals(""))
            {
                Rect container = new Rect(Screen.width * 0.3f, Screen.height * 0.3f, Screen.width * 0.4f, Screen.height * 0.4f);
                GUI.Box(container, (string)null);
                GUILayout.BeginArea(new RectOffset(20, 20, 20, 20).Remove(container));

                DrawLabel();
                DrawButton();

                GUILayout.EndArea();
            }
        }

        private void DrawLabel()
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.Label(_message);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }

        private static void DrawButton()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Quit", GUILayout.MaxWidth(Screen.width * 0.1f)))
#if UNITY_EDITOR
             UnityEditor.EditorApplication.ExitPlaymode();
#else
                Application.Quit();
#endif
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
