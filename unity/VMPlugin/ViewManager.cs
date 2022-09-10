#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Text;
using System.Net;
using System.IO;

[DisallowMultipleComponent]
public class ViewManager : MonoBehaviour
{
    public GameObject screenCanvas;
	public Camera currentCamera;

    private List<int> allDimInfo = new List<int>(); // all current dimensions in UnityTraverseCoordinateSystem (order,coord-id)
    public List<int> getAllDimInfo() { return allDimInfo; }
    public void setAllDimInfo(List<int> aD) { allDimInfo = aD; }
	public GameObject topWorldObject;
    public static bool isHololens = false;

    public static Matrix4x4 getWorldToTopMatrix(){
		ViewManager vm = FindObjectOfType<ViewManager> ();
		if (vm.topWorldObject != null)
			return vm.topWorldObject.transform.worldToLocalMatrix;
		return Matrix4x4.identity;
	}
	public static Matrix4x4 getTopToWorldMatrix(){
		ViewManager vm = FindObjectOfType<ViewManager> ();
		if (vm.topWorldObject != null)
			return vm.topWorldObject.transform.localToWorldMatrix;
		return Matrix4x4.identity;
	}
    public static Camera getCurrentCamera(){
		ViewManager vm = FindObjectOfType<ViewManager> ();
        if (vm != null) {
            if (vm.currentCamera != null)
                return vm.currentCamera;
        }
	    if (Camera.main != null)
			return Camera.main;
		if (Camera.current != null)
			return Camera.current;
        if (Camera.allCamerasCount > 0)
        {
            return Camera.allCameras[0];
        }
		return null;
	}
    // License Stuff
    public enum LicenseStatus
    {
        Unchecked = 0,
        Verified,
        Expired,
        Failed,
        NonExistent,
        Corrupt
    }
    static public Dictionary<LicenseStatus, string> licenseStatusStrings = new Dictionary<LicenseStatus, string>(){
        { LicenseStatus.Unchecked, "Unchecked" }, { LicenseStatus.Verified, "Verified" },
        { LicenseStatus.Expired, "Expired" }, { LicenseStatus.Failed, "Failed" },
        { LicenseStatus.NonExistent, "No License" }, { LicenseStatus.Corrupt, "Corrupt" }
    };
    // Licensing Stuff
    public bool m_checkLicenseOnStart = false;
    public bool checkLicenseOnStart
    {
        set
        {
            if (value != m_checkLicenseOnStart)
            {
                m_checkLicenseOnStart = value;
                changed();
            }
        }
        get { return m_checkLicenseOnStart; }
    }

    public string emailString = "";
    private string passwordString = "";
	public string passwordGet(){
		return passwordString;
	}
	public void passwordSet(string pw){
		passwordString = pw;
	}
    public static LicenseStatus licenseStatusGet()
    {
        return (LicenseStatus)DPManagerScript.licenseStatusGet();
    }
    public static void licenseStatusSet(LicenseStatus ls)
    {
        DPManagerScript.licenseStatusSet((int)ls);
#if UNITY_EDITOR
        ViewManagerScript.SetWindowDirty();
#endif
    }

    private static int days_left = -1;
    public static int getDaysLeft()
    {
        if (days_left < 0)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string expiration_date = DPManagerScript.GetExpirationDate();
            DateTime d1 = DateTime.ParseExact(expiration_date, "M-d-yyyy", provider);
            DateTime d2 = DateTime.Now;
            days_left = (d1.Date - d2.Date).Days + 1;
        }
        return days_left;
    }
    public static string error_message = "";
    public LicenseStatus CheckLicense()
    {
        error_message = "";
        //bool prevNotVerified = (licenseStatusGet() != LicenseStatus.Verified);
        int licenseStatusState = DPManagerScript.CheckLicenseStatus();
        licenseStatusSet((LicenseStatus)licenseStatusState);
        if (licenseStatusGet() == LicenseStatus.Verified)
        {
            emailString = DPManagerScript.GetEmail();
            getDaysLeft(); // just set days_left
            if (!DPManagerScript.DPisInitialized && Application.isPlaying)
                DPManagerScript.Init();
        }
        else if (licenseStatusGet() == LicenseStatus.Expired)
        {
            error_message = "Your license has expired.\n  Please contact blaine@everyware3d.com\n  to update your account.\n";
        }
        return (LicenseStatus)licenseStatusState;
    }
    static public bool isValidEmail(string email)
    {
        int idx = email.IndexOf("@");
        if (idx < 0)
            return false;
        if (email.LastIndexOf("@") != idx)
            return false;
        string suf = email.Substring(idx);
        if (suf.Length == 0)
            return false;
        int dotidx = suf.IndexOf(".");
        if (dotidx < 0)
            return false;
        string suf2 = suf.Substring(dotidx);
        if (suf2.Length <= 2) // including .
            return false;
        return true;
    }
