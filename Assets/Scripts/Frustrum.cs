using System.Reflection;
using UnityEngine;

public class Frustrum : MonoBehaviour
{
    public float baseWidth;
    public float baseHeight;
    public float fovAngle;

    public float drawingDistance;

    private float fovRadians;
    private float nearHipotenusa;
    private float nearAdjacent;

    private float farOpposite;
    private float farAdjacent;
    private float finalFrustrumHeight;

    // Update is called once per frame
    private void OnDrawGizmos()
    {
        Vector3 center = transform.position;

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

    }
}