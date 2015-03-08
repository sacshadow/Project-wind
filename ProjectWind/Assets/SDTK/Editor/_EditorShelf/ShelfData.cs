/*
	工具架编辑器
	
*/

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SDTK{

	[Serializable]
	public class Icon{
		public enum IconType{NONE,SCRIPT, EDITOR, PREFABE}
		
		public string name;
		public string tooltip;
		public string data;
		public string picID;
		public IconType iconType;
		
		public Icon(){
			name="New Icon";
			tooltip="";
			data="";
			picID="";
			iconType=IconType.NONE;
		}
		public Icon(string name, string data, IconType iconType){
			this.name=name;
			this.data=data;
			this.iconType=iconType;
			
			switch(iconType){
				case IconType.SCRIPT: picID=AssetDatabase.AssetPathToGUID("Assets/SDTK/Resources/script.png");break;
				case IconType.EDITOR: picID=AssetDatabase.AssetPathToGUID("Assets/SDTK/Resources/EditorScript.png");break;
				case IconType.PREFABE: picID=AssetDatabase.AssetPathToGUID("Assets/SDTK/Resources/prefab.png");break;
				default: picID="";break;
			}
			
			tooltip=this.name.Replace("\n"," ");
		}
		
		public void Apply(){
			if(iconType==IconType.NONE)
				throw new Exception("Icon No Data");
			else if(iconType==IconType.SCRIPT)
				AddComponents();
			else if(iconType==IconType.EDITOR)
				EditorApplication.ExecuteMenuItem(data);
			else if(iconType==IconType.PREFABE)
				InstancePrefab();
		}
		
		public static implicit operator GUIContent(Icon icon){
			GUIContent rt=new GUIContent(icon.name, icon.tooltip);
			if(icon.picID==null || icon.picID.Length<0)
				return rt;
			
			string path=AssetDatabase.GUIDToAssetPath(icon.picID);
			Texture2D pic=AssetDatabase.LoadAssetAtPath(path,typeof(Texture2D)) as Texture2D;
			
			rt.image=pic;
			return rt;
		}
		
		private void AddComponents(){
			Undo.RegisterUndo(Selection.gameObjects,"AddComponent "+data);
		
			foreach(GameObject go in Selection.gameObjects)
				go.AddComponent(data);
		}
		
		private void InstancePrefab(){
			if(data==null || data.Length==0)
				throw new Exception("Lose prefab");
			
			string path=AssetDatabase.GUIDToAssetPath(data);
			
			GameObject prefab=AssetDatabase.LoadAssetAtPath(path,typeof(GameObject)) as GameObject;
			
			if(prefab==null)
				throw new Exception("Lose prefab");
			
			UnityEngine.Object obj=PrefabUtility.InstantiatePrefab(prefab);
			Selection.activeObject=obj;
			EditorApplication.ExecuteMenuItem("GameObject/Move To View");
		}
		
	}

	[Serializable]
	public class Tag{
		public string name;
		public List<Icon> icon;
		
		public Tag(){
			name="New Tag";
			icon=new List<Icon>();
		}
		public Tag(string name){
			this.name=name;
			icon=new List<Icon>();
		}
		
		public void AddScript(string name, string data){
			icon.Add(new Icon(name,data,Icon.IconType.SCRIPT));
		}
		
		public void AddMenuItem(string name, string data){
			icon.Add(new Icon(name,data,Icon.IconType.EDITOR));
		}
		
		public void AddPrefab(UnityEngine.Object prefab){
			if(PrefabUtility.GetPrefabType(prefab)!=PrefabType.Prefab &&
				PrefabUtility.GetPrefabType(prefab)!=PrefabType.ModelPrefab 
			)
				throw new Exception("Only Prefab can save to shelf; object type is "+PrefabUtility.GetPrefabType(prefab));
			
			
			string path=AssetDatabase.GetAssetPath(prefab);
			string guid=AssetDatabase.AssetPathToGUID(path);
			
			icon.Add(new Icon(prefab.name,guid,Icon.IconType.PREFABE));
		}
		
	}
	
	[Serializable]
	public class Shelf{
		private const string cfgPath="Shelf/shelf.cfg";
		public static Shelf shelf;
		public static bool isChanged=false;
		
		public int lastOpenedTag;
		public List<Tag> tag;
		
		public Shelf(){
			lastOpenedTag=0;
			tag=new List<Tag>();
		}
		
		public int GetLastOpenedTag(){
			if(tag.Count==0)
				throw new System.Exception("Data Error");
			
			return Mathf.Clamp(lastOpenedTag,0,tag.Count-1);
		}
		
		public static void SaveShelf(){
			SDTKConfig.CheckDirectory("Shelf");
			SDTKConfig.SaveData<Shelf>(shelf,cfgPath);
		}
		
		public static void LoadShelf(){
			CheckDefault();
			shelf=SDTKConfig.LoadData<Shelf>(cfgPath);
		}
		
		public static void AddTag(string tagName){
			shelf.tag.Add(new Tag(tagName));
		}
		
		public static void RemveTag(int tagId){
			shelf.tag.RemoveAt(tagId);
		}
		
		public static void AcceptDrag(int tagId){
			DragAndDrop.AcceptDrag();
			
			UnityEngine.Object[] dragObjects= DragAndDrop.objectReferences;
			
			string path;
			
			foreach(UnityEngine.Object obj in dragObjects){
				
				path=AssetDatabase.GetAssetPath(obj);
				
				if(path.Contains("/Editor/"))//编辑器脚本，查找menuItem
					AcceptMenuItem(ShelfTools.Analysis(path), tagId);
				else if(path.Contains(".js") || path.Contains(".cs"))
					shelf.tag[tagId].AddScript(obj.name,obj.name);
				else if(path.Contains(".prefab"))
					shelf.tag[tagId].AddPrefab(obj);
			}
			
			SaveShelf();
			EditorShelf.Instance.RefreshIcon();
			isChanged=true;
		
			Event.current.Use();
		}
		
		/****** Priveate********/
		private static void AcceptMenuItem(List<string> menuItem, int tagId){
			string name;
			
			for(int i=0; i<menuItem.Count; i++){
				name=menuItem[i].Substring(menuItem[i].LastIndexOf("/")+1);
				shelf.tag[tagId].AddMenuItem(ShelfTools.WarpText(name), menuItem[i]);
			}
		}
		
		private static void CheckDefault(){
			if(!SDTKConfig.IsDataExists(cfgPath))
				CreateShlef();
		}
		
		private static void AddMenuItem(string name, string path){
			shelf.tag[shelf.tag.Count-1].AddMenuItem(name, path);
		}
		
		private static void CreateShlef(){
			Debug.Log("crt");
			shelf=new Shelf();
		
			AddTag("项目设置");
			AddMenuItem("渲染","Edit/Render Settings");
			AddMenuItem("输入","Edit/Project Settings/Input");
			AddMenuItem("Tag &\nLayer","Edit/Project Settings/Tags");
			AddMenuItem("NavMesh\nLayers","Edit/Project Settings/NavMeshLayers");
			AddMenuItem("音效","Edit/Project Settings/Audio");
			AddMenuItem("时间","Edit/Project Settings/Time");
			AddMenuItem("客户端","Edit/Project Settings/Player");
			AddMenuItem("物理","Edit/Project Settings/Physics");
			AddMenuItem("品质","Edit/Project Settings/Quality");
			AddMenuItem("网络","Edit/Project Settings/Network");
			AddMenuItem("编辑器","Edit/Project Settings/Editor");
			AddMenuItem("脚本执行\n顺序","Edit/Project Settings/Script Execution Order");
			
			AddTag("创建物体");
			AddMenuItem("摄像机","GameObject/Create Other/Camera");
			AddMenuItem("GUI\nText","GameObject/Create Other/GUI Text");		
			AddMenuItem("GUI\nTexture","GameObject/Create Other/GUI Texture");	
			AddMenuItem("3D\nText","GameObject/Create Other/3D Text");
			
			AddMenuItem("Directional\nLight","GameObject/Create Other/Directional Light");
			AddMenuItem("Point\nLight","GameObject/Create Other/Point Light");
			AddMenuItem("Spot\nLight","GameObject/Create Other/Spotlight");
			AddMenuItem("Area\nLight","GameObject/Create Other/Area Light");
			
			
			AddMenuItem("粒子系统","GameObject/Create Other/Particle System");
			AddMenuItem("粒子系统\n(Old)","SDTK/Create Partical Emitter");

			AddMenuItem("空物体","SDTK/Create Empty");
			AddMenuItem("正方","GameObject/Create Other/Cube");
			AddMenuItem("球体","GameObject/Create Other/Sphere");
			AddMenuItem("胶囊","GameObject/Create Other/Capsule");
			AddMenuItem("圆柱","GameObject/Create Other/Cylinder");
			AddMenuItem("面片","GameObject/Create Other/Plane");
			
			AddMenuItem("布料","GameObject/Create Other/Cloth");
			AddMenuItem("Audio\nReverb\nZone","GameObject/Create Other/Audio Reverb Zone");
			AddMenuItem("布娃娃\n系统","GameObject/Create Other/Ragdoll...");
			AddMenuItem("树木\n创建器","GameObject/Create Other/Tree");
			AddMenuItem("风场","GameObject/Create Other/Wind Zone");
			
			AddTag("物理组件");
			AddMenuItem("刚体","Component/Physics/Rigidbody");
			AddMenuItem("角色\n控制器","Component/Physics/Character Controller");
			AddMenuItem("Box\n碰撞","Component/Physics/Box Collider");
			AddMenuItem("Sphere\n碰撞","Component/Physics/Sphere Collider");
			AddMenuItem("Mesh\n碰撞","Component/Physics/Mesh Collider");
			AddMenuItem("Wheel\n碰撞","Component/Physics/Wheel Collider");
			AddMenuItem("悬挂\nJoint","Component/Physics/Hinge Joint");
			AddMenuItem("固定\nJoint","Component/Physics/Fixed Joint");
			AddMenuItem("弹簧\nJoint","Component/Physics/Spring Joint");
			AddMenuItem("可调\nJoint","Component/Physics/Configurable Joint");
			
			AddTag("窗口");
			AddMenuItem("Scene","Window/Scene");
			AddMenuItem("Game","Window/Game");
			AddMenuItem("Inspector","Window/Inspector");
			AddMenuItem("Hierarchy","Window/Hierarchy");
			AddMenuItem("Project","Window/Project");
			AddMenuItem("Animation","Window/Animation");
			AddMenuItem("Profiler","Window/Profiler");
			AddMenuItem("Particle\nEffect","Window/Particle Effect");
			AddMenuItem("Asset\nStore","Window/Asset Store");
			AddMenuItem("Asset\nServer","Window/Asset Server");
			AddMenuItem("Lightmapping","Window/Lightmapping");
			AddMenuItem("Occlusion\nCulling","Window/Occlusion Culling");
			AddMenuItem("Navigation","Window/Navigation");
			AddMenuItem("Console","Window/Console");
			
			AddTag("自定义编辑器");
			AddTag("自定义脚本");
			
			SaveShelf();
		}
		
		
		
	}
	


}