using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DisplayHP : MonoBehaviour {//显示hp, 让hp看着摄像机

	private static Transform cam;
	
	public TextMesh text;
	public PWCharacter character;
	
	// Use this for initialization
	void Start () {
		cam=Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(cam==null)
			cam=Camera.main.transform;
		transform.rotation=Quaternion.LookRotation(cam.forward);
		
		if(character!=null && text!=null)
		text.text=character.attribute.hp.ToString();
	}
}
