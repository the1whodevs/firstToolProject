using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int mapWidth;

    [SerializeField] private int platformLevels;

    [SerializeField] private float platformLevelHeight;

    [SerializeField] private GameObject floorPrefab;

    [SerializeField] private GameObject wallPrefab;

    [SerializeField] private bool UseStaticPlatforms;

    [SerializeField] private GameObject[] staticPlatforms;

    [SerializeField] private bool UseUpDownPlatforms;

    [SerializeField] private GameObject[] upDownPlatforms;

    [SerializeField] private bool UseLeftRightPlatforms;

    [SerializeField] private GameObject[] leftRightPlatforms;

    [SerializeField] private bool UseStaticTraps;

    [SerializeField] private GameObject[] staticTraps;

    [SerializeField] private bool UseEnemies;

    [SerializeField] private GameObject[] enemies;
}
