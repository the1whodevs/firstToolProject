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

    [SerializeField] private int xGridSize, zGridSize;

    [SerializeField] private float[] gridMultipliers;

    private Mesh mesh;

    public void GenerateTerrain(ref MeshFilter meshFilter, ref MeshRenderer meshRenderer, ref MeshCollider meshCollider, float div)
    {
        // First make the new vertex array
        CreateVertices(div);

        // Then create the needed triangles
        CreateTriangles();

        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.SetVertices(vertices);
        // mesh.vertices = vertices;
        mesh.SetTriangles(triangles, 0);
        // mesh.triangles = triangles;

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = GroundMaterial;
        meshCollider.sharedMesh = mesh;

        mesh.RecalculateNormals();
    }

    public void InitializeGridMultipliersLength()
    {
        gridMultipliers = new float[xGridSize * zGridSize];
    }

    private void CreateVertices(float div)
    {
        int currentIndex = 0;

        float xValue, zValue, y;

        xEdges = xSize * Subdivisions;
        zEdges = zSize * Subdivisions;

        Debug.LogFormat("X: {0} - Z:{1}", xEdges, zEdges);

        xVertices = xEdges + 1;
        zVertices = zEdges + 1;

        vertices = new Vector3[xVertices * zVertices];

        float PerlinMultiplier;

        for (int z = 0; z <= zEdges; z++)
        {
            for (int x = 0; x <= xEdges; x++)
            {
                xValue = x / (float)Subdivisions;
                zValue = z / (float)Subdivisions;

                int gridIndex = GetGridIndex(xValue, zValue);
                PerlinMultiplier = gridMultipliers[gridIndex];
                Debug.Log(PerlinMultiplier);
                y = PerlinMultiplier * heightMultiplier * Mathf.PerlinNoise(xValue + div, zValue + div);

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

    private int GetGridIndex(float x, float z)
    {
        int indexToReturn, column = -1, row = -1;

        // First find the column
        for (int ix = xGridSize; ix >= 1 ; ix--)
        {
            if (x < xEdges / ix)
            {
                //Debug.LogFormat("X: {0} - Div: {1}", x, (xEdges / ix));
                Debug.Log("x1");
                column = xGridSize - ix;
                break;
            }
            else if (Mathf.Approximately(x, xEdges / ix))
            {
                //Debug.LogFormat("X: {0} - Div: {1}", x, (xEdges / ix));
                Debug.Log("x2");
                column = xGridSize - ix + 1;
                break;
            }
        }

        // Then find the row
        for (int iz = zGridSize; iz >= 1; iz--)
        {
            if (z < zEdges / iz)
            {
                //Debug.LogFormat("Z: {0} - Div: {1}", z, (zEdges / iz));
                Debug.Log("z1");
                row = zGridSize - iz;
                break;
            }
            else if (Mathf.Approximately(z, zEdges / iz))
            {
                //Debug.LogFormat("Z: {0} - Div: {1}", z, (zEdges / iz));
                Debug.Log("z2");
                row = zGridSize - iz + 1;
                break;
            }
        }

        if (column >= xGridSize)
        {
            column = xGridSize - 1;
        }

        if (row >= zGridSize)
        {
            row = zGridSize - 1;
        }

        indexToReturn = column + (row * xGridSize);

        Debug.LogFormat("XZ: {0}, {1} - Column: {2} - Row: {3} - Index: {4}", x, z, column, row, indexToReturn);
        return indexToReturn;
    }

}
