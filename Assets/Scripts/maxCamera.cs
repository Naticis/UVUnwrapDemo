using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[AddComponentMenu("Camera-Control/3dsMax Camera Style")]
public class maxCamera : MonoBehaviour
{
	public static Transform target;
	public Vector3 targetOffset;
	public float distance = 5.0f;
	public float maxDistance = 20;
	public float minDistance = .6f;
	public float xSpeed = 200.0f;
	public float ySpeed = 200.0f;
	public int yMinLimit = -90;
	public int yMaxLimit =90;
	public int zoomRate = 40;
	public float panSpeed = 0.3f;
	public float zoomDampening = 5.0f;
	private float pinchSpeed = 0.2f; 
	public static float xDeg = 0.0f;
	public static float yDeg = 0.0f;
	public static float currentDistance;
	public static float desiredDistance;
	public static float desiredOrthoSize;
	public static Quaternion currentRotation;
	public static Quaternion desiredRotation;
	public static Quaternion rotation;
	public static Vector3 position;
	public  static bool hitsomething;
	public  static bool hitsomething2;
	public  bool rotating;
	public static bool reset;
	public static bool rotateindicator;
	public float sensitivityX = 5.0f;
	public float sensitivityY = 5.0f;

    public static bool IsOverUI;
	
	public bool invertX = false;
	public bool invertY = false;
	private float lastDist = 0.0f;
	private float curDist = 0.0f;
	public LayerMask Dynamiclayer;
	public LayerMask AntiDynamiclayer;
    public LayerMask Sculptlayer;
    private Vector3 beginTouch;
    private Vector3 endTouch;

    void Start() { Init(); }
	void OnEnable() { Init(); }

	public void Init()
	{
		//If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
		if (!target)
		{
			GameObject go = new GameObject("Cam Target");
			go.transform.position = transform.position + (transform.forward * distance);
			//go.transform.localRotation=transform.localRotation;
			transform.rotation=Quaternion.identity;
			go.transform.rotation=Quaternion.identity;
			target = go.transform;
		}
	

		distance = Vector3.Distance(transform.position, target.position);
		currentDistance = distance;
        desiredDistance = distance*2f;
		desiredOrthoSize=10f;
		//be sure to grab the current rotations as starting points.
		position = transform.position;
		rotation = transform.rotation;
		currentRotation = transform.rotation;
		desiredRotation = transform.rotation;

		xDeg = Vector3.Angle(Vector3.right, transform.right );
		yDeg = Vector3.Angle(Vector3.up, transform.up );

		//addon
		
		desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
		
		currentRotation = transform.rotation;
		
		
		rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
		
		transform.rotation = rotation;
		AntiDynamiclayer=~Dynamiclayer;
	}




	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	void Update()
	{

        if (Input.GetMouseButtonUp(0))
        {
            hitsomething = false;
            rotating = false;
            rotateindicator = false;
           // PaintVerts.editing = false;
            //PaintVerts.ignoringRaycast = false;
            IsOverUI = false;
        }

        if (!IsOverUI)
                if (IsPointerOverUIObject())
                {
                    IsOverUI = true;
                
                }

        if (Input.GetMouseButtonDown(0))
        {
            endTouch = beginTouch;
        }
        else
        {
            beginTouch = Input.mousePosition;
        }

        if (hitsomething2)
			rotating=false;

	
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
	
		RaycastHit hit2;
		RaycastHit hit3;

		if(!hitsomething2)
			if(Input.GetMouseButtonDown(0))
		{
              
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {

				hitsomething=true;
				rotating=false;
				rotateindicator=false;
				
			}
			else
			{
				
				hitsomething=false;
				rotating=true;
				rotateindicator=true;
				
			}
	


		}
		
	
	}
	
	void LateUpdate()
	{

        if (IsOverUI)
            return;

        if (hitsomething)
            return;

	
		if(reset)
		{
			Init ();
			reset=false;
		}
       
        if (endTouch == beginTouch)
            return;

			if (Application.platform != RuntimePlatform.Android)
		{
			if (Input.GetMouseButton(0) && rotating)
		{ 

				xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
				yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

			////////OrbitAngle
			
			//Clamp the vertical axis for the orbit
			/*yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
			// set camera rotation 
			desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
			
			currentRotation = transform.rotation;
			
			
			rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
			
			transform.rotation = rotation;*/
		}
		else if (Input.GetMouseButton(1))
		{ 
			
			//grab the rotation of the camera so we can move in a psuedo local XY space
				float cameradist=Vector3.Distance(target.position,transform.position)*.1f;
			target.rotation = transform.rotation;
				target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed*cameradist);
				target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed*cameradist, Space.World);
		}


