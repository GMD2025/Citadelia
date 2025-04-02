using UnityEngine;

namespace _Scripts.ResourceSystem.Data
{
    // filename is default filename
    // menuname is the unit menu structure it will appear under when request to create it
    [CreateAssetMenu(fileName = "NewResource", menuName = "Data/Resources/Resource", order = 1)]
    public class ResourceData : ScriptableObject
    {
        public string resourceName;
        public Sprite icon;
        public int initialAmount;
        public int maxAmount;
    }
}