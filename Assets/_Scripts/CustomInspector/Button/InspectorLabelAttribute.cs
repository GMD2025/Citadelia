using System;

namespace _Scripts.CustomInspector
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class InspectorLabelAttribute : Attribute
    {
        public string Label;

        public InspectorLabelAttribute(string label)
        {
            Label = label;
        }
    }
}