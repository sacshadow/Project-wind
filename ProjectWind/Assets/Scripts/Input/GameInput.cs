using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameInput : MonoBehaviour {
	public const float clickCountInterval=0.25f;
	public const float maxRayDistance=100f;
	public const float fingerMoveSensitive=25f;
	public const float directionActiveSensitive=5f;
	public const float directionSensitive=0.1f;
	
	public static bool
		click,//点击
		hold,//按住
		doubleClick,//双击
		slide,//滑动
		shieldHold,//防御键按住
		shieldClick,//点击防御键
		shieldDoubleClick,//双击击防御键
		shieldDrag;//拖拽防御区
	
	public static GameObject currentPointObject;
	public static RaycastHit lastHit;
	public static ClickedObject pointObject;
	public static ClickedObject clickedObject;//点击的物体
	public static Vector2 fingerMove, fingerPoint;
	public static InputDirection inputDirection;
	public static List<InputDirection> directionList;
	
	public LayerMask mask=~0;
	
	private PWInputBase input;
	
	private float lastClicktime;
	private int clickCount=0;
	
	private Vector2 lastFingerPoint;
	private float changedTime=0;
	
	// Use this for initialization
	void Start () {
		input=new PCInput();
		directionList=new List<InputDirection>();
	}
	
	// Update is called once per frame
	void Update () {
		CheckClick();
		CheckPointObject();
		CheckFingerMove();
		CheckSlideOrDrag();
		CheckInputDirection();
	}
	
	//debug
	//~ void OnGUI(){
		//~ GUILayout.Toggle(click,"Click "+clickCount.ToString());
		//~ GUILayout.Toggle(hold,"Hold");
		//~ GUILayout.Toggle(doubleClick,"DoubleClick");
		//~ GUILayout.Toggle(slide,"Slide");
		
		//~ GUILayout.Label("clickedObject"+clickedObject.ToString());
		//~ if(currentPointObject==null)
			//~ GUILayout.Label("currentPointObject Null");
		//~ else
			//~ GUILayout.Label("currentPointObject "+currentPointObject.ToString());
		
		//~ GUILayout.Label("Change "+(changedTime>directionSensitive).ToString());
		//~ GUILayout.Label("inputDirection "+inputDirection.ToString());
		//~ GUILayout.Box("Array: "+directionString);
		//~ GUILayout.Label("fingerMove "+fingerMove.ToString());
		//~ GUILayout.Label("pointObject "+pointObject.ToString());
		
		//~ GUILayout.Space(100);
		
		//~ if(click || hold)
			//~ GUILayout.Label(input.GetMousePosition().ToString());
	//~ }
	
	private void CheckPointObject(){
		pointObject=ClickedObject.EMPTY;
		
		if(!click && !hold)
			return;
		
		
		currentPointObject = GetNearObject();
		
		pointObject=GetClickObject();
		
		if(click)
			clickedObject=pointObject;
	}
	
	private GameObject GetNearObject() {
		Ray ray=Camera.main.ScreenPointToRay(input.GetMousePosition());
		
		RaycastHit[] allHit = Physics.SphereCastAll(ray,1.5f, maxRayDistance, mask);
		
		if(allHit.Length == 0)
			return null;
		
		var enemy = allHit.Where(x=>x.transform.tag == "Enemy").ToList();
		
		if(enemy.Count == 0)
			lastHit = Nearset(new List<RaycastHit>(allHit));
		
		else
			lastHit = Nearset(enemy);
		
		return lastHit.transform.gameObject;
		
	}
	
	private RaycastHit Nearset(List<RaycastHit> hit) {
		if(hit.Count == 1)
			return hit[0];
		
		Vector3 camPoint = Camera.main.transform.position;
			
		hit.Sort((lhs,rhs)=>
			Vector3.Distance(lhs.point, camPoint) < Vector3.Distance(rhs.point, camPoint) ? -1: 1
		);
		
		return hit[0];
	}
	
	private void CheckClick(){
		if(input.GetClick())
			SetClick();
		else
			click=doubleClick=false;
		
		if(Time.time-lastClicktime>clickCountInterval)
			clickCount=0;
		
		hold=input.GetHold();
	}
	
	private void SetClick(){
		click=true;
		clickCount++;
		
		doubleClick=(clickCount==2);
		lastClicktime=Time.time;
	}
	
	private void CheckSlideOrDrag(){
		if(hold && fingerMove.magnitude>fingerMoveSensitive)
			slide=true;
		else
			slide=false;
	}
	
	private static ClickedObject GetClickObject(){
		if(currentPointObject == null )
			return ClickedObject.NULL;
		else if(currentPointObject.tag=="Player")
			return ClickedObject.CHARACTER;
		else if(currentPointObject.tag== "Enemy")
			return ClickedObject.ENEMY;
		else if(currentPointObject.tag=="Event")
			return ClickedObject.EVENT;
		else
			return ClickedObject.EMPTY;
	}
	
	private void CheckFingerMove(){
		if(!hold)
			return;
		
		changedTime+=Time.deltaTime;
		
		if(click)
			UpdateFingerPoint(input.GetMousePosition());
		
		fingerPoint=input.GetMousePosition();
		Vector2 currentFingerPoint=fingerPoint;
		
		fingerMove=currentFingerPoint-lastFingerPoint;
		
		if(fingerMove.magnitude>fingerMoveSensitive)
			UpdateFingerPoint(currentFingerPoint);
	}
	
	private void UpdateFingerPoint(Vector2 newPoint){
		lastFingerPoint=newPoint;
		changedTime=0;
	}
	
	private void CheckInputDirection(){
		inputDirection=InputDirection.NONE;
		
		if(click){
			directionString="";
			directionList=new List<InputDirection>();
		}
		
		if(!hold)
			return;
		
		if(changedTime>directionSensitive)
			return;
		
		if(fingerMove.magnitude<directionActiveSensitive)
			return;
		
		inputDirection=GetInputDirection(fingerMove);
		
		UpdateDirectionList();
	}
	
	private InputDirection GetInputDirection(Vector2 move){
		float angle=Vector2.Angle(Vector2.up, move);
		int segment=Mathf.RoundToInt(angle/45f);
		
		segment=move.x>0?segment:8-segment;
		segment=segment%8;
		
		return (InputDirection)(1+segment);
	}
	
	private static string directionString="";//debug
	private void UpdateDirectionList(){
		if(inputDirection==InputDirection.NONE)
			return;
		
		if(directionList.Count==0){
			directionList.Add(inputDirection);
			return;
		}
		
		if(inputDirection!=directionList[directionList.Count-1])
			directionList.Add(inputDirection);
		
		if(directionList.Count>8)
			directionList.RemoveAt(0);
		
		directionString="";
		for(int i=0; i<directionList.Count; i++){
			directionString+=directionList[i].ToString()+"-";
		}
	}
}
