using System;
using System.Collections;
using System.Collections.Generic;
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
    /// The highest Y the terrain can have.
    /// </summary>
    private SerializedProperty maxHeight;

    private SerializedProperty seed;

    float div;

    private GameObject terrainGameObject;

    private Vector3[] vertices;

    private List<int> triList = new List<int>();
    private int[] triangles;
    private int trianglesMade;

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

        // minHeight = serializedObject.FindProperty("minHeight");
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

        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(GroundMaterial, new GUIContent("Ground Material"));

        EditorGUILayout.Space();

        xSize.intValue = EditorGUILayout.DelayedIntField(new GUIContent("Terrain Width"), xSize.intValue);
        zSize.intValue = EditorGUILayout.DelayedIntField(new GUIContent("Terrain Depth"), zSize.intValue);
        maxHeight.floatValue = EditorGUILayout.DelayedFloatField(new GUIContent("Maximum Terrain Height"), maxHeight.floatValue);

        EditorGUILayout.Space();

        Subdivisions.intValue = EditorGUILayout.DelayedIntField(new GUIContent("Edges per Axis Unit"), Subdivisions.intValue);

        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();

        div = 1 / seed.floatValue;

        if (GUILayout.Button("Generate Terrain"))
        {
            GenerateTerrain();
        }

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

    private void GenerateTerrain()
    {
        xSpace = (float)xSize.intValue / (Subdivisions.intValue * xSize.intValue);
        zSpace = (float)zSize.intValue / (Subdivisions.intValue * zSize.intValue);


        // First make the new vertex array
        CreateVertices();

        // Then create the needed triangles
        CreateTriangles();

        Debug.LogFormat("For {0} vertices, I made {1} triangles.", vertices.Length, trianglesMade);

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = (Material)GroundMaterial.objectReferenceValue;
        meshCollider.sharedMesh = mesh;

        mesh.RecalculateNormals();

        // AssetDatabase.CreateAsset((mesh, Application.dataPath + "/Ultimate Terrain Generator/Generated Meshes/");
        // AssetDatabase.SaveAssets();
    }

    private void CreateVertices()
    {
        int currentIndex = 0;

        float xValue, zValue, y;

        vertices = new Vector3[(xSize.intValue * Subdivisions.intValue + 1) * (zSize.intValue * Subdivisions.intValue + 1)];

        for (int z = 0; z <= zSize.intValue * Subdivisions.intValue; z++)
        {
            for (int x = 0; x <= xSize.intValue * Subdivisions.intValue; x++)
            {
                xValue = x / (float)Subdivisions.intValue;
                zValue = z / (float)Subdivisions.intValue;

                y = Mathf.Clamp(Mathf.PerlinNoise(xValue + div + vertices.Length, zValue + div + vertices.Length), 0.0f, maxHeight.floatValue);

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

        for (int i = 0; i < vertices.Length; i++)
        {
            // Returns true if a triangle is made. False if not.
            // Used for customizing the code to the user's need.
            bool BL = TryMakingBLTriangle(i);
            bool TR = TryMakingTRTriangle(i);

            #region Example Returned Bool Usage

            /* An example of how the bool returned values could be used.
                 * In this case, it generates "walls" around the terrain.
                 */
            //if (!BL || !TR)
            //{
            //    GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    g.name = vertices[i].ToString();
            //    g.transform.localScale = new Vector3(xSpace, xSpace, zSpace);
            //    g.transform.position = vertices[i];
            //} 

            #endregion
        }

        // Convert the triangle list to array.
        triangles = new int[triList.Count];
        triangles = triList.ToArray();
    }

    /// <summary>
    /// Try making a Bottom-Left triangle with the vertice at the 'currentIndex'
    /// being the start, positioned at the Bottom-Left of the triangle to be made.
    /// </summary>
    /// <param name="currentIndex"></param>
    private bool TryMakingBLTriangle(int currentIndex)
    {
        // If we're at the last index, index + 1 throws IndexOutOfRange!
        if (currentIndex >= vertices.Length - 1)
        {
            return false;
        }

        Vector3 currentVertex = vertices[currentIndex];
        Vector3 nextVertex = vertices[currentIndex + 1];

        int vertexAboveMeIndex = FindVertexAt(currentVertex.x, currentVertex.z + zSpace);

        // For a BL triangle to be made:
        // --> A vertex exists at the position (currentVertex.x, currentVertex.z + zSpace)
        if (vertexAboveMeIndex == -1)
        {
            return false;
        }

        // --> The next vertex must have pos.z == to the current vertex
        // --> The next vertex must have pos.x == currentVertex.x + xSpace
        if (currentVertex.z != nextVertex.z)
        {
            return false;
        }

        float neededNextX = currentVertex.x + xSpace;
        if (!Mathf.Approximately(nextVertex.x, neededNextX))
        {
            return false;
        }

        triList.Add(currentIndex);
        triList.Add(vertexAboveMeIndex);
        triList.Add(currentIndex + 1);

        trianglesMade++;
        return true;
    }

    /// <summary>
    /// Try making a Top-Right triangle with the vertice at the 'currentIndex'
    /// being the start, positioned at the Top-Right of the triangle to be made.
    /// </summary>
    /// <param name="currentIndex"></param>
    private bool TryMakingTRTriangle(int currentIndex)
    {
        // If we're at 0, currentIndex - 1 throws IndexOutOfRange!
        if (currentIndex == 0)
        {
            return false;
        }

        Vector3 currentVertex = vertices[currentIndex];
        Vector3 previousVertex = vertices[currentIndex - 1];

        int vertexBelowMeIndex = FindVertexAt(currentVertex.x, currentVertex.z - zSpace);

        // For a TR triangle to be made:
        // --> A vertex exists at the position (currentVertex.x, currentVertex.z - zSpace)
        if (vertexBelowMeIndex == -1)
        {
            return false;
        }

        // --> The previous vertex must have pos.z == to the current vertex
        // --> The previous vertex must have pos.x == currentVertex.x - xSpace
        if (currentVertex.z != previousVertex.z)
        {
            return false;
        }

        float neededPreviousX = currentVertex.x - xSpace;
        if (!Mathf.Approximately(previousVertex.x, neededPreviousX))
        {
            return false;
        }

        triList.Add(currentIndex);
        triList.Add(vertexBelowMeIndex);
        triList.Add(currentIndex - 1);

        trianglesMade++;
        return true;
    }

    /// <summary>
    /// Returns the index of the first vertex found in the 'vertices' array,
    /// with the given x and z position (y is ignored). If no vertex is found,
    /// returns -1 instead.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private int FindVertexAt(float x, float z)
    {

        for (int i = 0; i < vertices.Length; i++)
        {
            if (Mathf.Approximately(vertices[i].x, x) && Mathf.Approximately(vertices[i].z, z))
            {
                return i;
            }
        }

        return -1;
    }

    public static class MeshSaverEditor
    {

        public static void SaveMeshAs(ref MeshFilter mf)
        {
            Mesh m = mf.sharedMesh;
            SaveMesh(m, m.name, true, true);
        }

        private static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
        {
            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/Ultimate Terrain Generator/Generated Meshes/", name, "mesh");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);

            Mesh meshToSave = (makeNewInstance) ? Instantiate(mesh) as Mesh : mesh;

            if (optimizeMesh)
                MeshUtility.Optimize(meshToSave);

            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();
        }

    }

}