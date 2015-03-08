using UnityEngine;
using System;
using System.Collections;

public delegate bool Process();
//角色行为
[RequireComponent(typeof(PWCharacter))]
public class CharacterBehaviour : MonoBehaviour {
	
	public CloseWeapon closeWeapon;//近身武器
	public RangeWeapon rangeWeapon;//远程武器
	//~ public ParticleEmitter rangeWeapon;
	
	private CharacterDecision decision;//角色行为
	private Animation anim;//动画
	private PWCharacter character;//角色
	
	private NavMeshPath navPath;//自动寻路路径
	private int node, layer=~0;//路径, 检测层
	
	private Process process;//进程
	private bool inProgress=false,isWalk=false;
	private float endTime=0, endCount=0, endProcess=0, waitTime=0, dmgTime=0, dmgProcess=0.3f;
	private string currentDmgState="";
	
	private Vector3 vector3, direct;//临时记录
	
	// Use this for initialization
	void Start () {
		navPath=new NavMeshPath();
		character=GetComponent<PWCharacter>();
		decision=GetComponent<CharacterDecision>();
		anim=GetComponentInChildren<Animation>();
		
		StartCoroutine(NormalState());
	}
	
	private IEnumerator NormalState(){
		string order="";
		DamageState damage;
		
		while(true){
			closeWeapon.SeekOverrideProcess(decision.LookAttackOrder());//武器技能优先级判断
			
			if(!inProgress || isWalk)//获取指令
				order=decision.GetDecision();
			
			if(order.Length>0)//处理指令
				ProcessNormalOrder(order);
			
			if(inProgress && !process())//执行指令. 如果指令完成,
				EndProcess();//停止进程
			
			
			damage=decision.GetDamage();//获取伤害
			if(damage!=null){//处理伤害
				node=navPath.corners.Length;
				
				if(!inProgress || isWalk)
					yield return StartCoroutine(OnDamageState(damage));
				else{
					anim.CrossFade("hit_normal");
					character.ApplyDamage(damage.dmgValue);
					endProcess=Time.time+0.25f;
				}
			}
			yield return 1;
		}
	}
	
	private IEnumerator OnDamageState(DamageState damage){
		dmgTime=0;
		
		ProcessDamage(damage);
		while(dmgTime<dmgProcess){//伤害时停止其他动作
			dmgTime+=Time.deltaTime;
			
			ProcessDamage(decision.GetDamage());//继续获取其他伤害
			
			character.FaceTo(direct,4);
			
			if(currentDmgState=="back")//不同伤害类型执行不同效果
				character.Slide(-direct,1,false);
			if(currentDmgState=="up")
				character.OnHitUp();
			if(currentDmgState=="down"){
				if(dmgTime<0.8f)
					character.Slide(-direct,0.5f,false);
				else
					character.StopMove();
			}
			
			yield return 1;
		}//结束后返回原始状态
		currentDmgState="";
		anim.CrossFade("idle");
		character.StopMove();
	}
	
	private void ProcessDamage(DamageState damage){//处理伤害
		if(damage==null)
			return;
		
		EndProcess();//结束其他进程
		//TODO: 根据当前状态不同, 伤害量, 硬直时间等也应该不同
		character.ApplyDamage(damage.dmgValue);//承受伤害
		
		direct=(damage.dmgLocation-transform.position).normalized;
		if(direct.magnitude==0)
			Debug.Log("? check mask and hit");
		
		//~ Debug.Log("d "+damage.dmgType);
		
		currentDmgState=damage.dmgType;//记录伤害类型
		string sd="normal";
		
		dmgProcess=0.3f;//硬直时间
		
		if(currentDmgState=="up")
			sd="up";
		if(currentDmgState=="down"){
			dmgProcess=3f;
			sd="down";
		}
		
		anim["hit_"+sd].time=0f;
		anim.CrossFade("hit_"+sd);
		
		dmgTime=0f;
	}
	
	private void ProcessNormalOrder(string order){//执行一般命令
		if(Time.time<endProcess)
			return;
		
		switch(order){
			case "stop": node=navPath.corners.Length;break;
			case "look": OnLookBegin();break;
			case "move": OnMoveBegin(); break;
			case "jump": OnJumpBegin();break;
			case "slide": OnSliderBegin(); break;
			case "attack": OnAttackBegin(); break;
		}
	}
	
