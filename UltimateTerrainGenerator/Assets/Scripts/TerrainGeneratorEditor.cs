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

        //seed.intValue = EditorGUILayout.DelayedIntField(new GUIContent("Seed"), seed.intValue);
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
        // First make the new vertex array
        CreateVertices();

        // Then create the needed triangles
        CreateTriangles();

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = (Material)GroundMaterial.objectReferenceValue;
        meshCollider.sharedMesh = mesh;

        mesh.RecalculateNormals();
    }

    private void CreateVertices()
    {
        vertices = new Vector3[(xSize.intValue + 1) * (zSize.intValue + 1)];

        int currentIndex = 0;

        // Then iterate through all needed vertices and set their position
        for (int z = 0; z <= zSize.intValue; z++)
        {
            for (int x = 0; x <= xSize.intValue; x++)
            {
                float height = Mathf.PerlinNoise(x * div, z * div) * 2.0f;
                vertices[currentIndex] = new Vector3(x, height, z);

                currentIndex++;
            }
        }
    }


    private void CreateTriangles()
    {
        triangles = new int[xSize.intValue * zSize.intValue * 6];

        int currentVertex = 0;
        int numOfTriangles = 0;

        for (int z = 0; z < zSize.intValue; z++)
        {
            for (int x = 0; x < xSize.intValue; x++)
            {

                triangles[numOfTriangles] = currentVertex + 0;
                triangles[numOfTriangles + 1] = currentVertex + xSize.intValue + 1;
                triangles[numOfTriangles + 2] = currentVertex + 1;
                triangles[numOfTriangles + 3] = currentVertex + 1;
                triangles[numOfTriangles + 4] = currentVertex + xSize.intValue + 1;
                triangles[numOfTriangles + 5] = currentVertex + xSize.intValue + 2;

                currentVertex++;
                numOfTriangles += 6;
            }

            currentVertex++;
        }
    }
}
