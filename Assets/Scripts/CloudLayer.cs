// CloudLayer.cs
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CloudLayer : MonoBehaviour 
{
    [Header("Cloud Settings")]
    [SerializeField] private float cloudHeight = 100f;
    [SerializeField] private float gridSize = 100f;
    [SerializeField] private int gridResolution = 10;
    [SerializeField] private Material cloudMaterial;
    
    private Camera mainCamera;
    private Vector3 lastCameraPosition;
    private Mesh cloudMesh;

    private void Start()
    {
        mainCamera = Camera.main;
        lastCameraPosition = mainCamera.transform.position;
        
        GenerateCloudMesh();
        UpdatePosition();
    }

    private void Update()
    {
        if (Vector3.Distance(mainCamera.transform.position, lastCameraPosition) > gridSize * 0.1f)
        {
            UpdatePosition();
            lastCameraPosition = mainCamera.transform.position;
        }
    }

    private void GenerateCloudMesh()
    {
        cloudMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = cloudMesh;

        Vector3[] vertices = new Vector3[(gridResolution + 1) * (gridResolution + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[gridResolution * gridResolution * 6];

        float cellSize = gridSize / gridResolution;
        int triangleIndex = 0;

        // Generate vertices and UVs
        for (int z = 0; z <= gridResolution; z++)
        {
            for (int x = 0; x <= gridResolution; x++)
            {
                float xPos = (x * cellSize) - (gridSize * 0.5f);
                float zPos = (z * cellSize) - (gridSize * 0.5f);
                
                int vertexIndex = z * (gridResolution + 1) + x;
                vertices[vertexIndex] = new Vector3(xPos, 0, zPos);
                uvs[vertexIndex] = new Vector2((float)x / gridResolution, (float)z / gridResolution);

                // Generate triangles (except for last row/column)
                if (x < gridResolution && z < gridResolution)
                {
                    int topLeft = vertexIndex;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + gridResolution + 1;
                    int bottomRight = bottomLeft + 1;

                    triangles[triangleIndex] = topLeft;
                    triangles[triangleIndex + 1] = bottomLeft;
                    triangles[triangleIndex + 2] = topRight;
                    triangles[triangleIndex + 3] = topRight;
                    triangles[triangleIndex + 4] = bottomLeft;
                    triangles[triangleIndex + 5] = bottomRight;

                    triangleIndex += 6;
                }
            }
        }

        cloudMesh.vertices = vertices;
        cloudMesh.triangles = triangles;
        cloudMesh.uv = uvs;
        cloudMesh.RecalculateNormals();
    }

    private void UpdatePosition()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        float x = Mathf.Floor(cameraPosition.x / gridSize) * gridSize;
        float z = Mathf.Floor(cameraPosition.z / gridSize) * gridSize;
        
        transform.position = new Vector3(x, cloudHeight, z);
    }
}