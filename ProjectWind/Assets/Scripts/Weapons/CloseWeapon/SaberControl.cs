using UnityEngine;
using System.Collections;

public class SaberControl : WeaponControl {

	public Saber saber;
	
	void Awake(){
		
	}
	
	public override string ControlUpdate(){//技能发动条件
		
		if(saber.GetLastState()=="up"){
			if(GameInput.inputDirection==InputDirection.DOWNLEFT)
				return "dodge";
			if(GameInput.inputDirection==InputDirection.DOWN)
				return "down";
			else
				return "";
		}
		
		if(GameInput.inputDirection==InputDirection.UP)
			return "up";
		
		if(saber.GetLastState()=="" && GameInput.inputDirection==InputDirection.LEFT)
			return "dodge";
		
		if(saber.GetLastState()=="dodge" && GameInput.inputDirection==InputDirection.RIGHT)
			return "dodge_att";
		
		if(saber.GetLastState()=="down" && GameInput.inputDirection==InputDirection.RIGHT)
			return "dodge_att";
		
		if(saber.GetLastState()=="normal"){
			if(GameInput.inputDirection==InputDirection.LEFT || GameInput.inputDirection==InputDirection.RIGHT){
				if(saber.GetCount()==0)
					return "up";
				if(saber.GetCount()==1)
					return "down";
				if(saber.GetCount()==2)
					return "dodge";
			}
		}
		
		return "normal";
	}
}
