using UnityEngine;
using System.Collections;

public abstract class PWInputBase {
	public abstract Vector3 GetMousePosition();
	
	public abstract bool GetClick();//点击
	public abstract bool GetHold();//按住
}
