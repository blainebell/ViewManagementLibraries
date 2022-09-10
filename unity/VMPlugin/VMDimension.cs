using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class VMDimension : MonoBehaviour {
	public static Dictionary<int, VMDimension> m_VMDimensions = new Dictionary<int, VMDimension>();
	public int m_level;
	public List<VMObject> children;
	public bool m_overrideShowLevelsAbove;
	public int m_numberOfLevelsAboveToShow;
	public int vmobjectID;
	public GameObject [] allTurnOnWhenZoomedInto;
	public GameObject [] allTurnTransparentWhenZoomedInto;

	public VMDimension [] allMenuItemLinks;
	public GameObject[] addToBBX;
	public bool nextWhenZoomedTo;
	private bool nextWhenZoomedToOnNext;
    public int m_traverseOnStartupMode; // 0 - do not, 1 - "To Only", 2 - "Add to"
    public int m_traverseOnStartupOrder; // only used when Mode is "Add to", m_traverseOnStartupMode == 2

	public bool m_isTopForNavigation;
    public float topY;
	public VMObject [] allVMObjectsToTurnOnForNavigation;

    public bool targetScaleIsSet = true;
    public Vector3 targetScale = new Vector3(1.0f, 1.0f, 1.0f);
    public InterpolateTRSScript target;

    private Matrix4x4 dimToWorld = Matrix4x4.identity;
    public Matrix4x4 dimToWorldGet() { return dimToWorld; }
    public void dimToWorldSet(Matrix4x4 mat) { dimToWorld = mat; }

    // this is the traversal order when not in game mode
    static public SortedDictionary<int, VMDimension> traversalOrder = new SortedDictionary<int, VMDimension>();

    public int GetVMDimensionID(){
		if (m_level==0)
			return 0;
		return GetInstanceID();
	}
	public void AddShowChildrenOnClick (){
		foreach (VMObject vmo in children) {
			vmo.computePlacement ();
			GameObject go = vmo.gameObject;
			BoxCollider bc = go.GetComponent<BoxCollider> ();
			if (bc!=null)
				bc.enabled = true;
		}
	}
	public void setToFlyoverLayoutForAllChildren(){
		foreach (VMObject vmo in children) {
			vmo.setToFlyoverLayoutImpl ();
		}
	}
	public void setToStillLayoutForAllChildren(){
		foreach (VMObject vmo in children) {
			vmo.setToStillLayoutImpl ();
		}
	}
	public void setToRotateLayoutForAllChildren(bool isRotating){
		foreach (VMObject vmo in children) {
			if (isRotating) {
				vmo.setToFlyoverLayoutImpl ();
			} else {
				vmo.setToStillLayoutImpl ();
				vmo.computePlacement ();
			}
		}
	}
	public void AddShowChildrenOnClickExcept(int exceptID){
		foreach (VMObject vmo in children) {
			if (vmo.GetVMInstanceID () != exceptID) {
				vmo.computePlacement ();
				GameObject go = vmo.gameObject;
				BoxCollider bc = go.GetComponent<BoxCollider> ();
				if (bc != null)
					bc.enabled = true;
			}
		}
	}
	public void SetBoxCollidersEnabled (bool en){
		foreach (VMObject vmo in children) {
			GameObject go = vmo.gameObject;
			BoxCollider bc = go.GetComponent<BoxCollider> ();
			bc.enabled = en;
		}
	}

	public void levelChanged(int value){
		bool chg = m_level != value;
		m_level = value;
		if (!DPUtils.isPrefab(gameObject)){
			DPManagerScript.Call_s_s_s_i_i ("SetDataValue-If-Exists-map", "UnityCoordinateSystem", "level", "id", GetInstanceID (), value);
		}
		if (chg)
			changed ();
	}
	public int level {
		set {
			if (value != m_level) {
				levelChanged(value);
			}
		}
		get { return m_level; }
	}


    void OnDestroy(){
        traverseOnStartupModeChanged(0);
    }
    public void traverseOnStartupModeChanged(int value)
    {
        bool chg = m_traverseOnStartupMode != value;
        int prevvalue = m_traverseOnStartupMode;
        m_traverseOnStartupMode = value;
        //if (!DPUtils.isPrefab(gameObject))
        {
            // need to figure out whether
            if (value == 1) { // if set to "To Only", set all to 0
                VMDimension[] alldims = FindObjectsOfType<VMDimension>();
                foreach (VMDimension dim in alldims) {
                    if (dim != this)
                        dim.traverseOnStartupMode = 0;
                }
                if (!Application.isPlaying) {
                    traversalOrder.Clear();
                    traversalOrder.Add(1, this);
                    ViewManager.invalidateView();
                }
                traverseOnStartupOrder = -1;
            } else if (value == 2) { // if "Add to", then if any are set to "To Only", then set it to "Add to" and set order to 1
                int order = 1;
                if (traverseOnStartupOrder <= 0) { // need to set traverseOnStartupOrder
                    VMDimension[] alldims = FindObjectsOfType<VMDimension>();
                    foreach (VMDimension dim in alldims) {
                        if (dim != this) {
                            if (dim.traverseOnStartupMode == 1) { // any "To Only", set order and break
                                dim.traverseOnStartupOrder = 1;
                                order = 2;
                                dim.traverseOnStartupMode = 2;
                                break;
                            }
                            if (dim.traverseOnStartupMode == 2)
                            {
                                if (order <= dim.traverseOnStartupOrder)
                                    order = dim.traverseOnStartupOrder + 1;
                            }
                        }
                    }
                } else {
                    order = traverseOnStartupOrder;
                }
                if (traversalOrder.ContainsKey(order))
                    traversalOrder.Remove(order);
                traversalOrder.Add(order, this);
                traverseOnStartupOrder = order;
                ViewManager.invalidateView ();
            } else { // value 0
                if (prevvalue == 1)
                    traversalOrder.Clear();
                if (prevvalue == 2){
                    VMDimension dim;
                    if (traversalOrder.TryGetValue(traverseOnStartupOrder, out dim))
                    {
                        traversalOrder.Remove(traverseOnStartupOrder);
                    }
                    traverseOnStartupOrder = -1;
                }
                if (chg)
                    ViewManager.invalidateView ();
            }
        }
        if (chg)
            changed();
    }
    public void traverseOnStartupOrderChanged(int value)
    {
        bool chg = m_traverseOnStartupOrder != value;
        m_traverseOnStartupOrder = value;
        if (chg)
            changed();
    }

    public int traverseOnStartupMode
    {
        set
        {
            if (value != m_traverseOnStartupMode)
            {
                traverseOnStartupModeChanged(value);
            }
        }
        get { return m_traverseOnStartupMode; }
    }
    public int traverseOnStartupOrder
    {
        set
        {
            if (value != m_traverseOnStartupOrder)
            {
                traverseOnStartupOrderChanged(value);
            }
        }
        get { return m_traverseOnStartupOrder; }
    }

    public void overrideShowLevelsAboveChanged(bool value){
		bool chg = m_overrideShowLevelsAbove != value;
		m_overrideShowLevelsAbove = value;
		if (!DPUtils.isPrefab(gameObject)){
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityCoordinateSystem", "override-show-levels-above", "id", GetInstanceID (), value);
		}
		if (chg)
			changed ();
	}
	public bool overrideShowLevelsAbove {
		set {
			if (value != m_overrideShowLevelsAbove) {
				overrideShowLevelsAboveChanged(value);
			}
		}
		get { return m_overrideShowLevelsAbove; }
	}


	public void isTopForNavigationChanged(bool value){
		bool chg = m_isTopForNavigation != value;
		m_isTopForNavigation = value;
		if (!DPUtils.isPrefab(gameObject)){
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityCoordinateSystem", "is-top-for-navigation", "id", GetInstanceID (), value);
		}
		if (chg)
			changed ();
	}
	public bool isTopForNavigation {
		set {
			if (value != m_isTopForNavigation) {
				isTopForNavigationChanged(value);
			}
		}
		get { return m_isTopForNavigation; }
	}



	public void numberOfLevelsAboveToShowChanged(int value){
		bool chg = m_numberOfLevelsAboveToShow != value;
		m_numberOfLevelsAboveToShow = value;
		if (!DPUtils.isPrefab(gameObject)){
			DPManagerScript.Call_s_s_s_i_i ("SetDataValue-If-Exists-map", "UnityCoordinateSystem", "number-of-levels-above-to-show", "id", GetInstanceID (), value);
		}
		if (chg)
			changed ();
	}
	public int numberOfLevelsAboveToShow {
		set {
			if (value != m_numberOfLevelsAboveToShow) {
				numberOfLevelsAboveToShowChanged(value);
			}
		}
		get { return m_numberOfLevelsAboveToShow; }
	}

	public void nextWhenZoomedToOnNextSet(bool v)
	{
	    nextWhenZoomedToOnNext = v;
	}
	public bool nextWhenZoomedToOnNextGet()
	{
	    return nextWhenZoomedToOnNext;
	}
	public int GetRelatedVMInstanceID(){
		VMObject vmo = GetComponent<VMObject> ();
		if (vmo == null)
			return 0;
		return vmo.GetVMInstanceID ();
	}
	double [] ComputeBBX (){
		Bounds result = new Bounds ();
		bool resultIsSet = false;
		foreach (GameObject obj in addToBBX) {
			int nverts = 0;
			List< KeyValuePair<MeshFilter,Mesh> > meshList = new List< KeyValuePair<MeshFilter,Mesh> >() ;
			VMObject.GetMeshListForObject (meshList, obj, ref nverts);
			if (nverts > 0) {
				Bounds bounds = VMObject.GetBoundsFromMeshList (meshList, obj);
				if (resultIsSet)
					result.Encapsulate (bounds);
				else {
					result = bounds;
					resultIsSet = true;
				}
			}
		}
		return new double[] { result.min.x, result.min.y, result.min.z, result.max.x, result.max.y, result.max.z };
	}
	// Use this for initialization
	void Start () {
	}
    void AddToDP()
    {
        if (DPManagerScript.DPisInitialized)
        {
			string name = gameObject.name;
            VMObject vmo = GetComponent<VMObject>();
			if (vmo != null) {
				name = vmo.textForLabel;
			}
			if (level == 0) {
				if (m_VMDimensions.ContainsKey (0))
					Debug.Log ("WARNING: multiple VMDimension with level 0 name=" + gameObject.name);
				m_VMDimensions [0] = this;
			} else {
				m_VMDimensions [GetInstanceID ()] = this;
			}
			double [] bbx = ComputeBBX ();
			DPManagerScript.Call_i_s_i_cD("Create-UnityCoordinateSystem-map", GetVMDimensionID(), name, GetRelatedVMInstanceID(), bbx);
            levelChanged(m_level);
            overrideShowLevelsAboveChanged(m_overrideShowLevelsAbove);
			isTopForNavigationChanged (m_isTopForNavigation);
            numberOfLevelsAboveToShowChanged(m_numberOfLevelsAboveToShow);
            traverseOnStartupModeChanged(m_traverseOnStartupMode);
            if (traverseOnStartupMode == 1){
                setTraverseToOnly();
            } else if (traverseOnStartupMode == 2){
                DPManagerScript.executeWithID("settraverse", (new DPActionFunc {
                    actionPerformedFunc = () => SetAllTraverseInOrder()
                }));
            }
            addedToDP = true;
        }
    }
    void SetAllTraverseInOrder(){
        DPManagerScript.Call_noargs("Set-Unity-Traverse-map"); // reset
        foreach (KeyValuePair<int, VMDimension> orddim in traversalOrder){
            orddim.Value.addToTraverse();
        }
    }
	void RemoveFromDP(){
		DPManagerScript.Call_i( "Remove-UnityCoordinateSystem-With-ID-map", GetInstanceID() );
		m_VMDimensions.Remove(GetInstanceID ());
		addedToDP = false;
	}
	bool addedToDP = false;
	// Update is called once per frame
	void Update () {
		if (enabled != addedToDP) {
			if (enabled)
				AddToDP ();
			else
				RemoveFromDP ();
		}
	}
    public int moveInPlaying(int dir){
        if (Application.isPlaying)
        {
			List<int> alldiminfo = DPManagerScript.Call_rsa_wrapper_il ("Get-All-Coord-Systems-Traversing-map");
            int[] dinfo = alldiminfo.ToArray();
            int pl = -1;
            for (int i = 0; i < dinfo.Length; i++){
                if (dinfo[i] == GetVMDimensionID()){
                    pl = i;
                    break;
                }
            }
            if (pl >= 0){
                int newpl = pl + dir;
                if (newpl >= 0 && newpl < dinfo.Length){
                    int tmp = dinfo[pl];
                    dinfo[pl] = dinfo[newpl];
                    dinfo[newpl] = tmp;
                    DPManagerScript.Call_iba_wrapper("Set-Unity-Traverse-Array-map", dinfo);
                    ViewManager.invalidateView ();
                    return newpl;
                }
            }
        }
        return -1;
    }
    public void removeFromPlaying(){
        if (Application.isPlaying){
			List<int> alldiminfo = DPManagerScript.Call_rsa_wrapper_il("Get-All-Coord-Systems-Traversing-Plus-Order-map");
            int[] dinfo = alldiminfo.ToArray();
            int[] dnew = new int[System.Math.Max(0, (dinfo.Length / 2) - 1)];
            int pl = 0;
            for (int i = 0; i < dinfo.Length; i += 2)
            {
                int cs = dinfo[i + 1];
                if (cs != GetVMDimensionID())
                {
                    dnew[pl] = dinfo[i + 1];
                    pl++;
                }
            }
            DPManagerScript.Call_iba_wrapper("Set-Unity-Traverse-Array-map", dnew);
            ViewManager.invalidateView ();
        }
    }
	public void setTraverseToOnly(){
        DPManagerScript.Call_iba_wrapper("Set-Unity-Traverse-Array-map", new int[] { GetVMDimensionID() } );
        //DPManagerScript.Call_i("Set-Unity-Traverse-map", GetVMDimensionID());
    }
    public void addToTraverse(){
		DPManagerScript.Call_i ("Add-UnityTraverseCoordinateSystem-map", GetVMDimensionID());
	}
	public static void clearTraversal(){
		DPManagerScript.Call_s ("Remove-All-Instances-Of-DataType-map", "UnityTraverseCoordinateSystem");
	}
	public void changed(){
#if UNITY_EDITOR
		if (VMDimensionEditor.currentEditor != null && VMDimensionEditor.currentEditor.target!=null) {
			EditorUtility.SetDirty (VMDimensionEditor.currentEditor.target);
			if (!Application.isPlaying)
				UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
		}
#endif
	}
}
