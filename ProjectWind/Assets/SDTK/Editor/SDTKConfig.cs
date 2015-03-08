/*
	�༭������
	last change: 2012-9-22
*/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SDTK{

	public sealed class SDTKConfig {
		public static readonly string  dataPath=Application.dataPath+"/SDTK/_Data/";//Ĭ������·��
		
		private static void CheckDirectory(){//���·��
			DataRW.CheckDirectory(dataPath);
		}
		
		public static void CheckDirectory(string path){
			DataRW.CheckDirectory(dataPath+path);
		}
		
		public static bool IsDataExists(string cfgName){//��������Ƿ����
			CheckDirectory();
			return DataRW.IsDataExists(dataPath+cfgName);
		}
		
		public static void SaveData<T>(T data, string cfgName){//��������
			CheckDirectory();
			DataRW.SetClassToXML<T>(data,dataPath+cfgName);
		}
		public static T LoadData<T>(string cfgName){//��ȡ����
			CheckDirectory();
			return DataRW.GetClassFromXML<T>(dataPath+cfgName);
		}
		
		public static List<string> GetCfgList(string path, string ext){//��ȡ�����б�
			List<string> rt=new List<string>();
			DataRW.CheckDirectory(dataPath+path);
			DirectoryInfo directory=new DirectoryInfo(dataPath+path);
			FileInfo[] file=directory.GetFiles();
			
			foreach(FileInfo f in file){
				if(f.Extension==ext)
					rt.Add(f.Name.Substring(0,f.Name.Length-ext.Length));
			}
			
			return rt;
		}
		
	}
}