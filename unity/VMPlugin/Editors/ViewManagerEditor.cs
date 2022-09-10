#define DEBUG_VM
#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ViewManager))]
public class ViewManagerEditor : Editor {
	public SerializedProperty screenCanvas;
	public SerializedProperty currentCamera;
	public SerializedProperty topWorldObject;
	public SerializedProperty tryToLabelAll;
	public SerializedProperty showEmptySpace;
	public SerializedProperty showBSPPartitionPlanes;
	public SerializedProperty showProjections;
	public SerializedProperty clipLinesForProjections;
	public SerializedProperty debugTraversal;
	public SerializedProperty highlightObject;
	public SerializedProperty highlightedObjectString;
	public SerializedProperty highlightPartNumber;
	public SerializedProperty highlightPart;

	public SerializedProperty highlightOrderedPart;
	//public SerializedProperty highlightPartOrder;

	public int highlightedObject = -1;

	public SerializedProperty highlightColor;
	public SerializedProperty saveHighlightColorToDisk;
	void OnEnable()
	{
		try { // Why does OnEnable get called before serializedObject is populated to the object?
			screenCanvas = serializedObject.FindProperty ("screenCanvas");
			currentCamera = serializedObject.FindProperty ("currentCamera");
			topWorldObject = serializedObject.FindProperty ("topWorldObject");
			tryToLabelAll = serializedObject.FindProperty ("m_tryToLabelAll");
			showEmptySpace = serializedObject.FindProperty ("m_showEmptySpace");
			showBSPPartitionPlanes = serializedObject.FindProperty ("m_showBSPPartitionPlanes");
			showProjections = serializedObject.FindProperty ("m_showProjections");
			clipLinesForProjections = serializedObject.FindProperty ("m_clipLinesForProjections");
			debugTraversal = serializedObject.FindProperty ("debugTraversal");
			highlightObject = serializedObject.FindProperty ("highlightObject");
			highlightedObjectString = serializedObject.FindProperty ("highlightedObject");

			highlightPartNumber = serializedObject.FindProperty ("highlightPartNumber");
			highlightPart = serializedObject.FindProperty ("highlightPart");

			highlightOrderedPart = serializedObject.FindProperty ("highlightOrderedPart");
			highlightColor = serializedObject.FindProperty ("highlightColor");
			saveHighlightColorToDisk = serializedObject.FindProperty ("saveHighlightColorToDisk");

		} catch (Exception ex){
			ex.GetBaseException (); // just to not get a warning on not using variable ex
			//Debug.Log ("Exception in OnEnable ex=" + ex);
		}

	}
	public bool first = true;
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(screenCanvas);
		EditorGUILayout.PropertyField (currentCamera);
		EditorGUILayout.PropertyField (topWorldObject);
		if (GUILayout.Button ("Please see View Manager Window for configurations", new GUILayoutOption [] {  })) {
			ViewManagerScript.ShowWindow ();
		}
#if NEVER
		ViewManager vmins = (target as ViewManager);
		EditorGUILayout.PropertyField (tryToLabelAll);
		if (tryToLabelAll.boolValue != vmins.tryToLabelAll) {
			vmins.tryToLabelAll = tryToLabelAll.boolValue;
		}
		EditorGUILayout.PropertyField(showEmptySpace);
		if (showEmptySpace.boolValue != vmins.showEmptySpace) {
			vmins.showEmptySpace = showEmptySpace.boolValue;
		}
		EditorGUILayout.PropertyField(showBSPPartitionPlanes);
		if (showBSPPartitionPlanes.boolValue != vmins.showBSPPartitionPlanes) {
			vmins.showBSPPartitionPlanes = showBSPPartitionPlanes.boolValue;
		}
		EditorGUILayout.PropertyField(showProjections);
		if (showProjections.boolValue != vmins.showProjections) {
			vmins.showProjections = showProjections.boolValue;
		}


