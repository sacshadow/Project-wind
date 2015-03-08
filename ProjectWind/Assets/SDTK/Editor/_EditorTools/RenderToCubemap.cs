using UnityEngine;
using UnityEditor;
using System.Collections;

//from unity3d script reference

// Render scene from a given point into a static cube map.
// Place this script in Editor folder of your project.
// Then use the cubemap with one of Reflective shaders!
public class RenderToCubemap : ScriptableWizard {
	public Transform renderFromPosition;
	public Cubemap cubemap;

	void OnWizardUpdate () {
		helpString = "Select transform to render from and cubemap to render into";
		isValid = (renderFromPosition != null) && (cubemap != null);
	}

	void OnWizardCreate () {
		// create temporary camera for rendering
		GameObject go = new GameObject( "CubemapCamera", typeof(Camera) );
		// place it on the object
		go.transform.position = renderFromPosition.position;
		go.transform.rotation = Quaternion.identity;

		// render into cubemap 
		go.camera.RenderToCubemap( cubemap );

		// destroy temporary camera
		DestroyImmediate( go );
	}

	[MenuItem("SDTK/Render To Cubemap")]
	public static void RenderCubemap () {
		ScriptableWizard.DisplayWizard<RenderToCubemap>("Render cubemap", "Render!");
	}
}