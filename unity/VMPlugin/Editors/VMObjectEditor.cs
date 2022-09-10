#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;

[CustomEditor(typeof(VMObject))]
[CanEditMultipleObjects]
public class VMObjectEditor : Editor {
	public static VMObjectEditor currentEditor = null;
	public SerializedProperty coordinateSystem;
	public SerializedProperty tryToLabel;
	public SerializedProperty annotation;
	public SerializedProperty textForLabel;
	public SerializedProperty canMove;
	public SerializedProperty tryInside;
	public SerializedProperty tryOutside;
	public SerializedProperty tryOnCentroid;
	public SerializedProperty outsideLineWidth;
	public SerializedProperty sizeIsDefault;
	public SerializedProperty minHeight;
	public SerializedProperty maxHeight;
	public SerializedProperty minHeightFloat;
	public SerializedProperty maxHeightFloat;
	public SerializedProperty showBBX;
	public SerializedProperty alwaysFacing;
	public SerializedProperty planeOriented;
	public SerializedProperty planeOrientedAxisAligned;
	public SerializedProperty scheduleMode;
	public SerializedProperty scheduleDurationEveryMS;
	public SerializedProperty hysteresisMode;
	public SerializedProperty interpolateBetween;
	public SerializedProperty interpolateBetweenTimeMS;
	public SerializedProperty zMode;
	public SerializedProperty constantZDistance;
	public SerializedProperty insideBuffer;
	public SerializedProperty outsideBuffer;
	public SerializedProperty maxOutsideDistance;
	public SerializedProperty timeToStayVisibleWhenNoSpace;
	public SerializedProperty shouldMoveWhenByPercentage;
	public SerializedProperty moveWhenNotVisiblePercentage;
	public SerializedProperty fadeIn;
	public SerializedProperty fadeOut;
	public SerializedProperty fadeTime;

	/* Behavior */
    public SerializedProperty showWhenNotVisible;
    public SerializedProperty placeWithDirectView;
	public SerializedProperty whenShowInDirectPlacement;

    public SerializedProperty screenStabilized;
    public SerializedProperty notVisibleLayoutMode;
    public SerializedProperty notVisibleArrowLayoutMode;
    public SerializedProperty notVisibleArrowMode;