		EditorGUILayout.PropertyField(clipLinesForProjections);
		if (clipLinesForProjections.boolValue != vmins.clipLinesForProjections) {
			vmins.clipLinesForProjections = clipLinesForProjections.boolValue;
			//DPManagerScript.Call_s_s_s_s_b ("SetDataValue-map", "UnityMain", "clip-lines-of-faces", "uid", "main-uid", clipLinesForProjections.boolValue);
		}
#endif

#if DEBUG_VM
		ViewManager vmins = (target as ViewManager);
		EditorGUILayout.PropertyField(debugTraversal);

		GUI.enabled = true;
		bool prevhighlightOrderedPart = highlightOrderedPart.boolValue;
		EditorGUILayout.PropertyField(highlightOrderedPart);
		bool highlightOrderedPartChanged = prevhighlightOrderedPart != highlightOrderedPart.boolValue;
		int ntotalparts = DPManagerScript.Call_ri_noargs ("Get-Number-Of-Total-BSPObjectParts-For-Debug-Coord-System-map");
		if (ntotalparts <= 0) {
			GUI.enabled = false;
			ntotalparts = 0;
		}
		int prevHighlightPartOrder = vmins.highlightPartOrder;
		vmins.highlightPartOrder = EditorGUILayout.IntSlider (vmins.highlightPartOrder, 1, ntotalparts, new GUILayoutOption[0]);
		int objPartID = -1;
		int partID = -1;
		bool objPartIDSet = false;
		if (highlightOrderedPartChanged || vmins.highlightPartOrder != prevHighlightPartOrder) {
			if (highlightOrderedPart.boolValue) {
				Camera curCamera = ViewManager.getCurrentCamera();
				Vector3 pos = curCamera.transform.position;
				Vector3 forward = curCamera.transform.forward;
				float[] posa = { pos.x, pos.y, pos.z };
				float[] forwarda = { forward.x, forward.y, forward.z };
				//int[] viewRect = { 0, 0, curCamera.pixelWidth, curCamera.pixelHeight };
				//float [] viewSize = { curCamera.pixelWidth, curCamera.pixelHeight };
				Matrix4x4 worldToCam = curCamera.projectionMatrix * curCamera.worldToCameraMatrix;
				float[] worldToCamArr = { worldToCam.m00, worldToCam.m01, worldToCam.m02, worldToCam.m03,
					worldToCam.m10, worldToCam.m11, worldToCam.m12, worldToCam.m13,
					worldToCam.m20, worldToCam.m21, worldToCam.m22, worldToCam.m23,
					worldToCam.m30, worldToCam.m31, worldToCam.m32, worldToCam.m33
				};
				objPartID = DPManagerScript.Call_ri_p3_p3_t3f_i ("Get-BSPObjectPart-Order-Visual-map", posa, forwarda, worldToCamArr, vmins.highlightPartOrder-1);
//				Debug.Log ("objPartID=" + objPartID);
				if (vmins.highlightPart) {
					partID = DPManagerScript.Call_ri_noargs("GetDataInTableFieldSortedByWithKey-map\tUnityMain\tlast-part-number\tuid\tmain-uid");
					vmins.highlightPartNumber = partID;
				}
				objPartIDSet = true; // objPartID!=0;
			}
		}


		GUI.enabled = false;
		if (!highlightObject.boolValue) {
			highlightedObjectString.stringValue = "";
		}
		EditorGUILayout.PropertyField(highlightedObjectString);

		GUI.enabled = true;
		EditorGUILayout.PropertyField(highlightObject);

