using System;
using UnityEngine;

namespace _Scripts.Utils
{
    public class RotationBehaviour : MonoBehaviour
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private Axis axis = Axis.X;
        [SerializeField, Tooltip("Use local space for rotation")] private bool isLocal;

        private void Update()
        {
            Vector3 rotationAxis = axis switch
            {
                Axis.X => Vector3.right,
                Axis.Y => Vector3.up,
                Axis.Z => Vector3.forward,
                _ => Vector3.up
            };

            Space space = isLocal ? Space.Self : Space.World;
            transform.Rotate(rotationAxis, speed * Time.deltaTime, space);
        }
    }

    internal enum Axis
    {
        X,
        Y,
        Z
    }
}