    void OnDisable(){
		if (currentEditor == this)
			currentEditor = null;
	}
	void OnEnable()
	{
		currentEditor = this;
		try { // Why does OnEnable get called before serializedObject is populated to the object?
			coordinateSystem = serializedObject.FindProperty ("m_coordinateSystem");
			tryToLabel = serializedObject.FindProperty ("m_tryToLabel");
			annotation = serializedObject.FindProperty ("m_annotation");
			textForLabel = serializedObject.FindProperty ("m_textForLabel");
			canMove = serializedObject.FindProperty ("m_canMove");
			sizeIsDefault = serializedObject.FindProperty ("m_sizeIsDefault");
			minHeight = serializedObject.FindProperty ("m_minHeight");
			maxHeight = serializedObject.FindProperty ("m_maxHeight");
			minHeightFloat = serializedObject.FindProperty ("minHeightFloat");
			maxHeightFloat = serializedObject.FindProperty ("maxHeightFloat");
			tryInside = serializedObject.FindProperty ("m_tryToLabelInside");
			tryOutside = serializedObject.FindProperty ("m_tryToLabelOutside");
			tryOnCentroid = serializedObject.FindProperty ("m_tryToLabelOnCentroid");
			outsideLineWidth = serializedObject.FindProperty ("m_outsideLineWidth");
			showBBX = serializedObject.FindProperty ("m_showBBX");
			alwaysFacing = serializedObject.FindProperty ("m_alwaysFacing");
			planeOriented = serializedObject.FindProperty ("m_planeOriented");
			planeOrientedAxisAligned = serializedObject.FindProperty ("m_planeOrientedAxisAligned");
			scheduleMode = serializedObject.FindProperty ("m_scheduleMode");
			scheduleDurationEveryMS = serializedObject.FindProperty ("m_scheduleDurationEveryMS");
			hysteresisMode = serializedObject.FindProperty ("m_hysteresisMode");
			interpolateBetween = serializedObject.FindProperty ("m_interpolateBetween");
			interpolateBetweenTimeMS = serializedObject.FindProperty ("m_interpolateBetweenTimeMS");
			zMode = serializedObject.FindProperty ("m_zMode");
			constantZDistance = serializedObject.FindProperty ("m_constantZDistance");
			insideBuffer = serializedObject.FindProperty ("m_insideBuffer");
			outsideBuffer = serializedObject.FindProperty ("m_outsideBuffer");
			maxOutsideDistance = serializedObject.FindProperty ("m_maxOutsideDistance");
			timeToStayVisibleWhenNoSpace = serializedObject.FindProperty ("m_timeToStayVisibleWhenNoSpace");
			shouldMoveWhenByPercentage = serializedObject.FindProperty ("m_shouldMoveWhenByPercentage");
			moveWhenNotVisiblePercentage = serializedObject.FindProperty ("m_moveWhenNotVisiblePercentage");
			fadeIn = serializedObject.FindProperty ("m_fadeIn");
			fadeOut = serializedObject.FindProperty ("m_fadeOut");
			fadeTime = serializedObject.FindProperty ("m_fadeTime");
            showWhenNotVisible = serializedObject.FindProperty("m_showWhenNotVisible");
            placeWithDirectView = serializedObject.FindProperty("m_placeWithDirectView");
			whenShowInDirectPlacement = serializedObject.FindProperty("m_whenShowInDirectPlacement");
            screenStabilized = serializedObject.FindProperty("m_screenStabilized");
            notVisibleLayoutMode = serializedObject.FindProperty("m_notVisibleLayoutMode");
            notVisibleArrowLayoutMode = serializedObject.FindProperty("m_notVisibleArrowLayoutMode");
            notVisibleArrowMode = serializedObject.FindProperty("m_notVisibleArrowMode");
        } catch (Exception ex){
			ex.GetBaseException (); // just to not get a warning on not using variable ex
			//Debug.Log ("Exception in OnEnable ex=" + ex);
		}

	}
	bool annotationFold = false, geometryFold = true, advancedFold = true, debugFold = true, strategiesFold = true;
	public override void OnInspectorGUI()
	{
		VMObject vmins = (target as VMObject);
		bool hasPrefabMessage = false, hasPrefabWarning = false, hasPrefabInfo = false;
		int nclones = 0;
		if (Application.isPlaying && DPUtils.isPrefab (vmins.gameObject)) {
			ViewManager vm = FindObjectOfType<ViewManager> ();
			if (vm.prefabsControlWithSameTag) {
				GameObject[] gos = GameObject.FindGameObjectsWithTag (vmins.gameObject.tag);
				hasPrefabMessage = true;
				nclones = gos.Length;
				hasPrefabWarning = (nclones == 0);
				foreach (GameObject go in gos) {
					if (DPUtils.isPrefab(go))
						nclones--;
				}
			} else {
				hasPrefabMessage = true;
				hasPrefabInfo = true;  // just tell user controls don't do anything to existing objects
			}
		}
		serializedObject.Update();

		//EditorGUILayout.PropertyField (coordinateSystem);
		vmins.coordinateSystem = (VMDimension) EditorGUILayout.ObjectField ("Coordinate System", vmins.coordinateSystem, typeof(VMDimension), true, new GUILayoutOption[0]); 

		EditorGUILayout.PropertyField (tryToLabel);
		if (tryToLabel.boolValue != vmins.tryToLabel) {
			vmins.tryToLabel = tryToLabel.boolValue;
		}
		textForLabel.stringValue = EditorGUILayout.TextArea( textForLabel.stringValue, GUILayout.MaxHeight(100) );
		if (textForLabel.stringValue != vmins.textForLabel) {
			vmins.textForLabel = textForLabel.stringValue;
		}

		EditorGUILayout.PropertyField (annotation);

		if (advancedFold = EditorGUILayout.Foldout (advancedFold, "Advanced:")) {
		} else {
			return;
		}

		EditorGUILayout.PropertyField (canMove);
		if (canMove.boolValue != vmins.canMove) {
			vmins.canMove = canMove.boolValue;
		}
		GUI.enabled = !vmins.tryToLabelOnCentroid;
		EditorGUILayout.PropertyField (tryInside, new GUIContent ("Inside:"), new GUILayoutOption[] { });
		if (tryInside.boolValue != vmins.tryToLabelInside) {
			vmins.tryToLabelOnCentroid = false;
			tryOnCentroid.boolValue = false;
			vmins.tryToLabelInside = tryInside.boolValue;
		}

		GUILayout.BeginHorizontal (new GUILayoutOption[] {  });
		EditorGUILayout.PropertyField (tryOutside, new GUIContent ("Outside:"), new GUILayoutOption[] { });
		if (tryOutside.boolValue != vmins.tryToLabelOutside) {
			vmins.tryToLabelOnCentroid = false;
			tryOnCentroid.boolValue = false;
			vmins.tryToLabelOutside = tryOutside.boolValue;
		}

		GUI.enabled = !vmins.tryToLabelOnCentroid && vmins.tryToLabelOutside;
		EditorGUILayout.PropertyField (outsideLineWidth, new GUILayoutOption[] { GUILayout.Width (175.0f) });
		if (vmins.outsideLineWidth != outsideLineWidth.floatValue){
			vmins.outsideLineWidth = outsideLineWidth.floatValue;
		}
		GUI.enabled = true;
		GUILayout.EndHorizontal ();

		EditorGUILayout.PropertyField (tryOnCentroid, new GUIContent ("On Centroid:"), new GUILayoutOption[] { });
		if (tryOnCentroid.boolValue != vmins.tryToLabelOnCentroid) {
			if (tryOnCentroid.boolValue) {
				tryOutside.boolValue = false;
				tryInside.boolValue = false;
				vmins.tryToLabelOutside = false;
				vmins.tryToLabelInside = false;
			}
			vmins.tryToLabelOnCentroid = tryOnCentroid.boolValue;
		}
			
		// BEHAVIOR
		{
			GUILayout.BeginHorizontal (new GUILayoutOption[] { });
			string[] boptions = { "<none>", "Direct and screen stabilized", "Screen stabilized only if not visible" };
			//Debug.Log("vmins.behaviorMode=" + vmins.behaviorMode);
			vmins.behaviorMode = EditorGUILayout.Popup ("Behavior Mode:", vmins.behaviorMode, boptions, new GUILayoutOption[] { GUILayout.Width (300) });
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
		}

		EditorGUILayout.PropertyField(showWhenNotVisible, new GUIContent("Show When Not Visible:"), new GUILayoutOption[] { });
        if (showWhenNotVisible.boolValue != vmins.showWhenNotVisible)
            vmins.showWhenNotVisible = showWhenNotVisible.boolValue;

		EditorGUILayout.PropertyField(placeWithDirectView, new GUIContent("Place With Direct View:"), new GUILayoutOption[] { });
        if (placeWithDirectView.boolValue != vmins.placeWithDirectView)
        {
            vmins.placeWithDirectView = placeWithDirectView.boolValue;
        }

		EditorGUILayout.PropertyField(whenShowInDirectPlacement, new GUIContent("When Looking, show Direct View:"), new GUILayoutOption[] { GUILayout.Width(300) });
		if (whenShowInDirectPlacement.boolValue != vmins.whenShowInDirectPlacement)
			vmins.whenShowInDirectPlacement = whenShowInDirectPlacement.boolValue;

		// END BEHAVIOR

		GUI.enabled = false;
		GUILayout.BeginHorizontal(new GUILayoutOption[] { });
		GUILayout.Space(30.0f);
		EditorGUILayout.Toggle ("Has Direct Placement", vmins.directViewPlacement != null);
		EditorGUILayout.Toggle ("Screen Stabilized", vmins.screenStabilized);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal(new GUILayoutOption[] { });
		GUILayout.Space(30.0f);
		EditorGUILayout.Toggle ("Showing Direct Placement", vmins.showingDirectViewPlacementGet());

        string[] soptions = { "Manual", "Continuous", "Every", "Only Changed" };
        EditorGUILayout.LabelField("Schedule:", new GUILayoutOption[] { GUILayout.Width(70) });
        vmins.scheduleMode = EditorGUILayout.Popup(vmins.scheduleMode, soptions, new GUILayoutOption[] { GUILayout.Width(80) });
        GUILayout.EndHorizontal();

        GUI.enabled = true;

        GUILayout.BeginHorizontal(new GUILayoutOption[] { });
        if (GUILayout.Button("Request Direct Placement", new GUILayoutOption[] { GUILayout.Width(180.0f) }))
        {
            vmins.requestPlacementWithDirectView = true;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        {
            GUILayout.BeginHorizontal(new GUILayoutOption[] { });
            GUILayout.Space(30.0f);
            string[] loptions = { "Last Placement", "Closest To", "Discrete Closest To" };
            vmins.notVisibleLayoutMode = EditorGUILayout.Popup("Layout Mode:", vmins.notVisibleLayoutMode, loptions, new GUILayoutOption[] { GUILayout.Width(300) });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[] { });
            GUILayout.Space(30.0f);
            string[] aoptions = { "Full Line", "Cut Line", "Arrow" };
            vmins.notVisibleArrowMode = EditorGUILayout.Popup("Arrow Mode:", vmins.notVisibleArrowMode, aoptions, new GUILayoutOption[] { });

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(new GUILayoutOption[] { });
            GUILayout.Space(60.0f);

            string[] loptions = { "None", "Outside", "Inside", "Left", "Right", "Top", "Bottom" };
            vmins.notVisibleArrowLayoutMode = EditorGUILayout.Popup("Layout:", vmins.notVisibleArrowLayoutMode, loptions, new GUILayoutOption[] { });

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal(new GUILayoutOption[] { });

        GUILayout.Label("Set strategy to:", GUILayout.Width(150));

        if (GUILayout.Button("Flyover", new GUILayoutOption[] { GUILayout.Width(80.0f) }))
        {
            vmins.setToFlyoverLayoutImpl();
        }
        if (GUILayout.Button("Still", new GUILayoutOption[] { GUILayout.Width(80.0f) }))
        {
            vmins.setToStillLayoutImpl();
        }
        if (GUILayout.Button("Off", new GUILayoutOption[] { GUILayout.Width(80.0f) }))
        {
            vmins.turnOff(true);
        }
        GUILayout.EndHorizontal();

        /*
        EditorGUILayout.PropertyField(screenStabilized, new GUIContent("Screen Stabilized:"), new GUILayoutOption[] { });
        if (screenStabilized.boolValue != vmins.screenStabilized)
            vmins.screenStabilized = screenStabilized.boolValue;
		*/
        GUILayout.BeginHorizontal (new GUILayoutOption[] {  });

        EditorGUILayout.PropertyField (shouldMoveWhenByPercentage, new GUIContent ("Move If Overlapped:"), new GUILayoutOption[] { });
		if (shouldMoveWhenByPercentage.boolValue != vmins.shouldMoveWhenByPercentage) 
			vmins.shouldMoveWhenByPercentage = shouldMoveWhenByPercentage.boolValue;

		GUI.enabled = vmins.shouldMoveWhenByPercentage;
		EditorGUILayout.PropertyField (moveWhenNotVisiblePercentage, new GUIContent ("Less-than Percentage:"), new GUILayoutOption[] { GUILayout.Width (200.0f) });
		if (vmins.moveWhenNotVisiblePercentage != moveWhenNotVisiblePercentage.floatValue){
			vmins.moveWhenNotVisiblePercentage = moveWhenNotVisiblePercentage.floatValue;
		}
		GUILayout.EndHorizontal ();


		GUILayout.BeginHorizontal (new GUILayoutOption[] {  });
		EditorGUILayout.PropertyField (fadeIn, new GUIContent ("Fade In:"), new GUILayoutOption[] { });
		if (fadeIn.boolValue != vmins.fadeIn) 
			vmins.fadeIn = fadeIn.boolValue;
		EditorGUILayout.PropertyField (fadeOut, new GUIContent ("Fade Out:"), new GUILayoutOption[] { });
		if (fadeOut.boolValue != vmins.fadeOut) 
			vmins.fadeOut = fadeOut.boolValue;
		GUILayout.EndHorizontal ();

		GUI.enabled = vmins.fadeIn || vmins.fadeOut;
		EditorGUILayout.PropertyField (fadeTime, new GUIContent ("Fade Time:"), new GUILayoutOption[] { GUILayout.Width (200.0f) });
		if (vmins.fadeTime != fadeTime.floatValue){
			vmins.fadeTime = fadeTime.floatValue;
		}

		GUI.enabled = true;

		if (geometryFold = EditorGUILayout.Foldout (geometryFold, "Geometry Parameters:")) {
			GUILayout.BeginHorizontal (new GUILayoutOption[] {  });
			GUILayout.Space (30.0f);
			string[] goptions = { "BBX", "BBX Rotated", "Geometry" };
			vmins.geometryMode = EditorGUILayout.Popup ("Geometry Mode:", vmins.geometryMode, goptions);
			GUILayout.EndHorizontal ();
		}

		if (annotationFold = EditorGUILayout.Foldout (annotationFold, "Annotation Parameters:")) {
			EditorGUILayout.PropertyField (sizeIsDefault);
			if (sizeIsDefault.boolValue != vmins.sizeIsDefault) {
				vmins.sizeIsDefault = sizeIsDefault.boolValue;
			}
			if (vmins.sizeIsDefault) {
				GUI.enabled = false;
			}

			if (vmins.minHeight > vmins.maxHeight) {
				GUI.color = Color.red;
			}
			
			ViewManager vm = FindObjectOfType<ViewManager> ();
			if (vm.inPixels) {
				// serializedObject might not get updated if manually set?
				int prevMinHeight = vmins.minHeight;
				//int prevMaxHeight = vmins.maxHeight;
				minHeight.intValue = vmins.minHeight;
				maxHeight.intValue = vmins.maxHeight;
				if (EditorGUILayout.PropertyField (minHeight, new GUILayoutOption[] { GUILayout.Width (175.0f) })) {
					vmins.minHeight = minHeight.intValue;
				}
				if (EditorGUILayout.PropertyField (maxHeight, new GUILayoutOption[] { GUILayout.Width (175.0f) })) {
					vmins.maxHeight = maxHeight.intValue;
				}
				if (vmins.minHeight != prevMinHeight) {
					vmins.minHeightChanged (vmins.minHeight);
				}
				if (vmins.maxHeight != prevMinHeight) {
					vmins.maxHeightChanged (vmins.maxHeight);
				}
			} else {
				float height = ViewManager.getCurrentCamera().pixelHeight;
				// serializedObject might not get updated if manually set?
				minHeightFloat.floatValue = vmins.minHeight / height;
				maxHeightFloat.floatValue = vmins.maxHeight / height;
				if (EditorGUILayout.PropertyField (minHeightFloat, new GUIContent ("Min Height"), new GUILayoutOption[] { GUILayout.Width (175.0f) })) {
					vmins.minHeight = (int)Mathf.Round (minHeightFloat.floatValue * height);
					vmins.minHeightChanged (vmins.minHeight);
				}
				if (EditorGUILayout.PropertyField (maxHeightFloat, new GUIContent ("Max Height"), new GUILayoutOption[] { GUILayout.Width (250.0f) })) {
					vmins.maxHeight = (int)Mathf.Round (maxHeightFloat.floatValue * height);
					vmins.maxHeightChanged (vmins.maxHeight);
				}
			}

			GUI.enabled = true;

			GUILayout.Label ("Buffers (relative to miny):", GUILayout.Width (150));

			GUI.enabled = vmins.m_tryToLabelInside;

			GUILayout.BeginHorizontal ();
			GUILayout.Space (15.0f);
			EditorGUILayout.PropertyField (insideBuffer, new GUIContent ("Inside:"), new GUILayoutOption[] { GUILayout.Width (200) });
			if (insideBuffer.floatValue != vmins.m_insideBuffer) {
				vmins.insideBuffer = insideBuffer.floatValue;
			}
			GUILayout.EndHorizontal ();

			GUI.enabled = vmins.m_tryToLabelOutside;
			GUILayout.BeginHorizontal ();
			GUILayout.Space (15.0f);

			EditorGUILayout.PropertyField (outsideBuffer, new GUIContent ("Outside:"), new GUILayoutOption[] { GUILayout.Width (200) });
			if (outsideBuffer.floatValue != vmins.m_outsideBuffer) {
				vmins.outsideBuffer = outsideBuffer.floatValue;
			}
			GUILayout.EndHorizontal ();

			GUI.enabled = true;
	
		}
		if (strategiesFold = EditorGUILayout.Foldout (strategiesFold, "Strategies and Placements:")) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Max Outside Distance:", GUILayout.Width (150));

			EditorGUILayout.PropertyField (maxOutsideDistance, GUIContent.none, new GUILayoutOption[] { });
			if (maxOutsideDistance.floatValue != vmins.maxOutsideDistance) {
				vmins.maxOutsideDistance = maxOutsideDistance.floatValue;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.PropertyField (alwaysFacing, new GUIContent ("Always facing:"));
			if (alwaysFacing.boolValue != vmins.alwaysFacing) {
				vmins.alwaysFacing = alwaysFacing.boolValue;
			}

			GUI.enabled = !vmins.alwaysFacing;

			EditorGUILayout.PropertyField (planeOriented, new GUIContent ("Relative to plane:"));
			if (planeOriented.boolValue != vmins.planeOriented) {
				vmins.planeOriented = planeOriented.boolValue;
			}
			if (!vmins.planeOriented)
				GUI.enabled = false;
			GUILayout.BeginHorizontal (new GUILayoutOption[] { });
			GUILayout.Space (10.0f);
			EditorGUILayout.PropertyField (planeOrientedAxisAligned, new GUIContent ("Axis Aligned/Y-Up:"));
			if (planeOrientedAxisAligned.boolValue != vmins.planeOrientedAxisAligned) {
				vmins.planeOrientedAxisAligned = planeOrientedAxisAligned.boolValue;
			}

			GUILayout.EndHorizontal ();
			GUI.enabled = true;

			vmins.scheduleMode = EditorGUILayout.Popup ("Schedule Mode:", vmins.scheduleMode, soptions);
			GUI.enabled = (vmins.scheduleMode == 2);
			int prevscheduleDurationEveryMS = scheduleDurationEveryMS.intValue;
			EditorGUILayout.PropertyField (scheduleDurationEveryMS, new GUIContent ("   every milliseconds:"), new GUILayoutOption[]{ });
			if (prevscheduleDurationEveryMS != scheduleDurationEveryMS.intValue) {
				vmins.scheduleDurationEveryMS = scheduleDurationEveryMS.intValue;
			}
			GUI.enabled = (vmins.scheduleMode != 1);
			bool prevInterpolateBetween = interpolateBetween.boolValue;
			EditorGUILayout.PropertyField (interpolateBetween, new GUIContent ("Interpolate:"), new GUILayoutOption[] { });
			if (interpolateBetween.boolValue != prevInterpolateBetween) {
				vmins.interpolateBetween = interpolateBetween.boolValue;
			}
			GUI.enabled = interpolateBetween.boolValue && (vmins.scheduleMode != 1);

			GUILayout.BeginHorizontal (new GUILayoutOption[] { });
			GUILayout.Space (10.0f);

			int prevInterpolateBetweenTimeMS = interpolateBetweenTimeMS.intValue;
			EditorGUILayout.PropertyField (interpolateBetweenTimeMS, new GUIContent ("Time (ms):"), new GUILayoutOption[] { });
			if (prevInterpolateBetweenTimeMS != interpolateBetweenTimeMS.intValue) {
				vmins.interpolateBetweenTimeMS = interpolateBetweenTimeMS.intValue;
			}
			GUILayout.EndHorizontal ();

			GUI.enabled = true;

			string[] hoptions = { "Best", "Closest To Previous" };
			vmins.hysteresisMode = EditorGUILayout.Popup ("Hysteresis Mode:", vmins.hysteresisMode, hoptions);

			GUI.enabled = (vmins.scheduleMode == 0);
			if (GUILayout.Button ("Manually Compute Placement", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
				// if prefab, then call on all spawned objects
				vmins.computePlacement ();
			}
			GUI.enabled = true;

			string[] zoptions = new string[]{ "constant", "minz", "maxz", "centerz" };
			vmins.zMode = EditorGUILayout.Popup ("Annotation Depth:", vmins.zMode, zoptions);
			if (vmins.zMode != 0)
				GUI.enabled = false;
			float prevconstantZDistance = constantZDistance.floatValue;
			EditorGUILayout.PropertyField (constantZDistance);
			if (prevconstantZDistance != constantZDistance.floatValue) {
				vmins.constantZDistance = constantZDistance.floatValue;
			}
		}
		GUI.enabled = true;

//			GUILayout.EndHorizontal();
//			vmins.minHeight = EditorGUILayout.PropertyField (minHeight, GUILayout.Width (175.0f));
/*			if (minHeight.intValue != vmins.minHeight) {
				vmins.minHeight = minHeight.intValue;
			}*/
//			vmins.maxHeight = EditorGUILayout.PropertyField (maxHeight, GUILayout.Width (175.0f));
			/*if (maxHeight.intValue != vmins.maxHeight) {
				vmins.maxHeight = maxHeight.intValue;
			}*/
		if (debugFold = EditorGUILayout.Foldout (debugFold, "Debug:")) {
			EditorGUILayout.PropertyField (showBBX, new GUIContent ("Show BBX:"), new GUILayoutOption[] { });
			if (showBBX.boolValue != vmins.showBBX) {
				vmins.showBBX = showBBX.boolValue;
			}
			GUILayout.BeginHorizontal (new GUILayoutOption[] { });
			if (GUILayout.Button ("Highlight Visibility", new GUILayoutOption [] { GUILayout.Width (150.0f) })) {
				// if prefab, then call on all spawned objects
				//ViewManager vm = FindObjectOfType<ViewManager> ();
				int objID = vmins.GetVMInstanceID ();
				DPManagerScript.Call_noargs ("Remove-BSPObjectPart-Visual-map");
				DPManagerScript.Call_i ("Set-Highlighted-For-Debugging-map", objID);
			}
			if (GUILayout.Button ("Highlight Off", new GUILayoutOption [] { GUILayout.Width (150.0f) })) {
				DPManagerScript.Call_noargs ("Unset-Highlighted-For-Debugging-map");
			}
			GUILayout.EndHorizontal ();
		}
		if (hasPrefabMessage){
			if (hasPrefabWarning) {
				GUI.color = Color.red;
				GUIStyle s = new GUIStyle (EditorStyles.textField);
				//s.normal.textColor = Color.red;
				GUI.color = Color.red;
				s.wordWrap = true;
				EditorGUILayout.LabelField ("Warning: Prefab has no clones tag '" + vmins.gameObject.tag + "', parameters only control new objects", s, new GUILayoutOption[0]);
			} else if (hasPrefabInfo) {
				EditorStyles.label.wordWrap = true;
				EditorGUILayout.LabelField( "Prefab parameters do not control any live objects", new GUILayoutOption[0] );
			} else {
				EditorStyles.label.wordWrap = true;
				EditorGUILayout.LabelField( "Prefab parameters controls " + nclones + " objects with same tag '" + vmins.gameObject.tag + "'", new GUILayoutOption[0] );
				if (GUILayout.Button ("Syncronize All Game Objects", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
					// if prefab, then call on all spawned objects
					vmins.CallAllChanged();
				}
			}
		}
		serializedObject.ApplyModifiedProperties ();
	}
}
#endif
