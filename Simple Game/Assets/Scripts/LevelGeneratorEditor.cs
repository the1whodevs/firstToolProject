using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
[CanEditMultipleObjects]
public class LevelGeneratorEditor : Editor
{
    SerializedProperty mapWidth;

    SerializedProperty platformLevels;

    SerializedProperty platformLevelHeight;

    SerializedProperty floorPrefab;

    SerializedProperty wallPrefab;

    SerializedProperty UseStaticPlatforms;
    SerializedProperty staticPlatforms;
    bool staticPlatformsFoldout = false;

    SerializedProperty UseUpDownPlatforms;
    SerializedProperty upDownPlatforms;
    bool upDownPlatformsFoldout = false;

    SerializedProperty UseLeftRightPlatforms;
    SerializedProperty leftRightPlatforms;
    bool leftRightPlatformsFoldout = false;

    SerializedProperty UseStaticTraps;
    SerializedProperty staticTraps;
    bool staticTrapsFoldout = false;

    SerializedProperty UseEnemies;
    SerializedProperty enemies;
    bool enemiesFoldout = false;

    private Vector3 firstGroundPos, lastGroundPos, topLeftWallPos, topRightWallPos;

    private void OnEnable()
    {
        mapWidth = serializedObject.FindProperty("mapWidth");

        platformLevels = serializedObject.FindProperty("platformLevels");

        platformLevelHeight = serializedObject.FindProperty("platformLevelHeight");

        floorPrefab = serializedObject.FindProperty("floorPrefab");

        wallPrefab = serializedObject.FindProperty("wallPrefab");

        UseStaticPlatforms = serializedObject.FindProperty("UseStaticPlatforms");
        staticPlatforms = serializedObject.FindProperty("staticPlatforms");

        UseUpDownPlatforms = serializedObject.FindProperty("UseUpDownPlatforms");
        upDownPlatforms = serializedObject.FindProperty("upDownPlatforms");

        UseLeftRightPlatforms = serializedObject.FindProperty("UseLeftRightPlatforms");
        leftRightPlatforms = serializedObject.FindProperty("leftRightPlatforms");

        UseStaticTraps = serializedObject.FindProperty("UseStaticTraps");
        staticTraps = serializedObject.FindProperty("staticTraps");

        UseEnemies = serializedObject.FindProperty("UseEnemies");
        enemies = serializedObject.FindProperty("enemies");
    }

    public override void OnInspectorGUI()
    {
        #region Show Fields
        EditorGUILayout.Space();

        EditorGUILayout.IntSlider(mapWidth, 10, 100, new GUIContent("Map Width"));
        EditorGUILayout.IntSlider(platformLevels, 1, 10, new GUIContent("Platform Levels"));
        EditorGUILayout.Slider(platformLevelHeight, 0.5f, 10.0f, new GUIContent("Platform Level Height"));

        EditorGUILayout.PropertyField(floorPrefab, new GUIContent("Floor Prefab"));

        EditorGUILayout.PropertyField(wallPrefab, new GUIContent("Wall Prefab"));

        EditorGUILayout.PropertyField(UseStaticPlatforms, new GUIContent("Use Static Platforms "));

        Rect rect = EditorGUILayout.BeginVertical();

        rect.height = 50.0f;

        if (UseStaticPlatforms.boolValue)
        {
            ArrayInspectorGUI(staticPlatforms, "Static Platforms", ref staticPlatformsFoldout);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(UseUpDownPlatforms, new GUIContent("Use Up/Down Platforms "));

        if (UseUpDownPlatforms.boolValue)
        {
            ArrayInspectorGUI(upDownPlatforms, "UpDown Platforms", ref upDownPlatformsFoldout);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(UseLeftRightPlatforms, new GUIContent("Use Left/Right Platforms "));

        if (UseLeftRightPlatforms.boolValue)
        {
            ArrayInspectorGUI(leftRightPlatforms, "LeftRight Platforms", ref leftRightPlatformsFoldout);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(UseStaticTraps, new GUIContent("Use Static Traps Platforms "));

        if (UseStaticTraps.boolValue)
        {
            ArrayInspectorGUI(staticTraps, "UpDown Platforms", ref staticTrapsFoldout);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(UseEnemies, new GUIContent("Use Enemies "));

        if (UseEnemies.boolValue)
        {
            ArrayInspectorGUI(enemies, "UpDown Platforms", ref enemiesFoldout);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(); 
        #endregion

        if (GUILayout.Button("Generate"))
        {
            // Generate ground
            GenerateGround();

            // Generate walls
            GenerateWalls();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void GenerateWalls()
    {
        GameObject wallParent = new GameObject("Wall Parent");

        // Left wall is at firstGroundPos
        GameObject g = Instantiate((GameObject)wallPrefab.objectReferenceValue, wallParent.transform);

        float yIncrement = g.GetComponent<PlatformInfo>().platformEndingPoint.y * 2;
        Vector3 pos = firstGroundPos + Vector3.up * yIncrement;

        g.transform.position = pos;

        // Just making the wall very tall!
        for (int i = 0; i < platformLevels.intValue * mapWidth.intValue; i++)
        {
            GameObject go = Instantiate((GameObject)wallPrefab.objectReferenceValue, wallParent.transform);
            pos += Vector3.up * yIncrement;
            go.transform.position = pos;
        }

        topLeftWallPos = pos;

        // Right wall is at lastGroundPos
        g = Instantiate((GameObject)wallPrefab.objectReferenceValue, wallParent.transform);
        pos = lastGroundPos + Vector3.up * yIncrement;
        g.transform.position = pos;

        // Just making the wall very tall!
        for (int i = 0; i < platformLevels.intValue * mapWidth.intValue; i++)
        {
            GameObject go = Instantiate((GameObject)wallPrefab.objectReferenceValue, wallParent.transform);
            pos += Vector3.up * yIncrement;
            go.transform.position = pos;
        }

        topRightWallPos = pos;
    }

    private void GenerateGround()
    {
        Vector3 pos = Vector3.zero;
        firstGroundPos = pos;

        GameObject ground = new GameObject("Ground Parent");

        PlatformInfo lastPlatInfo = null;

        for (int i = 0; i < mapWidth.intValue; i++)
        {
            GameObject g = Instantiate((GameObject)floorPrefab.objectReferenceValue, ground.transform);
            g.transform.position = pos;
            lastPlatInfo = g.GetComponent<PlatformInfo>();

            pos += Vector3.right * lastPlatInfo.platformEndingPoint.x * 2;
        }

        lastGroundPos = pos - Vector3.right * lastPlatInfo.platformEndingPoint.x * 2;
    }

    void ArrayInspectorGUI(SerializedProperty property, string title, ref bool boolToUse)
    {
        int prevIndent = EditorGUI.indentLevel;

        property.arraySize = EditorGUILayout.DelayedIntField(title, property.arraySize);
        EditorGUI.indentLevel++;
        boolToUse = EditorGUILayout.Foldout(boolToUse, "Elements", true);

        if (boolToUse)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < property.arraySize; i++)
            {
                var prop = property.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(prop);

                if (prop.objectReferenceValue == null)
                {
                    property.arraySize = i + 1;
                    break;
                }
            }
        }

        EditorGUI.indentLevel = prevIndent;
        EditorGUILayout.Space();
    }

}
