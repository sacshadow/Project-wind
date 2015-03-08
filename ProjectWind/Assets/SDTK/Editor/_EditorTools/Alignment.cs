using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public enum AlignPoint{PIVOT=0, CENTER}

public class Alignment: EditorWindow {
	private Transform targetTransform;
	private string[] s_alignObjects=new string[]{"x","y","z","All"};
	//~ private AlignPoint alignPoint=AlignPoint.PIVOT;
	
	private void Registance(){
		Undo.RegisterUndo(Selection.transforms,"Align Objects");
	}
	
	private void SetAll(){
		Set(0);
		Set(1);
		Set(2);
	}	
	
	private void AlignObjects(int index){
		Registance();
		
		if(index!=3)
			Set(index);
		else
			SetAll();
	}
	
	private void Set(int index){
		Transform[] alignObjects=Selection.transforms;
		Vector3 target=targetTransform.position;
		Vector3 change;
		for(int i=0; i<alignObjects.Length; i++){
			change=alignObjects[i].position;
			change[index]=target[index];
			alignObjects[i].position=change;
		}
	}
	
	private void ShowGUI(){
		Vector3 p=targetTransform.position;
		//~ alignPoint=(AlignPoint)EditorGUILayout.EnumPopup("对齐方式：",alignPoint);TODO 不同的对齐方式
		GUILayout.Label("对齐到 "+targetTransform.name);
		GUILayout.Label(String.Format("x: {0} y:{1} z:{2}",p.x.ToString("f3"),p.y.ToString("f3"),p.z.ToString("f3")));
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		for(int i=0; i<s_alignObjects.Length; i++){
			if(GUILayout.Button(s_alignObjects[i]))
				AlignObjects(i);
		}
		GUILayout.EndHorizontal();
	}
	
	void OnEnable(){
		OnSelectionChange();
	}
	
	void OnGUI(){
		if(targetTransform!=null)
			ShowGUI();
		else
			GUILayout.Label("没有选择物体");
	}
	
	void OnSelectionChange(){
		targetTransform=Selection.activeTransform;
	}
	
	void OnInspectorUpdate(){
		Repaint();
	}
	
	[MenuItem("SDTK/Modify/Alignment")]
	public static void AlignmentWindow(){
		EditorWindow.GetWindow(typeof(Alignment));
	}
}
