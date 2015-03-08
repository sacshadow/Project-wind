using UnityEngine;
using System.Collections;

//伤害
public class DamageState {
	public GameObject from;//来自单位
	public Vector3 dmgLocation;//来自位置
	public string dmgType;//伤害类型
	public int dmgValue;//伤害量
	
	public DamageState(GameObject from, string dmgType, int value){
		this.from=from;
		this.dmgLocation=from.transform.position;
		this.dmgType=dmgType;
		this.dmgValue=value;
	}
}