	private void OnLookBegin(){//看向目标方向
		if(!IsLegalToMove() || isWalk)
			return;
		
		isWalk=true;
		vector3=decision.v3;
		SetNewProcess(OnLook);
	}
	
	private bool OnLook(){
		return character.FaceTo(vector3,2);
	}
	
	private void OnMoveBegin(){//移动
		if(!IsLegalToMove())
			return;
		
		if(!NavMesh.CalculatePath(transform.position,decision.v3,layer,navPath)){
			NavMeshHit hit;
			if(NavMesh.SamplePosition(decision.v3,out hit, 20, ~0)){
				NavMesh.CalculatePath(transform.position,hit.position,layer,navPath);
			}
			else
				return;
		}
		
		isWalk=true;
		node=1;
		
		if(Vector3.Distance(navPath.corners[node],transform.position)<0.5f)
			node=2;
		
		SetNewProcess(OnMove);
		anim.CrossFade("walk");
	}
	
	private bool OnMove(){
		if(node>=navPath.corners.Length)
			return false;
		
		if(character.MoveTo(navPath.corners[node]))
			node+=1;
		
		return true;
	}
	
	private void OnJumpBegin(){//跳跃
		if(!IsLegalToMove())
			return;
		
		vector3=decision.v3;
		if(!character.OnJumpBegin(vector3))
			return;
		
		endTime=anim["jump_ed"].length;
		endCount=0;
		
		SetNewProcess(OnJump);
		anim.CrossFade("jump_bg");
		waitTime=0.3f;
	}
	
	private bool OnJump(){
		bool b=character.OnJump(vector3);
		
		if(decision.LookDecision()=="attack")
			SetOnJumpAttack();
		
		if(b)
			return true; 
		else if(endCount==0)
			anim.CrossFade("jump_ed");
		
		endCount+=Time.deltaTime;
		
		if(endCount>endTime)
			return false;
		
		return true;
	}
	
	private void SetOnJumpAttack(){
		Transform t=null;
		if(decision.go!=null)
			t=decision.go.transform;
		
		rangeWeapon.OnJumpAttack(decision.v3,t);
		decision.GetDecision();
	}
	
	private void OnSliderBegin(){//闪避
		if(!IsLegalToMove())
			return;
		
		vector3=decision.v3;
		
		string act=(character.IsFaceTo(vector3)?"slide_f":"slide_b");
		anim.CrossFade(act);
		
		SetNewProcess(OnSlider);
		waitTime=0.1f;
	}
	
	private bool OnSlider(){
		return character.Slide(vector3);
	}
	
	private void OnAttackBegin(){//攻击
		if(!IsLegalToMove())
			return;
		
		string type=decision.GetAttackOrder();//获取攻击指令
		Vector3 dir=decision.v3-transform.position;
		
		//写入基本指令
		closeWeapon.BeginAttack();
		
		closeWeapon.faceDirection=decision.v3;
		closeWeapon.attackPoint=dir.normalized*(dir.magnitude-1.5f)+transform.position;
		rangeWeapon.faceDirection=decision.v3;
		rangeWeapon.attackPoint=decision.v3;
		
		if(type=="jump" && dir.magnitude<4)//跳跃攻击距离限制
			return;
		
		if(type=="range"){//远程
			SetNewProcess(rangeWeapon.GetProcess(type));
		}
		else//近身
			SetNewProcess(closeWeapon.GetProcess(type));
	}
	
	private void SetNewProcess(Process p){//开启进程
		process=p;
		inProgress=true;
	}
	
	private void EndProcess(){//结束进程
		endProcess=Time.time+waitTime;
		inProgress=isWalk=false;
		anim.CrossFade("idle");
		character.StopMove();
		waitTime=0;
	}
	
	private bool IsLegalToMove(){//是否可以执行移动动作
		bool rt=!inProgress || isWalk ;
		if(rt)
			EndProcess();
		return rt;
	}
	
	
}
