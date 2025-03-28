using _Scripts.CustomInspector.Button;
using UnityEngine;

namespace _Scripts.Utils
{
    [ExecuteAlways]
    public class EquilateralTriangleBuilder : MonoBehaviour
    {
        [Header("Triangle Points")] 
        [SerializeField] private GameObject leftTop;
        [SerializeField] private GameObject rightTop;
        [SerializeField] private GameObject centerBottom;

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
            cachedLeftTopLocalPosition = leftTop.transform.localPosition;
            cachedRightTopLocalPosition = rightTop.transform.localPosition;
            cachedCenterBottomLocalPosition = centerBottom.transform.localPosition;

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

            leftTop.transform.position = centerPosition.TransformPoint(topVertex);
            rightTop.transform.position = centerPosition.TransformPoint(bottomLeftVertex);
            centerBottom.transform.position = centerPosition.TransformPoint(bottomRightVertex);
        }

        [InspectorButton("Reset Position")]
        private void ResetPosition()
        {
            leftTop.transform.localPosition = cachedLeftTopLocalPosition;
            rightTop.transform.localPosition = cachedRightTopLocalPosition;
            centerBottom.transform.localPosition = cachedCenterBottomLocalPosition;
        }
    }

    internal enum TrianglePlane
    {
        XY,
        XZ
    }
}