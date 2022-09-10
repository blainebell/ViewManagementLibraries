#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(VMDimension))]
[CanEditMultipleObjects]
public class VMDimensionEditor : Editor {
	public static VMDimensionEditor currentEditor = null;
	public SerializedProperty level;
	public SerializedProperty children;
	public SerializedProperty overrideShowLevelsAbove;
	public SerializedProperty numberOfLevelsAboveToShow;
	public SerializedProperty allTurnOnWhenZoomedInto;
	public SerializedProperty allTurnTransparentWhenZoomedInto;
    public SerializedProperty allMenuItemLinks;
	public SerializedProperty addToBBX;
	public SerializedProperty nextWhenZoomedTo;
    public SerializedProperty isTopForNavigation;
    public SerializedProperty topY;
    public SerializedProperty allVMObjectsToTurnOnForNavigation;
    public SerializedProperty targetScaleIsSet;
    public SerializedProperty targetScale;
    public SerializedProperty targetP;

    void OnDisable(){
		if (currentEditor == this)
			currentEditor = null;
	}
	void OnEnable()
	{
		currentEditor = this;
		try { // Why does OnEnable get called before serializedObject is populated to the object?
			level = serializedObject.FindProperty ("m_level");
			children = serializedObject.FindProperty ("children");
			overrideShowLevelsAbove = serializedObject.FindProperty ("m_overrideShowLevelsAbove");
			numberOfLevelsAboveToShow = serializedObject.FindProperty ("m_numberOfLevelsAboveToShow");
			allTurnOnWhenZoomedInto = serializedObject.FindProperty ("allTurnOnWhenZoomedInto");
			allTurnTransparentWhenZoomedInto = serializedObject.FindProperty ("allTurnTransparentWhenZoomedInto");
			allMenuItemLinks = serializedObject.FindProperty("allMenuItemLinks");
			addToBBX = serializedObject.FindProperty("addToBBX");
			nextWhenZoomedTo = serializedObject.FindProperty("nextWhenZoomedTo");
			isTopForNavigation = serializedObject.FindProperty("m_isTopForNavigation");
            topY = serializedObject.FindProperty("topY");
			allVMObjectsToTurnOnForNavigation = serializedObject.FindProperty("allVMObjectsToTurnOnForNavigation");
            targetScaleIsSet = serializedObject.FindProperty("targetScaleIsSet");
            targetScale = serializedObject.FindProperty("targetScale");
            targetP = serializedObject.FindProperty("target");
        } catch (Exception ex){
			ex.GetBaseException (); // just to not get a warning on not using variable ex
			//Debug.Log ("Exception in OnEnable ex=" + ex);
		}

	}
	public override void OnInspectorGUI()
	{
		VMDimension dimins = (target as VMDimension);
		serializedObject.Update();

		EditorGUILayout.PropertyField (level);
		if (dimins.level != level.intValue){
			dimins.level = level.intValue;
		}

		EditorGUILayout.PropertyField (children, true);
		EditorGUILayout.PropertyField (overrideShowLevelsAbove);
		if (dimins.overrideShowLevelsAbove != overrideShowLevelsAbove.boolValue){
			dimins.overrideShowLevelsAbove = overrideShowLevelsAbove.boolValue;
		}
		GUI.enabled = dimins.overrideShowLevelsAbove;
		EditorGUILayout.PropertyField (numberOfLevelsAboveToShow);
		if (dimins.numberOfLevelsAboveToShow != numberOfLevelsAboveToShow.intValue){
			dimins.numberOfLevelsAboveToShow = numberOfLevelsAboveToShow.intValue;
		}
		GUI.enabled = true;

		EditorGUILayout.PropertyField(allTurnOnWhenZoomedInto, true);

		EditorGUILayout.PropertyField(allTurnTransparentWhenZoomedInto, true);
		EditorGUILayout.PropertyField(allMenuItemLinks, true);
		EditorGUILayout.PropertyField(addToBBX, true);

        GUILayout.BeginHorizontal();
        GUILayout.Space(15.0f);
        GUILayout.Label("Next When Zoomed To:", GUILayout.Width(150));
        EditorGUILayout.PropertyField(nextWhenZoomedTo, GUIContent.none, true, GUILayout.MinWidth(100));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

		GUI.enabled = Application.isPlaying;
		GUILayout.BeginHorizontal (new GUILayoutOption[] { });
		GUILayout.Label ("Set Traverse:", GUILayout.Width (90));
		if (GUILayout.Button ("To Only", new GUILayoutOption [] { GUILayout.Width (80.0f) })) {
			dimins.setTraverseToOnly ();
            ViewManagerScript.invalidate();
		}
		if (GUILayout.Button ("Add to", new GUILayoutOption [] { GUILayout.Width (80.0f) })) {
			dimins.addToTraverse ();
            ViewManagerScript.invalidate();
        }
        if (GUILayout.Button ("Remove", new GUILayoutOption [] { GUILayout.Width (80.0f) })) {
            // only when playing, remove this dimension
            dimins.removeFromPlaying();
        }
        GUILayout.EndHorizontal ();
        GUI.enabled = !Application.isPlaying;
        // 
        GUILayout.BeginHorizontal();
        GUILayout.Space(15.0f);
        GUILayout.Label("On Startup:");
        dimins.traverseOnStartupMode = EditorGUILayout.Popup(dimins.traverseOnStartupMode, new string[] { "  ", "To Only", "Add to" }, new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUI.enabled = true;
/*		if (GUILayout.Button ("Calculate Placement", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
			VMUIObject vmo = target as VMUIObject;
			vmo.calculateRect ();
		}*/


		GUILayout.BeginHorizontal();
		GUILayout.Space(30.0f);
		GUILayout.Label("Set All To:", new GUILayoutOption[] { GUILayout.MaxWidth(85) });
		if (GUILayout.Button("Flyover", new GUILayoutOption[] { GUILayout.Width(80.0f) }))
		{
			DPUtils.setToFlyoverLayoutFor (dimins);
			DPManagerScript.Call_i ("Set-To-All-New-Layouts-For-CoordinateSystem-map", dimins.GetVMDimensionID ());
		}
		if (GUILayout.Button("Still", new GUILayoutOption[] { GUILayout.Width(80.0f) }))
		{
			DPUtils.setToStillLayoutFor (dimins);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(30.0f);
		GUILayout.Label("", new GUILayoutOption[] { GUILayout.MaxWidth(85) });
		if (GUILayout.Button("Continuous", new GUILayoutOption[] { GUILayout.Width(80.0f) }))
		{
			DPUtils.TurnToContinuousLayout(dimins);
		}
		if (GUILayout.Button("Off/Manual", new GUILayoutOption[] { GUILayout.Width(80.0f) }))
		{
			DPUtils.TurnToStillLayoutAndOff (true, dimins);
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Set to Debug", new GUILayoutOption[] { GUILayout.Width(80.0f) })){
			DPManagerScript.Call_s_s_s_s_i ("SetDataValue-map", "UnityMain", "debug-coord-system", "uid", "main-uid", dimins.GetVMDimensionID ());
		}

		EditorGUILayout.PropertyField (isTopForNavigation, true);
        EditorGUILayout.PropertyField(topY, true);

        EditorGUILayout.PropertyField(allVMObjectsToTurnOnForNavigation, true);

        EditorGUILayout.PropertyField(targetP, true);
        EditorGUILayout.PropertyField(targetScaleIsSet, true);
        GUI.enabled = targetScaleIsSet.boolValue;
        EditorGUILayout.PropertyField(targetScale, true);
        GUI.enabled = true;
        serializedObject.ApplyModifiedProperties ();
	}
}

#endif