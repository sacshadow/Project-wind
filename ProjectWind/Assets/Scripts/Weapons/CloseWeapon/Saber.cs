using UnityEngine;
using System.Collections;

//剑
public class Saber : CloseWeapon {
	private static string[] att=new string[]{"att_1","att_2","att_3"};//连续技动作名称
	
	public RuttingIndia saberLight;//拖尾效果
	
	
	protected int count;
	protected float animTimeCount=0;
	protected AnimationState currentAnim;
	
	public int GetCount(){
		return count;
	}
	
	public override void EndMove(){//结束动作
		count=0;
	}
	
	private void EndCurrentAttack(){//提前结束当前攻击
		if(currentAnim!=null)
			animTimeCount=currentAnim.length+1;
	}
	
	public override void SeekOverrideProcess(string type){//技能优先级判断
		switch(lastState){
			case "jump":
			case "dodge_att":return;
		}
		
		switch(type){
			case "jump":
			case "dodge_att":EndCurrentAttack();break;
		}
			
	}
	
	public override Process GetProcess(string type){//获取技能进程
		lastState=type;
		
		Debug.Log("type "+type+" accept");
		
		if(type=="up")
			return OnAttackUp;
		if(type=="jump")
			return OnJumpAttack;
		if(type=="down")
			return OnJumpAttackDown;
		if(type=="dodge")
			return OnDodgeBack;
		if(type=="dodge_att")
			return OnDogeAttack;
		
		return OnNormalAttack;
	}
	
	public override bool OnNormalAttack(){//普通攻击
		if(!isAttack)
			NormalAttackBegin();
		
		character.FaceTo(faceDirection-transform.position,4);
		
		if(count==2)
			character.MoveTo(transform.position+transform.forward,0.5f);
		
		animTimeCount+=Time.deltaTime;
		
		if(animTimeCount>currentAnim.length/2f && !damageCheck)
			CheckDamage(1);
		
		if(animTimeCount>currentAnim.length){
			count=(count+1)%3;
			isAttack=false;
			saberLight.SetUpdate(false);
			animTimeCount=0;
			lastAttTime=Time.time;
		}
		
		return isAttack;
	}
	
	public bool OnJumpAttack(){//跳跃攻击
		if(!isAttack)
			JumpAttackBegin();
		
		if(!isAttack)
			return false;
		
		if(character.OnJump(attackPoint))
			return true;
		
		if(animTimeCount==0){
			anim.CrossFade("att_jump_ed");
			saberLight.SetUpdate(true);
			character.StopMove();
		}
		
		animTimeCount+=Time.deltaTime;
		
		if(animTimeCount>currentAnim.length/2f && !damageCheck)
			CheckDamage(1.5f,2);
		
		if(animTimeCount>currentAnim.length){
			saberLight.SetUpdate(false);
			isAttack=false;
			animTimeCount=0;
		}
		
		return isAttack;
	}
	
	public bool OnDodgeBack(){//向后闪避
		if(!isAttack)
			SetAttackBegin("dodge");
		
		animTimeCount+=Time.deltaTime;
		
		if(animTimeCount<currentAnim.length/1.5f)
			character.Slide(-character.forward,0.7f,true);
		else
			character.StopMove();
			
		if(animTimeCount>currentAnim.length)
			OnAttackEnd();
		
		return isAttack;
	}
	
	public bool OnDogeAttack(){//闪避后攻击攻击
		if(!isAttack)
			SetAttackBegin("dodge_att");

		animTimeCount+=Time.deltaTime;
		
		if(animTimeCount<currentAnim.length/2f)
			character.Slide(character.forward,0.9f,true);
		else if(!damageCheck){
			character.StopMove();
			CheckDamage(1.25f);
		}
		
		if(animTimeCount>currentAnim.length)
			OnAttackEnd();
		
		return isAttack;
	}
	
	private bool OnJumpAttackDown(){//跳跃向下攻击
		if(!isAttack)
			SetAttackBegin("att_jump_ed");
		
		animTimeCount+=Time.deltaTime;
		
		character.MoveTo(transform.position+transform.forward,1f);
		
		if(animTimeCount>currentAnim.length/2f && !damageCheck)
			CheckDamage(1.2f);
		
		if(animTimeCount>currentAnim.length)
			OnAttackEnd();
		
		return isAttack;
	}
	
	private bool OnAttackUp(){//击飞攻击
		if(!isAttack)
			SetAttackBegin("att_up");
		
		animTimeCount+=Time.deltaTime;
		
		character.FaceTo(faceDirection-transform.position,4);
		
		if(animTimeCount>0.15f)
			character.MoveTo(transform.position+transform.forward,1f);
		
		if(animTimeCount>currentAnim.length/2f && !damageCheck)
			CheckDamage(0.5f);
		
		if(animTimeCount>currentAnim.length)
			OnAttackEnd();
		
		return isAttack;
	}
	
	private void SetAttackBegin(string type){
		animTimeCount=0;
		damageCheck=false;
		isAttack=true;
		saberLight.SetUpdate(true);
		currentAnim=anim[type];
		anim[type].time=0;
		anim.CrossFade(type);
	}
	
	protected override void OnAttackEnd(){
		base.OnAttackEnd();
		count=0;
		saberLight.SetUpdate(false);
		animTimeCount=0;
	}
	
	
	private void NormalAttackBegin(){
		if(Time.time-lastAttTime>0.5f)
			count=0;
		
		currentAnim=anim[att[count]];
		anim.CrossFade(att[count]);
		
		saberLight.SetUpdate(true);
		lastState="normal";
		
		isAttack=true;
	}
	
	private void JumpAttackBegin() {
		if(!character.OnJumpBegin(attackPoint)) {
			isAttack=false;
			return;
		}
		
		lastState="jump";
		
		animTimeCount=0;
		currentAnim=anim["att_jump_ed"];
		anim.CrossFade("att_jump_bg");
		isAttack=true;
	}
	
	private void CheckDamage(float effectivness) {//造成伤害
		CheckDamage(effectivness,1);
	}
	private void CheckDamage(float effectivness, float rad) {
		Collider[] hit=Physics.OverlapSphere(transform.position+transform.forward*1f,rad,1<<8);
			
		string msg="normal";
		
		if(lastState=="up")
			msg="up";
		else if(count==2 || lastState=="dodge_att")
			msg="back";
		else if(lastState=="down" || lastState=="jump")
			msg="down";
		
		for(int i=0; i<hit.Length; i++){
			if(hit[i].transform!=this.transform){
				//Debug.Log("Hit "+hit[i].gameObject);
				
				hit[i].SendMessage("OnDamage",GetDamageState(msg,effectivness));
			}
		}
		
		damageCheck=true;
	}
}
