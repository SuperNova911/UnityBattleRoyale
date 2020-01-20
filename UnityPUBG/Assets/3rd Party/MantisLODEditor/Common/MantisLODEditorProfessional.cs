/*--------------------------------------------------------
  MantisLODEditorProfessional.cs

  Created by MINGFEN WANG on 13-12-26.
  Copyright (c) 2013 MINGFEN WANG. All rights reserved.
  http://www.mesh-online.net/
--------------------------------------------------------*/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class MantisLODEditorProfessional: MonoBehaviour {
	#if UNITY_EDITOR
	[MenuItem("Window/Mantis LOD Editor/Component/Editor/Mantis LOD Editor Professional")]
	public static void AddComponent() {
		GameObject SelectedObject = Selection.activeGameObject;
		if (SelectedObject) {
			// Register root object for undo.
			Undo.RegisterCreatedObjectUndo(SelectedObject.AddComponent(typeof(MantisLODEditorProfessional)), "Add Mantis LOD Editor Professional");
		}
	}
	[MenuItem ("Window/Mantis LOD Editor/Component/Editor/Mantis LOD Editor Professional", true)]
	static bool ValidateAddComponent () {
		// Return false if no gameobject is selected.
		return Selection.activeGameObject != null;
	}
	#endif
}
