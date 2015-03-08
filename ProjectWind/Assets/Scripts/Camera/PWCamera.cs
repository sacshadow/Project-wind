using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class PWCamera : MonoBehaviour {
	public static Vector3 directionZ, directionX;
	
	//将屏幕坐标转换成对应的平面世界坐标
	public static Vector3 ConvertDirection(Vector2 screenDirection){
		return (directionZ*screenDirection.y+directionX*screenDirection.x).normalized;
	}
	
	public Transform target;//跟踪目标
	public float distance=20;//跟踪距离
	public float lookAngleX=45, lookAngleY=45;//偏移角度
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(target==null)
			return;
		
		Quaternion q=Quaternion.Euler(lookAngleX,lookAngleY,0);
		
		transform.position=q*Vector3.forward*-distance+target.position;
		transform.rotation=q;
		
		
		directionZ=transform.forward;
		directionX=transform.right;
		directionZ.y=0;
		directionZ.Normalize();
	}
}
