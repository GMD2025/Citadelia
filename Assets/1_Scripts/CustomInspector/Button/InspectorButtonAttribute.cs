using System;

namespace _1_Scripts.CustomInspector.Button
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InspectorButtonAttribute : Attribute
    {
        public string ButtonName { get; }
    
        public InspectorButtonAttribute(string buttonName)
        {
            ButtonName = buttonName;
        }
    }
}