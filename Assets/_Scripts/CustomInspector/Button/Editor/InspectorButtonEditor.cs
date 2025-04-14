using System.Reflection;
using UnityEditor;
using UnityEngine;
using _Scripts.CustomInspector;

namespace _Scripts.CustomInspector.Button.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class InspectorButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw serialized fields first
            DrawDefaultInspector();

            var targetType = target.GetType();
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                // Check for [InspectorButton]
                var buttonAttr = method.GetCustomAttribute<InspectorButtonAttribute>();
                if (buttonAttr == null) continue;

                // Check for [InspectorLabel] optionally
                var labelAttr = method.GetCustomAttribute<InspectorLabelAttribute>();
                if (labelAttr != null && !string.IsNullOrWhiteSpace(labelAttr.Label))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(labelAttr.Label, EditorStyles.boldLabel);
                }

                // Use custom name if provided, otherwise fallback to method name
                string buttonText = string.IsNullOrWhiteSpace(buttonAttr.ButtonName)
                    ? ObjectNames.NicifyVariableName(method.Name)
                    : buttonAttr.ButtonName;

                if (GUILayout.Button(buttonText))
                {
                    method.Invoke(target, null);
                }
            }

            Repaint();
        }
    }
}