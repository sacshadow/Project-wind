using UnityEngine;
using System.Collections;

//单个敌人ai
public class EnemyAI : MonoBehaviour {
	
	public LayerMask mask=~0;//检测层
	
	private CharacterDecision decision;//ai身上的命令接受
	
	private Transform target;//目标
	
	private float attTime=2;//攻击时间
	
	// Use this for initialization
	void Start () {
		if(!EnemyAIControl.ai.Contains(this))//ai注册
			EnemyAIControl.ai.Add(this);
		decision=GetComponent<CharacterDecision>();
		
		target=GameObject.FindWithTag("Player").transform;
		
		StartCoroutine(DecisionUpdate());
	}
	
	// Update is called once per frame
	IEnumerator DecisionUpdate () {
		
		Vector3 lastDis=Vector3.zero;
		
		while(true){//正常状态
			
			float dis=Vector3.Distance(target.position, transform.position);
			
			if(EnemyAIControl.follow){//跟着目标
			
				if(lastDis!=target.position){
				
					if(dis>10){
						Movement(dis,target.position);
					}
					else if(dis<8){
						//~ decision.StopMove();
						
						decision.LookTo(target.position);
						
						lastDis=target.position;
					}
				}
				
				if(EnemyAIControl.attack){//攻击目标
					
					attTime-=Time.deltaTime;
					
					if(attTime<=0){
						StartCoroutine(TryAttack());
						attTime=Random.Range(5,20);
					}
				}
				
			}
			else
				lastDis=Vector3.zero;
			
			yield return 1;
		}
		
	}
	
	//移动检测
	private void Movement(float dis, Vector3 targetPosition){
		RaycastHit hit;
		Vector3 offset=targetPosition-transform.position;
		offset.y=0;
		Vector3 v1=transform.position+offset.normalized*2.5f+Vector3.up*30;
		
		if(Physics.Raycast(v1,Vector3.up*-1,out hit, 60,mask)){//跳跃地形检测
			if(ShouldJumpWall(dis,offset)){}//越过墙壁检测
			else if((hit.point.y-transform.position.y)<-1){
				decision.JumpTo(hit.point+transform.forward*3);
			}
			else{
				decision.MoveTo(target.position);
			}
		}
		else{
			decision.MoveTo(target.position);
		}
	}
	
	
	//越过墙壁检测
	private bool ShouldJumpWall(float dis, Vector3 offset){
		RaycastHit hit;
		bool rt=false;
		
		if(dis>20)
			return false;
		
		
		if(Physics.Raycast(transform.position+Vector3.up,offset.normalized, out hit, 5,mask)){
			if(Vector3.Angle(Vector3.up,hit.normal)<85)//垂直5度以内的是墙壁
				return false;
			
			if(hit.distance<1)//如果距离墙壁太近就不要跳
				return false;
			
			float value=hit.collider.ClosestPointOnBounds(Vector3.up*100).y;
			Vector3 pt=hit.point+transform.forward;
			pt.y=value;
			decision.JumpTo(pt);
			return true;
		}
		
		return false;
	}
	
	private IEnumerator TryAttack(){
		//~ int radAtt=Randoem.Range(1,3);
		
		//~ if(!Physics.Raycast)
		
		while(true){
			
			yield return 1;
		}
		
	}
	
	void OnDestroy(){
		EnemyAIControl.ai.Remove(this);
	}
}
