using UnityEngine;

namespace _Scripts.Data
{
    [CreateAssetMenu(fileName = "ProjectConfig", menuName = "Scriptable Objects/ProjectConfig", order = -1)]
    public class ProjectConfig : ScriptableObject
    {
        [SerializeField] public string GameplaySceneName;
    }
}