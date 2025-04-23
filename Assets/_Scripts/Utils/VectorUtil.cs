using UnityEngine;

namespace _Scripts.Utils
{
    public static class VectorUtil
    {
        public static Vector2 ToVector2(Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

        public static Vector2Int ToVector2Int(Vector3Int vector3Int)
        {
            return new Vector2Int(vector3Int.x, vector3Int.y);
        }

        public static Vector2Int ToVector2Int(Vector3 vector3)
        {
            return new Vector2Int(Mathf.FloorToInt(vector3.x), Mathf.FloorToInt(vector3.y));
        }
        public static Vector3Int ToVector3Int(Vector2Int vector2Int)
        {
            return new Vector3Int(vector2Int.x, vector2Int.y, 0);
        }
        
        public static Vector3Int ToVector3Int(Vector3 vector3)
        {
            return Vector3Int.FloorToInt(vector3);
        }

        public static Vector3 ToVector3(Vector2 vector2)
        {
            return new Vector3(vector2.x, vector2.y, 0f);
        }
    }
}