using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//角色行为
[RequireComponent(typeof(CharacterController))]
public class PWCharacter : MonoBehaviour {
	private const float downSpeed=-5, maxSlideTime=0.5f,slideSpeed=2f;
	
	public Attribute attribute;//属性
	
	public float moveSpeed=3.6f, rotateSpeed=360;
	
	private CharacterController self;
	
	private Vector3 move, orgMove=Vector3.up*downSpeed;
	
	private CollisionFlags collisionFlage;
	private Vector3 lastDirection,displacement, startPosition;
	
	[HideInInspector]
	public bool isSlide, isJump;
	
	public Vector3 forward{
		get{
			return transform.forward;
		}
	}
	
	public void ApplyDamage(int dmg){//承受伤害
		int removedHp=Mathf.RoundToInt(dmg*(100f/(100+attribute.armor)));
		attribute.hp-=Mathf.Max(removedHp,1);
	}
	
	public Vector3 GetPoint(float afterSecond){//获取提前量
		Vector3 nextPoint=transform.position+move*afterSecond;
		nextPoint.y=40;
		
		RaycastHit hit;
		
		if(Physics.Raycast(nextPoint,Vector3.up*-1,out hit,100)){
			return hit.point;
		}
		
		return transform.position;
	}
	
	
	private float slideTime=0, jumpProcess=0, readyTime=0,lerpSpeed,hRate;
	
	// Use this for initialization
	void Start () {
		self=GetComponent<CharacterController>();
		move=orgMove;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!isJump)
			collisionFlage=self.Move(move*Time.deltaTime);
		else
			collisionFlage=self.Move(move);
	}
	
	public void StopMove(){//停止移动
		move=orgMove;
	}
	
	public bool MoveTo(Vector3 point){//移动到目标地点
		return MoveTo(point,1);
	}
	public bool MoveTo(Vector3 point, float spd){
		Vector3 dir=GetFlatDirection(point-transform.position);
		bool isFace=FaceTo(dir,1);
		
		Debug.DrawLine(transform.position,point,Color.red);
		
		if(dir.magnitude>=0.2f)
			move=orgMove+dir.normalized*moveSpeed*attribute.moveSpeed*spd;
		else
			move=orgMove;
		
		if((collisionFlage & CollisionFlags.Sides)>0)
			return true;
		
		return isFace && dir.magnitude<0.2f;
	}
	
	public bool FaceTo(Vector3 direction, float rate){//看向目标地点
		Vector3 dir=GetFlatDirection(direction);
		
		if(dir.magnitude<0.05)
			dir=lastDirection;
		
		Quaternion q=Quaternion.LookRotation(dir);
		transform.rotation=Quaternion.RotateTowards(transform.rotation,q,rotateSpeed*Time.deltaTime*rate);
		lastDirection=dir;
		
		return Vector3.Angle(transform.forward,dir)<0.5f;
	}
	
	public bool Slide(Vector3 direction){//滑向目标方向
		return Slide(direction,1,true);
	}
	public bool Slide(Vector3 direction,float rate,bool tryFace){
		Vector3 dir=GetFlatDirection(direction).normalized;
		
		if(!isSlide){
			isSlide=true;
			slideTime=0;
		}
		
		slideTime+=Time.deltaTime;
		
		if(tryFace){
			if(IsFaceTo(dir))
				FaceTo(dir,4);
			else
				FaceTo(dir*-1,4);
		}
		
		if(slideTime<maxSlideTime)
			move=orgMove+dir*moveSpeed*attribute.moveSpeed*slideSpeed*rate;
		else{
			move=orgMove;
			isSlide=false;
		}
		
		return isSlide;
	}
	
	public bool IsFaceTo(Vector3 direction){//检测朝向
		return Vector3.Dot(transform.forward,GetFlatDirection(direction))>-0.5f;
	}

	public void OnHitUp(){//被击飞
		move=Vector3.up*4f;
	}
	
	public bool OnJumpBegin(Vector3 point){//跳跃
		displacement=point-transform.position;
		if(displacement.magnitude<1 ||displacement.magnitude>50)
			return false;
		
		isJump=true;
		jumpProcess=0;
		
		hRate=Mathf.Abs(displacement.y)*4+2;
		lerpSpeed=16f/(displacement.magnitude+hRate/4);
		
		startPosition=transform.position;
		
		readyTime=0;
		
		return true;
	}
	
	public bool OnJump(Vector3 point){
		Vector3 dir=GetFlatDirection(displacement);
		bool isFace=true;
		
		if(readyTime<0.4f)
			isFace=FaceTo(dir,2);
		
		readyTime+=Time.deltaTime;
		
		if(!isFace || readyTime<0.4f){
			startPosition=transform.position;
			return true;
		}
		
		if((int)(collisionFlage & (CollisionFlags.Sides|CollisionFlags.Above))>0)
			isJump=false;
		
		jumpProcess=Mathf.Clamp01(jumpProcess+lerpSpeed*Time.deltaTime);
		
		Vector3 p=Vector3.Lerp(startPosition,point+Vector3.up*0.05f,jumpProcess);
		p.y+=jumpProcess*(1-jumpProcess)*hRate;
		
		if(Physics.Raycast(transform.position,transform.forward,0.5f))
			isJump=false;
		
		move=p-transform.position;
		
		if(jumpProcess==1){
			isJump=false;
			move=orgMove;
		}
		
		return isJump;
	}
	
	private Vector3 GetFlatDirection(Vector3 orgDirection){//获取平面方向
		return (new Vector3(orgDirection.x,0,orgDirection.z));
	}
}
