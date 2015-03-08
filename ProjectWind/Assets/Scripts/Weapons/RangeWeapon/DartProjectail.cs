using UnityEngine;
using System.Collections;

public class DartProjectail : MonoBehaviour {
	
	
	public float liveSecond=5f;
	public float speed=40;
	private DamageState dmg;
	
	public void SetDamageState(DamageState dmgState){
		this.dmg=dmgState;
	}
	
	// Use this for initialization
	void Start () {
		Invoke("Kill",liveSecond);
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, speed*Time.deltaTime)){
			transform.position=hit.point;
			hit.transform.gameObject.SendMessage("OnDamage",dmg,SendMessageOptions.DontRequireReceiver);
			this.enabled=false;
		}
		else
			transform.Translate(Vector3.forward*speed*Time.deltaTime);
		
	}
	
	void Kill(){
		Destroy(gameObject);
	}
}
