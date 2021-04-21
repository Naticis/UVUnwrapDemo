using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum Face { X, Y, Z, NegX, NegY, NegZ, NA };


public class SphericalCoordinates : MonoBehaviour
{
    [SerializeField]
    private Mesh mesh;

    [SerializeField]
    private Vector3[] vertices;
    [SerializeField]
    private Vector3[] normals;
    [SerializeField]
    private Vector2[] uvs;


    [SerializeField]
    List<Vector2> newUvs2;

    [SerializeField]
    List<Vector2> tempUvs2, finalUvs2;

    [SerializeField]
    List<V3WithFace> v3WithFaces;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        v3WithFaces = new List<V3WithFace>();
    }

    private int vertsCount = 0;
    private void Start()
    {
        SphericalUnwrap();
    }

    private void CubicUnwrap()
    {
        vertices = mesh.vertices;
        normals = mesh.normals;
        uvs = mesh.uv;
        newUvs2 = new List<Vector2>();
        tempUvs2 = new List<Vector2>();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 face = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z);
            if (Mathf.Abs(normals[i].x) >= Mathf.Abs(normals[i].y) && Mathf.Abs(normals[i].x) >= Mathf.Abs(normals[i].z))
            {
                if (normals[i].x >= 0)
                    v3WithFaces.Add(new V3WithFace(Face.X, face));
                else
                    v3WithFaces.Add(new V3WithFace(Face.NegX, face));
                tempUvs2.Add(new Vector2(face.y, face.z));
            }
            else if (Mathf.Abs(normals[i].y) >= Mathf.Abs(normals[i].x) && Mathf.Abs(normals[i].y) >= Mathf.Abs(normals[i].z))
            {
                if (normals[i].y >= 0)
                    v3WithFaces.Add(new V3WithFace(Face.Y, face));
                else
                    v3WithFaces.Add(new V3WithFace(Face.NegY, face));
                tempUvs2.Add(new Vector2(face.x, face.z));
            }
            else if (Mathf.Abs(normals[i].z) >= Mathf.Abs(normals[i].y) && Mathf.Abs(normals[i].z) >= Mathf.Abs(normals[i].x))
            {
                if (normals[i].x >= 0)
                    v3WithFaces.Add(new V3WithFace(Face.Z, face));
                else
                    v3WithFaces.Add(new V3WithFace(Face.NegZ, face));
                tempUvs2.Add(new Vector3(face.x, face.y));
            }
        }

        for (int i = 0; i < v3WithFaces.Count; i++)
        {
            Vector2 finalUV = Vector2.zero;
            switch (v3WithFaces[i].face)
            {
                case Face.X:
                    {
                        finalUV.x = 2f / 3 + tempUvs2[i].x / 3f;
                        finalUV.y = 2f / 4 + tempUvs2[i].y / 4;
                        break;
                    }
                case Face.Y:
                    {
                        finalUV.x = 1f / 3 + tempUvs2[i].x / 3f;
                        finalUV.y = 3f / 4 + tempUvs2[i].y / 4;
                        break;
                    }
                case Face.Z:
                    {
                        finalUV.x = 1f / 3 + tempUvs2[i].x / 3f;
                        finalUV.y = 2f / 4 + tempUvs2[i].y / 4;
                        break;
                    }
                case Face.NegX:
                    {
                        finalUV.x = 0f / 3 + tempUvs2[i].x / 3f;
                        finalUV.y = 2f / 4 + tempUvs2[i].y / 4;
                        break;
                    }
                case Face.NegY:
                    {
                        finalUV.x = 1f / 3 + tempUvs2[i].x / 3f;
                        finalUV.y = 1f / 4 + tempUvs2[i].y / 4;
                        break;
                    }
                case Face.NegZ:
                    {
                        finalUV.x = 1f / 3 + tempUvs2[i].x / 3f;
                        finalUV.y = 0f / 4 + tempUvs2[i].y / 4;
                        break;
                    }
                case Face.NA:
                    {
                        break;
                    }
            }
            finalUvs2.Add(finalUV);
        }
        mesh.uv = finalUvs2.ToArray();
    }

    private void SphericalUnwrap()
    {
        vertices = mesh.vertices;
        normals = mesh.normals;
        uvs = mesh.uv;

        for (int i = 0; i < vertices.Length; i++)
        {
            //float radial = 0;
            //float azimuthal = 0;
            //float polar = 0;

            //radial = Mathf.Sqrt(vert.x * vert.x + vert.y * vert.y + vert.z * vert.z);
            //azimuthal = Mathf.Acos(vert.z / radial);
            //polar = Mathf.Atan2(vert.y, vert.z);

            //newUvs3.Add(new Vector3(radial, azimuthal, polar));
            //newUvs2.Add(new Vector2(azimuthal, polar));


            float u = 0.5f + Mathf.Atan2(vertices[i].z, vertices[i].x) / (2 * Mathf.PI);
            float v = 0.5f - Mathf.Asin(vertices[i].y) / Mathf.PI;
            newUvs2.Add(new Vector2(u, v));
        }

        mesh.uv = newUvs2.ToArray();
    }


}

[System.Serializable]
public class V3WithFace
{ 
    public Face face = Face.NA;
    public float x = -5, y = -5, z = -5;

    public V3WithFace(Face face, float x, float y, float z)
    {
        this.face = face;
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public V3WithFace(Face face, Vector3 v3)
    {
        this.face = face;
        this.x = v3.x;
        this.y = v3.y;
        this.z = v3.z;
    }

    public Vector3 GetV3()
    {
        return new Vector3(x, y, z);
    }
}
