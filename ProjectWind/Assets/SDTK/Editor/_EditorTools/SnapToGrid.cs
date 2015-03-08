/*	
	物体吸附到网格
*/
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using SDTK;

[Serializable]
public class STG_Config{
	public float snapSize;
	public STG_Config(){
		this.snapSize=1;
	}
	public STG_Config(float snapSize){
		this.snapSize=snapSize;
	}
}

public class SnapToGrid : EditorWindow {
	private static string[] s_snapSize=new string[]{"1","2","5","10","Custom"};
	private static float[] sizeoptions=new float[]{1,2,5,10};
	private static int index=0;
	private static float snapSize=1;
	private const string svName="stg.xml";
	
	private static void SnapGridSize(Transform t){
		Vector3 p=t.position;
		if(snapSize<=0)
			snapSize=1;
		for(int i=0; i<3; i++){
			p[i]=Mathf.Round(p[i]/snapSize)*snapSize;
		}
		t.position=p;
	}
	
	[MenuItem("SDTK/Auto Snap To Grid &x")]
	public static void SnapToGridSize(){
		if(!SDTKConfig.IsDataExists(svName))
			SDTKConfig.SaveData<STG_Config>(new STG_Config(1),svName);

		SDTKConfig.LoadData<STG_Config>(svName);
		
		Undo.RegisterUndo(Selection.transforms,"SnapToGrid");
		
		foreach(Transform t in Selection.transforms){
			SnapGridSize(t);
		}
	}
	
	
	private void Apply(){
		SDTKConfig.SaveData<STG_Config>(new STG_Config(snapSize),svName);
		SnapToGridSize();
	}
	
	void OnGUI(){
		if(index!=4){
			snapSize=sizeoptions[index];
			GUILayout.Label("Snap: "+snapSize.ToString());
		}
		else
			snapSize=EditorGUILayout.FloatField("Snap: ",snapSize,GUILayout.Width(220));
		
		index=GUILayout.Toolbar(index,s_snapSize);
		
		GUILayout.Label("Select Transform: "+Selection.transforms.Length);
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Apply"))
			Apply();
		if(GUILayout.Button("Close"))
			Close();
		GUILayout.EndHorizontal();
	}
	
	void OnInspectorUpdate(){
		Repaint();
	}
	
	[MenuItem("SDTK/Modify/Snap To Grid")]
	public static void SnapToGridWindow(){
		EditorWindow.GetWindow(typeof(SnapToGrid));
	}
}
