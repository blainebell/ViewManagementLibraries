#define BOUNDS_FROM_GEOMETRY
//#define BOUNDS_FROM_BBX_TRANSFORMED
//#define ADD_OUTLINE

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class VMObject : MonoBehaviour {
	public int GetVMInstanceID(){
		return GetInstanceID ();
	}
	public VMDimension m_coordinateSystem;
	public void coordinateSystemChanged(VMDimension value){
		if (m_coordinateSystem != null)
			m_coordinateSystem.children.Remove (this);
		if (value != null)
			value.children.Add (this);
		m_coordinateSystem = value;
		changed ();
	}
	public VMDimension coordinateSystem {
		set {
			if (value != m_coordinateSystem) {
				coordinateSystemChanged (value);
			}
		}
		get { return m_coordinateSystem; }
	}
    public Color32 m_labelColor = Color.white;
    public void labelColorChanged(Color32 value)
    {
        m_labelColor = value;
        SetAnnotationColor(m_labelColor);
        changed();
    }
    public Color32 labelColor
    {
        set
        {
            if (!m_labelColor.Equals(value))
            {
                labelColorChanged(value);
            }
        }
        get { return m_labelColor; }
    }

    private bool visible = false;
    public bool isVisible() { return visible; }
    public GameObject m_annotation;
    public Bounds orig_m_annotationBounds;
    public bool m_annotationIsInit = false;
	private GameObject textannotation = null, annotationline = null;
    public bool getActiveSelf()
    {
        if (textannotation != null)
            return textannotation.activeSelf;
        if (m_annotation != null)
            return m_annotation.activeSelf;
        return false;
    }
    public void setAnnotationParent(Transform parent)
    {
        if (textannotation != null)
            textannotation.transform.SetParent(parent);
        if (m_annotation != null)
            m_annotation.transform.SetParent(parent);
    }
    public void setAnnotationRotation(Quaternion quat)
    {
        if (textannotation != null)
            textannotation.transform.rotation = quat;
        if (m_annotation != null)
            m_annotation.transform.rotation = quat;
    }
    public void setAnnotationPosition(Vector3 pos)
    {
        if (textannotation != null)
            textannotation.transform.position = pos;
        if (m_annotation != null)
            m_annotation.transform.position = pos;
    }
    public void setAnnotationLocalPosition(Vector3 pos)
    {
        if (textannotation != null)
            textannotation.transform.localPosition = pos;
        if (m_annotation != null)
            m_annotation.transform.localPosition = pos;
    }
    public void setAnnotationEulerAngles(Vector3 eA)
    {
        if (textannotation != null)
            textannotation.transform.eulerAngles = eA;
        if (m_annotation != null)
            m_annotation.transform.eulerAngles = eA;
    }
    
    public void setAnnotationLocalScale(Vector3 s)
    {
        if (textannotation != null)
            textannotation.transform.localScale = s;
        if (m_annotation != null)
            m_annotation.transform.localScale = s;
    }

    public Matrix4x4 getAnnotationLocalToWorld()
    {
        if (textannotation != null)
            return textannotation.transform.localToWorldMatrix;
        if (m_annotation != null)
            return m_annotation.transform.localToWorldMatrix;
        return new Matrix4x4();
    }
    public Vector3 getAnnotationPosition()
    {
        if (textannotation != null)
            return textannotation.transform.position;
        if (m_annotation != null)
            return m_annotation.transform.position;
        return new Vector3();
    }
    public Vector3 getAnnotationLocalPosition()
    {
        if (textannotation != null)
            return textannotation.transform.localPosition;
        if (m_annotation != null)
            return m_annotation.transform.localPosition;
        return new Vector3();
    }
    public Vector3 getAnnotationLocalScale()
    {
        if (textannotation != null)
            return textannotation.transform.localScale;
        if (m_annotation != null)
            return m_annotation.transform.localScale;
        return new Vector3();
    }
    public Quaternion getAnnotationRotation()
    {
        if (textannotation != null)
            return textannotation.transform.rotation;
        if (m_annotation != null)
            return m_annotation.transform.rotation;
        return new Quaternion();
    }
    public GameObject annotationGet()
    {
        if (textannotation!=null)
            return textannotation;
        if (m_annotation != null)
            return m_annotation;
        return null;
    }
    public GameObject annotationlineGet()
    {
        return annotationline;
    }
	public int annotationFontSize = 0;

	public Bounds annotationBoundsOrig;
	public static Dictionary<int, GameObject> m_VMObjects = new Dictionary<int, GameObject>();
	public static Dictionary<int, GameObject> m_VMObjectsByGameObjectID = new Dictionary<int, GameObject>();

	public bool prevActive = false, prevActiveLine = false;  // used internally
	bool m_shouldInterpolate = false;
	public bool shouldInterpolate {
		get { return m_shouldInterpolate; }
	}
	private Vector3 m_endPoint = new Vector3();
    private Vector3 m_startPoint = new Vector3();

    private float m_startScale = 1.0f, m_endScale = 1.0f;
    private Quaternion m_startQuat = Quaternion.identity, m_endQuat = Quaternion.identity;

    public Vector3 startPoint
    {
        set
        {
            if (!m_startPoint.Equals(value))
                m_startPoint = value;
        }
        get { return m_startPoint; }
    }
    public Vector3 endPoint
    {
        set
        {
            if (!m_endPoint.Equals(value))
                m_endPoint = value;
        }
        get { return m_endPoint; }
    }
    public float startScale
    {
        set
        {
            if (m_startScale != value)
                m_startScale = value;
        }
        get { return m_startScale; }
    }
    public float endScale
    {
        set
        {
            if (m_endScale != value)
                m_endScale = value;
        }
        get { return m_endScale; }
    }
    public Quaternion startQuat
    {
        set
        {
            if (!m_startQuat.Equals(value))
                m_startQuat = value;
        }
        get { return m_startQuat; }
    }
    public Quaternion endQuat
    {
        set
        {
            if (!m_endQuat.Equals(value))
                m_endQuat = value;
        }
        get { return m_endQuat; }
    }

    private float m_startTime, m_endTime;
    private float m_duration;

    public float startTime
    {
        set
        {
            if (m_startTime != value)
                m_startTime = value;
        }
        get { return m_startTime; }
    }
    public float endTime
    {
        set
        {
            if (m_endTime != value)
                m_endTime = value;
        }
        get { return m_endTime; }
    }
    public float duration
    {
        set {
            if (m_duration != value)
                m_duration = value;
        }
        get { return m_duration; }
    }

    public float lineWidthStart = 3.0f, lineWidthEnd = 3.0f;

    private Vector3 m_lineStartPoint1 = new Vector3 (), m_lineEndPoint1 = new Vector3 ();
	private Vector3 m_lineStartPoint2 = new Vector3 (), m_lineEndPoint2 = new Vector3 ();
	private bool m_isAnimating = false, m_isAnimatingLine = false;
    private float m_startTimeFadeIn, m_startTimeFadeOut;
    private bool m_isFadingIn = false, m_isFadingOut = false;

    public bool isFadingIn
    {
        set
        {
            if (m_isFadingIn != value)
                m_isFadingIn = value;
        }
        get { return m_isFadingIn; }
    }
    public bool isFadingOut
    {
        set
        {
            if (m_isFadingOut != value)
                m_isFadingOut = value;
        }
        get { return m_isFadingOut; }
    }


    public bool isAnimating
    {
        set
        {
            if (m_isAnimating != value)
                m_isAnimating = value;
        }
        get { return m_isAnimating; }
    }

    public bool isAnimatingLine
    {
        set
        {
            if (m_isAnimatingLine != value)
                m_isAnimatingLine = value;
        }
        get { return m_isAnimatingLine; }
    }
    public float startTimeFadeIn
    {
        set
        {
            if (m_startTimeFadeIn != value)
                m_startTimeFadeIn = value;
        }
        get { return m_startTimeFadeIn; }
    }
    public float startTimeFadeOut
    {
        set
        {
            if (m_startTimeFadeOut != value)
                m_startTimeFadeOut = value;
        }
        get { return m_startTimeFadeOut; }
    }



    public Vector3 lineStartPoint1
    {
        set
        {
            if (!m_lineStartPoint1.Equals(value))
                m_lineStartPoint1 = value;
        }
        get { return m_lineStartPoint1; }
    }
    public Vector3 lineStartPoint2
    {
        set
        {
            if (!m_lineStartPoint2.Equals(value))
                m_lineStartPoint2 = value;
        }
        get { return m_lineStartPoint2; }
    }
    public Vector3 lineEndPoint1
    {
        set
        {
            if (!m_lineEndPoint1.Equals(value))
                m_lineEndPoint1 = value;
        }
        get { return m_lineEndPoint1; }
    }
    public Vector3 lineEndPoint2
    {
        set
        {
            if (!m_lineEndPoint2.Equals(value))
                m_lineEndPoint2 = value;
        }
        get { return m_lineEndPoint2; }
    }

    private bool recomputeLine =  false;
	private bool noRecomputeLine = false;
	public void setRecomputeLine(bool val){
		recomputeLine = val;
	}
	public void setNoRecomputeLine(bool val){
		noRecomputeLine = val;
	}
	public VMObject(){
	}
	~VMObject(){
	}
	public static float lineMaxAlpha = 0.5f;
	public void SetAnnotationAlpha(float alpha){
        Color newColor;
        if (textannotation != null)
        {
            TextMesh tm = textannotation.GetComponent<TextMesh>();
            newColor = new Color(tm.color.r, tm.color.g, tm.color.b, (1.0f - alpha));
            tm.color = newColor;
        } else
        {
            newColor = new Color(1.0f, 1.0f, 1.0f, (1.0f - alpha));
        }
        if (annotationline != null)
        {
            LineRenderer lr = annotationline.GetComponent<LineRenderer>();
            newColor.a = lineMaxAlpha * newColor.a;
            lr.startColor = lr.endColor = newColor;
        }
    //		Debug.Log ("SetAnnotationAlpha: alpha=" + alpha);
    }
	public void SetAnnotationColor(Color col){
		Color newColor = col;
        if (textannotation != null)
        {
            TextMesh tm = textannotation.GetComponent<TextMesh>();
            newColor.a = tm.color.a;
            tm.color = newColor;
        }
		LineRenderer lr = annotationline.GetComponent<LineRenderer> ();
        lr.startColor = lr.endColor = newColor;
	}
	public void checkFadeIn(bool ann){
		if (ann) {
			if (fadeIn) {
				if (!isFadingIn) { // not sure why TurnOffAnnotationFor gets called every frame?
					float curTime = Time.time + Time.smoothDeltaTime;
					startTimeFadeIn = curTime;
					isFadingIn = true;
				}
			}
		} else {
			isFadingIn = false;
		}
	}
	public void SetAnnotationsActive(bool ann, bool line){
		bool prevAnn = false;
		if (textannotation != null) {
			prevAnn = textannotation.activeSelf;
			textannotation.SetActive (ann);
		}
        if (m_annotation != null)
        {
            prevAnn = m_annotation.activeSelf;
            m_annotation.SetActive(ann);
        }
        if (annotationline != null) {
			annotationline.SetActive (line);
		}
		if (!line) {
			recomputeLine = false;
			noRecomputeLine = false;
		}
		if (ann) {
			isFadingOut = false;
			SetAnnotationAlpha (0.0f);
		}
		DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "is-active", "id", GetVMInstanceID (), ann);
		if (prevAnn!=ann) checkFadeIn (ann);
	}
	public void SetAnnotationToActive(){
		bool prevAnn = false;
		if (textannotation != null) {
			prevAnn = textannotation.activeSelf;
			textannotation.SetActive (true);
		}
        if (m_annotation != null)
        {
            prevAnn = m_annotation.activeSelf;
            m_annotation.SetActive(true);
        }
        isFadingOut = false;
		DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "is-active", "id", GetVMInstanceID (), true);
		if (!prevAnn) checkFadeIn (true);
	}

	public bool m_tryToLabel = false;
	public void tryToLabelChanged(bool value){
		m_tryToLabel = value;
		if (!DPUtils.isPrefab (gameObject)) {
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "try-to-label-Manual", "id", GetVMInstanceID (), value);
			if (!value)
				SetAnnotationsActive (false, false);
		}
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.tryToLabel = value;
					}
				}
			}
		}
		changed ();
	}
	public bool tryToLabel {
		set {
			if (value != m_tryToLabel) {
				tryToLabelChanged(value);
			}
		}
		get { return m_tryToLabel; }
	}

	private Vector2 screenStabilizedPlacement = new Vector2();
	private bool screenStabilizedPlacementIsSet = false;
    private bool m_screenStabilizedIsAnimating = false;
	public bool screenStabilizedIsAnimating {
		set {
			m_screenStabilizedIsAnimating = value;
			if (!value) {
				screenStabilizedPlacementIsSet = false;
			}
		}
		get { return m_screenStabilizedIsAnimating; }
	}
	private bool placeInMiddleOfScreen(){
		if (!geometryCentroidIsSet)
			return false;
		Camera curCamera = ViewManager.getCurrentCamera();
		Vector3 objectToCamVect = getTargetPoint() - curCamera.transform.position;
		float objectToCamVectDist = objectToCamVect.magnitude;
		Vector3 screenPoint = new Vector3(curCamera.pixelWidth/2.0f,curCamera.pixelHeight/2.0f, objectToCamVectDist);
		Vector3 worldPoint = curCamera.ScreenToWorldPoint (screenPoint);
		float zscale = DPUtils.getScaleFromZDistance(objectToCamVectDist);
		float ar = zscale * minHeight / annotationBoundsOrig.size.y;

        setAnnotationLocalPosition(worldPoint);
        setAnnotationRotation(curCamera.transform.rotation);
        setAnnotationLocalScale(new Vector3(ar, ar, 1.0f));
        SetAnnotationsActive (true, false);
		return true;
	}
	private bool placeMinScale(){
		if (!geometryCentroidIsSet)
			return false;
		Camera curCamera = ViewManager.getCurrentCamera();
		Vector3 objectToCamVect = getTargetPoint() - curCamera.transform.position;
		float objectToCamVectDist = objectToCamVect.magnitude;
		float zscale = DPUtils.getScaleFromZDistance(objectToCamVectDist);
		float ar = zscale * minHeight / annotationBoundsOrig.size.y;
        setAnnotationRotation(curCamera.transform.rotation);
        setAnnotationLocalScale(new Vector3(ar, ar, 1.0f));
        return true;
	}

    private bool m_screenStabilized = false;
    public void screenStabilizedChanged(bool value)
    {
        m_screenStabilized = value;
        if (!DPUtils.isPrefab(gameObject))
        {
            DPManagerScript.Call_s_s_s_i_b("SetDataValue-If-Exists-map", "UnityObject", "screen-stabilized", "id", GetVMInstanceID(), value);
        }
        if (DPUtils.isPrefab(gameObject))
        {
            ViewManager vm = FindObjectOfType<ViewManager>();
            if (vm.prefabsControlWithSameTag)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag(gameObject.tag);
                foreach (GameObject go in gos)
                {
                    VMObject vmo = go.GetComponent<VMObject>();
                    if (vmo != null && vmo != this)
                    {
                        vmo.screenStabilized = value;
                    }
                }
            }
        }
        else //if (annotation.activeSelf)
        {
            if (m_screenStabilized)
            { // recompute screen matrix
				if (getActiveSelf()) {
					Camera curCamera = ViewManager.getCurrentCamera();
					screenStabilizedIsAnimating = true;
					// TODO set screenStabilizedPlacement
					float sWidth = curCamera.pixelWidth;
					float sHeight = curCamera.pixelHeight;
					Vector3 pt = curCamera.WorldToScreenPoint(getAnnotationLocalPosition());
					if (pt.z < 0.0f) {
						pt.x = (sWidth / 2.0f) - pt.x;
						pt.y = (sHeight / 2.0f) - pt.y;
						pt.z = -pt.z;
					}
					Vector2 pt2 = new Vector2 (pt.x / sWidth, pt.y / sHeight);
					Vector2 center = new Vector2 (.5f, .5f);
					if (DPUtils.ClipLineToBox2d (ref pt2, center)) {
						pt.x = pt2.x * sWidth; pt.y = pt2.y * sHeight;
					}
					Vector3 tPoint = getTargetPoint ();
					Vector3 objectToCamVect = tPoint - curCamera.transform.position;
					float zval = objectToCamVect.magnitude;
					Vector3 centerScreen = curCamera.transform.position + curCamera.transform.forward * zval;
					Vector3 spt = curCamera.WorldToScreenPoint (centerScreen);
					screenStabilizedPlacement = new Vector2(pt.x - spt.x, pt.y - spt.y);
					screenStabilizedPlacementIsSet = true;
					m_screenStabilizedIsAnimating = true;
				} else { // if not active, then put into middle of screen
					if (!placeInMiddleOfScreen())
						m_screenStabilized = false;
				}
				SetAnnotationAlpha (0.0f);
				/*if (!isFadingIn) { // not sure why TurnOffAnnotationFor gets called every frame?
					float curTime = Time.time + Time.smoothDeltaTime;
					startTimeFadeIn = curTime;
					isFadingOut = false;
					isFadingIn = true;
				}*/
            }
            else
            {
                // take screen stabilized position and put it back into world coordinates
                // NEED TO SET Vector3 startPoint, endPoint
                //             ??? float startScale, endScale;
                //             Quaternion startQuat, endQuat;
                {
                    ViewManager vm = FindObjectOfType<ViewManager>();
                    endPoint = getAnnotationLocalPosition();
                    endScale = getAnnotationLocalScale().x;
                    endQuat = getAnnotationRotation();
                    prevActiveLine = annotationline.activeSelf;
                    // do not need to set the line info, since it gets set already when screenStabilized
					Vector3 spt = ViewManager.getCurrentCamera().WorldToScreenPoint(getAnnotationLocalPosition());
                    float lineWidth = outsideLineWidth * DPUtils.getScaleFromZDistance(spt.z);
                    lineWidthEnd = lineWidth;
                    screenStabilizedIsAnimating = false;
				}
            }
        }
        changed();
    }
    public bool screenStabilized
    {
        set
        {
            if (value != m_screenStabilized)
            {
                screenStabilizedChanged(value);
            }
        }
        get { return m_screenStabilized; }
    }


    public bool m_showWhenNotVisible = false;
    public void showWhenNotVisibleChanged(bool value)
    {
        m_showWhenNotVisible = value;
        if (!DPUtils.isPrefab(gameObject))
        {
            DPManagerScript.Call_s_s_s_i_b("SetDataValue-If-Exists-map", "UnityObject", "show-when-not-visible", "id", GetVMInstanceID(), value);
            //if (!value)
            //    SetAnnotationsActive(false, false);
        }
        if (DPUtils.isPrefab(gameObject))
        {
            ViewManager vm = FindObjectOfType<ViewManager>();
            if (vm.prefabsControlWithSameTag)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag(gameObject.tag);
                foreach (GameObject go in gos)
                {
                    VMObject vmo = go.GetComponent<VMObject>();
                    if (vmo != null && vmo != this)
                    {
                        vmo.showWhenNotVisible = value;
                    }
                }
            }
        }
		if (showWhenNotVisible) {
			if (!getActiveSelf()) {
				// if showWhenNotVisible and not active, place in middle of screen
				screenStabilized = true;
			}
		} else {
            if (screenStabilized)
            {
                screenStabilized = false;
                //turnOff(true);
            }
		}
        changed();
    }
    public bool showWhenNotVisible
    {
        set
        {
            if (value != m_showWhenNotVisible)
            {
                showWhenNotVisibleChanged(value);
            }
        }
        get { return m_showWhenNotVisible; }
    }


    public Vector3 getWorldTargetPoint()
    {
        Matrix4x4 topToWorld = ViewManager.getTopToWorldMatrix();
        if (behaviorMode != 2 && whenShowInDirectPlacement && directViewPlacement != null)
        {
            return topToWorld.MultiplyPoint(directViewPlacement.linePoint2);
        }
        return topToWorld.MultiplyPoint(geometryCentroid);
    }
    public Vector3 getWorldTargetPoint1()
    {
        Matrix4x4 topToWorld = ViewManager.getTopToWorldMatrix();
        if (behaviorMode != 2 && whenShowInDirectPlacement && directViewPlacement != null)
        {
            return topToWorld.MultiplyPoint(directViewPlacement.linePoint1);
        }
        return topToWorld.MultiplyPoint(geometryCentroid);
    }
    public Vector3 getTargetPoint(){
		if (behaviorMode!=2 && whenShowInDirectPlacement && directViewPlacement != null) {
			return directViewPlacement.linePoint2;
		}
		return geometryCentroid;
	}
	public bool m_whenShowInDirectPlacement = false; // when looking in the direction, show in the direct placement
	public void whenShowInDirectPlacementChanged(bool value)
	{
		m_whenShowInDirectPlacement = value;
		if (!DPUtils.isPrefab(gameObject))
		{
			DPManagerScript.Call_s_s_s_i_b("SetDataValue-If-Exists-map", "UnityObject", "when-looking-in-direction-show-in-direct-placement", "id", GetVMInstanceID(), value);
			//if (!value)
			//    SetAnnotationsActive(false, false);
		}
        if (DPUtils.isPrefab(gameObject))
        {
            ViewManager vm = FindObjectOfType<ViewManager>();
            if (vm.prefabsControlWithSameTag)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag(gameObject.tag);
                foreach (GameObject go in gos)
                {
                    VMObject vmo = go.GetComponent<VMObject>();
                    if (vmo != null && vmo != this)
                    {
                        vmo.whenShowInDirectPlacement = value;
                    }
                }
            }
        } else {
            if (whenShowInDirectPlacement && directViewPlacement == null)
                requestPlacementWithDirectView = true;
        }
		changed();
	}
	public bool whenShowInDirectPlacement
	{
		set
		{
			if (value != whenShowInDirectPlacement)
			{
				whenShowInDirectPlacementChanged(value);
			}
		}
		get { return m_whenShowInDirectPlacement; }
	}

	public class DirectViewPlacement
	{
		public Vector3 fromWorldPoint, worldPoint;
		public Quaternion rotation;
		public bool lineShown;
		public Vector3 linePoint1, linePoint2;  // linePoint1 - point closest to label, linePoint2 - target point on object
		public float scale, zdistance, lineWidth;
		public void turnDirectViewPlacement (VMObject vmo){
            vmo.isAnimating = true;
            vmo.startPoint = vmo.endPoint;
            vmo.startScale = vmo.endScale;
            vmo.startQuat = vmo.endQuat;
            float curTime = Time.time + Time.smoothDeltaTime;
            vmo.startTime = curTime;
            vmo.duration = (vmo.interpolateBetweenTimeMS / 1000.0f);
            vmo.endTime = curTime + vmo.duration;

            vmo.endPoint = worldPoint;
            vmo.endQuat = rotation;
            vmo.endScale = scale;
            if (lineShown) {
                vmo.lineStartPoint1 = vmo.lineEndPoint1;
				vmo.lineStartPoint2 = vmo.lineEndPoint2;
				vmo.lineEndPoint1 = linePoint1;
				vmo.lineEndPoint2 = linePoint2;
                vmo.isAnimatingLine = true;
                vmo.lineWidthStart = vmo.lineWidthEnd = lineWidth;
                vmo.annotationline.SetActive(true);
            }
            vmo.SetAnnotationsActive(true, lineShown);
        }
    }
	private DirectViewPlacement m_directViewPlacement = null;
	private bool showingDirectViewPlacement = false;
	public bool showingDirectViewPlacementGet(){
		return showingDirectViewPlacement;
	}
	public void showingDirectViewPlacementSet(bool value){
		if (showingDirectViewPlacement != value) {
			showingDirectViewPlacement = value;
			changed ();
		}
	}
	public DirectViewPlacement directViewPlacement
	{
		set {
			m_directViewPlacement = value;
		}
		get { return m_directViewPlacement; }
	}
	public void turnDirectViewPlacement (bool on){
		if (m_directViewPlacement != null) {
			if (on != showingDirectViewPlacement) {
				if (on) {
					m_directViewPlacement.turnDirectViewPlacement (this);
				} else {
				}
				showingDirectViewPlacement = on;
			}
		}
	}
    private bool m_requestPlacementWithDirectView = false;
    public void requestPlacementWithDirectViewChanged(bool value)
    {
        m_requestPlacementWithDirectView = value;
        if (!DPUtils.isPrefab(gameObject))
        {
            DPManagerScript.Call_s_s_s_i_b("SetDataValue-If-Exists-map", "UnityObject", "request-place-with-direct-view", "id", GetVMInstanceID(), value);
        }
        if (DPUtils.isPrefab(gameObject))
        {
            ViewManager vm = FindObjectOfType<ViewManager>();
            if (vm.prefabsControlWithSameTag)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag(gameObject.tag);
                foreach (GameObject go in gos)
                {
                    VMObject vmo = go.GetComponent<VMObject>();
                    if (vmo != null && vmo != this)
                    {
                        vmo.requestPlacementWithDirectView = value;
                    }
                }
            }
        }
        changed();
    }
    public bool requestPlacementWithDirectView
    {
        set
        {
            if (value != m_requestPlacementWithDirectView)
            {
                requestPlacementWithDirectViewChanged(value);
            }
        }
        get { return m_requestPlacementWithDirectView; }
    }


    private Matrix4x4 worldToDirectView;
    public Matrix4x4 worldToDirectViewGet() { return worldToDirectView; }
    public void worldToDirectViewSet(Matrix4x4 mat) { worldToDirectView = mat; }
    public bool m_placeWithDirectView = false;
    public void placeWithDirectViewChanged(bool value)
    {
        m_placeWithDirectView = value;
        if (value)
        {
            setToStillLayoutImpl(); // turn to manual/best if placing with direct view
            if (directViewPlacement == null)
                requestPlacementWithDirectView = true;
            else
            {
                turnDirectViewPlacement(true);
            }
        } else
        {
            if (showingDirectViewPlacementGet())
            {
                turnOff(true);
                showingDirectViewPlacementSet(false);
            }
        }
        if (!DPUtils.isPrefab(gameObject))
        {
            DPManagerScript.Call_s_s_s_i_b("SetDataValue-If-Exists-map", "UnityObject", "place-with-direct-view", "id", GetVMInstanceID(), value);
        }
        if (DPUtils.isPrefab(gameObject))
        {
            ViewManager vm = FindObjectOfType<ViewManager>();
            if (vm.prefabsControlWithSameTag)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag(gameObject.tag);
                foreach (GameObject go in gos)
                {
                    VMObject vmo = go.GetComponent<VMObject>();
                    if (vmo != null && vmo != this)
                    {
                        vmo.placeWithDirectView = value;
                    }
                }
            }
        }
        changed();
    }
    public bool placeWithDirectView
    {
        set
        {
            if (value != m_placeWithDirectView)
            {
                placeWithDirectViewChanged(value);
            }
        }
        get { return m_placeWithDirectView; }
    }



    public int m_notVisibleLayoutMode = 0;  // 0 - last placement, 1 - closest to projection, 2 - discrete/closest to projection
    public void notVisibleLayoutModeChanged(int value)
    {
        m_notVisibleLayoutMode = value;
        if (DPUtils.isPrefab(gameObject))
        {
            ViewManager vm = FindObjectOfType<ViewManager>();
            if (vm.prefabsControlWithSameTag)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag(gameObject.tag);
                foreach (GameObject go in gos)
                {
                    VMObject vmo = go.GetComponent<VMObject>();
                    if (vmo != null && vmo != this)
                    {
                        vmo.notVisibleLayoutMode = value;
                    }
                }
            }
        } else
        {
            if (m_notVisibleLayoutMode == 1)
                screenStabilizedIsAnimating = true;
        }
        changed();
    }
    public int notVisibleLayoutMode
    {
        set
        {
            if (value != m_notVisibleLayoutMode)
            {
                notVisibleLayoutModeChanged(value);
            }
        }
        get { return m_notVisibleLayoutMode; }
    }

    public int m_notVisibleArrowLayoutMode = 0;  // 0 - none, outside, inside, left, right, top, bottom 
    public void notVisibleArrowLayoutModeChanged(int value)
    {
        m_notVisibleArrowLayoutMode = value;
        if (DPUtils.isPrefab(gameObject))
        {
            ViewManager vm = FindObjectOfType<ViewManager>();
            if (vm.prefabsControlWithSameTag)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag(gameObject.tag);
                foreach (GameObject go in gos)
                {
                    VMObject vmo = go.GetComponent<VMObject>();
                    if (vmo != null && vmo != this)
                    {
                        vmo.notVisibleArrowLayoutMode = value;
                    }
                }
            }
        }
        changed();
    }
    public int notVisibleArrowLayoutMode
    {
        set
        {
            if (value != m_notVisibleArrowLayoutMode)
            {
                notVisibleArrowLayoutModeChanged(value);
            }
        }
        get { return m_notVisibleArrowLayoutMode; }
    }

    public int m_notVisibleArrowMode = 0;  // 0 - none, 1 - cut line, 2 - cut arrow
    public void notVisibleArrowModeChanged(int value)
    {
        m_notVisibleArrowMode = value;
        if (DPUtils.isPrefab(gameObject))
        {
            ViewManager vm = FindObjectOfType<ViewManager>();
            if (vm.prefabsControlWithSameTag)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag(gameObject.tag);
                foreach (GameObject go in gos)
                {
                    VMObject vmo = go.GetComponent<VMObject>();
                    if (vmo != null && vmo != this)
                    {
                        vmo.notVisibleArrowMode = value;
                    }
                }
            }
        }
        changed();
    }
    public int notVisibleArrowMode
    {
        set
        {
            if (value != m_notVisibleArrowMode)
            {
                notVisibleArrowModeChanged(value);
            }
        }
        get { return m_notVisibleArrowMode; }
    }


    public string m_textForLabel;
	public void textForLabelChanged(string value){
		m_textForLabel = value;
		DPManagerScript.Call_s_s_s_i_s ("SetDataValue-If-Exists-map", "UnityObject", "label-string", "id", GetVMInstanceID (), m_textForLabel);
		/*
		if (annotation != null) {
			TextMesh tm = annotation.gameObject.GetComponent<TextMesh> ();
			if (tm != null) {
				if (m_textForLabel.Trim ().Length > 0)
					tm.text = m_textForLabel;
				else
					tm.text = GetVMInstanceID ().ToString ();
			}
		}*/
        changed();
	}
	public string textForLabel {
		set {
			if (value != m_textForLabel) {
				textForLabelChanged(value);
			}
		}
		get { return m_textForLabel; }
	}


	public bool m_canMove = false;
	public void canMoveChanged(bool value){
		m_canMove = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "can-move", "id", GetVMInstanceID (), value);

		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.canMove = value;
					}
				}
			}
		}
		changed ();
	}
	public bool canMove {
		set {
			if (value != m_canMove) {
				canMoveChanged(value);
			}
		}
		get { return m_canMove; }
	}

	public bool m_showBBX = false;
	public void showBBXChanged(bool value){
		m_showBBX = value;
		if (!DPUtils.isPrefab (gameObject)) {
			DPManagerScript.Call_i_b ("Change-Show-BBX-map", GetVMInstanceID (), value);
		}
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.showBBX = value;
					}
				}
			}
		}
		changed ();
	}
	public bool showBBX {
		set {
			if (value != m_showBBX) {
				showBBXChanged(value);
			}
		}
		get { return m_showBBX; }
	}


	public int m_geometryMode = 0;  // 0 - from bbx, 1 - from bbx rotated, 2 - from geometry
	public void geometryModeChanged(int value){
		m_geometryMode = value;
		if (!DPUtils.isPrefab(gameObject)){
            // regenerate geometry
            prevTransMatrix = transform.worldToLocalMatrix;
            reloadGeometry();
			showBBXChanged (showBBX);
		}
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.geometryMode = value;
					}
				}
			}
		}
		changed ();
	}
	public int geometryMode {
		set {
			if (value != m_geometryMode) {
				geometryModeChanged(value);
			}
		}
		get { return m_geometryMode; }
	}




	public bool m_sizeIsDefault = true;
	public void sizeIsDefaultChanged(bool value){
		m_sizeIsDefault = value;
		ViewManager vm = FindObjectOfType<ViewManager>();
        if (vm == null)
        {
            Debug.Log("WARNING: VMObject.sizeIsDefaultChanged vm=null");
            return;
        }
		if (m_sizeIsDefault) {
			minHeight = vm.defaultAnnotationMinHeight;
			maxHeight = vm.defaultAnnotationMaxHeight;
		}
		if (DPUtils.isPrefab(gameObject)){
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.sizeIsDefault = value;
						if (m_sizeIsDefault) {
							vmo.minHeight = vm.defaultAnnotationMinHeight;
							vmo.maxHeight = vm.defaultAnnotationMaxHeight;
						}
					}
				}
			}
		}
		changed ();
	}
	public bool sizeIsDefault {
		set {
			if (value != m_sizeIsDefault) {
				sizeIsDefaultChanged(value);
			}
		}
		get { return m_sizeIsDefault; }
	}

	public int m_minHeight = 24;
	public float minHeightFloat = 0.0f;
	public void minHeightChanged(int value){
		bool vchanged = value != m_minHeight;
		m_minHeight = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_i ("SetDataValue-If-Exists-map", "UnityObject", "min-y-for-best-placement", "id", GetVMInstanceID (), value);
		float height = ViewManager.getCurrentCamera().pixelHeight;
		minHeightFloat = m_minHeight / height;

		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.minHeight = value;
						vmo.minHeightFloat = minHeightFloat;
					}
				}
			}
		}
		if (vchanged)
			changed ();
	}
	public int minHeight {
		set {
			if (value != m_minHeight) {
				minHeightChanged(value);
			}
		}
		get { return m_minHeight; }
	}
	public int m_maxHeight = 40;
	public float maxHeightFloat = 0.0f;
	public void maxHeightChanged(int value){
		bool vchanged = value != m_maxHeight;
		m_maxHeight = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_i ("SetDataValue-If-Exists-map", "UnityObject", "max-y-for-best-placement", "id", GetVMInstanceID (), value);
		float height = ViewManager.getCurrentCamera().pixelHeight;
		maxHeightFloat = m_maxHeight / height;

		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.maxHeight = value;
						vmo.maxHeightFloat = maxHeightFloat;
					}
				}
			}
		}
		if (vchanged)
			changed ();
	}
	public int maxHeight {
		set {
			if (value != m_maxHeight) {
				maxHeightChanged(value);
			}
		}
		get { return m_maxHeight; }
	}



	public bool m_tryToLabelInside = true;
	public void tryToLabelInsideChanged(bool value){
		m_tryToLabelInside = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "try-to-label-inside", "id", GetVMInstanceID (), value);

		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.tryToLabelInside = value;
					}
				}
			}
		}
		changed ();
	}
	public bool tryToLabelInside {
		set {
			if (value != m_tryToLabelInside) {
				tryToLabelInsideChanged(value);
			}
		}
		get { return m_tryToLabelInside; }
	}




	public bool m_tryToLabelOutside = true;
	public void tryToLabelOutsideChanged(bool value){
		m_tryToLabelOutside = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "try-to-label-outside", "id", GetVMInstanceID (), value);

		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.tryToLabelOutside = value;
					}
				}
			}
		}
		changed ();
	}
	public bool tryToLabelOutside {
		set {
			if (value != m_tryToLabelOutside) {
				tryToLabelOutsideChanged(value);
			}
		}
		get { return m_tryToLabelOutside; }
	}


	public bool m_tryToLabelOnCentroid = false;
	public void tryToLabelOnCentroidChanged(bool value){
		m_tryToLabelOnCentroid = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "try-to-label-on-centroid", "id", GetVMInstanceID (), value);

		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.tryToLabelOnCentroid = value;
					}
				}
			}
		}
		changed ();
	}
	public bool tryToLabelOnCentroid {
		set {
			if (value != m_tryToLabelOnCentroid) {
				tryToLabelOnCentroidChanged(value);
			}
		}
		get { return m_tryToLabelOnCentroid; }
	}



	public bool m_alwaysFacing = true;
	public void alwaysFacingChanged(bool value){
		m_alwaysFacing = value;
		if (!DPUtils.isPrefab(gameObject)){
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "always-facing", "id", GetVMInstanceID (), value);
		}
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.alwaysFacing = value;
					}
				}
			}
		}
		changed ();
	}
	public bool alwaysFacing {
		set {
			if (value != m_alwaysFacing) {
				alwaysFacingChanged(value);
			}
		}
		get { return m_alwaysFacing; }
	}



	public bool m_planeOriented = false;
	public void planeOrientedChanged(bool value){
		m_planeOriented = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "plane-oriented", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.planeOriented = value;
					}
				}
			}
		}
		changed ();
	}
	public bool planeOriented {
		set {
			if (value != m_planeOriented) {
				planeOrientedChanged(value);
			}
		}
		get { return m_planeOriented; }
	}


	public bool m_planeOrientedAxisAligned = false;
	public void planeOrientedAxisAlignedChanged(bool value){
		m_planeOrientedAxisAligned = value;
//		if (!DPUtils.isPrefab(gameObject))
//			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "plane-oriented", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.planeOrientedAxisAligned = value;
					}
				}
			}
		}
		changed ();
	}
	public bool planeOrientedAxisAligned {
		set {
			if (value != m_planeOrientedAxisAligned) {
				planeOrientedAxisAlignedChanged(value);
			}
		}
		get { return m_planeOrientedAxisAligned; }
	}





	public int m_scheduleMode = 0;  // 0 - manual, 1 - continuous, 2 - every ms, 3 - only changed
	public void scheduleModeChanged(int value){
		m_scheduleMode = value;
		if (!DPUtils.isPrefab(gameObject)){
			if (m_scheduleMode != 3) {
				setNoRecomputeLine (false);
				setRecomputeLine(false);
			}
            if (addedToDP)
    			DPManagerScript.Call_i_b_b_b_i_b ("Set-Schedule-For-UnityObject-map", GetVMInstanceID (), m_scheduleMode == 0, m_scheduleMode == 1,
	    			m_scheduleMode == 2, m_scheduleDurationEveryMS, m_scheduleMode == 3);
			m_shouldInterpolate = m_interpolateBetween && scheduleMode != 1;  // interpolate only when not continuous
		}
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.scheduleMode = value;
					}
				}
			}
		}
		changed ();
	}
	public int scheduleMode {
		set {
			if (value != m_scheduleMode) {
				scheduleModeChanged(value);
			}
		}
		get { return m_scheduleMode; }
	}

	public int m_behaviorMode = 0;  // 0 - none, 1 - direct and screen stabilized, 2 - only screen stabilized if
	public int behaviorMode {
		set {
			m_behaviorMode = value;
/*			if (value != m_behaviorMode) {
				//scheduleModeChanged(value);
			}
			*/
		}
		get { return m_behaviorMode; }
	}

	public int m_scheduleDurationEveryMS = 1000;
	public void scheduleDurationEveryMSChanged(int value){
		m_scheduleDurationEveryMS = value;
		if (!DPUtils.isPrefab(gameObject)){
			DPManagerScript.Call_i_b_b_b_i_b ("Set-Schedule-For-UnityObject-map", GetVMInstanceID (), m_scheduleMode == 0, m_scheduleMode == 1, 
																				  m_scheduleMode == 2, m_scheduleDurationEveryMS, 
																				  m_scheduleMode == 3);
		}
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.scheduleDurationEveryMS = value;
					}
				}
			}
		}
		changed ();
	}
	public int scheduleDurationEveryMS {
		set {
			if (value != m_scheduleDurationEveryMS) {
				scheduleDurationEveryMSChanged(value);
			}
		}
		get { return m_scheduleDurationEveryMS; }
	}



	public bool m_interpolateBetween = true;
	public void interpolateBetweenChanged(bool value){
		m_interpolateBetween = value;
		if (!DPUtils.isPrefab(gameObject)){
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "interpolate-between-results", "id", GetVMInstanceID (), value);
			//m_shouldInterpolate = m_interpolateBetween;  // interpolate only when not continuous
			m_shouldInterpolate = m_interpolateBetween && scheduleMode != 1;  // interpolate only when not continuous
		}
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.interpolateBetween = value;
					}
				}
			}
		}
		changed ();
	}
	public bool interpolateBetween {
		set {
			if (value != m_interpolateBetween) {
				interpolateBetweenChanged(value);
			}
		}
		get { return m_interpolateBetween; }
	}


	public int m_interpolateBetweenTimeMS = 400;
	public void interpolateBetweenTimeMSChanged(int value){
		m_interpolateBetweenTimeMS = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_i ("SetDataValue-If-Exists-map", "UnityObject", "interpolate-between-results-time-ms", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.interpolateBetweenTimeMS = value;
					}
				}
			}
		}
		changed ();
	}
	public int interpolateBetweenTimeMS {
		set {
			if (value != m_interpolateBetweenTimeMS) {
				interpolateBetweenTimeMSChanged(value);
			}
		}
		get { return m_interpolateBetweenTimeMS; }
	}


	public int m_hysteresisMode = 0;  // 0 - best, 1 - closest to previous
	public void hysteresisModeChanged(int value){
		m_hysteresisMode = value;
		if (!DPUtils.isPrefab(gameObject)){
            if (addedToDP)
			    DPManagerScript.Call_i_b_b ("Set-Hysteresis-For-UnityObject-map", GetVMInstanceID (), m_hysteresisMode == 0, m_hysteresisMode == 1);
		}
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.hysteresisMode = value;
					}
				}
			}
		}
		changed ();
	}
	public int hysteresisMode {
		set {
			if (value != m_hysteresisMode) {
				hysteresisModeChanged(value);
			}
		}
		get { return m_hysteresisMode; }
	}




	public int m_zMode = 1; /* 0 - constant
							   1 - minz
							   2 - maxz
							   3 - middlez
							 */
	public void zModeChanged(int value){
		m_zMode = value;
		if (!DPUtils.isPrefab (gameObject)) {
			DPManagerScript.Call_i_b_b_b_b_f ("Set-Z-Mode-map", GetVMInstanceID (), value == 0, value == 1, value == 2, value == 3, constantZDistance);
		}
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.zMode = value;
					}
				}
			}
		}
		changed ();
	}
	public int zMode {
		set {
			if (value != m_zMode) {
				zModeChanged(value);
			}
		}
		get { return m_zMode; }
	}
		
	public float m_constantZDistance = 10;
	public void constantZDistanceChanged(float value){
		m_constantZDistance = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_f ("SetDataValue-If-Exists-map", "UnityObject", "z-mode-constant-distance", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.constantZDistance = value;
					}
				}
			}
		}
		changed ();
	}
	public float constantZDistance {
		set {
			if (value != m_constantZDistance) {
				constantZDistanceChanged(value);
			}
		}
		get { return m_constantZDistance; }
	}


	public float m_outsideLineWidth = 3.0f;
	public void outsideLineWidthChanged(float value){
		m_outsideLineWidth = value;
		//if (!DPUtils.isPrefab(gameObject))
		//	DPManagerScript.Call_s_s_s_i_f ("SetDataValue-If-Exists-map", "UnityObject", "z-mode-constant-distance", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.outsideLineWidth = value;
					}
				}
			}
		}
		changed ();
	}
	public float outsideLineWidth {
		set {
			if (value != m_outsideLineWidth) {
				outsideLineWidthChanged(value);
			}
		}
		get { return m_outsideLineWidth; }
	}


	public float m_insideBuffer = 0.25f;
	public void insideBufferChanged(float value){
		m_insideBuffer = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_f ("SetDataValue-If-Exists-map", "UnityObject", "inside-buffer-relative", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.insideBuffer = value;
					}
				}
			}
		}
		changed ();
	}
	public float insideBuffer {
		set {
			if (value != m_insideBuffer) {
				insideBufferChanged(value);
			}
		}
		get { return m_insideBuffer; }
	}

	public float m_outsideBuffer = 0.25f;
	public void outsideBufferChanged(float value){
		m_outsideBuffer = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_f ("SetDataValue-If-Exists-map", "UnityObject", "outside-buffer-relative", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.outsideBuffer = value;
					}
				}
			}
		}
		changed ();
	}
	public float outsideBuffer {
		set {
			if (value != m_outsideBuffer) {
				outsideBufferChanged(value);
			}
		}
		get { return m_outsideBuffer; }
	}



	public float m_maxOutsideDistance = 0.1f;
	public void maxOutsideDistanceChanged(float value){
		m_maxOutsideDistance = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_f ("SetDataValue-If-Exists-map", "UnityObject", "max-outside-distance-relative", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.maxOutsideDistance = value;
					}
				}
			}
		}
		changed ();
	}
	public float maxOutsideDistance {
		set {
			if (value != m_maxOutsideDistance) {
				maxOutsideDistanceChanged(value);
			}
		}
		get { return m_maxOutsideDistance; }
	}


	public bool m_shouldMoveWhenByPercentage = false;
	public void shouldMoveWhenByPercentageChanged(bool value){
		m_shouldMoveWhenByPercentage = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "should-move-when-by-percentage", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.shouldMoveWhenByPercentage = value;
					}
				}
			}
		}
		changed ();
	}
	public bool shouldMoveWhenByPercentage {
		set {
			if (value != m_shouldMoveWhenByPercentage) {
				shouldMoveWhenByPercentageChanged(value);
			}
		}
		get { return m_shouldMoveWhenByPercentage; }
	}

	public float m_moveWhenNotVisiblePercentage = 0.0f;
	public void moveWhenNotVisiblePercentageChanged(float value){
		m_moveWhenNotVisiblePercentage = value;
		if (!DPUtils.isPrefab (gameObject)) {
			DPManagerScript.Call_s_s_s_i_f ("SetDataValue-If-Exists-map", "UnityObject", "percentage-overlap-with-own-visible-space", "id", GetVMInstanceID (), value);
			DPManagerScript.Call_s_s_s_i_f ("SetDataValue-If-Exists-map", "UnityObject", "move-when-not-visible-percentage", "id", GetVMInstanceID (), value);
		}
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.moveWhenNotVisiblePercentage = value;
					}
				}
			}
		}
		changed ();
	}
	public float moveWhenNotVisiblePercentage {
		set {
			if (value != m_moveWhenNotVisiblePercentage) {
				moveWhenNotVisiblePercentageChanged(value);
			}
		}
		get { return m_moveWhenNotVisiblePercentage; }
	}






	public bool m_fadeIn = true;
	public void fadeInChanged(bool value){
		m_fadeIn = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "fade-in", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.fadeIn = value;
					}
				}
			}
		}
		changed ();
	}
	public bool fadeIn {
		set {
			if (value != m_fadeIn) {
				fadeInChanged(value);
			}
		}
		get { return m_fadeIn; }
	}

	public bool m_fadeOut = true;
	public void fadeOutChanged(bool value){
		m_fadeOut = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "fade-out", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.fadeOut = value;
					}
				}
			}
		}
		changed ();
	}
	public bool fadeOut {
		set {
			if (value != m_fadeOut) {
				fadeOutChanged(value);
			}
		}
		get { return m_fadeOut; }
	}


	public bool shouldResetFadeTime = false;
	public float resetFadeTime;

	public float m_fadeTime = 1.0f;
	public void fadeTimeChanged(float value){
		m_fadeTime = value;
		if (!DPUtils.isPrefab(gameObject))
			DPManagerScript.Call_s_s_s_i_f ("SetDataValue-If-Exists-map", "UnityObject", "fade-time", "id", GetVMInstanceID (), value);
		if (DPUtils.isPrefab(gameObject)){
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag){
				GameObject [] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos){
					VMObject vmo = go.GetComponent<VMObject>();
					if (vmo != null && vmo != this){
						vmo.fadeTime = value;
					}
				}
			}
		}
		changed ();
	}
	public float fadeTime {
		set {
			if (value != m_fadeTime) {
				fadeTimeChanged(value);
			}
		}
		get { return m_fadeTime; }
	}





	void OnBecameVisible(){
		visible = true;
	}
	void OnBecameInvisible() {
		visible = false;
	}

	static int [] bbxindices = new int[] {
		3,2,1,0,-1, // bottom
		4,5,6,7,-1, // top
		1,5,4,0,-1, // left
		2,3,7,6,-1, // right
		2,6,5,1,-1, // front
		0,4,7,3,-1,  // back
	};

	public static void GetMeshListForObject(List< KeyValuePair<MeshFilter,Mesh> > meshList, GameObject obj, ref int nverts){
		HashSet<int> hs = new HashSet<int> ();
		{
			Mesh mesh = null;
			MeshFilter mf = obj.GetComponent<MeshFilter> ();
			VMGeometry vmgeo = obj.GetComponent<VMGeometry> ();
			if (vmgeo == null || !vmgeo.doNotInclude) {
				if (mf != null) {
					if (mf.sharedMesh != null) {
						mesh = mf.sharedMesh;
					} else {
						mesh = mf.mesh;
					}
				}
				if (mesh != null) {
					hs.Add(mf.GetInstanceID());
					meshList.Add (new KeyValuePair<MeshFilter,Mesh> (mf, mesh));
					nverts += mesh.vertexCount;
				}
			}
		}
		MeshFilter[] mfs = obj.GetComponentsInChildren<MeshFilter> ();
		if (mfs != null) {
			foreach (MeshFilter mf in mfs){
				VMGeometry vmgeo = mf.gameObject.GetComponent<VMGeometry> ();
				if (vmgeo != null && vmgeo.doNotInclude) {
					continue;
				}
				if (hs.Contains(mf.GetInstanceID())){
					continue;
				}
				if (mf.sharedMesh != null) {
					meshList.Add (new KeyValuePair<MeshFilter,Mesh>(mf, mf.sharedMesh) );
					nverts += mf.sharedMesh.vertexCount;
				} else if (mf.mesh != null){
					meshList.Add (new KeyValuePair<MeshFilter,Mesh>(mf, mf.mesh) );
					nverts += mf.mesh.vertexCount;
				}
			}
		}
	}
	public static Bounds GetBoundsFromMeshList(List< KeyValuePair<MeshFilter,Mesh> > meshList, GameObject obj){
		Matrix4x4 worldToHead;
		if (obj != null) {
			worldToHead = obj.transform.worldToLocalMatrix;
		} else {
			worldToHead = Matrix4x4.identity;
		}
		Mesh mesh = meshList [0].Value;
		Transform trans = meshList [0].Key.gameObject.transform;
		Vector3 vert = trans.localToWorldMatrix.MultiplyPoint(meshList [0].Value.vertices [0]);
		Bounds bounds = new Bounds (vert, new Vector3 ());
		foreach (KeyValuePair<MeshFilter,Mesh> kvp in meshList){
			mesh = kvp.Value;
			trans = kvp.Key.gameObject.transform;
			List<Vector3> verts = new List<Vector3>();
			mesh.GetVertices(verts);
			foreach (Vector3 v in verts){
				Vector3 pt = trans.localToWorldMatrix.MultiplyPoint (v);
				bounds.Encapsulate (pt);
			}
		}
		return bounds;
	}
	public static Bounds GetBoundsFromMeshListBounds(List< KeyValuePair<MeshFilter,Mesh> > meshList, GameObject obj){
		Matrix4x4 worldToHead;
		if (obj != null) {
			worldToHead = obj.transform.worldToLocalMatrix;
		} else {
			worldToHead = Matrix4x4.identity;
		}
		Mesh mesh = meshList [0].Value;
		Transform trans = meshList [0].Key.gameObject.transform;
		Vector3 vert = trans.localToWorldMatrix.MultiplyPoint(meshList [0].Value.vertices [0]);
		Bounds bounds = new Bounds (vert, new Vector3 ());
		foreach (KeyValuePair<MeshFilter,Mesh> kvp in meshList){
			mesh = kvp.Value;
			trans = kvp.Key.gameObject.transform;
			bounds.Encapsulate (trans.localToWorldMatrix.MultiplyPoint (mesh.bounds.min));
			bounds.Encapsulate (trans.localToWorldMatrix.MultiplyPoint (mesh.bounds.max));
		}
		return bounds;
	}

    private Vector3 geometryCentroid = Vector3.zero;
	private bool geometryCentroidIsSet = false;
    public Vector3 geometryCentroidGet() { return geometryCentroid;  }
	public void reloadGeometry(){
		if (DPUtils.isPrefab (gameObject))
			return;
		ViewManager vm = FindObjectOfType<ViewManager> ();
		Matrix4x4 worldToTop = Matrix4x4.identity;
		Matrix4x4 topToWorld = Matrix4x4.identity;			
		if (vm.topWorldObject != null) {
			worldToTop = vm.topWorldObject.transform.worldToLocalMatrix;
			topToWorld = vm.topWorldObject.transform.localToWorldMatrix;
		}
		Matrix4x4 mat = transform.localToWorldMatrix;
		float[] coords = null;
		int[] indices = null;
		int nverts = 0;
		List< KeyValuePair<MeshFilter,Mesh> > meshList = new List< KeyValuePair<MeshFilter,Mesh> >() ;
		GetMeshListForObject (meshList, gameObject, ref nverts);
		if (meshList.Count == 0) {
			Debug.Log ("WARNING: VMObject.reloadGeometry() has no geometry name=" + gameObject.name);
			return;
        }
		VMGeometry sgeo = GetComponent<VMGeometry> ();
		float[] splitplaneverts = null;
		if (sgeo != null) {
			if (sgeo.splitPlanes != null) {
				MeshFilter mf = sgeo.splitPlanes.GetComponent<MeshFilter> ();
				if (mf != null) {
					List<Vector3[]> allPlanes = new List<Vector3[]> ();
					int nsplitverts = 0;
					if (!DPUtils.isPrefab(sgeo.splitPlanes) && mf.mesh != null) nsplitverts += AddToAllPlanes (allPlanes, mf.mesh);
					if (nsplitverts == 0 && mf.sharedMesh != null) nsplitverts += AddToAllPlanes (allPlanes, mf.sharedMesh);
					splitplaneverts = new float[nsplitverts * 3];
					int pl = 0;
					// TODO : NEED TO CHANGE EITHER SPLIT PLANES ON HALLWAY OR THIS
					Matrix4x4 l2wmat = worldToTop * sgeo.splitPlanes.transform.localToWorldMatrix;
					foreach (Vector3 [] planearr in allPlanes){
						for (int i = 0; i < planearr.Length; i++) {
							Vector3 v = planearr [i];
							v = l2wmat.MultiplyPoint (v);
							splitplaneverts [pl++] = v.x; splitplaneverts [pl++] = v.y; splitplaneverts [pl++] = v.z;
						}
					}
				}
			}
		}
		if (geometryMode == 2){ // from geometry
			coords = new float[nverts*3];
			List<KeyValuePair<int[], int> > allsmidxs = new List<KeyValuePair<int[], int> > ();
			int nidx = 0;
			int pl = 0;
			foreach (KeyValuePair<MeshFilter,Mesh> kvp in meshList) {
				Mesh mesh = kvp.Value;
				Matrix4x4 transL2W = worldToTop * kvp.Key.transform.localToWorldMatrix;
				Vector3[] verts = mesh.vertices;
				foreach (Vector3 coord in verts) {
					Vector3 c = transL2W.MultiplyPoint(coord);
					coords [pl++] = c.x;
					coords [pl++] = c.y;
					coords [pl++] = c.z;
				}
				int smc = mesh.subMeshCount;
				for (int i = 0; i < smc; i++) {
					int[] idxs = mesh.GetIndices (i);
					int newnidx = 4 * (idxs.Length / 3);
					nidx += newnidx; // need to add -1 to every triangle
					allsmidxs.Add (new KeyValuePair<int[], int>(idxs, (i==0) ? verts.Length : 0) );
				}
			}
			indices = new int[nidx];
			pl = 0;
			int startpl = 0;
			foreach (KeyValuePair<int[], int> idxinfo in allsmidxs) {
				int[] idxs = idxinfo.Key;
				for (int n = 0; n < idxs.Length;) {
					indices [pl++] = startpl + idxs [n++];
					indices [pl++] = startpl + idxs [n++];
					indices [pl++] = startpl + idxs [n++];
					indices [pl++] = -1; // need to add -1 to every triangle
				}
				startpl += idxinfo.Value;
			}
            Bounds bounds = GetBoundsFromMeshList(meshList, gameObject);
            geometryCentroid = bounds.center; // used by showWhenNotVisible for line (currently)
        } else if (geometryMode == 0) { // from bbx
			/* calculate bounds of object from mesh by transforming each vertex */
			Bounds bounds = GetBoundsFromMeshList(meshList, gameObject);
			//bounds.Expand (-5.0f);
			Vector3 min = worldToTop * bounds.min;
			Vector3 max = worldToTop * bounds.max;
			VMGeometry geo = GetComponent<VMGeometry> ();
			if (geo != null) {
				min.x += geo.xMinInset;
				min.y += geo.yMinInset;
				min.z += geo.zMinInset;
				max.x -= geo.xMaxInset;
				max.y -= geo.yMaxInset;
				max.z -= geo.zMaxInset;
			}
			coords = new float[] {
				min.x, min.y, min.z,
				min.x, min.y, max.z,
				max.x, min.y, max.z,
				max.x, min.y, min.z,
				min.x, max.y, min.z,
				min.x, max.y, max.z,
				max.x, max.y, max.z,
				max.x, max.y, min.z
			};
			indices = bbxindices;
            geometryCentroid = bounds.center;
        } else if (geometryMode == 1) { // from bbx rotated
			Matrix4x4 worldToHead = transform.worldToLocalMatrix;
			Bounds bounds = new Bounds();
			bool first = true;
			foreach (KeyValuePair<MeshFilter,Mesh> kvp in meshList) {
				Mesh mesh = kvp.Value;
				Transform trans = kvp.Key.gameObject.transform;
				Matrix4x4 localToHead = worldToHead * trans.localToWorldMatrix;
				Vector3 bmin = localToHead.MultiplyPoint (mesh.bounds.min);
				Vector3 bmax = localToHead.MultiplyPoint (mesh.bounds.max);
				if (first) {
					bounds = new Bounds (bmin, Vector3.zero);
					first = false;
				} else {
					bounds.Encapsulate (bmin);
				}
				bounds.Encapsulate (bmax);
			}
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			Vector3 [] coordsV = new Vector3[] {
				new Vector3(min.x, min.y, min.z),
				new Vector3(min.x, min.y, max.z),
				new Vector3(max.x, min.y, max.z),
				new Vector3(max.x, min.y, min.z),
				new Vector3(min.x, max.y, min.z),
				new Vector3(min.x, max.y, max.z),
				new Vector3(max.x, max.y, max.z),
				new Vector3(max.x, max.y, min.z)
			};
			coords = new float[24];
			int pl = 0;
			foreach(Vector3 coord in coordsV){
				Vector3 pt = mat.MultiplyPoint (coord);
				coords[pl++] = pt.x;
				coords[pl++] = pt.y;
				coords[pl++] = pt.z;
			}
			indices = bbxindices;
            geometryCentroid = mat.MultiplyPoint(bounds.center);
        }
		geometryCentroidIsSet = true;
		DPManagerScript.Call_i_iba_fba_fba_b_b_b( "Set-UnityObject-Geometry-map", GetVMInstanceID(), 
																				  indices, sizeof(int) * indices.Length, 
																				  coords, sizeof(float) * coords.Length,
																				  splitplaneverts, sizeof(float) * (splitplaneverts!=null ? splitplaneverts.Length : 0),
																				  geometryMode == 0, geometryMode == 1, geometryMode == 2);
	}
	// Use this for initialization
	bool isInitialized = false;
    void Start() {
        Init();
    }
    void Init() {
		if (DPUtils.isPrefab (gameObject)) {
			Debug.Log ("WARNING: VMObject.Init() : should prefab be started?");
			return;
		}
        if (m_annotationIsInit)
            return;
		int goid = gameObject.GetInstanceID ();
		/*if (m_VMObjectsByGameObjectID.ContainsKey (goid)) {
			//Debug.Log ("WARNING: VMObject with name '" + name + "' has Multiple VMObjects attached, please remove");
			return;
		}*/
		m_VMObjects [GetVMInstanceID ()] = gameObject;
		m_VMObjectsByGameObjectID [goid] = gameObject;
		if (textForLabel == null || m_textForLabel.Trim ().Length == 0)
			textForLabel = GetVMInstanceID ().ToString ();
		if (textannotation == null) {
            TextMesh tm = null;
            string tname = null;
            if (m_annotation != null)
            {
                tname = m_annotation.name;
                Bounds abounds = DPUtils.GenerateLocalBounds(m_annotation);
                m_annotation = Object.Instantiate(m_annotation);
				CameraFacing cf = m_annotation.GetComponent<CameraFacing> ();
				if (cf != null) {
					cf.enabled = false;
				}
                m_annotation.transform.localPosition = Vector3.zero;
                m_annotation.transform.localScale = Vector3.one;
                m_annotation.transform.localRotation = Quaternion.identity;
                Bounds abounds2 = DPUtils.GenerateLocalBounds(m_annotation);
                orig_m_annotationBounds = abounds2;
                m_annotationIsInit = true;
            }
            else
            {
                textannotation = new GameObject();
                textannotation.AddComponent<MeshRenderer>();
                MeshRenderer mr = textannotation.GetComponent<MeshRenderer>();
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                string txt = textForLabel;
                tm = textannotation.AddComponent<TextMesh>();
                tname = textForLabel;
                tm.anchor = TextAnchor.MiddleCenter;
                tm.alignment = TextAlignment.Center;
                tm.text = txt;
                tm.fontSize = 60;
                origAnnotationText = txt;
                textannotation.name = "Label_" + tm.text;
                gameObject.name = gameObject.name + "_" + tm.text;
            }
            ViewManager vm = FindObjectOfType<ViewManager>();
            if (vm.topWorldObject != null)
            {
                setAnnotationParent(vm.topWorldObject.gameObject.transform);
            }
            annotationline = new GameObject ();
			annotationline.name = "Label_Line_" + tname;
			annotationline.AddComponent<LineRenderer> ();
			SetAnnotationsActive (false, false);
			LineRenderer lr = annotationline.GetComponent<LineRenderer> ();
			if (vm.topWorldObject != null) {
				annotationline.transform.SetParent (vm.topWorldObject.gameObject.transform);
				lr.useWorldSpace = false;
			}
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            lr.startWidth = lr.endWidth = 0.1f;
			Material lineMaterial = (Material)Resources.Load("LineMaterial", typeof(Material));
            lineMaterial.renderQueue = 4000;
			lr.material = lineMaterial;
			Material material = lr.material;
			Color newColor = new Color (1.0f, 1.0f, 1.0f, 0.0f);
			lr.startColor = lr.endColor = newColor;
			if (tm!=null)
                tm.color = newColor;
			if (vm.topWorldObject == null) {
				int layer = DPUtils.addLayerIfNot ("UI");
				annotationline.layer = layer;
                if (textannotation != null)
                    textannotation.layer = layer;
                if (m_annotation != null)
                    m_annotation.layer = layer;
            }
		}
		MeshFilter mf = GetComponent<MeshFilter> ();
		Mesh mesh = null;
		if (mf != null)
			mesh = mf.mesh;
		if (mesh == null) {
			bool hasMesh = false;
			MeshFilter[] mfs = GetComponentsInChildren<MeshFilter> ();
			foreach (MeshFilter mfsi in mfs) {
				Mesh meshi = mfsi.mesh;
				if (meshi != null) {
					hasMesh = true;
					break;
				}
			}
			if (!hasMesh) {
				Debug.Log ("WARNING: VMObject.Start() has no geometry mfs.length=" + mfs.Length);
				return;
			}
		}
		isInitialized = true;
	}
	bool addedToDP = false, onQueueToAdd = false;
    public void ResetDP()
    {
        turnOff(false);
        addedToDP = false;
        onQueueToAdd = false;
        if (enabled){
            bool executedNow = DPManagerScript.execute(new DPActionFunc
            {
                actionPerformedFunc = () => AddToDP()
            });
            onQueueToAdd = !executedNow; // delayed until DP comes up
        }
    }
    int AddToAllPlanes(List<Vector3[]> allPlanes, Mesh mesh){
		int nverts = 0;
		for (int i = 0; i < mesh.subMeshCount; i++) {
			Vector3[] verts = mesh.vertices;
			int[] idxs = mesh.GetIndices (i);
			Vector3[] vertsarr = new Vector3[idxs.Length];
			for (int j=0; j<idxs.Length; j++){
				vertsarr[j] = verts[idxs[j]];
			}
			nverts += idxs.Length;
			allPlanes.Add(vertsarr);
		}
		return nverts;
	}
	void AddToDP(){
        if (addedToDP)
        {
            //Debug.Log("WARNING: VMObject.AddToDP called when already added name=" + name);
            return;
        }
        if (!isInitialized)
        {
            Init();
        }
        onQueueToAdd = false;
        int coordSystemID = 0;
		if (coordinateSystem!=null && coordinateSystem.level != 0) {
			coordSystemID = coordinateSystem.GetInstanceID ();
		}
		DPManagerScript.Call_i_s_i ("Create-UnityObject-map", GetVMInstanceID (), gameObject.name, coordSystemID);
		addedToDP = true;
		minHeightChanged(minHeight);
		maxHeightChanged(maxHeight);
		sizeIsDefaultChanged(sizeIsDefault);
		tryToLabelChanged (tryToLabel);
        showWhenNotVisibleChanged(showWhenNotVisible);
        placeWithDirectViewChanged(placeWithDirectView);
        requestPlacementWithDirectViewChanged(requestPlacementWithDirectView);
        whenShowInDirectPlacementChanged(whenShowInDirectPlacement);
        screenStabilizedChanged(screenStabilized);
		canMoveChanged (canMove);
		tryToLabelInsideChanged (tryToLabelInside);
		tryToLabelOutsideChanged (tryToLabelOutside);
		tryToLabelOnCentroidChanged (m_tryToLabelOnCentroid);
		planeOrientedChanged (planeOriented);
		interpolateBetweenChanged(interpolateBetween);
		scheduleModeChanged (scheduleMode);
		scheduleDurationEveryMSChanged (scheduleDurationEveryMS);
		hysteresisModeChanged (hysteresisMode);
		zModeChanged (zMode);
		alwaysFacingChanged (alwaysFacing);
		constantZDistanceChanged (constantZDistance);
		outsideLineWidthChanged (outsideLineWidth);
		insideBufferChanged (insideBuffer);
		outsideBufferChanged (outsideBuffer);
		maxOutsideDistanceChanged (maxOutsideDistance);
		shouldMoveWhenByPercentageChanged(m_shouldMoveWhenByPercentage);
		moveWhenNotVisiblePercentageChanged(m_moveWhenNotVisiblePercentage);
		fadeInChanged (m_fadeIn);
		fadeOutChanged (m_fadeOut);
		fadeTimeChanged (m_fadeTime);
		geometryModeChanged(geometryMode);
		if (textForLabel == null || textForLabel.Trim ().Length == 0) {
			textForLabel = GetVMInstanceID ().ToString();
		}
		textForLabelChanged (textForLabel);
		coordinateSystemChanged (coordinateSystem);
		UpdateAnnotationBounds();

		changed ();
#if UNITY_EDITOR
		ViewManagerScript.invalidate ();
#endif
	}
	public void CallAllChanged(){
		DPManagerScript.execute (new DPActionFunc {
			actionPerformedFunc = () => CallAllChangedImpl ()
		});
	}
	void CallAllChangedImpl(){
		minHeightChanged(minHeight);
		maxHeightChanged(maxHeight);
		sizeIsDefaultChanged(sizeIsDefault);
		tryToLabelChanged (tryToLabel);
        showWhenNotVisibleChanged(showWhenNotVisible);
        placeWithDirectViewChanged(placeWithDirectView);
        requestPlacementWithDirectViewChanged(requestPlacementWithDirectView);
        whenShowInDirectPlacementChanged(whenShowInDirectPlacement);
        screenStabilizedChanged(screenStabilized);
        canMoveChanged(canMove);
		tryToLabelInsideChanged (tryToLabelInside);
		tryToLabelOutsideChanged (tryToLabelOutside);
		tryToLabelOnCentroidChanged (m_tryToLabelOnCentroid);
		planeOrientedChanged (planeOriented);
		interpolateBetweenChanged(interpolateBetween);
		scheduleModeChanged (scheduleMode);
		scheduleDurationEveryMSChanged (scheduleDurationEveryMS);
		hysteresisModeChanged (hysteresisMode);
		zModeChanged (zMode);
		alwaysFacingChanged (alwaysFacing);
		constantZDistanceChanged (constantZDistance);
		outsideLineWidthChanged (outsideLineWidth);
		insideBufferChanged (insideBuffer);
		outsideBufferChanged (outsideBuffer);
		maxOutsideDistanceChanged (maxOutsideDistance);
		shouldMoveWhenByPercentageChanged(m_shouldMoveWhenByPercentage);
		moveWhenNotVisiblePercentageChanged(m_moveWhenNotVisiblePercentage);
		fadeInChanged (m_fadeIn);
		fadeOutChanged (m_fadeOut);
		fadeTimeChanged (m_fadeTime);
		geometryModeChanged(geometryMode);
		textForLabelChanged (textForLabel);
	}
	void RemoveFromDP(){
		DPManagerScript.Call_i( "Remove-UnityObject-With-ID-map", GetVMInstanceID() );
		addedToDP = false;
	}
	void OnDestroy(){
		RemoveFromDP ();
		if (textannotation != null) {
			Destroy (textannotation);
			textannotation = null;
		}
        if (m_annotation != null)
        {
            Destroy(m_annotation);
            m_annotation = null;
        }
        if (annotationline != null) {
			Destroy (annotationline);
			annotationline = null;
		}
		m_VMObjects.Remove (GetVMInstanceID ());
		m_VMObjectsByGameObjectID.Remove (gameObject.GetInstanceID ());;

#if UNITY_EDITOR
		if (VMObjectEditor.currentEditor != null) {
			Invoke ("SetDirtyImpl", .1f);
		}
#endif
	}
	void SetDirtyImpl(){
#if UNITY_EDITOR
		EditorUtility.SetDirty (VMObjectEditor.currentEditor.target);
#endif
	}
	public class StoreTransform
	{
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 localScale;
		public StoreTransform(Transform t){
			position = t.position;
			rotation = t.localRotation;
			localScale = t.localScale;
			t.position = new Vector3();
			t.rotation = Quaternion.identity;
			t.localScale = new Vector3(1.0f,1.0f,1.0f);
		}
		public void RestoreTransform(Transform t){
			t.position = position;
			t.localRotation = rotation;
			t.localScale = localScale;
		}
	}

	// Update is called once per frame
	public void UpdateAnnotationBounds(){
        if (textannotation != null)
        {
            StoreTransform st = new StoreTransform(textannotation.transform);
            MeshRenderer rend = textannotation.GetComponent<MeshRenderer>();
            annotationBoundsOrig = rend.bounds;
            float labelBBXAspectRatio = annotationBoundsOrig.size.x / annotationBoundsOrig.size.y;
            TextMesh tm = textannotation.GetComponent<TextMesh>();
            DPManagerScript.Call_s_s_s_i_f("SetDataValue-If-Exists-map", "UnityObject", "annotation-aspect-ratio", "id", GetVMInstanceID(), labelBBXAspectRatio);
            origAnnotationText = tm.text;
            st.RestoreTransform(textannotation.transform);
        } else if (m_annotation != null)
        {
            //Bounds abounds = DPUtils.GenerateLocalBounds(m_annotation);
            annotationBoundsOrig = orig_m_annotationBounds;
            float labelBBXAspectRatio = annotationBoundsOrig.size.x / annotationBoundsOrig.size.y;
            DPManagerScript.Call_s_s_s_i_f("SetDataValue-If-Exists-map", "UnityObject", "annotation-aspect-ratio", "id", GetVMInstanceID(), labelBBXAspectRatio);
        }
    }
    void ComputePitchAndYAngleFromCamera(out float angleBetweenInY, out float pitchAngle, Vector3 vup)
    {
		Camera curCamera = ViewManager.getCurrentCamera ();
		Vector3 wTarget = getWorldTargetPoint ();
		Vector3 objectToCamVect = wTarget - curCamera.transform.position;
        Vector3 objectToCamNoPitch = objectToCamVect;
        objectToCamNoPitch.y = 0;
        objectToCamNoPitch.Normalize();
        objectToCamVect.Normalize();
		Vector3 forward = curCamera.transform.forward;
        Vector3 forwardNoY = forward;
        forwardNoY.y = 0.0f;
        forwardNoY.Normalize();
        angleBetweenInY = Vector3.Angle(objectToCamNoPitch, forwardNoY);
        Vector3 crossp = Vector3.Cross(objectToCamNoPitch, forwardNoY);
        if (crossp.y > 0) angleBetweenInY = -angleBetweenInY;
        Quaternion q = Quaternion.AngleAxis(angleBetweenInY, vup);
        Vector3 forwardR = q * forward;

        pitchAngle = Vector3.Angle(objectToCamVect, forwardR);
        Vector3 crossp2 = Vector3.Cross(objectToCamVect, forwardR);
        Vector3 crossp3 = Vector3.Cross(crossp2, objectToCamVect);
        if (crossp3.y > 0) pitchAngle = -pitchAngle;
		//Debug.Log ("ComputePitchAndYAngleFromCamera: name=" + gameObject.name + " wTarget=" + wTarget + " target=" + getTargetPoint() + 
		//	" directViewPlacement.linePoint2=" + directViewPlacement.linePoint2 + " fromWorldPt=" + directViewPlacement.fromWorldPoint + " worldPt=" + directViewPlacement.worldPoint + " campos=" + curCamera.transform.position + 
		//	" origobjectToCamVect=" + origobjectToCamVect + " angleBetweenInY=" + angleBetweenInY + " pitchAngle=" + pitchAngle);
    }
    private string origAnnotationText = null;
    Matrix4x4 prevTransMatrix = new Matrix4x4();
	void Update () {
		if (!isInitialized)
			return;
		ViewManager vm = FindObjectOfType<ViewManager> ();
		if (enabled != addedToDP) {
			if (enabled) {
				if (!onQueueToAdd)
				{
					bool executedNow = DPManagerScript.execute(new DPActionFunc {
						actionPerformedFunc = () => AddToDP()
					});
					onQueueToAdd = !executedNow; // delayed until DP comes up
				}
			} else
				RemoveFromDP ();
		}

        if (!getActiveSelf()){
			if (tryToLabel) {
/*                if (showWhenNotVisible)
                {
                    // if showWhenNotVisible and not active, place in middle of screen
					if (!screenStabilized) {
						placeInMiddleOfScreen ();
						screenStabilized = true;
					}
                }*/
            } else {
				return;
			}
        }
        string goname = gameObject.name;
		Camera curCamera = ViewManager.getCurrentCamera ();
		Matrix4x4 worldToCam = curCamera.transform.worldToLocalMatrix;
		Matrix4x4 annToWorld = getAnnotationLocalToWorld();
		bool prevActive = getActiveSelf();
        if (whenShowInDirectPlacement)
        {
			{
				float angleBetweenInY, pitchAngle;
				Vector3 vup;
				bool isHMD = vm.screenProjectionMode == 1 || vm.screenProjectionMode == 2;
				bool isVive = vm.screenProjectionMode == 2;
				if (isHMD)
					vup = Vector3.up;  // Y-up, not camera up, looks weird in VR
                else
					vup = curCamera.transform.up;
				ComputePitchAndYAngleFromCamera (out angleBetweenInY, out pitchAngle, vup);
				float fov, fovh;
				if (isVive) {
					fov = curCamera.fieldOfView * (1.0f - 2.0f * vm.closeToEdgeDistance) / 1.25f;
					fovh = fov * curCamera.pixelWidth / (float)curCamera.pixelHeight;
				} else {
					fov = curCamera.fieldOfView / 1.5f;// / 3.0f; //  1.5f; // 2.0f;
					fovh = fov * curCamera.pixelWidth / (float)curCamera.pixelHeight;
				}

				if (Mathf.Abs (angleBetweenInY) <= fov && Mathf.Abs (pitchAngle) <= fovh) {
					if (behaviorMode == 2 || directViewPlacement == null) {
						turnOff (false);
						//Debug.Log ("turnOff name=" + gameObject.name);
						return;
					}
					screenStabilized = false;
					turnDirectViewPlacement (true);
				} else {
					if (behaviorMode == 2) {
						//screenStabilizedIsAnimating = false;
						SetAnnotationsActive (true, true);
					}
					turnDirectViewPlacement (false);
					screenStabilized = true;
					if (!prevActive)
						placeMinScale ();
				}
			}
        }
        if (screenStabilized)
        {
            bool isHMD = vm.screenProjectionMode == 1 || vm.screenProjectionMode == 2;
            bool isVive = vm.screenProjectionMode == 2;
			Vector3 cright = ViewManager.getWorldToTopMatrix ().MultiplyVector(curCamera.transform.right);
			Vector3 cup = ViewManager.getWorldToTopMatrix ().MultiplyVector(curCamera.transform.up);
			Vector3 screenPoint = curCamera.WorldToScreenPoint(getAnnotationPosition());
            float zval;
            float zscale;
			Vector3 labelSizeInPixels;
			Vector3 wtPoint = getWorldTargetPoint1 ();
			Vector3 targetScreenPoint = curCamera.WorldToScreenPoint(wtPoint);
			Vector3 objectToCamVect = wtPoint - curCamera.transform.position;
			zval = objectToCamVect.magnitude;
			zscale = DPUtils.getScaleFromZDistance(zval);
			float screenMatScale = (((float)m_minHeight) / annotationBoundsOrig.size.y);

			labelSizeInPixels = .5f * annotationBoundsOrig.size * screenMatScale;

            Vector3 vup;
            if (isHMD)
                vup = Vector3.up;  // Y-up, not camera up, looks weird in VR
            else
				vup = curCamera.transform.up;

            if (notVisibleLayoutMode == 1) // closest to projection
            {
                bool byVector = isHMD;
                Vector2 pt = new Vector2();
                if (byVector)
                {
                    float angleBetweenInY, pitchAngle;
                    ComputePitchAndYAngleFromCamera(out angleBetweenInY, out pitchAngle, vup);
                    float cTEdgeDist = vm.closeToEdgeDistance;
					float fov = (1.0f - 2.0f * cTEdgeDist) * curCamera.fieldOfView / 2.0f;
                    int[] closestToEdgeRect = vm.closestToEdgeRect;
                    float angleBetweenInYNorm = (angleBetweenInY + fov)/ (2.0f * fov);
                    float pitchAngleNorm = (pitchAngle + fov) / (2.0f * fov);
                    if (angleBetweenInYNorm <= 0.0f)
                        pt.x = closestToEdgeRect[0];
                    else if (angleBetweenInYNorm >= 1.0f)
                        pt.x = closestToEdgeRect[2];
                    else {
                        pt.x = angleBetweenInYNorm * (closestToEdgeRect[2] - closestToEdgeRect[0]) + closestToEdgeRect[0];
                    }
                    if (pitchAngleNorm <= 0.0f)
                        pt.y = closestToEdgeRect[1];
                    else if (pitchAngleNorm >= 1.0f)
                        pt.y = closestToEdgeRect[3];
                    else {
                        pt.y = pitchAngleNorm * (closestToEdgeRect[3] - closestToEdgeRect[1]) + closestToEdgeRect[1];
                    }

                    bool resetTarget = targetScreenPoint.z <= 0.0f;
                    if (resetTarget){
						float pW = curCamera.pixelWidth / 2.0f;
						float pH = curCamera.pixelHeight / 2.0f;
						float ffov = curCamera.fieldOfView / 2.0f;
                        targetScreenPoint.x = ((ffov + angleBetweenInY) * pW / ffov);
                        targetScreenPoint.y = ((ffov + pitchAngle) * pH / ffov);
                        // check to see if it moved enough to animate
						Vector3 spt = curCamera.WorldToScreenPoint(getAnnotationPosition());
                        Vector2 sptdiff = new Vector2(pt.x - spt.x, pt.y - spt.y);
                        if (prevActive && sptdiff.magnitude > 100.0f)
                            screenStabilizedIsAnimating = true;
                    }
                    if (screenStabilizedIsAnimating)
                    {
						Vector3 spt = curCamera.WorldToScreenPoint(getAnnotationPosition());
                        Vector2 sptdiff = new Vector2(pt.x - spt.x, pt.y - spt.y);
                        if (sptdiff.magnitude < vm.screenStabilizedAnimationSpeed)
                            screenStabilizedIsAnimating = false;
                        else
                        {
                            sptdiff = vm.screenStabilizedAnimationSpeed * sptdiff.normalized;
                        }
                        screenPoint = new Vector3(spt.x + sptdiff.x, spt.y + sptdiff.y, zval);
                    }
                    else
                    {
                        screenPoint = new Vector3(pt.x, pt.y, zval);
                    }
                    setAnnotationPosition(curCamera.ScreenToWorldPoint(screenPoint));
                }
                else
                {
                    int[] closestToEdgeRect = vm.closestToEdgeRect;

                    if (targetScreenPoint.z < 0.0f)  // if its behind, just flip relative to middle of screen
                    {
						targetScreenPoint.x = (curCamera.pixelWidth / 2.0f) - targetScreenPoint.x;
						targetScreenPoint.y = (curCamera.pixelHeight / 2.0f) - targetScreenPoint.y;
                        targetScreenPoint.z = -targetScreenPoint.z;
                    }

                    if (targetScreenPoint.x - labelSizeInPixels.x < closestToEdgeRect[0])
                        pt.x = closestToEdgeRect[0] + labelSizeInPixels.x;
                    else if (targetScreenPoint.x + labelSizeInPixels.x > closestToEdgeRect[2])
                        pt.x = closestToEdgeRect[2] - labelSizeInPixels.x;
                    else
                        pt.x = targetScreenPoint.x;
                    if (targetScreenPoint.y - labelSizeInPixels.y < closestToEdgeRect[1])
                        pt.y = closestToEdgeRect[1] + labelSizeInPixels.y;
                    else if (targetScreenPoint.y + labelSizeInPixels.y > closestToEdgeRect[3])
                        pt.y = closestToEdgeRect[3] - labelSizeInPixels.y;
                    else
                        pt.y = targetScreenPoint.y;

					Vector3 centerScreen = curCamera.transform.position + curCamera.transform.forward * zval;

					Vector3 spt = curCamera.WorldToScreenPoint (centerScreen);
                    Vector2 sptdiff = new Vector2(pt.x - spt.x, pt.y - spt.y);
					if (prevActive && sptdiff.magnitude > 100.0f)
                        screenStabilizedIsAnimating = true;
                    Vector2 screenPoint2 = new Vector2(spt.x, spt.y);
					pt = pt - screenPoint2;
					if (!screenStabilizedPlacementIsSet) {
						screenStabilizedIsAnimating = false;
					} else if (screenStabilizedIsAnimating) {
						Vector2 diffV = pt - screenStabilizedPlacement;
						if (diffV.magnitude < vm.screenStabilizedAnimationSpeed) {
							screenStabilizedIsAnimating = false;
						} else {
							pt = screenStabilizedPlacement + (vm.screenStabilizedAnimationSpeed * diffV.normalized);
						}
					}
					screenStabilizedPlacement = pt;
					screenStabilizedPlacementIsSet = true;
					Vector3 apos = centerScreen +
						zscale * ((pt.x * cright) + (pt.y * cup));
                    setAnnotationPosition(ViewManager.getTopToWorldMatrix().MultiplyPoint(apos));
					//Vector3 curScale = annotation.transform.localScale;
                }
				screenPoint = curCamera.WorldToScreenPoint(getAnnotationPosition());
            }
            Quaternion newRot;
            if (isHMD){
				Vector3 viewVect = getAnnotationPosition() - curCamera.transform.position;
                newRot = new Quaternion();
                newRot.SetLookRotation(viewVect, vup);
                cright = newRot * Vector3.right;
                vup = newRot * Vector3.up;
            }
            else {
				newRot = curCamera.transform.localToWorldMatrix.GetRotation (); // * screenRot;
            }
            setAnnotationRotation(newRot);
            // need to set the line to point to the direction of the related object
            screenPoint.z = targetScreenPoint.z = 0.0f;
            Vector3 lineInScreen = targetScreenPoint - screenPoint;
			Vector3 startpos = getAnnotationLocalPosition();
            LineRenderer lr = annotationline.GetComponent<LineRenderer>();
            List<Vector3> linePts = new List<Vector3>();
            if (vm.debugLabelOutline) {
                Vector3 xdim, ydim;
				Vector3 asize2 = labelSizeInPixels * zscale;
				xdim = cright * asize2.x;
				ydim = vup * asize2.y;
                // four lines for debug label outline
				linePts.Add(startpos + xdim + ydim); linePts.Add(startpos - xdim + ydim);
				linePts.Add(startpos - xdim + ydim); linePts.Add(startpos - xdim - ydim);
				linePts.Add(startpos - xdim - ydim); linePts.Add(startpos + xdim - ydim);
				linePts.Add(startpos + xdim - ydim); linePts.Add(startpos + xdim + ydim);
             }
            if (notVisibleArrowMode == 1 || notVisibleArrowMode == 2)
            { // cut line or cut arrow
                float[] labelRect = { -labelSizeInPixels.x, -labelSizeInPixels.y,
                                      labelSizeInPixels.x, labelSizeInPixels.y };
                Vector2 center = new Vector2(.5f, .5f);
                Vector2 relPt = new Vector2();
                DPUtils.relativeTo(labelRect, lineInScreen, out relPt);
                if (!(lineInScreen.x >= labelRect[0] && lineInScreen.x <= labelRect[2] &&
                      lineInScreen.y >= labelRect[1] && lineInScreen.y <= labelRect[3]) &&
                    DPUtils.ClipLineToBox2d(ref relPt, center)){
                    DPUtils.relativeFrom(labelRect, relPt, out relPt);
					linePts.Add(lineEndPoint2 = startpos + zscale * (relPt.x * cright +
						relPt.y * cup));
                    if (notVisibleArrowMode == 1)
                    {
						linePts.Add(lineEndPoint1 = startpos + zscale * (lineInScreen.x * cright +
							lineInScreen.y * cup));
                    }
                    else 
                    {
                        Vector2 spt = new Vector2(screenPoint.x, screenPoint.y);
                        relPt += spt;
                        // find intersection of screen between targetScreenPoint and screenPoint
						float distRatio = isVive ? .25f : isHMD ? .75f : .5f;
						float closestToEdgeDistX = vm.closeToEdgeDistance * curCamera.pixelWidth;
						float closestToEdgeDistY = vm.closeToEdgeDistance * curCamera.pixelHeight;
                        float oneMinusDistRatio = 1.0f - distRatio;
                        float[] screenRect = new float[] { oneMinusDistRatio * closestToEdgeDistX,
                                                           oneMinusDistRatio * closestToEdgeDistY,
							curCamera.pixelWidth - oneMinusDistRatio * closestToEdgeDistX,
							curCamera.pixelHeight - closestToEdgeDistY * oneMinusDistRatio };
                        Vector2 tpt = new Vector2(targetScreenPoint.x, targetScreenPoint.y),
                                screenPoint2 = new Vector2(screenPoint.x, screenPoint.y);
                        DPUtils.relativeTo(screenRect, tpt, out tpt);
                        DPUtils.relativeTo(screenRect, relPt, out relPt);
                        if (DPUtils.ClipLineToBox2d(ref tpt, relPt))
                        {
                            DPUtils.relativeFrom(screenRect, tpt, out tpt);
                            DPUtils.relativeFrom(screenRect, relPt, out relPt);
                            relPt = relPt - screenPoint2;
                            tpt = tpt - screenPoint2;
							//tpt = new Vector2(targetScreenPoint.x, targetScreenPoint.y) - screenPoint2;
							Vector3 endpt = startpos + zscale * (tpt.x * cright + tpt.y * cup);
                            lineEndPoint1 = endpt;
                            linePts.Add(endpt);
							if (!screenStabilizedIsAnimating)
                            {
                                Vector2 linen = lineInScreen.normalized;
                                Vector2 norm = DPUtils.RotateVector2(linen, 90.0f);
                                float headSize = tpt.magnitude < 50.0f ? 5 : tpt.magnitude > 150.0f ? 10.0f : (5 + (5 * (tpt.magnitude - 50) / 100));
								Vector2 pt1 = headSize * (-linen + norm);
                                Vector2 pt2 = headSize * (-linen - norm);
								Vector3 arrpt1 = startpos + zscale * ((tpt.x + pt1.x) * cright + (tpt.y + pt1.y) * cup);
								Vector3 arrpt2 = startpos + zscale * ((tpt.x + pt2.x) * cright + (tpt.y + pt2.y) * cup);
                                linePts.Add(arrpt1);
                                linePts.Add(arrpt2);
                                linePts.Add(endpt);
                            }
                        }
                        else
                        {
							linePts.Add(lineEndPoint1 = startpos + zscale * (lineInScreen.x * cright + lineInScreen.y * cup));
                        }
                    }
                }
            }
            else 
            { // default full line (for now
                linePts.Add(lineEndPoint2 = startpos);
				linePts.Add(lineEndPoint1 = startpos + zscale * (lineInScreen.x * cright + lineInScreen.y * cup));
            }
            lr.positionCount = linePts.Count;
            lr.SetPositions(linePts.ToArray());
            lr.startWidth = lr.endWidth = lineWidthEnd = outsideLineWidth * DPUtils.getScaleFromZDistance(zval); //lineWidthEnd;
            annotationline.SetActive(true);
            return;
        }
        if (canMove) {
			if (vm.topWorldObject==null && !transform.worldToLocalMatrix.Equals(prevTransMatrix)) {
				// need to move, regenerate geometry, delete/add to BSPTree
				prevTransMatrix = transform.worldToLocalMatrix;
				reloadGeometry ();
			}
		}
		if (origAnnotationText != null) {
			TextMesh tm = textannotation.GetComponent<TextMesh> ();
			bool annotationBoundsOrigIsSet = false;
			if (!origAnnotationText.Equals (tm.text)) { // only change bounds for text changes (for now)
				annotationBoundsOrigIsSet = true;
			}
			if (annotationFontSize != tm.fontSize){
				annotationBoundsOrigIsSet = true;
				annotationFontSize = tm.fontSize;
			}
			if (annotationBoundsOrigIsSet)
				UpdateAnnotationBounds ();
		}
		if (isFadingOut) {
			float curTime = Time.time + Time.smoothDeltaTime;
			if (curTime >= startTimeFadeOut + m_fadeTime) {
				SetAnnotationsActive (false, false);
				isFadingOut = false;
				DPManagerScript.Call_i ("Turn-Off-Placements-For-map", GetVMInstanceID ());
				if (shouldResetFadeTime) {
					fadeTime = resetFadeTime;
				}
			} else {
				float alpha = (curTime - startTimeFadeOut) / m_fadeTime;
				SetAnnotationAlpha (alpha);
			}
			recomputeLine = false;
		}
		if (isFadingIn) {
			float curTime = Time.time + Time.smoothDeltaTime;
			if (curTime >= startTimeFadeIn + m_fadeTime) {
				isFadingIn = false;
				SetAnnotationAlpha (0.0f);
				if (shouldResetFadeTime) {
					fadeTime = resetFadeTime;
				}
			} else {
				float alpha = (curTime - startTimeFadeIn) / m_fadeTime;
				SetAnnotationAlpha (1.0f - alpha);
			}
		}

		if (isAnimating) {
			float curTime = Time.time + Time.smoothDeltaTime;
			if (curTime >= endTime) {
                // set to end animation values
                setAnnotationLocalPosition(endPoint);
                setAnnotationLocalScale(new Vector3(endScale, endScale, 1.0f));
                setAnnotationEulerAngles(endQuat.eulerAngles);
                isAnimating = false;
			} else {
				float interp = (curTime - startTime)/duration;
				float scale = Mathf.Lerp(startScale, endScale, interp);
                setAnnotationLocalPosition(Vector3.Lerp(startPoint, endPoint, interp));
                setAnnotationLocalScale(new Vector3(scale, scale, 1.0f));
                setAnnotationEulerAngles(Quaternion.Lerp(startQuat, endQuat, interp).eulerAngles);
            }
		}
//		Debug.Log ("name='" + gameObject.name + "' annotation.transform.localPosition=" + annotation.transform.localPosition + " position=" + annotation.transform.position +
//			"\ntransformed=" + (annotation.transform.parent.transform.localToWorldMatrix * annotation.transform.localPosition) + "\nlocalToWorld=" + annotation.transform.localToWorldMatrix + "\nworldToTop=" + ViewManager.getWorldToTopMatrix());
		if (isAnimatingLine) {
			float curTime = Time.time + Time.smoothDeltaTime;
			LineRenderer lr = annotationline.GetComponent<LineRenderer> ();
			if (curTime >= endTime) {
				// set to end animation values
				lr.positionCount = 2;
				lr.SetPositions (new Vector3[] { lineEndPoint1, lineEndPoint2 });
				//Debug.Log ("isAnimatingLine: finished lineEndPoint1=" + lineEndPoint1 + " lineEndPoint2=" + lineEndPoint2);
				lr.startWidth = lr.endWidth = lineWidthEnd;
				isAnimatingLine = false;
			} else {
				float interp = (curTime - startTime) / duration;
				lr.positionCount = 2;
				lr.SetPositions (new Vector3[] { Vector3.Lerp (lineStartPoint1, lineEndPoint1, interp),
					Vector3.Lerp (lineStartPoint2, lineEndPoint2, interp)
				});
				//Debug.Log ("isAnimatingLine: interp=" + interp + " lineStartPoint1=" + lineStartPoint1 + " lineEndPoint1=" + lineEndPoint1 + 
				//		   " lineStartPoint2=" + lineStartPoint2 + " lineEndPoint2=" + lineEndPoint2);
				float lw = Mathf.Lerp (lineWidthStart, lineWidthEnd, interp);
				lr.startWidth = lr.endWidth = lw;
			}
		}
		if (alwaysFacing && (m_scheduleMode != 1)) {
			// if always facing and not scheduled continuously, set oritentation towards the user
			// TODO: should exclude frames that do get layout for performance
			Quaternion q = new Quaternion ();
            Vector3 up;
            bool isHMD = vm.screenProjectionMode == 1 || vm.screenProjectionMode == 2;
            if (ViewManager.distortionCorrection && ViewManager.distortionCorrectionFunc!=null)
            {
				Vector3 screenPt = curCamera.WorldToScreenPoint(getAnnotationPosition());
                Vector2 vup = ViewManager.distortionCorrectionFunc.call(screenPt.x/curCamera.pixelWidth, screenPt.y/curCamera.pixelHeight);
                up = vup;
            } else {
                if (isHMD)
                    up = Vector3.up;  // Y-up, not camera up, looks weird in VR
                else
                    up = curCamera.transform.up;
            }
            Vector3 viewVect;
            if (isHMD)
				viewVect = getAnnotationPosition() - curCamera.transform.position;
            else
                viewVect = curCamera.transform.forward;
            q.SetLookRotation(viewVect, up);
            setAnnotationEulerAngles(q.eulerAngles);
			startQuat = endQuat = q;
		}
        if (showWhenNotVisible && !screenStabilized)
        {
            // need to check if projection goes outside rect, if it does, set to screen stabilized
            int[] closestToEdgeRect = FindObjectOfType<ViewManager>().closestToEdgeRect;
            Vector3 screenPt = curCamera.WorldToScreenPoint(endPoint);

            Matrix4x4 annToCam = worldToCam * annToWorld;
            float zscale;
			float screenMatScale = (minHeightFloat / annotationBoundsOrig.size.y);
			Vector3 asize = annotationBoundsOrig.size * screenMatScale;
            float zval;
            bool isHMD = vm.screenProjectionMode == 1 || vm.screenProjectionMode == 2;
            if (isHMD)
            {
				Vector3 tPoint = getTargetPoint ();
				Vector3 camInWorld = (ViewManager.getWorldToTopMatrix ().MultiplyPoint(curCamera.transform.position));
				Vector3 objectToCamVect = tPoint - camInWorld;
                zval = objectToCamVect.magnitude;
                zscale = DPUtils.getScaleFromZDistance(zval);
                if (screenPt.x < closestToEdgeRect[0] || screenPt.x > closestToEdgeRect[2] ||
                    screenPt.y < closestToEdgeRect[1] || screenPt.y > closestToEdgeRect[3])
                    //if (screenPt.x - asize.x < closestToEdgeRect[0] || screenPt.x + asize.x > closestToEdgeRect[2] ||
                    //    screenPt.y - asize.y < closestToEdgeRect[1] || screenPt.y + asize.y > closestToEdgeRect[3])
                    {
                        screenStabilized = true;
                }
            }
            else
            {
                zval = screenPt.z;
                zscale = DPUtils.getScaleFromZDistance(screenPt.z);
                asize = (annotationBoundsOrig.size * annToCam.GetScale().x / zscale) / 2.0f;
                if (screenPt.x - asize.x < closestToEdgeRect[0] || screenPt.x + asize.x > closestToEdgeRect[2] ||
                    screenPt.y - asize.y < closestToEdgeRect[1] || screenPt.y + asize.y > closestToEdgeRect[3])
                {
                    screenStabilized = true;
                }
            }
        }
	}
    // this gets called for all VMObjects after the ViewManager is called (not necessarily every frame)
	public void UpdateAnnotation () {
		if (!isInitialized)
			return;
		if (recomputeLine && !isFadingOut && !screenStabilized && !showingDirectViewPlacement) {
			if (noRecomputeLine)
				noRecomputeLine = false;
			else {
				// if not animating line, and not computing line, then the line should be repositioned
				// vmobj.lineEndPoint1 - world point
				DPManagerScript.Call_s_s_s_i ("Broadcasts-FieldSortedByWithKey-map", "UnityObject", "recompute-annotation-line", "id", GetVMInstanceID ());
			}
		}
	}

	/* computePlacement: Used in "Manual" Schedule mode to fire off a new placement for this VMObject annotation */
	public void computePlacement(){
		if (DPUtils.isPrefab (gameObject)) {
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.prefabsControlWithSameTag) {
				GameObject[] gos = GameObject.FindGameObjectsWithTag (gameObject.tag);
				foreach (GameObject go in gos) {
					VMObject vmo = go.GetComponent<VMObject> ();
					if (vmo != null && vmo != this) {
						DPManagerScript.Call_i ("Compute-Placement-For-UnityObject-map", vmo.GetVMInstanceID ());
					}
				}
			}
		} else {
			DPManagerScript.Call_i ("Compute-Placement-For-UnityObject-map", GetVMInstanceID ());
		}
	}
	public void setToFlyoverLayout(){
		DPManagerScript.execute (new DPActionFunc {
			actionPerformedFunc = () => setToFlyoverLayoutImpl ()
		});
	}
	public void setToFlyoverLayoutImpl(){
		moveWhenNotVisiblePercentage = .5f;
		shouldMoveWhenByPercentage = true;
		scheduleMode = 3; // only changed
		hysteresisMode = 1; // closest to previous
		zMode = 3;  // middle/center
	}
	public void setToStillLayout(){
		DPManagerScript.execute (new DPActionFunc {
			actionPerformedFunc = () => setToStillLayoutImpl ()
		});
	}
	public void setToStillLayoutImpl(){
		shouldMoveWhenByPercentage = false;
		scheduleMode = 0; // manual
		hysteresisMode = 0; // best
		zMode = 3;  // middle/center
		//Debug.Log("setToStillLayoutImpl after zMode is set");
		DPManagerScript.Call_s_s_s_i_b ("SetDataValue-If-Exists-map", "UnityObject", "schedule-satisfied-Manual", "id", GetVMInstanceID (), false);
	}
	public void setToContinuousImpl(){
		shouldMoveWhenByPercentage = false;
		scheduleMode = 1; // continuous
		hysteresisMode = 0; // best
		zMode = 3;  // middle/center
	}
	public void setToContinuous(){
		DPManagerScript.execute (new DPActionFunc {
			actionPerformedFunc = () => setToContinuousImpl ()
		});
	}
	public void turnOff(bool checkFade){
		if (textannotation == null && m_annotation == null)
			return;
		if (!getActiveSelf())
			return;
		if (checkFade) {
			if (fadeOut) {
				if (!isFadingOut) { // not sure why TurnOffAnnotationFor gets called every frame?
					float curTime = Time.time + Time.smoothDeltaTime;
					startTimeFadeOut = curTime;
					isFadingOut = true;
					isFadingIn = false;
				}
			} else {
				SetAnnotationsActive (false, false);
				DPManagerScript.Call_i ("Turn-Off-Placements-For-map", GetVMInstanceID ());
			}
		} else {
			SetAnnotationsActive (false, false);
			SetAnnotationAlpha (1.0f);
		}
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
