/*****************************************************************************
// File Name :         RiverSpline.cs
// Author :            Charlie Polonus
// Creation Date :     September 24, 2024
//
// Brief Description : Generates a rectangular mesh based on the bezier curve
                       provided from the BezierCurve class, to the detail
                       specified through the settings
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains methods able to generate a river mesh based around a bezier curve
/// </summary>
[RequireComponent(typeof(BezierCurve))]
public class RiverSpline : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshFilter _mesh;
    [SerializeField] private MeshCollider _collider;

    [Header("River Settings")]
    [Min(1)] [SerializeField] private float _width;
    [Min(0.001f)] [SerializeField] private float _idealVertexDistance;
    [Min(1)] [SerializeField] private int _riverSmoothness;

    /// <summary>
    /// Create a mesh based on the attached bezier curve and the settings degined by the user
    /// </summary>
    public void DrawMesh()
    {
        BezierCurve activeBezier = GetComponent<BezierCurve>();

        // Don't create a mesh if there isn't enough information to make one
        if (activeBezier.BezierPoints.Length < 2)
        {
            return;
        }

        Mesh mesh = new();

        // Get the position and derivatives of the attached bezier curve
        Vector3[] curvePositions = activeBezier.All3dPoints(_riverSmoothness);
        Vector3[] curveDerivatives = activeBezier.All3dDerivatives(_riverSmoothness);

        // Resize the desired vertex distance to match the width the best it can
        _idealVertexDistance = Mathf.Min(_idealVertexDistance, _width);
        float extraDistance = (_width % _idealVertexDistance) / Mathf.Floor(_width / _idealVertexDistance);
        float vertexDistance = _idealVertexDistance + extraDistance;
        int verticesPerPoint = Mathf.RoundToInt(_width / vertexDistance) + 1;

        // Set the size of the rectangle in vertices
        int sizeX = verticesPerPoint;
        int sizeY = _riverSmoothness * (activeBezier.BezierPoints.Length - 1) + 1;

        // Prepare the vertex and uv arrays
        Vector3[] vertices = new Vector3[sizeX * sizeY];
        Vector2[] uv = new Vector2[vertices.Length];

        // Prepare info about how many triangles/total size of triangles the area has
        int xTriangles = sizeX - 1;
        int yTriangles = sizeY - 1;
        int[] triangles = new int[xTriangles * yTriangles * 6];

        // Assign the vertices across the mesh
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                // Determine how far to the side the vertices need to go based on the width
                Vector3 normalDirection = Vector3.Cross(curveDerivatives[y].normalized, Vector3.up);
                float horizontalRatio = -((sizeX - 1) / 2f) + x;

                // Set the vertices and uv based on the given location information
                vertices[y * sizeX + x] = curvePositions[y] + (horizontalRatio * vertexDistance * normalDirection);
                uv[((y + 1) * sizeX) - x - 1] = new Vector2(x / (float)sizeX, y / (float)sizeY);
            }
        }

        // The order in which to read triangles, going back and forth from |\ triangles and \| triangles
        int[] squareTriangleIndices = new int[]
        {
            0, 1, sizeX,
            sizeX, 1, sizeX + 1
        };

        // Go through and add all of the necessary triangles to the array
        for (int y = 0; y < yTriangles; y++)
        {
            int yIndexOffset = xTriangles * 6 * y;
            int yValueOffset = y * sizeX;

            for (int x = 0; x < xTriangles; x++)
            {
                int xIndexOffset = x * 6;

                for (int i = 0; i < squareTriangleIndices.Length; i++)
                {
                    triangles[i + xIndexOffset + yIndexOffset] = squareTriangleIndices[i] + x + yValueOffset;
                }
            }
        }

        // Assign all the arrays to the temporary mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Set the actual mesh renderer and mesh collider to read from the temporary mesh
        _mesh.sharedMesh = mesh;
        _mesh.sharedMesh.RecalculateNormals();
        _collider.sharedMesh = null;
        _collider.sharedMesh = mesh;
    }

    /// <summary>
    /// Delete both the mesh collider and mesh renderer
    /// </summary>
    public void ResetMesh()
    {
        _mesh.sharedMesh = null;
        _collider.sharedMesh = null;
    }
}