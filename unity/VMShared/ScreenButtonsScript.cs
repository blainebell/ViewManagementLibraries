using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScreenButtonsScript : ScreenButtonsImpl {
	public bool showOtherLabels = true, showAngleOption = true;

	public override void addScreenMenu(int nindent, int level, string menuItemStr, string actionCmdStr, string objectName){
		ScreenButtonsScript.ScreenButtonMenuItem mi = new ScreenButtonsScript.ScreenButtonMenuItem ();
		mi.indent = nindent;
		mi.level = level;
		mi.itemString = menuItemStr;
		mi.action = actionCmdStr;
		mi.objectName = objectName;
		orderToIndentMenuItemMax++;
		orderToIndentMenuItem [orderToIndentMenuItemMax] = mi;
	}
    public override void addScreenMenuWithUID(string uid, int nindent, int level, string menuItemStr, string actionCmdStr, string objectName) {
        ScreenButtonsScript.ScreenButtonMenuItem mi = new ScreenButtonsScript.ScreenButtonMenuItem();
        mi.indent = nindent;
		mi.level = level;
		mi.itemString = menuItemStr;
		mi.action = actionCmdStr;
		mi.objectName = objectName;
		orderToIndentMenuItemByUID [uid] = mi;
    }
    public override void removeScreenMenuWithUID(string uid)
    {
        orderToIndentMenuItemByUID.Remove(uid);
    }

    Popup popup, popup2 ;

	GUIStyle listStyle = new GUIStyle();
	GUIStyle labelBG = new GUIStyle();

	void Start(){
		popup = new Popup ();
		popup.SetSelectedItemIndex (2);
		popup2 = new Popup ();
		popup2.SetSelectedItemIndex (1);
		//listStyle.normal.textColor = Color.white;
		var tex = new Texture2D(2, 2);
		var colors = new Color[4];
		for (int i=0; i<4; i++)
			colors[i] = Color.white;
		tex.SetPixels(colors);
		tex.Apply();
		listStyle.hover.background = tex;
		listStyle.onHover.background = tex;
		listStyle.padding.left = listStyle.padding.right = listStyle.padding.top = listStyle.padding.bottom = 4;
		if (itemCanvas!=null)
			itemCanvas.gameObject.SetActive (true);
		labelBG.normal.textColor = Color.white;
		labelBG.alignment = TextAnchor.MiddleCenter;

//		GameObject topGO = GameObject.Find (topLevelObjectName);
//		VMObject vmTopGO = topGO.GetComponent < VMObject> ();
//		int topLevelObjectID = vmTopGO.GetVMInstanceID ();
		DPManagerScript.execute (new DPActionFunc {
			actionPerformedFunc = () => StartupAndMoveToTop()
		});
	}
	void StartupAndMoveToTop(){
		if (topLevelObjectName != null && topLevelObjectName.Length > 0) {
			GameObject topObject = GameObject.Find (topLevelObjectName);
			VMDimension vmd = topObject.GetComponent<VMDimension> ();
			int objID = -1;
			VMObject vmo = topObject.GetComponent<VMObject> ();
			if (vmo != null)
				objID = vmo.GetVMInstanceID ();
			if (vmd != null) {
				DPManagerScript.Call_i_s_i ("UnityCoordinateSystem-map", vmd.GetVMDimensionID (), topLevelObjectName, objID);
			} else {
				DPManagerScript.Call_i_s_i ("UnityCoordinateSystem-map", 0, topLevelObjectName, objID);
			}
			//DPManagerScript.Call_i_s_i ("UnityCoordinateSystem-map", 0, "world", -1);
			DPUtils.TurnAllToStillLayout ();
			Invoke ("MoveToTop", 1.0f);
		} else {
		}
	}
	void MoveToTop () {
		AnimateCameraScript acs = FindObjectOfType<AnimateCameraScript> ();
		acs.duration = 2.0f;
		GotoTopLevelWithIndex (2);
		acs.duration = .5f;
		if (rotateWhenMovedToTopLevel) {
			MouseScript ms = FindObjectOfType<MouseScript> ();
			ms.rotateAfterAnimation ();
		}
	}
	public class ScreenButtonMenuItem {
		public int indent;
		public int level;
		public string itemString;
		public string action;
		public string objectName;
        public void press(MouseScript ms) {
            GameObject selgo = GameObject.Find(objectName);
            if (selgo != null)
            {
                VMDimension vmd = selgo.GetComponent<VMDimension>();
                if (vmd != null)
                {
                    ms.turnOffAndClearAllGameObjectsOn();
                    ms.turnOnAllGameObjectsOnAndCheck(vmd, true);
                }
            }
            ms.rotateAfterAnimationIfAlreadyAnimating();
            ViewManager vm = FindObjectOfType<ViewManager>();
            //Debug.Log("Item Clicked: objectName='" + mitem.objectName + "' mi.action='" + mitem.action + "'" );
            DPManagerScript.Call_s("Set-Rotation-Position-In-MouseScript-map", objectName);
            DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "use-current-vector-for-next", "uid", "main-uid", vm.useCurrentVectorForMenu);
            DPManagerScript.Call_noargs("Reset-Labels-In-CoordinateSystem-To-Show-All-If-map");
            DPManagerScript.Call_s("CreateInstancesFromTableMappings-From-String-Replace-Tabs-map", action);
        }
    }

    public SortedDictionary<int, ScreenButtonMenuItem> orderToIndentMenuItem = new SortedDictionary<int, ScreenButtonMenuItem>();
    public SortedDictionary<string, ScreenButtonMenuItem> orderToIndentMenuItemByUID = new SortedDictionary<string, ScreenButtonMenuItem>();

    public string topLevelName;
	public string topLevelObjectName;
	//public string topLevelChildrenTag;

	public bool rotateWhenMovedToTopLevel = false;
	public int orderToIndentMenuItemMax = 1;
	public List<Rect> allRects = new List<Rect> ();
	public SortedDictionary<int, Rect> allPermanentRects = new SortedDictionary<int, Rect> ();
	public ItemCanvasScript itemCanvas;
	public GameObject itemCanvasObjectPanel;
	public WorldToScreenAnimationScript currentProductAnimation;
	public ShoppingCartScript shoppingCart;
	public CheckoutScript checkoutScript;
	Rect rotateRect = new Rect (20, 20, 100, 20);
	Rect otherLabelsLabelRect = new Rect (120, 20, 80, 20);
	Rect otherLabelsRect = new Rect (200, 20, 100, 20);
	Rect mallRect = new Rect (20, 40, 80, 20);
	Rect popupRect = new Rect (100, 40, 60, 20);

	public int m_otherObjectsLabeledMode = 1;
	public void otherObjectsLabeledModeChanged(int value){
		m_otherObjectsLabeledMode = value;
		DPManagerScript.Call_b_b_b ("Set-Other-Objects-Labeled-Mode-map", m_otherObjectsLabeledMode == 0, m_otherObjectsLabeledMode == 1, m_otherObjectsLabeledMode == 2);
//		DPManagerScript.Call_s_s_s_s_b ("SetDataValue-If-Exists-map", "UnityMain", "label-other-objects", "uid", "main-uid", value);
//		DPManagerScript.Call_noargs ("Set-To-All-New-Layouts-map");
//		DPManagerScript.Call_noargs ("SetupStillView-From-Last-map");
	}
	public int otherObjectsLabeledMode {
		set {
			if (value != m_otherObjectsLabeledMode) {
				otherObjectsLabeledModeChanged(value);
			}
		}
		get { return m_otherObjectsLabeledMode; }
	}

	public Vector3 [] closestFromVector;
	public VMObject [] closestVMObject;
	public VMConnector [] closestVMConnector;

	public int numberOfClosestFromVector(){
		return Math.Min (closestFromVector.Length, closestVMObject.Length);
	}

	public bool isInsideRect(float x, float yarg){
		float ph = ViewManager.getCurrentCamera().pixelHeight;
		float y = ph - yarg;
		foreach (Rect r in allRects) {
			if (x >= r.xMin && y >= r.yMin &&
			    x <= r.xMax && y <= r.yMax) {
				return true;
			}
		}
		foreach (Rect r in allPermanentRects.Values) {
			if (x >= r.xMin && yarg >= r.yMin &&
				x <= r.xMax && yarg <= r.yMax) {
				return true;
			}
		}
		return false;
	}
	public bool isInsidePermanentRect(float x, float yarg){
		float ph = ViewManager.getCurrentCamera().pixelHeight;
		foreach (Rect r in allPermanentRects.Values) {
			if (x >= r.xMin && yarg >= r.yMin &&
				x <= r.xMax && yarg <= r.yMax) {
				return true;
			}
		}
		return false;
	}
	public void GotoTopLevel(int toVal){
		ViewManager vm = FindObjectOfType<ViewManager> ();
		vm.doNotMoveWhenCloseToEdge = false;

		MouseScript ms = FindObjectOfType<MouseScript> ();
		ms.turnOffAndClearAllGameObjectsOn ();

		DPManagerScript.Call_noargs("Unset-LastCmd-ObjName-map");
		DPManagerScript.Call_noargs ("Reset-Labels-In-CoordinateSystem-To-Show-All-If-map");
		DPManagerScript.Call_s_i_i ("SetupStillView-Towards-With-Index-Vector-After-Animation-map", topLevelObjectName, toVal, 0);
	}

	public override void GotoTopLevelWithIndex(int toVal){
		MouseScript ms = FindObjectOfType<MouseScript> ();
		GameObject selgo = GameObject.Find(topLevelObjectName);
		DPUtils.TurnAllOff (false);
		if (selgo!=null){
			VMDimension vmd = selgo.GetComponent<VMDimension>();
			if (vmd!=null){
				ms.turnOffAndClearAllGameObjectsOn ();
				ms.turnOnAllGameObjectsOnAndCheck (vmd, true);
			}
		}
		ViewManager vm = FindObjectOfType<ViewManager>();
		DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "use-current-vector-for-next", "uid", "main-uid", vm.useCurrentVectorForMenu);
		DPManagerScript.Call_noargs ("Reset-Labels-In-CoordinateSystem-To-Show-All-If-map");
		GotoTopLevel (toVal);
		DPManagerScript.Call_s_s_s_s_s("SetDataValue-map", "UnityMain", "last-cmd-obj-name", "uid", "main-uid", topLevelObjectName);
		DPManagerScript.Call_noargs("Set-Should-Report-Labels-Not-Shown-In-First-CoordinateSystem-map");
	}

	void OnGUI () {
		allRects.Clear ();
		allRects.Add(popupRect);
		allRects.Add(rotateRect);
		allRects.Add (otherLabelsLabelRect);
		allRects.Add (otherLabelsRect);

		MouseScript ms = FindObjectOfType<MouseScript> ();
		if(GUI.Button(rotateRect, ms.getRotatingString())) {
			// need to set rotationPosition in MouseScript to center of UnityMain.last-cmd-obj-name
			ms.isRotating = !ms.isRotating;
		}

		if (showOtherLabels) {
			GUI.Label (otherLabelsLabelRect, "Other Labels:", labelBG);
			otherObjectsLabeledMode = popup2.List (otherLabelsRect, DPUtils.TraverseVals, GUIStyle.none, listStyle, allRects);
		}
        if (topLevelName != null && topLevelName.Length > 0)
        {
            int toVal = 2; // angle
    		if (showAngleOption) {
	    		toVal = popup.List (popupRect, DPUtils.ContentVals, GUIStyle.none, listStyle, allRects);
		    }
		    allRects.Add (mallRect);
            if (GUI.Button(mallRect, topLevelName))
            {
                ms.rotateAfterAnimationIfAlreadyAnimating();
                DPManagerScript.Call_s("Set-Rotation-Position-In-MouseScript-map", topLevelObjectName);
                GotoTopLevelWithIndex(toVal);
            }
        }
		int plx = 20, ply = 60;
        string[] alluids = new string[orderToIndentMenuItemByUID.Count];
        orderToIndentMenuItemByUID.Keys.CopyTo(alluids, 0);
        foreach (string miduid in alluids)
        {
            try
            {
                ScreenButtonMenuItem mitem = orderToIndentMenuItemByUID[miduid];
                Rect nrect = new Rect(plx + 20 * mitem.indent, ply, 80, 20);
                allRects.Add(nrect);
                if (GUI.Button(nrect, mitem.itemString))
                {
                    mitem.press(ms);
                }
                ply += 20;
            }
            catch { }
        }

        int[] allkeys = new int[orderToIndentMenuItem.Count];
		orderToIndentMenuItem.Keys.CopyTo (allkeys, 0);
		foreach (int mid in allkeys) {
			try {
				ScreenButtonMenuItem mitem = orderToIndentMenuItem [mid];
				Rect nrect = new Rect (plx + 20 * mitem.indent, ply, 80, 20);
				allRects.Add(nrect);
				if (GUI.Button (nrect, mitem.itemString)) {
                    mitem.press(ms);
				}
				ply += 20;
            }
            catch { }
        }
	}
	public override void removeMenuGreaterThanOrEqualTo(int int_arg){
		List<int> idxtoremove = new List<int> ();
		foreach (int key in orderToIndentMenuItem.Keys) {
			ScreenButtonsScript.ScreenButtonMenuItem mi = orderToIndentMenuItem[key];
			if (mi.level >= int_arg)
				idxtoremove.Add (key);
		}
		foreach (int key in idxtoremove) {
			orderToIndentMenuItem.Remove (key);
		}
	}
	public override void clearOrderToIndentMenuItem(){
		orderToIndentMenuItem.Clear ();
	}
	public override void addPermanentRect (int key, Rect rct){
		allPermanentRects.Add (key, rct);
	}
	public override void removePermanentRect (int insid){
		if (allPermanentRects.ContainsKey (insid)) {
			allPermanentRects.Remove (insid);
		}
	}
	public override VMObject getClosestVMObject (out Vector3 connectorCentroid, out bool connectorCentroidIsSet){
		GameObject topObject = GameObject.Find (topLevelObjectName);
		connectorCentroidIsSet = false;
		connectorCentroid = new Vector3 ();
		if (topObject != null) {
			Renderer rend = topObject.GetComponent<Renderer> ();
			Bounds bound = rend.bounds;
			Vector3 pos = ViewManager.getCurrentCamera().transform.position;
			//Debug.Log ("eyepos=" + pos + " bounds=" + bound + " contains=" + bound.Contains (pos));
			if (!bound.Contains (pos)) {
				Vector3 vect = (pos - bound.center).normalized;
				int closestVector = -1;
				float closestAngle = float.MaxValue;
				int nvectors = numberOfClosestFromVector();

				for (int pl = 0; pl < nvectors; pl++) {
					float ang = Vector3.Angle (vect, closestFromVector[pl]);
					if (ang < closestAngle) {
						closestVector = pl;
						closestAngle = ang;
					}
				}
				if (closestAngle < 45.0f) {
					VMConnector vmc = closestVMConnector [closestVector];
					if (vmc != null) {
						Renderer r = vmc.GetComponent<Renderer> ();
						if (r != null) {
							connectorCentroid = r.bounds.center;
						}
						connectorCentroidIsSet = (r != null);
					}
					return closestVMObject [closestVector];
				}
			}
		}
		return null;
	}
	public void changed(){
		#if UNITY_EDITOR
		if (VMObjectEditor.currentEditor != null && VMObjectEditor.currentEditor.target!=null) {
			EditorUtility.SetDirty (VMObjectEditor.currentEditor.target);
			if (!Application.isPlaying)
				UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
		}
		#endif
	}

}
