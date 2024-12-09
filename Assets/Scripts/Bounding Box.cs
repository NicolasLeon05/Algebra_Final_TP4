using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ManualBoundingBoxCalculator : MonoBehaviour
{
    private Bounds bounds;
    private List<Vector3> worldVertices = new List<Vector3>();

    void OnDrawGizmos()
    {
        UpdateBoundingBox();
        // Dibuja los Gizmos de la bounding box y vértices
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        Gizmos.color = Color.red;
        foreach (Vector3 worldVertex in worldVertices)
        {
            Gizmos.DrawSphere(worldVertex, 0.5f);
        }
    }

    private void UpdateBoundingBox()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            worldVertices.Clear();

            Vector3 min = transform.TransformPoint(vertices[0]);
            Vector3 max = min;

            foreach (Vector3 vertex in vertices)
            {
                Vector3 worldVertex = transform.TransformPoint(vertex);
                worldVertices.Add(worldVertex);
                min = Vector3.Min(min, worldVertex);
                max = Vector3.Max(max, worldVertex);
            }

            bounds = new Bounds();
            bounds.SetMinMax(min, max);
        }
    }

    /*public Bounds GetBounds()
    {
        UpdateBoundingBox();
        return bounds;
    }
    

    public List<Vector3> GetWorldVertices()
    {
        return new List<Vector3>(worldVertices);
    }
   
    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
     */
}
