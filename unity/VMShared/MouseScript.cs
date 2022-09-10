#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MouseScript : MouseScriptImpl {
    public Transform controlledTransform;
    public Transform getControlledTransform()
    {
        if (controlledTransform != null)
            return controlledTransform;
		Camera cam = ViewManager.getCurrentCamera ();
		if (cam != null) {
			return cam.transform;
		}
		return null;
    }

	public bool labelsAreClickable = true;
    void Start () {
		DPManagerScript.execute (new DPActionFunc () {
			actionPerformedFunc = () => DPManagerScript.Call_s_s_s_s_b ("SetDataValue-map", "UnityMain", "should-populate-all-screen-placements-Manual", "uid", "main-uid", true)
		});
	}
	Vector3 currentMousePosition = new Vector3 (-100, -100, 0);
	public override void clearCurrentMousePosition(){
		currentMousePosition = new Vector3 (-100, -100, 0);
	}
	Ray ray;
	RaycastHit hit;
	RaycastHit [] hits;
	int mouseOnID = 0;
	VMObject mouseOnVMObject = null;
	public void mouseOnVMObjectSet(VMObject vmo)
	{
	    mouseOnVMObject = vmo;
	}
	WorldToScreenAnimationScript currentW2sa = null;

	string rotatingString = "Rotate";
	public string getRotatingString(){
		return rotatingString;
	}
	public void isRotatingChanged(bool val){
		DPManagerScript.Call_b ("Set-Can-Show-More-Button-map", !val);
		if (val) {
			DPManagerScript.Call_noargs ("Set-Rotation-Position-For-MouseScript-map");
			rotatingString = "Stop Rotating";
			DPManagerScript.execute (new DPActionFunc () {
				actionPerformedFunc = () => DPManagerScript.Call_noargs("Set-To-Flyover-Layout-For-All-Traversing-map")
				//				actionPerformedFunc = () => DPUtils.setToFlyoverLayoutForAll ()
			});
			ViewManager vm = FindObjectOfType<ViewManager> ();
			vm.doNotMoveWhenCloseToEdge = true;

//			if (currentDimension != null) {
//				MouseOverScript mos = currentDimension.gameObject.GetComponent<MouseOverScript> ();
//				if (mos!=null)
//					Debug.Log ("mos.zoomInto=" + mos.zoomInto);
				// need to check bounds and rotate around center at min dimension distance 
//			}
		} else {
			rotatingString = "Rotate";
			DPUtils.TurnAllToStillLayout ();
			ViewManager vm = FindObjectOfType<ViewManager> ();
			vm.traverseNextFrame = true;
			//DPManagerScript.Call_noargs ("Set-To-All-New-Layouts-map");
			DPManagerScript.Call_s_s_b("SetSlotInAllInstancesTo-map", "UnityObject", "previous-annotation-projection-is-set", false);
			DPManagerScript.Call_s_s_b("SetSlotInAllInstancesTo-map", "UnityObject", "placement-changed-for-animation", true);
			DPManagerScript.Call_b ("Do-All-Annotation-Placements-In-Traversed-CoordinateSystems-map", true);
		}
	}

	public bool isRotating {
		set {
			if (value != m_isRotating) {
				m_isRotating = value;
				isRotatingChanged(value);
			}
		}
		get { return m_isRotating; }
	}
	public override bool getIsRotating(){
		return m_isRotating;
	}

	public void unsetIf(WorldToScreenAnimationScript w2sa){
		if (w2sa == currentW2sa)
			currentW2sa = null;
	}
		
	List<GameObject> allGameObjectsOn = new List<GameObject>();
	List<GameObject> allGameObjectsTransparent = new List<GameObject>();
    List<string> menuItemUIDsAdded = new List<string>();
	public bool m_turnOnLights = false;
	public void turnOnLightsChanged(bool val){
		if (val) {
			if (currentDimension != null) {
				turnOnAllGameObjectsOnAndCheck (currentDimension, true);
			}
		} else {
			turnOffAndClearAllGameObjectsOn ();
		}
	}

	public bool turnOnLights {
		set {
			if (value != m_turnOnLights) {
				m_turnOnLights = value;
				turnOnLightsChanged(value);
			}
		}
		get { return m_turnOnLights; }
	}

	public bool stopRotatingOnLabelClick = false;

	VMDimension currentDimension = null;
	public VMDimension currentDimensionGet()
	{
		return currentDimension;
	}
	public void turnOffAndClearAllGameObjectsOn(){
		foreach (GameObject goo in allGameObjectsOn) {
			goo.SetActive (false);
		}
		foreach (GameObject goo in allGameObjectsTransparent) {
			goo.SetActive (true);
/*			Renderer [] rend = goo.GetComponentsInChildren<Renderer> ();
			foreach (Renderer r in rend) {
				r.material.color = new Color (r.material.color.r, r.material.color.g, r.material.color.b, 1.0f);
			}*/
		}
        ScreenButtonsImpl sbi = FindObjectOfType<ScreenButtonsImpl>();
        foreach (string uid in menuItemUIDsAdded)
        {
            sbi.removeScreenMenuWithUID(uid);
        }
        menuItemUIDsAdded.Clear();
        allGameObjectsOn.Clear ();
		allGameObjectsTransparent.Clear();
	}
	public void turnOnAllGameObjectsOnAndCheck (VMDimension vmdim, bool callRelayout)
    {
		turnOnAllGameObjectsOn (vmdim);
		if (labelsAreShown && !labelsForObjectsAreShown) {
			changeShowingObjectLabels (false, callRelayout);
		}
	}		
	public void turnOnAllGameObjectsOn (VMDimension vmdim){
		if (vmdim != null){
			if (turnOnLights) {
				foreach (GameObject goo in vmdim.allTurnOnWhenZoomedInto) {
					goo.SetActive (true);
					allGameObjectsOn.Add (goo);
				}
			}
			foreach (GameObject goo in vmdim.allTurnTransparentWhenZoomedInto) {
				goo.SetActive (false);
/*				Renderer [] rend = goo.GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in rend) {
					r.material.color = new Color (r.material.color.r, r.material.color.g, r.material.color.b, .5f); //r.material.color.a);
				}*/
				allGameObjectsTransparent.Add (goo);
			}
            foreach (VMDimension vmd in vmdim.allMenuItemLinks)
            {
                ScreenButtonsImpl sbi = FindObjectOfType<ScreenButtonsImpl>();
                GameObject vmdgo = vmd.gameObject;
                VMObject vmo = vmdgo.GetComponent<VMObject>();
                bool zoomInto = false;
                MouseOverScript mos2 = vmdgo.GetComponent<MouseOverScript>();
                if (mos2 != null)
                {
                    zoomInto = mos2.zoomInto;
                }
                menuItemUIDsAdded.Add(vmdgo.name);

                bool useVect = false;
                Vector3 vN = mos2.viewNormal;
                mos2.getViewNormal(out useVect, out vN);

                string cmd = "SetupStillView-Towards-With-Vector-And-Coordinate-System-Except-After-Animation-Str-Clear-map\t" + vmdgo.name + "\t" +
                    vmo.GetVMInstanceID().ToString() + "\t" + vmd.level.ToString() + "\t" +
                    "(" + vN.x + "," + vN.y + "," + vN.z + ")" + "\t" + 
                    vmd.GetInstanceID() + "\t" + zoomInto.ToString();
                Debug.Log("adding cmd for menu item: " + cmd);
                sbi.addScreenMenuWithUID(vmdgo.name, 0, 1, vmo.textForLabel, cmd, vmdgo.name);
            }
		}
		currentDimension = vmdim;
		MouseOverScript mos = null;
		if (currentDimension!=null && currentDimension.gameObject!=null)
			mos = currentDimension.gameObject.GetComponent<MouseOverScript> ();
		if (mos != null) {
			bool multViewNormals = mos.viewNormalForFromVector.Length > 1;
			DPManagerScript.Call_s_s_s_s_b ("SetDataValue-map", "UnityMain", "current-dimension-has-multiple-view-normals", "uid", "main-uid", multViewNormals);
		}
	}

	public void moreButtonPressed(){
		DPManagerScript.Call_noargs ("Filter-Labels-On-Report-map");
		DPManagerScript.Call_s_s_s_s_b ("SetDataValue-map", "UnityMain","do-not-change-view", "uid", "main-uid", true);
		DPUtils.setFadeTimeForAllWithReset (0.2f, 1.0f);
		DPManagerScript.Call_noargs ("SetupStillView-From-Last-map");
	}
	public void backButtonPressed(){
		DPManagerScript.Call_noargs ("Back-Button-Pressed-map");
	}
	public void nextButtonPressed(){
		nextPreviousButtonPressed (1);
	}
	public void previousButtonPressed(){
		nextPreviousButtonPressed (-1);
	}
	public void nextPreviousButtonPressed(int incr){
		MouseOverScript mos = currentDimension.gameObject.GetComponent<MouseOverScript> ();
		if (mos != null) {
			bool useViewNormal = false;
			int viewNormalIdx = -1;
			Vector3 viewNormal = mos.viewNormal;
			if ((mos.numberOfViewNormalsFromVector ()) > 0) {
				useViewNormal = mos.getClosestViewNormalIdx (ViewManager.getCurrentCamera().transform.forward, out viewNormalIdx);
			}
			if (useViewNormal) {
				int newView = viewNormalIdx + incr;
				if (newView < 0)
					newView += mos.viewNormalForFromVector.Length;
				viewNormal = mos.viewNormalForFromVector [newView % mos.viewNormalForFromVector.Length];
				float[] viewNormalArr = new float[] { viewNormal.x, viewNormal.y, viewNormal.z };
				int whichBSPTree = 0;
				GameObject go = null;
				if (currentDimension != null) {
					go = currentDimension.gameObject;
					whichBSPTree = currentDimension.GetInstanceID ();
				}
				int level = currentDimension.level;
				int objID = -1;
				if (go != null) {
					VMObject vmo = go.GetComponent<VMObject> ();
					if (vmo != null) {
						objID = vmo.GetVMInstanceID ();
					}
				}
				AnimateCameraScript acs = FindObjectOfType<AnimateCameraScript> ();
				//acs.setTryToRotateAfterAnimation(isRotating); stop rotating when next/previous pressed
				acs.setShouldExecuteViewManagementAfterAnimation(true);
				DPManagerScript.Call_noargs ("Reset-Labels-In-CoordinateSystem-To-Show-All-If-map");
				DPManagerScript.Call_s_i_i_p3_i_b ("SetupStillView-Towards-With-Vector-And-Coordinate-System-Except-After-Animation-map",
					go.name, objID, level,
					viewNormalArr,
					whichBSPTree, mos.zoomInto);
				if (isRotating){
					turnRotateOffTmp = true; // turn off rotation
					DPManagerScript.Call_noargs ("Set-Rotation-Position-For-MouseScript-map");
				}
			}
		} else {
			Debug.Log ("nextButtonPressed: mos=null");
		}
	}

	bool setupAndAnimatePrefabIntoScreenSpace(MouseOverScript mos, ShoppingProduct sp, Renderer rend){
		bool shouldZoomIn = true;
		ScreenButtonsScript sbs = FindObjectOfType<ScreenButtonsScript> ();
		Vector3 pos = rend.bounds.center;
		Quaternion quat = new Quaternion ();
		Bounds bounds = new Bounds();
		if (sp != null && sp.prefabForClick!=null)
			bounds = DPUtils.GetBoundsInGameObject (sp.prefabForClick);
		if (bounds.size.magnitude > 0.0f) {
			sp.prefabForClick.SetActive (false);
			GameObject newGO = (GameObject)GameObject.Instantiate (sp.prefabForClick, pos, quat);
			Vector3 prefabExtents = bounds.extents;
			Vector3 extents3d = rend.bounds.extents;
			float minScale = Mathf.Min (new float[] {
				extents3d.x / prefabExtents.x,
				extents3d.y / prefabExtents.y,
				extents3d.z / prefabExtents.z
			});

			Camera curCamera = ViewManager.getCurrentCamera ();
			Vector3 ls = new Vector3 (minScale, minScale, minScale);
			newGO.transform.localScale = ls;
			WorldToScreenAnimationScript w2sa = newGO.AddComponent<WorldToScreenAnimationScript> ();
			w2sa.startTime = Time.time + Time.smoothDeltaTime;
			w2sa.endTime = w2sa.startTime + 1.0f; // 1 second for now
			w2sa.startPosition = curCamera.WorldToScreenPoint (pos);
			w2sa.startQuat = Quaternion.identity;
			w2sa.positionOffset = -bounds.center;
			w2sa.sp = sp;
			GameObject objPanel = sbs.itemCanvasObjectPanel;
			RectTransform rt = objPanel.GetComponent<RectTransform> ();
			Rect rtrect = DPUtils.GetRectTransformScreenBounds (rt);

			float zval = 1.5f;
			Vector2 center = rtrect.center;
			Vector3 screenPt = new Vector3 (center.x, center.y, zval);
			Vector3 minPt = new Vector3 (rtrect.min.x, rtrect.min.y, zval);
			Vector3 maxPt = new Vector3 (rtrect.max.x, rtrect.max.y, zval);

			w2sa.screenDestination = screenPt;

			float maxExtent = Mathf.Max (new float[] { prefabExtents.x, prefabExtents.y, prefabExtents.z });
			Vector2 endScreenSize = new Vector2 (maxPt.x - minPt.x, maxPt.y - minPt.y);
			float minScreenDim = Mathf.Min (endScreenSize.x, endScreenSize.y);

			float buffer = .9f;
			float val = zval * Mathf.Tan (curCamera.fieldOfView / 2.0f * Mathf.Deg2Rad);
			float height = val / Mathf.Max (curCamera.pixelHeight, curCamera.pixelWidth);
			float endScale = buffer * height * minScreenDim / maxExtent;

			w2sa.startScale = ls;
			w2sa.endScale = new Vector3 (endScale, endScale, endScale);
			w2sa.setMouseScript (this);
			w2sa.rotationAfterSelectedBehavior = sp.rotationAfterSelectedBehavior;
			w2sa.endQuatIsInvCamera = sp.rotatePrefabToLocalCoordinateSystem;
			currentW2sa = w2sa;
			shouldZoomIn = false;
			sbs.currentProductAnimation = w2sa;

			newGO.transform.position = curCamera.ScreenToWorldPoint (w2sa.startPosition) - minScale * bounds.center;
			newGO.transform.localScale = ls;
			newGO.SetActive (true);

			DPUtils.TurnAllToStillLayoutAndOff (true);
		} else {
			Debug.Log ("mos.prefabForClick: MeshRenderer doesn't exist");
		}
		return shouldZoomIn;
	}
	void checkAndSetHighlightedLabelFromMousePosition (bool isShiftKeyDown, bool isLeftMouseClicked){
		int overlapKey = 0;
		VMObject overlapVMObject = null;
		if (!isShiftKeyDown) {
			overlapKey = DPManagerScript.Call_ri_p2_p2 ("Check-Whether-Point-Inside-Screen-Placement-With-Size-map", 
				new float[] { currentMousePosition.x, currentMousePosition.y }, new float[] { 20.0f, 20.0f });
			//Debug.Log ("overlapKey=" + overlapKey);
			if (overlapKey == 0) {
				// if not close to annotation, try ray casting

				ray = ViewManager.getCurrentCamera().ScreenPointToRay (Input.mousePosition);

				hits = Physics.RaycastAll (ray);
				string hitsres = "#Hits=" + hits.Length + "\n";
				foreach (RaycastHit h in hits) {
					hitsres += "\t" + h.ToString () + " name=" + h.collider.name + "\n";
				}
				if (isLeftMouseClicked)
					Debug.Log(hitsres);
				if (Physics.Raycast (ray, out hit)) {
					GameObject go = GameObject.Find (hit.collider.name);
					if (go != null) {
						overlapVMObject = go.GetComponent<VMObject> ();
						if (overlapVMObject != null) {
							overlapKey = overlapVMObject.GetVMInstanceID ();
						}
					}
				}
			} else {
				GameObject go;
				VMObject.m_VMObjects.TryGetValue (overlapKey, out go);
				if (go != null)
					overlapVMObject = go.GetComponent<VMObject> ();
			}
			if (overlapVMObject == null) {
				overlapKey = 0;
			}
		}
		if (mouseOnID != overlapKey) {
			if (mouseOnID != 0) {
				// turn off mouseOn
				mouseOnVMObject.SetAnnotationColor (new Color (1.0f, 1.0f, 1.0f));
			}
			if (overlapKey != 0) {
				overlapVMObject.SetAnnotationColor (new Color (0.0f, 0.0f, 1.0f));
			}
			mouseOnID = overlapKey;
			if (overlapKey != 0)
				mouseOnVMObject = overlapVMObject;
			else
				mouseOnVMObject = null;
		}
	}
	void clickedOnLabel(){
		ViewManager vm = FindObjectOfType<ViewManager> ();
		vm.doNotMoveWhenCloseToEdge = false;
		GameObject go = mouseOnVMObject.gameObject;
		MouseOverScript mos = go.GetComponent<MouseOverScript> ();
		if (mos!=null && mos.linkViewObject != null) {
			mos = mos.linkViewObject;
			go = mos.gameObject;
		}
		int level = -1;
		VMDimension vmdim = go.GetComponent<VMDimension>();
		if (vmdim != null)
		{
		    level = vmdim.level;
		}
		else
		{
		    if (mouseOnVMObject.coordinateSystem != null)
		    {
		        level = mouseOnVMObject.coordinateSystem.level + 1;
		    }
		}
		int objID = -1;
		if (mouseOnVMObject != null)
		{
		    VMObject vmo = go.GetComponent<VMObject>();
		    if (vmo != null)
		    {
		        objID = vmo.GetVMInstanceID();
		    }
		}
		ShoppingProduct sp = go.GetComponent<ShoppingProduct>();
	    clickedOnLabelImpl(mos, sp, go, vmdim, level, objID);
	}
    public void clickedOnLabelImpl(MouseOverScript mos, ShoppingProduct sp, GameObject go, VMDimension vmdim, int level, int objID){
        ViewManager vm = FindObjectOfType<ViewManager>();
        Renderer rend = null;
        if (go != null)
            rend = go.GetComponent<Renderer> ();
		if (rend != null) {
			int whichBSPTree = 0;
			VMDimension vmd = go.GetComponent<VMDimension> ();
			if (vmd != null) {
				whichBSPTree = vmd.GetInstanceID ();
			}

			bool shouldZoomIn = true;
			if (sp!=null && sp.prefabForClick != null) {
				// prefab is set, so create it and animate towards screen-stabilized cart
				shouldZoomIn = setupAndAnimatePrefabIntoScreenSpace(mos, sp, rend);
			}
			if (mos != null && shouldZoomIn) {
				BoxCollider bc = go.GetComponent<BoxCollider> ();
				bc.enabled = false;
				bool useVect = false;
                Vector3 viewNormal = mos.viewNormal;
                mos.getViewNormal(out useVect, out viewNormal);

                if (vmdim!=null && vmdim.nextWhenZoomedTo){
                    vmdim.nextWhenZoomedToOnNextSet(true);
                }
				if (useVect)
					DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "use-current-vector-for-next", "uid", "main-uid", vm.useCurrentVector);
				float[] viewNormalArr = new float[] { viewNormal.x, viewNormal.y, viewNormal.z };
				// first turn off all GameObjects that are on, then turn on ones for this Coordinate System
				turnOffAndClearAllGameObjectsOn ();
				turnOnAllGameObjectsOnAndCheck (vmdim, false);
				AnimateCameraScript acs = FindObjectOfType<AnimateCameraScript> ();
				acs.setShouldExecuteViewManagementAfterAnimation(true);
				acs.setTryToRotateAfterAnimation(isRotating);
				DPManagerScript.Call_noargs ("Reset-Labels-In-CoordinateSystem-To-Show-All-If-map");
				DPManagerScript.Call_s_i_i_p3_i_b ("SetupStillView-Towards-With-Vector-And-Coordinate-System-Except-After-Animation-map", 
					go.name, objID, level,
					viewNormalArr,
					whichBSPTree, mos.zoomInto);
				if (isRotating){
					if (stopRotatingOnLabelClick) {
						isRotating = false;
						acs.setTryToRotateAfterAnimation(false);
					} else {
						turnRotateOffTmp = true; // turn off rotation
						DPManagerScript.Call_noargs ("Set-Rotation-Position-For-MouseScript-map");
					}
				}
			}
		}
	}
	bool checkClickedOnScreenButton(Vector3 mp){
		ScreenButtonsScript sbs = FindObjectOfType<ScreenButtonsScript> ();
        if (sbs == null) {
            return false;
        }
		CheckoutScript checkoutScript = sbs.checkoutScript;
		if (currentW2sa != null || (checkoutScript!=null && checkoutScript.getIsTurnedOn ())) {
			if (!sbs.isInsidePermanentRect (Input.mousePosition.x, Input.mousePosition.y)) {
				if (currentW2sa != null) {
					GameObject.Destroy (currentW2sa.gameObject);
					currentW2sa = null;
				} else {
					checkoutScript.hide ();
					// if shopping cart has something in it, show it
					sbs.shoppingCart.showIfCartIsNotEmpty ();
				}
				if (labelsAreShown) {
					DPUtils.setFadeTimeForAllWithReset (0.2f, 1.0f);
					DPManagerScript.Call_s_s_s_s_b ("SetDataValue-map", "UnityMain", "do-not-change-view", "uid", "main-uid", true);
					DPManagerScript.Call_noargs ("SetupStillView-From-Last-map");
					DPManagerScript.Call_b ("Do-All-Annotation-Placements-In-Traversed-CoordinateSystems-map", false);
				}
				return true;
			}
		}
		if (sbs.isInsideRect (mp.x, mp.y)) {  // mouse is on a screen object, disreguard click
			return true;
		}
		return false;
	}
	bool noInteraction = false;
	public void SetNoInteraction(bool ni){
		noInteraction = ni;
	}
	public void changeShowingObjectLabels(bool show, bool callRelayout){
		if (labelsAreShown && currentDimension!=null && currentDimension.children.Count > 0){
			// should turn off all labels that are associated with objects that are toggled
			bool hasChanged = false;
			int nchanged = 0;
			foreach (VMObject vmo in currentDimension.children) {
				GameObject go = vmo.gameObject;
				VMBehavior vmb = go.GetComponent<VMBehavior> ();
				if (vmb!=null && vmb.isObject) {
					if (vmo.tryToLabel != show) {
						vmo.tryToLabel = show;
						hasChanged = true;
						nchanged++;
					}
				}
			}
			if (callRelayout && hasChanged) {
				// turn labels back on
				DPUtils.setFadeTimeForAllWithReset (0.2f, 1.0f);
				DPManagerScript.Call_s_s_s_s_b ("SetDataValue-map", "UnityMain", "do-not-change-view", "uid", "main-uid", true);
				DPManagerScript.Call_noargs ("SetupStillView-From-Last-map");
				DPManagerScript.Call_b ("Do-All-Annotation-Placements-In-Traversed-CoordinateSystems-map", false);
			}
		}
	}
	bool IsAnimating(){
		AnimateCameraScript acs = FindObjectOfType<AnimateCameraScript> ();
		return acs != null && acs.getIsAnimating ();
	}
	bool labelsAreShown = true, labelsForObjectsAreShown = false;
	public void labelsForObjectsAreShownSet(bool v)
	{
	    labelsForObjectsAreShown = v;
	}
	void Update () {
		bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		if (Input.GetKeyDown ("e")) {
			rotateDirection = !rotateDirection;
		} else if (Input.GetKeyDown ("n")) {
			nextButtonPressed ();
		} else if (Input.GetKeyDown ("b")) {
			previousButtonPressed ();
		} else if (Input.GetKeyDown ("r")) {
			isRotating = !isRotating;
		} else if (Input.GetKeyDown ("a")) {
			ScreenButtonsImpl sbi = FindObjectOfType<ScreenButtonsImpl> ();
			sbi.GotoTopLevelWithIndex (2);
		} else if (Input.GetKeyDown ("l")) {
			// toggle labels for objects on/off, only if labels and furnature are shown
			labelsForObjectsAreShown = !labelsForObjectsAreShown;
			bool furnatureIsShown = allGameObjectsOn.Count > 0;
			if (labelsAreShown && furnatureIsShown) {
				changeShowingObjectLabels (labelsForObjectsAreShown, !IsAnimating());
			}
		} else if (Input.GetKeyDown ("f")) {
			// toggling any game objects that were turned on
			if (currentDimension) {
				bool turnOff = allGameObjectsOn.Count > 0;
				if (turnOff) {
					turnOffAndClearAllGameObjectsOn ();
				} else {
					turnOnAllGameObjectsOn (currentDimension);
				}
				changeShowingObjectLabels (!turnOff, !IsAnimating());
			} else {
				turnOffAndClearAllGameObjectsOn ();
			}
		} else if (Input.GetKey ("down")) {
			backButtonPressed ();
			return;
		} else if (Input.GetKey ("up")) {
			DPManagerScript.Call_noargs ("Forward-Pressed-map");
			return;
		} else if (Input.GetKey ("escape")) {
#if UNITY_EDITOR
				Debug.Log ("EditorApplication.isPlaying = false;");
				EditorApplication.isPlaying = false;
#else
				Debug.Log ("Application.Quit ();");
				Application.Quit ();
#endif
		} else if (Input.GetKeyDown (KeyCode.Space)) { // toggle labels on and off
			if (labelsAreShown) {
				// turn off all labels
				DPUtils.TurnAllOff (false);
			} else {
				DPUtils.setFadeTimeForAllWithReset (0.2f, 1.0f);
				DPManagerScript.Call_s_s_s_s_b ("SetDataValue-map", "UnityMain", "do-not-change-view", "uid", "main-uid", true);
				DPManagerScript.Call_noargs ("SetupStillView-From-Last-map");
				DPManagerScript.Call_b ("Do-All-Annotation-Placements-In-Traversed-CoordinateSystems-map", false);
			}
			labelsAreShown = !labelsAreShown;
		} else if (labelsAreClickable){
			Vector3 mp = Input.mousePosition;
			bool isLeftMouseClicked = !isShiftKeyDown && Input.GetMouseButtonDown (0);
			if (!noInteraction) {
				if (isLeftMouseClicked) {
					// check if ScreenButtonsScript has a button in that place, if so, just disregard
					if (checkClickedOnScreenButton (mp)) {
						return;
					}
                    Debug.Log("mouseOnID: " + mouseOnID);
					if (mouseOnID != 0) {
						clickedOnLabel ();
					}
				} else {
					if (isShiftKeyDown || !currentMousePosition.Equals (mp)) {
						currentMousePosition = mp;
						checkAndSetHighlightedLabelFromMousePosition (isShiftKeyDown, isLeftMouseClicked);
					}
				}
			}
		}
			// check if rotating, if so, then update transform
		if (isRotating && !turnRotateOffTmp && rotationTime > 0.0f) {
            Transform trans = getControlledTransform();
			Vector3 pos = trans.position;
			trans.RotateAround (rotatePoint, Vector3.up, (rotateDirection ? -1.0f : 1.0f) * 360.0f * Time.deltaTime / rotationTime);
		}
	}
	public float rotationTime = 30.0f;  // in seconds
	public bool m_isRotating = false;
	public bool rotateWhenFinishedAnimating = false;
	public bool turnRotateOffTmp = false; // used when animating view and rotate should start back up
	public bool rotateDirection = false;
	public Vector3 rotatePoint = new Vector3 ();
	public override void setRotatePoint(Vector3 rotp){
		rotatePoint = rotp;
	}
	public void rotateAfterAnimationIfAlreadyAnimating(){
		if (isRotating){
			AnimateCameraScript acs = FindObjectOfType<AnimateCameraScript>();
			acs.setTryToRotateAfterAnimation(true);
			turnRotateOffTmp = true;
		}
	}
	public void rotateAfterAnimation(){
		AnimateCameraScript acs = FindObjectOfType<AnimateCameraScript>();
		acs.setTryToRotateAfterAnimation(true);
		turnRotateOffTmp = true;
		rotateWhenFinishedAnimating = true;
		//isRotating = true;
	}
}
