using UnityEngine;
using System.Collections;
//镖
public class Dart : RangeWeapon{
	private static string[] att=new string[]{"att_1","att_2","att_3"};//动作
	
	public DartProjectail projectail;
	
	protected int count;
	protected float animTimeCount=0;
	protected AnimationState currentAnim;
	
	public override void EndMove(){
		
	}
	
	public override Process GetProcess(string type){//获取技能进程
		
		return OnNormalAttack;
	}
	
	
	public override void OnJumpAttack(Vector3 point, Transform target){//跳跃特殊攻击
		Vector3 p=point+Vector3.up;
		
		Debug.DrawLine(transform.position, point);
		
		character.FaceTo(point-transform.position,4);
		if(target!=null)
			p=target.position+Vector3.up;
		
		Shot(p,0.25f);
	}
	
	public override bool OnNormalAttack(){//默认攻击
		if(!isAttack)
			NormalAttackBegin();
		
		character.FaceTo(faceDirection-transform.position,4);
		
		animTimeCount+=Time.deltaTime;
		
		if(animTimeCount>currentAnim.length/2f && !damageCheck)
			Shot(attackPoint+Vector3.up,1);
		
		if(animTimeCount>currentAnim.length){
			damageCheck=false;
			count=(count+1)%2;
			isAttack=false;
			animTimeCount=0;
			lastAttTime=Time.time;
		}
		
		return isAttack;
	}
	
	private void Shot(Vector3 point, float damageEffectiveness){
		damageCheck=true;
		Quaternion q=Quaternion.LookRotation(point-transform.position);
		DartProjectail temp=Instantiate(projectail,transform.position+transform.forward,q) as DartProjectail;
		temp.SetDamageState(GetDamageState("normal",damageEffectiveness));
	}
	
	private void NormalAttackBegin(){
		if(Time.time-lastAttTime>0.5f)
			count=0;
		
		currentAnim=anim[att[count]];
		anim.CrossFade(att[count]);
		
		lastState="normal";
		
		isAttack=true;
	}
}
