using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    /// <summary>
    /// The material to be used for the ground. Make sure you've added the respective textures,
    /// and normals too!
    /// </summary>
    private SerializedProperty GroundMaterial;

    /// <summary>
    /// The size of the terrain on the x axis, as seen for top-down view.
    /// </summary>
    private SerializedProperty xSize;

    /// <summary>
    /// The size of the terrain on the z axis, as seen for top-down view.
    /// </summary>
    private SerializedProperty zSize;

    /// <summary>
    /// The number of subdivisions the terrain will have. The higher this number is
    /// the more detailed the terrain will be, as well as the number of triangles.
    /// </summary>
    private SerializedProperty Subdivisions;

    /// <summary>
    /// The lowest Y the terrain can have.
    /// </summary>
    private SerializedProperty minHeight;

    /// <summary>
    /// The highest Y the terrain can have.
    /// </summary>
    private SerializedProperty maxHeight;

    private SerializedProperty seed;

    float div;

    private GameObject terrainGameObject;

    private Vector3[] vertices;

    private int[] triangles;

    private float xSpace;
    private float zSpace;

    private Mesh mesh;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void OnEnable()
    {
        GroundMaterial = serializedObject.FindProperty("GroundMaterial");

        xSize = serializedObject.FindProperty("xSize");
        zSize = serializedObject.FindProperty("zSize");

        seed = serializedObject.FindProperty("seed");

        Subdivisions = serializedObject.FindProperty("Subdivisions");

        minHeight = serializedObject.FindProperty("minHeight");
        maxHeight = serializedObject.FindProperty("maxHeight");

        if (terrainGameObject == null)
        {
            terrainGameObject = GameObject.Find("Ultimate Terrain");

            if (!terrainGameObject)
            {
                terrainGameObject = new GameObject("Ultimate Terrain",
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(MeshCollider));
            }

            meshFilter = terrainGameObject.GetComponent<MeshFilter>();
            meshRenderer = terrainGameObject.GetComponent<MeshRenderer>();
            meshCollider = terrainGameObject.GetComponent<MeshCollider>();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(seed, new GUIContent("Seed"));

        EditorGUILayout.PropertyField(GroundMaterial, new GUIContent("Ground Material"));

        EditorGUILayout.Space();

        xSize.intValue = EditorGUILayout.DelayedIntField(new GUIContent("X-Axis"), xSize.intValue);
        zSize.intValue = EditorGUILayout.DelayedIntField(new GUIContent("Z-Axis"), zSize.intValue);

        EditorGUILayout.Space();

        Subdivisions.intValue = EditorGUILayout.DelayedIntField(new GUIContent("Subdivisions"), Subdivisions.intValue);

        EditorGUILayout.Space();

        minHeight.floatValue = EditorGUILayout.DelayedFloatField(new GUIContent("Minimum Height"), minHeight.floatValue);
        maxHeight.floatValue = EditorGUILayout.DelayedFloatField(new GUIContent("Maximum Height"), maxHeight.floatValue);

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();

        div = 1 / seed.floatValue;

        if (GUILayout.Button("Generate Terrain"))
        {
            GenerateTerrain();
        }

        EditorGUI.indentLevel--;
    }

    private void GenerateTerrain()
    {
        xSpace = (float) xSize.intValue / Subdivisions.intValue;
        //Debug.Log("X-Size Int:" + xSize.intValue);
        //Debug.Log("Subdivs Int:" + Subdivisions.intValue);
        //Debug.Log("X-Space:" + xSpace);

        zSpace = (float) zSize.intValue / Subdivisions.intValue;
        //Debug.Log("Z-Size Int:" + zSize.intValue);
        //Debug.Log("Subdivs Int:" + Subdivisions.intValue);
        //Debug.Log("Z-Space:" + zSpace);

        // First make the new vertex array
        CreateVertices();

        // Then create the needed triangles
        CreateTriangles();

        mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = (Material)GroundMaterial.objectReferenceValue;
        meshCollider.sharedMesh = mesh;

        mesh.RecalculateNormals();
    }

    private void CreateVertices()
    {
        vertices = new Vector3[(xSize.intValue * Subdivisions.intValue + 1) * (zSize.intValue * Subdivisions.intValue + 1)];
        Debug.Log("Vertices Length: " + vertices.Length);
        int currentIndex = 0;

        for (int z = 0; z <= zSize.intValue * Subdivisions.intValue; z++)
        {
            for (int x = 0; x <= xSize.intValue * Subdivisions.intValue; x++)
            {
                float xValue = x / (float)Subdivisions.intValue;
                float zValue = z / (float)Subdivisions.intValue;

                float y = Mathf.Clamp(Mathf.PerlinNoise(xValue, zValue) * 2.0f, minHeight.floatValue, maxHeight.floatValue);

                vertices[currentIndex] = new Vector3(xValue, y, zValue);
                //Debug.Log("Vertex #" + currentIndex + ": (X:" + vertices[currentIndex].x + ", Z:" + vertices[currentIndex].z + ")");
                currentIndex++;
            }
        }


        #region Old Code

        // float z = 0.0f;

        //while (z <= (zSize.intValue * Subdivisions.intValue - 1)) //!Mathf.Approximately(z, (float) zSize.intValue * Subdivisions.intValue - 1)
        //{
        //    float x = 0.0f;

        //    while (x <= (xSize.intValue * Subdivisions.intValue - 1)) //!Mathf.Approximately(x, (float)xSize.intValue * Subdivisions.intValue - 1)
        //    {
        //        Debug.LogFormat("Index: {0} // XZ: ({1}, {2})", currentIndex, x, z);
        //        float y = Mathf.Clamp(Mathf.PerlinNoise(x * div, z * div) * 2.0f, minHeight.floatValue, maxHeight.floatValue);
        //        vertices[currentIndex] = new Vector3(x, y, z);

        //        currentIndex++;
        //        x += xSpace;

        //    }

        //    z += zSpace;

        //} 
        #endregion

        #region Oldest Code
        //for (int z = 0; z <= zSize.intValue * Subdivisions.intValue; z++)
        //{
        //    for (int x = 0; x <= xSize.intValue * Subdivisions.intValue; x++)
        //    {
        //        float y = Mathf.Clamp(Mathf.PerlinNoise(x * div, z * div) * 2.0f, minHeight.floatValue, maxHeight.floatValue);
        //        vertices[currentIndex] = new Vector3(x, y, z);

        //        currentIndex++;
        //    }
        //}

        //// Then iterate through all needed vertices and set their position
        //for (int z = 0; z <= zSize.intValue; z++)
        //{
        //    for (int x = 0; x <= xSize.intValue; x++)
        //    {
        //        float height = Mathf.Clamp(Mathf.PerlinNoise(x * div, z * div) * 2.0f, minHeight.floatValue, maxHeight.floatValue);
        //        vertices[currentIndex] = new Vector3(x, height, z);

        //        currentIndex++;
        //    }
        //} 
        #endregion
    }


    private void CreateTriangles()
    {
        int neededTris = ((xSize.intValue * Subdivisions.intValue + 1) - 2) * zSize.intValue;
        triangles = new int[neededTris * 3]; //(xSize.intValue * Subdivisions.intValue + 1) * (zSize.intValue * Subdivisions.intValue + 1) * 3

        int currentVertex = 0;
        int currentTriangleIndex = 0;

        Debug.Log("Triangles Needed:" + neededTris + " -- Length: " + triangles.Length);

        for (int z = 0; z < zSize.intValue * Subdivisions.intValue; z++) //(int z = 0; z < zSize.intValue * Subdivisions.intValue; z++)
        {
            for (int x = 0; x < xSize.intValue * Subdivisions.intValue; x++) //(int x = 0; x < xSize.intValue * Subdivisions.intValue; x++)
            {
                Debug.Log(currentVertex + " -- " + currentTriangleIndex);

                triangles[currentTriangleIndex] = currentVertex;
                triangles[currentTriangleIndex + 1] = currentVertex + 1;
                triangles[currentTriangleIndex + 2] = currentVertex + xSize.intValue + Subdivisions.intValue;
                //triangles[numOfTriangles + 3] = currentVertex + 1;
                //triangles[numOfTriangles + 4] = currentVertex + xSize.intValue + zSize.intValue + Subdivisions.intValue;
                //triangles[numOfTriangles + 5] = currentVertex + zSize.intValue + Subdivisions.intValue;

                currentVertex++;
                currentTriangleIndex += 3; //6
            }

            currentVertex++;
        }

        //for (int z = 0; z <= zSize.intValue * Subdivisions.intValue - 1; z++)
        //{
        //    for (int x = 0; x <= xSize.intValue * Subdivisions.intValue - 1; x++)
        //    {
        //        Debug.LogFormat("Current X, Z: ({0}, {1})", x, z);

        //        // ****** THIS IS WORKING *********** 
        //        //triangles[numOfTriangles] = currentVertex;
        //        //triangles[numOfTriangles + 1] = currentVertex + 1;
        //        //triangles[numOfTriangles + 2] = currentVertex + xSize.intValue + Subdivisions.intValue;
        //        //triangles[numOfTriangles + 3] = currentVertex + 1;
        //        //triangles[numOfTriangles + 4] = currentVertex + xSize.intValue + zSize.intValue + Subdivisions.intValue;
        //        //triangles[numOfTriangles + 5] = currentVertex + zSize.intValue + Subdivisions.intValue;

        //        //currentVertex++;
        //        //numOfTriangles += 6;
        //    }

        //    currentVertex++;
        //}
    }
}
