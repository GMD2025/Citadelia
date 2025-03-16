using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace _Scripts.CustomInspector.Button.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class InspectorButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector first
            DrawDefaultInspector();

            // Get the type of the MonoBehaviour
            var targetType = target.GetType();
        
            // Get all methods in the MonoBehaviour class
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        
            // Iterate through methods to find those with the InspectorButtonAttribute
            foreach (var method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<InspectorButtonAttribute>();

                if (buttonAttribute == null) continue;
                // Display a button in the Inspector with the name from the attribute
                if (GUILayout.Button(buttonAttribute.ButtonName))
                {
                    // Invoke the method when the button is clicked
                    method.Invoke(target, null);
                }
            }

            // Refresh the inspector to make sure it's updated
            Repaint();
        }
    }
}