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
    /// The lowest Y the terrain can have.
    /// </summary>
    [SerializeField] private float minHeight;

    /// <summary>
    /// The highest Y the terrain can have.
    /// </summary>
    [SerializeField] private float maxHeight;

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
}
