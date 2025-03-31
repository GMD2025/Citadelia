using System.Collections;
using UnityEngine;

namespace _Scripts.Utils
{
    public class DestroySelfController: MonoBehaviour
    {
        [SerializeField] private float timeOnScene = 7f;

        public IEnumerator Start()
        {
            yield return new WaitForSeconds(timeOnScene);
            Destroy(gameObject);
        }
    }
}