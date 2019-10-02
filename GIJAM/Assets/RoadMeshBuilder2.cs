using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadMeshBuilder2 : MonoBehaviour {
    // Naive implementation of road builder
    public Vector3 vertexOffset;
    public float delayBeforeUpdate = 1f;
    public GameObject car;

    List<Vector3> vertices;
    List<Vector2> uvs;
    List<Vector3> normals;
    List<int> tris;
    int prevVerticesLength = 0;
    MeshCollider road;
    Mesh roadMesh;
    MeshFilter roadMeshFilter;
    int[] lastTwo;
    Vector3 carPos;
    float carLowestYExtent;

	// Use this for initialization
	void Start () {
        roadMeshFilter = GetComponent<MeshFilter>();
        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        tris = new List<int>();
        normals = new List<Vector3>();
        road = GetComponent<MeshCollider>();
        roadMesh = roadMeshFilter.mesh;
        lastTwo = new int[2];

        // Get car's lowest point to initialize road with
        carPos = car.transform.position;
        carLowestYExtent = GetCarLowestY();
        carPos.y += carLowestYExtent;

        // Add vertices corner to corner
        vertices.Add(new Vector3(carPos.x + vertexOffset.x, carPos.y + vertexOffset.y, carPos.z + vertexOffset.z));
        uvs.Add(Vector2.zero);
        normals.Add(Vector3.up);
        vertices.Add(new Vector3(carPos.x - vertexOffset.x, carPos.y + vertexOffset.y, carPos.z - vertexOffset.z));
        uvs.Add(new Vector2(0, 1));
        normals.Add(Vector3.up);
        vertices.Add(new Vector3(carPos.x + vertexOffset.x, carPos.y + vertexOffset.y, carPos.z - vertexOffset.z));
        uvs.Add(new Vector2(1, 0));
        normals.Add(Vector3.up);
        vertices.Add(new Vector3(carPos.x - vertexOffset.x, carPos.y + vertexOffset.y, carPos.z + vertexOffset.z));
        uvs.Add(Vector2.one);
        normals.Add(Vector3.up);

        // Give initial triangles
        int[] hardcode = new int[] { 2, 1, 0, 1, 3, 0 };
        tris.AddRange(hardcode);
        
        // Convert lists to array and apply to mesh
        roadMesh.vertices = vertices.ToArray();
        roadMesh.uv = uvs.ToArray();
        roadMesh.triangles = tris.ToArray();
        roadMesh.normals = normals.ToArray();
        roadMeshFilter.mesh = roadMesh;
        road.sharedMesh = roadMeshFilter.sharedMesh;
        lastTwo[0] = 0;
        lastTwo[1] = 3;
        StartCoroutine("buildMesh");
    }
	
	// Update is called once per frame
	void Update () {
	}

    float GetCarLowestY()
    {
        MeshFilter[] meshes = car.GetComponentsInChildren<MeshFilter>(true);
        float lowest = Mathf.Infinity;
        foreach (MeshFilter m in meshes)
        {
            float temp = m.sharedMesh.bounds.min.y;
            if (temp < lowest)
            {
                lowest = temp;
            }
        }
        return lowest;
    }

    // Add current vertices to array
    void addVertices()
    {
        
        // Prune similar vertices
    }

    void CalcTris()
    {
        tris.Clear();
        //Debug.Log(prevVerticesLength);
        for (int i = 0; i < vertices.Count; i+=2)
        {
            Debug.Log("Triangle" + (i - 2) + " " + (i - 1) + " " + i);
            tris.Add(i);
            tris.Add((i+1)%vertices.Count);
            tris.Add((i+2)%vertices.Count);
        }
        //prevVerticesLength = vertices.Count;
    }

    IEnumerator buildMesh()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBeforeUpdate);
            roadMesh.Clear();
            int newAdd = vertices.Count;
            // Add two new points in front
            Vector3 newV = new Vector3(car.transform.position.x + vertexOffset.x,
                car.transform.position.y + vertexOffset.y,
                car.transform.position.z + vertexOffset.z);
            
            Debug.Log(newV.ToString());
            vertices.Add(newV);
            uvs.Add(Vector2.zero);
            normals.Add(Vector3.up);
            newV = new Vector3(car.transform.position.x - vertexOffset.x,
                car.transform.position.y + vertexOffset.y,
                car.transform.position.z + vertexOffset.z);
            Debug.Log(newV.ToString());
            vertices.Add(newV);
            uvs.Add(Vector2.zero);
            normals.Add(Vector3.up);

            // Add two new triangles
            tris.Add(newAdd);
            tris.Add(lastTwo[0]);
            tris.Add(lastTwo[1]);
            tris.Add(lastTwo[1]);
            tris.Add(newAdd + 1);
            tris.Add(newAdd);

            lastTwo[0] = newAdd;
            lastTwo[1] = newAdd + 1;

            roadMesh.vertices = vertices.ToArray();
            roadMesh.uv = uvs.ToArray();
            roadMesh.triangles = tris.ToArray();
            roadMesh.normals = normals.ToArray();
            roadMeshFilter.mesh = roadMesh;
            road.sharedMesh = roadMeshFilter.mesh;
        }
        
    } 
}
