using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif


public static class DPUtils
{
    public static float TOLER = 0.00000763f;
    public static bool equals(float f1, float f2)
    {
        return Mathf.Abs(f1 - f2) < TOLER;
    }
    public static bool equals(Vector3 v1, Vector3 v2)
    {
        return Mathf.Abs(v1.x - v2.x) < TOLER &&
               Mathf.Abs(v1.y - v2.y) < TOLER &&
               Mathf.Abs(v1.z - v2.z) < TOLER;
    }
    public static bool equals(Quaternion q1, Quaternion q2)
    {
        return Mathf.Abs(q1.x - q2.x) < TOLER &&
            Mathf.Abs(q1.y - q2.y) < TOLER &&
            Mathf.Abs(q1.z - q2.z) < TOLER &&
            Mathf.Abs(q1.w - q2.w) < TOLER;
    }
    public static bool equals(Matrix4x4 m1, Matrix4x4 m2)
    {
        return equals(m1, m2, TOLER);
    }
    public static bool equals(Matrix4x4 m1, Matrix4x4 m2, float thresh)
    {
        return Mathf.Abs(m1.m00 - m2.m00) < thresh &&
                Mathf.Abs(m1.m01 - m2.m01) < thresh &&
                Mathf.Abs(m1.m02 - m2.m02) < thresh &&
                Mathf.Abs(m1.m03 - m2.m03) < thresh &&
                Mathf.Abs(m1.m10 - m2.m10) < thresh &&
                Mathf.Abs(m1.m11 - m2.m11) < thresh &&
                Mathf.Abs(m1.m12 - m2.m12) < thresh &&
                Mathf.Abs(m1.m13 - m2.m13) < thresh &&
                Mathf.Abs(m1.m20 - m2.m20) < thresh &&
                Mathf.Abs(m1.m21 - m2.m21) < thresh &&
                Mathf.Abs(m1.m22 - m2.m22) < thresh &&
                Mathf.Abs(m1.m23 - m2.m23) < thresh &&
                Mathf.Abs(m1.m30 - m2.m30) < thresh &&
                Mathf.Abs(m1.m31 - m2.m31) < thresh &&
                Mathf.Abs(m1.m32 - m2.m32) < thresh &&
                Mathf.Abs(m1.m33 - m2.m33) < thresh;
    }
    public static bool isPrefab(GameObject go)
    {
#if UNITY_EDITOR
        return (PrefabUtility.GetPrefabParent(go) == null && PrefabUtility.GetPrefabObject(go) != null);
#else
		return false;
#endif
    }
    public static float getScaleFromZDistance(float distance)
    {
        Camera curCamera = ViewManager.getCurrentCamera();
        if (curCamera.orthographic)
        {
            return 1.0f;
        }
        else
        {
            float frustumHeight = 2.0f * distance * Mathf.Tan(curCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            return frustumHeight / ((float)curCamera.pixelHeight);
        }
    }

    public static void setAllToStillLayoutAndPlace(MonoBehaviour mb)
    {
        //		VMObject [] vmos = mb.FindObj
    }

    public static void SetBoxCollidersWithTagEnabled(string tag, bool en)
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject go in gos)
        {
            BoxCollider bc = go.GetComponent<BoxCollider>();
            bc.enabled = en;
        }
    }
    public static void AddShowTagOnClick(string showTagsOnClick)
    {
        DPUtils.SetBoxCollidersWithTagEnabled(showTagsOnClick, true);
        GameObject[] gos = GameObject.FindGameObjectsWithTag(showTagsOnClick);
        foreach (GameObject go in gos)
        {
            VMObject vmo = go.GetComponent<VMObject>();
            if (vmo != null)
            {
                vmo.computePlacement();
            }
        }
    }
    public static void AddShowTagOnClickExcept(string showTagsOnClickExcept, int exceptID)
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(showTagsOnClickExcept);
        foreach (GameObject go in gos)
        {
            VMObject vmo2 = go.GetComponent<VMObject>();
            if (vmo2 != null && vmo2.GetVMInstanceID() != exceptID)
            {
                BoxCollider gobc = go.GetComponent<BoxCollider>();
                gobc.enabled = true;
                vmo2.computePlacement();
            }
            else
            {
                BoxCollider gobc = go.GetComponent<BoxCollider>();
                gobc.enabled = false;
            }
        }
    }
    public static void AddShowTagOnClicks(string showTagsOnClick, string showTagsOnClickExcept, int exceptID)
    {
        if (showTagsOnClick != null && showTagsOnClick.Length > 0)
        {
            DPUtils.AddShowTagOnClick(showTagsOnClick);
        }
        if (showTagsOnClickExcept != null && showTagsOnClickExcept.Length > 0 && exceptID >= 0)
        {
            DPUtils.AddShowTagOnClickExcept(showTagsOnClickExcept, exceptID);
        }
    }
    public static Vector3 GetVector3If(Vector3 vect1, Vector3 vect2)
    {
        if (vect1.magnitude > 0.0f)
        {
            return vect1.normalized;
        }
        else
        {
            return ViewManager.getCurrentCamera().transform.forward;
        }
    }

	public static void ComputePlacementForAll()
	{
		VMObject[] vmos = UnityEngine.Object.FindObjectsOfType<VMObject>();
		foreach (VMObject vmo in vmos)
		{
			vmo.computePlacement();
		}
	}
    public static void TurnAllOff(bool checkFade)
    {
        VMObject[] vmos = UnityEngine.Object.FindObjectsOfType<VMObject>();
        foreach (VMObject vmo in vmos)
        {
            vmo.turnOff(checkFade);
        }
    }
	public static void TurnAllToStillLayout()
    {
        VMObject[] vmos = UnityEngine.Object.FindObjectsOfType<VMObject>();
        foreach (VMObject vmo in vmos)
        {
            vmo.setToStillLayout();
        }
    }

    public static void TurnAllToStillLayoutAndOff(bool checkFade)
    {
        VMObject[] vmos = UnityEngine.Object.FindObjectsOfType<VMObject>();
        foreach (VMObject vmo in vmos)
        {
            vmo.setToStillLayout();
            vmo.turnOff(checkFade);
        }
    }
    public static void TurnAllToContinuousLayout()
    {
        VMObject[] vmos = UnityEngine.Object.FindObjectsOfType<VMObject>();
        foreach (VMObject vmo in vmos)
        {
            vmo.setToContinuousImpl();
        }
    }
    public static void setToFlyoverLayoutForAll()
    {
        VMObject[] vmos = UnityEngine.Object.FindObjectsOfType<VMObject>();
        foreach (VMObject vmo in vmos)
        {
            vmo.setToFlyoverLayoutImpl();
        }
    }
    public static void setToStillLayoutForAll()
    {
        VMObject[] vmos = UnityEngine.Object.FindObjectsOfType<VMObject>();
        foreach (VMObject vmo in vmos)
        {
            vmo.setToStillLayoutImpl();
        }
    }



    public static void TurnToStillLayoutAndOff(bool checkFade, VMDimension dim)
    {
        foreach (VMObject vmo in dim.children)
        {
            vmo.setToStillLayout();
            vmo.turnOff(checkFade);
        }
    }
    public static void TurnToContinuousLayout(VMDimension dim)
    {
        foreach (VMObject vmo in dim.children)
        {
            vmo.setToContinuousImpl();
        }
    }
    public static void setToFlyoverLayoutFor(VMDimension dim)
    {
        foreach (VMObject vmo in dim.children)
        {
            vmo.setToFlyoverLayoutImpl();
        }
    }
    public static void setToStillLayoutFor(VMDimension dim)
    {
        foreach (VMObject vmo in dim.children)
        {
            vmo.setToStillLayoutImpl();
        }
    }



    static public GUIContent[] ContentVals = new GUIContent[] { new GUIContent("Same"), new GUIContent("Top"), new GUIContent("Angle") };
    static public GUIContent[] TraverseVals = new GUIContent[] { new GUIContent("Only Current"), new GUIContent("One Above"), new GUIContent("All") };

    static public Vector3 GetVectorAtAngleFrom(Vector3 fromVect, float angle)
    {
        Vector3 vect = fromVect;
        vect.y = 0.0f;
        if (vect.magnitude <= 0.00001)
        {
            vect.x = 0.0f;
            vect.y = 0.0f;
            vect.z = 1.0f;
        }
        else
        {
            vect.Normalize();
        }
        Camera curCamera = ViewManager.getCurrentCamera();
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero,
            Quaternion.AngleAxis(angle, curCamera.transform.right),
            Vector3.one);
        vect = matrix.MultiplyPoint(vect);
        return vect;
    }
    static public Vector3 GetVectorFromContentIndex(int toVal)
    {
        Vector3 vect;
        Camera curCamera = ViewManager.getCurrentCamera();
        if (toVal == 1)
        { // Top
            vect = new Vector3(0.0f, -1.0f, 0.0f);
        }
        else if (toVal == 0)
        { // Same
            vect = curCamera.transform.forward;
        }
        else
        { // Angle
          // angle
            vect = GetVectorAtAngleFrom(curCamera.transform.forward, 40.0f);
        }
        return vect;
    }
    public static Bounds GetBoundsInGameObject(GameObject go)
    {
        Bounds bounds = new Bounds();
        bool boundsIsSet = false;
        Renderer mr = go.GetComponent<Renderer>();
        if (mr != null)
        {
            bounds = mr.bounds;
        }
        Renderer[] mrs = go.GetComponentsInChildren<Renderer>();
        foreach (Renderer mr2 in mrs)
        {
            Bounds b = mr2.bounds;
            if (mr2.bounds.size.magnitude > 0)
            {
                if (boundsIsSet)
                {
                    bounds.Encapsulate(mr2.bounds);
                }
                else
                {
                    bounds = mr2.bounds;
                    boundsIsSet = true;
                }
            }
        }
        return bounds;
    }
    static public Rect GetRectTransformScreenBounds(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        Vector2[] screencorners = new Vector2[4];
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue),
        max = new Vector2(float.MinValue, float.MinValue);
        bool cameraIsOverlay = false;
        for (int i = 0; i < 4; i++)
        {
            if (cameraIsOverlay)
            {
                min.x = Mathf.Min(corners[i].x, min.x);
                min.y = Mathf.Min(corners[i].y, min.y);
                max.x = Mathf.Max(corners[i].x, max.x);
                max.y = Mathf.Max(corners[i].y, max.y);
            }
            else
            {
                Camera curCamera = ViewManager.getCurrentCamera();
                screencorners[i] = RectTransformUtility.WorldToScreenPoint(curCamera, corners[i]);
                min.x = Mathf.Min(screencorners[i].x, min.x);
                min.y = Mathf.Min(screencorners[i].y, min.y);
                max.x = Mathf.Max(screencorners[i].x, max.x);
                max.y = Mathf.Max(screencorners[i].y, max.y);
            }
        }
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }
    static public void setFadeTimeForAll(float fadeTime)
    {
        VMObject[] vmos = UnityEngine.Object.FindObjectsOfType<VMObject>();
        foreach (VMObject vmo in vmos)
        {
            if (!DPUtils.isPrefab(vmo.gameObject))
                vmo.fadeTime = fadeTime;
        }
    }
    static public void setFadeTimeForAllWithReset(float fadeTime, float resetFadeTime)
    {
        VMObject[] vmos = UnityEngine.Object.FindObjectsOfType<VMObject>();
        foreach (VMObject vmo in vmos)
        {
            if (!DPUtils.isPrefab(vmo.gameObject))
            {
                vmo.fadeTime = fadeTime;
                vmo.resetFadeTime = resetFadeTime;
                vmo.shouldResetFadeTime = true;
            }
        }
    }

    static public Vector3 ClipLineToUnitBox(Vector3 tmpProj, Vector3 tmpProj2, Vector3 boxSize)
    {
        if (tmpProj.x < -boxSize.x)
        {
            tmpProj.y = tmpProj.y + (tmpProj2.y - tmpProj.y) * (-boxSize.x - tmpProj.x) / (tmpProj2.x - tmpProj.x); //y = y0 + (y1 - y0) * (xmin - x0) / (x1 - x0);
            tmpProj.z = tmpProj.z + (tmpProj2.z - tmpProj.z) * (-boxSize.x - tmpProj.x) / (tmpProj2.x - tmpProj.x);
            tmpProj.x = -boxSize.x;                  //x = xmin;
        }
        else if (tmpProj.x > boxSize.x)
        {
            tmpProj.y = tmpProj.y + (tmpProj2.y - tmpProj.y) * (boxSize.x - tmpProj.x) / (tmpProj2.x - tmpProj.x);  //y = y0 + (y1 - y0) * (xmax - x0) / (x1 - x0);
            tmpProj.z = tmpProj.z + (tmpProj2.z - tmpProj.z) * (boxSize.x - tmpProj.x) / (tmpProj2.x - tmpProj.x);
            tmpProj.x = boxSize.x; //x = xmax;
        }
        if (tmpProj.y < -boxSize.y)
        {
            tmpProj.x = tmpProj.x + (tmpProj2.x - tmpProj.x) * (-boxSize.y - tmpProj.y) / (tmpProj2.y - tmpProj.y);  //x = x0 + (x1 - x0) * (ymin - y0) / (y1 - y0);
            tmpProj.z = tmpProj.z + (tmpProj2.z - tmpProj.z) * (-boxSize.y - tmpProj.y) / (tmpProj2.y - tmpProj.y);
            tmpProj.y = -boxSize.y; //y = ymin;
        }
        else if (tmpProj.y > boxSize.y)
        {
            tmpProj.x = tmpProj.x + (tmpProj2.x - tmpProj.x) * (boxSize.y - tmpProj.y) / (tmpProj2.y - tmpProj.y);  //x = x0 + (x1 - x0) * (ymax - y0) / (y1 - y0);
            tmpProj.z = tmpProj.z + (tmpProj2.z - tmpProj.z) * (boxSize.y - tmpProj.y) / (tmpProj2.y - tmpProj.y);
            tmpProj.y = boxSize.y; //y = ymax;
        }
        if (tmpProj.z < -boxSize.z)
        {
            tmpProj.x = tmpProj.x + (tmpProj2.x - tmpProj.x) * (-boxSize.z - tmpProj.z) / (tmpProj2.z - tmpProj.z);  //x = x0 + (x1 - x0) * (ymin - y0) / (y1 - y0);
            tmpProj.y = tmpProj.y + (tmpProj2.y - tmpProj.y) * (-boxSize.z - tmpProj.z) / (tmpProj2.z - tmpProj.z);
            tmpProj.z = -boxSize.z; //y = ymin;
        }
        else if (tmpProj.z > boxSize.z)
        {
            tmpProj.x = tmpProj.x + (tmpProj2.x - tmpProj.x) * (boxSize.z - tmpProj.z) / (tmpProj2.z - tmpProj.z);  //x = x0 + (x1 - x0) * (ymax - y0) / (y1 - y0);
            tmpProj.y = tmpProj.y + (tmpProj2.y - tmpProj.y) * (boxSize.z - tmpProj.z) / (tmpProj2.z - tmpProj.z);
            tmpProj.z = boxSize.z; //y = ymax;
        }
        return tmpProj;
    }

    public static bool GotoViewWithViewVector(Bounds bounds, AnimateCameraImpl acs, Vector3 viewVector, bool insideBounds)
    {
        Vector3 pos;
        if (insideBounds)
        {
            Vector3 vvnorm = -viewVector.normalized;
            Vector3 bext = bounds.extents;
            float maxdim = bext.x;
            if (maxdim < bext.y)
                maxdim = bext.y;
            if (maxdim < bext.z)
                maxdim = bext.z;
            Vector3 clipped = ClipLineToUnitBox(vvnorm * maxdim * 3.0f, Vector3.zero, bounds.size / 2.0f);
            pos = clipped + bounds.center;
            Vector3 nclipped = clipped.normalized;
        }
        else
        {
            Camera curCamera = ViewManager.getCurrentCamera();
            float fov = (float)(curCamera.fieldOfView * Mathf.Deg2Rad);
            float distx = (float)((bounds.size.x / 2.0) / Math.Tan(fov / 2.0));
            float distz = (float)((bounds.size.z / 2.0) / Math.Tan(fov / 2.0));
            float dist = (float)(Math.Max(distx, distz) + bounds.size.y / 2.0);
            pos = bounds.center - (viewVector * dist);
        }
        //		Debug.Log ("bounds=" + bounds + " dist: x: " + distx + " z: " + distz + " dist=" + dist);
        bool moves = true;
        if (acs != null)
        {
            moves = acs.animateCameraTo(pos, true, bounds.center, false);
        }
        else
        {
            Camera curCamera = ViewManager.getCurrentCamera();
            curCamera.transform.position = pos;
            curCamera.transform.LookAt(bounds.center, Vector3.up);
        }
        return moves;
    }
    public static bool GotoViewOfObjectWithViewVector(GameObject go, AnimateCameraImpl acs, Vector3 viewVector)
    {
        if (go != null)
        {
            Renderer rend = go.GetComponent<Renderer>();
            Bounds bounds = rend.bounds;
            return GotoViewWithViewVector(bounds, acs, viewVector, false);
        }
        else
        {
            Debug.Log("GotoViewOfObjectWithViewVector: go is null");
        }
        return false;
    }
    static public void addTagIfNot(string tagname)
    {
#if UNITY_EDITOR
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tagname)) { found = true; break; }
        }
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = tagname;
            tagManager.ApplyModifiedProperties();
        }
