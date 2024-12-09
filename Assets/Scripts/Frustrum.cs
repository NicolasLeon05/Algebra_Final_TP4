using System.Collections.Generic;
using UnityEngine;

public class FrustumCulling : MonoBehaviour
{
    public float baseWidth = 10f;
    public float baseHeight = 5f;
    public float fovAngle = 60f;
    public float drawingDistance = 100f;

    private float fovRadians;
    private float nearHipotenusa;
    private float nearAdjacent;

    private float farOpposite;
    private float farAdjacent;
    private float finalFrustrumHeight;

    private Camera mainCamera;
    private Plane[] frustumPlanes;

    Vector3 nearCenter;
    Vector3 farCenter;

    Vector3 point1;
    Vector3 point2;
    Vector3 point3;
    Vector3 point4;

    Vector3 point5;
    Vector3 point6;
    Vector3 point7;
    Vector3 point8;

    private void Start()
    {
        mainCamera = Camera.main;
        CalculateFrustumPlanes();
    }

    private void Update()
    {
        CalculateFrustumPlanes();
        PerformFrustumCulling();
        transform.position = mainCamera.transform.position;
    }

    private void CalculateFrustumPlanes()
    {
        if (mainCamera == null) return;

        Vector3 center = mainCamera.transform.position;

        fovRadians = fovAngle * Mathf.PI / 180.0f;
        nearHipotenusa = (baseWidth / 2) / Mathf.Sin(fovRadians / 2);
        nearAdjacent = nearHipotenusa * Mathf.Cos(fovRadians / 2);

        farAdjacent = drawingDistance + nearAdjacent;
        farOpposite = farAdjacent * Mathf.Tan(fovRadians / 2);

        finalFrustrumHeight = baseHeight * (farAdjacent / nearAdjacent);

        nearCenter = mainCamera.transform.position + mainCamera.transform.forward * nearAdjacent;
        farCenter = mainCamera.transform.position + mainCamera.transform.forward * drawingDistance;

        point1 = nearCenter + mainCamera.transform.up * (baseHeight / 2) - mainCamera.transform.right * (baseWidth / 2);
        point2 = nearCenter + mainCamera.transform.up * (baseHeight / 2) + mainCamera.transform.right * (baseWidth / 2);
        point3 = nearCenter - mainCamera.transform.up * (baseHeight / 2) + mainCamera.transform.right * (baseWidth / 2);
        point4 = nearCenter - mainCamera.transform.up * (baseHeight / 2) - mainCamera.transform.right * (baseWidth / 2);

        point5 = farCenter + mainCamera.transform.up * (finalFrustrumHeight / 2) - mainCamera.transform.right * (farOpposite);
        point6 = farCenter + mainCamera.transform.up * (finalFrustrumHeight / 2) + mainCamera.transform.right * (farOpposite);
        point7 = farCenter - mainCamera.transform.up * (finalFrustrumHeight / 2) + mainCamera.transform.right * (farOpposite);
        point8 = farCenter - mainCamera.transform.up * (finalFrustrumHeight / 2) - mainCamera.transform.right * (farOpposite);

        frustumPlanes = new Plane[6];
        frustumPlanes[0] = new Plane(point4, point2, point1); //Near
        frustumPlanes[1] = new Plane(point8, point5, point6); //Far
        frustumPlanes[2] = new Plane(point4, point1, point5); //Left
        frustumPlanes[3] = new Plane(point3, point6, point2); //Right
        frustumPlanes[4] = new Plane(point1, point2, point5); //Top
        frustumPlanes[5] = new Plane(point4, point8, point3); //Bottom
    }


    private void PerformFrustumCulling()
    {
        MeshRenderer[] meshRenderers = FindObjectsOfType<MeshRenderer>();

        foreach (MeshRenderer renderer in meshRenderers)
        {
            bool isVisible = IsObjectInFrustum(renderer);
            //renderer.gameObject.SetActive(isVisible);
            renderer.enabled = isVisible;
        }
    }

    private bool IsObjectInFrustum(MeshRenderer renderer)
    {
        Bounds bounds = renderer.bounds;

        if (!IsAABBInFrustum(bounds))
        {
            return false;
        }

        Vector3[] vertices = renderer.GetComponent<MeshFilter>().sharedMesh.vertices;

        foreach (Vector3 vertex in vertices)
        {
            Vector3 worldVertex = renderer.transform.TransformPoint(vertex);

            if (IsVertexInFrustum(worldVertex))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsAABBInFrustum(Bounds bounds)
    {
        foreach (var plane in frustumPlanes)
        {
            Vector3 p = bounds.center + Vector3.Scale(bounds.extents, new Vector3(
                plane.normal.x > 0 ? 1 : -1,
                plane.normal.y > 0 ? 1 : -1,
                plane.normal.z > 0 ? 1 : -1
            ));

            if (plane.GetDistanceToPoint(p) < 0)
            {
                return false;
            }
        }
        Debug.Log($"Bounding box adentro del frustum");
        return true;
    }

    private bool IsVertexInFrustum(Vector3 vertex)
    {
        foreach (var plane in frustumPlanes)
        {
            if (plane.GetDistanceToPoint(vertex) < 0)
            {
                return false;
            }
        }
        Debug.Log($"Vertice adentro del frustum");
        return true;
    }

    private void OnDrawGizmos()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 center = mainCamera.transform.position;

        // Base screen triangle
        fovRadians = fovAngle * Mathf.PI / 180.0f;
        nearHipotenusa = (baseWidth / 2) / Mathf.Sin(fovRadians / 2);
        nearAdjacent = nearHipotenusa * Mathf.Cos(fovRadians / 2);


        // Frustrum triangle
        farAdjacent = drawingDistance + nearAdjacent;
        farOpposite = farAdjacent * Mathf.Tan(fovRadians / 2);

        // Frustrum height
        finalFrustrumHeight = baseHeight * (farAdjacent / nearAdjacent);

        //Drawing distance
        Vector3 ddVector = center + new Vector3(0, 0, drawingDistance);
        Vector3 missingDdVector = center + new Vector3(0, 0, -nearAdjacent);

        Vector3 bigFrustrumHalfWidth = ddVector + new Vector3(farOpposite, 0, 0);

        //Base screen
        Gizmos.color = Color.red;
        Gizmos.DrawLine(point1, point2);
        Gizmos.DrawLine(point2, point3);
        Gizmos.DrawLine(point3, point4);
        Gizmos.DrawLine(point4, point1);

        // Frustrum
        Gizmos.color = Color.green;
        Gizmos.DrawLine(point5, point6);
        Gizmos.DrawLine(point6, point7);
        Gizmos.DrawLine(point7, point8);
        Gizmos.DrawLine(point8, point5);

        //Cone
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(point1, point5);
        Gizmos.DrawLine(point2, point6);
        Gizmos.DrawLine(point3, point7);
        Gizmos.DrawLine(point4, point8);

        //Drawing distance
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(center, 0.1f);
        Gizmos.DrawLine(center, ddVector);

        //Missing Drawing distance
        Gizmos.color = Color.green;
        Gizmos.DrawLine(center, missingDdVector);
        Gizmos.DrawSphere(ddVector, 0.1f);

        //Auxiliars
        Gizmos.color = Color.white;
        Gizmos.DrawLine(missingDdVector, bigFrustrumHalfWidth);
        Gizmos.DrawLine(ddVector, bigFrustrumHalfWidth);

    }
}