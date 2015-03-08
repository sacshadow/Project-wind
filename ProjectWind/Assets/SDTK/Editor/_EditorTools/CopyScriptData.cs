/*
	ToDo: copy more than one same script on object to other
	
	复制脚本信息到其他物体
	Copy scripts from a gameObject and paste to others
	
	感谢 海马 提供技术支持
	Thanks to 海马 who teach me how to us reflection
*/
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

public class ScriptData{
	public string scriptName;
	public Type t;
	public FieldInfo[] field;
	public object[] data;
	public string[] dataString;
	public bool isPaste;
	
	public ScriptData(MonoBehaviour script){
		t=script.GetType();
		scriptName=t.ToString();
		
		
		field=t.GetFields();
		data=new object[field.Length];
		dataString=new string[field.Length];
		
		for(int dCount=0; dCount<data.Length; dCount++){
			data[dCount]=field[dCount].GetValue(script);
			dataString[dCount]=field[dCount].GetValue(script).ToString();
		}
		
		isPaste=true;
	}
	
	public void PasteToScript(MonoBehaviour target){
		if(target.GetType() != t)
			throw new Exception("Target script "+target.GetType().ToString()+"!= org script "+scriptName);
		
		for(int fCount=0; fCount<field.Length; fCount++){
			field[fCount].SetValue(target,data[fCount]);
		}
	}
	
	public string GetFileInfo(int index){
		return field[index].Name+":        "+dataString[index];
	}
}

public class CopyScriptData : EditorWindow {
	public static string[] step=new string[]{"Copy","Paste"};
	
	private GameObject selectObject;
	private Transform[] selectTransforms;
	private string selectObjectNames;
	
	private MonoBehaviour[] mono;
	private bool[] isCopy;
	private string[] scriptName;
	private string lastCopiedObjectName="";
	private List<ScriptData> data;
	
	private int selectStep=0;
	
	private Vector2 scrollPosition;

	[MenuItem("SDTK/Copy Script Data")]
	public static void CopyScriptDataWindow(){
		EditorWindow.GetWindow(typeof(CopyScriptData));
	}
	
	void OnInspectorUpdate(){
		Repaint();
		
		if(selectObject==null)
			return;
		
		MonoBehaviour[] temp=selectObject.GetComponents<MonoBehaviour>();
		
		if(temp.Length!=mono.Length)
			OnSelectionChange();
	}
	
	void OnEnable(){
		OnSelectionChange();
	}
	
	void OnSelectionChange(){
		Transform newSelect= Selection.activeTransform;
		selectTransforms=Selection.transforms;
		
		//~ selectScript=0;
		
		if(newSelect==null){
			selectObject=null;
			return;
		}
		
		selectObject=newSelect.gameObject;
			
		mono=selectObject.GetComponents<MonoBehaviour>();
			
		isCopy=new bool[mono.Length];
		scriptName=new string[mono.Length];
		
		for(int mCount=0; mCount<mono.Length; mCount++){
			isCopy[mCount]=true;
			
			scriptName[mCount]=mono[mCount].GetType().ToString();
		}
		
		selectObjectNames="";
		for(int sCount=0; sCount<selectTransforms.Length; sCount++){
			selectObjectNames+=selectTransforms[sCount].gameObject.name+"; ";
		}
	}
	
	void OnGUI(){		
		selectStep=GUILayout.Toolbar(selectStep, step);
		
		if(selectStep==0)
			CopyFromSelect();
		else if(selectStep==1)
			PasteToSelect();
		
	}
	