		string[] options = new string[]{};
		bool highlightedObjectTurnedOff = false;
		if (!highlightObject.boolValue) {
			GUI.enabled = false;
			if (highlightedObject != -1) {
				DPManagerScript.Call_noargs ("Unset-Highlighted-For-Debugging-map");
				highlightedObject = -1;
				highlightedObjectTurnedOff = vmins.highlightPart;
				vmins.highlightPart = false;
				highlightPart.boolValue = false;
			}
		} else {
			// query DP for all UnityObject.id
			options = DPManagerScript.Call_rsa_wrapper("Get-All-UnityObject-IDs-map");
		}
		if (objPartIDSet) {
			if (objPartID == 0) {
				if (highlightedObject != -1) {
					DPManagerScript.Call_noargs ("Unset-Highlighted-For-Debugging-map");
					highlightedObject = -1;
					highlightedObjectTurnedOff = vmins.highlightPart;
					//vmins.highlightPart = false;
					//highlightPart.boolValue = false;
				}
			} else {
				string objstr = objPartID.ToString ();
				int pl = 0;
				foreach (string opt in options) {
					if (opt == objstr) {
						highlightedObject = pl;
//						Debug.Log ("set highlightedObject=" + pl + " objstr=" + objstr);
					}
					pl++;
				}
			}
		}

		int highObj = EditorGUILayout.Popup("Highlighted Object ID:", highlightedObject, options);
		bool prevhighlightPart = highlightPart.boolValue;
		EditorGUILayout.PropertyField (highlightPart);
		bool highlightPartToggleChanged = highlightPart.boolValue != prevhighlightPart;
		bool highlightObjChanged = highlightedObject != highObj || objPartIDSet;
		if (highlightObject.boolValue && highObj >= 0) {
			highlightedObject = highObj;
			if (highlightedObject < options.Length) {
				if (options[highlightedObject] != vmins.highlightedObject) {
					vmins.highlightedObject = options [highlightedObject];
					int res = -1;
					if (!vmins.highlightPart && int.TryParse (vmins.highlightedObject, out res)) {
						DPManagerScript.Call_i ("Set-Highlighted-For-Debugging-map", res);
					}
				}
			} else {
				if (highlightedObject != -1) {
					DPManagerScript.Call_noargs ("Unset-Highlighted-For-Debugging-map");
					if (!vmins.highlightPart)
						highlightedObject = -1;
				}
			}
		}
		if (highlightPartToggleChanged && highlightPart.boolValue) {
			DPManagerScript.Call_noargs ("Unset-Highlighted-For-Debugging-map");
		}
		int nparts = 0;
		if (vmins.highlightObject && vmins.highlightPart){
			int res;
			if (int.TryParse (vmins.highlightedObject, out res)) {
				nparts = DPManagerScript.Call_ri_i ("Get-Number-Of-BSPObjectParts-map", res);
			}
		}
		List<string> pstr = new List<string>();
		for (int i = 0; i < nparts; i++) {
			pstr.Add (i.ToString ());
		}
		if (nparts>0 && (vmins.highlightPartNumber < 0 || vmins.highlightPartNumber >= nparts))
			vmins.highlightPartNumber = 0;
		int prevHighlightPartNumber = vmins.highlightPartNumber;
		vmins.highlightPartNumber = EditorGUILayout.Popup("Highlighted Part#:", vmins.highlightPartNumber, pstr.ToArray());
		if ((highlightObjChanged && vmins.highlightPart) || highlightPartToggleChanged || prevHighlightPartNumber != vmins.highlightPartNumber) {
			// change or remove
			int res;
			bool parse_successful = int.TryParse (vmins.highlightedObject, out res);
			if (parse_successful && highlightPart.boolValue) {
				DPManagerScript.Call_i_i ("Create-BSPObjectPart-Visual-map", res, vmins.highlightPartNumber);
			} else {
				DPManagerScript.Call_noargs ("Remove-BSPObjectPart-Visual-map");
			}
		} else if (highlightedObjectTurnedOff) {
			DPManagerScript.Call_noargs ("Remove-BSPObjectPart-Visual-map");
		}
		GUI.enabled = true;
		EditorGUILayout.PropertyField(highlightColor);

		EditorGUILayout.PropertyField(saveHighlightColorToDisk);

		serializedObject.ApplyModifiedProperties ();
		first = false;
#endif
	}
}
#endif
