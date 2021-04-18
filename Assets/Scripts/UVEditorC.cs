using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using GameDrawEditor;
//using GameDraw;
//using UnityEditor;
//using GD;

public class UVEditorC : MonoBehaviour {
public static Vector2[] tricounter;
 public static Vector3 angles;
    public static Vector3 box = Vector3.one;
    public static bool instantChange = true;
    public static int multi = -1;
    public static Vector2 position;
    public static float radius = 1f;
    public static Vector2 scale = Vector2.one;
    public static float sensitivity = 0.01f;
    public static Vector2[] UVUntransformed;
	public static Texture2D texsize;
public static List<int> list = new List<int>();

    public GameObject gameObject;
    public Mesh GameObjectMesh;
    // Use this for initialization
    void Start () {
       var go= Instantiate(gameObject, Vector3.zero,Quaternion.identity);
        GameObjectMesh = go.GetComponent<MeshFilter>().mesh;
        maxCamera.target = go.transform;

    }
	
	public static void Unwrap(Mesh mesh)
{
   Vector2[] vectorArray = UnwrapUVs(mesh);
    mesh.uv = vectorArray;
   //this.block = false;
}
	
	public static Vector2[] UnwrapUVs(Mesh mesh)
{//create new unwrapping
		//Unwrapping testuv = new Unwrapping();
   Vector2[] vectorArray = UnityEditor.Unwrapping.GeneratePerTriangleUV(mesh);
  	 Vector2[] triUV = UnityEditor.Unwrapping.GeneratePerTriangleUV(mesh);
        UnityEditor.MeshUtility.SetPerTriangleUV2(mesh, triUV);
    return mesh.uv2;
	}
 

        public void MakeSphere()
    {
        MakeSpherical(GameObjectMesh, 2f, true,false, false);

        }

    public void UnityUnwrap()
    {
        Unwrap(GameObjectMesh);

    }

