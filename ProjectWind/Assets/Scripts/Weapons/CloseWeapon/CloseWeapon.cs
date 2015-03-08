using UnityEngine;
using System.Collections;

//近战武器
public abstract class CloseWeapon : Weapon {

	[HideInInspector]
	public bool isAttack, damageCheck;
	[HideInInspector]
	public Vector3 faceDirection, attackPoint;
	
	protected string lastState;
	
	protected float lastAttTime=0f;
	protected Animation anim;
	protected PWCharacter character;
	
	public abstract void SeekOverrideProcess(string type);//检查技能优先级
	
	public virtual void BeginAttack(){//进入攻击状态
		isAttack=false;
		damageCheck=false;
	}
	
	public abstract void EndMove();//结束攻击
	
	public virtual string GetLastState(){//获取最后一个技能
		if(!isAttack && Time.time-lastAttTime>0.5f)
			return "";
		
		return lastState;
	}
	
	public abstract Process GetProcess(string type);//获取技能进程
	
	public abstract bool OnNormalAttack();	//默认攻击
	
	
	void Awake(){
		character=GetComponent<PWCharacter>();
		anim=GetComponentInChildren<Animation>();
	}
	
	protected virtual DamageState GetDamageState(string type, float damageEffectiveness){//获取伤害
		float dmgValue=(character.attribute.strength+weaponDamage)*damageEffectiveness;
		return new DamageState(character.gameObject, type, Mathf.RoundToInt(dmgValue));
	}
	
	protected virtual void OnAttackEnd(){//结束一个技能
		isAttack=false;
		lastAttTime=Time.time;
	}
}
