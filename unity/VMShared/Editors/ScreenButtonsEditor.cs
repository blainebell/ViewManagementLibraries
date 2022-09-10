
#define DEBUG_VM
#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ScreenButtonsScript))]
[CanEditMultipleObjects]
public class ScreenButtonsEditor : Editor {
	public SerializedProperty topLevelName;
	public SerializedProperty topLevelObjectName;
	//public SerializedProperty topLevelChildrenTag;
	public SerializedProperty rotateWhenMovedToTopLevel;
	public SerializedProperty orderToIndentMenuItemMax;
	public SerializedProperty allRects;
	public SerializedProperty itemCanvas;
	public SerializedProperty itemCanvasObjectPanel;
	public SerializedProperty currentProductAnimation;
	public SerializedProperty shoppingCart;
	public SerializedProperty checkoutScript;
	public SerializedProperty m_otherObjectsLabeledMode;

	public SerializedProperty closestFromVector;
	public SerializedProperty closestVMObject;
	public SerializedProperty closestVMConnector;

	void OnEnable()
	{
		try { // Why does OnEnable get called before serializedObject is populated to the object?
			topLevelName = serializedObject.FindProperty ("topLevelName");
			topLevelObjectName = serializedObject.FindProperty ("topLevelObjectName");
			//topLevelChildrenTag = serializedObject.FindProperty ("topLevelChildrenTag");
			rotateWhenMovedToTopLevel = serializedObject.FindProperty ("rotateWhenMovedToTopLevel");
			orderToIndentMenuItemMax = serializedObject.FindProperty ("orderToIndentMenuItemMax");
			allRects = serializedObject.FindProperty ("allRects");
			itemCanvas = serializedObject.FindProperty ("itemCanvas");
			itemCanvasObjectPanel = serializedObject.FindProperty ("itemCanvasObjectPanel");
			currentProductAnimation = serializedObject.FindProperty ("currentProductAnimation");
			shoppingCart = serializedObject.FindProperty ("shoppingCart");
			checkoutScript = serializedObject.FindProperty ("checkoutScript");
			m_otherObjectsLabeledMode = serializedObject.FindProperty ("m_otherObjectsLabeledMode");
			closestFromVector = serializedObject.FindProperty ("closestFromVector");
			closestVMObject = serializedObject.FindProperty ("closestVMObject");
			closestVMConnector = serializedObject.FindProperty ("closestVMConnector");
		} catch (Exception ex){
			ex.GetBaseException (); // just to not get a warning on not using variable ex
			//Debug.Log ("Exception in OnEnable ex=" + ex);
		}

	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		ScreenButtonsScript sbs = target as ScreenButtonsScript;
		EditorGUILayout.PropertyField (topLevelName);
		EditorGUILayout.PropertyField (topLevelObjectName);
		//EditorGUILayout.PropertyField (topLevelChildrenTag);
		EditorGUILayout.PropertyField (rotateWhenMovedToTopLevel);
		EditorGUILayout.PropertyField (orderToIndentMenuItemMax);
		EditorGUILayout.PropertyField (allRects);
		EditorGUILayout.PropertyField (itemCanvas);
		EditorGUILayout.PropertyField (itemCanvasObjectPanel);
		EditorGUILayout.PropertyField (currentProductAnimation);
		EditorGUILayout.PropertyField (shoppingCart);
		EditorGUILayout.PropertyField (checkoutScript);
		EditorGUILayout.PropertyField (m_otherObjectsLabeledMode);
		if (sbs.otherObjectsLabeledMode != m_otherObjectsLabeledMode.intValue) {
			sbs.otherObjectsLabeledMode = m_otherObjectsLabeledMode.intValue;
		}

		int arrsize = Math.Min(closestFromVector.arraySize, Math.Min(closestVMObject.arraySize, closestVMConnector.arraySize));

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("# From Vectors:");
		int narrsize = EditorGUILayout.IntField(arrsize, new GUILayoutOption[0]);

		if (narrsize != arrsize){
			Vector3 [] oldClosestFromVector = sbs.closestFromVector;
			VMObject [] oldClosestVMObject = sbs.closestVMObject;
			VMConnector [] oldClosestVMConnector = sbs.closestVMConnector;
			sbs.closestFromVector = new Vector3[narrsize];
			sbs.closestVMObject = new VMObject[narrsize];
			sbs.closestVMConnector = new VMConnector[narrsize];
			int cparrsize = (narrsize > arrsize) ? arrsize : narrsize;
			int i=0;
			for (; i<cparrsize; i++){
				sbs.closestFromVector[i] = oldClosestFromVector[i];
				sbs.closestVMObject[i] = oldClosestVMObject[i];
				sbs.closestVMConnector[i] = oldClosestVMConnector[i];
			}
			if (narrsize > arrsize){
				while (i < narrsize){
					sbs.closestFromVector[i] = new Vector3();
					sbs.closestVMObject[i] = new VMObject();
					sbs.closestVMConnector[i] = new VMConnector();
					i++;
				}
			}
			sbs.changed();
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Closest From");
		GUILayout.Label ("Closest VMObject");
		GUILayout.EndHorizontal();
		for (int ri = 0; ri < arrsize; ri++){
			GUILayout.BeginHorizontal();
			GUILayout.Label (ri.ToString ());
			Vector3 prevClosestVect = sbs.closestFromVector [ri];
			Vector3 newClosestVect = EditorGUILayout.Vector3Field ("", prevClosestVect, new GUILayoutOption[0]);
			if (!newClosestVect.Equals (prevClosestVect)) {
				sbs.closestFromVector [ri] = newClosestVect;
				sbs.changed ();
			}
			GUILayout.Label (":::");
			{
				VMObject prevGameObj = sbs.closestVMObject [ri];
				VMObject newGameObj = (VMObject)EditorGUILayout.ObjectField (prevGameObj, typeof(VMObject), 
                    true /* allowSceneObjects, do we need this true? */, new GUILayoutOption[0]);
				if (newGameObj!=null && prevGameObj!=null && !newGameObj.Equals (prevGameObj)) {
					sbs.closestVMObject [ri] = newGameObj;
					sbs.changed ();
				}
			}
			GUILayout.Label (":::");
			{
				VMConnector prevGameObj = null;
				if (sbs.closestVMConnector.Length > ri) {
					prevGameObj = sbs.closestVMConnector [ri];
				}
				VMConnector newGameObj = (VMConnector)EditorGUILayout.ObjectField (prevGameObj, typeof(VMConnector),
                    true /* allowSceneObjects, do we need this true? */, new GUILayoutOption[0]);
				if (newGameObj!=null && !newGameObj.Equals (prevGameObj)) {
					if (sbs.closestVMConnector.Length != arrsize) {
						sbs.closestVMConnector = new VMConnector[arrsize];
					}
					sbs.closestVMConnector [ri] = newGameObj;
					sbs.changed ();
				}
			}

			GUILayout.EndHorizontal();
		}


		serializedObject.ApplyModifiedProperties ();
	}
}

#endif