			// affect the desired Zoom distance if we roll the scrollwheel
			if(!Camera.main.orthographic)
				desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);

			if(Camera.main.orthographic)
				desiredOrthoSize -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate* Mathf.Abs(Camera.main.orthographicSize);
			//clamp the zoom min/max
		}
		 if (Application.platform == RuntimePlatform.Android)
		{
		// If Control and Alt and Middle button? ZOOM!
		if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
		{
				if(!Camera.main.orthographic)
			desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate*0.125f * Mathf.Abs(desiredDistance);

				if(Camera.main.orthographic)
					Camera.main.orthographicSize -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate*0.125f * Mathf.Abs(desiredDistance);
		}
		// If middle mouse and left alt are selected? ORBIT
		else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && rotating)
		{ 
				float rotatespeed=Vector3.Distance(target.position,transform.position);
			
				//rotaton speed
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
				if(rotatespeed<1f)
					rotatespeed=1f;
				//xDeg += rotatespeed*touchDeltaPosition.x * 5f* 0.02f; 
				//yDeg -= rotatespeed*touchDeltaPosition.y * 5f * 0.02f; 
				
				xDeg += touchDeltaPosition.x * 60f *0.7f*Time.deltaTime;
				
				yDeg -= touchDeltaPosition.y* 30f *0.8f*Time.deltaTime;

			//xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			//yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			////////OrbitAngle
			
			//Clamp the vertical axis for the orbit
		/*	yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

			desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
			
			currentRotation = transform.rotation;
			
			rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
			transform.rotation = rotation;*/
		

		}
		// otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
		else if (Input.touchCount > 1 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)//Input.GetMouseButton(0)
		{

			
			
			Touch touch1 = Input.GetTouch(0);
			
			
			Touch touch2 = Input.GetTouch(1);
			
			
			curDist = Vector2.Distance(touch1.position, touch2.position);
			
			
			
			
			
			float dotProduct = Vector2.Dot(touch1.deltaPosition.normalized, touch2.deltaPosition.normalized);
			
			
			
			
			
			if (dotProduct >= 0.8) {
				
				
				// Panning

				//grab the rotation of the camera so we can move in a psuedo local XY space
				target.rotation = transform.rotation;
				//target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
				//target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
					float cameradist=Vector3.Distance(target.position,transform.position)*.05f;
					//target.rotation = transform.rotation;
					//target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed*cameradist);
					//target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed*cameradist, Space.World);

					//float cameradist=Vector3.Distance(target.position,transform.position)*.2f;
				// Get movement of the finger since last frame
				Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
				
				// Move object across XY plane
					target.Translate (-touchDeltaPosition.x * panSpeed* 1f*Time.deltaTime*cameradist, -touchDeltaPosition.y * panSpeed* 5f*Time.deltaTime*cameradist, 0);
				
				
				
				
			} else if (dotProduct <= -0.8) {
				
					float pinchAmount = 0;
				// Zooming
					DetectTouchMovement.Calculate();
					
					if (Mathf.Abs(DetectTouchMovement.pinchDistanceDelta) > 0) { // zoom
						pinchAmount = DetectTouchMovement.pinchDistanceDelta;
					}
					if(!Camera.main.orthographic)
					desiredDistance-=pinchAmount* Time.deltaTime * zoomRate*.2f*0.0125f * Mathf.Abs(desiredDistance);
					/*Camera.main.orthographic=!Camera.main.orthographic;*/ //toggle Perspective or Orthographic
			if(Camera.main.orthographic)
						desiredOrthoSize -=pinchAmount* Time.deltaTime * zoomRate*.2f*0.0125f* Mathf.Abs(Camera.main.orthographicSize);
				
				if(curDist > lastDist) {
					
					
					// Zoom out

					
					//desiredDistance -= Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition)*pinchSpeed/10;
					
					
				} else {
					
					
					// Zoom in
					
				
					//desiredDistance += Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition)*pinchSpeed/10;
					
					
				}
				
				
				lastDist = curDist;
				
				
			}
			
		}
			
		}	
	
		
		////////Orbit Position
		yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
		
		desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
		
		currentRotation = transform.rotation;
		
		rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
		transform.rotation = rotation;

		desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
		// For smoothing of the zoom, lerp distance
		currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
		if(desiredOrthoSize<0)
			desiredOrthoSize=1;

		Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, desiredOrthoSize, Time.deltaTime * zoomDampening);
		// calculate position based on the new currentDistance 
	
		position =  target.position -(rotation * Vector3.forward * currentDistance + targetOffset);//
		transform.position = position;
	}
	
	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
}