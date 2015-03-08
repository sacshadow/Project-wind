/*
	批量设置预设体
	2011-12-30
*/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SDTK;

[System.Serializable]
public class MP_Config{
	public string dataPath;
}

public class MakePrefabs{
	private const string svName="mp.xml";
	public static MP_Config cfg;
	
	//create select gameObjects to perfabs by their name and store in path "Assets/Prefabs/" 
	//将选择的物体做成 prefab并直接放置在"Assets/Prefabs/" 目录下
	[MenuItem("SDTK/Make Prefabs &m")]
	public static void QuickCreate(){
		if (Selection.transforms.Length==0)
			return;
			
		GetConfig();
		
		CreatePrefabs(Selection.transforms,cfg.dataPath);
	}
	
	//Create prefab of Current Select gameObject
	//将当前选择的物体做成prefab放在指定目录下
	[MenuItem("SDTK/Modify/Make Prefab")]
	public static void CreateAtPath(){
		if (Selection.transforms.Length<1){ 
			EditorUtility.DisplayDialog("Make Perfab Error","No GameObject Selected","Ok");
			return;
		}
		
		GetConfig();
		
		string title="Camera prefab Not allowed; If an existed folder selected, press 'Yes' of the next dialog";
		
		string path=EditorUtility.SaveFolderPanel(title,Application.dataPath+"/"+cfg.dataPath,"");
		
		if(path.Length != 0 && path.Contains(Application.dataPath)){
			PathAnalysis(path);
		}
		else{
			Debug.LogError("Path Error; Prefabs not created");
		}
	}
	
	private static void SaveData(){
		SDTKConfig.SaveData<MP_Config>(cfg,svName);
	}
	private static void LoadData(){
		cfg=SDTKConfig.LoadData<MP_Config>(svName);
		if(cfg==null)
			CreateDefaultConfig();
	}
	
	private static void CreateDefaultConfig(){
		cfg=new MP_Config();
		cfg.dataPath="Prefabs/";
		SaveData();
	}
	
	private static void PathAnalysis(string path){
		int i=Application.dataPath.Length+1;
		string localPath=path.Substring(i,path.Length-i);
		
		cfg.dataPath=localPath+"/";
		
		SaveData();
		
		CreatePrefabs(Selection.transforms,cfg.dataPath);
	}
	
	private static void GetConfig(){
		if(!SDTKConfig.IsDataExists(svName))
			CreateDefaultConfig();
		else
			LoadData();
	}	
	
	private static string ReplaceGameObject(GameObject go, string path){
		if (EditorUtility.DisplayDialog("Are you sure?", "The prefab <"+go.name+"> already exists. Do you want to replace it?", "Yes", "No")){
			CreateNew(go, path);
			return "";
		}
		return go.name+"; ";
	}
	
    private static void CreatePrefabs(Transform[] trans, string localPath) {
		string path;
		string warning="";
		
		DataRW.CheckDirectory(Application.dataPath+"/"+localPath);
		AssetDatabase.Refresh();
		
        foreach(Transform  t in trans) {
			path="assets/"+localPath+t.gameObject.name+".prefab";
			
			if(t.gameObject.GetComponent<Camera>()!=null){
				Debug.LogWarning("Sorry, CameraObjects can not use this function to Create it's Perfab");
				warning+=t.gameObject.name+";";
				continue;
			}
			
            if (AssetDatabase.LoadAssetAtPath(path, typeof(GameObject))!=null) {
                warning+=ReplaceGameObject(t.gameObject,path);
            }
			else{
				CreateNew(t.gameObject,path);
			}
        }
		
		if(warning.Length>0){
			Debug.LogWarning("Prefab of "+warning+"is not create");
		}
		
		AssetDatabase.Refresh();
    }

    private static void CreateNew(GameObject obj, string localPath) {
        Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
		//~ Transform parent=obj.transform.parent;    //TODO: 保持物体层级； 尝试 使用物体唯一ID判断
        PrefabUtility.ReplacePrefab(obj, prefab);
		
        GameObject.DestroyImmediate(obj);
        //~ Object temp=EditorUtility.InstantiatePrefab(prefab);
		PrefabUtility.InstantiatePrefab(prefab);
		//~ (temp as GameObject).transform.parent=parent;
    }
}
