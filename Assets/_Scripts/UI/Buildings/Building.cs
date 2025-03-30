using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Buildings")]
public class Building : ScriptableObject
{
    public Sprite sprite;
    public string name;
    public Vector2Int cellsize;
}
