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
    /// The highest Y the terrain can have.
    /// </summary>
    private SerializedProperty heightMultiplier;

    private SerializedProperty seed;

    private SerializedProperty terrainGameObject;

    private SerializedProperty minecraftMode;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private float div;

    void OnEnable()
    {
        terrainGameObject = serializedObject.FindProperty("terrainGameObject");

        GroundMaterial = serializedObject.FindProperty("GroundMaterial");

        xSize = serializedObject.FindProperty("xSize");
        zSize = serializedObject.FindProperty("zSize");

        seed = serializedObject.FindProperty("seed");

        Subdivisions = serializedObject.FindProperty("Subdivisions");

        heightMultiplier = serializedObject.FindProperty("heightMultiplier");

        minecraftMode = serializedObject.FindProperty("doMinecraft");

        if (terrainGameObject.objectReferenceValue == null)
        {
            terrainGameObject.objectReferenceValue = GameObject.Find("Ultimate Terrain");

            if (!terrainGameObject.objectReferenceValue)
            {
                terrainGameObject.objectReferenceValue = new GameObject("Ultimate Terrain",
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(MeshCollider));
            }
        }

        meshFilter = ((GameObject)terrainGameObject.objectReferenceValue).GetComponent<MeshFilter>();
        meshRenderer = ((GameObject)terrainGameObject.objectReferenceValue).GetComponent<MeshRenderer>();
        meshCollider = ((GameObject)terrainGameObject.objectReferenceValue).GetComponent<MeshCollider>();

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(seed, new GUIContent("Seed"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(minecraftMode, new GUIContent("Minecraft Mode"));
        Subdivisions.intValue = EditorGUILayout.DelayedIntField(new GUIContent("Edges per Axis Unit"), Subdivisions.intValue);
        EditorGUILayout.PropertyField(GroundMaterial, new GUIContent("Ground Material"));

        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        EditorGUILayout.Space();

        xSize.intValue = EditorGUILayout.DelayedIntField(new GUIContent("Terrain Width"), xSize.intValue);
        zSize.intValue = EditorGUILayout.DelayedIntField(new GUIContent("Terrain Depth"), zSize.intValue);
        heightMultiplier.floatValue = EditorGUILayout.DelayedFloatField(new GUIContent("Height Multiplier"), heightMultiplier.floatValue);

        if (!minecraftMode.boolValue)
        {
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();

            div = 1 / seed.floatValue;

            if (GUILayout.Button("Generate Terrain"))
            {
                ((TerrainGenerator)target).GenerateTerrain(ref meshFilter, ref meshRenderer, ref meshCollider, div);
            } 
        }
        else
        {
            EditorGUILayout.LabelField(new GUIContent("Coming soon!"));

            // Box Height
            // GenerateMinecraftTerrain button

            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.Separator();

        if (meshFilter.sharedMesh != null)
        {
            if (GUILayout.Button("Save Mesh As..."))
            {
                MeshSaverEditor.SaveMeshAs(ref meshFilter);
            }
        }


        EditorGUI.indentLevel--;
    }

    public static class MeshSaverEditor
    {

        public static void SaveMeshAs(ref MeshFilter mf)
        {
            Mesh m = mf.sharedMesh;
            SaveMesh(m, m.name, true);
        }

        private static void SaveMesh(Mesh mesh, string name, bool optimizeMesh)
        {
            string path = EditorUtility.SaveFilePanel("Save Mesh As...", "Assets/Ultimate Terrain Generator/Generated Meshes/", name, "mesh");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);

            //if (optimizeMesh)
            //    MeshUtility.Optimize(mesh);

            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
        }

    }

}