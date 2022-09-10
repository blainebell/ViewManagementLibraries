#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ShoppingProduct))]
public class ShoppingProductEditor : Editor {
	public SerializedProperty prefabForClick;
	public SerializedProperty productName;
	public SerializedProperty description;
	public SerializedProperty price;
	public SerializedProperty priceSuffix;
	public SerializedProperty productImage;
	public SerializedProperty rotatePrefabToLocalCoordinateSystem;

	void OnEnable()
	{
		try { // Why does OnEnable get called before serializedObject is populated to the object?
			prefabForClick = serializedObject.FindProperty ("prefabForClick");
			productName = serializedObject.FindProperty ("productName");
			description = serializedObject.FindProperty ("description");
			price = serializedObject.FindProperty ("price");
			priceSuffix = serializedObject.FindProperty ("priceSuffix");
			productImage = serializedObject.FindProperty ("productImage");
			rotatePrefabToLocalCoordinateSystem = serializedObject.FindProperty ("rotatePrefabToLocalCoordinateSystem");
		} catch (Exception ex){
			ex.GetBaseException (); // just to not get a warning on not using variable ex
			//Debug.Log ("Exception in OnEnable ex=" + ex);
		}

	}
	// Update is called once per frame
	public override void OnInspectorGUI(){

		ShoppingProduct sp = (ShoppingProduct)target;

		EditorGUILayout.PropertyField (prefabForClick);
		EditorGUILayout.PropertyField (productName);
		EditorGUILayout.PropertyField (description);
		EditorGUILayout.PropertyField (price);
		EditorGUILayout.PropertyField (priceSuffix);
		EditorGUILayout.PropertyField (productImage);
		EditorGUILayout.PropertyField (rotatePrefabToLocalCoordinateSystem);

		string[] options = { "From Start", "No Rotation", "At Destination" }; // 0 - from start (default), 1 - no rotation, 2 - after endpoint
		sp.rotationAfterSelectedBehavior = EditorGUILayout.Popup ("Rotation:", sp.rotationAfterSelectedBehavior, options);

		serializedObject.ApplyModifiedProperties ();

	}
}

#endif
