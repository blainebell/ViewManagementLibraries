#define DEBUG_VM
#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MouseScript))]
public class MouseScriptEditor : Editor {
    public SerializedProperty controlledTransform;
    public SerializedProperty rotationTime;
	public SerializedProperty isRotating;
	public SerializedProperty turnOnLights;
	public SerializedProperty rotatePoint;
	public SerializedProperty stopRotatingOnLabelClick;
	public SerializedProperty labelsAreClickable;
	void OnEnable()
	{
		try { // Why does OnEnable get called before serializedObject is populated to the object?
            controlledTransform = serializedObject.FindProperty("controlledTransform");
            rotationTime = serializedObject.FindProperty ("rotationTime");
			isRotating = serializedObject.FindProperty ("m_isRotating");
			turnOnLights = serializedObject.FindProperty ("m_turnOnLights");
			rotatePoint = serializedObject.FindProperty ("rotatePoint");
			stopRotatingOnLabelClick = serializedObject.FindProperty ("stopRotatingOnLabelClick");
			labelsAreClickable = serializedObject.FindProperty ("labelsAreClickable");
		} catch (Exception ex){
			ex.GetBaseException (); // just to not get a warning on not using variable ex
			//Debug.Log ("Exception in OnEnable ex=" + ex);
		}

	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		MouseScript ms = target as MouseScript;
        EditorGUILayout.PropertyField(controlledTransform);
        EditorGUILayout.PropertyField (rotationTime);
		EditorGUILayout.PropertyField (isRotating);
		if (ms.isRotating != isRotating.boolValue)
			ms.isRotating = isRotating.boolValue;
		EditorGUILayout.PropertyField (turnOnLights);
		if (ms.turnOnLights != turnOnLights.boolValue)
			ms.turnOnLights = turnOnLights.boolValue;
		EditorGUILayout.PropertyField (rotatePoint);
		EditorGUILayout.PropertyField (stopRotatingOnLabelClick);
		EditorGUILayout.PropertyField (labelsAreClickable);
		serializedObject.ApplyModifiedProperties ();
	}
}

#endif