    public static void MakeCubic(Mesh mesh, Vector3 box)
{
   //  List<int> list = new List<int>();
    Vector2[] uv = mesh.uv;
    if ((uv == null) || (uv.Length < mesh.vertexCount))
    {
        uv = new Vector2[mesh.vertexCount];
    }
		//box = Vector3.one;//new Vector3(1f / mesh.bounds.size.x, 1f / mesh.bounds.size.y, 1f / mesh.bounds.size.z);

    int[] triangles = mesh.triangles;
    for (int i = 0; i < triangles.Length; i += 3)
    {
        if ((((list.Count == 0) || list.Contains(triangles[i])) || list.Contains(triangles[i + 1])) || list.Contains(triangles[i + 2]))
        {
            Vector3 vector = mesh.vertices[triangles[i]];
            Vector3 vector2 = mesh.vertices[triangles[i + 1]];
            Vector3 vector3 = mesh.vertices[triangles[i + 2]];
            Vector3 normalized = Vector3.Cross(vector - vector3, vector2 - vector3).normalized;
            if ((Vector3.Dot(Vector3.up, normalized) > 0.5f) || (Vector3.Dot(-Vector3.up, normalized) > 0.5f))
            {
                uv[triangles[i]] = new Vector2(vector.x / box.x, vector.z / box.z);
                uv[triangles[i + 1]] = new Vector2(vector2.x / box.x, vector2.z / box.z);
                uv[triangles[i + 2]] = new Vector2(vector3.x / box.x, vector3.z / box.z);
            }
            else if ((Vector3.Dot(Vector3.right, normalized) > 0.5f) || (Vector3.Dot(Vector3.left, normalized) > 0.5f))
            {
                uv[triangles[i]] = new Vector2(vector.y / box.y, vector.z / box.z);
                uv[triangles[i + 1]] = new Vector2(vector2.y / box.y, vector2.z / box.z);
                uv[triangles[i + 2]] = new Vector2(vector3.y / box.y, vector3.z / box.z);
            }
            else
            {
                uv[triangles[i]] = new Vector2(vector.y / box.y, vector.x / box.x);
                uv[triangles[i + 1]] = new Vector2(vector2.y / box.y, vector2.x / box.x);
                uv[triangles[i + 2]] = new Vector2(vector3.y / box.y, vector3.x / box.x);
            }
        }
    }
		
	//	for (int k=0;k<uv.Length;k++)
		//	uv[k]=new Vector2(uv[k].x/texsize.width,uv[k].y/texsize.height);
		
    mesh.uv = uv;
}

public static void MakeCubicNew(Mesh mesh)
{
   //  List<int> list = new List<int>();
    Vector2[] uv = mesh.uv;
				Vector3[] normals = mesh.normals;
		Vector3[] vertices = mesh.vertices;
    if ((uv == null) || (uv.Length < mesh.vertexCount))
    {
        uv = new Vector2[mesh.vertexCount];
    }
		
    int[] triangles = mesh.triangles;
    for (int num = 0; num < mesh.vertices.Length; num++)
        {
			Vector3 Rv = vertices[num]-(2*(Vector3.Dot(normals[num],vertices[num])*normals[num])); 
			var rx=Rv.x;//vertices[num].x;
			var ry=Rv.y;//vertices[num].y;
			var rz=Rv.z;//vertices[num].z;
			float s=0;
			float t=0;
			float ma;
			float sc;
			float tc;
if((rx >= ry) && (rx  >= rz)) 
{ 
sc = -rz; 
tc = -ry; 
ma = Mathf.Abs(rx);  //absolute value
s = ((sc/ma) + 1) / 2; 
t = ((tc/ma) + 1) / 2; 

//cout << "+rx (" << s << "," << t << ")" << endl; 
} 

  if((rx <= ry) && (rx  <= rz)) 
{ 
sc = +rz; 
tc = -ry; 
ma = Mathf.Abs(rx); 
s = ((sc/ma) + 1) / 2; 
t = ((tc/ma) + 1) / 2; 

//cout << "-rx (" << s << "," << t << ")" << endl; 
} 

if((ry >= rz) && (ry >= rx)) 
{ 
sc = +rx; 
tc = +rz; 
ma = Mathf.Abs(ry); 
s = ((sc/ma) + 1) / 2; 
t = ((tc/ma) + 1) / 2; 

//cout << "+ry (" << s << "," << t << ")" << endl; 
} 

if((ry <= rz) && (ry <= rx)) 
{ 
sc = +rx; 
tc = -rz; 
ma = Mathf.Abs(ry); 
s = ((sc/ma) + 1) / 2; 
t = ((tc/ma) + 1) / 2; 

//cout << "-ry (" << s << "," << t << ")" << endl; 
} 

if((rz >= ry) && (rz >= rx)) 
{ 
sc = +rx; 
tc = -ry; 
ma = Mathf.Abs(rz); 
s = ((sc/ma) + 1) / 2; 
t = ((tc/ma) + 1) / 2; 

//cout << "+rz (" << s << "," << t << ")" << endl; 
} 

if((rz <= ry) && (rz <= rx)) 
{ 
sc = -rx; 
tc = -ry; 
ma = Mathf.Abs(rz); 
s = ((sc/ma) + 1) / 2; 
t = ((tc/ma) + 1) / 2; 

//cout << "-rz (" << s << "," << t << ")" << endl; 
} 
uv[num].x=s;
uv[num].y=t;			
      
    }
		
	//	for (int k=0;k<uv.Length;k++)
		//	uv[k]=new Vector2(uv[k].x/texsize.width,uv[k].y/texsize.height);
		
    mesh.uv = uv;
}
	
	public static void MakeCubicNew2(Mesh mesh)
{
   //  List<int> list = new List<int>();
    Vector2[] uv = mesh.uv;
				Vector3[] normals = mesh.normals;
		Vector3[] vertices = mesh.vertices;
    if ((uv == null) || (uv.Length < mesh.vertexCount))
    {
        uv = new Vector2[mesh.vertexCount];
    }
		
    int[] triangles = mesh.triangles;
    for (int num = 0; num < mesh.vertices.Length; num++)
        {
			Vector3 Rv = vertices[num];//-(2*(Vector3.Dot(normals[num],vertices[num])*normals[num])); 
			var rx=Rv.x;//vertices[num].x;
			var ry=Rv.y;//vertices[num].y;
			var rz=Rv.z;//vertices[num].z;
			float s=0;
			float t=0;
			float ma;
			float sc;
			float tc;
			
if((Mathf.Abs(rx) >= Mathf.Abs(ry)) && (Mathf.Abs(rx)  >= Mathf.Abs(rz))) 
{ 
	ma=Mathf.Abs(rx);
	sc=ry;
	tc=rz;
	s=((sc/ma)/2)+.5f;
    t=((tc/ma)/2)+.5f;
} 
			if((Mathf.Abs(rx) <= Mathf.Abs(ry)) && (Mathf.Abs(rx)  <= Mathf.Abs(rz))) 
{ 
	ma=Mathf.Abs(rx);
	sc=-ry;
	tc=-rz;
	s=((sc/ma)/2)+.5f;
    t=((tc/ma)/2)+.5f;
} 


  if((Mathf.Abs(ry) >= Mathf.Abs(rx)) && (Mathf.Abs(ry)  >= Mathf.Abs(rz))) 
{ 
	ma=Mathf.Abs(ry);
	sc=rx;
	tc=rz;
	s=((sc/ma)/2)+.5f;
    t=((tc/ma)/2)+.5f;
} 
			  if((Mathf.Abs(ry) <= Mathf.Abs(rx)) && (Mathf.Abs(ry)  <= Mathf.Abs(rz))) 
{ 
	ma=Mathf.Abs(ry);
	sc=-rx;
	tc=-rz;
	s=((sc/ma)/2)+.5f;
    t=((tc/ma)/2)+.5f;
} 

if((Mathf.Abs(rz) >= Mathf.Abs(rx)) && (Mathf.Abs(rz)  >= Mathf.Abs(ry))) 
{ 
	ma=Mathf.Abs(rz);
	sc=rx;
	tc=ry;
	s=((sc/ma)/2)+.5f;
    t=((tc/ma)/2)+.5f;

} 

			if((Mathf.Abs(rz) <= Mathf.Abs(rx)) && (Mathf.Abs(rz)  <= Mathf.Abs(ry))) 
{ 
	ma=Mathf.Abs(rz);
	sc=-rx;
	tc=-ry;
	s=((sc/ma)/2)+.5f;
    t=((tc/ma)/2)+.5f;

} 


uv[num].x=s;
uv[num].y=t;			
      
    }
		
	//	for (int k=0;k<uv.Length;k++)
		//	uv[k]=new Vector2(uv[k].x/texsize.width,uv[k].y/texsize.height);
		
    mesh.uv = uv;
}

