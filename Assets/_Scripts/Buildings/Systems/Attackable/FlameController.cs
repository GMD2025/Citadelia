using UnityEngine;

namespace _Scripts.Buildings.Systems.Attackable
{
    public class FlameController: MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                // TODO: damage! (scriptable object)
                Destroy(this, 0.2f);
            }
        }
    }
}