#endif
    }
    static public int addLayerIfNot(string layername)
    {
        int layerNum = -1;
#if UNITY_EDITOR
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        bool found = false;
        for (int i = 0; i < layersProp.arraySize; i++)
        {
            SerializedProperty t = layersProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(layername)) { layerNum = LayerMask.NameToLayer(layername); found = true; break; }
        }
        if (!found)
        {
            for (int i = 8; i < layersProp.arraySize; i++)
            {
                SerializedProperty t = layersProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(""))
                {
                    layersProp.InsertArrayElementAtIndex(i);
                    SerializedProperty n = layersProp.GetArrayElementAtIndex(i);
                    n.stringValue = layername;
                    layerNum = i;
                    tagManager.ApplyModifiedProperties();
                    break;
                }
            }
        }
#else
        layerNum = LayerMask.NameToLayer(layername);
#endif
        return layerNum;
    }
    public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
        return q;
    }
    public static Color BlendColors(Color c1, Color c2)
    {
        float am1 = 1.0f - c1.a;
        return new Color(c1.r * c1.a + c2.r * am1, c1.g * c1.a + c2.g * am1, c1.b * c1.a + c2.b * am1, c1.a * c1.a + c2.a * am1);
    }

    public static Color AlphaBlendColors(Color c1, Color c2)
    {
        return new Color(Mathf.Clamp01(c1.r * c1.a + c2.r * c2.a),
                          Mathf.Clamp01(c1.g * c1.a + c2.g * c2.a),
                          Mathf.Clamp01(c1.b * c1.a + c2.b * c2.a),
                          Mathf.Clamp01(c1.a * c1.a + c2.a * c2.a));
    }
    public static Texture2D AlphaBlend(Texture2D aBottom, Texture2D aTop)
    {
        if (aBottom.width != aTop.width || aBottom.height != aTop.height)
            throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");
        var bData = aBottom.GetPixels();
        var tData = aTop.GetPixels();
        int count = bData.Length;
        var rData = new Color[count];

        var res = new Texture2D(aTop.width, aTop.height);
        for (int i = 0; i < count; i++)
        {
            rData[i] = DPUtils.AlphaBlendColors(bData[i], tData[i]);
        }
        res.SetPixels(rData);
        res.Apply();
        return res;
    }
    public static Texture2D ImageDiff(Texture2D aBottom, Texture2D aTop)
    {
        if (aBottom.width != aTop.width || aBottom.height != aTop.height)
            throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");
        var bData = aBottom.GetPixels();
        var tData = aTop.GetPixels();
        int count = bData.Length;
        var rData = new Color[count];
        Color32 almostclear = new Color32(0, 0, 0, 1);  // TODO : this is work around of a bug in Unity PNG Encoding/Decoding
        var res = new Texture2D(aTop.width, aTop.height);
        for (int i = 0; i < count; i++)
        {
            Color c1 = bData[i], c2 = tData[i];
            if (c1.Equals(c2))
            {
                rData[i] = almostclear;
            }
            else
            {
                rData[i] = c2;
            }
        }
        res.SetPixels(rData);
        res.Apply();
        return res;
    }
    public static Color AddAlphaBlendColors(Color c1, Color c2)
    {
        float am1 = 1.0f - c1.a;
        return new Color(Mathf.Clamp01(c1.r * c1.a + c2.r * c2.a * am1),
        Mathf.Clamp01(c1.g * c1.a + c2.g * c2.a * am1),
        Mathf.Clamp01(c1.b * c1.a + c2.b * c2.a * am1),
        Mathf.Clamp01(c1.a + c2.a * am1));
    }
    public static Texture2D ImageAdd(Texture2D aBottom, Texture2D aTop)
    {
        if (aBottom.width != aTop.width || aBottom.height != aTop.height)
            throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");
        var bData = aBottom.GetPixels();
        var tData = aTop.GetPixels();
        int count = bData.Length;
        var rData = new Color[count];

        var res = new Texture2D(aTop.width, aTop.height);
        for (int i = 0; i < count; i++)
        {
            //Color c1 = bData [i], c2 = tData [i];
            rData[i] = DPUtils.AddAlphaBlendColors(bData[i], tData[i]);
        }
        res.SetPixels(rData);
        res.Apply();
        return res;
    }
    public static Texture2D ConvertBlackToTransparency(Texture2D tex)
    {
        var res = new Texture2D(tex.width, tex.height);
        var bData = tex.GetPixels();
        int count = bData.Length;
        var rData = new Color[count];
        for (int i = 0; i < count; i++)
        {
            Color c1 = bData[i];
            float gs = c1.grayscale;
            rData[i] = new Color(1.0f, 1.0f, 1.0f, gs);
        }
        res.SetPixels(rData);
        res.Apply();
        return res;
    }
    public static Texture2D MakeTex(int width, int height, Color col)
    {
        var pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
    public static string Marshal_PtrToStringAnsi(IntPtr iptr)
    {
        if (iptr.Equals(IntPtr.Zero))
            return "";
        return Marshal.PtrToStringAnsi(iptr);
    }

    public static bool ClipLineToBox2d(ref Vector2 tmpProj, Vector2 tmpProj2)
    {
        if (tmpProj.x < 0.0f)
        {
            tmpProj.y = tmpProj.y + (tmpProj2.y - tmpProj.y) * (0.0f - tmpProj.x) / (tmpProj2.x - tmpProj.x); //y = y0 + (y1 - y0) * (xmin - x0) / (x1 - x0);
            tmpProj.x = 0.0f;                  //x = xmin;
        }
        else if (tmpProj.x > 1.0f)
        {
            tmpProj.y = tmpProj.y + (tmpProj2.y - tmpProj.y) * (1.0f - tmpProj.x) / (tmpProj2.x - tmpProj.x);  //y = y0 + (y1 - y0) * (xmax - x0) / (x1 - x0);
            tmpProj.x = 1.0f; //x = xmax;
        }
        if (tmpProj.y < 0.0f)
        {
            tmpProj.x = tmpProj.x + (tmpProj2.x - tmpProj.x) * (0.0f - tmpProj.y) / (tmpProj2.y - tmpProj.y);  //x = x0 + (x1 - x0) * (ymin - y0) / (y1 - y0);
            tmpProj.y = 0.0f; //y = ymin;
        }
        else if (tmpProj.y > 1.0f)
        {
            tmpProj.x = tmpProj.x + (tmpProj2.x - tmpProj.x) * (1.0f - tmpProj.y) / (tmpProj2.y - tmpProj.y);  //x = x0 + (x1 - x0) * (ymax - y0) / (y1 - y0);
            tmpProj.y = 1.0f; //y = ymax;
        }
        return tmpProj.y >= 0.0f && tmpProj.y <= 1.0f && tmpProj.x >= 0.0f && tmpProj.x <= 1.0f;
    }
    public static void relativeTo(float[] src1rect, Vector2 srcpt, out Vector2 dest)
    {
        dest.x = (srcpt.x - src1rect[0]) / (src1rect[2] - src1rect[0]);
        dest.y = (srcpt.y - src1rect[1]) / (src1rect[3] - src1rect[1]);
    }
    public static void relativeFrom(float[] src1rect, Vector2 srcpt, out Vector2 dest)
    {
        dest.x = (srcpt.x * (src1rect[2] - src1rect[0])) + src1rect[0];
        dest.y = (srcpt.y * (src1rect[3] - src1rect[1])) + src1rect[1];
    }
    public static Vector2 RotateVector2(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    public static String floatArrayToString(float[] farr)
    {
        string[] foo = farr.OfType<float>().Select(o => o.ToString()).ToArray();
        return string.Join(",", foo);
    }
    public static String intArrayToString(int[] farr)
    {
        string[] foo = farr.OfType<int>().Select(o => o.ToString()).ToArray();
        return string.Join(",", foo);
    }
    public static Vector3 GetScale(this Matrix4x4 m)
    {
        var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
        var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
        var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);

        return new Vector3(x, y, z);
    }
    private static float _copysign(float sizeval, float signval)
    {
        return Mathf.Sign(signval) == 1 ? Mathf.Abs(sizeval) : -Mathf.Abs(sizeval);
    }
    public static Quaternion GetRotation(this Matrix4x4 matrix)
    {
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + matrix.m00 + matrix.m11 + matrix.m22)) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + matrix.m00 - matrix.m11 - matrix.m22)) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - matrix.m00 + matrix.m11 - matrix.m22)) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - matrix.m00 - matrix.m11 + matrix.m22)) / 2;
        q.x = _copysign(q.x, matrix.m21 - matrix.m12);
        q.y = _copysign(q.y, matrix.m02 - matrix.m20);
        q.z = _copysign(q.z, matrix.m10 - matrix.m01);
        return q;
    }
    public static Vector3 GetPosition(this Matrix4x4 matrix)
    {
        var x = matrix.m03;
        var y = matrix.m13;
        var z = matrix.m23;

        return new Vector3(x, y, z);
    }
    public static Vector3 ScreenToWorldPoint(Matrix4x4 projectionMatrix, Matrix4x4 camdiff, Matrix4x4 worldToCamera,
                                             int pixelWidth, int pixelHeight, Vector3 screenPt)
    {
        Matrix4x4 w2c = camdiff * worldToCamera;
        Matrix4x4 worldToCam = projectionMatrix * w2c;
        Matrix4x4 camToWorld = worldToCam.inverse;
        float[] camToWorldArr = { camToWorld.m00, camToWorld.m01, camToWorld.m02, camToWorld.m03,
                    camToWorld.m10, camToWorld.m11, camToWorld.m12, camToWorld.m13,
                    camToWorld.m20, camToWorld.m21, camToWorld.m22, camToWorld.m23,
                    camToWorld.m30, camToWorld.m31, camToWorld.m32, camToWorld.m33
                };
        Vector3 pt = new Vector3((screenPt.x / pixelWidth) * 2.0f - 1.0f, (screenPt.y / pixelHeight) * 2.0f - 1.0f, screenPt.z);
        Vector3 worldPt = new Vector3();
        double w = camToWorldArr[12] * pt.x + camToWorldArr[13] * pt.y + camToWorldArr[14] * pt.z + camToWorldArr[15];
        double d = camToWorldArr[0] * pt.x + camToWorldArr[1] * pt.y + camToWorldArr[2] * pt.z + camToWorldArr[3];
        double d1 = camToWorldArr[4] * pt.x + camToWorldArr[5] * pt.y + camToWorldArr[6] * pt.z + camToWorldArr[7];
        worldPt.z = (float)((camToWorldArr[8] * pt.x + camToWorldArr[9] * pt.y + camToWorldArr[10] * pt.z + camToWorldArr[11]) / w);
        worldPt.x = (float)(d / w);
        worldPt.y = (float)(d1 / w);
        return worldPt;
    }
    public static void GenerateSphereGameObject(String goname, Vector3 worldPt, Color col, float scale)
    {
        GameObject spherego = GameObject.Find(goname);
        if (spherego == null)
        {
            spherego = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spherego.name = goname;
            spherego.GetComponent<Renderer>().material.color = col;
        }
        spherego.transform.position = worldPt;
        spherego.transform.localScale = scale * Vector3.one;
    }
    public static void computeCheckSum(byte[] dataRead, byte[] checksum)
    {
        int dataReadTotalLength = dataRead.Length;
        int padding = dataReadTotalLength % 2;
        ulong sum = 0L;
        int i = 0;
        for (; i < dataReadTotalLength - padding; i += 2)
        {
            sum += (ulong)((((dataRead[i]) << 8) & 0xFF00) + ((dataRead[i + 1]) & 0xFF));
        }
        if (padding > 0)
        {
            sum += (ulong)((((dataRead[i]) << 8) & 0xFF00));
        }
        while ((sum >> 16) > 0)
            sum = (sum & 0xFFFF) + (sum >> 16);

        sum = ~sum;
        checksum[0] = (byte)((sum & 0xff00) >> 8);
        checksum[1] = (byte)(sum & 0xff);

        /*		if (check){
			if ( ( ( (sum & 0xff00) >> 8) == (int)(checksum[0] & 0xff) ) &&
				( (sum & 0xff) == (int)(checksum[1] & 0xff) ) ){
				return true;
			}
			return false;
		}
		if (write){
			checksum[0] = ((sum & 0xff00) >> 8);
			checksum[1] = (sum & 0xff);
			return true;
		}
		return false;
		*/
    }
    public static double angle(Vector3 v1, Vector3 v2)
    {
        double vDot = Vector3.Dot(v1, v2) / (v1.magnitude * v2.magnitude);
        if (vDot < -1.0)
            vDot = -1.0;
        else if (vDot > 1.0)
            vDot = 1.0;
        return Math.Acos(vDot);
    }
    public static double angle_deg(Vector3 v1, Vector3 v2)
    {
        double rad = angle(v1, v2);
        return rad * 180.0f / Math.PI;
    }
    public static Bounds GenerateLocalBounds(GameObject go)
    {
        float[] coords = null;
        int[] indices = null;
        int nverts = 0;
        List<KeyValuePair<MeshFilter, Mesh>> meshList = new List<KeyValuePair<MeshFilter, Mesh>>();
        VMObject.GetMeshListForObject(meshList, go, ref nverts);
        if (meshList.Count == 0)
        {
            Debug.Log("WARNING: VMObject.reloadGeometry() has no geometry name=" + go.name);
            return new Bounds();
        }

        coords = new float[nverts * 3];
        List<KeyValuePair<int[], int>> allsmidxs = new List<KeyValuePair<int[], int>>();
        int nidx = 0;
        int pl = 0;
        foreach (KeyValuePair<MeshFilter, Mesh> kvp in meshList)
        {
            Mesh mesh = kvp.Value;
            Matrix4x4 transL2W = Matrix4x4.identity;// worldToTop * kvp.Key.transform.localToWorldMatrix;
            Vector3[] verts = mesh.vertices;
            foreach (Vector3 coord in verts)
            {
                Vector3 c = transL2W.MultiplyPoint(coord);
                coords[pl++] = c.x;
                coords[pl++] = c.y;
                coords[pl++] = c.z;
            }
            int smc = mesh.subMeshCount;
            for (int i = 0; i < smc; i++)
            {
                int[] idxs = mesh.GetIndices(i);
                int newnidx = 4 * (idxs.Length / 3);
                nidx += newnidx; // need to add -1 to every triangle
                allsmidxs.Add(new KeyValuePair<int[], int>(idxs, (i == 0) ? verts.Length : 0));
            }
        }
        indices = new int[nidx];
        pl = 0;
        int startpl = 0;
        foreach (KeyValuePair<int[], int> idxinfo in allsmidxs)
        {
            int[] idxs = idxinfo.Key;
            for (int n = 0; n < idxs.Length;)
            {
                indices[pl++] = startpl + idxs[n++];
                indices[pl++] = startpl + idxs[n++];
                indices[pl++] = startpl + idxs[n++];
                indices[pl++] = -1; // need to add -1 to every triangle
            }
            startpl += idxinfo.Value;
        }
        Bounds bounds = VMObject.GetBoundsFromMeshList(meshList, go);
        return bounds;
    }
    public static void getClosestVMObjectPoint(Bounds bound, out Vector3 connectorCentroid, out bool connectorCentroidIsSet)
    {
        Vector3 campos = ViewManager.getCurrentCamera().transform.position;
        Vector3 dirToBounds = (bound.center - campos);
        dirToBounds.Normalize();
        Vector3 dirToBoundsAbs = new Vector3(Math.Abs(dirToBounds.x), Math.Abs(dirToBounds.y), Math.Abs(dirToBounds.z));
        /* never y if (dirToBoundsAbs.y > dirToBoundsAbs.x && dirToBoundsAbs.y > dirToBoundsAbs.z) {
            // y is max
            connectorCentroid.x = bound.center.x;
            connectorCentroid.z = bound.center.z;
            connectorCentroid.y = dirToBounds.y > 0 ? bound.max.y : bound.min.y;
        } else */
        if (dirToBoundsAbs.z > dirToBoundsAbs.x && dirToBoundsAbs.z > dirToBoundsAbs.y) {
            // z is max
            connectorCentroid.x = bound.center.x;
            connectorCentroid.y = bound.center.y;
            connectorCentroid.z = dirToBounds.z < 0 ? bound.max.z : bound.min.z;
        } else {
            // x is max
            connectorCentroid.y = bound.center.y;
            connectorCentroid.z = bound.center.z;
            connectorCentroid.x = dirToBounds.x < 0 ? bound.max.x : bound.min.x;
        }
        connectorCentroidIsSet = true;
    }
}