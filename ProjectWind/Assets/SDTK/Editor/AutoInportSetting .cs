/*
	修改导入设定
	change settings when import
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

//AssetPostprocessor提供对导入物体属性的自定义修改
//根据素材被放入的文件路径,提供,配置不同的属性
public class AutoImportSetting : AssetPostprocessor {
	
	//修改图片属性
	//change Texture format
	private void SetDefaultTextureSetting(TextureImporter textureImporter){
		SetDefaultTextureSetting(textureImporter,TextureImporterFormat.RGBA32);
	}		
	private void SetDefaultTextureSetting(TextureImporter textureImporter,TextureImporterFormat format){
		textureImporter.maxTextureSize=2048;
		textureImporter.npotScale=TextureImporterNPOTScale.None;
		textureImporter.mipmapEnabled=false;
		textureImporter.textureFormat=format;
	}
	
	//设置图片
	void OnPreprocessTexture (){
		TextureImporter textureImporter=assetImporter as TextureImporter;
		
		if(assetPath.Contains("/GUI/"))
			SetDefaultTextureSetting(textureImporter);
		else if(assetPath.Contains("/Normal/")){
			textureImporter.normalmap=true;
			SetDefaultTextureSetting(textureImporter);
		}
		else if(assetPath.Contains("/Sprite Atlases/"))
			SetDefaultTextureSetting(textureImporter, TextureImporterFormat.DXT5);
	}
	
	//设置模型
	void OnPreprocessModel(){
		ModelImporter modelImporter=assetImporter as ModelImporter;
		
		if(assetPath.Contains("/AutoScale/"))
			modelImporter.globalScale=1f;
		
		if(assetPath.Contains("_sd.obj")){
			modelImporter.normalImportMode=ModelImporterTangentSpaceMode.Calculate;
			modelImporter.tangentImportMode=ModelImporterTangentSpaceMode.Calculate;
		}
	}
	
	//设置声音
	void OnPreprocessAudio(){
		AudioImporter audioImporter=assetImporter as AudioImporter; 
		
		if(assetPath.Contains("/2DSound/"))
			audioImporter.threeD=false;
	}
	
	
	
}
