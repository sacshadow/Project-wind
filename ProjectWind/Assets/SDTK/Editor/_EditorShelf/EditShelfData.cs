using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SDTK;

public class EditShelfData : EditorWindow {
	public static string[] page=new string[]{"标签","图标"};
	public static int pageSelect=0, lastPageSelect=-1;
	public static int tagSelect=0, lastTagSelect=-1;
	public static string[] tagName;
	public static string[] iconName;
	
	private int iconSelect=0, lastIconSelect=1;
	private Icon icon;
	private Vector2 scrollPosition;
	
	public static void OpenWindow(int tagId){
		EditShelfData wd= EditorWindow.GetWindow(typeof(EditShelfData)) as EditShelfData;
		
		pageSelect=1;
		lastTagSelect=tagSelect=tagId;
		
		Shelf.LoadShelf();
		wd.GetTagName();
		wd.GetIconName();
	}
	
	void OnEnable(){
		wantsMouseMove=true;
	}
	
	private void GetTagName(){
		tagName=new string[Shelf.shelf.tag.Count];
		for(int tCount=0; tCount<tagName.Length; tCount++)
			tagName[tCount]=Shelf.shelf.tag[tCount].name;
	}
	
	private void GetIconName(){
		iconName=new string[Shelf.shelf.tag[tagSelect].icon.Count];
		for(int iCount=0; iCount<iconName.Length; iCount++){
			iconName[iCount]=Shelf.shelf.tag[tagSelect].icon[iCount].name;
			iconName[iCount]=iconName[iCount].Replace("\n"," ");
		}
	}
	
	void OnInspectorUpdate(){
		Repaint();
	}
	
	void OnGUI(){
		EditorGUILayout.BeginVertical();
		
		pageSelect=GUILayout.Toolbar(pageSelect, page,GUILayout.Width(200));
		
		if(lastPageSelect!=pageSelect){
			lastPageSelect=pageSelect;
			scrollPosition=Vector2.zero;
			iconSelect=0;
			GetIconName();
			RefreshIcon();
		}

		if(pageSelect==0)
			ShowTag();
		else
			ShowIcon();
		
		GUILayout.Space(20);
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("应用"))
			Shelf.SaveShelf();
		if(GUILayout.Button("关闭")){
			Shelf.SaveShelf();
			Close();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	private void ShowTag(){
		tagSelect=Mathf.Clamp(tagSelect,0,Shelf.shelf.tag.Count-1);
		scrollPosition=GUILayout.BeginScrollView(scrollPosition,"box");
		tagSelect=GUILayout.SelectionGrid(tagSelect,tagName,1);
		
		if(lastTagSelect!=tagSelect)
			RefreshTag();
		
		GUILayout.EndScrollView();
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("上移"))
			TagMove(-1);
		if(GUILayout.Button("下移"))
			TagMove(1);
		if(GUILayout.Button("新建"))
			NewTag();
		if(GUILayout.Button("删除"))
			DeleteTag();
		GUILayout.EndHorizontal();
		
		GUILayout.Space(20);
		GUILayout.Label("标签名称:");
		Shelf.shelf.tag[tagSelect].name=EditorGUILayout.TextField(Shelf.shelf.tag[tagSelect].name);
		GUILayout.Space(60);
		
		if(GUI.changed)
			OnGUIChange();
	}
	
	private void OnGUIChange(){
		GetTagName();
		GetIconName();
		
		if(EditorShelf.Instance!=null)
			EditorShelf.Instance.RefreshShelf();
	}
	
	private void RefreshTag(){
		if(EditorShelf.Instance!=null){
			EditorShelf.tagId=tagSelect;
			
			EditorShelf.Instance.RefreshIcon();
			EditorShelf.Instance.RefreshShelf();
		}
		lastTagSelect=tagSelect;
	}
	
	private void TagMove( int offset){
		if(tagSelect+offset==Shelf.shelf.tag.Count || tagSelect+offset<0)
			return;
		
		Tag temp=Shelf.shelf.tag[tagSelect];
		
		Shelf.shelf.tag.RemoveAt(tagSelect);
		tagSelect+=offset;
		Shelf.shelf.tag.Insert(tagSelect, temp);
		
		Shelf.SaveShelf();
		if(EditorShelf.Instance!=null)
				EditorShelf.Instance.RefreshShelf();
		GetTagName();
	}
	
	private void NewTag(){
		Shelf.AddTag("New Tag");
		
		tagSelect=Shelf.shelf.tag.Count-1;
		OnGUIChange();
		RefreshTag();
	}
	
	private void DeleteTag(){
		if(Shelf.shelf.tag.Count<=1)
			return;
		
		Shelf.shelf.tag.RemoveAt(tagSelect);
		
		tagSelect=Mathf.Clamp(tagSelect,0,Shelf.shelf.tag.Count-1);
		
		OnGUIChange();
		RefreshTag();
	}
	
