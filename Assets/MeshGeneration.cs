using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MeshGeneration : MonoBehaviour
{
    Mesh _mesh;
    
    [Header("Mesh Settings")]
    public Vector3[] _vertices; // Vertices of the mesh
    public int[] _triangles; // Triangles of the mesh
    public Vector2[] _uvs; // UVs of the mesh
    public Vector3[] _normals; // Normals of the mesh
    
    [Header("Mesh Generation Settings")] 
    public int width; // Width of the mesh
    public int height; // Height of the mesh
    public float scale; // Scale of the mesh
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMesh();
        }
    }
    
    private void GenerateMesh()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        
        _vertices = new Vector3[(width + 1) * (height + 1)]; // Vertices of the mesh
        _triangles = new int[width * height * 6]; // Triangles of the mesh
        _uvs = new Vector2[_vertices.Length]; // UVs of the mesh
        _normals = new Vector3[_vertices.Length]; // Normals of the mesh
        
        for (int i = 0, y = 0; y <= height; y++) // Generate the vertices
        {
            for (int x = 0; x <= width; x++, i++) // Generate the vertices
            {
                float xCoord = (float)x / width * scale; // Calculate the x coordinate
                float yCoord = (float)y / height * scale; // Calculate the y coordinate
                
                float z = Mathf.PerlinNoise(xCoord, yCoord); // Calculate the z coordinate
                
                _vertices[i] = new Vector3(x, z, y); // Set the vertex position
                _uvs[i] = new Vector2((float)x / width, (float)y / height); // Set the UVs
                _normals[i] = Vector3.up; // Set the normals
            }
        }
        
        for (int ti = 0, vi = 0, y = 0; y < height; y++, vi++) // Generate the triangles
        {
            for (int x = 0; x < width; x++, ti += 6, vi++) // Generate the triangles
            {
                _triangles[ti] = vi; // Set the triangles
                _triangles[ti + 3] = _triangles[ti + 2] = vi + 1; // Set the triangles
                _triangles[ti + 4] = _triangles[ti + 1] = vi + width + 1; // Set the triangles
                _triangles[ti + 5] = vi + width + 2; // Set the triangles
                
                //yield return new WaitForSeconds(0.1f);
            }
        }
        
        _mesh.vertices = _vertices; // Set the vertices
        _mesh.triangles = _triangles; // Set the triangles
        _mesh.uv = _uvs; // Set the UVs
        _mesh.normals = _normals; // Set the normals
        
        /* NOTE: The following code is not necessary for this example, looking at the methods that you could use.
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.Optimize();
        _mesh.UploadMeshData(false);
        _mesh.SetColors(new Color[_vertices.Length]);
        */
    }
    
    private void OnDrawGizmos()
    {
        if (_vertices == null)
        {
            return;
        }
        
        for (int i = 0; i < _vertices.Length; i++)
        {
            Gizmos.DrawSphere(_vertices[i], 0.1f);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (_vertices == null)
        {
            return;
        }
        
        for (int i = 0; i < _vertices.Length; i++)
        {
            Gizmos.DrawSphere(_vertices[i], 0.1f);
        }
    }
    
    private void OnValidate() // Called when the script is loaded or a value is changed in the inspector
    {
        GenerateMesh();
        // Pretty good for generating the mesh in the editor
    }
}
