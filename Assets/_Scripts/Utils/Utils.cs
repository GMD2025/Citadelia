using UnityEngine;

namespace _Scripts.Utils
{
    public class Utils
    {
        //destroys object both in the play, build, and editor modes
        public static void SmartDestroy(Object obj)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                Object.DestroyImmediate(obj);
            else
                Object.Destroy(obj);
#else
    Object.Destroy(obj);
#endif
        }
    }
}