using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter))]
public class MeshGeneration : MonoBehaviour
{
    Mesh _mesh;
    
    [Header("Mesh Settings")]
    private Vector3[] _vertices; // mesh vertices
    private int[] _triangles; // mesh triangles
    private Vector2[] _uvs; // mesh uv
    private Vector3[] _normals; // mesh normals

    [Header("Mesh Generation Settings")]
    [Range(1,100)]public int width; // x
    [Range(1,100)]public int length; // z
    [Range(1,10)]public float scale; // noise scale
    [Range(1,5)] public float heightMultiplier; // height multiplier
    [Range(1,2)]public int octaves; // noise octaves
    [Range(1,2)]public float persistence; // noise persistence
    [Range(1,2)]public float lacunarity; // noise lacunarity
    
    void Start()
    {
        GenerateMesh();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMesh();
        }
    }

    void GenerateMesh()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        _vertices = new Vector3[(width + 1) * (length + 1)]; 
        _triangles = new int[width * length * 6]; 
        _uvs = new Vector2[_vertices.Length];
        _normals = new Vector3[_vertices.Length];

        System.Random prng = new System.Random();
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++) // generate random offsets for each octave
        {
            float offsetX = prng.Next(-100000, 100000); 
            float offsetY = prng.Next(-100000, 100000); 
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0) // prevent division by zero
        {
            scale = 0.0001f; 
        }

        for (int i = 0, y = 0; y <= length; y++) // generate vertices
        {
            for (int x = 0; x <= width; x++, i++)  // generate vertices
            {
                float amplitude = 1; // amplitude of the noise
                float frequency = 1; // frequency of the noise
                float noiseHeight = 0; // height of the noise

                for (int j = 0; j < octaves; j++) // generate noise
                {
                    float sampleX = x / scale * frequency + octaveOffsets[j].x; // sample noise
                    float sampleY = y / scale * frequency + octaveOffsets[j].y; // sample noise

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // generate perlin noise
                    noiseHeight += perlinValue * amplitude; // add noise to height

                    amplitude *= persistence; 
                    frequency *= lacunarity;
                }

                float height = noiseHeight * heightMultiplier; // multiply noise height by height multiplier
                _vertices[i] = new Vector3(x, height, y); // set vertex position
                _uvs[i] = new Vector2((float)x / width, (float)y / length); // set uv
                _normals[i] = Vector3.up; // set normal
            }
        }

        for (int ti = 0, vi = 0, y = 0; y < length; y++, vi++) // generate triangles
        {
            for (int x = 0; x < width; x++, ti += 6, vi++)
            {
                _triangles[ti] = vi;
                _triangles[ti + 3] = _triangles[ti + 2] = vi + 1;
                _triangles[ti + 4] = _triangles[ti + 1] = vi + width + 1;
                _triangles[ti + 5] = vi + width + 2;
            }
        }

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.uv = _uvs;
        _mesh.normals = _normals;
    }
    
    private void OnDrawGizmos() // draw vertices in editor
    {
        if (_vertices == null)
        {
            return;
        }

        foreach (var t in _vertices)
        {
            Gizmos.DrawSphere(t, 0.1f);
        }
    }
}
