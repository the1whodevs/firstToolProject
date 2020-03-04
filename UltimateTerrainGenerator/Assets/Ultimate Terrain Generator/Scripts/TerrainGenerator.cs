using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    /// <summary>
    /// The material to be used for the ground. Make sure you've added the respective textures,
    /// and normals too!
    /// </summary>
    [SerializeField] private Material GroundMaterial;

    /// <summary>
    /// The number of subdivisions the terrain will have. The higher this number is
    /// the more detailed the terrain will be, as well as the number of triangles.
    /// </summary>
    [SerializeField] private int Subdivisions;

    /// <summary>
    /// The highest Y the terrain can have.
    /// </summary>
    [SerializeField] private float heightMultiplier;

    /// <summary>
    /// The size of the terrain on the x axis, as seen from top-down view.
    /// </summary>
    [SerializeField] private int xSize;

    /// <summary>
    /// The size of the terrain on the z axis, as seen for top-down view.
    /// </summary>
    [SerializeField] private int zSize;

    /// <summary>
    /// A seed that is used to generate different terrains.
    /// </summary>
    [Range(0.1f, 5.0f)]
    [SerializeField] private float seed;

    /// <summary>
    /// If true, instead of making a mesh vertex by vertex, it positions and scales cubes on the 
    /// calculated vertex positions.
    /// </summary>
    [SerializeField] private bool doMinecraft = false;

    [SerializeField] private GameObject terrainGameObject;

    private Vector3[] vertices;

    private int[] triangles;
    private int trianglesMade;

    private int xEdges, zEdges, xVertices, zVertices;

    private Mesh mesh;

    public void GenerateTerrain(ref MeshFilter meshFilter, ref MeshRenderer meshRenderer, ref MeshCollider meshCollider, float div)
    {
        int c = terrainGameObject.transform.childCount;
        while (c > 0)
        {
            DestroyImmediate(terrainGameObject.transform.GetChild(c - 1).gameObject);
            c--;
        }

        // First make the new vertex array
        CreateVertices(div);

        // Then create the needed triangles
        CreateTriangles();

        Debug.LogFormat("For {0} vertices, I made {1} triangles.", vertices.Length, trianglesMade);

        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = GroundMaterial;
        meshCollider.sharedMesh = mesh;

        mesh.RecalculateNormals();
    }

    private void CreateVertices(float div)
    {
        int currentIndex = 0;

        float xValue, zValue, y;

        xEdges = xSize * Subdivisions;
        zEdges = zSize * Subdivisions;

        xVertices = xEdges + 1;
        zVertices = zEdges + 1;

        vertices = new Vector3[xVertices * zVertices];

        for (int z = 0; z <= zEdges; z++)
        {
            for (int x = 0; x <= xEdges; x++)
            {
                xValue = x / (float)Subdivisions;
                zValue = z / (float)Subdivisions;

                y = heightMultiplier * Mathf.PerlinNoise(xValue + div, zValue + div);

                vertices[currentIndex] = new Vector3(xValue, y, zValue);
                currentIndex++;
            }
        }

    }


    private void CreateTriangles()
    {
        trianglesMade = 0;

        int neededTris = xVertices * zVertices * 2;

        triangles = new int[neededTris * 3];

        for (int iz = 0; iz <= zEdges - 1; iz++)
        {
            for (int ix = 0; ix <= xEdges - 1; ix++)
            {
                int currentVertexIndex = iz * xVertices + ix;
                int nextVerteIndex = iz * xVertices + ix + 1;
                int vertexAboveMeIndex = (iz + 1) * xVertices + ix;
                int nextOfVertexAboveMeIndex = (iz + 1) * xVertices + ix + 1;

                int arrayIndex = (iz * zVertices + ix) * 6;

                triangles[arrayIndex + 0] = currentVertexIndex;
                triangles[arrayIndex + 1] = vertexAboveMeIndex;
                triangles[arrayIndex + 2] = nextVerteIndex;
                trianglesMade++;

                triangles[arrayIndex + 3] = nextVerteIndex;
                triangles[arrayIndex + 4] = vertexAboveMeIndex;
                triangles[arrayIndex + 5] = nextOfVertexAboveMeIndex;
                trianglesMade++;
            }
        }

    }

}
