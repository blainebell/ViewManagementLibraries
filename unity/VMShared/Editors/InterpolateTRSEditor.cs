#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(InterpolateTRSScript))]
[CanEditMultipleObjects]
public class InterpolateTRSEditor : Editor {
	public SerializedProperty durationT;
	public SerializedProperty durationR;
	public SerializedProperty durationS;

	void OnEnable()
	{
		try { // Why does OnEnable get called before serializedObject is populated to the object?
            durationT = serializedObject.FindProperty ("durationT");
            durationR = serializedObject.FindProperty ("durationR");
            durationS = serializedObject.FindProperty ("durationS");
		} catch (Exception ex){
			ex.GetBaseException (); // just to not get a warning on not using variable ex
			//Debug.Log ("Exception in OnEnable ex=" + ex);
		}

	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		InterpolateTRSScript itrs = (InterpolateTRSScript)target;
        EditorGUILayout.PropertyField(durationT);
        EditorGUILayout.PropertyField(durationR);
        EditorGUILayout.PropertyField(durationS);

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
