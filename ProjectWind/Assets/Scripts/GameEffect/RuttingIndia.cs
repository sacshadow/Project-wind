using UnityEngine;
using System.Collections;

//拖尾效果
public class RuttingIndia : MonoBehaviour {
	public const float updateInterval=0.02f;
	private const float alphaDropPreSecond=3;
	
	private static Color init=new Color(1,1,1,1);
	private static Vector3[] vSample=new Vector3[]{Vector3.zero,Vector3.right, Vector3.forward, new Vector3(1,0,1)};
	private static Vector2[] uvSample=new Vector2[]{Vector2.zero, Vector2.right, Vector2.up, Vector2.one};
	private static int[] tSample=new int[]{0,2,1, 1,2,3};
	private static float alphaDecal{
		get{
			return alphaDropPreSecond*updateInterval;
		}
	}
	
	public Material ruttingIndiaMaterial;
	public int segment=15;
	public float minSegmentLength=0.5f;
	public float ruttingWidth=0.25f;
	
	//~ [HideInInspector]
	public bool isUpdate=false;
	
	private GameObject ruttingIndia;
	private Mesh mesh;
	
	private Vector3[] vertices;
	private Color[] colors;
	
	private Transform track;
	private int segmentUpdateIndex=0;
	
	
	
	private Vector3 lastPosition;
	private Quaternion lastRotation;
	
	void OnDrawGizmos(){
		Vector3 p=transform.position;
		Vector3 r=transform.right;
		
		Gizmos.DrawLine(p+r*ruttingWidth, p-r*ruttingWidth);
	}
	
	// Use this for initialization
	void Start () {
		track=transform;
		
		ruttingIndia=new GameObject("Rutting India");
		
		MeshFilter mFilter=ruttingIndia.AddComponent<MeshFilter>();
		ruttingIndia.AddComponent<MeshRenderer>().material=ruttingIndiaMaterial;
		
		mesh=new Mesh();
		
		SetMesh();
		
		mFilter.mesh=mesh;
		
		lastPosition=track.position;
		lastRotation=track.rotation;
		
		SetCountInit();
		InvokeRepeating("UpdateRutting",0,updateInterval);
	}
	
	//~ void LateUpdate(){
		//~ UpdateRutting();
	//~ }
	
	public void SetUpdate(bool update){
		if(!this.enabled)
			return;
		
		if(update != isUpdate ){
			CountNext();
			SetCountInit();
		}
		
		isUpdate=update;
	}
	
	private void SetMesh(){
		vertices=new Vector3[segment*4];
		Vector2[] uv=new Vector2[segment*4];
		colors=new Color[segment*4];
		int[] triangles=new int[segment*6];
		
		for(int i=0; i<segment; i++){
			for(int vCount=0; vCount<4; vCount++){
				int count=vCount+i*4;
				vertices[count]=vSample[vCount];
				uv[count]=uvSample[vCount];
				colors[count]=new Color(1,1,1,0);
			}
			
			for(int tCount=0; tCount<6; tCount++){
				triangles[tCount+i*6]=tSample[tCount]+i*4;
			}
		}
		
		mesh.vertices=vertices;
		mesh.uv=uv;
		mesh.colors=colors;
		mesh.triangles=triangles;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		//~ TangentSolver.Solver(mesh);
	}
	
	private void UpdateRutting () {
		if(isUpdate)
			UpdateSegment();
		
		UpdateColor();
	}
	
	private void UpdateSegment(){
		float dis=Vector3.Distance(lastPosition, track.position);
		
		if(dis<minSegmentLength)
			return;
		
		SetSegment(segmentUpdateIndex*4+2,track.position, track.rotation);
		
		if(dis>minSegmentLength){
			CountNext();
			SetCountInit();
		}
		
		mesh.vertices=vertices;
		mesh.RecalculateBounds();
		
		//~ TangentSolver.Solver(mesh);
	}
	
	private void SetCountInit(){
		SetSegment(segmentUpdateIndex*4,lastPosition,lastRotation);
		SetSegment(segmentUpdateIndex*4+2,lastPosition,lastRotation);
	}
	
	public void SetSegment(int index, Vector3 pos, Quaternion q){
		Vector3 offset=q*Vector3.right*ruttingWidth;
		vertices[index]=pos-offset;
		vertices[index+1]=pos+offset;
		colors[index+1]=colors[index]=init;
	}
	
	private void CountNext(){
		segmentUpdateIndex+=1;
		segmentUpdateIndex=segmentUpdateIndex%segment;
		
		lastPosition=track.position;
		lastRotation=track.rotation;
	}
	
	private void UpdateColor(){
		bool isChanged=false;
		
		for(int i=0; i<segment*4; i++){
			
			if(colors[i].a>0){
				colors[i].a-=alphaDecal;
				isChanged=true;
			}
		}
		
		if(isChanged)
			mesh.colors=colors;
	}
	
	void OnDestroy(){
		if(mesh != null)
			Destroy(mesh);
		
		if(ruttingIndia!=null)
			Destroy(ruttingIndia);
	}
}