	public static void MakePlanar(Mesh mesh)
{
    int num;
   //  List<int> list = new List<int>();
    Vector2[] uv = mesh.uv;
		int[] triangles = mesh.triangles;
    /*if ((uv == null) || (uv.Length < mesh.vertexCount))
    {
        uv = new Vector2[mesh.vertexCount];
    }*/
    if (list.Count == 0)
    {
        for (num = 0; num < mesh.vertices.Length; num++)
        {
			//	if(mesh.normals[num].z>0)
            uv[num] = new Vector2(((mesh.vertices[num].x + 0.5f)/mesh.vertices[num].z), ((mesh.vertices[num].y+0.5f)/mesh.vertices[num].z));
			//	else
			//uv[num] = new Vector2((mesh.vertices[num].x), ((mesh.vertices[num].y-mesh.vertices[num].z)));		
        }
    }
    else
    {
        for (num = 0; num < list.Count; num++)
        {
            uv[list[num]] = new Vector2(mesh.vertices[list[num]].x/mesh.vertices[list[num]].z*1/mesh.vertices[list[num]].z, mesh.vertices[list[num]].y/mesh.vertices[list[num]].z*1/mesh.vertices[list[num]].z);
        }
    }
		
	//for (int k=0;k<uv.Length;k++)
		//	uv[k]=new Vector2(uv[k].x/texsize.width,uv[k].y/texsize.height);
		
    mesh.uv = uv;
}

 


