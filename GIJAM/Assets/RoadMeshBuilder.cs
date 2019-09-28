using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadMeshBuilder : MonoBehaviour {

    public Vector3 vertexOffset;
    public float delayBeforeUpdate = 1f;
    public GameObject car;

    List<Vector3> vertices;
    List<Vector2> uvs;
    List<int> tris;
    int prevVerticesLength = 0;
    MeshCollider road;
    Mesh roadMesh;
    MeshFilter roadMeshFilter;

	// Use this for initialization
	void Start () {
        roadMeshFilter = GetComponent<MeshFilter>();
        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        tris = new List<int>();
        road = GetComponent<MeshCollider>();
        roadMesh = roadMeshFilter.mesh;
        addVertices();
        CalcTris();
        string debug = "";
        foreach (Vector3 i in vertices)
        {
            debug += i.ToString() + " ";
        }
        Debug.Log(debug);
        debug = "";
        foreach (int i in tris)
        {
            debug += i + " ";
        }
        Debug.Log(debug);
        roadMesh.vertices = vertices.ToArray();
        roadMesh.uv = uvs.ToArray();
        roadMesh.triangles = tris.ToArray();
        roadMeshFilter.mesh = roadMesh;
        road.sharedMesh = roadMeshFilter.sharedMesh;
        //StartCoroutine("buildMesh");
    }
	
	// Update is called once per frame
	void Update () {
	}

    // Add current vertices to array
    void addVertices()
    {
        Vector3 carPos = car.transform.position;
        // Add back vertices first, front later
        vertices.Add(new Vector3(carPos.x - vertexOffset.x, carPos.y + vertexOffset.y, carPos.z + vertexOffset.z));
        uvs.Add(Vector2.zero);
        vertices.Add(new Vector3(carPos.x + vertexOffset.x, carPos.y + vertexOffset.y, carPos.z + vertexOffset.z));
        uvs.Add(new Vector2(0, 1));
        vertices.Add(new Vector3(carPos.x + vertexOffset.x, carPos.y + vertexOffset.y, carPos.z - vertexOffset.z));
        uvs.Add(new Vector2(1, 0));
        vertices.Add(new Vector3(carPos.x - vertexOffset.x, carPos.y + vertexOffset.y, carPos.z - vertexOffset.z));
        uvs.Add(Vector2.one);

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
            addVertices();
            CalcTris();
            roadMesh.vertices = vertices.ToArray();
            roadMesh.uv = uvs.ToArray();
            roadMesh.triangles = tris.ToArray();
            roadMeshFilter.mesh = roadMesh;
            road.sharedMesh = roadMeshFilter.mesh;
        }
        
    } 
}
