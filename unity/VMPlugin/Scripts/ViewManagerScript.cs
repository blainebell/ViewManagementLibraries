#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class ViewManagerScript : EditorWindow
{
    static DPManagerScript dpManager = null;
    //	static ViewManager viewManager = null;

	static bool isGame = false;
	public static GameObject [] allPrefabs = null;
	public static GameObject [] allGameObjects = null;
	public static void invalidate(){
		allPrefabs = null;
		allGameObjects = null;
		SetWindowDirty ();
	}
	// Add menu item named "My Window" to the Window menu
	[MenuItem ("Window/View Management")]
	public static void ShowWindow ()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow (typeof(ViewManagerScript));
		if (dpManager == null) {
			dpManager = (DPManagerScript)FindObjectOfType (typeof(DPManagerScript));
		}
        reloadPrefabsAndGameObjects ();
	}
	public static void reloadPrefabsAndGameObjects(){
		if (allPrefabs==null || isGame != Application.isPlaying) {
			VMObject[] vmobjects = Resources.FindObjectsOfTypeAll<VMObject> ();
			List<GameObject> golist = new List<GameObject> ();
			List<GameObject> prefablist = new List<GameObject> ();
			for (int i = 0; i < vmobjects.Length; i++) {
				VMObject vmo = vmobjects [i] as VMObject;
				GameObject go = vmo.gameObject;
				if (DPUtils.isPrefab (go)) {//PrefabUtility.GetPrefabParent(go) == null && PrefabUtility.GetPrefabObject(go) != null){
					//Debug.Log ("vmobjects[" + i + "]=" + vmobjects[i].name);
					prefablist.Add (vmo.gameObject);
				} else {
					golist.Add (vmo.gameObject);
				}
			}
			allPrefabs = prefablist.ToArray ();
			allGameObjects = golist.ToArray ();
			isGame = Application.isPlaying;
		}
	}
    bool clearPrevious = false;
    Vector2 scrollPos = new Vector2(), prefabScrollPos = new Vector2(),
        allGameObjectsScrollPos = new Vector2(), traverseScrollPos = new Vector2();
    bool globalFold = true, debuggingFold = true, allPrefabsFold = true, allGameObjectsFold = true,
         projectionFold = true, defaultsFold = true, licenseFold = true, editorFold = true;
    int highlightedTraverseItem = -1, highlightedTraverseDimID = -1;
    Texture2D highlightTex = null;
    bool traverseChecked = false, hasBeenRunning = false;
    void OnGUI ()
	{
		ViewManager vm = FindObjectOfType<ViewManager> ();
		if (vm == null) {
			GUILayout.Label ("View Manager: PLEASE ATTACH View Manager", EditorStyles.boldLabel);
			return;
		}

        scrollPos = EditorGUILayout.BeginScrollView (scrollPos, true, true, new GUILayoutOption[0]);
		GUILayout.BeginHorizontal (new GUILayoutOption[] { GUILayout.Height (22.0f) });
		GUILayout.Label ("View Manager", EditorStyles.boldLabel);
		GameObject vmgo = null;
		vmgo = vm.gameObject;
		GUI.enabled = false;
		GUILayout.BeginVertical();
		GUILayout.Space(10.0f);
#if DEBUG_VM
		EditorGUILayout.ObjectField (vmgo, typeof(GameObject), true, new GUILayoutOption[] { });
#endif
		GUILayout.EndVertical ();
		GUI.enabled = true;
		GUILayout.EndHorizontal ();

		reloadPrefabsAndGameObjects ();

        // License Stuff
        bool requestLicense = false;
        if (licenseFold = EditorGUILayout.Foldout(licenseFold, "License"))
        {
            bool showLogin = false;
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(22.0f) });
            GUILayout.Space(30.0f);
            GUILayout.Label("Status:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(120.0f) });
            string statusLabel = ViewManager.licenseStatusStrings[ViewManager.licenseStatusGet()];
            GUILayout.Label(statusLabel, EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(130.0f) });
			if (ViewManager.licenseStatusGet () != ViewManager.LicenseStatus.Unchecked) {
				if (GUILayout.Button ("Clear License", new GUILayoutOption[] { GUILayout.Width (100.0f) })) {
					ViewManager.licenseStatusSet (ViewManager.LicenseStatus.Unchecked);
					vm.ClearLicense ();
					ViewManager.error_message = "";
				}
			}
            GUILayout.Space(30.0f);
            GUILayout.EndHorizontal();
            if (ViewManager.licenseStatusGet() == ViewManager.LicenseStatus.Verified)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(22.0f) });
                GUILayout.Space(30.0f);
                GUILayout.Label("Email:", new GUILayoutOption[] { GUILayout.Width(120.0f) });
                GUILayout.Label(DPManagerScript.GetEmail());
                GUILayout.FlexibleSpace();
                vm.checkLicenseOnStart = GUILayout.Toggle(vm.checkLicenseOnStart, "Check on start", new GUILayoutOption[] { GUILayout.Width(110.0f) });
                GUILayout.Space(30.0f);
                GUILayout.EndHorizontal();
                // Expiration Date:
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(22.0f) });
                GUILayout.Space(30.0f);
                GUILayout.Label("Expiration Date:", new GUILayoutOption[] { GUILayout.Width(120.0f) });
                GUILayout.Label(DPManagerScript.GetExpirationDate());
                GUILayout.FlexibleSpace();
                GUILayout.Label("Days left:");
                GUILayout.Label(ViewManager.getDaysLeft().ToString());
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else if (ViewManager.licenseStatusGet() == ViewManager.LicenseStatus.Unchecked)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(22.0f) });
                GUILayout.Space(30.0f);
                if (GUILayout.Button("Check License", new GUILayoutOption[] { GUILayout.Width(150.0f) }))
                {
                    vm.CheckLicense();
                }
                GUILayout.FlexibleSpace();
                vm.checkLicenseOnStart = GUILayout.Toggle(vm.checkLicenseOnStart, "Check on start", new GUILayoutOption[] { GUILayout.Width(110.0f) });
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else if (ViewManager.licenseStatusGet() == ViewManager.LicenseStatus.Failed)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(22.0f) });
                GUILayout.FlexibleSpace();
                vm.checkLicenseOnStart = GUILayout.Toggle(vm.checkLicenseOnStart, "Check on start", new GUILayoutOption[] { GUILayout.Width(110.0f) });
                GUILayout.Space(30.0f);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(22.0f) });
                GUILayout.Space(30.0f);
                GUILayout.Label("Your license has failed.  Please login to fix it.\n");
                GUILayout.EndHorizontal();
                showLogin = true;
            }
            else if (ViewManager.licenseStatusGet() == ViewManager.LicenseStatus.NonExistent)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(22.0f) });
                GUILayout.FlexibleSpace();
                vm.checkLicenseOnStart = GUILayout.Toggle(vm.checkLicenseOnStart, "Check on start", new GUILayoutOption[] { GUILayout.Width(110.0f) });
                GUILayout.Space(30.0f);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(22.0f) });
                GUILayout.Space(30.0f);
                GUILayout.Label("You need a license to use this plugin.\nPlease login with the correct credentials to install it.\n");// Contact blaine@everyware3d.com to obtain a license.");
                GUILayout.EndHorizontal();
                showLogin = true;
            }
            if (showLogin)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(22.0f) });
                GUILayout.Space(30.0f);
                GUI.SetNextControlName("MyTextField");
                bool ch = false;
                string prevEmailString = vm.emailString;
                vm.emailString = EditorGUILayout.TextField("Email:", vm.emailString, new GUILayoutOption[] { });
                ch = (vm.emailString != prevEmailString);
                GUILayout.Space(30.0f);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(22.0f) });
                GUILayout.Space(30.0f);
                GUI.SetNextControlName("MyTextField");
				string prevPasswordString = vm.passwordGet();
				vm.passwordSet(EditorGUILayout.PasswordField("Password:", vm.passwordGet(), new GUILayoutOption[] { }));
					if (vm.passwordGet() != prevPasswordString)
                    ch = true;
                GUILayout.Space(30.0f);
                GUILayout.EndHorizontal();
				GUI.enabled = vm.passwordGet().Length > 0 && ViewManager.isValidEmail(vm.emailString);

                if (ch)
                {
                    vm.changed();
                }

                requestLicense = GUI.enabled && (Event.current.type == EventType.KeyUp) &&
                                      (Event.current.keyCode == KeyCode.Return) &&
                                      (GUI.GetNameOfFocusedControl() == "MyTextField");

                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(22.0f) });
                GUILayout.Space(30.0f);
                if (GUILayout.Button("Request License", new GUILayoutOption[] { GUILayout.Width(150.0f) }))
                {
                    requestLicense = true;
                }
                GUILayout.EndHorizontal();
                GUI.enabled = true;
            }
            if (ViewManager.error_message.Length > 0)
            {
                var prevCol = GUI.contentColor;
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.red;
                style.fontStyle = FontStyle.Bold;
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(ViewManager.error_message, style);
                GUILayout.Space(20);
                GUILayout.EndHorizontal();
                GUI.contentColor = prevCol;
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                if (GUILayout.Button("Contact", new GUILayoutOption[] { GUILayout.Width(100.0f) }))
                {
                    Application.OpenURL("mailto:blaine@everyware3d.com?subject=Unity Plugin License");
                }
                GUILayout.EndHorizontal();
            }
        }
        // END LICENSE STUFF
        if (ViewManager.licenseStatusGet() == ViewManager.LicenseStatus.Verified)
        {
            if (globalFold = EditorGUILayout.Foldout(globalFold, "Global"))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30.0f);
                GUILayout.Label("Screen Projection Mode:");
                vm.screenProjectionMode = EditorGUILayout.Popup(vm.screenProjectionMode, new string[] { "Screen", "Bridge", "Vive"}, new GUILayoutOption[] { GUILayout.Width(100)  });
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Space(30.0f);
				vm.shareRendering = GUILayout.Toggle(vm.m_shareRendering, "Share Rendering", new GUILayoutOption[] {  });
				vm.shareScreen = GUILayout.Toggle(vm.m_shareScreen, "Share Screen", new GUILayoutOption[] {  });
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("Coordinate System Traversal Order:");
                if (!Application.isPlaying){
                    GUILayout.Label("(On Startup)");
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                float mh = 70.0f;
                traverseScrollPos = EditorGUILayout.BeginScrollView(traverseScrollPos, false, false, new GUILayoutOption[] { GUILayout.MinHeight(mh), GUILayout.Height(mh), GUILayout.MaxHeight(100) });

                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
                GUIStyle highlightStyle = new GUIStyle(GUI.skin.label);
                boldStyle.fontStyle = FontStyle.Bold;
                if (highlightTex == null)
                    highlightTex = DPUtils.MakeTex(1, 1, new Color(.5f, .5f, .5f, 1.0f));
                highlightStyle.normal.background = highlightTex;
                // column titles
                GUILayout.BeginHorizontal();
                GUILayout.Space(30.0f);
                GUILayout.Label("Order", boldStyle, new GUILayoutOption[] { GUILayout.Width(50.0f) });
                GUILayout.Label("Dimension Name", boldStyle, new GUILayoutOption[] { GUILayout.Width(140.0f) });
                GUI.enabled = !Application.isPlaying;
                if (GUILayout.Button("refresh", new GUILayoutOption[] { GUILayout.Width(60.0f) })){
                    VMDimension.traversalOrder.Clear();
                    VMDimension [] alldims = Resources.FindObjectsOfTypeAll<VMDimension>();
                    foreach (VMDimension dim in alldims){
                        if (dim.traverseOnStartupMode==1){
                            VMDimension.traversalOrder.Add(1, dim);
                        } else if (dim.traverseOnStartupMode == 2){
                            VMDimension.traversalOrder.Add(dim.traverseOnStartupOrder, dim);
                        }
                    }
                }
                if (GUILayout.Button("reorder", new GUILayoutOption[] { GUILayout.Width(60.0f) }))
                {
                    int order = 1;
                    SortedDictionary<int, VMDimension> oldtO = VMDimension.traversalOrder;
                    VMDimension.traversalOrder = new SortedDictionary<int, VMDimension>();
                    foreach (int dimid in oldtO.Keys){
                        VMDimension dim;
                        if (oldtO.TryGetValue(dimid, out dim)) {
                            dim.traverseOnStartupOrder = order;
                            VMDimension.traversalOrder.Add(order, dim);
                            order++;
                        }
                    }
                    highlightedTraverseItem = -1;
                    highlightedTraverseDimID = -1;
                }

                GUI.enabled = highlightedTraverseItem >= 0 && VMDimension.traversalOrder.Count > 1;
                char upArrow = '\u25B2';
                if (GUILayout.Button(upArrow.ToString(), new GUILayoutOption[] { GUILayout.Width(20.0f) })) {
                    VMDimension dim;
                    if (Application.isPlaying){
                        if (VMDimension.m_VMDimensions.TryGetValue(highlightedTraverseDimID, out dim)){
                            int res = dim.moveInPlaying(-1);
                            if (res >= 0){
                                highlightedTraverseItem = res + 1;
                                highlightedTraverseDimID = dim.GetVMDimensionID();
                            }
                        }
                    } else if (VMDimension.traversalOrder.TryGetValue(highlightedTraverseItem, out dim)) {
                        int prevKey = -1;
                        foreach (KeyValuePair<int, VMDimension> info in VMDimension.traversalOrder){
                            if (info.Key == highlightedTraverseItem)
                                break;
                            prevKey = info.Key;
                        }
                        if (prevKey >= 0) {
                            // swap with previous item
                            VMDimension pdim;
                            if (VMDimension.traversalOrder.TryGetValue(prevKey, out pdim)){
                                VMDimension.traversalOrder.Remove(prevKey);
                                VMDimension.traversalOrder.Remove(highlightedTraverseItem);
                                VMDimension.traversalOrder.Add(prevKey, dim);
                                VMDimension.traversalOrder.Add(highlightedTraverseItem, pdim);
                                dim.traverseOnStartupOrder = prevKey;
                                pdim.traverseOnStartupOrder = highlightedTraverseItem;
                                highlightedTraverseItem = prevKey;
                                highlightedTraverseDimID = pdim.GetVMDimensionID();
                            }
                        }
                    }
                }
                char downArrow = '\u25BC';
                if (GUILayout.Button(downArrow.ToString(), new GUILayoutOption[] { GUILayout.Width(20.0f) })){
                    VMDimension dim;
                    if (Application.isPlaying){
                        if (VMDimension.m_VMDimensions.TryGetValue(highlightedTraverseDimID, out dim)){
                            int res = dim.moveInPlaying(1);
                            if (res >= 0){
                                highlightedTraverseItem = res + 1;
                                highlightedTraverseDimID = dim.GetVMDimensionID();
                            }
                        }
                    }
                    else if (VMDimension.traversalOrder.TryGetValue(highlightedTraverseItem, out dim)){
                        int afterKey = -1;
                        bool prevIsKey = false;
                        foreach (KeyValuePair<int, VMDimension> info in VMDimension.traversalOrder){
                            if (prevIsKey){
                                afterKey = info.Key;
                                break;
                            }
                            prevIsKey = info.Key == highlightedTraverseItem;
                        }
                        if (afterKey >= 0){
                            // swap with previous item
                            VMDimension pdim;
                            if (VMDimension.traversalOrder.TryGetValue(afterKey, out pdim)){
                                VMDimension.traversalOrder.Remove(afterKey);
                                VMDimension.traversalOrder.Remove(highlightedTraverseItem);
                                VMDimension.traversalOrder.Add(afterKey, dim);
                                VMDimension.traversalOrder.Add(highlightedTraverseItem, pdim);
                                dim.traverseOnStartupOrder = afterKey;
                                pdim.traverseOnStartupOrder = highlightedTraverseItem;
                                highlightedTraverseItem = afterKey;
                                highlightedTraverseDimID = dim.GetVMDimensionID();
                            }
                        }
                    }
                }
                if (GUILayout.Button("x", new GUILayoutOption[] { GUILayout.Width(20.0f) }))
                {
                    if (Application.isPlaying){
                        VMDimension dim;
                        if (VMDimension.m_VMDimensions.TryGetValue(highlightedTraverseDimID, out dim)){
                            dim.removeFromPlaying();
                        }
                    }
                    else {
                        VMDimension dim;
                        if (VMDimension.traversalOrder.TryGetValue(highlightedTraverseItem, out dim)){
                            VMDimension.traversalOrder.Remove(highlightedTraverseItem);
                            highlightedTraverseDimID = highlightedTraverseItem = -1;
                            dim.traverseOnStartupMode = 0;
                            dim.traverseOnStartupOrder = -1;
                        }
                    }
                }
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                SortedDictionary<int, VMDimension> tdict;
                //bool isTraverseDict = false;
                if (Application.isPlaying){
                    int[] dinfo = vm.getAllDimInfo().ToArray();
                    tdict = new SortedDictionary<int, VMDimension>();

                    for (int i = 0; i < dinfo.Length; i += 2){
                        VMDimension dim;
                        if (VMDimension.m_VMDimensions.TryGetValue(dinfo[i + 1], out dim)){
                            tdict.Add(dinfo[i], dim);
                        }
                    }
                    hasBeenRunning = true;
                } else {
                    if (hasBeenRunning){
                        VMDimension.traversalOrder.Clear();
                        hasBeenRunning = false;
                    }
                    if (VMDimension.traversalOrder.Count == 0) {
                        if (!traverseChecked){
                            VMDimension[] alldims = FindObjectsOfType<VMDimension>();
                            foreach (VMDimension dim in alldims){
                                if (dim.traverseOnStartupMode == 1){
                                    VMDimension.traversalOrder.Add(1, dim);
                                    break;
                                } else if (dim.traverseOnStartupMode == 2)
                                    VMDimension.traversalOrder.Add(dim.traverseOnStartupOrder, dim);
                            }
                            traverseChecked = VMDimension.traversalOrder.Count == 0;
                        }
                    } else {
                        traverseChecked = false;
                    }
                    tdict = VMDimension.traversalOrder;
                    //isTraverseDict = true;
                }
                foreach (KeyValuePair<int, VMDimension> orddim in tdict){
                    try
                    {
                        string name = orddim.Value.gameObject.name;
                    } catch (MissingReferenceException) { continue; }
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(30.0f);
                    bool isHighlighted = highlightedTraverseItem == orddim.Key;
                    GUIStyle bstyle = isHighlighted ? highlightStyle : labelStyle;
                    bool pressed = false;
                    if (GUILayout.Button(orddim.Key.ToString(), bstyle, new GUILayoutOption[] { GUILayout.Width(50.0f) })){
                        pressed = true;
                    }
                    if (GUILayout.Button(orddim.Value.gameObject.name, bstyle, new GUILayoutOption[] { GUILayout.Width(140.0f) })){
                        pressed = true;
                    }
                    EditorGUILayout.ObjectField("", orddim.Value, typeof(VMDimension), true, new GUILayoutOption[0]);

                    if (pressed){
                        if (isHighlighted){
                            highlightedTraverseItem = -1;
                            highlightedTraverseDimID = -1;
                        } else {
                            highlightedTraverseItem = orddim.Key;
                            highlightedTraverseDimID = orddim.Value.GetVMDimensionID();
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                GUILayout.Space(20.0f);

                GUILayout.BeginHorizontal();
                GUILayout.Space(30.0f);
                vm.prefabsControlWithSameTag = GUILayout.Toggle(vm.prefabsControlWithSameTag, "Prefab Parameters Control Objects With Same Tag", new GUILayoutOption[] {  });
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
				GUILayout.Space(30.0f);
				vm.doNotMoveWhenCloseToEdge = GUILayout.Toggle(vm.doNotMoveWhenCloseToEdge, "Do Not Move When Close To Edge", new GUILayoutOption[] { });
				GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(30.0f);
                GUILayout.Label("Total VM Objects:");
#if NEVER
			vm.tryToLabelAll = EditorGUILayout.Toggle ("Try To Label All", vm.tryToLabelAll, new GUILayoutOption[] { });
#endif
                int nprf = 0;
                if (allPrefabs != null)
                    nprf = allPrefabs.Length;
                if (Application.isPlaying)
                {
                    // count objects
                    VMObject[] vmobjects = Resources.FindObjectsOfTypeAll<VMObject>();
                    int nvmos = 0;
                    if (vmobjects != null)
                        nvmos = vmobjects.Length;
                    GUILayout.Label(nprf + " Prefabs, " + (nvmos - nprf) + " Objects");
                }
                else
                {
                    GUILayout.Label(nprf + " Prefabs");
                }
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal(new GUILayoutOption[] { });

                bool doNotMove = EditorGUILayout.ToggleLeft("Edge Distance", vm.doNotMoveWhenCloseToEdge, new GUILayoutOption[] { GUILayout.MaxWidth(125) });
                if (doNotMove != vm.doNotMoveWhenCloseToEdge)
                    vm.doNotMoveWhenCloseToEdge = doNotMove;
                GUI.enabled = vm.doNotMoveWhenCloseToEdge;
                string closeToEdgeString = EditorGUILayout.TextField("Edge distance:", vm.closeToEdgeDistance.ToString(), new GUILayoutOption[] { GUILayout.Width(200.0f) });
                float closeToEdge;
                if (float.TryParse(closeToEdgeString, out closeToEdge) && closeToEdge != vm.closeToEdgeDistance)
                {
                    vm.closeToEdgeDistance = closeToEdge;
                }

                GUILayout.EndHorizontal();

                GUI.enabled = true;

                GUILayout.BeginHorizontal();
                GUILayout.Space(30.0f);
                clearPrevious = GUILayout.Toggle(clearPrevious, "Clear", new GUILayoutOption[0]);
                if (GUILayout.Button("Do All Placements", new GUILayoutOption[] { GUILayout.Width(150.0f) }))
                {
                    DPManagerScript.Call_b("Do-All-Annotation-Placements-In-Traversed-CoordinateSystems-map", clearPrevious);
                }
                if (GUILayout.Button("Stagger Every", new GUILayoutOption[] { GUILayout.Width(100.0f) }))
                {
                    DPManagerScript.Call_noargs("Stagger-All-Every-Scheduled-Annotation-map");
                }
                GUILayout.EndHorizontal();

                bool shortenLines = EditorGUILayout.ToggleLeft("Shorten Outside Lines", vm.shortenOutsideLines, new GUILayoutOption[] { GUILayout.MaxWidth(150) });
                if (shortenLines != vm.shortenOutsideLines)
                    vm.shortenOutsideLines = shortenLines;

                // slider for time delay between computations
                GUILayout.Label("Delay for Computations:" + vm.delayBetweenComputationFrames.ToString(), new GUILayoutOption[0]);
                float delayBCF = GUILayout.HorizontalSlider(vm.delayBetweenComputationFrames, 0.0f, 1.0f, new GUILayoutOption[0]);
                if (delayBCF != vm.delayBetweenComputationFrames)
                    vm.delayBetweenComputationFrames = delayBCF;

                GUILayout.Label("Screen Stabilized Animation Speed:" + vm.screenStabilizedAnimationSpeed.ToString(), new GUILayoutOption[0]);
                float ssAnimSpeed = GUILayout.HorizontalSlider(vm.screenStabilizedAnimationSpeed, 0.0f, 50.0f, new GUILayoutOption[0]);
                if (ssAnimSpeed != vm.screenStabilizedAnimationSpeed)
                    vm.screenStabilizedAnimationSpeed = ssAnimSpeed;

            }
            if (debuggingFold = EditorGUILayout.Foldout(debuggingFold, "Debugging"))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30.0f);
                GUILayout.Label("Show:", new GUILayoutOption[] { GUILayout.MaxWidth(85) });

                vm.showEmptySpace = EditorGUILayout.ToggleLeft("Empty Space", vm.showEmptySpace, new GUILayoutOption[] { GUILayout.MaxWidth(85) });
                vm.showBSPPartitionPlanes = EditorGUILayout.ToggleLeft("BSP Planes", vm.showBSPPartitionPlanes, new GUILayoutOption[] { GUILayout.MaxWidth(85) });
                vm.showProjections = EditorGUILayout.ToggleLeft("Projections", vm.showProjections, new GUILayoutOption[] { GUILayout.MaxWidth(85) });
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(120.0f);
                vm.showResults = EditorGUILayout.ToggleLeft("Results", vm.showResults, new GUILayoutOption[] { GUILayout.MaxWidth(85) });
                vm.showPrevAnnotationProjections = EditorGUILayout.ToggleLeft("Previous Annotation Projections", vm.showPrevAnnotationProjections, new GUILayoutOption[] { GUILayout.MaxWidth(180) });
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(120.0f);
                bool showCloseToEdge = EditorGUILayout.ToggleLeft("Edge Rectangle", vm.showCloseToEdgeRectangle, new GUILayoutOption[] { GUILayout.MaxWidth(85) });
                if (showCloseToEdge != vm.showCloseToEdgeRectangle)
                {
                    vm.showCloseToEdgeRectangle = showCloseToEdge;
                }
                bool debugLabelOutline = EditorGUILayout.ToggleLeft("Label Outlines", vm.debugLabelOutline, new GUILayoutOption[] { GUILayout.MaxWidth(150) });
                if (debugLabelOutline != vm.debugLabelOutline)
                {
                    vm.debugLabelOutline = debugLabelOutline;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                vm.checkLineOverlap = EditorGUILayout.ToggleLeft("Check Line Overlap", vm.checkLineOverlap, new GUILayoutOption[] { GUILayout.MaxWidth(200) });

                vm.useCurrentVector = EditorGUILayout.ToggleLeft("Use Current Vector", vm.useCurrentVector, new GUILayoutOption[] { GUILayout.MaxWidth(200) });
                GUILayout.EndHorizontal();

                vm.useCurrentVectorForMenu = EditorGUILayout.ToggleLeft("Use Current Vector For Menu", vm.useCurrentVectorForMenu, new GUILayoutOption[] { GUILayout.MaxWidth(200) });

                GUILayout.BeginHorizontal();
                GUILayout.Space(30.0f);
                if (GUILayout.Button("Debug Traversal", new GUILayoutOption[] { GUILayout.Width(115.0f) }))
                {
                    vm.debugTraversal = true;
                }
                if (GUILayout.Button("Report Geometry Issues", new GUILayoutOption[] { GUILayout.Width(200.0f) }))
                {
                    vm.reportGeometry = true;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(30.0f);
                if (GUILayout.Button("Save Existing Traversal", new GUILayoutOption[] { GUILayout.Width(140.0f) }))
                {
                    vm.saveTraversal = true;
                }
                GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Space(30.0f);
				GUILayout.Label("Set All To:", new GUILayoutOption[] { GUILayout.MaxWidth(85) });
				if (GUILayout.Button("Flyover", new GUILayoutOption[] { GUILayout.Width(80.0f) }))
				{
					DPUtils.setToFlyoverLayoutForAll ();
					DPManagerScript.Call_noargs ("Set-To-All-New-Layouts-map");
				}
				if (GUILayout.Button("Still", new GUILayoutOption[] { GUILayout.Width(80.0f) }))
				{
					DPUtils.setToStillLayoutForAll ();
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Space(30.0f);
				GUILayout.Label("", new GUILayoutOption[] { GUILayout.MaxWidth(85) });
				if (GUILayout.Button("Continuous", new GUILayoutOption[] { GUILayout.Width(80.0f) }))
				{
					DPUtils.TurnAllToContinuousLayout();
				}
				if (GUILayout.Button("Off/Manual", new GUILayoutOption[] { GUILayout.Width(80.0f) }))
				{
					DPUtils.TurnAllToStillLayoutAndOff (true);
				}
				GUILayout.EndHorizontal();
            }

            if (projectionFold = EditorGUILayout.Foldout(projectionFold, "Projection Parameters"))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30.0f);
                vm.clipLinesForProjections = EditorGUILayout.ToggleLeft("Clip Edge Lines to Project Faces", vm.clipLinesForProjections, new GUILayoutOption[] { GUILayout.MaxWidth(200) });
                ViewManager.distortionCorrection = EditorGUILayout.ToggleLeft("Distortion Correction", ViewManager.distortionCorrection, new GUILayoutOption[] { GUILayout.MaxWidth(200) });
                GUILayout.EndHorizontal();
            }

            GUIStyle scrollbarStyle = new GUIStyle(GUI.skin.horizontalScrollbar);
            scrollbarStyle.fixedHeight = 0;
            scrollbarStyle.fixedWidth = 0;
            if (allPrefabsFold = EditorGUILayout.Foldout(allPrefabsFold, "All Prefabs:"))
            {
                //int fs = GUI.skin.font.fontSize;
                int mh = 50; //Math.Min((1+allPrefabs.Length) * GUI.skin.font.fontSize, 100);
                prefabScrollPos = EditorGUILayout.BeginScrollView(prefabScrollPos, new GUILayoutOption[] { GUILayout.MinHeight(mh), GUILayout.Height(mh), GUILayout.MaxHeight(100) });
                GUI.enabled = false;
                if (allPrefabs != null)
                {
                    foreach (GameObject go in allPrefabs)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(30.0f);
                        EditorGUILayout.ObjectField(go, typeof(GameObject), false, new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                }
                GUI.enabled = true;
                EditorGUILayout.EndScrollView();
            }
            if (Application.isPlaying)
            {
                if (allGameObjectsFold = EditorGUILayout.Foldout(allGameObjectsFold, "All Game Objects:"))
                {
                    int ngos = 0;
                    if (allGameObjects != null)
                        ngos = allGameObjects.Length;
                    int mh = Math.Min((1 + ngos) * GUI.skin.font.fontSize, 200);
                    allGameObjectsScrollPos = EditorGUILayout.BeginScrollView(allGameObjectsScrollPos, new GUILayoutOption[] { GUILayout.MinHeight(mh), GUILayout.Height(mh), GUILayout.MaxHeight(200) });
                    GUI.enabled = false;
                    if (ngos > 0)
                    {
                        foreach (GameObject go in allGameObjects)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(30.0f);
                            EditorGUILayout.ObjectField(go, typeof(GameObject), false, new GUILayoutOption[0]);
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndScrollView();
                }
            }
            GUILayout.BeginHorizontal();
            if (defaultsFold = EditorGUILayout.Foldout(defaultsFold, "Annotation Defaults:"))
            {
                GUILayout.Space(150.0f);
                bool inPixelsChanged = false;
                if (vm.inPixels)
                {
                    if (GUILayout.Button("In Pixels", new GUILayoutOption[] { GUILayout.Width(75.0f) }))
                        inPixelsChanged = true;
                }
                else
                {
                    if (GUILayout.Button("In Screen Space", new GUILayoutOption[] { GUILayout.Width(100.0f) }))
                        inPixelsChanged = true;
                }
                if (inPixelsChanged)
                {
                    GUI.FocusControl("");
                    GUI.UnfocusWindow();
                    vm.inPixels = !vm.inPixels;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(30.0f);

                GUIStyle s = new GUIStyle(EditorStyles.textField);

                EditorGUIUtility.labelWidth = 80.0f;

                if (vm.defaultAnnotationMinHeight > vm.defaultAnnotationMaxHeight)
                    s.normal.textColor = Color.red;
                if (vm.inPixels)
                {
                    try
                    {
                        vm.defaultAnnotationMinHeight = int.Parse(EditorGUILayout.TextField("Min Height:",
                            vm.defaultAnnotationMinHeight.ToString(), s, new GUILayoutOption[] { GUILayout.ExpandWidth(false) }));
                    }
                    catch (Exception ex) { ex.ToString(); }
                    try
                    {
                        vm.defaultAnnotationMaxHeight = int.Parse(EditorGUILayout.TextField("Max Height:",
                            vm.defaultAnnotationMaxHeight.ToString(), s, new GUILayoutOption[] { GUILayout.ExpandWidth(false) }));
                    }
                    catch (Exception ex) { ex.ToString(); }
                }
                else
                {
					float height = ViewManager.getCurrentCamera().pixelHeight;
                    float minHeight = vm.defaultAnnotationMinHeight / height;
                    float maxHeight = vm.defaultAnnotationMaxHeight / height;
                    try
                    {
                        minHeight = float.Parse(EditorGUILayout.TextField("Min Height:", minHeight.ToString(), s, new GUILayoutOption[] { }));
                    }
                    catch (Exception ex) { ex.ToString(); }
                    try
                    {
                        maxHeight = float.Parse(EditorGUILayout.TextField("Max Height:", maxHeight.ToString(), s, new GUILayoutOption[] { }));
                    }
                    catch (Exception ex) { ex.ToString(); }
                    vm.defaultAnnotationMinHeight = (int)Mathf.Round(minHeight * height);
                    vm.defaultAnnotationMaxHeight = (int)Mathf.Round(maxHeight * height);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.EndHorizontal();
            }

            //#if DONOTSHOW
            if (editorFold = EditorGUILayout.Foldout(editorFold, "Editor:"))
            {
                UnityEngine.Object[] gos = Selection.objects;
                if (GUILayout.Button("Create BBX of Selected", new GUILayoutOption[] { GUILayout.Width(170.0f) }))
                {
                    if (!Application.isPlaying && gos.Length > 0)
                    {
                        List<KeyValuePair<MeshFilter, Mesh>> meshList = new List<KeyValuePair<MeshFilter, Mesh>>();
                        int nverts = 0;
                        foreach (UnityEngine.Object ob in gos)
                        {
                            GameObject go = (GameObject)ob;
                            VMObject.GetMeshListForObject(meshList, go, ref nverts);
                        }
                        Bounds bounds = VMObject.GetBoundsFromMeshListBounds(meshList, null);
                        Debug.Log("#game objects=" + gos.Length + " bounds=" + bounds + " meshList.Count=" + meshList.Count + " nverts=" + nverts);
                        // Creating Object/Cube with size of the bounds of the selected objects
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = bounds.center;
                        cube.transform.localScale = bounds.size;
                        Selection.activeObject = cube;
                    }
                }
            }
			if (GUILayout.Button ("Create Overview Image", new GUILayoutOption[] { GUILayout.Width (170.0f) })) {
				int twidth = 500, theight = 500;
				GameObject go = GameObject.Find ("OrthoCamera");
				Camera cam = go.GetComponent<Camera> ();
//				cam.orthographic = true;
//				cam.orthographicSize = 10.0f;

				/*cam.transform.position = new Vector3 (0, 10, 0);
				Quaternion q = new Quaternion ();
				q.SetLookRotation (new Vector3 (0, -1, 0), Vector3.forward);
				cam.transform.rotation = q;
				*/
				//cam.transform.LookAt (new Vector3 ());
				RenderTexture renderTexture = new RenderTexture (twidth, theight, 24);
				cam.targetTexture = renderTexture;
			
				cam.Render ();
				RenderTexture.active = renderTexture;
				Texture2D virtualPhoto = new Texture2D (twidth, theight, TextureFormat.RGB24, false);
				virtualPhoto.ReadPixels (new Rect (0, 0, twidth, theight), 0, 0);
				byte[] bytes = virtualPhoto.EncodeToPNG();
				System.IO.File.WriteAllBytes("ScreenShot.png", bytes );

			}
            if (GUILayout.Button("Test", new GUILayoutOption[] { GUILayout.Width(170.0f) }))
            {
                MouseScript[] ms = FindObjectsOfType<MouseScript>();
                Debug.Log("#ms: " + ms.Length);
            }
                /*
                if (GUILayout.Button ("Do NoZ", new GUILayoutOption[] { GUILayout.Width (170.0f) })) {
                    UnityEngine.Object[] gos = Selection.objects;
                    Debug.Log ("#selected objects: " + gos.Length);
                    foreach (UnityEngine.Object ob in gos) {
                        GameObject go = (GameObject)ob;
                        MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer> ();
                        Debug.Log ("#selected MeshRenderers: " + mrs.Length);
                        foreach (MeshRenderer mr in mrs) {
                            Material material = mr.sharedMaterial;
                            material.shader = Shader.Find ("Standard NoZ");
                            Debug.Log("name=" + mr.gameObject.name + " layer=" + mr.gameObject.layer);
                        }
                        //int layer = DPUtils.addLayerIfNot ("UI");
                        //go.layer = layer;
                    }
                }
                if (GUILayout.Button ("Do Standard", new GUILayoutOption[] { GUILayout.Width (170.0f) })) {
                    UnityEngine.Object[] gos = Selection.objects;
                    foreach (UnityEngine.Object ob in gos) {
                        GameObject go = (GameObject)ob;
                        MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer> ();
                        foreach (MeshRenderer mr in mrs) {
                            Material material = mr.sharedMaterial;
                            material.shader = Shader.Find ("Standard");
                        }
                    }
                }
                if (GUILayout.Button ("Do Change Camvas default material", new GUILayoutOption[] { GUILayout.Width (170.0f) })) {
                    Canvas.GetDefaultCanvasMaterial ().shader = Shader.Find ("UI/Default");
                }
    */

                {
				//GameObject[] gos = GameObject.FindGameObjectsWithTag ("plane");
				//Debug.Log ("# objects=" + gos.Length);
			}
#if DONOTSHOW
			if (GUILayout.Button ("Remove All MouseOverScripts", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
				MouseOverImpl[] moss = FindObjectsOfType<MouseOverImpl> ();
				int nempty = 0;
				foreach (MouseOverImpl mos in moss) {
					if (DPUtils.equals (mos.viewNormalGet(), Vector3.zero) &&
						(mos.closestFromVectorGet() == null || mos.closestFromVectorGet().Length == 0) &&
						(mos.viewNormalForFromVectorGet() == null || mos.viewNormalForFromVectorGet().Length == 0) &&
						!mos.hasViewAngleGet() &&
						!mos.zoomIntoGet() &&
						mos.linkViewObjectGet() == null) {
						DestroyImmediate (mos);
						changed ();
						nempty++;
					}
				}
				Debug.Log ("#moss=" + moss.Length + " nempty=" + nempty);
			}
			if (GUILayout.Button ("Remove all Tags from VMObjects", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
				VMObject[] vmos = FindObjectsOfType<VMObject> ();
				int ntagsremoved = 0;
				foreach (VMObject vmo in vmos) {
					GameObject go = vmo.gameObject;
					if (go.tag != "" && go.tag != "Untagged") {
						go.tag = "Untagged";
						vmo.changed ();
						ntagsremoved++;
					}
				}
				Debug.Log ("#vmos=" + vmos.Length + " ntagsremoved=" + ntagsremoved);
				//Debug.Log ("name=" + pos.gameObject.name);
			}
#endif
            if (ViewManager.distortionCorrectionFunc != null)
            {
                if (GUILayout.Button("Show Distortion Vectors", new GUILayoutOption[] { GUILayout.Width(200.0f) }))
                {
                    float width = ViewManager.getCurrentCamera().pixelWidth;
                    float height = ViewManager.getCurrentCamera().pixelHeight;
                    float hwidth = width / 2.0f, hheight = height / 2.0f;

                    Color col = Color.white;
                    GameObject newgo = GameObject.Find("distortion-vectors");
                    if (newgo == null)
                    {
                        newgo = new GameObject("distortion-vectors");
                        newgo.AddComponent<MeshFilter>();
                        newgo.AddComponent<MeshRenderer>();
                        if (vm.screenCanvas != null)
                            newgo.transform.SetParent(vm.screenCanvas.transform);
                        newgo.transform.localRotation = Quaternion.identity;
                        newgo.transform.localScale = Vector3.one;
                        newgo.transform.localPosition = Vector3.zero;
                    }
                    MeshFilter mf = newgo.GetComponent<MeshFilter>();
                    Mesh mesh = mf.mesh;
                    MeshRenderer mr = newgo.GetComponent<MeshRenderer>();
                    Material material = mr.material;
                    mesh.Clear();
                    List<Vector3> ptlist = new List<Vector3>();
                    List<Color32> collist = new List<Color32>();
                    List<int> tris = new List<int>();

                    float startf = .2f, endf = .81f, incrf = .02f; ;
                    for (float fU = startf; fU <= endf; fU += incrf)
                    {
                        for (float fV = startf; fV <= endf; fV += incrf)
                        {
                            Vector2 vect = 20.0f * ViewManager.distortionCorrectionFunc.call(fU, fV);
                            if (vect.magnitude > 1.0f)
                            {
                                Vector2 vectnorm = vect.normalized;
                                float cx = (fU * width) - hwidth, cy = (fV * height) - hheight;
                                Vector3 pt1 = new Vector3(cx, cy, 0.0f);
                                Vector3 pt2 = new Vector3(cx + vect.x, cy + vect.y, 0.0f);
                                Vector2 vectperp = Quaternion.Euler(0, 0, 90.0f) * vectnorm;
                                Vector3 vectperp3 = new Vector3(vectperp.x, vectperp.y, 0.0f);
                                int pl = ptlist.Count;
                                tris.Add(pl); tris.Add(pl + 1); tris.Add(pl + 2);
                                tris.Add(pl + 2); tris.Add(pl + 3); tris.Add(pl);
                                ptlist.Add(pt1 + vectperp3);
                                ptlist.Add(pt2 + vectperp3);
                                ptlist.Add(pt2 - vectperp3);
                                ptlist.Add(pt1 - vectperp3);
                                collist.Add(Color.white); collist.Add(Color.white);
                                collist.Add(Color.white); collist.Add(Color.white);

                                pl = ptlist.Count;
                                Vector3 vect3d = new Vector3(vectnorm.x, vectnorm.y, 0.0f);
                                tris.Add(pl); tris.Add(pl + 1); tris.Add(pl + 2);
                                tris.Add(pl + 2); tris.Add(pl + 3); tris.Add(pl);
                                float hwid = 2.5f;
                                ptlist.Add(pt1 + hwid * vectperp3 - vect3d * hwid);
                                ptlist.Add(pt1 + hwid * vectperp3 + vect3d * hwid);
                                ptlist.Add(pt1 - hwid * vectperp3 + vect3d * hwid);
                                ptlist.Add(pt1 - hwid * vectperp3 - vect3d * hwid);
                                collist.Add(Color.red); collist.Add(Color.red);
                                collist.Add(Color.red); collist.Add(Color.red);
                                //nlines++;
                            }
                        }
                    }
                    //Debug.Log ("nlines=" + nlines);
                    mesh.vertices = ptlist.ToArray();
                    mesh.triangles = tris.ToArray();
                    mesh.colors32 = collist.ToArray();

                    material.shader = Shader.Find("Unlit/Color");
                }
            }
#if DONOTSHOW
            if (GUILayout.Button("Check ViewManagers", new GUILayoutOption[] { GUILayout.Width(200.0f) }))
            {
                ViewManager[] vms = FindObjectsOfType<ViewManager>();
                Debug.Log("#vms=" + vms.Length);
            }
				if (GUILayout.Button ("Create BBX of Selected For Each", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
					foreach (UnityEngine.Object ob in gos) {
						int nverts = 0;
						List< KeyValuePair<MeshFilter,Mesh> > meshList = new List< KeyValuePair<MeshFilter,Mesh> >() ;
						GameObject go = (GameObject)ob;
						VMObject.GetMeshListForObject (meshList, go, ref nverts);
						Bounds bounds = VMObject.GetBoundsFromMeshListBounds (meshList, null);
						Debug.Log ("#game objects=" + gos.Length + " bounds=" + bounds + " meshList.Count=" + meshList.Count + " nverts=" + nverts);
						// Creating Object/Cube with size of the bounds of the selected objects
						GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						cube.name = go.name + "_LOD1";
						cube.transform.position = bounds.center;
						cube.transform.localScale = bounds.size;
					}
				}
				if (GUILayout.Button("Create Parent GameObjects For", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
					foreach (UnityEngine.Object ob in gos) {
						GameObject go = (GameObject)ob;
						GameObject newgo = new GameObject (go.name + "_Parent");
						newgo.transform.localRotation = Quaternion.identity;
						newgo.transform.localScale = Vector3.one;
						newgo.transform.localPosition = Vector3.zero;
						go.transform.SetParent (newgo.transform);
					}
				}
				if (GUILayout.Button ("Turn off shadows for all", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
					foreach (UnityEngine.Object ob in gos) {
						GameObject go = (GameObject)ob;
						MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer> ();
						foreach (MeshRenderer mr in mrs) {
							mr.shadowCastingMode =  ShadowCastingMode.Off;
							mr.receiveShadows = false;
						}
					}
				}
			}
		}

		if (GUILayout.Button ("Turn off all box colliders", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
			VMObject[] vmos = FindObjectsOfType<VMObject> ();
			Debug.Log ("#vmos=" + vmos.Length);
			VMObject fvmo = vmos [0];
			GameObject rootgo = fvmo.gameObject.transform.root.gameObject;
			Debug.Log ("root=" + rootgo.name);
			Scene s = rootgo.scene;
			GameObject [] rootgos = s.GetRootGameObjects ();
			string res = "#root game objects=" + rootgos.Length + "\n";
			foreach (GameObject rootgox in rootgos) {
				res += "\t" + rootgox.name + "\n";
				BoxCollider[] bcs = rootgox.GetComponentsInChildren<BoxCollider> (true);
				res += "\t\t#boxcolliders=" + bcs.Length + "\n";
				foreach (BoxCollider bc in bcs) {
					bc.enabled = false;
				}
			}
			Debug.Log (res);
		}

		if (GUILayout.Button ("Make textures smaller", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
			Debug.Log ("Make Textures Smaller");
			string [] assets = AssetDatabase.GetAllAssetPaths ();
			HashSet<Type> typeset = new HashSet<Type> ();
			string cpstrs = "cpstrs:\n";
			foreach (string asset in assets) {
				Type t = AssetDatabase.GetMainAssetTypeAtPath (asset);
				if (t == typeof(Texture2D)) {
					Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath (asset, t);
					int texw = tex.width, texh = tex.height;
					int texm = Math.Max (texw, texh);
					if (texm > 128) {
						int texn = Math.Min (256, texm >> 2);
						float perc = 100.0f*texn / texm;
						cpstrs += "convert -resize " + perc + "%x" + perc + "% \"" + asset + "\" \"" + asset + "\"\n";
					}
				} else {
					typeset.Add (t);
				}
			}
			Debug.Log (cpstrs);
            }
        }
			if (GUILayout.Button ("Change always facing to true", new GUILayoutOption [] { GUILayout.Width (200.0f) })) {
				VMObject[] vmos = FindObjectsOfType<VMObject> ();
				int nchanged = 0;
				foreach (VMObject vmo in vmos) {
					if (!vmo.alwaysFacing){
						nchanged++;
						vmo.alwaysFacing = true;
					}
				}
				Debug.Log("#vmos=" + vmos.Length + " nchanged=" + nchanged);
			}
#endif
        }
        GUILayout.FlexibleSpace();
        GUILayout.Space(100.0f);
        EditorGUILayout.EndScrollView();
        if (requestLicense) {
            vm.RequestAndCheckLicense();
        }
    }
    public Dictionary<string, bool> tagvals = new Dictionary<string, bool>();

	void OnLostFocus ()
	{
		//		Debug.Log ("OnLostFocus called");
	}

	public static ViewManagerScript Instance
	{
		get { return GetWindow< ViewManagerScript >(); }
	}
    public static void SetWindowDirty()
    {
        ViewManagerScript vms = GetWindow<ViewManagerScript>();
        if (vms != null)
            EditorUtility.SetDirty(vms);
    }
	public static void changed(){
#if UNITY_EDITOR
		if (VMObjectEditor.currentEditor != null && VMObjectEditor.currentEditor.target!=null) {
			EditorUtility.SetDirty (VMObjectEditor.currentEditor.target);
			if (!Application.isPlaying)
				UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
		}
#endif
	}
}

#endif
