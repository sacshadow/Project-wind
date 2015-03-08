using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAIControl : MonoBehaviour {

	public static List<EnemyAI> ai=new List<EnemyAI>();
	
	public static bool follow=true, attack=false;
	
	public GameObject aiPrefab;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		
		follow=GUILayout.Toggle(follow,"follow");
		attack=GUILayout.Toggle(attack,"attack");
		
		if(GUILayout.Button("+"))
			CreateAI();
		
		if(GUILayout.Button("-"))
			RemoveAI();
	}
	
	void CreateAI(){
		RaycastHit hit;
		Vector3 rdPoint=Random.insideUnitSphere*20;
		rdPoint.y=20;

		
		if(Physics.Raycast(rdPoint,Vector3.up*-1,out hit, 40)){
			GameObject.Instantiate(aiPrefab,hit.point,Quaternion.identity);
		}
		
	}
	
	void RemoveAI(){
		if(ai.Count<=0)
			return;
		
		EnemyAI temp=ai[ai.Count-1];
		
		Destroy(temp.gameObject);
	}
	
}
