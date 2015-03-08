using UnityEngine;
using System.Collections;

//远程武器
public abstract class RangeWeapon : Weapon {

	[HideInInspector]
	public bool isAttack, damageCheck;
	[HideInInspector]
	public Vector3 faceDirection, attackPoint;
	
	
	public PWCharacter character;
	protected string lastState="";
	
	protected float lastAttTime=0f;
	protected Animation anim;
	
	
	protected virtual DamageState GetDamageState(string type, float damageEffectiveness){//伤害计算
		float dmgValue=weaponDamage*damageEffectiveness;
		return new DamageState(character.gameObject, type, Mathf.RoundToInt(dmgValue));
	}
	
	void Awake(){
		anim=character.GetComponentInChildren<Animation>();
	}
	
	public virtual void BeginAttack(){//开始攻击
		isAttack=false;
		damageCheck=false;
	}
	
	public abstract void EndMove();//结束攻击
	
	
	public abstract void OnJumpAttack(Vector3 point, Transform target);//跳跃技能
	
	public virtual string GetLastState(){//获取之前技能
		if(!isAttack && Time.time-lastAttTime>0.5f)
			return "";
		
		return lastState;
	}
	
	public abstract Process GetProcess(string type);//获取技能进程
	
	public abstract bool OnNormalAttack();//默认攻击
}
