using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViewAnimation : MonoBehaviour {

	public Animation anim;
	
	private string[] animClip;
	
	// Use this for initialization
	void Start () {
		if(anim==null)
			anim=GetComponentInChildren<Animation>();
		
		List<string> animList=new List<string>();
		
		foreach(AnimationState ac in anim){
			animList.Add(ac.name);
		}
		
		animClip=animList.ToArray();
	}
	
	// Update is called once per frame
	void Update () {
		
		
		
	}
	
	void OnGUI(){
		for(int i=0; i<animClip.Length; i++){
			if(GUILayout.Button(animClip[i]))
				anim.CrossFade(animClip[i]);
		}

		
	}
}
