#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(VMUIObject))]
[CanEditMultipleObjects]
public class VMUIObjectEditor : Editor {
	public SerializedProperty blockViewManagerSpace;
	public SerializedProperty manualBlock;
	void OnEnable()
	{
		try { // Why does OnEnable get called before serializedObject is populated to the object?
			blockViewManagerSpace = serializedObject.FindProperty ("blockViewManagerSpace");
			manualBlock = serializedObject.FindProperty ("manualBlock");
		} catch (Exception ex){
			ex.GetBaseException (); // just to not get a warning on not using variable ex
			//Debug.Log ("Exception in OnEnable ex=" + ex);
		}

	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField (blockViewManagerSpace);
		EditorGUILayout.PropertyField (manualBlock);
		if (GUILayout.Button ("Calculate Placement", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
			VMUIObject vmo = target as VMUIObject;
			vmo.calculateRect ();
		}
		serializedObject.ApplyModifiedProperties ();
	}
}

#endif