	private void CopyFromSelect(){
		if(selectObject==null){
			GUILayout.Label("Nathing Selected");
			return;
		}
		
		GUILayout.Label("Copy from: "+selectObject.name);
		
		GUILayout.Space(20);
		
		Rect r=EditorGUILayout.BeginHorizontal("Toolbar");
		GUILayout.Label("Scripts to Copy:");
		EditorGUILayout.EndHorizontal();
		
		if(mono==null || mono.Length==0)
			return;
		scrollPosition=GUILayout.BeginScrollView(scrollPosition, "Box",GUILayout.Width(r.width));
		for(int mCount=0; mCount<mono.Length; mCount++){
			GUILayout.BeginHorizontal();
			GUILayout.Label(scriptName[mCount]);
			GUILayout.FlexibleSpace();
			isCopy[mCount]=GUILayout.Toggle(isCopy[mCount],"");
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("All"))
			SetAllSelect(true);
		if(GUILayout.Button("None"))
			SetAllSelect(false);
		
		GUILayout.EndHorizontal();
		
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Copy"))
			Copy();
		
		if(GUILayout.Button("Cancel"))
			Close();
		
		GUILayout.EndHorizontal();
	}
	
	private string[] reactType=new string[]{"Ignore","Add new script"};
	private int react=0;
	
	private void PasteToSelect(){
		if(selectTransforms==null|| selectTransforms.Length==0){
			GUILayout.Label("Nathing Selected");
			return;
		}
		
		GUILayout.Label("Paste to: "+selectObjectNames);

		GUILayout.Space(20);
		
		Rect r=EditorGUILayout.BeginHorizontal("Toolbar");
		GUILayout.Label("Scripts copyed from "+ lastCopiedObjectName+":");
		EditorGUILayout.EndHorizontal();
		
		if(data==null || data.Count==0)
			return;
		scrollPosition=GUILayout.BeginScrollView(scrollPosition, "Box",GUILayout.Width(r.width));
		for(int dCount=0; dCount<data.Count; dCount++){
			GUILayout.BeginHorizontal();
			GUILayout.Label(data[dCount].scriptName);
			GUILayout.FlexibleSpace();
			data[dCount].isPaste=GUILayout.Toggle(data[dCount].isPaste,"");
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("All"))
			SetAllPaste(true);
		if(GUILayout.Button("None"))
			SetAllPaste(false);
		
		GUILayout.EndHorizontal();
		
		GUILayout.Space(40);
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("If scripts does not exist, ");
		
		react=EditorGUILayout.Popup(react, reactType);
		
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Paste"))
			Paste();
		
		if(GUILayout.Button("Cancel"))
			Close();
		
		GUILayout.EndHorizontal();
	}
	
	private void SetAllPaste(bool state){
		if(data==null || data.Count==0)
			return;
		
		for(int dCount=0; dCount<data.Count; dCount++)
			data[dCount].isPaste=state;
	}
	
	private void SetAllSelect(bool state){
		for(int cCount=0; cCount<isCopy.Length; cCount++)
			isCopy[cCount]=state;
	}
	
	private void Copy(){
		lastCopiedObjectName=selectObject.name;
		
		data=new List<ScriptData>();
		string copyedScript="";
		
		for(int mCount=0; mCount<mono.Length; mCount++){
			if(isCopy[mCount]){
				data.Add(new ScriptData(mono[mCount]));
				copyedScript+=mono[mCount].ToString()+";";
			}
		}
		
		Debug.Log(data.Count+" Scripts copied: "+copyedScript);
		
		if(data.Count>0)
			selectStep=1;
	}
	
	private void Paste(){
		for(int tCount=0; tCount<selectTransforms.Length; tCount++)
			PasteTo(selectTransforms[tCount].gameObject);
	}
	
	private void PasteTo(GameObject go){
		foreach(ScriptData sd in data)
			if(sd.isPaste) PasteData(sd,go);
		
	}
	
	private void PasteData(ScriptData sd, GameObject go){
		MonoBehaviour temp=go.GetComponent(sd.scriptName) as MonoBehaviour;
		
		if(temp==null){
			if(reactType[react]=="Ignore")
				return;
			else if(reactType[react]=="Add new script")
				temp=go.AddComponent(sd.scriptName) as MonoBehaviour;
			else return;
		}
		
		sd.PasteToScript(temp);
		
	}
	
}
