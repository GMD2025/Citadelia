using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Scripts.CustomInspector.Button;

namespace _Scripts.Utils
{
    public class PolygonBuilder : MonoBehaviour
    {
        [Header("Polygon Settings")]
        [SerializeField] private PolygonBlueprint polygon = new();

        [Header("Vertex Instance Settings")]
        [SerializeField] private GameObject pointPrefab;

        private List<PolygonVertex> vertices = new();

        private void Start()
        {
            BuildPolygon();
        }

        [InspectorButton("Build Polygon")]
        private void BuildPolygon()
        {
            ClearInstances();
            vertices.Clear();

            var positions = polygon.GetVertices();
            foreach (var pos in positions)
            {
                vertices.Add(new PolygonVertex
                {
                    position = pos,
                    instance = null
                });
            }

            FillMissingInstances();
        }

        public void FillMissingInstances(int numberToFill = Int32.MaxValue)
        {
            if (!pointPrefab)
            {
                Debug.LogError("Point Prefab is not assigned.");
                return;
            }

            int filled = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].instance == null)
                {
                    Vector3 worldPos = transform.TransformPoint(vertices[i].position);
                    GameObject instance = Instantiate(pointPrefab, worldPos, Quaternion.identity, transform);
                    instance.name = $"Point {i + 1}";

                    vertices[i].instance = instance;

                    filled++;
                    if (filled >= numberToFill)
                        break;
                }
            }
        }

        public void NotifyVertexInstanceRemoved(Transform instanceTransform)
        {
            int index = vertices.FindIndex(v =>
                v.instance && v.instance.GetInstanceID() == instanceTransform.gameObject.GetInstanceID());

            if (index >= 0)
            {
                vertices[index].instance = null;
            }
            else
            {
                Debug.LogWarning($"[PolygonInstancePlacer] No matching vertex found for {instanceTransform.name}");
            }
        }

        [InspectorButton("Clear Instances")]
        private void ClearInstances()
        {
            foreach (var vertex in vertices)
            {
                if (vertex.instance != null)
                    Utils.SmartDestroy(vertex.instance);
            }
            vertices.Clear();
        }
    }

    [Serializable]
    public class PolygonVertex
    {
        public Vector3 position;
        public GameObject instance;
    }
}
