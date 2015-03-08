using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace SDTK{
	
	public class ShelfTools {
		
		public static string WarpText(string name){
			string rt=name;
			rt.Replace("\\n","\n");
			
			string[] seg=rt.Split(' ');
		
			if(seg.Length==1)
				return name;
			
			int sum=0;
			
			for(int sCount=0; sCount<seg.Length; sCount++){
				if(seg[sCount].Contains("\n")){
					sum=0;
					continue;
				}
				
				sum+=seg[sCount].Length;
				if(sum>10 && sCount!=0){
					sum=0;
					seg[sCount-1]+="\n";
				}
			}
			
			rt=String.Join(" ",seg);
			
			return rt;
		}
		
		public static List<string> Analysis(string path){
			List<string> menuItem=new List<string>();
			string keyword;
			string keyword2="";
			
			if(path.Contains(".js")){
				keyword="@MenuItem(\"";
			}
			else if(path.Contains(".cs")){
				keyword="[MenuItem(\"";
				keyword2="[UnityEditor.MenuItem(\"";
			}
			else 
				return null;
			
			FileInfo file=new FileInfo(Application.dataPath+"/../"+path);
			StreamReader reader=file.OpenText ();
			
			string txt=reader.ReadLine();
			string itemPath="";
			
			while(txt!=null){
				bool isMatch=false;
				
				if(txt.Contains(keyword)){
					isMatch=true;
				}
				else if(keyword2!="" && txt.Contains(keyword2)){
					isMatch=true;
				}
				
				if(isMatch){
					itemPath=txt;
					
					if(itemPath.Contains("//"))
						itemPath=itemPath.Substring(0,itemPath.IndexOf("//"));
					
					itemPath=itemPath.Substring(itemPath.IndexOf("\"")+1);
					itemPath=itemPath.Substring(0,itemPath.LastIndexOf("\""));
					string[] sg=itemPath.Split(' ');
					
					if(sg[sg.Length-1].Length>1){
						char c=sg[sg.Length-1][0];
						if(c=='&' || c=='%' || c=='_' || c=='#')
							sg[sg.Length-1]="";
					}
					
					itemPath=String.Join(" ",sg);
					itemPath=itemPath.Trim("\t  ".ToCharArray());
					
					menuItem.Add(itemPath);
				}
				txt=reader.ReadLine();
			}
			
			reader.Close();
			
			return menuItem;
		}	
	}

}
