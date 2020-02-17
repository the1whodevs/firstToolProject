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

    ///// <summary>
    ///// Ideally used for top-down view, this should be the top-left corner of the terrain.
    ///// </summary>
    //private SerializedProperty TopLeftCorner;

    ///// <summary>
    ///// Ideally used for top-down view, this should be the top-right corner of the terrain.
    ///// </summary>
    //private SerializedProperty TopRightCorner;

    ///// <summary>
    ///// Ideally used for top-down view, this should be the bottom-left corner of the terrain.
    ///// </summary>
    //private SerializedProperty BottomLeftCorner;

    ///// <summary>
    ///// Ideally used for top-down view, this should be the bottom-right corner of the terrain.
    ///// </summary>
    //private SerializedProperty BottomRightCorner;

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

    //bool cornerFoldout = true;

    private GameObject terrainGameObject;

    private Mesh mesh;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void OnEnable()
    {
        GroundMaterial = serializedObject.FindProperty("GroundMaterial");

        //TopLeftCorner = serializedObject.FindProperty("TopLeftCorner");
        //TopRightCorner = serializedObject.FindProperty("TopRightCorner");
        //BottomLeftCorner = serializedObject.FindProperty("BottomLeftCorner");
        //BottomRightCorner = serializedObject.FindProperty("BottomRightCorner");

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

        EditorGUILayout.PropertyField(GroundMaterial, new GUIContent("Ground Material"));

        EditorGUILayout.Space();

        Subdivisions.intValue = EditorGUILayout.DelayedIntField(new GUIContent("Subdivisions"), Subdivisions.intValue);

        EditorGUILayout.Space();

        minHeight.floatValue = EditorGUILayout.DelayedFloatField(new GUIContent("Minimum Height"), minHeight.floatValue);
        maxHeight.floatValue = EditorGUILayout.DelayedFloatField(new GUIContent("Maximum Height"), maxHeight.floatValue);

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Generate Terrain"))
        {
            GenerateTerrain();
        }

        EditorGUI.indentLevel--;
    }

    private void GenerateTerrain()
    {
        mesh = new Mesh();

        int tilesPerAxis = Subdivisions.intValue;
        int vertsPerAxis = tilesPerAxis + 1;

        int numVerts = vertsPerAxis * vertsPerAxis;
        int numTris = tilesPerAxis * tilesPerAxis * 2;

        float tileWidth = 2.0f / tilesPerAxis;

        Vector3[] vertices = new Vector3[numVerts];

        for (int ix = 0; ix <= vertsPerAxis - 1; ix++)
        {
            for (int iy = 0; iy <= vertsPerAxis - 1; iy++)
            {
                float x = tileWidth * ix - 1;
                float y = tileWidth * iy - 1;

                float h = Random.Range(minHeight.floatValue, maxHeight.floatValue);
                //float h = Mathf.PerlinNoise(x, y) * height;

                Vector3 vert = new Vector3(x, h, y);

                int i = iy * vertsPerAxis + ix;
                vertices[i] = vert;
            }
        }

        mesh.vertices = vertices;

        int[] triangles = new int[numTris * 6];

        for (int iy = 0; iy <= vertsPerAxis - 2; iy++)
        {
            for (int ix = 0; ix <= vertsPerAxis - 2; ix++)
            {
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
            }
        }

        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshRenderer.sharedMaterial = (Material)GroundMaterial.objectReferenceValue;
        meshCollider.sharedMesh = mesh;
    }
}
