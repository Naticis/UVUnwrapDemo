using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class MeshPainter : MonoBehaviour {
    private Vector2 vector1;

    // Use this for initialization
    public static Texture2D baseTex;
	public static Texture2D undo;
	public static bool textureToggle;
	public static Texture2D currentTexture;
	//public static float width=20f;
	//public static Color color1=Color.green;
	//public static float hardness=50f;
public static Vector2 dragEnd;
	public static Vector2 dragStart;
	public static int lastTriangleIndex;
	public Camera cam;
	private GameObject tempObject;
    private Vector2 pixelUV;
    private Vector2 previous;
    private RaycastHit hit;
    private Texture2D tex;
    private float paintradius;
    private float radius;
    void Start () {
        radius = 0.1f;
	}
   
    // Update is called once per frame
    void Update()
    {

        if (maxCamera.rotateindicator)
            return;

        if (!Input.GetMouseButton(0))
        {
            previous = Vector2.zero;
            vector1 = Vector2.zero;

            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            tex = (Texture2D)hit.transform.gameObject.GetComponent<Renderer>().material.mainTexture;

        }
            if (Input.GetMouseButtonDown(0))
        {
           
          
          
                pixelUV = Vector2.zero;//= hit.textureCoord;
                pixelUV.x = Mathf.Abs((hit.textureCoord.x - Mathf.Floor(hit.textureCoord.x)));
                pixelUV.y = Mathf.Abs((hit.textureCoord.y - Mathf.Floor(hit.textureCoord.y)));
                pixelUV.x *= (float)tex.width;
                pixelUV.y *= (float)tex.height;
                vector1 = pixelUV;
                paintradius = tex.width * radius * radius;
                      baseTex = tex;

                Brush(vector1, previous, tex, paintradius, 50f, Color.green);
            

        }
        else if (Input.GetMouseButton(0))
        {
            pixelUV.x = Mathf.Abs((hit.textureCoord.x - Mathf.Floor(hit.textureCoord.x)));
            pixelUV.y = Mathf.Abs((hit.textureCoord.y - Mathf.Floor(hit.textureCoord.y)));
            pixelUV.x *= (float)tex.width;
            pixelUV.y *= (float)tex.height;
            vector1 = pixelUV;

            baseTex = tex;

            Brush(vector1, previous, tex, paintradius, 50f, Color.green);
            previous = vector1;

        } else if (Input.GetMouseButtonUp(0))
        {
            previous = Vector2.zero;
            vector1 = Vector2.zero;
        }
    }

 public void SetRadius(float value)
    {
        radius = value;
    }

    public static float LogarithmicSlider(float position, float minIn, float maxIn, float minOut, float maxOut)
	{
		float num = minIn;
		float num2 = maxIn;
		float num3 = Mathf.Log(minOut);
		float num5 = (Mathf.Log(maxOut) - num3) / (num2 - num);
		return Mathf.Exp(num3 + (num5 * (position - num)));
	}
	
	public static void Brush(Vector2 p1, Vector2 p2, Texture2D currentTexture, float width,float hardness,Color color1 )
	{


		//Drawing.NumSamples = this.AntiAlias;
		if (p2 == Vector2.zero)
		{
			p2 = p1;
		}
		PaintLine(p1, p2, width, color1, LogarithmicSlider(hardness, 1f, 100f, 0.01f, 500f), currentTexture);
		if (currentTexture != null)
		{
			currentTexture.Apply();
		}
	}
	public static void BrushSpray(Vector2 p1, Vector2 p2,Vector2 p3, Vector2 p4, Texture2D currentTexture, float width,float hardness,Color color1 )
	{
		//Drawing.NumSamples = this.AntiAlias;


		if (p2 == Vector2.zero)
		{
			p2 = p1;
		}
		if (p4 == Vector2.zero)
		{
			p4 = p3;
		}
		PaintLineSpray(p1, p2,p3, p4, width, color1, LogarithmicSlider(hardness, 1f, 100f, 0.01f, 500f), currentTexture);
		if (currentTexture != null)
		{
			currentTexture.Apply();
		}
	}
	public static Texture2D PaintLine(Vector2 from, Vector2 to, float rad, Color col, float hardness, Texture2D tex)
	{
		if (tex == null)
		{
			return null;
		}
		if (from == to)
		{
			from.x = to.x - 1f;
		}
		float num = rad;
		int y = Mathf.FloorToInt(Mathf.Clamp(Mathf.Min(from.y, to.y) - num, 0f, (float) tex.height));
		int x = Mathf.FloorToInt(Mathf.Clamp(Mathf.Min(from.x, to.x) - num, 0f, (float) tex.width));
		int num4 = Mathf.FloorToInt(Mathf.Clamp(Mathf.Max(from.y, to.y) + num, 0f, (float) tex.height));
		int blockWidth = Mathf.FloorToInt(Mathf.Clamp(Mathf.Max(from.x, to.x) + num, 0f, (float) tex.width)) - x;
		int blockHeight = num4 - y;
		float num8 = (rad + 1f) * (rad + 1f);
		Color[] colors = tex.GetPixels(x, y, blockWidth, blockHeight, 0);
		Color[] colorArray2 = baseTex.GetPixels(x, y, blockWidth, blockHeight, 0);
		Vector2 vector = new Vector2((float) x, (float) y);
		for (int i = 0; i < blockHeight; i++)
		{
			for (int j = 0; j < blockWidth; j++)
			{
				Vector2 vector2 = new Vector2((float) j, (float) i) + vector;
				Vector2 point = vector2 + new Vector2(0.5f, 0.5f);
				Vector2 vector4 = NearestPointStrict(from, to, point);
				Vector2 vector5 = point - vector4;
				float sqrMagnitude = vector5.sqrMagnitude;
				if (sqrMagnitude <= num8)
				{
					Color color;
					sqrMagnitude = GaussFalloff(Mathf.Sqrt(sqrMagnitude), rad) * hardness;
					if (sqrMagnitude > 0f)
					{

                   

							color = Color.Lerp(colors[(i * blockWidth) + j], col, sqrMagnitude);
						
					}
					else
					{
						color = colors[(i * blockWidth) + j];
					}
					colors[(i * blockWidth) + j] = color;
				}
			}
		}
		tex.SetPixels((int) vector.x, (int) vector.y, blockWidth, blockHeight, colors, 0);
		return tex;
	}
	public static Texture2D PaintLineSpray(Vector2 from, Vector2 to, Vector2 from2, Vector2 to2,float rad, Color col, float hardness, Texture2D tex)
	{
		if (tex == null)
		{
			return null;
		}
		if (from == to)
		{
			from.x = to.x - 1f;
		}
		if (from2 == to2)
		{
			from2.x = to2.x - 1f;
		}
		float num = rad;
		int y = Mathf.FloorToInt(Mathf.Clamp(Mathf.Min(from.y, to.y) - num, 0f, (float) tex.height));
		int x = Mathf.FloorToInt(Mathf.Clamp(Mathf.Min(from.x, to.x) - num, 0f, (float) tex.width));
		int y2 = Mathf.FloorToInt(Mathf.Clamp(Mathf.Min(from2.y, to2.y) - num, 0f, (float) baseTex.height));
		int x2 = Mathf.FloorToInt(Mathf.Clamp(Mathf.Min(from2.x, to2.x) - num, 0f, (float) baseTex.width));
		int num4 = Mathf.FloorToInt(Mathf.Clamp(Mathf.Max(from.y, to.y) + num, 0f, (float) tex.height));
		int blockWidth = Mathf.FloorToInt(Mathf.Clamp(Mathf.Max(from.x, to.x) + num, 0f, (float) tex.width)) - x;
		int blockHeight = num4 - y;
		int num42 = Mathf.FloorToInt(Mathf.Clamp(Mathf.Max(from2.y, to2.y) + num, 0f, (float) baseTex.height));
		int blockWidth2 = Mathf.FloorToInt(Mathf.Clamp(Mathf.Max(from2.x, to2.x) + num, 0f, (float) baseTex.width)) - x2;
		int blockHeight2 = num42 - y2;
		float num8 = (rad + 1f) * (rad + 1f);
		//Color[] colors = undo.GetPixels(x, y, blockWidth, blockHeight, 0);
		Color[] colors = tex.GetPixels(x, y, blockWidth, blockHeight, 0);
		Color[] colorArray2 = baseTex.GetPixels(x2, y2, blockWidth2, blockHeight2, 0);
		Vector2 vector = new Vector2((float) x, (float) y);
		Vector2 vector_2 = new Vector2((float) x2, (float) y2);
		for (int i = 0; i < blockHeight2; i++)
		{
			for (int j = 0; j < blockWidth2; j++)
			{
				Vector2 vector2 = new Vector2((float) j, (float) i) + vector;
				Vector2 point = vector2 + new Vector2(0.5f, 0.5f);
				Vector2 vector4 = NearestPointStrict(from, to, point);
				Vector2 vector5 = point - vector4;
				float sqrMagnitude = vector5.sqrMagnitude;
				//---
				Vector2 vector2_2 = new Vector2((float) j, (float) i) + vector_2;
				Vector2 point2 = vector2_2 + new Vector2(0.5f, 0.5f);
				Vector2 vector4_2 = NearestPointStrict(from2, to2, point2);
				Vector2 vector5_2 = point2 - vector4_2;
				float sqrMagnitude_2 = vector5_2.sqrMagnitude;

				if (sqrMagnitude_2 <= num8)
				{
					Color color;
					sqrMagnitude = GaussFalloff(Mathf.Sqrt(sqrMagnitude_2), rad) * hardness;
					if (sqrMagnitude > 0f)
					{
						if (textureToggle)
						{
							color = Color.Lerp(colors[(i * blockWidth) + j], colorArray2[(i * blockWidth2) + j], sqrMagnitude);
						}
						else
						{
							color = Color.Lerp(colors[(i * blockWidth) + j], col, sqrMagnitude);
						}
					}
					else
					{
						color = colors[(i * blockWidth) + j];
					}
					colors[(i * blockWidth) + j] = color;
				}
			}
		}
		tex.SetPixels((int) vector.x, (int) vector.y, blockWidth, blockHeight, colors, 0);
		return tex;
	}
	

	public static float GaussFalloff(float distance, float inRadius)
	{
		return Mathf.Clamp01(Mathf.Pow(360f, -Mathf.Pow(distance / inRadius, 2.5f) - 0.01f));
	}
	
	
	public static Vector2 NearestPointStrict(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
	{
		Vector2 p = lineEnd - lineStart;
		Vector2 rhs = Normalize(p);
		float num = Vector2.Dot(point - lineStart, rhs) / Vector2.Dot(rhs, rhs);
		return (lineStart + ((Vector2) (Mathf.Clamp(num, 0f, p.magnitude) * rhs)));
	}
	public static Vector2 Normalize(Vector2 p)
	{
		float magnitude = p.magnitude;
		return (Vector2) (p / magnitude);
	}
	
	public static void UpdateUVs(GameObject workingGameObject, Mesh meshOriginal)
	{
		Vector3[] vertices = new Vector3[meshOriginal.vertices.Length];
		int[] triangles = new int[meshOriginal.triangles.Length];
		Vector2[] uv = new Vector2[meshOriginal.uv.Length];
		Vector3[] normals = meshOriginal.normals;
		vertices = meshOriginal.vertices;
		triangles = meshOriginal.triangles;
		uv = meshOriginal.uv;
		Vector3[] vectorArray3 = meshOriginal.normals;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector3 vector = workingGameObject.transform.TransformPoint(vertices[triangles[i]]);
			Vector3 vector2 = workingGameObject.transform.TransformPoint(vertices[triangles[i + 1]]);
			Vector3 vector3 = workingGameObject.transform.TransformPoint(vertices[triangles[i + 2]]);
			Vector3 normalized = Vector3.Cross(vector - vector3, vector2 - vector3).normalized;
			//Debug.Log(vector+vector2+vector3);
			if ((Vector3.Dot(Vector3.up, normalized) > 0.5) || (Vector3.Dot(-Vector3.up, normalized) > 0.5))
			{
				//Debug.Log("First");
				uv[triangles[i]] = new Vector2(vector.x, vector.z);
				uv[triangles[i + 1]] = new Vector2(vector2.x, vector2.z);
				uv[triangles[i + 2]] = new Vector2(vector3.x, vector3.z);
			}
			else if ((Vector3.Dot(Vector3.right, normalized) > 0.5) || (Vector3.Dot(Vector3.left, normalized) > 0.5))
			{
				//Debug.Log("Second");
				uv[triangles[i]] = new Vector2(vector.z, vector.y);
				uv[triangles[i + 1]] = new Vector2(vector2.z, vector2.y);
				uv[triangles[i + 2]] = new Vector2(vector3.z, vector3.y);
			}
			else
			{
				//Debug.Log("Third");
				uv[triangles[i]] = new Vector2(vector.x, vector.y);
				uv[triangles[i + 1]] = new Vector2(vector2.x, vector2.y);
				uv[triangles[i + 2]] = new Vector2(vector3.x, vector3.y);
			}
		}

		meshOriginal.uv = uv;
	}
	public static void UpdateMovingUVs(GameObject workingGameObject, Mesh meshOriginal)
	{
		Vector3[] vertices = new Vector3[meshOriginal.vertices.Length];
		int[] triangles = new int[meshOriginal.triangles.Length];
		Vector2[] uv = new Vector2[meshOriginal.uv.Length];
		Vector3[] normals = new Vector3[meshOriginal.normals.Length];
		vertices = meshOriginal.vertices;
		triangles = meshOriginal.triangles;
		uv = meshOriginal.uv;
		normals = meshOriginal.normals;
		for (int i = 0; i < triangles.Length; i += 3)
		{
		
			{//holdv
				Vector3 vector = workingGameObject.transform.TransformPoint(vertices[triangles[i]]);
				Vector3 vector2 = workingGameObject.transform.TransformPoint(vertices[triangles[i + 1]]);
				Vector3 vector3 = workingGameObject.transform.TransformPoint(vertices[triangles[i + 2]]);
				Vector3 normalized = Vector3.Cross(vector - vector3, vector2 - vector3).normalized;
				normals[triangles[i]] = new Vector3(normalized.x, normalized.y, normalized.z);
				normals[triangles[i + 1]] = new Vector3(normalized.x, normalized.y, normalized.z);
				normals[triangles[i + 2]] = new Vector3(normalized.x, normalized.y, normalized.z);
				if ((Vector3.Dot(Vector3.up, normalized) > 0.5) || (Vector3.Dot(-Vector3.up, normalized) > 0.5))
				{
					uv[triangles[i]] = new Vector2(vector.x, vector.z);
					uv[triangles[i + 1]] = new Vector2(vector2.x, vector2.z);
					uv[triangles[i + 2]] = new Vector2(vector3.x, vector3.z);
				}
				else if ((Vector3.Dot(Vector3.right, normalized) > 0.5) || (Vector3.Dot(Vector3.left, normalized) > 0.5))
				{
					uv[triangles[i]] = new Vector2(vector.z, vector.y);
					uv[triangles[i + 1]] = new Vector2(vector2.z, vector2.y);
					uv[triangles[i + 2]] = new Vector2(vector3.z, vector3.y);
				}
				else
				{
					uv[triangles[i]] = new Vector2(vector.x, vector.y);
					uv[triangles[i + 1]] = new Vector2(vector2.x, vector2.y);
					uv[triangles[i + 2]] = new Vector2(vector3.x, vector3.y);
				}
			}
		}
		//meshOriginal.uv = uv;
		//meshOriginal.normals = normals;
	}
	
	/*public static void MeshPainterOnSceneGUI()
	{
		Event current = Event.current;
		this.cam = Camera.current;
	

		Event event3 = Event.current;
		new Rect(0f, 0f, (float) (baseTex.width * this.zoom), (float) (baseTex.height * this.zoom));
		Vector2 mousePosition = current.mousePosition;
		mousePosition.y = Screen.height - mousePosition.y;
		switch (event3.type)
		{
		case EventType.MouseDown:
		{
			RaycastHit hit2;
			if (current.button != 1)
			{
				break;
			}
			Ray ray2 = Camera.main.ScreenPointToRay(new Vector3(current.mousePosition.x, Screen.height - (current.mousePosition.y + 35f), 0f));
			bool flag4 = false;
			Vector2 vector3 = new Vector2(0f, 0f);
			if (!Physics.Raycast(ray2, out hit2))
			{
				break;
			}
			MeshCollider collider2 = hit2.collider as MeshCollider;
			if ((collider2 != null) && (collider2.sharedMesh != null))
			{
				lastTriangleIndex = hit2.triangleIndex;
				bool flag5 = false;
				foreach (Transform transform2 in hit2.transform)
				{
					int[] triangles;
					if (!(transform2.name == "mpCanvas"))
					{
						continue;
					}
					tempObject = transform2.gameObject;
					MeshFilter component = (MeshFilter) this.tempObject.GetComponent(typeof(MeshFilter));
					Mesh sharedMesh = component.sharedMesh;
					Vector3[] vertices = new Vector3[sharedMesh.vertices.Length];
					vertices = sharedMesh.vertices;
					for (int j = 1; j < sharedMesh.subMeshCount; j++)
					{
						triangles = sharedMesh.GetTriangles(j);
						vertices[triangles[0]] = sharedMesh.vertices[triangles[0]];
						vertices[triangles[0]].x *= hit2.transform.localScale.x;
						vertices[triangles[0]].y *= hit2.transform.localScale.y;
						vertices[triangles[0]].z *= hit2.transform.localScale.z;
						vertices[triangles[1]] = sharedMesh.vertices[triangles[1]];
						vertices[triangles[1]].x *= hit2.transform.localScale.x;
						vertices[triangles[1]].y *= hit2.transform.localScale.y;
						vertices[triangles[1]].z *= hit2.transform.localScale.z;
						vertices[triangles[2]] = sharedMesh.vertices[triangles[2]];
						vertices[triangles[2]].x *= hit2.transform.localScale.x;
						vertices[triangles[2]].y *= hit2.transform.localScale.y;
						vertices[triangles[2]].z *= hit2.transform.localScale.z;
						vertices[triangles[0]] = (Vector3) (hit2.transform.rotation * vertices[triangles[0]]);
						vertices[triangles[0]] += hit2.transform.position;
						vertices[triangles[1]] = (Vector3) (hit2.transform.rotation * vertices[triangles[1]]);
						vertices[triangles[1]] += hit2.transform.position;
						vertices[triangles[2]] = (Vector3) (hit2.transform.rotation * vertices[triangles[2]]);
						vertices[triangles[2]] += hit2.transform.position;
						if (PointInPoly(hit2.point, vertices[triangles[0]], vertices[triangles[1]], vertices[triangles[2]], hit2.normal))
						{
							currentTexture = (Texture2D) this.tempObject.renderer.sharedMaterials[j].mainTexture;
							flag4 = true;
							vector3.x = Mathf.Abs((float) (hit2.textureCoord.x - Mathf.Floor(hit2.textureCoord.x)));
							vector3.y = Mathf.Abs((float) (hit2.textureCoord.y - Mathf.Floor(hit2.textureCoord.y)));
							flag5 = true;
							break;
						}
					}
					if (!flag5)
					{
						CreateSubmesh(hit2, transform2);
						tempObject = transform2.gameObject;
						component = (MeshFilter) this.tempObject.GetComponent(typeof(MeshFilter));
						sharedMesh = component.sharedMesh;
						vertices = new Vector3[sharedMesh.vertices.Length];
						vertices = sharedMesh.vertices;
						for (int k = 1; k < sharedMesh.subMeshCount; k++)
						{
							triangles = sharedMesh.GetTriangles(k);
							vertices[triangles[0]] = sharedMesh.vertices[triangles[0]];
							vertices[triangles[0]].x *= hit2.transform.localScale.x;
							vertices[triangles[0]].y *= hit2.transform.localScale.y;
							vertices[triangles[0]].z *= hit2.transform.localScale.z;
							vertices[triangles[1]] = sharedMesh.vertices[triangles[1]];
							vertices[triangles[1]].x *= hit2.transform.localScale.x;
							vertices[triangles[1]].y *= hit2.transform.localScale.y;
							vertices[triangles[1]].z *= hit2.transform.localScale.z;
							vertices[triangles[2]] = sharedMesh.vertices[triangles[2]];
							vertices[triangles[2]].x *= hit2.transform.localScale.x;
							vertices[triangles[2]].y *= hit2.transform.localScale.y;
							vertices[triangles[2]].z *= hit2.transform.localScale.z;
							vertices[triangles[0]] = (Vector3) (hit2.transform.rotation * vertices[triangles[0]]);
							vertices[triangles[0]] += hit2.transform.position;
							vertices[triangles[1]] = (Vector3) (hit2.transform.rotation * vertices[triangles[1]]);
							vertices[triangles[1]] += hit2.transform.position;
							vertices[triangles[2]] = (Vector3) (hit2.transform.rotation * vertices[triangles[2]]);
							vertices[triangles[2]] += hit2.transform.position;
							if (PointInPoly(hit2.point, vertices[triangles[0]], vertices[triangles[1]], vertices[triangles[2]], hit2.normal))
							{
								currentTexture = (Texture2D) tempObject.renderer.sharedMaterials[k].mainTexture;
								flag4 = true;
								vector3.x = Mathf.Abs((float) (hit2.textureCoord.x - Mathf.Floor(hit2.textureCoord.x)));
								vector3.y = Mathf.Abs((float) (hit2.textureCoord.y - Mathf.Floor(hit2.textureCoord.y)));
								flag5 = true;
								break;
							}
						}
					}
				}
				if (!flag5 && (tool != Tool.Eraser))
				{
					this.tempObject = this.CreateCanvas(hit2, "mpCanvas");
					this.lastTexture = currentTexture;
					this.tempObject.transform.parent = hit2.transform;
				}
				if (!flag4)
				{
					this.dragStart.x = hit2.textureCoord.x * 512f;
					this.dragStart.y = hit2.textureCoord.y * 512f;
				}
				else
				{
					this.dragStart.x = vector3.x * 512f;
					this.dragStart.y = vector3.y * 512f;
				}
				this.preDrag = Vector2.zero;
				if (!flag4)
				{
					this.dragEnd.x = hit2.textureCoord.x * 512f;
					this.dragEnd.y = hit2.textureCoord.y * 512f;
				}
				else
				{
					this.dragEnd.x = vector3.x * 512f;
					this.dragEnd.y = vector3.y * 512f;
				}
				if (tool == Tool.Brush)
				{
					this.Brush(this.dragEnd, this.preDrag);
				}
				if (tool == Tool.Eraser)
				{
					this.Eraser(this.dragEnd, this.preDrag);
				}
				this.preDrag = this.dragEnd;
				current.Use();
				break;
			}
			if (!this.errorMessageFlag)
			{
				Debug.Log("Object must have a Mesh Collider");
			}
			this.errorMessageFlag = true;
			return;
		}
		case EventType.MouseUp:
			this.errorMessageFlag = false;
			this.preDrag = Vector2.zero;
			return;
			
		case EventType.MouseMove:
			break;
			
		case EventType.MouseDrag:
		{
			RaycastHit hit;
			if (!this.hotkey || (current.button != 1))
			{
				break;
			}
			bool flag = false;
			bool flag2 = false;
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(current.mousePosition.x, Screen.height - (current.mousePosition.y + 35f), 0f));
			Vector2 vector2 = new Vector2(0f, 0f);
			if (!Physics.Raycast(ray, out hit))
			{
				break;
			}
			MeshCollider collider = hit.collider as MeshCollider;
			if ((collider != null) && (collider.sharedMesh != null))
			{
				if (this.dragStart != Vector2.zero)
				{
					if (this.lastTriangleIndex != hit.triangleIndex)
					{
						flag = true;
					}
					bool flag3 = false;
					foreach (Transform transform in hit.transform)
					{
						int[] numArray;
						if (!(transform.name == "mpCanvas"))
						{
							continue;
						}
						this.tempObject = transform.gameObject;
						MeshFilter filter = (MeshFilter) this.tempObject.GetComponent(typeof(MeshFilter));
						Mesh mesh = filter.sharedMesh;
						Vector3[] vectorArray = new Vector3[mesh.vertices.Length];
						vectorArray = mesh.vertices;
						for (int m = 1; m < mesh.subMeshCount; m++)
						{
							numArray = mesh.GetTriangles(m);
							vectorArray[numArray[0]] = mesh.vertices[numArray[0]];
							vectorArray[numArray[0]].x *= hit.transform.localScale.x;
							vectorArray[numArray[0]].y *= hit.transform.localScale.y;
							vectorArray[numArray[0]].z *= hit.transform.localScale.z;
							vectorArray[numArray[1]] = mesh.vertices[numArray[1]];
							vectorArray[numArray[1]].x *= hit.transform.localScale.x;
							vectorArray[numArray[1]].y *= hit.transform.localScale.y;
							vectorArray[numArray[1]].z *= hit.transform.localScale.z;
							vectorArray[numArray[2]] = mesh.vertices[numArray[2]];
							vectorArray[numArray[2]].x *= hit.transform.localScale.x;
							vectorArray[numArray[2]].y *= hit.transform.localScale.y;
							vectorArray[numArray[2]].z *= hit.transform.localScale.z;
							vectorArray[numArray[0]] = (Vector3) (hit.transform.rotation * vectorArray[numArray[0]]);
							vectorArray[numArray[0]] += hit.transform.position;
							vectorArray[numArray[1]] = (Vector3) (hit.transform.rotation * vectorArray[numArray[1]]);
							vectorArray[numArray[1]] += hit.transform.position;
							vectorArray[numArray[2]] = (Vector3) (hit.transform.rotation * vectorArray[numArray[2]]);
							vectorArray[numArray[2]] += hit.transform.position;
							if (this.PointInPoly(hit.point, vectorArray[numArray[0]], vectorArray[numArray[1]], vectorArray[numArray[2]], hit.normal))
							{
								currentTexture = (Texture2D) this.tempObject.renderer.sharedMaterials[m].mainTexture;
								flag2 = true;
								vector2.x = Mathf.Abs((float) (hit.textureCoord.x - Mathf.Floor(hit.textureCoord.x)));
								vector2.y = Mathf.Abs((float) (hit.textureCoord.y - Mathf.Floor(hit.textureCoord.y)));
								if (this.lastTexture != currentTexture)
								{
									flag = true;
								}
								this.lastTexture = currentTexture;
								flag3 = true;
								break;
							}
						}
						if (!flag3)
						{
							this.CreateSubmesh(hit, transform);
							this.tempObject = transform.gameObject;
							filter = (MeshFilter) this.tempObject.GetComponent(typeof(MeshFilter));
							mesh = filter.sharedMesh;
							vectorArray = new Vector3[mesh.vertices.Length];
							vectorArray = mesh.vertices;
							for (int n = 1; n < mesh.subMeshCount; n++)
							{
								numArray = mesh.GetTriangles(n);
								vectorArray[numArray[0]] = mesh.vertices[numArray[0]];
								vectorArray[numArray[0]].x *= hit.transform.localScale.x;
								vectorArray[numArray[0]].y *= hit.transform.localScale.y;
								vectorArray[numArray[0]].z *= hit.transform.localScale.z;
								vectorArray[numArray[1]] = mesh.vertices[numArray[1]];
								vectorArray[numArray[1]].x *= hit.transform.localScale.x;
								vectorArray[numArray[1]].y *= hit.transform.localScale.y;
								vectorArray[numArray[1]].z *= hit.transform.localScale.z;
								vectorArray[numArray[2]] = mesh.vertices[numArray[2]];
								vectorArray[numArray[2]].x *= hit.transform.localScale.x;
								vectorArray[numArray[2]].y *= hit.transform.localScale.y;
								vectorArray[numArray[2]].z *= hit.transform.localScale.z;
								vectorArray[numArray[0]] = (Vector3) (hit.transform.rotation * vectorArray[numArray[0]]);
								vectorArray[numArray[0]] += hit.transform.position;
								vectorArray[numArray[1]] = (Vector3) (hit.transform.rotation * vectorArray[numArray[1]]);
								vectorArray[numArray[1]] += hit.transform.position;
								vectorArray[numArray[2]] = (Vector3) (hit.transform.rotation * vectorArray[numArray[2]]);
								vectorArray[numArray[2]] += hit.transform.position;
								if (this.PointInPoly(hit.point, vectorArray[numArray[0]], vectorArray[numArray[1]], vectorArray[numArray[2]], hit.normal))
								{
									currentTexture = (Texture2D) this.tempObject.renderer.sharedMaterials[n].mainTexture;
									flag2 = true;
									vector2.x = Mathf.Abs((float) (hit.textureCoord.x - Mathf.Floor(hit.textureCoord.x)));
									vector2.y = Mathf.Abs((float) (hit.textureCoord.y - Mathf.Floor(hit.textureCoord.y)));
									if (this.lastTexture != currentTexture)
									{
										flag = true;
									}
									this.lastTexture = currentTexture;
									flag3 = true;
									break;
								}
							}
						}
					}
					if (!flag3 && (tool != Tool.Eraser))
					{
						this.tempObject = this.CreateCanvas(hit, "mpCanvas");
						this.lastTexture = currentTexture;
						this.tempObject.transform.parent = hit.transform;
					}
					this.lastTriangleIndex = hit.triangleIndex;
					if (!flag2)
					{
						this.dragEnd.x = hit.textureCoord.x * 512f;
						this.dragEnd.y = hit.textureCoord.y * 512f;
					}
					else
					{
						this.dragEnd.x = vector2.x * 512f;
						this.dragEnd.y = vector2.y * 512f;
					}
					if (flag)
					{
						this.preDrag = this.dragEnd;
					}
					if (tool == Tool.Brush)
					{
						this.Brush(this.dragEnd, this.preDrag);
					}
					if (tool == Tool.Eraser)
					{
						this.Eraser(this.dragEnd, this.preDrag);
					}
					this.preDrag = this.dragEnd;
					current.Use();
				}
				return;
			}
			if (!this.errorMessageFlag)
			{
				Debug.Log("Object must have a Mesh Collider");
			}
			this.errorMessageFlag = true;
			return;
		}
		default:
			return;
		}
	}
	
	public void CreateSubmesh(RaycastHit hit, Transform child)
	{
		this.tempObject = hit.transform.gameObject;
		MeshFilter component = (MeshFilter) this.tempObject.GetComponent(typeof(MeshFilter));
		Mesh sharedMesh = component.sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		Vector2[] uv = sharedMesh.uv;
		int[] triangles = sharedMesh.triangles;
		IntPoint[] pointArray = new IntPoint[4];
		pointArray[0].X = (long) (Mathf.Floor(hit.textureCoord.x) * 1000000f);
		pointArray[0].Y = (long) (Mathf.Floor(hit.textureCoord.y) * 1000000f);
		pointArray[1].X = (long) (Mathf.Floor(hit.textureCoord.x) * 1000000f);
		pointArray[1].Y = (long) (Mathf.Ceil(hit.textureCoord.y) * 1000000f);
		pointArray[2].X = (long) (Mathf.Ceil(hit.textureCoord.x) * 1000000f);
		pointArray[2].Y = (long) (Mathf.Ceil(hit.textureCoord.y) * 1000000f);
		pointArray[3].X = (long) (Mathf.Ceil(hit.textureCoord.x) * 1000000f);
		pointArray[3].Y = (long) (Mathf.Floor(hit.textureCoord.y) * 1000000f);
		List<IntPoint> list = new List<IntPoint> {
			pointArray[0],
			pointArray[1],
			pointArray[2],
			pointArray[3]
		};
		List<List<IntPoint>> ppg = new List<List<IntPoint>> {
			list
		};
		List<IntPoint> list3 = new List<IntPoint>();
		IntPoint[] pointArray2 = new IntPoint[3];
		pointArray2[0].X = (long) (uv[triangles[hit.triangleIndex * 3]].x * 1000000f);
		pointArray2[0].Y = (long) (uv[triangles[hit.triangleIndex * 3]].y * 1000000f);
		pointArray2[1].X = (long) (uv[triangles[(hit.triangleIndex * 3) + 1]].x * 1000000f);
		pointArray2[1].Y = (long) (uv[triangles[(hit.triangleIndex * 3) + 1]].y * 1000000f);
		pointArray2[2].X = (long) (uv[triangles[(hit.triangleIndex * 3) + 2]].x * 1000000f);
		pointArray2[2].Y = (long) (uv[triangles[(hit.triangleIndex * 3) + 2]].y * 1000000f);
		list3.Add(pointArray2[0]);
		list3.Add(pointArray2[1]);
		list3.Add(pointArray2[2]);
		List<List<IntPoint>> list4 = new List<List<IntPoint>> {
			list3
		};
		List<List<IntPoint>> solution = new List<List<IntPoint>>();
		Clipper clipper = new Clipper(0);
		clipper.AddPaths(ppg, PolyType.ptSubject, true);
		clipper.AddPaths(list4, PolyType.ptClip, true);
		clipper.Execute(ClipType.ctIntersection, solution);
		Vector2[] points = new Vector2[100];
		int[] numArray2 = new int[100];
		foreach (List<IntPoint> list6 in solution)
		{
			points = new Vector2[list6.Count];
			int index = 0;
			foreach (IntPoint point in list6)
			{
				points[index].x = ((float) point.X) / 1000000f;
				points[index].y = ((float) point.Y) / 1000000f;
				index++;
			}
			numArray2 = new Triangulator(points).Triangulate();
		}
		this.tempObject = child.gameObject;
		component = (MeshFilter) this.tempObject.GetComponent(typeof(MeshFilter));
		sharedMesh = component.sharedMesh;
		if (sharedMesh != null)
		{
			List<Vector3> list7 = new List<Vector3>(sharedMesh.vertices);
			List<Vector2> list8 = new List<Vector2>(sharedMesh.uv);
			List<Vector3> list9 = new List<Vector3>(sharedMesh.normals);
			List<Material> list10 = new List<Material>(this.tempObject.renderer.sharedMaterials);
			Material item = new Material(Shader.Find("Custom/Transparent"));
			item.SetTextureScale("_MainTex", new Vector2(1f, 1f));
			for (int i = 0; i < numArray2.Length; i += 3)
			{
				Vector3[] vectorArray4 = new Vector3[3];
				Vector2[] vectorArray5 = new Vector2[3];
				Vector3[] vectorArray6 = new Vector3[3];
				int[] numArray3 = new int[] { sharedMesh.vertices.Length, sharedMesh.vertices.Length + 1, sharedMesh.vertices.Length + 2 };
				vectorArray4[0] = this.Get3DFromUV(hit.transform, points[numArray2[i]], uv[triangles[hit.triangleIndex * 3]], uv[triangles[(hit.triangleIndex * 3) + 1]], uv[triangles[(hit.triangleIndex * 3) + 2]], vertices[triangles[hit.triangleIndex * 3]], vertices[triangles[(hit.triangleIndex * 3) + 1]], vertices[triangles[(hit.triangleIndex * 3) + 2]]);
				vectorArray4[1] = this.Get3DFromUV(hit.transform, points[numArray2[i + 1]], uv[triangles[hit.triangleIndex * 3]], uv[triangles[(hit.triangleIndex * 3) + 1]], uv[triangles[(hit.triangleIndex * 3) + 2]], vertices[triangles[hit.triangleIndex * 3]], vertices[triangles[(hit.triangleIndex * 3) + 1]], vertices[triangles[(hit.triangleIndex * 3) + 2]]);
				vectorArray4[2] = this.Get3DFromUV(hit.transform, points[numArray2[i + 2]], uv[triangles[hit.triangleIndex * 3]], uv[triangles[(hit.triangleIndex * 3) + 1]], uv[triangles[(hit.triangleIndex * 3) + 2]], vertices[triangles[hit.triangleIndex * 3]], vertices[triangles[(hit.triangleIndex * 3) + 1]], vertices[triangles[(hit.triangleIndex * 3) + 2]]);
				float x = points[numArray2[i]].x;
				float y = points[numArray2[i]].y;
				if (points[numArray2[i + 1]].x < x)
				{
					x = points[numArray2[i + 1]].x;
				}
				if (points[numArray2[i + 2]].x < x)
				{
					x = points[numArray2[i + 2]].x;
				}
				if (points[numArray2[i + 1]].y < y)
				{
					y = points[numArray2[i + 1]].y;
				}
				if (points[numArray2[i + 2]].y < y)
				{
					y = points[numArray2[i + 2]].y;
				}
				vectorArray5[0].x = points[numArray2[i]].x - Mathf.Floor(x);
				vectorArray5[0].y = points[numArray2[i]].y - Mathf.Floor(y);
				vectorArray5[1].x = points[numArray2[i + 1]].x - Mathf.Floor(x);
				vectorArray5[1].y = points[numArray2[i + 1]].y - Mathf.Floor(y);
				vectorArray5[2].x = points[numArray2[i + 2]].x - Mathf.Floor(x);
				vectorArray5[2].y = points[numArray2[i + 2]].y - Mathf.Floor(y);
				vectorArray6[0] = hit.normal;
				vectorArray6[1] = hit.normal;
				vectorArray6[2] = hit.normal;
				Vector3 lhs = hit.transform.TransformPoint(vectorArray4[1]) - hit.transform.TransformPoint(vectorArray4[0]);
				Vector3 rhs = hit.transform.TransformPoint(vectorArray4[2]) - hit.transform.TransformPoint(vectorArray4[0]);
				Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
				if (Vector3.Dot(hit.normal, normalized) > 0f)
				{
					list7.Add(vectorArray4[0]);
					list7.Add(vectorArray4[1]);
					list7.Add(vectorArray4[2]);
					list8.Add(vectorArray5[0]);
					list8.Add(vectorArray5[1]);
					list8.Add(vectorArray5[2]);
				}
				else
				{
					list7.Add(vectorArray4[0]);
					list7.Add(vectorArray4[2]);
					list7.Add(vectorArray4[1]);
					list8.Add(vectorArray5[0]);
					list8.Add(vectorArray5[2]);
					list8.Add(vectorArray5[1]);
				}
				list9.Add(vectorArray6[0]);
				list9.Add(vectorArray6[1]);
				list9.Add(vectorArray6[2]);
				Texture2D tex = new Texture2D(0x200, 0x200, TextureFormat.ARGB32, false) {
					hideFlags = HideFlags.HideAndDontSave,
					wrapMode = TextureWrapMode.Clamp
				};
				this.ClearCanvas(tex);
				item.mainTexture = tex;
				sharedMesh.vertices = list7.ToArray();
				sharedMesh.uv = list8.ToArray();
				sharedMesh.normals = list9.ToArray();
				sharedMesh.subMeshCount++;
				sharedMesh.SetTriangles(numArray3, sharedMesh.subMeshCount - 1);
				list10.Add(item);
				this.tempObject.renderer.sharedMaterials = list10.ToArray();
				EditorUtility.SetSelectedWireframeHidden(this.tempObject.renderer, true);
			}
		}
	}
	


	public bool PointInPoly(Vector3 point, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 normal)
	{
		float x = point.x;
		float y = point.y;
		float z = point.z;
		float num4 = vert1.x;
		float num5 = vert1.y;
		float num6 = vert1.z;
		float num7 = vert2.x;
		float num8 = vert2.y;
		float num9 = vert2.z;
		float num10 = vert3.x;
		float num11 = vert3.y;
		float num12 = vert3.z;
		float num13 = normal.x;
		float num14 = normal.y;
		float num15 = normal.z;
		float num16 = num7 - num4;
		float num17 = num8 - num5;
		float num18 = num9 - num6;
		float num19 = (num14 * num18) - (num15 * num17);
		float num20 = (num15 * num16) - (num13 * num18);
		float num21 = (num13 * num17) - (num14 * num16);
		double num22 = ((num19 * num4) + (num20 * num5)) + (num21 * num6);
		num22 = (((num19 * x) + (num20 * y)) + (num21 * z)) - num22;
		if (num22 < 9.9999999747524271E-07)
		{
			return false;
		}
		num16 = num10 - num7;
		num17 = num11 - num8;
		num18 = num12 - num9;
		num19 = (num14 * num18) - (num15 * num17);
		num20 = (num15 * num16) - (num13 * num18);
		num21 = (num13 * num17) - (num14 * num16);
		num22 = ((num19 * num7) + (num20 * num8)) + (num21 * num9);
		num22 = (((num19 * x) + (num20 * y)) + (num21 * z)) - num22;
		if (num22 < 9.9999999747524271E-07)
		{
			return false;
		}
		num16 = num4 - num10;
		num17 = num5 - num11;
		num18 = num6 - num12;
		num19 = (num14 * num18) - (num15 * num17);
		num20 = (num15 * num16) - (num13 * num18);
		num21 = (num13 * num17) - (num14 * num16);
		num22 = ((num19 * num10) + (num20 * num11)) + (num21 * num12);
		num22 = (((num19 * x) + (num20 * y)) + (num21 * z)) - num22;
		if (num22 < 1E-06)
		{
			return false;
		}
		return true;
	}*/
	

	


	

	
	

	
	

	
	

	
	

}