 public static void MakeSpherical(Mesh mesh, float r, bool ux, bool uy, bool uz)
{
    int num;
  //  List<int> list = new List<int>();
    Vector2[] uv = mesh.uv;
		int[] triangles = mesh.triangles;
		Vector3[] normals = mesh.normals;
		Vector3[] vertices = mesh.vertices;
		Vector3 Rv;
		
    if ((uv == null) || (uv.Length < mesh.vertexCount))
    {
        uv = new Vector2[mesh.vertexCount];
    }
    if (list.Count == 0)
    {
        for (num = 0; num < mesh.vertices.Length; num++)
        { 
				//Rv = vertices[num];//(2*Vector3.Dot(normals[num],vertices[num])*normals[num]) - vertices[num];
				r=Mathf.Sqrt((vertices[num].x*vertices[num].x)+(vertices[num].y*vertices[num].y)+(vertices[num].z*vertices[num].z));
				if(ux)
            uv[num] = new Vector2((Mathf.Atan2(mesh.vertices[num].z / r, mesh.vertices[num].y / r) / 3.141593f) / 2f, Mathf.Acos(mesh.vertices[num].x / r) / 3.141593f);
       
					if(uy)
            uv[num] = new Vector2((Mathf.Atan2(mesh.vertices[num].z / r, mesh.vertices[num].x / r) / 3.141593f) / 2f, Mathf.Acos(mesh.vertices[num].y / r) / 3.141593f);
				
					if(uz)
            uv[num] = new Vector2((Mathf.Atan2(mesh.vertices[num].y / r, mesh.vertices[num].x / r) / 3.141593f) / 2f, Mathf.Acos(mesh.vertices[num].z / r) / 3.141593f);
				// uv[num].x=Mathf.Acos(vertices[num].z/r);
			//	 uv[num].y=Mathf.Atan(vertices[num].y/vertices[num].x);
				//var ma=2*(Mathf.Sqrt(Mathf.Pow(Rv.x,2)+Mathf.Pow(Rv.y,2)+Mathf.Pow((Rv.z+1),2)));
			//uv[num].x=(Rv.x/ma)+(1/2);
			//uv[num].y=(Rv.y/ma)+(1/2);	
				//uv[num].x=Mathf.Pow(
			}
    }
    else
    {
        for (num = 0; num < list.Count; num++)
        {
				if(ux)
            uv[list[num]] = new Vector2((Mathf.Atan2(mesh.vertices[list[num]].z / r, mesh.vertices[list[num]].y / r) / 3.141593f) / 2f, Mathf.Acos(mesh.vertices[list[num]].x / r) / 3.141593f);
				if(uy)
            uv[list[num]] = new Vector2((Mathf.Atan2(mesh.vertices[list[num]].z / r, mesh.vertices[list[num]].x / r) / 3.141593f) / 2f, Mathf.Acos(mesh.vertices[list[num]].y / r) / 3.141593f);
				if(uz)
            uv[list[num]] = new Vector2((Mathf.Atan2(mesh.vertices[list[num]].y / r, mesh.vertices[list[num]].x / r) / 3.141593f) / 2f, Mathf.Acos(mesh.vertices[list[num]].z / r) / 3.141593f);
        }
    }
		
	//for (int k=0;k<uv.Length;k++)
		//	uv[k]=new Vector2(uv[k].x/texsize.width,uv[k].y/texsize.height);
		
    mesh.uv = uv;
}

public static void ScaleUV(Mesh mesh, float posx, float posy)
{
  //  List<int> list = new List<int>();
    Vector2[] uv = mesh.uv;
		int[] triangles = mesh.triangles;
		
		
    if ((uv == null) || (uv.Length < mesh.vertexCount))
    {
        uv = new Vector2[mesh.vertexCount];
    }
   /* if (UVUntransformed != null)
    {
        uv = UVUntransformed;
    }*/
    if (list.Count == 0)
    {
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i].Scale(new Vector2(posx, posy));
        }
    }
    else
    {
        for (int j = 0; j < list.Count; j++)
        {
            uv[list[j]].Scale(new Vector2(posx, posy));
        }
    }
    mesh.uv = uv;
}
public static void RotateUV(Mesh mesh, float Anglex, float Angley, float Anglez)
{
  //  List<int> list = new List<int>();
    Vector2[] uv = mesh.uv;
		int[] triangles = mesh.triangles;
   /* if ((UVUntransformed != null) || (UVUntransformed.Length < mesh.vertexCount))
    {
        uv = UVUntransformed;
    }*/
    if ((uv == null) || (uv.Length < mesh.vertexCount))
    {
        uv = new Vector2[mesh.vertexCount];
    }
    if (list.Count == 0)
    {
        for (int i = 0; i < uv.Length; i++)
        {
            Quaternion quaternion = Quaternion.Euler(Anglex, Angley, Anglez);
            uv[i] = (Vector2) (quaternion * uv[i]);
        }
    }
    else
    {
        for (int j = 0; j < list.Count; j++)
        {
            Quaternion quaternion2 = Quaternion.Euler(Anglex, Angley, Anglez);
            uv[list[j]] = (Vector2) (quaternion2 * uv[list[j]]);
        }
    }
    mesh.uv = uv;
}


 public static void TranslateUV(Mesh mesh, float posx, float posy)
{
   // List<int> list = new List<int>();
    Vector2[] uv = mesh.uv;
	int[] triangles = mesh.triangles;
		
		
    if ((uv == null) || (uv.Length < mesh.vertexCount))
    {
        uv = new Vector2[mesh.vertexCount];
    }
   /* if ((UVUntransformed != null) || (UVUntransformed.Length < mesh.vertexCount))
    {
        uv = UVUntransformed;
    }*/
    if (list.Count == 0)
    {
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] += new Vector2(posx * sensitivity, posy * sensitivity);
        }
    }
    else
    {
        for (int j = 0; j < list.Count; j++)
        {
            uv[list[j]] += new Vector2(posx * sensitivity, posy * sensitivity);
        }
    }
    mesh.uv = uv;
}

 

 


 

 


 

 

 


 


 


 


 

 


 

}
