using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFilterCounter : MonoBehaviour
{
    public Vector3[] verts;
    public Vector3[] normals;
    public Vector2[] uvs;

    private void Start()
    {
        MeshFilter mf = GetComponentInChildren<MeshFilter>();

        verts = mf.mesh.vertices;
        normals = mf.mesh.normals;
        uvs = mf.mesh.uv;
    }
}
