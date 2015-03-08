using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {//¸úËæ

	public Transform target;
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position=target.position;
		transform.rotation=target.rotation;
	}
	
	//~ void OnDrawGizmos(){
		//~ transform.position=target.position;
		//~ transform.rotation=target.rotation;
	//~ }
}
