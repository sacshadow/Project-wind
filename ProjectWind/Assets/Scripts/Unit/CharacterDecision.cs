using UnityEngine;
using System.Collections;

//接收指令
public class CharacterDecision : MonoBehaviour {

	[HideInInspector]
	public Vector3 v3;//数据存放
	[HideInInspector]
	public GameObject go;
	[HideInInspector]
	public string attackOrder="";
	[HideInInspector]
	public DamageState dmgReceive;
	
	private string decision="";
	
	public void StopMove(){//停止动作
		decision="stop";
	}
	
	public void LookTo(Vector3 point){//看向目标
		this.v3=point-transform.position;
		decision="look";
	}
	
	public void MoveTo(Vector3 point){//移动到目标
		this.v3=point;
		decision="move";
	}
	
	public void JumpTo(Vector3 point){//跳跃到目标
		NavMeshHit hit;
		if(NavMesh.SamplePosition(point,out hit,10,~0)){
			this.v3=hit.position;
		}
		else
			this.v3=point;
		decision="jump";
	}
	
	public void SlideTo(Vector3 direction){//向目标方向闪避
		this.v3=direction;
		decision="slide";
	}
	
	public void RangeAttack(Vector3 point){//远程攻击目标点
		this.v3=point;
		decision="attack";
		attackOrder="range";
	}
	
	public void JumpAttack(Vector3 point){//跳跃攻击
		this.v3=point;
		decision="attack";
		attackOrder="jump";
	}
	
	public void OnAttack(Vector3 point, string order){//攻击
		// Debug.Log("o "+order);
		
		if(order=="")
			return;
		
		this.v3=point;
		decision="attack";
		attackOrder=order;
		
	}
	
	public void OnDamage(DamageState dmgState){//接收伤害
		dmgReceive=dmgState;
	}
	
	public string  LookAttackOrder(){//查看攻击指令
		if(decision!="attack")
			return "";
		
		return attackOrder;
	}
	
	public string LookDecision(){//查看命令
		return decision;
	}
	
	public string GetDecision(){//获取命令
		string rt=decision;
		decision="";
		return rt;
	}
	
	public string GetAttackOrder(){//获取攻击命令
		string rt=attackOrder;
		attackOrder="";
		return rt;
	}
	
	public DamageState GetDamage(){//获取伤害
		DamageState rt=dmgReceive;
		dmgReceive=null;
		return rt;
	}
	

	
}
