using System.Reflection;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace _Scripts.CustomInspector.Button.Editor
{
    [CustomEditor(typeof(NetworkBehaviour), true)]
    public class NetworkInspectorButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector first
            DrawDefaultInspector();

            // Get the type of the MonoBehaviour
            var targetType = target.GetType();
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        
            bool hasButtons = false;
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<InspectorButtonAttribute>() != null)
                {
                    hasButtons = true;
                    break;
                }
            }
            
            // Only draw if we have buttons
            if (hasButtons)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
                
                // Draw all buttons
                foreach (var method in methods)
                {
                    var buttonAttribute = method.GetCustomAttribute<InspectorButtonAttribute>();
                    if (buttonAttribute == null) continue;
                    
                    if (GUILayout.Button(buttonAttribute.ButtonName))
                    {
                        // Invoke the method when the button is clicked
                        method.Invoke(target, null);
                    }
                }
            }
        }
    }
    
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class InspectorButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector first
            DrawDefaultInspector();

            // Get the type of the MonoBehaviour
            var targetType = target.GetType();
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        
            bool hasButtons = false;
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<InspectorButtonAttribute>() != null)
                {
                    hasButtons = true;
                    break;
                }
            }
            
            // Only draw if we have buttons
            if (hasButtons)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
                
                // Draw all buttons
                foreach (var method in methods)
                {
                    var buttonAttribute = method.GetCustomAttribute<InspectorButtonAttribute>();
                    if (buttonAttribute == null) continue;
                    
                    if (GUILayout.Button(buttonAttribute.ButtonName))
                    {
                        // Invoke the method when the button is clicked
                        method.Invoke(target, null);
                    }
                }
            }
        }
    }
}