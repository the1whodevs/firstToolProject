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
    /// Ideally used for top-down view, this should be the top-left corner of the terrain.
    /// </summary>
    private SerializedProperty TopLeftCorner;

    /// <summary>
    /// Ideally used for top-down view, this should be the top-right corner of the terrain.
    /// </summary>
    private SerializedProperty TopRightCorner;

    /// <summary>
    /// Ideally used for top-down view, this should be the bottom-left corner of the terrain.
    /// </summary>
    private SerializedProperty BottomLeftCorner;

    /// <summary>
    /// Ideally used for top-down view, this should be the bottom-right corner of the terrain.
    /// </summary>
    private SerializedProperty BottomRightCorner;

    /// <summary>
    /// The number of subdivisions the terrain will have. The higher this number is
    /// the more detailed the terrain will be, as well as the number of triangles.
    /// </summary>
    private SerializedProperty Smoothness;

    /// <summary>
    /// The lowest Y the terrain can have.
    /// </summary>
    private SerializedProperty minHeight;

    /// <summary>
    /// The highest Y the terrain can have.
    /// </summary>
    private SerializedProperty maxHeight;

    bool cornerFoldout = true;

    private GameObject terrainGameObject;

    private Mesh mesh;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void OnEnable()
    {
        GroundMaterial = serializedObject.FindProperty("GroundMaterial");

        TopLeftCorner = serializedObject.FindProperty("TopLeftCorner");
        TopRightCorner = serializedObject.FindProperty("TopRightCorner");
        BottomLeftCorner = serializedObject.FindProperty("BottomLeftCorner");
        BottomRightCorner = serializedObject.FindProperty("BottomRightCorner");

        Smoothness = serializedObject.FindProperty("Smoothness");

        minHeight = serializedObject.FindProperty("minHeight");
        maxHeight = serializedObject.FindProperty("maxHeight");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(GroundMaterial, new GUIContent("Ground Material"));

        EditorGUILayout.Space();

        cornerFoldout = EditorGUILayout.Foldout(cornerFoldout, "Corners", true);

        if (cornerFoldout)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(TopLeftCorner, new GUIContent("Top Left"));
            EditorGUILayout.PropertyField(TopRightCorner, new GUIContent("Top Right"));
            EditorGUILayout.PropertyField(BottomLeftCorner, new GUIContent("Bottom Left"));
            EditorGUILayout.PropertyField(BottomRightCorner, new GUIContent("Bottom Right"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        Smoothness.intValue = EditorGUILayout.DelayedIntField(new GUIContent("Smoothness"), Smoothness.intValue);

        EditorGUILayout.Space();

        minHeight.floatValue = EditorGUILayout.DelayedFloatField(new GUIContent("Minimum Height"), minHeight.floatValue);
        maxHeight.floatValue = EditorGUILayout.DelayedFloatField(new GUIContent("Maximum Height"), maxHeight.floatValue);

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField(new GUIContent("_____________________________"));

        if (GUILayout.Button("Generate Terrain"))
        {
            GenerateTerrain();
        }

        EditorGUI.indentLevel--;
        serializedObject.ApplyModifiedProperties();
    }

    private void GenerateTerrain()
    {
        Debug.Log("Someday maybe, not yet for sure!");

        if (!terrainGameObject)
        {
            terrainGameObject = new GameObject("Ultimate Terrain",
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(MeshCollider));

            meshFilter = terrainGameObject.GetComponent<MeshFilter>();
            meshRenderer = terrainGameObject.GetComponent<MeshRenderer>();
            meshCollider = terrainGameObject.GetComponent<MeshCollider>();
        }

        //TODO: Actually create the terrain with all the given variables!
        Debug.Log("TODO: Create the terrain with all the given variables!");
    }
}
