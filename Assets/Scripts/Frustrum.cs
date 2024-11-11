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

    private void Start()
    {
        mainCamera = Camera.main;
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

        Vector3 nearCenter = center + new Vector3(0, 0, nearAdjacent);
        Vector3 farCenter = center + new Vector3(0, 0, drawingDistance);

        Vector3 nearTopLeft = nearCenter + new Vector3(-baseWidth / 2, baseHeight / 2, 0);
        Vector3 nearTopRight = nearCenter + new Vector3(baseWidth / 2, baseHeight / 2, 0);
        Vector3 nearBottomLeft = nearCenter + new Vector3(-baseWidth / 2, -baseHeight / 2, 0);
        Vector3 nearBottomRight = nearCenter + new Vector3(baseWidth / 2, -baseHeight / 2, 0);

        Vector3 farTopLeft = farCenter + new Vector3(-farOpposite, finalFrustrumHeight / 2, 0);
        Vector3 farTopRight = farCenter + new Vector3(farOpposite, finalFrustrumHeight / 2, 0);
        Vector3 farBottomLeft = farCenter + new Vector3(-farOpposite, -finalFrustrumHeight / 2, 0);
        Vector3 farBottomRight = farCenter + new Vector3(farOpposite, -finalFrustrumHeight / 2, 0);

        frustumPlanes = new Plane[6];
        frustumPlanes[0] = new Plane(nearBottomLeft, nearBottomRight, nearTopLeft);  // Near
        frustumPlanes[1] = new Plane(farBottomLeft, farTopLeft, farBottomRight);     // Far
        frustumPlanes[2] = new Plane(nearBottomLeft, farBottomLeft, nearTopLeft);    // Left
        frustumPlanes[3] = new Plane(nearBottomRight, nearTopRight, farBottomRight); // Right
        frustumPlanes[4] = new Plane(nearTopLeft, nearTopRight, farTopLeft);         // Top
        frustumPlanes[5] = new Plane(nearBottomLeft, farBottomRight, nearBottomRight); // Bottom
    }

    private void PerformFrustumCulling()
    {
        MeshRenderer[] meshRenderers = FindObjectsOfType<MeshRenderer>();

        foreach (MeshRenderer renderer in meshRenderers)
        {
            bool isVisible = IsObjectInFrustum(renderer);
            renderer.gameObject.SetActive(isVisible);

            // Debug para cada objeto
            if (isVisible)
            {
                Debug.Log($"[VISIBLE] {renderer.gameObject.name}");
            }
            else
            {
                Debug.Log($"[INVISIBLE] {renderer.gameObject.name}");
            }
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


        // Base screen measures
        Vector3 baseTopLeft = center + new Vector3(-baseWidth / 2, baseHeight / 2, 0);
        Vector3 baseTopRight = center + new Vector3(baseWidth / 2, baseHeight / 2, 0);
        Vector3 baseBottomLeft = center + new Vector3(-baseWidth / 2, -baseHeight / 2, 0);
        Vector3 baseBottomRight = center + new Vector3(baseWidth / 2, -baseHeight / 2, 0);

        //  Frustrum measures
        Vector3 farTopLeft = center + new Vector3(-farOpposite, finalFrustrumHeight / 2, drawingDistance);
        Vector3 farTopRight = center + new Vector3(farOpposite, finalFrustrumHeight / 2, drawingDistance);
        Vector3 farBottomLeft = center + new Vector3(-farOpposite, -finalFrustrumHeight / 2, drawingDistance);
        Vector3 farBottomRight = center + new Vector3(farOpposite, -finalFrustrumHeight / 2, drawingDistance);


        //Drawing distance
        Vector3 ddVector = center + new Vector3(0, 0, drawingDistance);
        Vector3 missingDdVector = center + new Vector3(0, 0, -nearAdjacent);

        Vector3 bigFrustrumHalfWidth = ddVector + new Vector3(farOpposite, 0, 0);

        //Base screen
        Gizmos.color = Color.red;
        Gizmos.DrawLine(baseTopLeft, baseTopRight);
        Gizmos.DrawLine(baseTopRight, baseBottomRight);
        Gizmos.DrawLine(baseBottomRight, baseBottomLeft);
        Gizmos.DrawLine(baseBottomLeft, baseTopLeft);

        // Frustrum
        Gizmos.color = Color.green;
        Gizmos.DrawLine(farTopLeft, farTopRight);
        Gizmos.DrawLine(farTopRight, farBottomRight);
        Gizmos.DrawLine(farBottomRight, farBottomLeft);
        Gizmos.DrawLine(farBottomLeft, farTopLeft);

        //Cone
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(baseTopLeft, farTopLeft);
        Gizmos.DrawLine(baseTopRight, farTopRight);
        Gizmos.DrawLine(baseBottomRight, farBottomRight);
        Gizmos.DrawLine(baseBottomLeft, farBottomLeft);

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

        CalculateFrustumPlanes();

        if (frustumPlanes == null) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < frustumPlanes.Length; i++)
        {
            Plane plane = frustumPlanes[i];
            Vector3 planePosition = -plane.normal * plane.distance;
            Gizmos.DrawSphere(planePosition, 0.3f);
            Gizmos.DrawLine(Vector3.zero, plane.normal * 5f); // Debug normals
        }
    }
}