using System.Reflection;
using UnityEditor;
using UnityEngine;
using Unity.Netcode; // Needed because NetworkBehaviour exists

namespace _Scripts.CustomInspector.Button.Editor
{
    [CustomEditor(typeof(Component), true)]
    [CanEditMultipleObjects]
    public class InspectorButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var targetType = serializedObject.targetObject.GetType();
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var buttonAttr = method.GetCustomAttribute<InspectorButtonAttribute>();
                if (buttonAttr == null) continue;

                var labelAttr = method.GetCustomAttribute<InspectorLabelAttribute>();
                if (labelAttr != null && !string.IsNullOrWhiteSpace(labelAttr.Label))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(labelAttr.Label, EditorStyles.boldLabel);
                }

                string buttonText = string.IsNullOrWhiteSpace(buttonAttr.ButtonName)
                    ? ObjectNames.NicifyVariableName(method.Name)
                    : buttonAttr.ButtonName;

                if (GUILayout.Button(buttonText))
                {
                    foreach (var targetObj in serializedObject.targetObjects)
                    {
                        method.Invoke(targetObj, null);
                    }
                }
            }
        }
    }
}