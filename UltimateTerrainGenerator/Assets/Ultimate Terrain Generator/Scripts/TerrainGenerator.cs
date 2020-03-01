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

    private List<int> triList = new List<int>();

    private int[] triangles;
    private int trianglesMade;

    private float xSpace;
    private float zSpace;

    private Mesh mesh;

    public void GenerateTerrain(ref MeshFilter meshFilter, ref MeshRenderer meshRenderer, ref MeshCollider meshCollider, float div)
    {
        int c = terrainGameObject.transform.childCount;
        while (c > 0)
        {
            DestroyImmediate(terrainGameObject.transform.GetChild(c - 1).gameObject);
            c--;
        }

        xSpace = (float)xSize / (Subdivisions * xSize);
        zSpace = (float)zSize / (Subdivisions * zSize);


        // First make the new vertex array
        CreateVertices(div);

        // Then create the needed triangles
        CreateTriangles();

        Debug.LogFormat("For {0} vertices, I made {1} triangles.", vertices.Length, trianglesMade);

        mesh = new Mesh();

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

        vertices = new Vector3[(xSize * Subdivisions + 1) * (zSize * Subdivisions + 1)];

        for (int z = 0; z <= zSize * Subdivisions; z++)
        {
            for (int x = 0; x <= xSize * Subdivisions; x++)
            {
                xValue = x / (float)Subdivisions;
                zValue = z / (float)Subdivisions;

                y = heightMultiplier * Mathf.PerlinNoise(xValue + div, zValue + div);

                vertices[currentIndex] = new Vector3(xValue, y, zValue);

                //// **** TEMP ****
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.transform.position = vertices[currentIndex];
                //g.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                //g.transform.SetParent(terrainGameObject.transform, true);
                //g.name = currentIndex.ToString();
                //// **** TEMP ****

                currentIndex++;
            }
        }

    }


    private void CreateTriangles()
    {
        trianglesMade = 0;
        triList.Clear();

        int neededTris = (xSize * Subdivisions) * (zSize * Subdivisions) * 2;

        Debug.Log("Needed tris: " + neededTris);
        triangles = new int[neededTris * 6];

        /*
        triList.Add(iz * (zSize * Subdivisions) + ix); // BL
        triList.Add((iz + 1) * (xSize * Subdivisions) + ix); // Above me
        triList.Add(iz * (zSize * Subdivisions) + 1); //next

        triList.Add(iz * (zSize * Subdivisions) + 1); //next
        triList.Add((iz + 1) * (xSize * Subdivisions) + ix); // Above me
        triList.Add((iz + 1) * (xSize * Subdivisions) + ix + 1);
        */

        int currentIndex = 0;

        for (int iz = 0; iz <= (zSize * Subdivisions) - 1; iz++)
        {
            for (int ix = 0; ix <= (xSize * Subdivisions) - 1; ix++)
            {
                

                /*
                int A = iy * vertsPerAxis + ix;
                int B = iy * vertsPerAxis + ix + 1;
                int C = (iy + 1) * vertsPerAxis + ix;
                int D = (iy + 1) * vertsPerAxis + ix + 1;
                int i = (iy * (vertsPerAxis - 1) + ix) * 6;

                triangles[i + 0] = A;
                triangles[i + 1] = C;
                triangles[i + 2] = B;
                triangles[i + 3] = B;
                triangles[i + 4] = C;
                triangles[i + 5] = D;
                */

                int a = iz * (xSize * Subdivisions + 1) + ix;
                int b = iz * (xSize * Subdivisions + 1) + ix + 1;
                int c = (iz + 1) * (xSize * Subdivisions + 1) + ix;
                int d = (iz + 1) * (xSize * Subdivisions + 1) + ix + 1;
                int i = (iz * (zSize * Subdivisions) + ix) * 6;
                Debug.Log("~~~~~~~~~~~~ I: " + i);

                triangles[i + 0] = a;
                triangles[i + 1] = c;
                triangles[i + 2] = b;

                triangles[i + 3] = b;
                triangles[i + 4] = c;
                triangles[i + 5] = d;

                Debug.Log("I0: " + a);
                Debug.Log("I1: " + c);
                Debug.Log("I2: " + b);
                Debug.Log("I3: " + b);
                Debug.Log("I4: " + c);
                Debug.Log("I5: " + d);
            }
        }

        #region Old Implementation
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    // Returns true if a triangle is made. False if not.
        //    // Used for customizing the code to the user's need.
        //    bool BL = TryMakingBLTriangle(i);
        //    bool TR = TryMakingTRTriangle(i);

        //    #region Example Returned Bool Usage

        //    /* An example of how the bool returned values could be used.
        //         * In this case, it generates "walls" around the terrain.
        //         */
                //    //if (!BL || !TR)
                //    //{
                //    //    GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    //    g.name = vertices[i].ToString();
                //    //    g.transform.localScale = new Vector3(xSpace, 100.0f, zSpace);
                //    //    g.transform.position = vertices[i];
                //    //}

                //    #endregion
                //} 
#endregion
            }

            #region Old Implementation
            ///// <summary>
            ///// Try making a Bottom-Left triangle with the vertice at the 'currentIndex'
            ///// being the start, positioned at the Bottom-Left of the triangle to be made.
            ///// </summary>
            ///// <param name="currentIndex"></param>
            //private bool TryMakingBLTriangle(int currentIndex)
            //{
            //    // If we're at the last index, index + 1 throws IndexOutOfRange!
            //    if (currentIndex >= vertices.Length - 1)
            //    {
            //        return false;
            //    }

            //    Vector3 currentVertex = vertices[currentIndex];
            //    Vector3 nextVertex = vertices[currentIndex + 1];

            //    int vertexAboveMeIndex = FindVertexAt(currentVertex.x, currentVertex.z + zSpace);

            //    // For a BL triangle to be made:
            //    // --> A vertex exists at the position (currentVertex.x, currentVertex.z + zSpace)
            //    if (vertexAboveMeIndex == -1)
            //    {
            //        return false;
            //    }

            //    // --> The next vertex must have pos.z == to the current vertex
            //    // --> The next vertex must have pos.x == currentVertex.x + xSpace
            //    if (currentVertex.z != nextVertex.z)
            //    {
            //        return false;
            //    }

            //    float neededNextX = currentVertex.x + xSpace;
            //    if (!Mathf.Approximately(nextVertex.x, neededNextX))
            //    {
            //        return false;
            //    }

            //    triList.Add(currentIndex);
            //    triList.Add(vertexAboveMeIndex);
            //    triList.Add(currentIndex + 1);

            //    trianglesMade++;
            //    return true;
            //}

            ///// <summary>
            ///// Try making a Top-Right triangle with the vertice at the 'currentIndex'
            ///// being the start, positioned at the Top-Right of the triangle to be made.
            ///// </summary>
            ///// <param name="currentIndex"></param>
            //private bool TryMakingTRTriangle(int currentIndex)
            //{
            //    // If we're at 0, currentIndex - 1 throws IndexOutOfRange!
            //    if (currentIndex == 0)
            //    {
            //        return false;
            //    }

            //    Vector3 currentVertex = vertices[currentIndex];
            //    Vector3 previousVertex = vertices[currentIndex - 1];

            //    int vertexBelowMeIndex = FindVertexAt(currentVertex.x, currentVertex.z - zSpace);

            //    // For a TR triangle to be made:
            //    // --> A vertex exists at the position (currentVertex.x, currentVertex.z - zSpace)
            //    if (vertexBelowMeIndex == -1)
            //    {
            //        return false;
            //    }

            //    // --> The previous vertex must have pos.z == to the current vertex
            //    // --> The previous vertex must have pos.x == currentVertex.x - xSpace
            //    if (currentVertex.z != previousVertex.z)
            //    {
            //        return false;
            //    }

            //    float neededPreviousX = currentVertex.x - xSpace;
            //    if (!Mathf.Approximately(previousVertex.x, neededPreviousX))
            //    {
            //        return false;
            //    }

            //    triList.Add(currentIndex);
            //    triList.Add(vertexBelowMeIndex);
            //    triList.Add(currentIndex - 1);

            //    trianglesMade++;
            //    return true;
            //}

            ///// <summary>
            ///// Returns the index of the first vertex found in the 'vertices' array,
            ///// with the given x and z position (y is ignored). If no vertex is found,
            ///// returns -1 instead.
            ///// </summary>
            ///// <param name="x"></param>
            ///// <param name="z"></param>
            ///// <returns></returns>
            //private int FindVertexAt(float x, float z)
            //{
            //    if (verticesDict.ContainsKey(new Vector2(x, z)))
            //    {
            //        // Debug.Log("Found");
            //        return verticesDict[new Vector2(x, z)];
            //    }

            //    // Debug.Log("Not found");
            //    return -1;

            //    // Old way of checking if a vertex at (x, z) exists.
            //    //for (int i = 0; i < vertices.Length; i++)
            //    //{
            //    //    if (Mathf.Approximately(vertices[i].x, x) && Mathf.Approximately(vertices[i].z, z))
            //    //    {
            //    //        return i;
            //    //    }
            //    //}

            //} 
            #endregion
        }