#if !UNITY_WSA
    static public string sendRequest(string url, string method, Dictionary<string, string> postData)
    {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, string> pair in postData)
        {
            sb.Append(pair.Key + "=" + WWW.EscapeURL(pair.Value) + "&");
        }
        int last = sb.Length;
        sb.Remove(last - 1, 1);
        string postdata = sb.ToString();
        WebRequest rqst = HttpWebRequest.Create(url);
        rqst.Method = method;
        if (!string.IsNullOrEmpty(postdata))
        {
            rqst.ContentType = "application/x-www-form-urlencoded";

            byte[] byteData = UTF8Encoding.UTF8.GetBytes(postdata);
            rqst.ContentLength = byteData.Length;
            using (Stream postStream = rqst.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
                postStream.Close();
            }
        }
        ((HttpWebRequest)rqst).KeepAlive = false;
        WebResponse wr = rqst.GetResponse();
        StreamReader rsps = new StreamReader(wr.GetResponseStream());
        string strRsps = rsps.ReadToEnd();
        return strRsps;
    }
#endif
    public void CheckLicenseAndRequestIfNecessary()
    {
        LicenseStatus licStatus = CheckLicense();
        if (licStatus != LicenseStatus.Verified)
        {
            RequestAndCheckLicense();
        }
    }
    public void RequestAndCheckLicense()
    {
        // need to login and get confirmation
        if (!isValidEmail(emailString) || passwordString.Trim().Length == 0)
        {
            licenseStatusSet(LicenseStatus.Failed);
            error_message = "Please enter in a correct email/password\n  for validating license.";
            return;
        }
#if !UNITY_WSA
        try {
            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData["licuid"] = "unity-vm";
            string publicKey = sendRequest("http://blainebell.org/cgi-bin/getLicensePubKey.cgi", "POST", postData);
            if (emailString == null || emailString.Length == 0 || passwordString == null || passwordString.Length == 0)
            {
                error_message = "Please enter in an email and password\n  for validating license.";
                return;
            }
            string encLicReq = DPManagerScript.EncryptLicenseRequest(publicKey, emailString, passwordString);
            Dictionary<string, string> postData2 = new Dictionary<string, string>();
            postData2["licuid"] = "unity-vm";
            postData2["encryptedMessage"] = encLicReq;
            string resStr = sendRequest("http://blainebell.org/cgi-bin/postReqLicense.cgi", "POST", postData2);
            char[] charSeparators = new char[] { '\n' };
            string[] reqLicenseResult = resStr.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (reqLicenseResult.Length > 0)
            {
                string[] opLine = reqLicenseResult[0].Split('\t');
                switch (opLine[0])
                {
                    case "ERROR":
                        {
                            if (reqLicenseResult.Length > 1)
                            {
                                error_message = "";
                                for (int i = 1; i < reqLicenseResult.Length; i++)
                                {
                                    error_message += reqLicenseResult[i] + "\n";
                                }
                            }
                        }
                        break;
                    case "NEWLICENSE":
                        {
                            string email = opLine[1].Trim();
                            string exp_date = opLine[2].Trim();
                            string licenseStr = reqLicenseResult[1];
                            DPManagerScript.SaveNewLicense("UnityVM", publicKey, licenseStr, email, exp_date);
                            CheckLicense();
                            if (reqLicenseResult.Length > 2)
                            {
                                string outstr = "";
                                for (int i = 2; i < reqLicenseResult.Length; i++)
                                {
                                    outstr += reqLicenseResult[i];
                                }
                                Debug.Log(outstr);
                            }
                        }
                        break;
                    default:
                        {
                            string res = "";
                            for (int i = 0; i < reqLicenseResult.Length; i++)
                            {
                                res += reqLicenseResult[i] + "\n";
                            }
                            Debug.Log("Bad OP:\n" + res);
                        }
                        break;
                }
            }
        } catch (WebException ex)
        {
            Debug.Log(ex.ToString());
            error_message = "License Request could not be completed,\n  please check your internet connection or try again";
        }
#endif
    }
    public void ClearLicense()
    {
        bool ch = false;
        if (emailString.Length > 0)
            ch = true;
        if (passwordString.Length > 0)
            ch = true;
        emailString = "";
        passwordString = "";
        DPManagerScript.ClearLicense();
        DPManagerScript.ClearDP();
        // need to set VMObject.addedToDP to false
        if (ch)
            changed();
    }
    // License Stuff

    public bool m_prefabsControlWithSameTag = true;
    public void prefabsControlWithSameTagChanged(bool value)
    {
        m_prefabsControlWithSameTag = value;
    }
    public bool prefabsControlWithSameTag
    {
        set
        {
            if (value != m_prefabsControlWithSameTag)
            {
                prefabsControlWithSameTagChanged(value);
                changed();
            }
        }
        get { return m_prefabsControlWithSameTag; }
    }

    public bool m_inPixels = true;
    public void inPixelsChanged(bool value)
    {
        m_inPixels = value;
#if UNITY_EDITOR
        VMObject[] vmobjects = Resources.FindObjectsOfTypeAll<VMObject>();
        foreach (VMObject vmo in vmobjects)
        {
            if (vmo.sizeIsDefault)
            {
                vmo.changed();
            }
        }
#endif
    }
    public bool inPixels
    {
        set
        {
            if (value != m_inPixels)
            {
                inPixelsChanged(value);
            }
        }
        get { return m_inPixels; }
    }

    // default annotation sizes (in pixels)
    public int m_defaultAnnotationMinHeight = 24;
    public int m_defaultAnnotationMaxHeight = 40;
    public void defaultAnnotationMinHeightChanged(int value)
    {
        m_defaultAnnotationMinHeight = value;
        VMObject[] vmobjects = Resources.FindObjectsOfTypeAll<VMObject>();
        foreach (VMObject vmo in vmobjects)
        {
            if (vmo.sizeIsDefault)
            {
                vmo.minHeight = m_defaultAnnotationMinHeight;
#if UNITY_EDITOR
                EditorUtility.SetDirty(vmo);
#endif
            }
        }
    }
    public int defaultAnnotationMinHeight
    {
        set
        {
            if (value != m_defaultAnnotationMinHeight)
            {
                defaultAnnotationMinHeightChanged(value);
            }
        }
        get { return m_defaultAnnotationMinHeight; }
    }

    public void defaultAnnotationMaxHeightChanged(int value)
    {
        m_defaultAnnotationMaxHeight = value;
        VMObject[] vmobjects = Resources.FindObjectsOfTypeAll<VMObject>();
        foreach (VMObject vmo in vmobjects)
        {
            if (vmo.sizeIsDefault)
            {
                vmo.maxHeight = m_defaultAnnotationMaxHeight;
#if UNITY_EDITOR
                EditorUtility.SetDirty(vmo);
#endif
            }
        }
    }
    public int defaultAnnotationMaxHeight
    {
        set
        {
            if (value != m_defaultAnnotationMaxHeight)
            {
                defaultAnnotationMaxHeightChanged(value);
            }
        }
        get { return m_defaultAnnotationMaxHeight; }
    }

    public bool m_tryToLabelAll = false;
    public void tryToLabelAllChanged(bool value)
    {
        m_tryToLabelAll = value;
        DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "try-to-label-all", "uid", "main-uid", value);
    }
    public bool tryToLabelAll
    {
        set
        {
            if (value != m_tryToLabelAll)
            {
                tryToLabelAllChanged(value);
            }
        }
        get { return m_tryToLabelAll; }
    }
    public bool m_showEmptySpace = false;
    public void showEmptySpaceChanged(bool value)
    {
        m_showEmptySpace = value;
        DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "show-empty-space", "uid", "main-uid", value);
        if (!value)
        {
            GameObject go = GameObject.Find("screen-empty-space-rectangles");
            if (go != null)
                Destroy(go);
        }
    }
    public bool showEmptySpace
    {
        set
        {
            if (value != m_showEmptySpace)
            {
                showEmptySpaceChanged(value);
            }
        }
        get { return m_showEmptySpace; }
    }
    public bool m_showBSPPartitionPlanes = false;
    public void showBSPPartitionPlanesChanged(bool value)
    {
        m_showBSPPartitionPlanes = value;
        DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "show-bsp-partition-planes", "uid", "main-uid", value);
        if (value)
        {
            DPManagerScript.Call_noargs("Get-BSP-Planes-And-Show-map");
        }
        else
        {
            GameObject go = GameObject.Find("all-plane-objects");
            if (go != null)
                Destroy(go);
        }
    }
    public bool showBSPPartitionPlanes
    {
        set
        {
            if (value != m_showBSPPartitionPlanes)
            {
                showBSPPartitionPlanesChanged(value);
            }
        }
        get { return m_showBSPPartitionPlanes; }
    }
    public bool m_showProjections = false;
    public void showProjectionsChanged(bool value)
    {
        m_showProjections = value;
        DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "show-projections", "uid", "main-uid", value);
        if (!value)
        {
            GameObject go = GameObject.Find("screen-projection-rectangles");
            if (go != null)
                Destroy(go);
        }
    }
    public bool showProjections
    {
        set
        {
            if (value != m_showProjections)
            {
                showProjectionsChanged(value);
            }
        }
        get { return m_showProjections; }
    }

    public bool m_showResults = false;
    public void showResultsChanged(bool value)
    {
        m_showResults = value;
        DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "show-results", "uid", "main-uid", value);
        if (!value)
        {
            GameObject go = GameObject.Find("all-results");
            if (go != null)
                Destroy(go);
        }
    }
    public bool showResults
    {
        set
        {
            if (value != m_showResults)
            {
                showResultsChanged(value);
            }
        }
        get { return m_showResults; }
    }

    public bool m_showPrevAnnotationProjections = false;
    public void showPrevAnnotationProjectionsChanged(bool value)
    {
        m_showPrevAnnotationProjections = value;
        DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "show-previous-annotation-projections", "uid", "main-uid", value);
        if (!value)
        {
            GameObject go = GameObject.Find("screen-previous-projection-rectangles");
            if (go != null)
                Destroy(go);
        }
    }
    public bool showPrevAnnotationProjections
    {
        set
        {
            if (value != m_showPrevAnnotationProjections)
            {
                showPrevAnnotationProjectionsChanged(value);
            }
        }
        get { return m_showPrevAnnotationProjections; }
    }

    public bool m_showCloseToEdgeRectangle = false;
    public void showCloseToEdgeRectangleChanged(bool value)
    {
        m_showCloseToEdgeRectangle = value;
        DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "show-close-to-edge-rectangle", "uid", "main-uid", value);
    }
    public bool showCloseToEdgeRectangle
    {
        set
        {
            if (value != m_showCloseToEdgeRectangle)
            {
                showCloseToEdgeRectangleChanged(value);
            }
        }
        get { return m_showCloseToEdgeRectangle; }
    }

    public bool m_debugLabelOutline = false;
    public void debugLabelOutlineChanged(bool value)
    {
        m_debugLabelOutline = value;
//        DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "show-close-to-edge-rectangle", "uid", "main-uid", value);
    }
    public bool debugLabelOutline
    {
        set
        {
            if (value != m_debugLabelOutline)
            {
                debugLabelOutlineChanged(value);
            }
        }
        get { return m_debugLabelOutline; }
    }



    public bool m_clipLinesForProjections = true;
    public void clipLinesForProjectionsChanged(bool value)
    {
        m_clipLinesForProjections = value;
        DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "clip-lines-of-faces", "uid", "main-uid", value);
    }
    public bool clipLinesForProjections
    {
        set
        {
            if (value != m_clipLinesForProjections)
            {
                clipLinesForProjectionsChanged(value);
                changed();
            }
        }
        get { return m_clipLinesForProjections; }
    }


    public float m_delayBetweenComputationFrames = 0.0f;
    public void delayBetweenComputationFramesChanged(float value)
    {
        m_delayBetweenComputationFrames = value;
    }
    public float delayBetweenComputationFrames
    {
        set
        {
            if (value != m_delayBetweenComputationFrames)
            {
                delayBetweenComputationFramesChanged(value);
                changed();
            }
        }
        get { return m_delayBetweenComputationFrames; }
    }


    public bool m_doNotMoveWhenCloseToEdge = true;
    public void doNotMoveWhenCloseToEdgeChanged(bool value)
    {
        m_doNotMoveWhenCloseToEdge = value;
        DPManagerScript.Call_s_s_s_s_b("SetDataValue-If-Exists-map", "UnityMain", "do-not-move-when-close-to-edge", "uid", "main-uid", value);
    }
    public bool doNotMoveWhenCloseToEdge
    {
        set
        {
            if (value != m_doNotMoveWhenCloseToEdge)
            {
                doNotMoveWhenCloseToEdgeChanged(value);
                changed();
            }
        }
        get { return m_doNotMoveWhenCloseToEdge; }
    }
    int[] m_closestToEdgeRect = new int[4] { 0, 0, 0, 0 };
    public int [] closestToEdgeRect
    {
        set {
            if (value.Length == 4)
            {
                if (m_closestToEdgeRect[0] != value[0] || m_closestToEdgeRect[1] != value[1] || 
                    m_closestToEdgeRect[2] != value[2] || m_closestToEdgeRect[3] != value[3]) {
                    m_closestToEdgeRect = value;
                }
            }
        }
        get { return m_closestToEdgeRect;  }
    }
    public float m_closeToEdgeDistance = .1f;
    public void closeToEdgeDistanceChanged(float value)
    {
        m_closeToEdgeDistance = value;
        int pW = getCurrentCamera().pixelWidth;
        int pH = getCurrentCamera().pixelHeight;

        closestToEdgeRect = new int[] { (int)(m_closeToEdgeDistance * pW),
            (int)(m_closeToEdgeDistance * pH),
            (int)(pW * (1.0f-m_closeToEdgeDistance)),
            (int)(pH * (1.0f-m_closeToEdgeDistance)) };
        DPManagerScript.Call_s_s_s_s_f("SetDataValue-If-Exists-map", "UnityMain", "close-to-edge-distance", "uid", "main-uid", value);
    }
    public float closeToEdgeDistance
    {
        set
        {
            if (value != m_closeToEdgeDistance)
            {
                closeToEdgeDistanceChanged(value);
                changed();
            }
        }
        get { return m_closeToEdgeDistance; }
    }


    public bool m_checkLineOverlap = true;
    public void checkLineOverlapChanged(bool value)
    {
        m_checkLineOverlap = value;
		DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "should-check-line-overlap-on-outside-placement", "uid", "main-uid", value);
		DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "should-populate-all-screen-placements-Manual", "uid", "main-uid", value);
    }
    public bool checkLineOverlap
    {
        set
        {
            if (value != m_checkLineOverlap)
            {
                checkLineOverlapChanged(value);
            }
        }
        get { return m_checkLineOverlap; }
    }


    public bool m_useCurrentVector = false;
    public void useCurrentVectorChanged(bool value)
    {
        m_useCurrentVector = value;
        DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "use-current-vector-for-camera-transitions", "uid", "main-uid", value);
    }
    public bool useCurrentVector
    {
        set
        {
            if (value != m_useCurrentVector)
            {
                useCurrentVectorChanged(value);
            }
        }
        get { return m_useCurrentVector; }
    }

    public bool m_useCurrentVectorForMenu = false;
    public void useCurrentVectorForMenuChanged(bool value)
    {
        m_useCurrentVectorForMenu = value;
    }
    public bool useCurrentVectorForMenu
    {
        set
        {
            if (value != m_useCurrentVectorForMenu)
            {
                useCurrentVectorForMenuChanged(value);
            }
        }
        get { return m_useCurrentVectorForMenu; }
    }

    public bool m_shortenOutsideLines = true;
    public void shortenOutsideLinesChanged(bool value)
    {
        m_shortenOutsideLines = value;
        DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "use-shortening-with-outside-placement", "uid", "main-uid", value);
    }
    public bool shortenOutsideLines
    {
        set
        {
            if (value != m_shortenOutsideLines)
            {
                shortenOutsideLinesChanged(value);
            }
        }
        get { return m_shortenOutsideLines; }
    }
    public int m_screenProjectionMode = 0; // 0 - screen, 1 - Bridge, 2 - Vive
                                           // currently, this is used for 
                                           // rotating labels towards screen or eye
    public void screenProjectionModeChanged(int value)
    {
        m_screenProjectionMode = value;
        changed();
    }
    public int screenProjectionMode
    {
        set
        {
            if (value != m_screenProjectionMode)
            {
                screenProjectionModeChanged(value);
            }
        }
        get { return m_screenProjectionMode; }
    }


	public bool m_shareRendering = false;
	public void shareRenderingChanged(bool value)
	{
		bool change = (m_shareRendering != value);
		m_shareRendering = value;
		//DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "show-projections", "uid", "main-uid", value);
		if (value) {
			DPManagerScript.Call_noargs ("PeerSharesVideoFeed-3D-Graphics-map");
		} else {
			DPManagerScript.Call_noargs ("Remove-PeerSharesVideoFeed-3D-Graphics-map");
		}
		if (change)
			changed ();
	}
	public bool shareRendering
	{
		set
		{
			if (value != m_shareRendering)
			{
				shareRenderingChanged(value);
			}
		}
		get { return m_shareRendering; }
	}


	public bool m_shareScreen = false;
	public void shareScreenChanged(bool value)
	{
		bool change = (m_shareScreen != value);
		m_shareScreen = value;
		if (value) {
			DPManagerScript.Call_noargs ("PeerSharesScreen-3D-Graphics-map");
		} else {
			DPManagerScript.Call_noargs ("Remove-PeerSharesScreen-3D-Graphics-map");
		}
		if (change)
			changed ();
	}
	public bool shareScreen
	{
		set
		{
			if (value != m_shareScreen)
			{
				shareScreenChanged(value);
			}
		}
		get { return m_shareScreen; }
	}





	public bool m_reportRendering = false;
	RenderTexture _reportTexture = null;
	Texture2D _virtualPhoto = null;

    private void OnDestroy()
    {
        if (_webTex != null)
        {
            _webTex.Stop();
            _webTex = null;
        }
        if (screenSpace1 != null)
            Destroy(screenSpace1);
        if (_reportTexture != null)
            _reportTexture = null;
        if (_virtualPhoto != null)
            _virtualPhoto = null;
    }
    // for AR
    WebCamTexture _webTex = null;
    GameObject screenSpace1 = null;
    byte [] lastchecksum = new byte[2], tmpchecksum = new byte[2];
	bool lastchecksum_isset = false;
	public void invalidateReportRendering(){
		lastchecksum_isset = false;
	}
	int twidth = 320, theight = 240;
	public void reportRenderingChanged(bool value)
	{
		if (value) {
			Camera cam = getCurrentCamera ();
			float fwidth = cam.pixelWidth, fheight = cam.pixelHeight;
			while (fwidth > 300.0f && fheight > 300.0f) {
				fwidth /= 1.5f;
				fheight /= 1.5f;
			}
			twidth = (int)Mathf.Round(fwidth);
			theight = (int)Mathf.Round(fheight);
			_reportTexture = new RenderTexture (twidth, theight, 24);
			_virtualPhoto = new Texture2D (twidth, theight, TextureFormat.RGB24, false);
            if (isHololens)
            {
                _webTex = new WebCamTexture();
                _webTex.Play();
                ViewManager vm = GameObject.FindObjectOfType<ViewManager>();
                if (screenSpace1 == null)
                {
                    screenSpace1 = DPManagerScript.createScreenTexture(vm, "screenSpace1", 0.0f);
                }
                screenSpace1.GetComponent<Renderer>().material.mainTexture = _webTex;
                ScreenRelativeScript srs = screenSpace1.GetComponent<ScreenRelativeScript>();
                srs.adjustForImageSize = false;
                srs.screenRelative = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                screenSpace1.layer = DPUtils.addLayerIfNot("OnlyScreenSpace");
                cam.cullingMask &= (int)(0xffffffff ^ (1 << LayerMask.NameToLayer("OnlyScreenSpace")));
            }
        } else {
			_reportTexture = null;
            if (_webTex != null)
            {
                Destroy(screenSpace1);
                screenSpace1 = null;
                _webTex.Stop();
                _webTex = null;
            }
		}
		lastchecksum_isset = false;
		bool change = (m_reportRendering != value);
		m_reportRendering = value;
		if (change)
			changed ();
	}
	public bool reportRendering
	{
		set
		{
			if (value != m_reportRendering)
			{
				reportRenderingChanged(value);
			}
		}
		get { return m_reportRendering; }
	}

    public float m_screenStabilizedAnimationSpeed = 10.0f;
    public void screenStabilizedAnimationSpeedChanged(float value)
    {
        m_screenStabilizedAnimationSpeed = value;
    }
    public float screenStabilizedAnimationSpeed
    {
        set
        {
            if (value != m_screenStabilizedAnimationSpeed)
            {
                screenStabilizedAnimationSpeedChanged(value);
                changed();
            }
        }
        get { return m_screenStabilizedAnimationSpeed; }
    }

    static public bool distortionCorrection = false;
    static public DPActionVector2 distortionCorrectionFunc = null;
    public bool debugTraversal = false;
    public bool saveTraversal = false;
    public bool reportGeometry = false;
    public bool highlightObject = false;
    public string highlightedObject = "";
    public bool highlightPart = false;
    public int highlightPartNumber = -1;
    public bool highlightOrderedPart = false;
    public int highlightPartOrder = -1;

    public Color32 highlightColor = new Color32(255, 0, 0, 64);
    public bool saveHighlightColorToDisk = false;

    public static ViewManager Instance = null;

    ViewManager()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("ERROR: multiple ViewManager instances created");
        }
    }

    void Start()
    {
#if UNITY_2018_1_OR_NEWER
        if (UnityEngine.XR.XRDevice.isPresent)
        {
            isHololens = UnityEngine.XR.XRSettings.loadedDeviceName.ToLower().Equals("hololens");
        }
#endif
        if (screenCanvas == null) {
			screenCanvas = new GameObject ("screenCanvas");
			screenCanvas.AddComponent<Canvas> ();
			Canvas nCanvas = screenCanvas.GetComponent<Canvas> ();
			nCanvas.renderMode = RenderMode.ScreenSpaceCamera;
			nCanvas.planeDistance = 2.0f;
			nCanvas.worldCamera = getCurrentCamera ();
		}
        DPManagerScript dpmanager = FindObjectOfType<DPManagerScript>();
        if (dpmanager == null)
        {
            gameObject.AddComponent<DPManagerScript>();
            DPManagerScript.ReStart();
        }
        else
        {
            Debug.Log("WARNING: Multiple ViewManagers are instantiated, only one should be defined");
        }
#if UNITY_WEBGL && !UNITY_EDITOR
		Debug.Log ("ViewManager.Start() dpmanager=" + dpmanager + " calling Set-Unity-Game-Object-Name-map with name=" + gameObject.name);
		DPManagerScript.Call_s("Set-Unity-Game-Object-Name-map", gameObject.name);
#endif

        CallAllChangedImpl();
    }
    // Update is called once per frame
    static int curFrame = 0;
    static float lastComputed = 0;
    public bool traverseNextFrame = false;

	public bool reportPositionOrientation = false;
	public Matrix4x4 prevViewMatrix = new Matrix4x4 ();
	public bool prevViewMatrixIsSet = false;
	void UpdateImpl()
	{
		if (!DPManagerScript.DPisInitialized)
			return;
		if (Input.GetKeyDown (KeyCode.P)) {
			DPManagerScript.Call_noargs ("Print-All-UnityObjects-map");
		} else if (Input.GetKeyDown (KeyCode.G)) {
			DPManagerScript.Call_noargs ("Construct-BSP-Tree-For-All-Objects-map");
		} else {
			int curFrameCnt = Time.frameCount;
			if (curFrameCnt <= curFrame) {
				// this happens when multiple ViewManagers are instantiated, need to make it a singleton
				curFrame = curFrameCnt;
				DPManagerScript.Call_s ("CallFunctionOnDataManager-map", "callFunctionsToCallFromMain");
				return;
			}
			if (reportPositionOrientation) {
				Camera curCam = ViewManager.getCurrentCamera ();
				if (topWorldObject == null) {
					if (!prevViewMatrixIsSet || !transform.worldToLocalMatrix.Equals (prevViewMatrix)) {
						// need to move, regenerate geometry, delete/add to BSPTree
						prevViewMatrix = transform.worldToLocalMatrix;
						Matrix4x4 camdiff = curCam.transform.worldToLocalMatrix * curCam.worldToCameraMatrix.inverse;
						Matrix4x4 curCamTrans = (camdiff.inverse * curCam.transform.worldToLocalMatrix).inverse;
						Vector3 pos = curCamTrans.GetPosition ();
						Vector3 forward = curCamTrans * Vector3.back; // using back as forward
						forward.y = 0;
						forward.Normalize (); // for now, just normalize for direction
						float[] posa = { pos.x, pos.z };
						float[] forwarda = { forward.x, forward.z };//, forward.y  };
						DPManagerScript.Call_p2_p2 ("Report-Camera-Position-Orientation-map", posa, forwarda);
						prevViewMatrixIsSet = true;
					}
				} else {
					Matrix4x4 tmpMat = topWorldObject.transform.worldToLocalMatrix * curCam.transform.localToWorldMatrix ;
					if (!prevViewMatrixIsSet || !tmpMat.Equals (prevViewMatrix)) {
						prevViewMatrix = tmpMat;
						Vector3 pos = prevViewMatrix.GetPosition ();
						Vector3 forward = prevViewMatrix * Vector3.forward; // using back as forward
						forward.y = 0;
						forward.Normalize (); // for now, just normalize for direction
						//Debug.Log("Report-Camera-Position: pos=" + pos + " forward=" + forward);
						float[] posa = { pos.x, pos.z };
						float[] forwarda = { forward.x, forward.z };
						DPManagerScript.Call_p2_p2 ("Report-Camera-Position-Orientation-map", posa, forwarda);
						prevViewMatrixIsSet = true;
					}
				}
			}
			if (reportGeometry) {
				DPManagerScript.Call_noargs ("Check-And-Report-Back-Geometry-Issues-For-Unity-Editor-map");
				reportGeometry = false;
			}
			curFrame = curFrameCnt;
			if (!traverseNextFrame && m_delayBetweenComputationFrames > 0.0f) {
				float curTime = Time.time;
				if (curTime - lastComputed < m_delayBetweenComputationFrames) {
					DPManagerScript.Call_s ("CallFunctionOnDataManager-map", "callFunctionsToCallFromMain");
					return;
				}
			}
			DPManagerScript.Call_b_b ("Traverse-BSP-Object-Tree-And-Try-To-Place-All-Annotations-map", debugTraversal, saveTraversal);
			debugTraversal = false;
			saveTraversal = false;
			traverseNextFrame = false;
			VMObject[] vmobjects = Resources.FindObjectsOfTypeAll<VMObject> ();
			foreach (VMObject vmo in vmobjects) {
				vmo.UpdateAnnotation ();
			}
			lastComputed = Time.time;
		}
	}
	void Update()
	{
		UpdateImpl ();
		if (reportRendering) {
			// start with 320x240
			float startTime = Time.time;
            byte[] bytes = null;
            Camera cam = getCurrentCamera();

            if (isHololens)
            {
                cam.targetTexture = _reportTexture;

                // Render image from camera into texture
                int prevCulMask = cam.cullingMask;
                cam.cullingMask = (1 << LayerMask.NameToLayer("OnlyScreenSpace"));
                cam.Render();
                cam.cullingMask = prevCulMask & (int)(0xffffffff ^ (1 << LayerMask.NameToLayer("OnlyScreenSpace")));

                // clear depth buffer, and render scene into texture
                CameraClearFlags prevClearFlags = cam.clearFlags;
                cam.clearFlags = CameraClearFlags.Depth;
                cam.Render();
                cam.clearFlags = prevClearFlags;
                cam.targetTexture = null;

                RenderTexture.active = _reportTexture;
                _virtualPhoto.ReadPixels(new Rect(0, 0, twidth, theight), 0, 0);

                bytes = _virtualPhoto.EncodeToJPG();
            }
            else
            {
                cam.targetTexture = _reportTexture;
                cam.Render();
                cam.targetTexture = null;
                RenderTexture.active = _reportTexture;
                _virtualPhoto.ReadPixels(new Rect(0, 0, twidth, theight), 0, 0);
                //RenderTexture.active = null;
                bytes = _virtualPhoto.EncodeToJPG();
            }

            //Debug.Log("ReadPixels: size: " + twidth + "," + theight + " : bytes=" + bytes.Length);
            cam.enabled = true; // changes Camera.main for some reason, need to set enable to reset it

            bool shouldReport = true;
			if (m_screenProjectionMode == 0) {  // screen, not hmd mode
				DPUtils.computeCheckSum (bytes, tmpchecksum);

				if (lastchecksum_isset) {
					if (lastchecksum [0] == tmpchecksum [0] &&
					   lastchecksum [1] == tmpchecksum [1]) {
						shouldReport = false;
					}
				}
			}
			if (shouldReport) {
				//Debug.Log ("EncodeToJPG: Time.frameCount=" + Time.frameCount + " duration=" + (Time.time-startTime) + " bytes.length=" + bytes.Length);
				//byte[] bytes = _virtualPhoto.EncodeToPNG ();
				// TODO: TRY RAW _virtualPhoto.GetRawTextureData
				DPManagerScript.Call_ba_i_p2 ("Report-Rendering-PNG-map", bytes, bytes.Length, Time.frameCount, new float[] {
					twidth,
					theight
				});
				lastchecksum [0] = tmpchecksum [0];
				lastchecksum [1] = tmpchecksum [1];
				lastchecksum_isset = true;
			}
		}
    }
    public void changed()
    {
#if UNITY_EDITOR
        ViewManager vm = FindObjectOfType<ViewManager>();
        if (vm != null)
        {
            EditorUtility.SetDirty(vm);
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
#endif
    }
    public void CallAllChanged()
    {
        DPManagerScript.execute(new DPActionFunc
        {
            actionPerformedFunc = () => CallAllChangedImpl()
        });
    }
    void CallAllChangedImpl()
    {
        // TODO: CALL ALL
        prefabsControlWithSameTagChanged(m_prefabsControlWithSameTag);
        inPixelsChanged(m_inPixels);
        defaultAnnotationMinHeightChanged(m_defaultAnnotationMinHeight);
        defaultAnnotationMaxHeightChanged(m_defaultAnnotationMaxHeight);
        tryToLabelAllChanged(m_tryToLabelAll);
        showEmptySpaceChanged(m_showEmptySpace);
        showBSPPartitionPlanesChanged(m_showBSPPartitionPlanes);
        showProjectionsChanged(m_showProjections);
        showResultsChanged(m_showResults);
        showPrevAnnotationProjectionsChanged(m_showPrevAnnotationProjections);
        showCloseToEdgeRectangleChanged(m_showCloseToEdgeRectangle);
        clipLinesForProjectionsChanged(m_clipLinesForProjections);
        delayBetweenComputationFramesChanged(m_delayBetweenComputationFrames);
        doNotMoveWhenCloseToEdgeChanged(m_doNotMoveWhenCloseToEdge);
        closeToEdgeDistanceChanged(m_closeToEdgeDistance);
        checkLineOverlapChanged(m_checkLineOverlap);
        useCurrentVectorChanged(m_useCurrentVector);
        useCurrentVectorForMenuChanged(m_useCurrentVectorForMenu);
        shortenOutsideLinesChanged(m_shortenOutsideLines);
		shareRenderingChanged (m_shareRendering);
		shareScreenChanged (m_shareScreen);
    }
	public static void invalidateView(){
#if UNITY_EDITOR
		ViewManagerScript.invalidate();
#endif
	}
	public static void setViewToCurrentCamera(string setcamfunc, string setviewfunc, bool recomputeAll, 
                                              Matrix4x4 projectionMatrix, Matrix4x4 camdiff, Matrix4x4 worldToCamera,
                                              int pixelWidth, int pixelHeight,
                                              float nearClipPlane, float fieldOfView)
    {
        // projectionMatrix - curCamera.projectionMatrix
        // camdiff - curCamera.transform.worldToLocalMatrix * curCamera.worldToCameraMatrix.inverse
        // worldToCamera - curCamera.transform.worldToLocalMatrix

        // curCamera.worldToCameraMatrix = camdiff * worldToCamera

        Matrix4x4 curCamTrans = (camdiff.inverse * worldToCamera).inverse;
		if (setcamfunc!=""){
            Vector3 up = curCamTrans * Vector3.up;
            Vector3 right = curCamTrans * Vector3.right;
            float[] upv = { up.x, up.y, up.z };
            float[] rightv = { right.x, right.y, right.z };
			DPManagerScript.Call_p3_p3(setcamfunc, upv, rightv);
        }
		if (setviewfunc != "") {
			Vector3 pos = curCamTrans.GetPosition ();
			Vector3 forward = curCamTrans * Vector3.back; // using back as forward
			float[] posa = { pos.x, pos.y, pos.z };
			float[] forwarda = { forward.x, forward.y, forward.z };

			float[] viewSize = { pixelWidth, pixelHeight };
			Vector3 cameul = curCamTrans.GetRotation ().eulerAngles;
			Vector3 camscale = curCamTrans.GetScale ();
			Matrix4x4 w2c = camdiff * worldToCamera;
			float[] w2cArr = {  pos.x, pos.y, pos.z, 0.0f,
				cameul.x, cameul.y, cameul.z, 0.0f,
				camscale.x, camscale.y, camscale.z, 0.0f,
				0.0f, 0.0f, 0.0f, 0.0f
			};
			Matrix4x4 worldToCam = projectionMatrix * w2c;
			float[] worldToCamArr = { worldToCam.m00, worldToCam.m01, worldToCam.m02, worldToCam.m03,
				worldToCam.m10, worldToCam.m11, worldToCam.m12, worldToCam.m13,
				worldToCam.m20, worldToCam.m21, worldToCam.m22, worldToCam.m23,
				worldToCam.m30, worldToCam.m31, worldToCam.m32, worldToCam.m33
			};
			Matrix4x4 camToWorld = worldToCam.inverse;
			float[] camToWorldArr = { camToWorld.m00, camToWorld.m01, camToWorld.m02, camToWorld.m03,
				camToWorld.m10, camToWorld.m11, camToWorld.m12, camToWorld.m13,
				camToWorld.m20, camToWorld.m21, camToWorld.m22, camToWorld.m23,
				camToWorld.m30, camToWorld.m31, camToWorld.m32, camToWorld.m33
			};
			if (setviewfunc != "")
				DPManagerScript.Call_p3_p3_t3f_t3f_t3f_p2_f_f (setviewfunc, posa, forwarda, w2cArr, worldToCamArr, camToWorldArr,
					viewSize, nearClipPlane, fieldOfView);
		}
		if (recomputeAll)
		{
			// recompute all
			VMObject[] vmos = FindObjectsOfType<VMObject>();
			foreach (VMObject vmo in vmos)
			{
				vmo.computePlacement();
			}
		}
    }
}
