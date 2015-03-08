/*
	常用编辑工具合集
	最后修改 2011-2-14
*/
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SDTK{
	public class SimpleToolCollections {
		//Parent all other selected gameObjects to the FIRST selected( which show on the INSPECTOR ) gameObject
		//将其他选择物体作为当前选择物体的子物体
		[MenuItem("SDTK/Parent &p")]
		public static void Parent(){
			EditorApplication.ExecuteMenuItem("GameObject/Make Parent");
		}
		
		//Unparent all current selected gameObjects
		//移除当前选择物体的所有父子关系
		[MenuItem("SDTK/Unparent &u")]
		public static void Unparent(){
			EditorApplication.ExecuteMenuItem("GameObject/Clear Parent");
		}
		
		//Group all current selected gameObjects
		//对当前选择物体打组
		[MenuItem("SDTK/Group &g")]
		public static void Group(){
			if(Selection.transforms.Length==0)
				return;
			
			Undo.RegisterSceneUndo("Group");
			
			int index=0;
			
			while(GameObject.Find("Group_"+index.ToString())){
				index++;
			}
			
			//creat an empity gameObject for group parent
			GameObject newGroup=new GameObject("Group_"+index.ToString());
			newGroup.transform.position=Vector3.zero;
			
			//parent all select to group
			foreach(Transform t in Selection.transforms){
				t.parent=newGroup.transform;
			}
			
			Selection.activeGameObject=newGroup;
		}
		
		//Take a Screen Shot in Editor
		//在编辑器中截图并保存
		[MenuItem("SDTK/Taken Screen Shot &t")]
		public static void TakenTheScreenShot(){
			try{
				DataRW.CheckDirectory(Application.dataPath+"/ScreenShot/");
				EditorApplication.ExecuteMenuItem("Window/Game");
				ScreenShot.Instance.DoScreenShot(Application.dataPath+"/ScreenShot/");
				AssetDatabase.Refresh();
			}
			catch(Exception e){
				Debug.LogError(e.Message);
			}
		}
	
		//Create a C# script that inherit from EditorWindow
		//创建一个继承自EditorWindow的编辑器脚本
		//TODO: custom class name
		[MenuItem("Assets/Create/C# Editor Script")]
		public static void CreateEditorScript(){
			UnityEngine.Object o=Selection.activeObject;
			
			string path=Application.dataPath+"/Editor";//default path
			
			if(o!=null)//if select a folder, create new script under it;
				path=GetPathOf(AssetDatabase.GetAssetPath(o));
			
			DataRW.CheckDirectory(path);
			
			FileStream  file=new FileStream (path+"/NewEditorScript.cs",FileMode.OpenOrCreate);
			StreamWriter writer=new StreamWriter(file);
			
			writer.Write(NewEditorScript());
			
			writer.Close();
			AssetDatabase.Refresh();
			o=AssetDatabase.LoadAssetAtPath(path+"/NewEditorScript.cs",typeof(UnityEngine.Object));
			
			Selection.activeObject=o;
		}
		
		// script to write
		private static string NewEditorScript(){
			string script=
			"/*\r\n"+
			"\tCreateTime\r\n\t"+
			System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")+"\r\n"+
			"*/\r\n"+
			"using UnityEngine;\r\n"+
			"using UnityEditor;\r\n"+
			"using System.Collections;\r\n"+
			"using System.Collections.Generic;\r\n"+
			
			"\r\n"+
			"\r\n"+
			"public class NewEditorScript: EditorWindow{\r\n"+
			"\t\r\n"+
			"\t[MenuItem(\"My Editor/NewEditorScript\")]\r\n"+
			"\tpublic static void OpenWindow(){\r\n"+
			"\t\tEditorWindow.GetWindow(typeof(NewEditorScript));\r\n"+
			"\t}\r\n"+
			"\tvoid OnEnable(){\r\n"+
			"\t\t\r\n"+
			"\t}\r\n"+
			"\tvoid OnGUI(){\r\n"+
			"\t\t\r\n"+
			"\t}\r\n"+
			"}\r\n"+
			" ";
			
			return script;
		}
		
		//return the directory path
		private static string GetPathOf(string objectPath){
			if(!objectPath.Contains("."))
				return objectPath;
			
			List<string> seg=new List<string>(objectPath.Split('/'));
			seg.RemoveAt(seg.Count-1);
			
			return String.Join("/",seg.ToArray());
		}
		
		//创建一个唯一命名的新物体, 并作为当前选择物体的子物体
		//create a uniqu named GameObject and parent it under select gameObject
		[MenuItem("SDTK/Create Empty")]
		public static void CreateEmpty(){
			GetNGO("GameObject");
		}
		
		//创建一个旧的粒子系统
		//Create an legacy particle emitter;
		[MenuItem("SDTK/Create Partical Emitter")]
		public static void CreateParticalEmitter(){
			GameObject pe=GetNGO("ParticalEmitter");
			
			EditorApplication.ExecuteMenuItem("Component/Effects/Legacy Particles/Ellipsoid Particle Emitter");
			pe.AddComponent<ParticleAnimator>();
			pe.AddComponent<ParticleRenderer>();
		}
		
		//Get numbered game object
		private static GameObject GetNGO(string type){
			GameObject go=new GameObject();
			
			string name=type+"_0";
			int i=1;
			while(GameObject.Find(name)!=null){
				name=type+"_"+i.ToString();
				i++;
			}
			
			go.name=name;
			
			Transform t=Selection.activeTransform;
			
			if(t!=null)
				go.transform.parent=t;
			
			go.transform.localPosition=Vector3.zero;
			go.transform.localRotation=Quaternion.identity;
			go.transform.localScale=Vector3.zero;
			
			Selection.activeGameObject=go;
			
			return go;
		}	
	}
}
