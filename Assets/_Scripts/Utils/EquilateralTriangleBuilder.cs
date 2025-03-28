using _Scripts.CustomInspector.Button;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Utils
{
    [ExecuteAlways]
    public class EquilateralTriangleBuilder : MonoBehaviour
    {
        [Header("Triangle Points")] 
        [SerializeField] private GameObject point1GameObject;
        [SerializeField] private GameObject point2GameObject;
        [SerializeField] private GameObject point3GameObject;

        [Header("Settings")]
        [SerializeField] private Transform centerPosition;
        [SerializeField] private int size = 2;
        [SerializeField] private TrianglePlane trianglePlane = TrianglePlane.XY;

        // Cache the original local positions of the triangle points.
        private Vector3 cachedLeftTopLocalPosition;
        private Vector3 cachedRightTopLocalPosition;
        private Vector3 cachedCenterBottomLocalPosition;

        private void Start()
        {
            Position();
        }

        [InspectorButton("Position")]
        private void Position()
        {
            cachedLeftTopLocalPosition = point1GameObject.transform.localPosition;
            cachedRightTopLocalPosition = point2GameObject.transform.localPosition;
            cachedCenterBottomLocalPosition = point3GameObject.transform.localPosition;

            float triangleHeight = Mathf.Sqrt(3f) / 2f * size;

            Vector3 topVertex = new Vector3(0, 2f/3f * triangleHeight, 0);
            Vector3 bottomLeftVertex = new Vector3(-size / 2f, -triangleHeight / 3f, 0);
            Vector3 bottomRightVertex = new Vector3(size / 2f, -triangleHeight / 3f, 0);

            // if drawing in the XZ plane, swap the Y and Z coordinates.
            if (trianglePlane == TrianglePlane.XZ)
            {
                topVertex = new Vector3(topVertex.x, 0, topVertex.y);
                bottomLeftVertex = new Vector3(bottomLeftVertex.x, 0, bottomLeftVertex.y);
                bottomRightVertex = new Vector3(bottomRightVertex.x, 0, bottomRightVertex.y);
            }

            point1GameObject.transform.position = centerPosition.TransformPoint(topVertex);
            point2GameObject.transform.position = centerPosition.TransformPoint(bottomLeftVertex);
            point3GameObject.transform.position = centerPosition.TransformPoint(bottomRightVertex);
        }

        [InspectorButton("Reset Position")]
        private void ResetPosition()
        {
            point1GameObject.transform.localPosition = cachedLeftTopLocalPosition;
            point2GameObject.transform.localPosition = cachedRightTopLocalPosition;
            point3GameObject.transform.localPosition = cachedCenterBottomLocalPosition;
        }
    }

    internal enum TrianglePlane
    {
        XY,
        XZ
    }
}