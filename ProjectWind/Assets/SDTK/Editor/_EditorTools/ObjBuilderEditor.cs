/*
	生成模型
	
*/

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ObjBuilderEditor : EditorWindow {

	private string filePath;
	private string fileName="newObj";
	private List<GameObject> exportGOs;
	private bool isIncludeInactive=false;
	private bool isSaveSeparately=false;

	[MenuItem("SDTK/Obj Builder")]
	public static void OpenAttibuteWindow(){
		EditorWindow.GetWindow(typeof(ObjBuilderEditor));
	}
	
	//init
	void OnEnable(){
		filePath=EditorPrefs.GetString("ObjExport",Application.dataPath+"/CreatedObj");
		if(filePath == Application.dataPath+"/CreatedObj"){
			if(!Directory.Exists(filePath)){
				Directory.CreateDirectory(filePath);
				AssetDatabase.Refresh();
			}
		}
		
		isIncludeInactive=EditorPrefs.GetBool("Obj_isII",false);
		isSaveSeparately=EditorPrefs.GetBool("Obj_isSS",false);
		exportGOs=new List<GameObject>();
		
		OnSelectionChange();
	}
	
	void OnDisable(){
		EditorPrefs.SetBool("Obj_isII",isIncludeInactive);
		EditorPrefs.SetBool("Obj_isSS",isSaveSeparately);
	}
	
	void OnInspectorUpdate(){
		this.Repaint();
	}
	
	void OnGUI(){
		GUILayout.BeginHorizontal();
			GUILayout.Label("储存路径："+filePath);
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("更改")){
				filePath=EditorUtility.OpenFolderPanel("选择储存目录",filePath,"");
				EditorPrefs.SetString("ObjExport",filePath);
				
			}
			
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			GUILayout.Label("文件名: ");
			fileName=EditorGUILayout.TextField(fileName,GUILayout.MinWidth(80));
			GUILayout.Label((isSaveSeparately?"_name_sd.obj":"_sd.obj"));
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.Space(20);
		
		isIncludeInactive=GUILayout.Toggle(isIncludeInactive,"包含没有active的物体\nInclude non active GameObjects");
		isSaveSeparately=GUILayout.Toggle(isSaveSeparately,"储存为多个文件\nSave as Separate file");
			
		if(exportGOs!=null && exportGOs.Count>0){
			ShowInfo();
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
			if(GUILayout.Button("确定")){
				Apply();
				this.Close();
			}
			if(GUILayout.Button("应用")){
				Apply();
			}
			if(GUILayout.Button("取消")){
				this.Close();
			}
		GUILayout.EndHorizontal();
		
		GUILayout.Label("保险起见只有场景中的物体才能被选择导出\nFor safety reason, only scenes gameObjects can be use to export");
	}
	
	private void Apply(){
		MeshFilter[] meshFilters;
		List<MeshFilter> tempList=new List<MeshFilter>();
		int i=0;
		
		foreach(GameObject go in exportGOs){
			meshFilters=go.GetComponentsInChildren<MeshFilter>();
			
			if(isSaveSeparately){//save separately
				SaveAsObj(meshFilters,fileName+"_"+go.name,filePath);
			}
			else{
				for(i=0; i<meshFilters.Length; i++){
					tempList.Add(meshFilters[i]);
				}
			}
		}
		
		if(isSaveSeparately){
			AssetDatabase.Refresh();
			return;
		}
		
		meshFilters=new MeshFilter[tempList.Count];
		
		for(i=0; i<meshFilters.Length; i++){
			meshFilters[i]=tempList[i];
		}
		
		//save together
		SaveAsObj(meshFilters,fileName,filePath);
		AssetDatabase.Refresh();
	}
	
	private void SaveAsObj(MeshFilter[] meshFilters,string fileName,string filePath){
		bool isOverride=false;
		FileInfo createFile=new FileInfo(filePath+"/"+fileName+"_sd.obj");
		
		if(createFile.Exists){
			isOverride=EditorUtility.DisplayDialog("储存Mesh","文件 "+name+"_sd.obj 已存在，是否覆盖？","是","否");
			
			if(isOverride==false){
				return;
			}
		}
		ObjBuilder.SaveAsObj(meshFilters,fileName,filePath,isOverride);
	}
	
	private void ShowInfo(){
		GUILayout.Label("Total Select GameObject "+exportGOs.Count);
	}
	
	void OnSelectionChange(){
		exportGOs=new List<GameObject>();
		foreach(GameObject go in Selection.gameObjects){
			if(EditorUtility.IsPersistent((UnityEngine.Object) go )){//不选择不在场景里的物体
				continue;
			}
			
			if(go.GetComponentsInChildren<Renderer>(isIncludeInactive).Length>0){//检查物体的MeshFilter
				if(!exportGOs.Contains(go.transform.root.gameObject)){
					exportGOs.Add(go.transform.root.gameObject);
				}
			}
		}
	}
	
}
