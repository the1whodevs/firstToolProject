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
    /// Ideally used for top-down view, this should be the top-left corner of the terrain.
    /// </summary>
    [SerializeField] private Transform TopLeftCorner;

    /// <summary>
    /// Ideally used for top-down view, this should be the top-right corner of the terrain.
    /// </summary>
    [SerializeField] private Transform TopRightCorner;

    /// <summary>
    /// Ideally used for top-down view, this should be the bottom-left corner of the terrain.
    /// </summary>
    [SerializeField] private Transform BottomLeftCorner;

    /// <summary>
    /// Ideally used for top-down view, this should be the bottom-right corner of the terrain.
    /// </summary>
    [SerializeField] private Transform BottomRightCorner;

    /// <summary>
    /// The number of subdivisions the terrain will have. The higher this number is
    /// the more detailed the terrain will be, as well as the number of triangles.
    /// </summary>
    [SerializeField] private int Smoothness;

    /// <summary>
    /// The lowest Y the terrain can have.
    /// </summary>
    [SerializeField] private float minHeight;

    /// <summary>
    /// The highest Y the terrain can have.
    /// </summary>
    [SerializeField] private float maxHeight;
}
