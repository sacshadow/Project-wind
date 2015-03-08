using UnityEngine;
using UnityEditor;
using System.Collections;
using SDTK;

public class EditorShelf : EditorWindow {
	public static EditorShelf Instance;
	private static Shelf shelf{
		get{
			return Shelf.shelf;
		}
	}
	public static int tagId=0;
	private static int lastTagId=-1;
	private static string[] tagName;
	private static GUIContent[] icon;
	
	private Vector2 scrollPosition;
	
	[MenuItem("SDTK/Editor Shelf &q")]
	public static EditorShelf OpenWindow(){
		return EditorWindow.GetWindow(typeof(EditorShelf)) as EditorShelf;
	}
	
	void OnEnable(){
		Instance=this;
		Shelf.LoadShelf();
		RefreshShelf();
	}
	
	public void RefreshShelf(){
		GetTags();
		
		if(shelf.tag.Count>0)
			tagId=Mathf.Clamp(shelf.lastOpenedTag, 0, tagName.Length);
		
		RefreshIcon();
	}
	
	private void GetTags(){
		tagName=new string[shelf.tag.Count];
		
		for(int tCount=0; tCount<tagName.Length; tCount++)
			tagName[tCount]=shelf.tag[tCount].name;
	}
	
	void OnInspectorUpdate(){
		Repaint();
	}
	
	void OnGUI(){
		if(shelf==null || shelf.tag==null || shelf.tag.Count==0){
			GUILayout.Label("Error occur\nPlease delete the \"shelf.cfg\" file inside \"SDTK/_Data/Shelf\" folder, then close this window and open again");
			return;
		}
		
		scrollPosition=GUILayout.BeginScrollView(scrollPosition);
		DrawTag();
		DrawBtn();
		GUILayout.EndScrollView();
		
		DragUpdate();
	}
	
	private void DrawTag(){
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("设置",GUILayout.Width(40)))
			ShowSetup();
		
		tagId=GUILayout.Toolbar(tagId,tagName);
		
		if(tagId!=lastTagId)
			RefreshIcon();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	private void ShowSetup(){
		EditShelfData.OpenWindow(tagId);
	}
	
	public void RefreshIcon(){
		tagId=Mathf.Clamp(tagId,0,shelf.tag.Count-1);
		
		icon=new GUIContent[shelf.tag[tagId].icon.Count];
		
		for(int i=0; i<icon.Length; i++){
			icon[i]=shelf.tag[tagId].icon[i];
		}
		
		shelf.lastOpenedTag=lastTagId=tagId;
		Shelf.SaveShelf();
	}
	
	private void DrawBtn(){
		GUILayout.BeginHorizontal();
		
		for(int i=0; i<icon.Length; i++)
			DrawEach(i);
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	private void DrawEach(int index){
		
		if(Event.current.control){
			
			GUILayout.BeginVertical();
			if(GUILayout.Button(icon[index],GUILayout.Height(50)))
				shelf.tag[tagId].icon[index].Apply();
			if( GUILayout.Button("删除")){
				Shelf.shelf.tag[tagId].icon.RemoveAt(index);
				Shelf.isChanged=true;
				RefreshIcon();
			}
			GUILayout.EndVertical();
		}
		else if(GUILayout.Button(icon[index],GUILayout.Height(50)))
			shelf.tag[tagId].icon[index].Apply();
	}
	
		//监听拖拽
	private void DragUpdate(){
		EventType eventType = Event.current.type;
		if (eventType == EventType.DragUpdated)
			CheckLegal();
		
		if (eventType == EventType.DragPerform)
			Shelf.AcceptDrag(tagId);
	}
	
		//拖拽更新
	private void CheckLegal(){
		DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
		Event.current.Use();
	}
}
