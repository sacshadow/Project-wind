/*
	复制并保存素材
	最后修改 2011-2-14
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SDTK;

public enum AssetType{ANIMATION_CLIP=0, MESH};

public delegate Object GetAsset();

public class DuplicateAssets : EditorWindow {
	
	private AssetType assetType=AssetType.ANIMATION_CLIP;
	private System.Type[] types=new System.Type[]{typeof(AnimationClip),typeof(Mesh)};
	private string[] typeExts=new string[]{"anim","asset"};
	
	private List<GetAsset> getAssetType;
	
	private Object org;
	private string path=Application.dataPath;

	private AssetType oldAsset;	
	
	private Object GetMesh(){
		return new Mesh();
	}
	
	private Object GetAnimationClip(){
		return new AnimationClip();
	}
	
	private void DuplicateSelect(){
		string ext=typeExts[(int) assetType];
		path=EditorUtility.SaveFilePanelInProject("保存素材",org.name+"."+ext,ext,"Please enter a file name");
		
		if(path.Length<8)
			return;
		
		Object copy=getAssetType[(int)assetType]();
		EditorUtility.CopySerialized(org,copy);
		AssetDatabase.CreateAsset(copy, path);
	}
	
	void OnEnable(){
		getAssetType=new List<GetAsset>();
		getAssetType.Add(GetAnimationClip);
		getAssetType.Add(GetMesh);
	}
	
	//~ void OnSelectionChange(){
		//~ Object currentSelect = Selection.activeObject;
		
		//~ if(currentSelect !=null && currentSelect.GetType()==types[(int)assetType])
			//~ oldAsset=assetType=currentSelect;
	//~ }
	
	void OnGUI(){
		assetType=(AssetType)EditorGUILayout.EnumPopup(assetType);
		if(oldAsset!=assetType)
			org=null;
		oldAsset=assetType;
		
		org=EditorGUILayout.ObjectField("选择要复制的物体 ",org,types[(int)assetType],true);
		
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		
		if(GUILayout.Button("复制"))
			DuplicateSelect();
		if(GUILayout.Button("退出"))
			Close();
		GUILayout.EndHorizontal();
	}
	
	[MenuItem("SDTK/Duplicate Assets")]//添加至菜单快捷方式
	public static void OpenDuplicateAssetsWindow(){//打开窗口
		EditorWindow.GetWindow(typeof(DuplicateAssets));
	}
}
