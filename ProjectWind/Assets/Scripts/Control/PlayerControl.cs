using UnityEngine;
using System.Collections;

public enum PlayerMode {NORMAL, ATTACK, DEFENCE, RANGE, }

public class PlayerControl : MonoBehaviour {
	
	public CharacterDecision decision;//命令接收者
	public WeaponControl wpControl;//武器技能指令
	
	public PlayerMode mode {get; private set;}
	
	private GameObject lockTarget;//瞄准目标
	
	private bool isDrag, isSlide;//拖拽 滑动
	private float slideTime, lockTime=0;//滑动时间, 锁定时间
	
	private float count=0;
	private bool timeSegment=false;
	
	private string[] modeState = new string[]{"正常","攻击","防御","远程",};
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnGUI() {
		GUILayout.BeginArea(new Rect(0,280, 60, 240));
		
		GUILayout.SelectionGrid((int)mode, modeState ,1 , GUILayout.Width(60), GUILayout.Height(240));
		
		GUILayout.EndArea();
	}
	
	// Update is called once per frame
	void Update () {
		
		count+=Time.deltaTime;
		
		if(count>0.15f){
			timeSegment=true;
			count=0;
		}
		else
			timeSegment=false;
		
		if(GameInput.pointObject==ClickedObject.ENEMY){//锁定瞄准的目标
			lockTarget=GameInput.currentPointObject;
			lockTime=0;
			decision.go=GameInput.currentPointObject;
		}
		else
			decision.go=null;
		
		lockTime+=Time.deltaTime;
		if(lockTime>1)//超过1秒没有碰到目标就接触锁定
			lockTarget=null;
		
		//切换模式
		if(Input.GetKey("a")){
			AttackMode();
		}
		else if(Input.GetKey("s")){
			DefenseMode();
		}
		else if(Input.GetKey("d")){
			RangeMode();
		}
		else{
			NormalMode();
		}
		
	}
	
	
	private void AttackMode(){
		mode = PlayerMode.ATTACK;
	
		if(GameInput.click){//单击 原地攻击
			decision.OnAttack(GameInput.lastHit.point,"normal");
		}
		if(GameInput.doubleClick){//双击 根据距离判断 跳斩还是突刺
			if(GameInput.clickedObject==ClickedObject.ENEMY){
				if(Vector3.Distance(GameInput.lastHit.point,decision.transform.position)>6)
					decision.JumpAttack(GameInput.lastHit.point);
				else
					decision.OnAttack(GameInput.lastHit.point,"dodge_att");
			}
			else
				decision.JumpAttack(GameInput.lastHit.point);
		}
		
		if(GameInput.slide){//滑动, 根据命令攻击
			Vector3 attDirection;
			Vector3 dir=GameInput.fingerPoint-(new Vector2(Screen.width/2f,Screen.height/2f));
			
			if(lockTarget!=null)
				attDirection=lockTarget.transform.position;
			else
				attDirection=GameInput.lastHit.point;
			
			decision.OnAttack(attDirection,wpControl.ControlUpdate());//根据武器特点发动攻击
		}
		
	}
	
	private void DefenseMode(){
		mode = PlayerMode.DEFENCE;
	
		if(GameInput.click){//单击移动
			decision.MoveTo(GameInput.lastHit.point);
		}
		if(GameInput.doubleClick){//双击闪避
			decision.OnAttack(GameInput.lastHit.point,"dodge");
		}
		if(GameInput.slide){//滑动  闪避
			decision.SlideTo(PWCamera.ConvertDirection(GameInput.fingerMove));//屏幕方向转世界
		}
	}
	private void RangeMode(){
		mode = PlayerMode.RANGE;
	
		if(GameInput.click){//单击 攻击
			decision.OnAttack(GameInput.lastHit.point,"range");
		}
		if(GameInput.hold && timeSegment){//按住 连续攻击
			Vector3 attDirection;
			Vector3 dir=GameInput.fingerPoint-(new Vector2(Screen.width/2f,Screen.height/2f));
			
			if(lockTarget!=null)
				attDirection=lockTarget.transform.position;
			else
				attDirection=GameInput.lastHit.point;
			
			
			decision.OnAttack(attDirection,"range");
			
		}
	}
	private void NormalMode(){
		mode = PlayerMode.NORMAL;
		
		if(GameInput.hold && timeSegment){//按住 移动
			decision.MoveTo(GameInput.lastHit.point);
		}
		
		if(GameInput.click&& !GameInput.doubleClick){//单击 
			if(GameInput.clickedObject==ClickedObject.ENEMY){//单击敌人, 攻击
				string attType="normal";
				if(Vector3.Distance(GameInput.lastHit.point,decision.transform.position)>4)
					attType="range";
				
				decision.OnAttack(GameInput.lastHit.point,attType);
			}
			else{//否则 移动
				decision.MoveTo(GameInput.lastHit.point);
			}
		}
		if(GameInput.doubleClick){//双击 跳跃
			if(GameInput.clickedObject==ClickedObject.ENEMY){//双击敌人, 跳跃攻击
				decision.JumpAttack(GameInput.lastHit.point);
			}
			else//否则跳跃
				decision.JumpTo(GameInput.lastHit.point);
		}
		
	}
	
}
