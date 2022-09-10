#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MouseOverScript))]
[CanEditMultipleObjects]
public class MouseOverEditor : Editor {
	public SerializedProperty viewNormal;

	public SerializedProperty closestFromVector;
	public SerializedProperty viewNormalForFromVector;

	public SerializedProperty hasViewAngle;
	public SerializedProperty viewAngle;
	public SerializedProperty zoomInto;
	public SerializedProperty linkViewObject;

	void OnEnable()
	{
		try { // Why does OnEnable get called before serializedObject is populated to the object?
			viewNormal = serializedObject.FindProperty ("viewNormal");
			closestFromVector = serializedObject.FindProperty ("closestFromVector");
			viewNormalForFromVector = serializedObject.FindProperty ("viewNormalForFromVector");
			hasViewAngle = serializedObject.FindProperty ("hasViewAngle");
			viewAngle = serializedObject.FindProperty ("viewAngle");
			zoomInto = serializedObject.FindProperty ("zoomInto");
			linkViewObject = serializedObject.FindProperty ("linkViewObject");
		} catch (Exception ex){
			ex.GetBaseException (); // just to not get a warning on not using variable ex
			//Debug.Log ("Exception in OnEnable ex=" + ex);
		}

	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		MouseOverScript mos = (MouseOverScript)target;
		EditorGUILayout.PropertyField (viewNormal);

		int arrsize = Math.Min(closestFromVector.arraySize, viewNormalForFromVector.arraySize);

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("# From Vectors:");
		int narrsize = EditorGUILayout.IntField(arrsize, new GUILayoutOption[0]);

		if (narrsize != arrsize){
			Vector3 [] oldClosestFromVector = mos.closestFromVector;
			Vector3 [] oldViewNormalForFromVector = mos.viewNormalForFromVector;
			mos.closestFromVector = new Vector3[narrsize];
			mos.viewNormalForFromVector = new Vector3[narrsize];
			int cparrsize = (narrsize > arrsize) ? arrsize : narrsize;
			int i=0;
			for (; i<cparrsize; i++){
				mos.closestFromVector[i] = oldClosestFromVector[i];
				mos.viewNormalForFromVector[i] = oldViewNormalForFromVector[i];
			}
			if (narrsize > arrsize){
				while (i < narrsize){
					mos.closestFromVector[i] = new Vector3();
					mos.viewNormalForFromVector[i] = new Vector3();
					i++;
				}
			}
			mos.changed();
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Closest From");
		GUILayout.Label ("View Normal");
		GUILayout.EndHorizontal();
		for (int ri = 0; ri < arrsize; ri++){
			GUILayout.BeginHorizontal();
			GUILayout.Label (ri.ToString ());
			Vector3 prevClosestVect = mos.closestFromVector [ri];
			Vector3 newClosestVect = EditorGUILayout.Vector3Field ("", prevClosestVect, new GUILayoutOption[0]);
			if (!newClosestVect.Equals (prevClosestVect)) {
				mos.closestFromVector [ri] = newClosestVect;
				mos.changed ();
			}
			GUILayout.Label (":::");
			Vector3 prevViewVect = mos.viewNormalForFromVector [ri];
			Vector3 newViewVect = EditorGUILayout.Vector3Field ("", prevViewVect, new GUILayoutOption[0]);
			if (!newViewVect.Equals (prevViewVect)) {
				mos.viewNormalForFromVector [ri] = newViewVect;
				mos.changed ();
			}
			GUILayout.EndHorizontal();
		}

		//EditorGUILayout.PropertyField (closestFromVector);
		//EditorGUILayout.PropertyField (viewNormalForFromVector);

		EditorGUILayout.PropertyField (hasViewAngle);
		GUI.enabled = hasViewAngle.boolValue;
		EditorGUILayout.PropertyField (viewAngle);
		GUI.enabled = true;
		EditorGUILayout.PropertyField (zoomInto);
        MouseOverScript origLink = mos.linkViewObject;
		mos.linkViewObject = (MouseOverScript) EditorGUILayout.ObjectField (mos.linkViewObject, typeof(MouseOverScript), true, new GUILayoutOption[0]);
        if (origLink != mos.linkViewObject)
        {
            mos.changed();
        }
	}
}

#endif
