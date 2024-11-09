using System.Diagnostics;
using UnityEngine;

[ExecuteInEditMode]
public class ManualBoundingBoxCalculator : MonoBehaviour
{
    private Bounds bounds;
    private bool isColliding = false; // Variable para almacenar el estado de colisión actual

    void OnDrawGizmos()
    {
        // Obtiene el componente MeshFilter del objeto
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            // Obtiene los vértices del mesh
            Vector3[] vertices = meshFilter.sharedMesh.vertices;

            // Inicializa el vector mínimo y máximo
            Vector3 min = transform.TransformPoint(vertices[0]);
            Vector3 max = min;

            // Recorre los vértices para encontrar los límites
            foreach (Vector3 vertex in vertices)
            {
                Vector3 worldVertex = transform.TransformPoint(vertex);
                min = Vector3.Min(min, worldVertex);
                max = Vector3.Max(max, worldVertex);
            }

            // Crea la AABB a partir de los límites encontrados
            bounds = new Bounds();
            bounds.SetMinMax(min, max);

            // Configura el color para el Gizmo
            Gizmos.color = Color.green;

            // Dibuja la Bounding Box en la escena
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }

}