using UnityEngine;
using System.Collections;

public class PCInput : PWInputBase {
	public override Vector3 GetMousePosition(){
		return Input.mousePosition;
	}
	
	public override bool GetClick(){//点击
		return Input.GetMouseButtonDown(0);
	}
	public override bool GetHold(){//按住
		return Input.GetMouseButton(0);
	}
}
