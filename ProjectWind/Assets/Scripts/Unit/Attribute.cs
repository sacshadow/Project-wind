using UnityEngine;
using System.Collections;

//单位属性
[System.Serializable]
public class Attribute {
	public int hp, strength, armor;
	public float attackSpeed, moveSpeed;
	
	public Attribute(){
		hp=100;
		strength=20;
		armor=10;
		attackSpeed=1f;
		moveSpeed=1f;
	}
	
}