	string id="";
	Texture2D pic;
	
	private void ShowIcon(){
		if(Shelf.isChanged){
			GetIconName();
			RefreshIcon();
			Shelf.isChanged=false;
		}
		
		if(Shelf.shelf.tag[tagSelect].icon.Count!=0)
			iconSelect=Mathf.Clamp(iconSelect,0,Shelf.shelf.tag[tagSelect].icon.Count-1);
		scrollPosition=GUILayout.BeginScrollView(scrollPosition,"box");
		
		if(Shelf.shelf.tag[tagSelect].icon.Count!=0)
			iconSelect=GUILayout.SelectionGrid(iconSelect,iconName,1);
		
		if(lastIconSelect!=iconSelect)
			RefreshIcon();
		
		GUILayout.EndScrollView();
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("上移"))
			IconMove(-1);
		if(GUILayout.Button("下移"))
			IconMove(1);
		if(GUILayout.Button("删除"))
			DeleteIcon();
		GUILayout.EndHorizontal();
		
		GUILayout.Space(20);
		
		
		if(Event.current.type == EventType.MouseDown)
			GUIUtility.keyboardControl=0;
		if(Shelf.shelf.tag[tagSelect].icon.Count!=0){
			GUILayout.BeginHorizontal();
			GUILayout.Label("图标名称:",GUILayout.Width(60));
			icon.name=EditorGUILayout.TextArea(icon.name,GUILayout.Height(50));
		
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("提示:",GUILayout.Width(60));
			icon.tooltip=EditorGUILayout.TextField(icon.tooltip);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("图标:",GUILayout.Width(60));
			
			if(icon.picID==null || icon.picID.Length==0)
				pic=null;
			else if(id!=icon.picID)
				ReloadPic();
			
			pic=EditorGUILayout.ObjectField(pic,typeof(Texture2D),false,GUILayout.Width(50),GUILayout.Height(50)) as Texture2D;
			
			if(pic!=null)
				icon.picID=AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(pic));
			else
				icon.picID="";
			
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			switch(icon.iconType){
				case Icon.IconType.SCRIPT: ShowScript(); break;
				case Icon.IconType.EDITOR: ShowMenuItem(); break;
				case Icon.IconType.PREFABE: ShowPrefab(); break;
			}
			GUILayout.EndHorizontal();
		}
		
		GUILayout.Space(5);
		
		if(GUI.changed){
			GetIconName();
			RefreshTag();
		}
	
	}
	
	private void ShowMenuItem(){
		GUILayout.Label("命令:",GUILayout.Width(60));
		icon.data=EditorGUILayout.TextField(icon.data);
	}
	
	private void ShowScript(){
		
	}
	
	private void ShowPrefab(){
		
	}
	
	private void ReloadPic(){
		id=icon.picID;
		string path=AssetDatabase.GUIDToAssetPath(icon.picID);
		
		if(path==null || path.Length==0){
			pic=null;
			return;
		}
		
		pic=AssetDatabase.LoadAssetAtPath(path,typeof(Texture2D)) as Texture2D;
		
	}
	
	private void RefreshIcon(){
		if(Shelf.shelf.tag[tagSelect].icon.Count!=0)
			icon=Shelf.shelf.tag[tagSelect].icon[iconSelect];
		else
			return;
		
		lastIconSelect=iconSelect;
		
		if(icon.picID==null || icon.picID.Length==0)
			return;
		
		ReloadPic();
	}
	
	private void IconMove(int offset){
		if(iconSelect+offset==Shelf.shelf.tag[tagSelect].icon.Count || iconSelect+offset<0)
			return;
		
		Icon temp=Shelf.shelf.tag[tagSelect].icon[iconSelect];
		
		Shelf.shelf.tag[tagSelect].icon.RemoveAt(iconSelect);
		iconSelect+=offset;
		Shelf.shelf.tag[tagSelect].icon.Insert(iconSelect, temp);
		
		Shelf.SaveShelf();
		if(EditorShelf.Instance!=null)
				EditorShelf.Instance.RefreshShelf();
		
		GetIconName();
	}
	
	private void DeleteIcon(){
		if(Shelf.shelf.tag[tagSelect].icon.Count==0)
			return;
		
		Shelf.shelf.tag[tagSelect].icon.RemoveAt(iconSelect);
		iconSelect=Mathf.Clamp(iconSelect-1,0,Shelf.shelf.tag[tagSelect].icon.Count);
		
		Shelf.SaveShelf();
		if(EditorShelf.Instance!=null)
				EditorShelf.Instance.RefreshShelf();
		
		GetIconName();
		RefreshIcon();
	}
	
}
