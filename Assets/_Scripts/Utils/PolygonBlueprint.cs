using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Utils
{
    [Serializable]
    public class PolygonBlueprint
    {
        [SerializeField, Tooltip("Number of sides for the polygon. Must be at least 3.")]
        private int numberOfSides = 3;

        [SerializeField] private float radius = 2f;
        [SerializeField] private Transform centerPosition;
        [SerializeField] private CoordinatePlane polygonPlane = CoordinatePlane.XY;

        public List<Vector3> GetVertices()
        {
            var vertices = new List<Vector3>();

            if (numberOfSides < 3)
            {
                Debug.LogError("Polygon must have at least 3 sides.");
                return vertices;
            }

            float angleStep = 360f / numberOfSides;
            float rotationOffset = 90f;

            for (int i = 0; i < numberOfSides; i++)
            {
                float angle = angleStep * i + rotationOffset;
                float rad = angle * Mathf.Deg2Rad;

                // cos(rad) = X coordinate, sin(rad) = Y coordinate
                Vector3 localPos = polygonPlane switch
                {
                    CoordinatePlane.XY => new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius,
                    CoordinatePlane.XZ => new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius,
                    _ => Vector3.zero
                };

                vertices.Add(localPos);
            }

            return vertices;
        }
    }

    internal enum CoordinatePlane
    {
        XY,
        XZ
    }
}