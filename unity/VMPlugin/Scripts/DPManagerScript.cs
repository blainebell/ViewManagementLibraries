// these don't look like they are available (maybe in the free version?)
#define DEBUG_DP
//#define EDITOR_COMPILES_ANDROID

//#if UNITY_IOS
#if !UNITY_EDITOR_OSX && UNITY_IOS
#define UNITY_IOS_ONLY
#endif

#if !UNITY_EDITOR_OSX && UNITY_WEBGL
#define UNITY_WEBGL_ONLY
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using AOT;

public class DPManagerScript : MonoBehaviour {
#if UNITY_WSA && !UNITY_EDITOR
    const string dll = "DPCoreUWP";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    const string dll = "DPCoreDLL";
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
    const string dll = "DPCoreSO";
#elif UNITY_IOS_ONLY
	const string dll = "__Internal";
#elif (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
    const string dll = "DPCoreBundle";
#elif UNITY_WEBGL_ONLY
	const string dll = "__Internal";
#else
    const string dll = ""; // TODO: need to support other platforms
#endif
    public static DPManagerScript Instance = null;

	DPManagerScript(){
		Instance = this;
	}

	public delegate void Log_Delegate(string message);
	static public Log_Delegate Log_Callback = null;

#if !UNITY_ANDROID || (!EDITOR_COMPILES_ANDROID && UNITY_EDITOR)
	static MyDelegate unitylog_callback_delegate = null;
	static MyDelegate log_callback_delegate = null;
	static MyDelegate err_callback_delegate = null;
	static MyDelegate_rb obj_exists_callback_delegate = null;
	static MyDelegate_rb_i obj_annot_active_callback_delegate;

    [DllImport(dll)]
    public static extern void ClearLicense();
    [DllImport(dll)]
    public static extern bool ValidateLicense();
    [DllImport(dll)]
    public static extern bool SaveNewLicense(string publicKey, string licname, string licstr, string email, string expDate);
    [DllImport(dll)]
    public static extern IntPtr EncryptLicenseRequestImpl(string publicKey, string email, string passwd);
    public static string EncryptLicenseRequest(string publicKey, string email, string passwd)
    {
        IntPtr encLicReqPtr = EncryptLicenseRequestImpl(publicKey, email, passwd);
        string encLicReq = Marshal.PtrToStringAnsi(encLicReqPtr);
        return encLicReq;
    }
    [DllImport(dll)]
    public static extern int CheckLicenseStatus();
    [DllImport(dll)]
    public static extern IntPtr GetExpirationDateImpl();
    public static string GetExpirationDate()
    {
        IntPtr expDatePtr = GetExpirationDateImpl();
        string expDate = DPUtils.Marshal_PtrToStringAnsi(expDatePtr);
        return expDate;
    }
    [DllImport(dll)]
    public static extern IntPtr GetEmailImpl();
    public static string GetEmail()
    {
        IntPtr emailPtr = GetEmailImpl();
        string email = DPUtils.Marshal_PtrToStringAnsi(emailPtr);
        return email;
    }
    [DllImport(dll)]
    public static extern int licenseStatusGet();
    [DllImport(dll)]
    public static extern void licenseStatusSet(int licStatus);

    // End License Stuff

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void MyDelegate(string str);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate bool MyDelegate_rb(string str);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate bool MyDelegate_rb_i(int goid);

	[MonoPInvokeCallback(typeof(MyDelegate))]
	static void ErrCallBackFunction(string str)
	{
		if (str == null)
		{
			Debug.Log("ErrCallBackFunction: WARNING: str=null");
			return;
		}
		if (Log_Callback != null) {
			Log_Callback ("Error: " + str);
		} else if (log_errors && str.Trim().Length > 0) {
			Debug.Log ("Error: " + str);
		}
	}

	[MonoPInvokeCallback(typeof(MyDelegate))]
	static void LogCallBackFunction(string str)
	{
		if (str == null)
		{
			Debug.Log("LogCallBackFunction: WARNING: str=null");
			return;
		}
		if (Log_Callback != null) {
			Log_Callback (str);
		} else if (log_logs && str.Trim().Length > 0) {
		Debug.Log ("Log: " + str);
		}
	}
	[MonoPInvokeCallback(typeof(MyDelegate))]
	static void UnityLogCallBackFunction(string str)
	{
		if (log_unity && str.Trim().Length > 0) {
			Debug.Log ("Unity: " + str);
		}
		if (Log_Callback != null) {
			Log_Callback ("Unity: " + str);
		}
	}
	[MonoPInvokeCallback(typeof(MyDelegate_rb))]
	static bool UnityObjectExistsCallBackFunction(string str)
	{
		return GameObject.Find (str) != null;
	}

	// TODO: NEED TO REMOVE UnityObjectAnnotationActiveFunction function
	[MonoPInvokeCallback(typeof(MyDelegate_rb_i))]
	static bool UnityObjectAnnotationActiveFunction(int goid)
	{
		Dictionary<int, GameObject> vmobjs = VMObject.m_VMObjects;
		GameObject go = vmobjs [goid];
		VMObject vmo = go.GetComponent<VMObject> ();
		if (vmo != null) {
			return vmo.annotationGet().activeSelf;
		} else {
		Debug.Log ("UnityObjectAnnotationActiveFunction: goid=" + goid + " vmo=" + vmo);
		}
		return false;
	}




//BEGIN Delegates
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i ( int arg1 );
static MyDelegate_i unity_i_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_b ( int arg1, bool arg2 );
static MyDelegate_i_b unity_i_b_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_f ( int arg1, float arg2 );
static MyDelegate_i_f unity_i_f_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_f_f_f ( int arg1, float arg2, float arg3, float arg4 );
static MyDelegate_i_f_f_f unity_i_f_f_f_delegate;
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_fa ( int arg1, IntPtr arg2_1, int arg2_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_fa ( int arg1, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] float[] arg2_1, int arg2_2 );
static MyDelegate_i_fa unity_i_fa_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_fa_s ( int arg1, IntPtr arg2_1, int arg2_2, [MarshalAs(UnmanagedType.LPStr)] string arg3 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_fa_s ( int arg1, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] float[] arg2_1, int arg2_2, [MarshalAs(UnmanagedType.LPStr)] string arg3 );
static MyDelegate_i_fa_s unity_i_fa_s_delegate;
#endif
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i_b ( int arg1, int arg2, bool arg3 );
static MyDelegate_i_i_b unity_i_i_b_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i ( int arg1, int arg2 );
static MyDelegate_i_i unity_i_i_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2 );
static MyDelegate_i_s unity_i_s_delegate;
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_ba ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, IntPtr arg3_1, int arg3_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_ba ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] byte[] arg3_1, int arg3_2 );
static MyDelegate_i_s_ba unity_i_s_ba_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i_ia ( int arg1, int arg2, IntPtr arg3_1, int arg3_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i_ia ( int arg1, int arg2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] int[] arg3_1, int arg3_2 );
static MyDelegate_i_i_ia unity_i_i_ia_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i_fa ( int arg1, int arg2, IntPtr arg3_1, int arg3_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i_fa ( int arg1, int arg2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] float[] arg3_1, int arg3_2 );
static MyDelegate_i_i_fa unity_i_i_fa_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i_i_fa ( int arg1, int arg2, int arg3, IntPtr arg4_1, int arg4_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i_i_fa ( int arg1, int arg2, int arg3, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] float[] arg4_1, int arg4_2 );
static MyDelegate_i_i_i_fa unity_i_i_i_fa_delegate;
#endif
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_b ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, bool arg3 );
static MyDelegate_i_s_b unity_i_s_b_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3 );
static MyDelegate_i_s_s unity_i_s_s_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_s_b ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, bool arg4 );
static MyDelegate_i_s_s_b unity_i_s_s_b_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_s_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4 );
static MyDelegate_i_s_s_s unity_i_s_s_s_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i_s_s ( int arg1, int arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4 );
static MyDelegate_i_i_s_s unity_i_i_s_s_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i_i_s_s ( int arg1, int arg2, int arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5 );
static MyDelegate_i_i_i_s_s unity_i_i_i_s_s_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i_i_s_s_s ( int arg1, int arg2, int arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5, [MarshalAs(UnmanagedType.LPStr)] string arg6 );
static MyDelegate_i_i_i_s_s_s unity_i_i_i_s_s_s_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_i ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, int arg3 );
static MyDelegate_i_s_i unity_i_s_i_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_i_i ( int arg1, int arg2, int arg3 );
static MyDelegate_i_i_i unity_i_i_i_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_i_i ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, int arg3, int arg4 );
static MyDelegate_i_s_i_i unity_i_s_i_i_delegate;
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_i_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, int arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4 );
static MyDelegate_i_s_i_s unity_i_s_i_s_delegate;
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_i_fa ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, int arg3, IntPtr arg4_1, int arg4_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_i_fa ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, int arg3, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] float[] arg4_1, int arg4_2 );
static MyDelegate_i_s_i_fa unity_i_s_i_fa_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, IntPtr arg3_1, int arg3_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] float[] arg3_1, int arg3_2 );
static MyDelegate_i_s_fa unity_i_s_fa_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, IntPtr arg3_1, int arg3_2, [MarshalAs(UnmanagedType.LPStr)] string arg4 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] float[] arg3_1, int arg3_2, [MarshalAs(UnmanagedType.LPStr)] string arg4 );
static MyDelegate_i_s_fa_s unity_i_s_fa_s_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_s_fa_b_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, IntPtr arg4_1, int arg4_2, bool arg5, [MarshalAs(UnmanagedType.LPStr)] string arg6 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_s_fa_b_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] float[] arg4_1, int arg4_2, bool arg5, [MarshalAs(UnmanagedType.LPStr)] string arg6 );
static MyDelegate_i_s_s_fa_b_s unity_i_s_s_fa_b_s_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_s_fa_ba_b_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, IntPtr arg4_1, int arg4_2, IntPtr arg5_1, int arg5_2, bool arg6, [MarshalAs(UnmanagedType.LPStr)] string arg7 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_s_fa_ba_b_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] float[] arg4_1, int arg4_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=6)] byte[] arg5_1, int arg5_2, bool arg6, [MarshalAs(UnmanagedType.LPStr)] string arg7 );
static MyDelegate_i_s_s_fa_ba_b_s unity_i_s_s_fa_ba_b_s_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa_b_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, IntPtr arg3_1, int arg3_2, bool arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa_b_s ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] float[] arg3_1, int arg3_2, bool arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5 );
static MyDelegate_i_s_fa_b_s unity_i_s_fa_b_s_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa_fa ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, IntPtr arg3_1, int arg3_2, IntPtr arg4_1, int arg4_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa_fa ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] float[] arg3_1, int arg3_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=5)] float[] arg4_1, int arg4_2 );
static MyDelegate_i_s_fa_fa unity_i_s_fa_fa_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa_ia ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, IntPtr arg3_1, int arg3_2, IntPtr arg4_1, int arg4_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa_ia ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] float[] arg3_1, int arg3_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=5)] int[] arg4_1, int arg4_2 );
static MyDelegate_i_s_fa_ia unity_i_s_fa_ia_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa_ia_ia ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, IntPtr arg3_1, int arg3_2, IntPtr arg4_1, int arg4_2, IntPtr arg5_1, int arg5_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_s_fa_ia_ia ( int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] float[] arg3_1, int arg3_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=5)] int[] arg4_1, int arg4_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=7)] int[] arg5_1, int arg5_2 );
static MyDelegate_i_s_fa_ia_ia unity_i_s_fa_ia_ia_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_fa_ia_ia_ba_ia ( int arg1, IntPtr arg2_1, int arg2_2, IntPtr arg3_1, int arg3_2, IntPtr arg4_1, int arg4_2, IntPtr arg5_1, int arg5_2, IntPtr arg6_1, int arg6_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_fa_ia_ia_ba_ia ( int arg1, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] float[] arg2_1, int arg2_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] int[] arg3_1, int arg3_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=6)] int[] arg4_1, int arg4_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=8)] byte[] arg5_1, int arg5_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=10)] int[] arg6_1, int arg6_2 );
static MyDelegate_i_fa_ia_ia_ba_ia unity_i_fa_ia_ia_ba_ia_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_fa_ia_ia_ba_ia_fa ( int arg1, IntPtr arg2_1, int arg2_2, IntPtr arg3_1, int arg3_2, IntPtr arg4_1, int arg4_2, IntPtr arg5_1, int arg5_2, IntPtr arg6_1, int arg6_2, IntPtr arg7_1, int arg7_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_fa_ia_ia_ba_ia_fa ( int arg1, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] float[] arg2_1, int arg2_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] int[] arg3_1, int arg3_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=6)] int[] arg4_1, int arg4_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=8)] byte[] arg5_1, int arg5_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=10)] int[] arg6_1, int arg6_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=12)] float[] arg7_1, int arg7_2 );
static MyDelegate_i_fa_ia_ia_ba_ia_fa unity_i_fa_ia_ia_ba_ia_fa_delegate;
#endif
#if !UNITY_EDITOR_OSX && UNITY_IOS 
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_fa_ia_ba_ia_fa ( int arg1, IntPtr arg2_1, int arg2_2, IntPtr arg3_1, int arg3_2, IntPtr arg4_1, int arg4_2, IntPtr arg5_1, int arg5_2, IntPtr arg6_1, int arg6_2 );
#else
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MyDelegate_i_fa_ia_ba_ia_fa ( int arg1, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] float[] arg2_1, int arg2_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] int[] arg3_1, int arg3_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=6)] byte[] arg4_1, int arg4_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=8)] int[] arg5_1, int arg5_2, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=10)] float[] arg6_1, int arg6_2 );
static MyDelegate_i_fa_ia_ba_ia_fa unity_i_fa_ia_ba_ia_fa_delegate;
#endif
    //END Delegates
    //	public delegate void MyDelegate_s_fa([MarshalAs(UnmanagedType.LPStr)] string str, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] float[] fa, int fa_len);
    /*	public delegate void MyDelegate_i_s( int op, [MarshalAs(UnmanagedType.LPStr)] string str);
        public delegate void MyDelegate_i_s_b( int op, [MarshalAs(UnmanagedType.LPStr)] string str, bool val);
        public delegate void MyDelegate_i_s_fa( int op, [MarshalAs(UnmanagedType.LPStr)] string str, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] float[] fa, int fa_len);
        public delegate void MyDelegate_i_s_fa_ia( int op, [MarshalAs(UnmanagedType.LPStr)] string str,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] float[] fa, int fa_len,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=5)] int[] ia, int ia_len  );
        public delegate void MyDelegate_i_s_fa_ia_ia( int op, [MarshalAs(UnmanagedType.LPStr)] string str,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] float[] fa, int fa_len,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=5)] int[] ia, int ia_len,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=7)] int[] ia2, int ia2_len );
    */
    static public List<string> ConvertReturnedPtrToStringList(IntPtr ptr)
    {
        List<string> allIDs = new List<string>();
        if (!ptr.Equals(IntPtr.Zero)) {
            int nitems = Marshal.ReadInt32(ptr);
            IntPtr stringPtr = new IntPtr(ptr.ToInt64() + 4);
            while (allIDs.Count < nitems) {
                string buffer = Marshal.PtrToStringAnsi(stringPtr);
                allIDs.Add(buffer);
                stringPtr = new IntPtr(stringPtr.ToInt64() + buffer.Length + 1);
            }
            Marshal.FreeCoTaskMem(ptr);
        }
        return allIDs;
    }
    static public List<int> ConvertReturnedPtrToIntList(IntPtr ptr)
    {
        List<int> allIDs = new List<int>();
        if (!ptr.Equals(IntPtr.Zero))
        {
            IntPtr stringPtr = ptr;
            bool cont;
            do
            {
                string buffer = Marshal.PtrToStringAnsi(stringPtr);
                int val;
                if (int.TryParse(buffer, out val)){
                    allIDs.Add(val);
                }
                stringPtr = new IntPtr(stringPtr.ToInt64() + buffer.Length + 1);
                cont = buffer.Length > 0;
            } while (cont);
            Marshal.FreeCoTaskMem(ptr);
        }
        return allIDs;
    }

    //BEGIN SetFunctions
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_b (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_f (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_f_f_f (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_fa (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_fa_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_i_b (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_i (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_ba (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_i_ia (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_i_fa (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_i_i_fa (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_b (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_s_b (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_s_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_i_s_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_i_i_s_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_i_i_s_s_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_i (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_i_i (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_i_i (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_i_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_i_fa (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_fa (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_fa_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_s_fa_b_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_s_fa_ba_b_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_fa_b_s (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_fa_fa (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_fa_ia (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_s_fa_ia_ia (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_fa_ia_ia_ba_ia (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_fa_ia_ia_ba_ia_fa (IntPtr fp);
	[ DllImport (dll) ]
	public static extern void SetUnityFunction_i_fa_ia_ba_ia_fa (IntPtr fp);
//END SetFunctions

	[ DllImport (dll) ]
	public static extern void SetUnityLogFunction( IntPtr fp );
	[ DllImport (dll) ]
	public static extern void SetLogFunction( IntPtr fp );
	[ DllImport (dll) ]
	public static extern void SetErrFunction( IntPtr fp );
	[ DllImport (dll) ]
	public static extern void SetupDPProperty( [MarshalAs(UnmanagedType.LPStr)] string propName, [MarshalAs(UnmanagedType.LPStr)] string propvalue );
	[ DllImport (dll) ]
	public static extern void InitDP( [MarshalAs(UnmanagedType.LPStr)] string dataFilesDirectory );
    [DllImport(dll)]
    public static extern bool GetLicenseValidated();
    [DllImport(dll)]
    public static extern void DestroyDP();

	[ DllImport (dll) ]
	public static extern void SetUnityObjectExistsFunction( IntPtr fp );

	[ DllImport (dll) ]
	public static extern void SetUnityObjectAnnotationActiveFunction( IntPtr fp );

//BEGIN CPPCalls
[ DllImport (dll) ]
public static extern void Call_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1);
[ DllImport (dll) ]
public static extern void Call_iba( [MarshalAs(UnmanagedType.LPStr)] string func_name, [In, Out] int [] arg1_1, int arg1_2);
[ DllImport (dll) ]
public static extern void Call_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1);
[ DllImport (dll) ]
public static extern void Call_b_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, bool arg1, bool arg2);
[ DllImport (dll) ]
public static extern void Call_b_b_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, bool arg1, bool arg2, bool arg3);
[ DllImport (dll) ]
public static extern void Call_i_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2);
[ DllImport (dll) ]
public static extern void Call_s_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2);
[ DllImport (dll) ]
public static extern void Call_i_s_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, int arg3);
[ DllImport (dll) ]
public static extern void Call_i_s_i_cD( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, int arg3, double [] arg4);
[ DllImport (dll) ]
public static extern void Call_i_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, int arg2);
[ DllImport (dll) ]
public static extern void Call_i_i_i_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, int arg2, int arg3, bool arg4);
[ DllImport (dll) ]
public static extern void Call_i_i_i_p3_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, int arg2, int arg3, float [] arg4, bool arg5);
[ DllImport (dll) ]
public static extern void Call_i_i_i_p3_b_b_p3( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, int arg2, int arg3, float [] arg4, bool arg5, bool arg6, float [] arg7);
[ DllImport (dll) ]
public static extern void Call_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, bool arg1);
[ DllImport (dll) ]
public static extern void Call_i_fba( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, [In, Out] float [] arg2_1, int arg2_2);
[ DllImport (dll) ]
public static extern void Call_i_ba( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, [In, Out] byte [] arg2_1, int arg2_2);
[ DllImport (dll) ]
public static extern void Call_i_i_ba( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, int arg2, [In, Out] byte [] arg3_1, int arg3_2);
[ DllImport (dll) ]
public static extern void Call_i_i_ba_i_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, int arg2, [In, Out] byte [] arg3_1, int arg3_2, int arg4, int arg5);
[ DllImport (dll) ]
public static extern void Call_ba_i_p2( [MarshalAs(UnmanagedType.LPStr)] string func_name, [In, Out] byte [] arg1_1, int arg1_2, int arg2, float [] arg3);
[ DllImport (dll) ]
public static extern void Call_i_iba_fba( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, [In, Out] int [] arg2_1, int arg2_2, [In, Out] float [] arg3_1, int arg3_2);
[ DllImport (dll) ]
public static extern void Call_i_iba_fba_b_b_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, [In, Out] int [] arg2_1, int arg2_2, [In, Out] float [] arg3_1, int arg3_2, bool arg4, bool arg5, bool arg6);
[ DllImport (dll) ]
public static extern void Call_i_iba_fba_fba_b_b_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, [In, Out] int [] arg2_1, int arg2_2, [In, Out] float [] arg3_1, int arg3_2, [In, Out] float [] arg4_1, int arg4_2, bool arg5, bool arg6, bool arg7);
[ DllImport (dll) ]
public static extern void Call_i_iba_fba_f( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, [In, Out] int [] arg2_1, int arg2_2, [In, Out] float [] arg3_1, int arg3_2, float arg4);
[ DllImport (dll) ]
public static extern void Call_s_s_s_s_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, int arg5);
[ DllImport (dll) ]
public static extern void Call_s_s_s_s_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, bool arg5);
[ DllImport (dll) ]
public static extern void Call_s_s_s_s_f( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, float arg5);
[ DllImport (dll) ]
public static extern void Call_i_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, bool arg2);
[ DllImport (dll) ]
public static extern void Call_i_b_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, bool arg2, bool arg3);
[ DllImport (dll) ]
public static extern void Call_i_b_b_b_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, bool arg2, bool arg3, bool arg4, int arg5);
[ DllImport (dll) ]
public static extern void Call_i_b_b_b_i_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, bool arg2, bool arg3, bool arg4, int arg5, bool arg6);
[ DllImport (dll) ]
public static extern void Call_i_b_b_b_b_f( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, bool arg2, bool arg3, bool arg4, bool arg5, float arg6);
[ DllImport (dll) ]
public static extern void Call_s_i_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3);
[ DllImport (dll) ]
public static extern void Call_s_s_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, bool arg3);
[ DllImport (dll) ]
public static extern void Call_i_s_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3);
[ DllImport (dll) ]
public static extern void Call_i_t3f_t3f( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, float [] arg2, float [] arg3);
[ DllImport (dll) ]
public static extern void Call_i_s_i_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, int arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4);
[ DllImport (dll) ]
public static extern void Call_s_s_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3);
[ DllImport (dll) ]
public static extern void Call_s_s_s_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4);
[ DllImport (dll) ]
public static extern void Call_s_i_s_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4);
[ DllImport (dll) ]
public static extern void Call_s_s_s_i_t3f( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4, float [] arg5);
[ DllImport (dll) ]
public static extern void Call_s_p3_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, float [] arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3);
[ DllImport (dll) ]
public static extern void Call_s_i_s_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4);
[ DllImport (dll) ]
public static extern void Call_s_i_i_s_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5);
[ DllImport (dll) ]
public static extern void Call_s_i_p3_i_s_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, float [] arg3, int arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5, [MarshalAs(UnmanagedType.LPStr)] string arg6);
[ DllImport (dll) ]
public static extern void Call_s_i_i_p3_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3, float [] arg4, int arg5);
[ DllImport (dll) ]
public static extern void Call_s_i_i_p3_i_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3, float [] arg4, int arg5, bool arg6);
[ DllImport (dll) ]
public static extern void Call_s_i_i_p3_i_s_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3, float [] arg4, int arg5, [MarshalAs(UnmanagedType.LPStr)] string arg6, [MarshalAs(UnmanagedType.LPStr)] string arg7);
[ DllImport (dll) ]
public static extern void Call_s_s_s_i_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4, bool arg5);
[ DllImport (dll) ]
public static extern void Call_s_s_s_i_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4, int arg5);
[ DllImport (dll) ]
public static extern void Call_s_s_s_i_f( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4, float arg5);
[ DllImport (dll) ]
public static extern void Call_s_s_s_i_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5);
[ DllImport (dll) ]
public static extern void Call_s_s_s_s_s( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5);
[ DllImport (dll) ]
public static extern void Call_p2_p2( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2);
[ DllImport (dll) ]
public static extern void Call_p3_p3( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2);
[ DllImport (dll) ]
public static extern void Call_p3_p3_rI( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, int [] arg3);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, int arg4);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_rI( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, int [] arg4);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_rI_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, int [] arg4, bool arg5);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_rI_b_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, int [] arg4, bool arg5, bool arg6);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_rI_p2_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, int [] arg4, float [] arg5, bool arg6);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_t3f_rI_p2_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, int [] arg5, float [] arg6, bool arg7);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_t3f_rI_p2_f_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, int [] arg5, float [] arg6, float arg7, bool arg8);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_t3f_rI_p2_f_b_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, int [] arg5, float [] arg6, float arg7, bool arg8, bool arg9);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_t3f_rI_p2_f_f_b_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, int [] arg5, float [] arg6, float arg7, float arg8, bool arg9, bool arg10);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_t3f_t3f_rI_p2_f_f_b_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, float [] arg5, int [] arg6, float [] arg7, float arg8, float arg9, bool arg10, bool arg11);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_t3f_t3f_p2_f_f( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, float [] arg5, float [] arg6, float arg7, float arg8);
[ DllImport (dll) ]
public static extern void Call_p3_p3_t3f_rI_p2_b_b( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, int [] arg4, float [] arg5, bool arg6, bool arg7);
//END CPPCalls

    [DllImport (dll) ]
	public static extern int IsUp( );
	[ DllImport (dll) ]
	public static extern void Call_noargs( [MarshalAs(UnmanagedType.LPStr)] string func_name );

	[ DllImport (dll) ]
	public static extern void Call_line( [MarshalAs(UnmanagedType.LPStr)] string line_cmd );

	[ DllImport(dll) ]
	public static extern IntPtr Call_rsa( [MarshalAs(UnmanagedType.LPStr)] string func_name);

	public static String[] Call_rsa_wrapper(string func_name){
		return DPManagerScript.ConvertReturnedPtrToStringList(DPManagerScript.Call_rsa(func_name)).ToArray();
	}

	[ DllImport (dll) ]
	public static extern int Call_ri_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1);
	[ DllImport (dll) ]
	public static extern int Call_rb_i_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, int arg1, int arg2);
	[ DllImport (dll) ]
	public static extern int Call_rb_s_i_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3);
	[ DllImport (dll) ]
	public static extern int Call_rb_s_i_i_p3( [MarshalAs(UnmanagedType.LPStr)] string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3, float [] arg4);
	[ DllImport (dll) ]
	public static extern int Call_ri_noargs( [MarshalAs(UnmanagedType.LPStr)] string func_name);
	[ DllImport (dll) ]
	public static extern int Call_ri_p3_p3_t3f_i( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2, float [] arg3, int arg4);
	[ DllImport (dll) ]
	public static extern int Call_ri_p2_p2( [MarshalAs(UnmanagedType.LPStr)] string func_name, float [] arg1, float [] arg2);

	[ DllImport (dll) ]
	public static extern void SetIntAt( int addrptr, int val);
#else
	static AndroidJavaClass dpUnityAndroidClass = new AndroidJavaClass ("org.blainebell.unity.DPUnityAndroid");


/*	[DllImport(dll)]
	public static extern void ClearLicense();
	[DllImport(dll)]
	public static extern bool ValidateLicense();
	[DllImport(dll)]
	public static extern bool SaveNewLicense(string publicKey, string licname, string licstr, string email, string expDate);
	[DllImport(dll)]
	public static extern IntPtr EncryptLicenseRequestImpl(string publicKey, string email, string passwd);
	public static string EncryptLicenseRequest(string publicKey, string email, string passwd)
	{
		IntPtr encLicReqPtr = EncryptLicenseRequestImpl(publicKey, email, passwd);
		string encLicReq = Marshal.PtrToStringAnsi(encLicReqPtr);
		return encLicReq;
	}
	[DllImport(dll)]
	public static extern int CheckLicenseStatus();
	[DllImport(dll)]
	public static extern IntPtr GetExpirationDateImpl();
	public static string GetExpirationDate()
	{
		IntPtr expDatePtr = GetExpirationDateImpl();
		string expDate = DPUtils.Marshal_PtrToStringAnsi(expDatePtr);
		return expDate;
	}
	[DllImport(dll)]
	public static extern IntPtr GetEmailImpl();
	public static string GetEmail()
	{
		IntPtr emailPtr = GetEmailImpl();
		string email = DPUtils.Marshal_PtrToStringAnsi(emailPtr);
		return email;
	}
	[DllImport(dll)]
	public static extern int licenseStatusGet();
	[DllImport(dll)]
	public static extern void licenseStatusSet(int licStatus);
		*/

	public static string GetEmail() {
		// TODO : Licensing on Android
		return "";
	}
	public static string GetExpirationDate(){
		// TODO : Licensing on Android
		return "";
	}
	public static int licenseStatusGet(){
		// TODO : Licensing on Android
		return (int)ViewManager.LicenseStatus.NonExistent;
	}
	public static void licenseStatusSet(int licStatus){
		// TODO : Licensing on Android
	}
	public static int CheckLicenseStatus(){
		// TODO : Licensing on Android
		return (int)ViewManager.LicenseStatus.NonExistent;
	}
	public static string EncryptLicenseRequest(string publicKey, string email, string passwd){
		// TODO : Licensing on Android
		return "";
	}
	public static bool SaveNewLicense(string publicKey, string licname, string licstr, string email, string expDate){
		// TODO : Licensing on Android
		return false;
	}
	public static void ClearLicense(){
		// TODO : Licensing on Android
	}

	public static int IsUp( ){
		return dpUnityAndroidClass.CallStatic<int> ("IsUp", new object[]{ } );
	}
	public static void Call_noargs( string func_name ){
		dpUnityAndroidClass.CallStatic ("Call_noargs", new object[]{ func_name } );
	}
	public static void Call_line( string line_cmd ){
		dpUnityAndroidClass.CallStatic ("Call_line", new object[]{ line_cmd } );
	}
	public static int Call_ri_i( string func_name, int arg1){
		return dpUnityAndroidClass.CallStatic<int> ("Call_ri_i", new object[]{ func_name, arg1 } );
	}
	public static int Call_rb_i_i( string func_name, int arg1, int arg2){
		return dpUnityAndroidClass.CallStatic<int> ("Call_rb_i_i", new object[]{ func_name, arg1, arg2 } );
	}
	public static int Call_rb_s_i_i( string func_name, string arg1, int arg2, int arg3){
		return dpUnityAndroidClass.CallStatic<int> ("Call_rb_s_i_i", new object[]{ func_name, arg1, arg2, arg3 } );
	}
	public static int Call_rb_s_i_i_p3( string func_name, string arg1, int arg2, int arg3, float [] arg4){
		return dpUnityAndroidClass.CallStatic<int> ("Call_rb_s_i_i_p3", new object[]{ func_name, arg1, arg2, arg3, arg4 } );
	}
	public static int Call_ri_noargs( string func_name){
		return dpUnityAndroidClass.CallStatic<int> ("Call_ri_noargs", new object[]{ func_name } );
	}
	public static int Call_ri_p3_p3_t3f_i( string func_name, float [] arg1, float [] arg2, float [] arg3, int arg4){
		return dpUnityAndroidClass.CallStatic<int> ("Call_ri_p3_p3_t3f_i", new object[]{ func_name, arg1, arg2, arg3, arg4 } );
	}
	public static int Call_ri_p2_p2( string func_name, float [] arg1, float [] arg2){
		return dpUnityAndroidClass.CallStatic<int> ("Call_ri_p2_p2", new object[]{ func_name, arg1, arg2 } );
	}
	public static String[] Call_rsa_wrapper(string func_name){
		AndroidJavaObject retList = dpUnityAndroidClass.CallStatic<AndroidJavaObject>("Call_rsa", new object[] { func_name });
		return retList.Call<string[]> ("toArray", new object[0]);
	}
	public static void DestroyDP(){
		dpUnityAndroidClass.CallStatic("DestroyDP", new object[] { });
	}
	public static void SetupDPProperty( string propName, string propvalue ){
		dpUnityAndroidClass.CallStatic ("SetupDPProperty", new object[] { propName, propvalue });
	}
	public static void InitDP(string dirName){
		// TODO: IMPLEMENT
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
		DPcallbackIns = new DPAndroidCallback ();
		dpUnityAndroidClass.CallStatic("InitDP", new object[] { jo, DPcallbackIns });
	}
	static DPAndroidCallback DPcallbackIns = null;
	class DPAndroidCallback : AndroidJavaProxy {
		public DPAndroidCallback() : base("org.blainebell.unity.DPUnityCallback") {
		}
		override public AndroidJavaObject Invoke(string methodName, AndroidJavaObject [] javaArgs){
			int funcNum = javaArgs[0].Call<int> ("intValue", new object[0]);
			switch (funcNum){
				//BEGIN AndroidCallbacks
			case 1:
				{
					// calling Unity_Function_i_i
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					int arg2 = javaArgs[2].Call<int> ("intValue", new object[0]);;
					Unity_Function_i_i(arg1, arg2);
				}
				break;
			case 2:
				{
					// calling Unity_Function_i_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					Unity_Function_i_s(arg1, arg2);
				}
				break;
			case 3:
				{
					// calling Unity_Function_i_i_b
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					int arg2 = javaArgs[2].Call<int> ("intValue", new object[0]);;
					bool arg3 = javaArgs[3].Call<bool> ("booleanValue", new object[0]);;
					Unity_Function_i_i_b(arg1, arg2, arg3);
				}
				break;
			case 4:
				{
					// calling Unity_Function_i_s_b
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					bool arg3 = javaArgs[3].Call<bool> ("booleanValue", new object[0]);;
					Unity_Function_i_s_b(arg1, arg2, arg3);
				}
				break;
			case 5:
				{
					// calling Unity_Function_i_s_i
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					int arg3 = javaArgs[3].Call<int> ("intValue", new object[0]);;
					Unity_Function_i_s_i(arg1, arg2, arg3);
				}
				break;
			case 6:
				{
					// calling Unity_Function_i_s_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					string arg3 = javaArgs[3].Call<string> ("toString", new object[0]);;
					Unity_Function_i_s_s(arg1, arg2, arg3);
				}
				break;
			case 7:
				{
					// calling Unity_Function_i_s_i_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					int arg3 = javaArgs[3].Call<int> ("intValue", new object[0]);;
					string arg4 = javaArgs[4].Call<string> ("toString", new object[0]);;
					Unity_Function_i_s_i_s(arg1, arg2, arg3, arg4);
				}
				break;
			case 8:
				{
					// calling Unity_Function_i_s_s_b
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					string arg3 = javaArgs[3].Call<string> ("toString", new object[0]);;
					bool arg4 = javaArgs[4].Call<bool> ("booleanValue", new object[0]);;
					Unity_Function_i_s_s_b(arg1, arg2, arg3, arg4);
				}
				break;
			case 9:
				{
					// calling Unity_Function_i_s_s_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					string arg3 = javaArgs[3].Call<string> ("toString", new object[0]);;
					string arg4 = javaArgs[4].Call<string> ("toString", new object[0]);;
					Unity_Function_i_s_s_s(arg1, arg2, arg3, arg4);
				}
				break;
			case 10:
				{
					// calling Unity_Function_i_i_i_s_s_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					int arg2 = javaArgs[2].Call<int> ("intValue", new object[0]);;
					int arg3 = javaArgs[3].Call<int> ("intValue", new object[0]);;
					string arg4 = javaArgs[4].Call<string> ("toString", new object[0]);;
					string arg5 = javaArgs[5].Call<string> ("toString", new object[0]);;
					string arg6 = javaArgs[6].Call<string> ("toString", new object[0]);;
					Unity_Function_i_i_i_s_s_s(arg1, arg2, arg3, arg4, arg5, arg6);
				}
				break;
			case 11:
				{
					// calling Unity_Function_i_i_fa
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					int arg2 = javaArgs[2].Call<int> ("intValue", new object[0]);;
					float [] arg3 = javaArgs[3].Call<float []> ("valueGet", new object[0]);;
					Unity_Function_i_i_fa(arg1, arg2, arg3, arg3.Length);
				}
				break;
			case 12:
				{
					// calling Unity_Function_i_i_ia
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					int arg2 = javaArgs[2].Call<int> ("intValue", new object[0]);;
					int [] arg3 = javaArgs[3].Call<int []> ("valueGet", new object[0]);;
					Unity_Function_i_i_ia(arg1, arg2, arg3, arg3.Length);
				}
				break;
			case 13:
				{
					// calling Unity_Function_i_s_fa
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					float [] arg3 = javaArgs[3].Call<float []> ("valueGet", new object[0]);;
					Unity_Function_i_s_fa(arg1, arg2, arg3, arg3.Length);
				}
				break;
			case 14:
				{
					// calling Unity_Function_i_s_fa_fa
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					float [] arg3 = javaArgs[3].Call<float []> ("valueGet", new object[0]);;
					float [] arg4 = javaArgs[4].Call<float []> ("valueGet", new object[0]);;
					Unity_Function_i_s_fa_fa(arg1, arg2, arg3, arg3.Length, arg4, arg4.Length);
				}
				break;
			case 15:
				{
					// calling Unity_Function_i_s_s_fa_b_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					string arg3 = javaArgs[3].Call<string> ("toString", new object[0]);;
					float [] arg4 = javaArgs[4].Call<float []> ("valueGet", new object[0]);;
					bool arg5 = javaArgs[5].Call<bool> ("booleanValue", new object[0]);;
					string arg6 = javaArgs[6].Call<string> ("toString", new object[0]);;
					Unity_Function_i_s_s_fa_b_s(arg1, arg2, arg3, arg4, arg4.Length, arg5, arg6);
				}
				break;
			case 16:
				{
					// calling Unity_Function_i_s_s_fa_ba_b_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					string arg3 = javaArgs[3].Call<string> ("toString", new object[0]);;
					float [] arg4 = javaArgs[4].Call<float []> ("valueGet", new object[0]);;
					byte [] arg5 = javaArgs[5].Call<byte []> ("valueGet", new object[0]);;
					bool arg6 = javaArgs[6].Call<bool> ("booleanValue", new object[0]);;
					string arg7 = javaArgs[7].Call<string> ("toString", new object[0]);;
					Unity_Function_i_s_s_fa_ba_b_s(arg1, arg2, arg3, arg4, arg4.Length, arg5, arg5.Length, arg6, arg7);
				}
				break;
			case 17:
				{
					// calling Unity_Function_i_fa_ia_ba_ia_fa
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					float [] arg2 = javaArgs[2].Call<float []> ("valueGet", new object[0]);;
					int [] arg3 = javaArgs[3].Call<int []> ("valueGet", new object[0]);;
					byte [] arg4 = javaArgs[4].Call<byte []> ("valueGet", new object[0]);;
					int [] arg5 = javaArgs[5].Call<int []> ("valueGet", new object[0]);;
					float [] arg6 = javaArgs[6].Call<float []> ("valueGet", new object[0]);;
					Unity_Function_i_fa_ia_ba_ia_fa(arg1, arg2, arg2.Length, arg3, arg3.Length, arg4, arg4.Length, arg5, arg5.Length, arg6, arg6.Length);
				}
				break;
			case 19:
				{
					// calling Unity_Function_i_f
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					float arg2 = javaArgs[2].Call<float> ("floatValue", new object[0]);;
					Unity_Function_i_f(arg1, arg2);
				}
				break;
			case 20:
				{
					// calling Unity_Function_i_s_i_fa
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					int arg3 = javaArgs[3].Call<int> ("intValue", new object[0]);;
					float [] arg4 = javaArgs[4].Call<float []> ("valueGet", new object[0]);;
					Unity_Function_i_s_i_fa(arg1, arg2, arg3, arg4, arg4.Length);
				}
				break;
			case 21:
				{
					// calling Unity_Function_i_f_f_f
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					float arg2 = javaArgs[2].Call<float> ("floatValue", new object[0]);;
					float arg3 = javaArgs[3].Call<float> ("floatValue", new object[0]);;
					float arg4 = javaArgs[4].Call<float> ("floatValue", new object[0]);;
					Unity_Function_i_f_f_f(arg1, arg2, arg3, arg4);
				}
				break;
			case 22:
				{
					// calling Unity_Function_i_i_i
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					int arg2 = javaArgs[2].Call<int> ("intValue", new object[0]);;
					int arg3 = javaArgs[3].Call<int> ("intValue", new object[0]);;
					Unity_Function_i_i_i(arg1, arg2, arg3);
				}
				break;
			case 23:
				{
					// calling Unity_Function_i
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					Unity_Function_i(arg1);
				}
				break;
			case 24:
				{
					// calling Unity_Function_i_b
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					bool arg2 = javaArgs[2].Call<bool> ("booleanValue", new object[0]);;
					Unity_Function_i_b(arg1, arg2);
				}
				break;
			case 25:
				{
					// calling Unity_Function_i_fa
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					float [] arg2 = javaArgs[2].Call<float []> ("valueGet", new object[0]);;
					Unity_Function_i_fa(arg1, arg2, arg2.Length);
				}
				break;
			case 26:
				{
					// calling Unity_Function_i_fa_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					float [] arg2 = javaArgs[2].Call<float []> ("valueGet", new object[0]);;
					string arg3 = javaArgs[3].Call<string> ("toString", new object[0]);;
					Unity_Function_i_fa_s(arg1, arg2, arg2.Length, arg3);
				}
				break;
			case 27:
				{
					// calling Unity_Function_i_s_ba
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					byte [] arg3 = javaArgs[3].Call<byte []> ("valueGet", new object[0]);;
					Unity_Function_i_s_ba(arg1, arg2, arg3, arg3.Length);
				}
				break;
			case 28:
				{
					// calling Unity_Function_i_i_i_fa
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					int arg2 = javaArgs[2].Call<int> ("intValue", new object[0]);;
					int arg3 = javaArgs[3].Call<int> ("intValue", new object[0]);;
					float [] arg4 = javaArgs[4].Call<float []> ("valueGet", new object[0]);;
					Unity_Function_i_i_i_fa(arg1, arg2, arg3, arg4, arg4.Length);
				}
				break;
			case 29:
				{
					// calling Unity_Function_i_i_s_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					int arg2 = javaArgs[2].Call<int> ("intValue", new object[0]);;
					string arg3 = javaArgs[3].Call<string> ("toString", new object[0]);;
					string arg4 = javaArgs[4].Call<string> ("toString", new object[0]);;
					Unity_Function_i_i_s_s(arg1, arg2, arg3, arg4);
				}
				break;
			case 30:
				{
					// calling Unity_Function_i_i_i_s_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					int arg2 = javaArgs[2].Call<int> ("intValue", new object[0]);;
					int arg3 = javaArgs[3].Call<int> ("intValue", new object[0]);;
					string arg4 = javaArgs[4].Call<string> ("toString", new object[0]);;
					string arg5 = javaArgs[5].Call<string> ("toString", new object[0]);;
					Unity_Function_i_i_i_s_s(arg1, arg2, arg3, arg4, arg5);
				}
				break;
			case 31:
				{
					// calling Unity_Function_i_s_i_i
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					int arg3 = javaArgs[3].Call<int> ("intValue", new object[0]);;
					int arg4 = javaArgs[4].Call<int> ("intValue", new object[0]);;
					Unity_Function_i_s_i_i(arg1, arg2, arg3, arg4);
				}
				break;
			case 32:
				{
					// calling Unity_Function_i_s_fa_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					float [] arg3 = javaArgs[3].Call<float []> ("valueGet", new object[0]);;
					string arg4 = javaArgs[4].Call<string> ("toString", new object[0]);;
					Unity_Function_i_s_fa_s(arg1, arg2, arg3, arg3.Length, arg4);
				}
				break;
			case 33:
				{
					// calling Unity_Function_i_s_fa_b_s
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					float [] arg3 = javaArgs[3].Call<float []> ("valueGet", new object[0]);;
					bool arg4 = javaArgs[4].Call<bool> ("booleanValue", new object[0]);;
					string arg5 = javaArgs[5].Call<string> ("toString", new object[0]);;
					Unity_Function_i_s_fa_b_s(arg1, arg2, arg3, arg3.Length, arg4, arg5);
				}
				break;
			case 34:
				{
					// calling Unity_Function_i_s_fa_ia
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					float [] arg3 = javaArgs[3].Call<float []> ("valueGet", new object[0]);;
					int [] arg4 = javaArgs[4].Call<int []> ("valueGet", new object[0]);;
					Unity_Function_i_s_fa_ia(arg1, arg2, arg3, arg3.Length, arg4, arg4.Length);
				}
				break;
			case 35:
				{
					// calling Unity_Function_i_s_fa_ia_ia
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					string arg2 = javaArgs[2].Call<string> ("toString", new object[0]);;
					float [] arg3 = javaArgs[3].Call<float []> ("valueGet", new object[0]);;
					int [] arg4 = javaArgs[4].Call<int []> ("valueGet", new object[0]);;
					int [] arg5 = javaArgs[5].Call<int []> ("valueGet", new object[0]);;
					Unity_Function_i_s_fa_ia_ia(arg1, arg2, arg3, arg3.Length, arg4, arg4.Length, arg5, arg5.Length);
				}
				break;
			case 36:
				{
					// calling Unity_Function_i_fa_ia_ia_ba_ia
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					float [] arg2 = javaArgs[2].Call<float []> ("valueGet", new object[0]);;
					int [] arg3 = javaArgs[3].Call<int []> ("valueGet", new object[0]);;
					int [] arg4 = javaArgs[4].Call<int []> ("valueGet", new object[0]);;
					byte [] arg5 = javaArgs[5].Call<byte []> ("valueGet", new object[0]);;
					int [] arg6 = javaArgs[6].Call<int []> ("valueGet", new object[0]);;
					Unity_Function_i_fa_ia_ia_ba_ia(arg1, arg2, arg2.Length, arg3, arg3.Length, arg4, arg4.Length, arg5, arg5.Length, arg6, arg6.Length);
				}
				break;
			case 37:
				{
					// calling Unity_Function_i_fa_ia_ia_ba_ia_fa
					int arg1 = javaArgs[1].Call<int> ("intValue", new object[0]);;
					float [] arg2 = javaArgs[2].Call<float []> ("valueGet", new object[0]);;
					int [] arg3 = javaArgs[3].Call<int []> ("valueGet", new object[0]);;
					int [] arg4 = javaArgs[4].Call<int []> ("valueGet", new object[0]);;
					byte [] arg5 = javaArgs[5].Call<byte []> ("valueGet", new object[0]);;
					int [] arg6 = javaArgs[6].Call<int []> ("valueGet", new object[0]);;
					float [] arg7 = javaArgs[7].Call<float []> ("valueGet", new object[0]);;
					Unity_Function_i_fa_ia_ia_ba_ia_fa(arg1, arg2, arg2.Length, arg3, arg3.Length, arg4, arg4.Length, arg5, arg5.Length, arg6, arg6.Length, arg7, arg7.Length);
				}
				break;
//END AndroidCallbacks
			case 18:
				// UnityObjectExists
				// TODO
				break;
			default:
				Debug.Log ("WARNING: DPAndroidCallback: methodName='" + methodName + "' not found, should be an integer");
				break;
			}
			return null;
		}
	}

	//BEGIN AndroidCalls
	public static void Call_i( string func_name, int arg1){
		dpUnityAndroidClass.CallStatic ("Call_i", new object[]{ func_name, arg1 } );
	}
	public static void Call_iba( string func_name, [In, Out] int [] arg1_1, int arg1_2){
		dpUnityAndroidClass.CallStatic ("Call_iba", new object[]{ func_name, arg1_1 } );
	}
	public static void Call_s( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1){
		dpUnityAndroidClass.CallStatic ("Call_s", new object[]{ func_name, arg1 } );
	}
	public static void Call_b_b( string func_name, bool arg1, bool arg2){
		dpUnityAndroidClass.CallStatic ("Call_b_b", new object[]{ func_name, arg1, arg2 } );
	}
	public static void Call_b_b_b( string func_name, bool arg1, bool arg2, bool arg3){
		dpUnityAndroidClass.CallStatic ("Call_b_b_b", new object[]{ func_name, arg1, arg2, arg3 } );
	}
	public static void Call_i_s( string func_name, int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2){
		dpUnityAndroidClass.CallStatic ("Call_i_s", new object[]{ func_name, arg1, arg2 } );
	}
	public static void Call_s_i( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2){
		dpUnityAndroidClass.CallStatic ("Call_s_i", new object[]{ func_name, arg1, arg2 } );
	}
	public static void Call_i_s_i( string func_name, int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, int arg3){
		dpUnityAndroidClass.CallStatic ("Call_i_s_i", new object[]{ func_name, arg1, arg2, arg3 } );
	}
	public static void Call_i_s_i_cD( string func_name, int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, int arg3, double [] arg4){
		dpUnityAndroidClass.CallStatic ("Call_i_s_i_cD", new object[]{ func_name, arg1, arg2, arg3, arg4 } );
	}
	public static void Call_i_i( string func_name, int arg1, int arg2){
		dpUnityAndroidClass.CallStatic ("Call_i_i", new object[]{ func_name, arg1, arg2 } );
	}
	public static void Call_i_i_i_b( string func_name, int arg1, int arg2, int arg3, bool arg4){
		dpUnityAndroidClass.CallStatic ("Call_i_i_i_b", new object[]{ func_name, arg1, arg2, arg3, arg4 } );
	}
	public static void Call_i_i_i_p3_b( string func_name, int arg1, int arg2, int arg3, float [] arg4, bool arg5){
		dpUnityAndroidClass.CallStatic ("Call_i_i_i_p3_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_i_i_i_p3_b_b_p3( string func_name, int arg1, int arg2, int arg3, float [] arg4, bool arg5, bool arg6, float [] arg7){
		dpUnityAndroidClass.CallStatic ("Call_i_i_i_p3_b_b_p3", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6, arg7 } );
	}
	public static void Call_b( string func_name, bool arg1){
		dpUnityAndroidClass.CallStatic ("Call_b", new object[]{ func_name, arg1 } );
	}
	public static void Call_i_fba( string func_name, int arg1, [In, Out] float [] arg2_1, int arg2_2){
		dpUnityAndroidClass.CallStatic ("Call_i_fba", new object[]{ func_name, arg1, arg2_1 } );
	}
	public static void Call_i_ba( string func_name, int arg1, [In, Out] byte [] arg2_1, int arg2_2){
		dpUnityAndroidClass.CallStatic ("Call_i_ba", new object[]{ func_name, arg1, arg2_1 } );
	}
	public static void Call_i_i_ba( string func_name, int arg1, int arg2, [In, Out] byte [] arg3_1, int arg3_2){
		dpUnityAndroidClass.CallStatic ("Call_i_i_ba", new object[]{ func_name, arg1, arg2, arg3_1 } );
	}
	public static void Call_i_i_ba_i_i( string func_name, int arg1, int arg2, [In, Out] byte [] arg3_1, int arg3_2, int arg4, int arg5){
		dpUnityAndroidClass.CallStatic ("Call_i_i_ba_i_i", new object[]{ func_name, arg1, arg2, arg3_1, arg4, arg5 } );
	}
	public static void Call_ba_i_p2( string func_name, [In, Out] byte [] arg1_1, int arg1_2, int arg2, float [] arg3){
		dpUnityAndroidClass.CallStatic ("Call_ba_i_p2", new object[]{ func_name, arg1_1, arg2, arg3 } );
	}
	public static void Call_i_iba_fba( string func_name, int arg1, [In, Out] int [] arg2_1, int arg2_2, [In, Out] float [] arg3_1, int arg3_2){
		dpUnityAndroidClass.CallStatic ("Call_i_iba_fba", new object[]{ func_name, arg1, arg2_1, arg3_1 } );
	}
	public static void Call_i_iba_fba_b_b_b( string func_name, int arg1, [In, Out] int [] arg2_1, int arg2_2, [In, Out] float [] arg3_1, int arg3_2, bool arg4, bool arg5, bool arg6){
		dpUnityAndroidClass.CallStatic ("Call_i_iba_fba_b_b_b", new object[]{ func_name, arg1, arg2_1, arg3_1, arg4, arg5, arg6 } );
	}
	public static void Call_i_iba_fba_fba_b_b_b( string func_name, int arg1, [In, Out] int [] arg2_1, int arg2_2, [In, Out] float [] arg3_1, int arg3_2, [In, Out] float [] arg4_1, int arg4_2, bool arg5, bool arg6, bool arg7){
		dpUnityAndroidClass.CallStatic ("Call_i_iba_fba_fba_b_b_b", new object[]{ func_name, arg1, arg2_1, arg3_1, arg4_1, arg5, arg6, arg7 } );
	}
	public static void Call_i_iba_fba_f( string func_name, int arg1, [In, Out] int [] arg2_1, int arg2_2, [In, Out] float [] arg3_1, int arg3_2, float arg4){
		dpUnityAndroidClass.CallStatic ("Call_i_iba_fba_f", new object[]{ func_name, arg1, arg2_1, arg3_1, arg4 } );
	}
	public static void Call_s_s_s_s_i( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, int arg5){
		dpUnityAndroidClass.CallStatic ("Call_s_s_s_s_i", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_s_s_s_s_b( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, bool arg5){
		dpUnityAndroidClass.CallStatic ("Call_s_s_s_s_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_s_s_s_s_f( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, float arg5){
		dpUnityAndroidClass.CallStatic ("Call_s_s_s_s_f", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_i_b( string func_name, int arg1, bool arg2){
		dpUnityAndroidClass.CallStatic ("Call_i_b", new object[]{ func_name, arg1, arg2 } );
	}
	public static void Call_i_b_b( string func_name, int arg1, bool arg2, bool arg3){
		dpUnityAndroidClass.CallStatic ("Call_i_b_b", new object[]{ func_name, arg1, arg2, arg3 } );
	}
	public static void Call_i_b_b_b_i( string func_name, int arg1, bool arg2, bool arg3, bool arg4, int arg5){
		dpUnityAndroidClass.CallStatic ("Call_i_b_b_b_i", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_i_b_b_b_i_b( string func_name, int arg1, bool arg2, bool arg3, bool arg4, int arg5, bool arg6){
		dpUnityAndroidClass.CallStatic ("Call_i_b_b_b_i_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6 } );
	}
	public static void Call_i_b_b_b_b_f( string func_name, int arg1, bool arg2, bool arg3, bool arg4, bool arg5, float arg6){
		dpUnityAndroidClass.CallStatic ("Call_i_b_b_b_b_f", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6 } );
	}
	public static void Call_s_i_i( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3){
		dpUnityAndroidClass.CallStatic ("Call_s_i_i", new object[]{ func_name, arg1, arg2, arg3 } );
	}
	public static void Call_s_s_b( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, bool arg3){
		dpUnityAndroidClass.CallStatic ("Call_s_s_b", new object[]{ func_name, arg1, arg2, arg3 } );
	}
	public static void Call_i_s_s( string func_name, int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3){
		dpUnityAndroidClass.CallStatic ("Call_i_s_s", new object[]{ func_name, arg1, arg2, arg3 } );
	}
	public static void Call_i_t3f_t3f( string func_name, int arg1, float [] arg2, float [] arg3){
		dpUnityAndroidClass.CallStatic ("Call_i_t3f_t3f", new object[]{ func_name, arg1, arg2, arg3 } );
	}
	public static void Call_i_s_i_s( string func_name, int arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, int arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4){
		dpUnityAndroidClass.CallStatic ("Call_i_s_i_s", new object[]{ func_name, arg1, arg2, arg3, arg4 } );
	}
	public static void Call_s_s_s( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3){
		dpUnityAndroidClass.CallStatic ("Call_s_s_s", new object[]{ func_name, arg1, arg2, arg3 } );
	}
	public static void Call_s_s_s_i( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4){
		dpUnityAndroidClass.CallStatic ("Call_s_s_s_i", new object[]{ func_name, arg1, arg2, arg3, arg4 } );
	}
	public static void Call_s_i_s_i( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4){
		dpUnityAndroidClass.CallStatic ("Call_s_i_s_i", new object[]{ func_name, arg1, arg2, arg3, arg4 } );
	}
	public static void Call_s_s_s_i_t3f( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4, float [] arg5){
		dpUnityAndroidClass.CallStatic ("Call_s_s_s_i_t3f", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_s_p3_s( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, float [] arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3){
		dpUnityAndroidClass.CallStatic ("Call_s_p3_s", new object[]{ func_name, arg1, arg2, arg3 } );
	}
	public static void Call_s_i_s_s( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4){
		dpUnityAndroidClass.CallStatic ("Call_s_i_s_s", new object[]{ func_name, arg1, arg2, arg3, arg4 } );
	}
	public static void Call_s_i_i_s_s( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5){
		dpUnityAndroidClass.CallStatic ("Call_s_i_i_s_s", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_s_i_p3_i_s_s( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, float [] arg3, int arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5, [MarshalAs(UnmanagedType.LPStr)] string arg6){
		dpUnityAndroidClass.CallStatic ("Call_s_i_p3_i_s_s", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6 } );
	}
	public static void Call_s_i_i_p3_i( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3, float [] arg4, int arg5){
		dpUnityAndroidClass.CallStatic ("Call_s_i_i_p3_i", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_s_i_i_p3_i_b( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3, float [] arg4, int arg5, bool arg6){
		dpUnityAndroidClass.CallStatic ("Call_s_i_i_p3_i_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6 } );
	}
	public static void Call_s_i_i_p3_i_s_s( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, int arg3, float [] arg4, int arg5, [MarshalAs(UnmanagedType.LPStr)] string arg6, [MarshalAs(UnmanagedType.LPStr)] string arg7){
		dpUnityAndroidClass.CallStatic ("Call_s_i_i_p3_i_s_s", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6, arg7 } );
	}
	public static void Call_s_s_s_i_b( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4, bool arg5){
		dpUnityAndroidClass.CallStatic ("Call_s_s_s_i_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_s_s_s_i_i( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4, int arg5){
		dpUnityAndroidClass.CallStatic ("Call_s_s_s_i_i", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_s_s_s_i_f( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4, float arg5){
		dpUnityAndroidClass.CallStatic ("Call_s_s_s_i_f", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_s_s_s_i_s( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, int arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5){
		dpUnityAndroidClass.CallStatic ("Call_s_s_s_i_s", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_s_s_s_s_s( string func_name, [MarshalAs(UnmanagedType.LPStr)] string arg1, [MarshalAs(UnmanagedType.LPStr)] string arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, [MarshalAs(UnmanagedType.LPStr)] string arg4, [MarshalAs(UnmanagedType.LPStr)] string arg5){
		dpUnityAndroidClass.CallStatic ("Call_s_s_s_s_s", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_p2_p2( string func_name, float [] arg1, float [] arg2){
		dpUnityAndroidClass.CallStatic ("Call_p2_p2", new object[]{ func_name, arg1, arg2 } );
	}
	public static void Call_p3_p3( string func_name, float [] arg1, float [] arg2){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3", new object[]{ func_name, arg1, arg2 } );
	}
	public static void Call_p3_p3_rI( string func_name, float [] arg1, float [] arg2, int [] arg3){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_rI", new object[]{ func_name, arg1, arg2, arg3 } );
	}
	public static void Call_p3_p3_t3f_i( string func_name, float [] arg1, float [] arg2, float [] arg3, int arg4){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_i", new object[]{ func_name, arg1, arg2, arg3, arg4 } );
	}
	public static void Call_p3_p3_t3f_rI( string func_name, float [] arg1, float [] arg2, float [] arg3, int [] arg4){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_rI", new object[]{ func_name, arg1, arg2, arg3, arg4 } );
	}
	public static void Call_p3_p3_t3f_rI_b( string func_name, float [] arg1, float [] arg2, float [] arg3, int [] arg4, bool arg5){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_rI_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5 } );
	}
	public static void Call_p3_p3_t3f_rI_b_b( string func_name, float [] arg1, float [] arg2, float [] arg3, int [] arg4, bool arg5, bool arg6){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_rI_b_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6 } );
	}
	public static void Call_p3_p3_t3f_rI_p2_b( string func_name, float [] arg1, float [] arg2, float [] arg3, int [] arg4, float [] arg5, bool arg6){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_rI_p2_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6 } );
	}
	public static void Call_p3_p3_t3f_t3f_rI_p2_b( string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, int [] arg5, float [] arg6, bool arg7){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_t3f_rI_p2_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6, arg7 } );
	}
	public static void Call_p3_p3_t3f_t3f_rI_p2_f_b( string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, int [] arg5, float [] arg6, float arg7, bool arg8){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_t3f_rI_p2_f_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 } );
	}
	public static void Call_p3_p3_t3f_t3f_rI_p2_f_b_b( string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, int [] arg5, float [] arg6, float arg7, bool arg8, bool arg9){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_t3f_rI_p2_f_b_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 } );
	}
	public static void Call_p3_p3_t3f_t3f_rI_p2_f_f_b_b( string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, int [] arg5, float [] arg6, float arg7, float arg8, bool arg9, bool arg10){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_t3f_rI_p2_f_f_b_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 } );
	}
	public static void Call_p3_p3_t3f_t3f_t3f_rI_p2_f_f_b_b( string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, float [] arg5, int [] arg6, float [] arg7, float arg8, float arg9, bool arg10, bool arg11){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_t3f_t3f_rI_p2_f_f_b_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 } );
	}
	public static void Call_p3_p3_t3f_t3f_t3f_p2_f_f( string func_name, float [] arg1, float [] arg2, float [] arg3, float [] arg4, float [] arg5, float [] arg6, float arg7, float arg8){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_t3f_t3f_p2_f_f", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 } );
	}
	public static void Call_p3_p3_t3f_rI_p2_b_b( string func_name, float [] arg1, float [] arg2, float [] arg3, int [] arg4, float [] arg5, bool arg6, bool arg7){
		dpUnityAndroidClass.CallStatic ("Call_p3_p3_t3f_rI_p2_b_b", new object[]{ func_name, arg1, arg2, arg3, arg4, arg5, arg6, arg7 } );
	}
//END AndroidCalls
#endif

	public static void Call_iba_wrapper(string func_name, int[] arg1_1)
	{
		Call_iba(func_name, arg1_1, arg1_1.Length * 4);
	}

	public static List<int> Call_rsa_wrapper_il(string func_name){
		String[] retstrarr = Call_rsa_wrapper(func_name);
		//String[] retstrarr = Call_rsa_wrapper("Get-All-UnityObject-IDs-map");
		List<int> allIDs = new List<int>();
		for (int i = 0; i < retstrarr.Length; i++) {
			int val;
			if (int.TryParse (retstrarr [i], out val)) {
				allIDs.Add (val);
			}
		}
		return allIDs;
	}

	public static void ClearDP()
	{
		DestroyDP();
		isInitialized = false;
		DPisInitialized = false;
		VMObject[] vmos = FindObjectsOfType<VMObject>();
		foreach (VMObject vmo in vmos)
		{
			vmo.ResetDP();
		}
	}
	static public bool DP_IsUp(){
		return IsUp() != 0;
	}
	static public void DPCall_line(string str){
		Call_line(str);
	}
#if UNITYRENDER
	[ DllImport (dll) ]
	private static extern IntPtr GetRenderEventFunc();
	[ DllImport (dll) ]
	private static extern void SetTimeFromUnity(float t);
#endif

#if DEBUG_DP
	static bool log_errors = true, log_logs = true, log_unity = true;
#else
	static bool log_errors = true, log_logs = false, log_unity = true;
#endif

	static Vector3 normal2dTo3dVector(Vector2 normalpt, float dimsize, float yval){
		float dimsize2 = dimsize / 2.0f;
		return new Vector3 ((normalpt.x * dimsize) - dimsize2, yval, (normalpt.y * dimsize) - dimsize2);
	}
	static void SetLocalPositionOrientation(Vector2 normalpos, Vector2 normalview, bool yFromCur, float yvalarg){
		AnimateCameraImpl acs = FindObjectOfType<AnimateCameraImpl> ();
		if (acs!=null){
			Transform trans = acs.getControlledTransform ();
			float yval = trans.position.y;
			if (!yFromCur)
				yval = yvalarg;
            Vector3 pos = new Vector3 (normalpos.x, yval, normalpos.y);
            Vector3 curCamForward = trans.forward;
            Vector3 lookAtVector = pos + new Vector3(normalview.x, curCamForward.y, normalview.y);
            acs.setExecuteCommandAfterAnimation ("Request-Placement-With-Direct-View-On-For-Pointing-map");
			acs.animateCameraTo(pos, true, lookAtVector, acs.isControllingWorld()) ;
		}
	}
	static void GetViewPositionOrientationAndSendTo(int fromComputerID, int computerID, Vector2 normalpos, Vector2 normalview, bool yFromCur, float yvalarg){
		AnimateCameraImpl acs = FindObjectOfType<AnimateCameraImpl> ();
		if (acs != null) {
			Transform trans = acs.getControlledTransform ();
			float yval = trans.position.y;
			if (!yFromCur)
				yval = yvalarg;
			Vector3 pos = new Vector3 (normalpos.x, yval, normalpos.y);
			Vector3 lookAtVector = pos + new Vector3 (normalview.x, 0.0f, normalview.y);
			Camera curCamera = ViewManager.getCurrentCamera ();
			Vector3 curPos = curCamera.transform.position;
			Quaternion curQuat = curCamera.transform.rotation;
			curCamera.transform.position = pos;
			curCamera.transform.LookAt(lookAtVector);

			Call_noargs ("Save-UnityObject-Layouts-Clear-And-Compute-All-map");
			Call_b_b_b ("Traverse-BSP-Object-Tree-And-Try-To-Place-All-Annotations-map", false, false, true);
			Call_noargs ("Restore-UnityObject-Layouts-map");

			int prevCullingMask = curCamera.cullingMask;
			curCamera.cullingMask &= (int)(0xffffffff ^ (1 << LayerMask.NameToLayer ("UI") | 1 << LayerMask.NameToLayer("OnlyScreenSpace")));

			int twidth = curCamera.pixelWidth, theight = curCamera.pixelHeight;
			RenderTexture renderTexture = new RenderTexture (twidth, theight, 24);
			curCamera.targetTexture = renderTexture;
			curCamera.Render ();
			RenderTexture.active = renderTexture;
			Texture2D virtualPhoto = new Texture2D (twidth, theight, TextureFormat.RGB24, false);
			virtualPhoto.ReadPixels (new Rect (0, 0, twidth, theight), 0, 0);
			curCamera.targetTexture = null;

			curCamera.cullingMask = prevCullingMask;

			/*
			Matrix4x4 worldToDim = ViewManager.getTopToWorldMatrix();
			Vector3 curCamPosInWorld = ViewManager.getWorldToTopMatrix().MultiplyPoint(curCamera.transform.position);
			Matrix4x4 camdiff = curCamera.transform.worldToLocalMatrix * curCamera.worldToCameraMatrix.inverse;
			Matrix4x4 worldToDirectView = worldToDim * Matrix4x4.TRS(curCamPosInWorld, curCamera.transform.rotation, curCamera.transform.localScale);
			ViewManager.setViewToCurrentCamera("Set-Camera-Up-And-Right-map", "Set-View-Info-map", false, curCamera.projectionMatrix, 
				camdiff, worldToDirectView, curCamera.pixelWidth, curCamera.pixelHeight, curCamera.nearClipPlane, curCamera.fieldOfView);
			*/
			// COMPUTE LABEL PLACEMENTS FOR PERSPECTIVE

			byte[] bytes = virtualPhoto.EncodeToPNG ();

			Call_i_i_ba_i_i("Send-View-Image-Back-To-map", fromComputerID, computerID, bytes, bytes.Length, twidth, theight);

			curCamera.transform.position = curPos;
			curCamera.transform.rotation = curQuat;
		}
		//acs.setExecuteCommandAfterAnimation ("Request-Placement-With-Direct-View-On-For-Pointing-map");
		//acs.animateCameraTo(pos, true, lookAtVector, acs.isControllingWorld()) ;
	}
	static void GenerateScreenRectanglesFromFloatCoords(string str, float [] coords, int coords_len, Color32 col, bool debug){
		GameObject go = GameObject.Find(str);
/*		if (oldgo != null) {
			Destroy (oldgo);
		} else {*/
		if (go == null) {
			go = new GameObject (str);
#if !UNITY_IOS
			int layer = DPUtils.addLayerIfNot ("VMDebug");
			go.layer = layer;
#endif
//			go.tag = "VMDebug";
			go.AddComponent<MeshFilter> ();
			go.AddComponent<MeshRenderer> ();
			ViewManager vm = FindObjectOfType<ViewManager>();
			if (vm.screenCanvas!=null)
				go.transform.SetParent (vm.screenCanvas.transform);
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
		}
		Mesh mesh = go.GetComponent<MeshFilter>().mesh;
		MeshRenderer mr = go.GetComponent<MeshRenderer> ();
		Material material = mr.material;
		mesh.Clear ();

		Camera curCamera = ViewManager.getCurrentCamera ();
		float width = curCamera.pixelWidth;
		float height = curCamera.pixelHeight;
        float hwidth = width / 2.0f, hheight = height / 2.0f;
		int nverts = coords_len / 3;
		int nidx = 6 * (nverts / 4);
		Vector3 [] list = new Vector3[nverts];
		Color32[] colors = new Color32[nverts];

		if (debug) {
			Debug.Log ("Generating nverts=" + nverts);
		}
		int [] tris = new int[ nidx ];
		for (int i=0, i3 = 0; i3 < coords_len; i++, i3 += 3) {
            //list[i] = new Vector3( (coords[i3]), (coords[i3 + 1]), 0.0f);
            // works for Screen Space Camera
            list[i] = new Vector3(coords[i3] - hwidth, coords[i3 + 1] - hheight, 0.0f);
            /*			byte r = (byte)(((i % 3) == 0) ? 255 : 0);
                        byte g = (byte)(((i % 3) == 1) ? 255 : 0);
                        byte b = (byte)(((i % 3) == 2) ? 255 : 0);
                        colors [i] = new Color32 (  r, g, b, 64);
                        */
            colors [i] = col; //new Color32 (  255, 255, 255, 64);
		}
		for (int i = 0, ri = 0 ; i < nidx; i += 6, ri += 4) {
			tris [i] = ri;
			tris [i+1] = ri+1;
			tris [i+2] = ri+2;
			tris [i+3] = ri;
			tris [i+4] = ri+2;
			tris [i+5] = ri+3;
		}

		mesh.vertices = list;
		mesh.triangles = tris;
		mesh.colors32 = colors;
        material.SetFloat("_Mode", 3.0f); // Transparent
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.EnableKeyword("_EMISSION");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHATEST_ON");
        material.SetColor("_Color", col);
        material.SetColor("_EmissionColor", col); // Color.white);
        material.renderQueue = 3000;
		/*
		int layerID = 0;
		if ((layerID = LayerMask.NameToLayer ("VMScreen")) >= 0){
			go.layer = layerID;
		}*/
	}

	static public void GenerateObjectFromFloatCoords(string str, float [] coords, int coords_len, Color32 col, bool backfaces){
		GameObject oldgo = GameObject.Find(str);
		if (oldgo != null) {
			Destroy (oldgo);
		}
		GameObject go = new GameObject (str);
		ViewManager vm = FindObjectOfType<ViewManager> ();
		if (vm.topWorldObject != null) {
			go.transform.SetParent (vm.topWorldObject.transform);
		}
#if !UNITY_IOS
		int layer = DPUtils.addLayerIfNot ("VMDebug");
		go.layer = layer;
#endif
		//go.tag = "VMDebug";
		go.AddComponent<MeshFilter>();
		go.AddComponent<MeshRenderer>();
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;

		MeshFilter mf = go.GetComponent<MeshFilter> ();
		Mesh mesh = mf.mesh;
		MeshRenderer mr = go.GetComponent<MeshRenderer> ();
		//Material material = mr.material;
		mesh.Clear ();
		int mul = 1;
		if (backfaces)
			mul = 2;
		int coords_len3 = coords_len / 3;
		Vector3 [] list = new Vector3[coords_len3];
		int [] tris = new int[ mul * coords_len3 ];
		//Color32[] colors = new Color32[coords_len3];
		for (int i = 0; i < coords_len3; i++){
			int i3 = i * 3;
			list[i] = new Vector3 (coords [i3], coords [i3 + 1], coords [i3 + 2]);
			//logstr += " :: " + coords [i3] + "," + coords [i3 + 1] + "," + coords [i3 + 2] + Environment.NewLine;
			tris [i] = i;
			if (backfaces) {
				tris [2*coords_len3 - i - 1] = i;
			}
			//      colors[i] = col;
		}
		mesh.vertices = list;
		mesh.triangles = tris;
		// mesh.colors32 = colors;
		Vector3 [] mn = new Vector3[coords_len3];
		// TODO: NEED TO GENERATE NORMALS
		for (int i = 0; i < coords_len3; i+=3) {
			Vector3 v1 = list [i + 1] - list [i];
			Vector3 v2 = list [i + 2] - list [i];
			Vector3 result = new Vector3 (v1.y * v2.z - v1.z * v2.y,
				                 v1.z * v2.x - v1.x * v2.z,
				                 v1.x * v2.y - v1.y * v2.x);
			result.Normalize ();
			mn [i] = result; mn [i+1] = result; mn [i+2] = result;
		}
		mesh.normals = mn;
		mr.material.color = col;
		mr.sortingLayerName = "Transparency";
		mr.material.SetFloat("_Mode", 2.0f);
		mr.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		mr.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		mr.material.SetInt("_ZWrite", 0);
		mr.material.DisableKeyword("_ALPHATEST_ON");
		mr.material.EnableKeyword("_ALPHABLEND_ON");
		mr.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		mr.material.EnableKeyword("_EMISSION");
		mr.material.SetColor("_Color", col);
		mr.material.SetColor("_EmissionColor", col);
		mr.material.renderQueue = 3000;
	}

	static public void GenerateLineObjectFromFloatCoordsMulti(string str, float [] coords, int coords_len, Color32 col, float lineWidth){

		bool cont = true;
		int cnt = 1, pl = 0;

		while (cont) {
			GameObject go = GameObject.Find (str + "-" + cnt);
			if (go == null) {
				go = new GameObject (str + "-" + cnt);
				ViewManager vm = FindObjectOfType<ViewManager> ();
				if (vm.topWorldObject != null) {
					go.transform.SetParent (vm.topWorldObject.transform);
				}
				go.AddComponent<LineRenderer> ();
				go.transform.localRotation = Quaternion.identity;
				go.transform.localScale = Vector3.one;
				go.transform.position = Vector3.zero;
			}
			LineRenderer lr = go.GetComponent<LineRenderer> ();

			List<Vector3> coordList = new List<Vector3> ();
			bool cont2 = true;
			Vector3 lastCoord;
			int pl3 = pl * 3;
			coordList.Add (lastCoord = new Vector3 (coords [pl3], coords [pl3 + 1], coords [pl3 + 2]));
			pl++; pl3 += 3;
			do {
				Vector3 newPt = new Vector3 (coords [pl3], coords [pl3 + 1], coords [pl3 + 2]);
				coordList.Add (newPt);
				cont2 = !lastCoord.Equals(newPt);
				pl++; pl3 += 3;
				lastCoord = newPt;
			} while (cont2);
			Vector3 [] coordArr = coordList.ToArray();
			lr.positionCount = coordArr.Length;
			lr.SetPositions (coordArr);
			lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
			lr.material.color = col;
			lr.useWorldSpace = true;
            lr.startWidth = lr.endWidth = lineWidth;
			cont = pl3 < coords.Length;
			cnt++;
		}
		cont = true;
		GameObject go2 = GameObject.Find (str + "-" + cnt);
		while(go2!=null){
			Destroy(go2);
			cnt++;
			go2 = GameObject.Find (str + "-" + cnt);
		}
	}

	static public void GenerateLineObjectFromFloatCoords(string str, float [] coords, int coords_len, Color32 col){
		GameObject go = GameObject.Find(str);
		if (go == null) {
			go = new GameObject (str);
			ViewManager vm = FindObjectOfType<ViewManager> ();
			if (vm.topWorldObject != null) {
				go.transform.SetParent (vm.topWorldObject.transform);
			}
			go.AddComponent<MeshFilter>();
			go.AddComponent<LineRenderer>();
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			go.transform.position = Vector3.zero;
		}
		LineRenderer lr = go.GetComponent<LineRenderer> ();
		int coords_len3 = coords_len / 3;
		Vector3 [] list = new Vector3[coords_len3];
		int coords_len_3 = coords_len / 3;
		for (int i = 0; i < coords_len_3; i++){
			int i3 = i * 3;
			list[i] = new Vector3 (coords [i3], coords [i3 + 1], coords [i3 + 2]);
		}
        //		Debug.Log ("logstr=" + logstr);
        lr.positionCount = list.Length;
		lr.SetPositions (list);
		lr.material.color = col;
		lr.useWorldSpace = true;
        lr.startWidth = lr.endWidth = 0.01f;
//		lr.material.shader = Shader.Find ("Lines");
		//lr.sortingLayerName = "Transparency";
		//mr.material.SetFloat("_Mode", 3);
		//mr.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		//mr.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		//mr.material.SetInt("_ZWrite", 0);
		//mr.material.DisableKeyword("_ALPHATEST_ON");
		//mr.material.EnableKeyword("_ALPHABLEND_ON");
		//mr.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		//mr.material.renderQueue = 3000;
	}
//	static int lastFrameCount = 0;

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_fa_s))]
	static void Unity_Function_i_s_fa_s(int op, string str, IntPtr coordsptr, int coords_len, string afterCmdIf){
		float [] coords = new float[coords_len];
		Marshal.Copy (coordsptr, coords, 0, coords_len);
#else
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_s_fa_s(int op, string str, float [] coords, int coords_len, string afterCmdIf){
#endif
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_fa_b_s))]
	static void Unity_Function_i_s_fa_b_s(int op, string str, IntPtr coordsptr, int coords_len, bool b1, string afterCmdIf)
	{
		float [] coords = new float[coords_len];
		Marshal.Copy (coordsptr, coords, 0, coords_len);
#else
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_s_fa_b_s(int op, string str, float [] coords, int coords_len, bool b1, string afterCmdIf)
	{
#endif
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_s_fa_b_s))]
	static void Unity_Function_i_s_s_fa_b_s(int op, string prevstr, string str, IntPtr coordsptr, int coords_len, bool b1, string afterCmdIf)
	{
		float [] coords = new float[coords_len];
		Marshal.Copy (coordsptr, coords, 0, coords_len);
#else
	static void Unity_Function_i_s_s_fa_b_s(int op, string prevstr, string str, float [] coords, int coords_len, bool b1, string afterCmdIf)
	{
#endif
		switch (op) {
		case 1:
			//GotoViewOfObjectWithVector-map // GotoViewOfObjectWithVector-Impl-map
			{
				GameObject go = GameObject.Find (str);
				GameObject prevgo = GameObject.Find (prevstr);
                Vector3 firstPoint = new Vector3();
                bool firstPointIsSet = false;

                if (prevgo == null){
                   string[] allAdjacent = DPManagerScript.Call_rsa_wrapper("Get-All-Adjacent-To-Outside-UnityObjects-map");
                   string sout = "all adjacent: \n";
                   foreach (string adjname in allAdjacent){
                            sout = sout + "adjname: " + adjname + "\n";
                   }
                   Debug.Log(sout);
                   if (allAdjacent.Length > 0) {
                       prevgo = GameObject.Find(allAdjacent[0]);
                       Renderer rend = prevgo.GetComponent<Renderer>();
                       DPUtils.getClosestVMObjectPoint(rend.bounds, out firstPoint, out firstPointIsSet);
                   }
                }
				bool found = false;
				if (go != null && prevgo != null) {
					VMObject vmo = go.GetComponent<VMObject> ();
					VMObject prevvmo = prevgo.GetComponent<VMObject> ();
					if (prevvmo == null && !firstPointIsSet) {
						// if outside bounds, then find closest entrance
						ScreenButtonsImpl sbi = FindObjectOfType<ScreenButtonsImpl> ();
						prevvmo = sbi.getClosestVMObject (out firstPoint, out firstPointIsSet);
						if (prevvmo == vmo)
							prevvmo = null;
					}

					if (vmo != null && prevvmo != null) {
    					float[] firstPointInPath = null;
                        if (firstPointIsSet){
                            firstPointInPath = new float[] { firstPoint.x, firstPoint.y, firstPoint.z };
                        }
                        if (firstPointInPath != null) {
							found = DPManagerScript.Call_rb_s_i_i_p3 ("Find-Best-Path-And-Animate-With-First-Point-map", str, prevvmo.GetVMInstanceID (), vmo.GetVMInstanceID (), firstPointInPath) != 0;
						} else {
							found = DPManagerScript.Call_rb_s_i_i ("Find-Best-Path-And-Animate-map", str, prevvmo.GetVMInstanceID (), vmo.GetVMInstanceID ()) != 0;
						}
					}
				}
				if (!found) {
					Renderer rend = go.GetComponent<Renderer> ();
					Camera curCamera = ViewManager.getCurrentCamera ();
					Vector3 vect = DPUtils.GetVector3If (new Vector3 (coords [0], coords [1], coords [2]), curCamera.transform.forward);
					AnimateCameraImpl acs = FindObjectOfType<AnimateCameraImpl> ();
					bool insideBounds = b1;
					if (DPUtils.GotoViewWithViewVector (rend.bounds, acs, vect, insideBounds) && afterCmdIf != null && afterCmdIf.Length > 0) {
						if (acs != null)
							acs.setShouldExecuteViewManagementAfterAnimation (true);
						DPManagerScript.Call_noargs (afterCmdIf);
					}
				}
			}
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_s_fa_ba_b_s))]
	static void Unity_Function_i_s_s_fa_ba_b_s(int op, string prevstr, string str, IntPtr coordsptr, int coords_len,
                                               IntPtr bool_arrptr, int bool_arr_len, bool b1, string afterCmdIf)
	{
		float [] coords = new float[coords_len];
		Marshal.Copy (coordsptr, coords, 0, coords_len);
		byte [] bool_arr = new byte[bool_arr_len];
		Marshal.Copy (bool_arrptr, bool_arr, 0, bool_arr_len);
#else
	static void Unity_Function_i_s_s_fa_ba_b_s(int op, string prevstr, string str, float [] coords, int coords_len,
											   byte [] bool_arr, int bool_arr_len, bool b1, string afterCmdIf)
	{
#endif
		switch (op) {
		case 1:
			{ // Animate-Camera-On-Points-map
				bool[] lookatnext = new bool[bool_arr_len];
				for (int i = 0; i < bool_arr_len; i++) {
                    lookatnext[i] = Convert.ToBoolean (bool_arr [i]);
				}
                GameObject finalgo = GameObject.Find (str);
				Renderer rend = finalgo.GetComponent<Renderer> ();
				Bounds bounds = rend.bounds;
				MouseOverImpl mos = finalgo.GetComponent<MouseOverImpl> ();
				AnimateCameraImpl acs = FindObjectOfType<AnimateCameraImpl> ();
				int allPointsLen = coords_len / 3;
				Vector3[] allPoints = new Vector3[allPointsLen];
				Vector3[] allViewVectors = new Vector3[allPointsLen];
				int pt = 0;
				allPoints [pt] = new Vector3 (coords [0], coords [1], coords [2]);
				pt++;

				for (int i = 3; i < coords_len; i += 3) {
					allPoints [pt] = new Vector3 (coords [i], coords [i + 1], coords [i + 2]);
					allViewVectors [pt - 1] = (allPoints [pt] - allPoints [pt - 1]);
                    pt++;
				}
				pt--;

                allViewVectors [pt] = bounds.center - allPoints [pt - 1];
                // now set the last point and vector relative to MouseOverScript
                // insideBounds
                // use the 2nd to last link, since the last is from the connector to center
                //Vector3 vvnorm = (allPoints [pt-1] - allPoints [pt - 2]).normalized;
                    Vector3 vvnorm = allViewVectors[pt].normalized;
				Vector3 outViewForward;
				mos.getClosestViewNormalCheck(vvnorm, out outViewForward);
				Vector3 bext = bounds.extents;
				float maxdim = bext.x;
				if (maxdim < bext.y)
					maxdim = bext.y;
				if (maxdim < bext.z)
					maxdim = bext.z;
                // clip inside bbx (scaled a bit smaller
				Vector3 clipped = DPUtils.ClipLineToUnitBox (-outViewForward * maxdim * 3.0f,
                                                             Vector3.zero,
                                                             bounds.size / 2.0f);
				allPoints[pt] = .8f * clipped + bounds.center;
				allViewVectors [pt] = outViewForward;

				for (int i = 2; i < allViewVectors.Length; i++) {
					float ang = Vector3.Angle(allViewVectors [i-1], allViewVectors [i]);
//					Debug.Log ("#" + i + " ang=" + ang);
					if (ang > 90.0f) {
						allViewVectors [i-1] = ((allViewVectors [i] + allViewVectors [i-2])/2.0f).normalized;
						allViewVectors [i-1].y = 0.0f;
						allViewVectors [i-1].Normalize ();
					}
				}
                    /*				if (allViewVectors.Length > 2) {
                                        // account for when the last 2 vectors are very far apart, remove 2nd to last and average the 3rd to last with the last
                                        // to smooth out the transition
                                        int lastidx = allViewVectors.Length - 1;
                                        float ang = Vector3.Angle(allViewVectors [lastidx-1], allViewVectors [lastidx]);
                                        if (ang > 90.0f) {
                                            if (lastidx >= 2) {
                                                allViewVectors [lastidx - 1] = ((allViewVectors [lastidx] + allViewVectors [lastidx - 2])/2.0f).normalized;// + allViewVectors [allViewVectors.Length - 2]) / 2.0f;
                                            } else {
                                                allViewVectors [lastidx - 1] = allViewVectors [lastidx];
                                            }
                                            allViewVectors [lastidx-1].y = 0.0f;
                                            allViewVectors [lastidx-1].Normalize ();
                                        }
                                    }*/

                    // DEBUGGING PATH
                    /*for (int i=0; i<allPoints.Length; i++){
                            string ptname = "Path Point " + i;
                            string ptstr = "(" + allPoints[i].x + "," + allPoints[i].y + "," + allPoints[i].z + ")";
                            DPManagerScript.Call_line("Create-Unity-Cube-map\t" + ptname + "\t(1,0,0)\t" + ptstr + "\t(.05,.05,.05)");
                    }*/
                    acs.animateCameraAcrossPath (allPoints, allViewVectors, lookatnext);

				if (afterCmdIf != null && afterCmdIf.Length > 0) {
					acs.setShouldExecuteViewManagementAfterAnimation (true);
					DPManagerScript.Call_noargs (afterCmdIf);
				}
			}
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_fa))]
	static void Unity_Function_i_fa(int op, IntPtr farr, int farray_len)
	{
		float [] farray = new float[farray_len];
		Marshal.Copy(farr, farray, 0, farray_len);
#else
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_fa(int op, float [] farray, int farray_len)
	{
#endif
		switch (op){
			case 1:
			//Set-Local-Position-Orientation-map
						{
							Vector2 position = new Vector2(farray[0], farray[1]);
							Vector2 viewvector = new Vector2(farray[2], farray[3]);
							bool yFromCur = true;
							float yVal = 0.0f;
							if (farray_len > 4){
								int vmdid = (int)farray[4];
								VMDimension vmd;
								if (VMDimension.m_VMDimensions.TryGetValue(vmdid, out vmd))
                                {
									if (vmd.isTopForNavigation)
                                    {
										yFromCur = false;
										yVal = vmd.topY;
									}
                                    if (vmd.target!=null)
                                    {
                                        InterpolateTRSScript iscript = vmd.target;
                                        if (vmd.targetScaleIsSet)
                                        {
                                            iscript.AnimateScaleTo(vmd.targetScale);
                                        }
                                        // TODO: SET yFromCur to y from bbx
                                        if (!vmd.isTopForNavigation && farray_len > 5)
                                        {
                                            yVal = farray[5];
                                            yFromCur = false;
                                        }
                                    }
                                }
							}
							SetLocalPositionOrientation(position, viewvector, yFromCur, yVal);
						}
			break;
			case 2: // Animate-To-Centroid-map
                {
					Vector3 targetpt = new Vector3(farray[0],farray[1],farray[2]);
					AnimateCameraImpl acs = FindObjectOfType<AnimateCameraImpl> ();
					acs.animateCameraToPositionWithDuration (targetpt, 2.0f);//acs.getDefaultAnimationTime ());
					acs.setShouldExecuteViewManagementAfterAnimation (true);
					DPManagerScript.Call_s_s_s_s_b ("SetDataValue-map", "UnityMain", "do-not-change-view", "uid", "main-uid", true);
				}
			break;
		}

	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_fa_s))]
	static void Unity_Function_i_fa_s(int op, IntPtr farr, int farray_len, string str){
		float[] farray = new float[farray_len];
		Marshal.Copy (farr, farray, 0, farray_len);
#else
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_fa_s(int op, float [] farray, int farray_len, string str)
	{
#endif
		switch (op) {
		case 1:
			break;
		}
	}

#if UNITY_IOS_ONLY
[MonoPInvokeCallback(typeof(MyDelegate_i_b))]
#endif
static void Unity_Function_i_b(int op, bool bval)
{
	switch (op) {
	case 1:
		{ //Set-Report-Local-Position-Orientation-map
			ViewManager vm = FindObjectOfType<ViewManager>();
			vm.reportPositionOrientation = bval;
			vm.prevViewMatrixIsSet = false;
		}
		break;
	case 2:
		{ // Update-Report-Rendering-map
			ViewManager vm = FindObjectOfType<ViewManager>();
			vm.reportRendering = bval;
		}
		break;
		case 3:
		{ // Turn-All-Objects-For-Zooming-In-map
			foreach (VMDimension vmd in VMDimension.m_VMDimensions.Values) {
				foreach (GameObject go in vmd.allTurnOnWhenZoomedInto) {
					go.SetActive (bval);
				}
			}
		}
		break;
	}
}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_f))]
#endif
	static void Unity_Function_i_f(int op, float fval)
	{
		switch (op) {
		case 1:
			//
			{ //SetViewManager-DelayBetweenComputationFrames-map
				FindObjectOfType<ViewManager>().delayBetweenComputationFrames = fval;
			}
			break;
		}
	}
#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_f_f_f))]
#endif
	static void Unity_Function_i_f_f_f(int op, float fval1, float fval2, float fval3)
	{
		switch (op) {
		case 1:
			{
				Camera curCamera = ViewManager.getCurrentCamera ();
				curCamera.clearFlags = CameraClearFlags.Color;
				curCamera.backgroundColor = new Color (fval1, fval2, fval3);
			}
			break;
		}
	}
	static void DrawBorderPixel(Texture2D tex, int x, int y, Color32 insideCol, Color32 edgeCol, int [] rects, int rects_len){
		bool isInside = false;
		for (int i = 0; i < rects_len; i += 4) {
			if (x > rects [i] && x < rects [i + 2] && y > rects [i + 1] && y < rects [i + 3]) {
				isInside = true;
				break;
			}
		}
		if (isInside) {
			tex.SetPixel (x, y, insideCol);
		} else {
			tex.SetPixel (x, y, edgeCol);
		}
	}

	static void SaveRectanglesToFile(string fileName, float [] coords, int coords_len, Color col){
		Camera curCamera = ViewManager.getCurrentCamera ();
		int width = curCamera.pixelWidth;
		int height = curCamera.pixelHeight;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
/*		Color colBorder = new Color (); // Color.white
		colBorder.r = col.r;
		colBorder.g = col.g;
		colBorder.b = col.b;
		colBorder.a = 1.0f;*/
		Color colBorder = Color.white;
		Color32 almostclear = new Color32 (0, 0, 0, 1);  // TODO : this is work around of a bug in Unity PNG Encoding/Decoding
		for(int i=0; i<width; i++){
			for(int j=0;j<height; j++){
				tex.SetPixel(i, j, almostclear);
			}
		}
		int nrects = (coords_len / 12);
		int rects_len = nrects * 4;
		int[] rects = new int[rects_len];
		int rectoff = 0;
		for (int offstart = 0; offstart < coords_len; offstart += 12) {
			int roff = offstart + 12;
			int minx = int.MaxValue, miny = int.MaxValue, maxx = int.MinValue, maxy = int.MinValue;
			for (int off = offstart; off < roff; off += 3) {
				minx = (int)Math.Min (coords [off], minx);
				miny = (int)Math.Min (coords [off + 1], miny);
				maxx = (int)Math.Max (coords [off], maxx);
				maxy = (int)Math.Max (coords [off + 1], maxy);
			}
			rects [rectoff++] = minx;
			rects [rectoff++] = miny;
			rects [rectoff++] = maxx;
			rects [rectoff++] = maxy;
		}

		for (rectoff = 0; rectoff < rects_len; rectoff+=4){
			int minx = rects [rectoff], miny = rects [rectoff + 1], maxx = rects [rectoff + 2], maxy = rects [rectoff + 3];

			for (int y = miny+1; y < maxy; y++) {
				for (int x = minx+1; x < maxx; x++) {
					//Color color = tex.GetPixel (x, y);
					tex.SetPixel (x, y, col);
//					tex.SetPixel (x, y, DPUtils.BlendColors(color, col));
				}
			}

			// draw border separately since need to check if edge
			for (int x = minx; x <= maxx; x++) {
				// if y = miny, bottom edge
				DrawBorderPixel(tex, x, miny, col, colBorder, rects, rects_len);
				// if y = maxy, top edge
				DrawBorderPixel(tex, x, maxy, col, colBorder, rects, rects_len);
			}

			for (int y = miny + 1; y < maxy; y++) {
				// if x = minx, left edge (minus corners)
				DrawBorderPixel(tex, minx, y, col, colBorder, rects, rects_len);
				// if x = maxx, right edge (minus corners)
				DrawBorderPixel(tex, maxx, y, col, colBorder, rects, rects_len);
			}
		}
		tex.Apply();
		byte[] bytes = tex.EncodeToPNG ();
		File.WriteAllBytes (fileName, bytes);
	}
#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_fa))]
	static void Unity_Function_i_s_fa(int op, string str, IntPtr coordsptr, int coords_len){
		float [] coords = new float[coords_len];
		Marshal.Copy (coordsptr, coords, 0, coords_len);
#else
	static void Unity_Function_i_s_fa(int op, string str, float [] coords, int coords_len)
		{
#endif
		//		Debug.Log ("Unity_Function_i_s_fa: op=" + op + " name=" + str + " coords_len=" + coords_len + " len=" + coords_len);
		switch (op){
		case 1:
			GenerateObjectFromFloatCoords (str, coords, coords_len, new Color32 (  64, 64, 128, 128), true );
			break;
		case 2:
			GenerateScreenRectanglesFromFloatCoords (str, coords, coords_len, new Color32 (  255, 255, 255, 64), false);
			break;
		case 3:
			{
				ViewManager vm = FindObjectOfType<ViewManager>();
				Color32 col;
				if (vm != null)
					col = vm.highlightColor;
				else
					col = new Color32 (255, 0, 0, 64);
				GenerateScreenRectanglesFromFloatCoords (str, coords, coords_len, col, false);
				if (vm.saveHighlightColorToDisk) {
					// save image to of colors disk
					SaveRectanglesToFile ("Testing.png", coords, coords_len, col);
					vm.saveHighlightColorToDisk = false;
				}
			}
			break;
		case 4:
			GenerateScreenRectanglesFromFloatCoords (str, coords, coords_len, new Color32 (  0, 255, 0, 64), false);
			break;
		case 5:
			//screen-projection-rectangles
			GenerateScreenRectanglesFromFloatCoords (str, coords, coords_len, new Color32 (  0, 0, 255, 64), false);
			break;
		case 6:
			GenerateObjectFromFloatCoords (str, coords, coords_len, new Color32 (  64, 64, 200, 128), false);
			break;
		case 7:
			GenerateLineObjectFromFloatCoordsMulti (str, coords, coords_len, new Color32 (255, 0, 0, 255), .1f);
			//GenerateLineObjectFromFloatCoords (str, coords, coords_len, new Color32 (255, 0, 0, 255));
			break;
		case 8:
			GenerateObjectFromFloatCoords (str, coords, coords_len, new Color32 (  0, 255, 255, 64), false);
			break;
		case 9:
			//close-to-edge-rectangle
			GenerateScreenRectanglesFromFloatCoords (str, coords, coords_len, new Color32 (  128, 128, 128, 64), false);
			break;
		case 10:
			break;
		default:
			//			Debug.Log ("Unity_Function_i_s_fa WARNING: op=" + op + " not implemented");
			break;
		}
	}
	static byte rotColorAlpha = 64;
	static Color32[] rotColors = {
		Color.red,
		Color.blue,
		new Color32(119,51,131,rotColorAlpha), // purple
		new Color32(103,151,236,rotColorAlpha), // carolina blue
		new Color32(247,140,13,rotColorAlpha), // orange
		new Color32(35,147,23,rotColorAlpha), // green
		Color.yellow,
		Color.grey,
		Color.magenta
	};
	/*
	static Color32 [] rotColors = {
		new Color32(231,117,122,rotColorAlpha),
		new Color32(243,160,106,rotColorAlpha),
		new Color32(175,113,141,rotColorAlpha),
		new Color32(252,204,105,rotColorAlpha),
		new Color32(253,249,109,rotColorAlpha),
		new Color32(144,215,135,rotColorAlpha),
		new Color32(140,212,188,rotColorAlpha),
		new Color32(186,199,151,rotColorAlpha),
		new Color32(120,160,216,rotColorAlpha),
		new Color32(154,132,215,rotColorAlpha),
		new Color32(90,104,133,rotColorAlpha),
		new Color32(93,93,93,rotColorAlpha)
	};*/
	static byte f2byte(float fval){
		return (byte)(fval * 255.0f);
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_fa_fa))]
	static void Unity_Function_i_s_fa_fa(int op, string str, IntPtr colorptr, int color_len, IntPtr coordsptr, int coords_len)
	{
		float [] color = new float[color_len];
		Marshal.Copy (colorptr, color, 0, color_len);
		float [] coords = new float[coords_len];
		Marshal.Copy (coordsptr, coords, 0, coords_len);
#else
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_s_fa_fa(int op, string str, float [] color, int color_len, float [] coords, int coords_len)
	{
#endif
		//		Debug.Log ("Unity_Function_i_s_fa: op=" + op + " name=" + str + " coords_len=" + coords_len + " len=" + coords_len);
		switch (op){
		case 1:
			GenerateObjectFromFloatCoords (str, coords, coords_len, new Color32 (  f2byte(color[0]), f2byte(color[1]), f2byte(color[2]), 128), false );
			break;
		case 2:
			GenerateObjectFromFloatCoords (str, coords, coords_len, new Color32 (  f2byte(color[0]), f2byte(color[1]), f2byte(color[2]), 128), true );
			break;
		case 3:
			GenerateScreenRectanglesFromFloatCoords(str, coords, coords_len, new Color32 (  f2byte(color[0]), f2byte(color[1]), f2byte(color[2]), 128), false );
			break;
		}
	}
    static void TurnToScreenStabilized(int goid, bool ss){
		Dictionary<int, GameObject> vmobjs = VMObject.m_VMObjects;
		GameObject go = vmobjs [goid];
		VMObject vmo = go.GetComponent<VMObject> ();
        if (!go.activeSelf)
            return;
		if (vmo != null) {
			vmo.screenStabilized = ss;
		} else {
			Debug.Log ("TurnToScreenStabilized: goid=" + goid + " vmo=" + vmo);
		}
	}

	static void TurnOffAnnotationFor(int goid, bool checkFade){
		Dictionary<int, GameObject> vmobjs = VMObject.m_VMObjects;
		GameObject go = vmobjs [goid];
		VMObject vmo = go.GetComponent<VMObject> ();
        if (vmo.whenShowInDirectPlacement) // TODO : for now, just return
			return;
		if (vmo.m_tryToLabelOnCentroid)
			return;
		if (vmo != null) {
			vmo.turnOff (checkFade);
		} else {
			Debug.Log ("TurnOffAnnotationFor: goid=" + goid + " vmo=" + vmo);
		}
	}
	static void TurnOnAnnotationFor(int goid, bool showArrow){
		Dictionary<int, GameObject> vmobjs = VMObject.m_VMObjects;
		GameObject go = vmobjs [goid];
		VMObject vmo = go.GetComponent<VMObject> ();
		if (vmo != null) {
			vmo.SetAnnotationsActive (true, showArrow);
			//vmo.turnOff (checkFade);
		} else {
			Debug.Log ("TurnOnAnnotationFor: goid=" + goid + " vmo=" + vmo);
		}
	}
    public static GameObject createScreenTexture(ViewManager vm, string objname, float zval)
    {
        GameObject go = new GameObject(objname);
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        mesh.vertices = new Vector3[]
        {
                        new Vector3(-1.0f,-1.0f,zval),
                        new Vector3(-1.0f,1.0f,zval),
                        new Vector3(1.0f,1.0f,zval),
                        new Vector3(1.0f,-1.0f,zval)
        };
        mesh.uv = new Vector2[]
        {
                        new Vector2(0.0f,0.0f),
                        new Vector2(0.0f,1.0f),
                        new Vector2(1.0f,1.0f),
                        new Vector2(1.0f,0.0f)
        };
        mesh.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
        mr.material.color = Color.white;
        mr.material.shader = Shader.Find("Unlit/UnlitShader");
        mr.material.renderQueue = 5000;
        go.transform.SetParent(vm.screenCanvas.transform);
        go.AddComponent<ScreenRelativeScript>();
        ScreenRelativeScript srs = go.GetComponent<ScreenRelativeScript>();
        bool isHMD = vm.screenProjectionMode == 1 || vm.screenProjectionMode == 2;
        bool isVive = vm.screenProjectionMode == 2;
        srs.screenRelative = new Rect(.5f, .2f, .25f, .25f);
        return go;
    }

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s))]
#endif
	static void Unity_Function_i_s(int op, string str)
	{
		switch (op) {
		case 1:
			// Remove object
			{
				GameObject go = GameObject.Find (str);
				if (go != null) {
					Destroy (go);
				}
			}
			break;
		case 2:
			{// Create-Screen-Texture-With-Name-map
				GameObject go = GameObject.Find (str);
				if (go == null) { // Create
                    ViewManager vm = FindObjectOfType<ViewManager>();
                    go = createScreenTexture(vm, str, 0.0f);
					ScreenRelativeScript srs = go.GetComponent<ScreenRelativeScript> ();
                    bool isHMD = vm.screenProjectionMode == 1 || vm.screenProjectionMode == 2;
                    bool isVive = vm.screenProjectionMode == 2;
                    if (isHMD)
                        srs.screenRelative = new Rect(.5f, .2f, .25f, .25f);
                    else
						srs.screenRelative = new Rect (.6f, .1f, .35f, .35f);
				} else {
					Debug.Log ("WARNING: Create-Screen-Texture-With-Name-map: Object name=" + str + " already exists");
				}
			}
			break;
		case 3:
			{// EMPTY
			}
			break;
		case 4:
			{// Remove-Rest-Except-Children-Of-Tab-Delimited-map
				string[] coordids = str.Split ('	');
				int coordidslen = coordids.Length;
				int[] coordIDs = new int[coordidslen];
				for (int i = 0; i < coordidslen; i++) {
					coordIDs [i] = Convert.ToInt32 (coordids [i]);
				}
				VMObject[] vmos = FindObjectsOfType<VMObject> ();
				foreach (VMObject vmo in vmos) {
					GameObject go = vmo.gameObject;
					bool turnOff = true;
					if (vmo.coordinateSystem != null) {
						foreach (int coordID in coordIDs) {
							if (vmo.coordinateSystem.GetVMDimensionID () == coordID) {
								turnOff = false;
							}
						}
					}
					if (turnOff) {
						vmo.turnOff (true);
					}
				}
			}
			break;
		case 5:
			{// Set-Rotation-Position-In-MouseScript-Impl-map
				GameObject go = GameObject.Find (str);
				MouseScriptImpl ms = FindObjectOfType<MouseScriptImpl> ();
				if (ms == null)
					return;
				bool isSet = false;
				if (go != null) {
					Renderer r = go.GetComponent<Renderer> ();
					if (r != null) {
						ms.setRotatePoint (r.bounds.center);
						isSet = true;
					}
				}
				if (!isSet) {
					ms.setRotatePoint (new Vector3 ());
				}
			}
			break;
		case 6:
			{//Zoom-Back-From-map
				GameObject go = GameObject.Find (str);
				if (go != null) {
					AnimateCameraImpl acs = FindObjectOfType<AnimateCameraImpl> ();
					Renderer r = go.GetComponent<Renderer> ();
					Vector3 ext = r.bounds.extents;
					float maxext = Mathf.Max (new float[] { ext.x, ext.y, ext.z });
					Camera curCamera = ViewManager.getCurrentCamera ();
					acs.animateCameraToPositionWithDuration (curCamera.transform.forward * (-2.0f * maxext) + curCamera.transform.position, acs.getDefaultAnimationTime ());
					acs.setShouldExecuteViewManagementAfterAnimation (true);
					DPManagerScript.Call_s_s_s_s_b ("SetDataValue-map", "UnityMain", "do-not-change-view", "uid", "main-uid", true);
//					DPManagerScript.Call_noargs ("SetupStillView-From-Last-map");
				} else {
					Debug.Log ("WARNING: Zoom-Back-From-map object str=" + str + " not found");
				}
				break;
			}
		case 7:
			{//Zoom-Forward-From-map
				GameObject go = GameObject.Find (str);
				if (go != null) {
					MouseOverImpl mos = go.GetComponent<MouseOverImpl> ();
					AnimateCameraImpl acs = FindObjectOfType<AnimateCameraImpl> ();
					Renderer r = go.GetComponent<Renderer> ();
					bool canZoom = false;
					if (mos != null && mos.zoomIntoGet()) {
						canZoom = true;
					}
					Vector3 ext = r.bounds.extents;
					float maxext = Mathf.Max (new float[] { ext.x, ext.y, ext.z });
					Camera curCamera = ViewManager.getCurrentCamera ();
					Vector3 pos = curCamera.transform.forward * (1.0f * maxext) + curCamera.transform.position;
					if (!canZoom) {
						Bounds b = r.bounds;
						b.Expand (b.extents);
						canZoom = !b.Contains (pos);
					}
					if (canZoom) {
						acs.animateCameraToPositionWithDuration (pos, .5f);
						acs.setShouldExecuteViewManagementAfterAnimation (true);
						DPManagerScript.Call_s_s_s_s_b ("SetDataValue-map", "UnityMain", "do-not-change-view", "uid", "main-uid", true);
					} else {
						Debug.Log ("Too Close, not moving");
					}
				} else {
					Debug.Log ("WARNING: Zoom-Back-From-map object str=" + str + " not found");
				}
				break;
			}
		case 8:
			{// EMPTY
			}
			break;
		case 9:
			{// EMPTY
			}
			break;
		case 10:
			{// Save-ScreenShot-To-File-map
				string fileName = str;
				Camera curCamera = ViewManager.getCurrentCamera ();
				int resWidth = curCamera.pixelWidth;
				int resHeight = curCamera.pixelHeight;
				RenderTexture rt = new RenderTexture (resWidth, resHeight, 24);
				curCamera.targetTexture = rt;
				Texture2D screenShot = new Texture2D (resWidth, resHeight, TextureFormat.RGB24, false);
				curCamera.Render ();
				RenderTexture.active = rt;
				screenShot.ReadPixels (new Rect (0, 0, resWidth, resHeight), 0, 0);
				curCamera.targetTexture = null;
				RenderTexture.active = null;
				Destroy (rt);
				byte[] bytes = screenShot.EncodeToPNG ();
				System.IO.File.WriteAllBytes (fileName, bytes);
			}
			break;
		}
	}

	static void ResetLineForVMObject (int goid, float [] boundsdata, int boundsdata_len){
		Dictionary<int, GameObject> vmobjs = VMObject.m_VMObjects;
		GameObject govmobj = vmobjs [goid];
		VMObject vmobj = govmobj.GetComponent<VMObject> ();
		if (vmobj != null) {
			Camera curCamera = ViewManager.getCurrentCamera();
			Vector3 worldPoint = vmobj.lineEndPoint1;
			Vector3 screenPoint = curCamera.WorldToScreenPoint (worldPoint);
			float zval = screenPoint.z;
			float minx = boundsdata[0], miny = boundsdata[1], maxx = boundsdata[2], maxy = boundsdata[3];
			float maxOutsideDistance = boundsdata [4];
			float centerx = (minx + maxx) / 2.0f, centery = (miny + maxy) / 2.0f;
			float w = maxx - minx, h = maxy - miny;
			float w2 = w / 2.0f, h2 = h / 2.0f;

			float endx = Mathf.Round(screenPoint.x), endy = Mathf.Round(screenPoint.y);
			// clip to bounds around annotation in x and y
			float clipx = (endx - centerx), clipy = (endy - centery);

			if (clipx < -w2) { // left
				clipy = - clipy * w2 / clipx; //y = y0 + (y1 - y0) * (xmin - x0) / (x1 - x0);
				clipx = -w2;                  //x = xmin;
			} else if (clipx > w2) { // right
				clipy = clipy * w2 / clipx; //y = y0 + (y1 - y0) * (xmax - x0) / (x1 - x0);
				clipx = w2; //x = xmax;
			}

			if (clipy < -h2) { // bottom
				clipx = - clipx * h2 / clipy; //x = x0 + (x1 - x0) * (ymin - y0) / (y1 - y0);
				clipy = -h2; //y = ymin;
			} else if (clipy > h2) { // top
				clipx = clipx * h2 / clipy; //x = x0 + (x1 - x0) * (ymax - y0) / (y1 - y0);
				clipy = h2; //y = ymax;
			}
			clipx += centerx;
			clipy += centery;
			clipx = Mathf.Round(clipx);
			clipy = Mathf.Round(clipy);
			GameObject goannotationline = vmobj.annotationlineGet();
			vmobj.prevActiveLine = goannotationline.activeSelf;
			LineRenderer lr = goannotationline.GetComponent<LineRenderer> ();
			float lineWidth = vmobj.outsideLineWidth * DPUtils.getScaleFromZDistance (zval);

			Vector3 linept2 = curCamera.ScreenToWorldPoint (new Vector3 (clipx, clipy, zval));
			Vector3 lineEndPoint1;
//			Vector2 lineVect = new Vector2 (centerx - endx, centery - endy);
			Vector2 lineVect = new Vector2 (endx - clipx, endy - clipy);
			float mag = lineVect.magnitude;
			bool shouldChange = true;
			float actualDist = mag;
			if (mag > maxOutsideDistance) {
				actualDist = maxOutsideDistance;
			//} else if (mag < 5) {
			//	actualDist = 5;
			} else {
				shouldChange = false;
			}
			if (shouldChange){ //mag > maxOutsideDistance) {
				Vector2 lineVectNorm = new Vector2 (endx - centerx, endy - centery);
//				Vector2 lineVectNorm = new Vector2 (centerx - endx, centery - endy);
				lineVectNorm.Normalize ();
				Vector2 newScreenPoint = lineVectNorm * actualDist; // maxOutsideDistance;
				newScreenPoint.x += clipx; // endx;
				newScreenPoint.y += clipy; // endy;
				lineEndPoint1 = curCamera.ScreenToWorldPoint (new Vector3 (Mathf.Round(newScreenPoint.x), Mathf.Round(newScreenPoint.y), zval));
			} else {
				lineEndPoint1 = curCamera.ScreenToWorldPoint (new Vector3 (endx, endy, zval));
			}
			{
				lr.positionCount = 2;
				lr.SetPositions (new Vector3[] { lineEndPoint1, linept2 });
                lr.startWidth = lr.endWidth = lineWidth;
			}
			vmobj.lineEndPoint2 = linept2;
			goannotationline.SetActive (true);
		} else {
			Debug.Log ("ResetLineForVMObject: govmobj=" + govmobj + " vmobj=" + vmobj);
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_i_ia))]
	static void Unity_Function_i_i_ia(int op, int goid, IntPtr iarrayptr, int iarray_len){
		int [] iarray = new int[iarray_len];
		Marshal.Copy (iarrayptr, iarray, 0, iarray_len);
#else
	static void Unity_Function_i_i_ia(int op, int goid, int [] iarray, int iarray_len){
#endif
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_ba))]
	static void Unity_Function_i_s_ba(int op, string str, IntPtr barrayptr, int barray_len){
		byte [] barray = new byte[barray_len];
		Marshal.Copy (barrayptr, barray, 0, barray_len);
#else
	static void Unity_Function_i_s_ba(int op, string str, byte [] barray, int barray_len){
#endif
		switch (op){
		case 1:
			// Set-Image-To-Unity-Object-map
			GameObject go = GameObject.Find(str);
			//Debug.Log("Set-Image-To-Unity-Object-map: str='" + str + "' (go==null)=" + (go==null));
			if (go!=null){
				Renderer rend = go.GetComponent<Renderer> ();
				Material mat = rend.material;
				Texture2D tex = null;
				if (mat.mainTexture == null){
					tex = new Texture2D(2,2);
					tex.LoadImage(barray);
					mat.mainTexture = tex;
				} else {
					tex = (Texture2D)mat.mainTexture;
					tex.LoadImage(barray);
				}
				ScreenRelativeScript srs = go.GetComponent<ScreenRelativeScript>();
				if (srs!=null){
					srs.imageSizeSet(tex.width, tex.height);
				} else {
					Debug.Log("WARNING: ScreenRelativeScript not found");
				}
			} else {
				Debug.Log("WARNING: Set-Image-To-Unity-Object-map: cannot find object str='" + str + "'");
			}
			break;
		}
	}

#if UNITY_IOS_ONLY
[MonoPInvokeCallback(typeof(MyDelegate_i_i_i_fa))]
static void Unity_Function_i_i_i_fa(int op, int goid, int arg2, IntPtr farrayptr, int farray_len)
{
	float [] farray = new float[farray_len];
	Marshal.Copy (farrayptr, farray, 0, farray_len);
#else
static void Unity_Function_i_i_i_fa(int op, int goid, int arg2, float [] farray, int farray_len)
{
#endif
	switch (op) {
		case 1:
			{ // Generate-And-Set-Overhead-Map-map
				int coordID = arg2;
				float mapScale = 1.0f;
				if (farray_len > 6)
					mapScale = farray[6];
				VMDimension vmd = null;
				GameObject [] allTurnOnForDim = null;
				bool [] allTurnOnEnabled = null;
				if (VMDimension.m_VMDimensions.TryGetValue(coordID, out vmd)){
					if (vmd.allTurnOnWhenZoomedInto!=null && vmd.allTurnOnWhenZoomedInto.Length > 0){
						allTurnOnForDim = vmd.allTurnOnWhenZoomedInto;
						allTurnOnEnabled = new bool[allTurnOnForDim.Length];
						for (int i=0; i<allTurnOnForDim.Length; i++){
							allTurnOnEnabled[i] = allTurnOnForDim[i].activeSelf;
							if (!allTurnOnEnabled[i])
								allTurnOnForDim[i].SetActive(true);
						}
					}
				}
				int twidth = 500, theight = 500;
                Vector3 center = new Vector3(farray[0], farray[1], farray[2]);
                Vector3 size = new Vector3(farray[3], farray[4], farray[5]);
                Camera cam = InitOrthoCamera();
				GameObject go = GameObject.Find ("OrthoCamera");
				Vector3 campos = center;
				campos.y += size.y * 3;
				cam.orthographicSize = Mathf.Max(size.x,size.z) / (2.0f * mapScale);//10.0f;
				go.transform.position = campos;//new Vector3 (0, 10, 0);
				go.transform.LookAt(new Vector3(campos.x,0,campos.z));
				RenderTexture renderTexture = new RenderTexture (twidth, theight, 24);
				cam.targetTexture = renderTexture;

				cam.Render ();
				RenderTexture.active = renderTexture;
				Texture2D virtualPhoto = new Texture2D (twidth, theight, TextureFormat.RGB24, false);
				virtualPhoto.ReadPixels (new Rect (0, 0, twidth, theight), 0, 0);
				cam.targetTexture = null;

				if (allTurnOnEnabled!=null){
					for (int i=0; i<allTurnOnForDim.Length; i++){
						if (!allTurnOnEnabled[i])
							allTurnOnForDim[i].SetActive(false);
					}
				}
				byte[] bytes = virtualPhoto.EncodeToPNG ();
				Call_i_ba ("Set-Distributed3DHeadsetUser-Map-map", goid, bytes, bytes.Length);
			}
			break;
		case 2:
			//Get-View-Position-Orientation-And-Send-To-map
			{
				Vector2 position = new Vector2(farray[0], farray[1]);
				Vector2 viewvector = new Vector2(farray[2], farray[3]);
				bool yFromCur = true;
				float yVal = 0.0f;
				if (farray_len > 4){
					int vmdid = (int)farray[4];
					VMDimension vmd;
					if (VMDimension.m_VMDimensions.TryGetValue(vmdid, out vmd))
					{
						if (vmd.isTopForNavigation)
						{
							yFromCur = false;
							yVal = vmd.topY;
						}
						if (vmd.target!=null)
						{
							// TODO: SET yFromCur to y from bbx
							if (!vmd.isTopForNavigation && farray_len > 5)
							{
								yVal = farray[5];
								yFromCur = false;
							}
						}
					}
				}
				GetViewPositionOrientationAndSendTo(goid, arg2, position, viewvector, yFromCur, yVal);
			}
			break;
								
	}
}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_i_fa))]
	static void Unity_Function_i_i_fa(int op, int goid, IntPtr farrayptr, int farray_len)
	{
		float [] farray = new float[farray_len];
		Marshal.Copy (farrayptr, farray, 0, farray_len);
#else
	static void Unity_Function_i_i_fa(int op, int goid, float [] farray, int farray_len)
	{
#endif
		switch (op) {
		case 1:
			ResetLineForVMObject (goid, farray, farray_len);
			break;
		}

	}
#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_i_fa))]
	static void Unity_Function_i_s_i_fa(int op, string str, int idx, IntPtr farrayptr, int farray_len)
	{
		float [] farray = new float[farray_len];
		Marshal.Copy (farrayptr, farray, 0, farray_len);
#else
	static void Unity_Function_i_s_i_fa(int op, string str, int idx, float [] farray, int farray_len)
	{
#endif
		switch (op) {
		case 1:
			{ // Write-Rectangles-To-File-With-Color-In-Image-map
				Color32 col = new Color32();
				Color32 lcol = rotColors[idx % rotColors.Length];
				col.r = lcol.r;
				col.g = lcol.g;
				col.b = lcol.b;
				col.a = rotColorAlpha;
				SaveRectanglesToFile (str, farray, farray_len, col); //rotColors[idx % rotColors.Length]);
			}
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_b))]
#endif
	static void Unity_Function_i_s_b(int op, string str, bool val)
	{
		switch (op) {
		case 1:
			{
				GameObject go = GameObject.Find (str);
				if (go != null) {
					go.SetActive (val);
				}
			// SetActive
			}
			break;
		case 2:
			{// EMPTY
			}
			break;
		case 3:
			{// SetBoxCollidersOfObjectChildrenEnabled-map
				GameObject go = GameObject.Find (str);
				if (go != null) {
					VMDimension vmd = go.GetComponent<VMDimension> ();
					if (vmd != null) {
						vmd.SetBoxCollidersEnabled (val);
					}
				}
			}
			break;
		case 4:
			{
				//Turn-Active-All-GameObjects-With-Tag-map
				if (val) {
					GameObject[] gos = Resources.FindObjectsOfTypeAll<GameObject> ();
					foreach (GameObject go in gos) {
						if (go.tag == str)
							go.SetActive (val);
					}
				} else {
					GameObject[] gos = GameObject.FindGameObjectsWithTag (str);
					foreach (GameObject go in gos) {
						go.SetActive (val);
					}
				}
			}
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_s_b))]
#endif
	static void Unity_Function_i_s_s_b(int op, string str, string str2, bool val)
	{
		switch (op) {
		case 1:
			{ // UnityObject-Set-Active-Of-Child-map
				GameObject go = GameObject.Find (str);
				if (go != null) {
					Transform[] trs = go.GetComponentsInChildren<Transform> (true);
					foreach (Transform t in trs) {
						if (t.name == str2) {
							t.gameObject.SetActive (val);
						}
					}
				}
			}
			break;
		case 2:
			{ // UnityObject-Set-Active-Of-Child-RectTransform-map
				GameObject go = GameObject.Find (str);
				if (go != null) {
					RectTransform[] trs = go.GetComponentsInChildren<RectTransform> (true);
					foreach (RectTransform t in trs) {
						if (t.name == str2) {
							t.gameObject.SetActive (val);
						}
					}
				}
			}
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_i))]
#endif
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_s_i(int op, string str, int val)
	{
		switch (op) {
		case 1:
			{
				// AddShowTagOnClickExcept-map OLD
				DPUtils.AddShowTagOnClickExcept (str, val);
			}
			break;
		}
	}
#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_i_i))]
#endif
	static void Unity_Function_i_i_i(int op, int val, int val2)
	{
		switch (op) {
		case 1:
			{
				// AddShowChildrenOnClickExcept-map
				VMDimension vmd;
				if (VMDimension.m_VMDimensions.TryGetValue(val2, out vmd)){
					vmd.AddShowChildrenOnClickExcept(val);
				} else {
					Debug.Log("AddShowChildrenOnClickExcept-map: cannot find VMDimension id=" + val2);
				}
			}
			break;
		}
	}
#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_i_i))]
#endif
	static void Unity_Function_i_s_i_i(int op, string str, int val, int val2)
	{
		switch (op) {
		case 1:
			// Set-Placement-For-Screen-Texture-map
			GameObject go = GameObject.Find (str);
			int cnt = val;
			int nareas = val2;
			ViewManager vm = FindObjectOfType<ViewManager> ();
			ScreenRelativeScript srs = go.GetComponent<ScreenRelativeScript> ();
			bool isHMD = vm.screenProjectionMode == 1 || vm.screenProjectionMode == 2;
			float iwidth = isHMD ? .25f : .35f;
			if (nareas > 2) {
				iwidth = 1.5f / ((float)nareas);
			}
			float xoffset = isHMD ? .5f : .6f;
			if (nareas > 1) {
				float distBetweenCenters = 1.0f / ((float)nareas);
				xoffset = xoffset - distBetweenCenters * cnt;
			}
			float ymin = isHMD ? .2f : .1f;
			float yheight = isHMD ? .25f : .35f;
			srs.screenRelative = new Rect(xoffset, ymin, iwidth, yheight);
			//srs.screenRelative = new Rect(.5f, .2f, .25f, .25f);
							/*} else {
				srs.screenRelative = new Rect (.6f, .1f, .35f, .35f);
			}*/
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_i_s))]
#endif
	static void Unity_Function_i_s_i_s(int op, string str, int val, string str2)
	{
		switch (op) {
		case 1:
			{ // GotoViewOfObjectWithIndexVector-map
				AnimateCameraImpl acs = FindObjectOfType<AnimateCameraImpl> ();
				GameObject go = GameObject.Find (str);
				Vector3 vect = DPUtils.GetVectorFromContentIndex (val);
				if (DPUtils.GotoViewOfObjectWithViewVector (go, acs, vect) &&
					str2!=null && str2.Length > 2) {
					acs.setShouldExecuteViewManagementAfterAnimation(true);
					DPManagerScript.Call_noargs (str2);
				}
			}
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_s))]
#endif
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_s_s(int op, string str, string str2)
	{
		switch (op) {
		case 1:
			{
				// EMPTY
			}
			break;
		case 2:
			{ // UnityObject-Set-Text-Of-map
				GameObject go = GameObject.Find (str);
				if (go != null) {
					Text tm = go.GetComponent<Text> ();
					if (tm != null) {
						tm.text = str2;
					} else {
						Debug.Log ("WARNING: UnityObject-Set-Text-Of-map cannot find Text for object '" + str + "'");
					}
				}
			}
			break;
		case 3:
			{ // System-cp-map
				byte [] fbytes = File.ReadAllBytes(str);
				File.WriteAllBytes (str2, fbytes);
			}
			break;
		case 4:
			{ // Convert-Black-To-Transparent-In-Image-map
				byte [] img1 = File.ReadAllBytes(str);
				Texture2D tex1 = new Texture2D(2,2,TextureFormat.RGBA32, false);
				tex1.LoadImage(img1);
				Texture2D tex3 = DPUtils.ConvertBlackToTransparency(tex1);
				byte[] img3 = tex3.EncodeToPNG();
				File.WriteAllBytes (str2, img3);
			}
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_i_s_s))]
#endif
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_i_s_s(int op, int val, string str, string str2)
	{
	}
#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_i_i_s_s))]
#endif
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_i_i_s_s(int op, int val, int val2, string str, string str2)
	{
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_i_i_s_s_s))]
#endif
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_i_i_s_s_s(int op, int val, int val2, string str, string str2, string str3)
	{
		switch (op) {
		case 1:
			{ // Add-Screen-Menu-Item-map
				ScreenButtonsImpl sbs = FindObjectOfType<ScreenButtonsImpl> ();
				if (sbs != null) {
					sbs.addScreenMenu (val, val2, str, str2, str3);
				} else {
					Debug.Log ("WARNING: Add-Screen-Menu-Item-map but ScreenButtonsImpl does not exist");
				}
			}
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_s_s))]
#endif
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_s_s_s(int op, string str, string str2, string str3)
	{
		switch (op) {
		case 1:
			{
				// EMPTY
			}
			break;
		case 2:
			// Blend-File-Images-Into-map
			{
				if (!File.Exists(str) || !File.Exists(str2)){
					Debug.Log("WARNING: str=" + str + " exists=" + File.Exists(str) + " str2=" + str2 + " exists=" + File.Exists(str2));
					return;
				}
				byte [] img1 = File.ReadAllBytes(str);
				byte [] img2 = File.ReadAllBytes(str2);
				Texture2D tex1 = new Texture2D(2,2,TextureFormat.RGBA32, false);
				tex1.LoadImage(img1);
				Texture2D tex2 = new Texture2D(2,2,TextureFormat.RGBA32, false);
				tex2.LoadImage(img2);
				Texture2D tex3 = DPUtils.AlphaBlend(tex1,tex2);
				byte[] img3 = tex3.EncodeToPNG();
				File.WriteAllBytes (str3, img3);
			}
			break;
		case 3:
			{ // Save-Image-Diff-To-File-map
				byte [] img1 = File.ReadAllBytes(str);
				byte [] img2 = File.ReadAllBytes(str2);
				Texture2D tex1 = new Texture2D(2,2,TextureFormat.RGBA32, false);
				tex1.LoadImage(img1);
				Texture2D tex2 = new Texture2D(2,2,TextureFormat.RGBA32, false);
				tex2.LoadImage(img2);
				Texture2D tex3 = DPUtils.ImageDiff(tex1,tex2);
				byte[] img3 = tex3.EncodeToPNG();
				File.WriteAllBytes (str3, img3);
			}
			break;
		case 4:
			{ // Add-Image-To-Image-map
				byte [] img1 = File.ReadAllBytes(str);
				byte [] img2 = File.ReadAllBytes(str2);
				Texture2D tex1 = new Texture2D(2,2,TextureFormat.RGBA32, false);
				tex1.LoadImage(img1);
				Texture2D tex2 = new Texture2D(2,2,TextureFormat.RGBA32, false);
				tex2.LoadImage(img2);
				Texture2D tex3 = DPUtils.ImageAdd(tex1,tex2);
				byte[] img3 = tex3.EncodeToPNG();
				File.WriteAllBytes (str3, img3);
			}
			break;
		}
	}
#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_fa_ia_ia))]
	static void Unity_Function_i_s_fa_ia_ia(int op, string str,
					IntPtr coordsptr, int coords_len,
					IntPtr int_valsptr, int int_vals_len,
					IntPtr int_vals2ptr, int int_vals2_len)
	{
		float [] coords = new float[coords_len];
		Marshal.Copy (coordsptr, coords, 0, coords_len);
		int [] int_vals = new int[int_vals_len];
		Marshal.Copy (int_valsptr, int_vals, 0, int_vals_len);
		int [] int_vals2 = new int[int_vals2_len];
		Marshal.Copy (int_vals2ptr, int_vals2, 0, int_vals2_len);
#else
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_s_fa_ia_ia(int op, string str,
											float [] coords, int coords_len,
											int [] int_vals, int int_vals_len,
											int [] int_vals2, int int_vals2_len)
	{
#endif
		switch (op) {
		case 1:
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_i_b))]
#endif
	static void Unity_Function_i_i_b(int op, int goid, bool val){
	switch (op) {
		case 1:
			SetBothAnnotationsActive (goid, val);
			break;
		case 2:
			// TurnOffAnnotationFor-map
			TurnOffAnnotationFor(goid, val);
			break;
		case 3:
			//
			TurnOnAnnotationFor(goid, val);
			break;
        case 4:
            TurnToScreenStabilized(goid, val);
            break;
        case 5:  // Set-Request-Place-With-Direct-View-map
                {
                    GameObject go;
                    if (VMObject.m_VMObjects.TryGetValue(goid, out go)){
                        VMObject vmo = go.GetComponent<VMObject>();
                        if (vmo != null) {
                            vmo.requestPlacementWithDirectView = val;
                        } else {
                            Debug.Log("WARNING: Set-Request-Place-With-Direct-View-map: goid=" + goid + " vmo=" + vmo);
                        }
                    }
                }
                break;
		case 6:
			{ // Turn-When-Show-In-Direct-Placement-To-map
				Dictionary<int, GameObject> vmobjs = VMObject.m_VMObjects;
				GameObject go = vmobjs [goid];
				VMObject vmo = go.GetComponent<VMObject> ();
				vmo.whenShowInDirectPlacement = val;
				vmo.tryToLabel = val;
                    if (val)
                    {
                        vmo.placeWithDirectView = true;
                        vmo.notVisibleLayoutMode = 1; // closest to
                        vmo.notVisibleArrowMode = 2; // cut arrow
                    }
                    else
                    {
                        vmo.showingDirectViewPlacementSet(false);
                    }
			}
			break;
		case 7:
			{ // Turn-Labels-In-Coordinate-System-For-Navigation-map
				VMDimension vmd;
				if (VMDimension.m_VMDimensions.TryGetValue (goid, out vmd)) {
					foreach (VMObject vmo in vmd.allVMObjectsToTurnOnForNavigation) {
						vmo.tryToLabel = val;
					}
				}
			}
			break;
		case 8:
			{ // Turn-Objects-In-Coordinate-System-For-Zooming-In-map
				VMDimension vmd;
				if (VMDimension.m_VMDimensions.TryGetValue (goid, out vmd)) {
					foreach (GameObject go in vmd.allTurnOnWhenZoomedInto) {
						go.SetActive (val);
					}
				}
			}
			break;
		case 9:
			{ // Set-Try-To-Label-For-All-In-Coordinate-System-map
				VMDimension vmd;
				if (VMDimension.m_VMDimensions.TryGetValue (goid, out vmd)) {
					foreach (VMObject vmo in vmd.children) {
						vmo.tryToLabel = val;
					}											
				} else {
					Debug.Log ("Set-Try-To-Label-For-All-In-Coordinate-System-map: cannot find VMDimension id=" + goid);
				}
			}
			break;
		case 10:
			{ // Set-Try-To-Place-Inside-For-All-In-Coordinate-System-map
				VMDimension vmd;
				if (VMDimension.m_VMDimensions.TryGetValue (goid, out vmd)) {
					foreach (VMObject vmo in vmd.children) {
						vmo.tryToLabelInside = val;
					}											
				} else {
					Debug.Log ("Set-Try-To-Label-For-All-In-Coordinate-System-map: cannot find VMDimension id=" + goid);
				}
			}
			break;
        }
    }

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_i))]
#endif
	static void Unity_Function_i_i(int op, int int_arg){
		switch (op) {
		case 1:
            { //Get-CoordinateSystem-To-World-For-map
                    int coordSysID = int_arg;
                    VMDimension vmd;
                    if (VMDimension.m_VMDimensions.TryGetValue(coordSysID, out vmd))
                    {
                        if (vmd.target != null)
                        {
                            Matrix4x4 dim2World = vmd.target.gameObject.transform.localToWorldMatrix;
                            float[] dim2WorldArray =
                            {
                            dim2World.m00, dim2World.m01, dim2World.m02, dim2World.m03,
                            dim2World.m10, dim2World.m11, dim2World.m12, dim2World.m13,
                            dim2World.m20, dim2World.m21, dim2World.m22, dim2World.m23,
                            dim2World.m30, dim2World.m31, dim2World.m32, dim2World.m33
                            };
                            Matrix4x4 world2dim = vmd.target.gameObject.transform.worldToLocalMatrix;
                            float[] world2dimArray =
                            {
                            world2dim.m00, world2dim.m01, world2dim.m02, world2dim.m03,
                            world2dim.m10, world2dim.m11, world2dim.m12, world2dim.m13,
                            world2dim.m20, world2dim.m21, world2dim.m22, world2dim.m23,
                            world2dim.m30, world2dim.m31, world2dim.m32, world2dim.m33
                            };
                            Call_i_t3f_t3f("Set-UnityCoordinateSystem-Transforms-map", coordSysID, dim2WorldArray, world2dimArray);
                        }
                    }
            }
            break;
		case 2:
			// Remove-Screen-Menu-Item-Greater-Than-Or-Equal-map
			{
				ScreenButtonsImpl sbs = FindObjectOfType<ScreenButtonsImpl> ();
				if (sbs != null)
					sbs.removeMenuGreaterThanOrEqualTo (int_arg);
			}
			break;
		case 3:
			{//AddShowChildrenOnClick-And-Remove-Rest-map
				int coordSysID = int_arg;
				VMDimension vmd;
				bool vmd_set = false;
				if (VMDimension.m_VMDimensions.TryGetValue (coordSysID, out vmd)) {
					vmd_set = true;
				}
				VMObject[] vmos = FindObjectsOfType<VMObject> ();
				foreach (VMObject vmo in vmos) {
					GameObject go = vmo.gameObject;
					bool turnOff = true;
					if (vmd_set) {
						turnOff = vmo.coordinateSystem.GetVMDimensionID () != vmd.GetVMDimensionID ();
					}
					if (turnOff) {
						vmo.turnOff (true);
					}
				}
			}
			break;
		case 4:
			{ //AddShowChildrenOnClick-map
				VMDimension vmd;
				if (VMDimension.m_VMDimensions.TryGetValue (int_arg, out vmd)) {
					vmd.AddShowChildrenOnClick ();
				} else {
					Debug.Log ("AddShowChildrenOnClick-map: cannot find VMDimension id=" + int_arg);
				}
			}
			break;
		case 5:
			{ // SetChildrenOfToFlyover-map
				VMDimension vmd;
				if (VMDimension.m_VMDimensions.TryGetValue (int_arg, out vmd)) {
					vmd.setToFlyoverLayoutForAllChildren ();
				} else {
					Debug.Log ("SetChildrenOfToFlyover-map: cannot find VMDimension id=" + int_arg);
				}
			}
			break;
		case 6:
			{ // SetChildrenOfToRotateLayout-map
				MouseScriptImpl ms = FindObjectOfType<MouseScriptImpl> ();
				if (ms == null)
					return;
				VMDimension vmd;
				if (VMDimension.m_VMDimensions.TryGetValue (int_arg, out vmd)) {
					vmd.setToFlyoverLayoutForAllChildren ();
				} else {
					Debug.Log ("SetChildrenOfToRotateLayout-map: cannot find VMDimension id=" + int_arg);
				}
			}
			break;
        case 7:
                { // Set-View-To-Direct-View-For-Object-From-Current-Camera-Position-map
                    GameObject go;
                    if (VMObject.m_VMObjects.TryGetValue(int_arg, out go))
                    {
                        VMObject vmobj = go.GetComponent<VMObject>();
                        Camera curCam = ViewManager.getCurrentCamera();
						Vector3 curCamPosInWorld = ViewManager.getWorldToTopMatrix().MultiplyPoint(curCam.transform.position);
                        Matrix4x4 camdiff = curCam.transform.worldToLocalMatrix * curCam.worldToCameraMatrix.inverse;
						Vector3 direction = (vmobj.geometryCentroidGet() - curCamPosInWorld).normalized;
                        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up); // need to use Y-up, not curCam.transform.up

                        Matrix4x4 worldToDim = ViewManager.getTopToWorldMatrix();
                        VMDimension vmd = vmobj.coordinateSystem;
                        if (vmd!=null && vmd.target != null)
                        {
                            worldToDim = vmd.target.transform.worldToLocalMatrix;
                            vmd.dimToWorldSet(vmd.target.transform.localToWorldMatrix);
                        }
                        Vector3 curCamInDim = worldToDim * curCamPosInWorld;
                        Matrix4x4 worldToDirectView = worldToDim * Matrix4x4.TRS(curCamPosInWorld, rotation, curCam.transform.localScale);
                        worldToDirectView = worldToDirectView.inverse;
                        vmobj.worldToDirectViewSet(worldToDirectView);

                        ViewManager.setViewToCurrentCamera("Set-Camera-Up-And-Right-map", "Set-View-Info-map", false, curCam.projectionMatrix, 
												           camdiff, worldToDirectView,
                                                           curCam.pixelWidth, curCam.pixelHeight, curCam.nearClipPlane, curCam.fieldOfView);
                    } else
                    {
                        Debug.Log("WARNING: Set-View-To-Direct-View-For-Object-From-Current-Camera-Position-map");
                    }
                }
                break;
        case 8:
                // Set-View-To-Current-Camera-For-CoordinateSystem-map
                {
                    VMDimension vmd;
                    if (VMDimension.m_VMDimensions.TryGetValue(int_arg, out vmd))
                    {
                        vmd.setToFlyoverLayoutForAllChildren();
                        ViewManager vm = FindObjectOfType<ViewManager>();
                        Camera curCam = ViewManager.getCurrentCamera();
                        Matrix4x4 camdiff = curCam.transform.worldToLocalMatrix * curCam.worldToCameraMatrix.inverse;
                        Matrix4x4 dimToWorld = ViewManager.getTopToWorldMatrix();
                        if (vmd.target != null)
                        {
                            dimToWorld = vmd.target.transform.localToWorldMatrix;
                        }
                        ViewManager.setViewToCurrentCamera("Set-Camera-Up-And-Right-map", "Set-View-Info-map", vm.debugTraversal,
                                                curCam.projectionMatrix, camdiff, curCam.transform.worldToLocalMatrix * dimToWorld,
                                                curCam.pixelWidth, curCam.pixelHeight, curCam.nearClipPlane, curCam.fieldOfView);
                    }
                    else
                    {
                        Debug.Log("SetChildrenOfToRotateLayout-map: cannot find VMDimension id=" + int_arg);
                    }
                }
                break;
            case 9:
                { // Animate-Target-To-Scale-If-map
                    VMDimension vmd;
                    if (VMDimension.m_VMDimensions.TryGetValue(int_arg, out vmd))
                    {
                        if (vmd.target != null)
                        {
                            InterpolateTRSScript iscript = vmd.target;
                            if (vmd.targetScaleIsSet)
                            {
                                Vector3 targ = vmd.targetScale;
                                Matrix4x4 dim2World;
                                Matrix4x4 localT = new Matrix4x4();
                                localT.SetTRS(vmd.target.gameObject.transform.localPosition, vmd.target.gameObject.transform.localRotation, targ);
								if (vmd.target != null && vmd.target.gameObject.transform.parent != null)
                                {
                                    dim2World = vmd.target.gameObject.transform.parent.localToWorldMatrix;
                                    dim2World = dim2World * localT;
                                }
                                else
                                {
                                    dim2World = localT;
                                }
                                //dim2World = Matrix4x4.Scale(scaleChange) * dim2World;
                                float[] dim2WorldArray =
                                {
                                    dim2World.m00, dim2World.m01, dim2World.m02, dim2World.m03,
                                    dim2World.m10, dim2World.m11, dim2World.m12, dim2World.m13,
                                    dim2World.m20, dim2World.m21, dim2World.m22, dim2World.m23,
                                    dim2World.m30, dim2World.m31, dim2World.m32, dim2World.m33
                                };
                                Call_s_s_s_i_t3f("SetDataValue-map", "UnityCoordinateSystem", "cs-to-world-target", "id", int_arg, dim2WorldArray);
                                iscript.AnimateScaleTo(vmd.targetScale);
                            }
                        }
                    }
                }
                break;
            case 10:
                { // Set-cs-to-world-target-For-map 
                    VMDimension vmd;
                    if (VMDimension.m_VMDimensions.TryGetValue(int_arg, out vmd))
                    {
                        Vector3 targ = vmd.targetScale;
						Matrix4x4 dim2World = Matrix4x4.identity;
                        Matrix4x4 localT = new Matrix4x4();
						if (vmd.target != null){
	                        localT.SetTRS(vmd.target.gameObject.transform.localPosition, vmd.target.gameObject.transform.localRotation, targ);
							if (vmd.target.gameObject.transform.parent != null)
	                        {
	                            dim2World = vmd.target.gameObject.transform.parent.localToWorldMatrix;
	                            dim2World = dim2World * localT;
	                        } else
	                        {
	                            dim2World = localT;
	                        }
						}
                        //Matrix4x4 dim2World = vmd.target.gameObject.transform.localToWorldMatrix;
                        //dim2World.SetTRS(dim2World.GetPosition(), dim2World.GetRotation(), targ);
                        //dim2World = Matrix4x4.Scale(scaleChange) * dim2World;
                        float[] dim2WorldArray =
                        {
                                    dim2World.m00, dim2World.m01, dim2World.m02, dim2World.m03,
                                    dim2World.m10, dim2World.m11, dim2World.m12, dim2World.m13,
                                    dim2World.m20, dim2World.m21, dim2World.m22, dim2World.m23,
                                    dim2World.m30, dim2World.m31, dim2World.m32, dim2World.m33
                                };
                        Call_s_s_s_i_t3f("SetDataValue-map", "UnityCoordinateSystem", "cs-to-world-target", "id", int_arg, dim2WorldArray);
                    }
                }
                break;
			case 11:
				{ // Turn-To-Still-Layout-And-Off-map
					VMDimension vmd;
					if (VMDimension.m_VMDimensions.TryGetValue(int_arg, out vmd))
					{
						DPUtils.TurnToStillLayoutAndOff(true, vmd);
					}
				}
				break;
			case 12:
				{ // Set-Dimension-To-Traverse-Only-map
					VMDimension vmd;
					if (VMDimension.m_VMDimensions.TryGetValue(int_arg, out vmd))
					{
						vmd.setTraverseToOnly();
					}
				}
				break;
		default:
			Debug.Log("Unity_Function_i_i: WARNING: op=" + op + " int_arg=" + int_arg);
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i))]
#endif
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i(int op){
		switch (op) {
        case 0:
            {
                    ViewManager vm = FindObjectOfType<ViewManager>();
                    if (vm != null)
                    {
                        vm.CheckLicenseAndRequestIfNecessary();
                    }
            }
            break;
		case 1:
			// TurnAllToStillLayoutAndOff-map
			DPUtils.TurnAllToStillLayoutAndOff(false);
			break;
		case 2:
			ScreenButtonsImpl sbs = FindObjectOfType<ScreenButtonsImpl> ();
			if (sbs != null)
				sbs.clearOrderToIndentMenuItem ();
			break;
		case 3:
            {
                FindObjectOfType<ViewManager>().debugTraversal = true;
            }
			break;
		case 4:
			// Turn-Camera-SkyBox-map
			ViewManager.getCurrentCamera().clearFlags = CameraClearFlags.Skybox;
			break;
        case 5:
                //Set-View-To-Current-Camera-map
                {
                    ViewManager vm = FindObjectOfType<ViewManager>();
					Camera curCam = ViewManager.getCurrentCamera();
                    Matrix4x4 camdiff = curCam.transform.worldToLocalMatrix * curCam.worldToCameraMatrix.inverse;
					ViewManager.setViewToCurrentCamera("Set-Camera-Up-And-Right-map", "Set-View-Info-map", vm.debugTraversal,
											curCam.projectionMatrix, camdiff, curCam.transform.worldToLocalMatrix * ViewManager.getTopToWorldMatrix(),
                                            curCam.pixelWidth, curCam.pixelHeight, curCam.nearClipPlane, curCam.fieldOfView);
                }
                break;
		case 6:
			{
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
				//Application.OpenURL(webplayerQuitURL);
#else
				Application.Quit();
#endif
			}
			break;
		case 7:
			{
				ViewManager vm = FindObjectOfType<ViewManager>();
				vm.invalidateReportRendering();
			}
			break;
		case 8:
			{
				VMObject[] vmos = UnityEngine.Object.FindObjectsOfType<VMObject>();
				foreach (VMObject vmo in vmos) {
					if (vmo.m_tryToLabel && vmo.whenShowInDirectPlacement){
						vmo.requestPlacementWithDirectView = true;
					}
				}
			}
			break;
		case 9:
			{
				VMObject[] vmos = UnityEngine.Object.FindObjectsOfType<VMObject>();
				foreach (VMObject vmo in vmos) {
					if (vmo.m_tryToLabel && vmo.whenShowInDirectPlacement){
						vmo.tryToLabel = false;
						vmo.turnOff (true);
						vmo.showingDirectViewPlacementSet(false);
					}
				}
			}
		break;
        case 10:
                // Reload-TraverseCoordinateSystem-Into-Unity-map
                {
                    DPManagerScript.execute(new DPActionFunc
                    {
                        actionPerformedFunc = () => ReloadAllDimInfo()
                    });
                }
                break;
        case 11:
            { //Invalidate-Local-Position-Orientation-map
                ViewManager vm = FindObjectOfType<ViewManager>();
                vm.prevViewMatrixIsSet = false;
            }
            break;
        default:
			Debug.Log ("Unity_Function_i WARNING: op=" + op);
			break;
        }
    }
    public static void ReloadAllDimInfo()
    {
        ViewManager vm = FindObjectOfType<ViewManager>();
        if (vm != null)
        {
			vm.setAllDimInfo(DPManagerScript.Call_rsa_wrapper_il("Get-All-Coord-Systems-Traversing-Plus-Order-map"));
#if UNITY_EDITOR
            ViewManagerScript.invalidate();
#endif
        }
    }

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_fa_ia_ia_ba_ia))]
	static void Unity_Function_i_fa_ia_ia_ba_ia(int op,
						IntPtr coordsptr, int coords_len,
						IntPtr int_valsptr, int int_vals_len,
						IntPtr int_vals2ptr, int int_vals2_len,
						IntPtr bool_valsptr, int bool_vals_len,
						IntPtr int_vals3ptr, int int_vals3_len)
	{
		float [] coords = new float[coords_len];
		Marshal.Copy (coordsptr, coords, 0, coords_len);
		int [] int_vals = new int[int_vals_len];
		Marshal.Copy (int_valsptr, int_vals, 0, int_vals_len);
		int [] int_vals2 = new int[int_vals2_len];
		Marshal.Copy (int_vals2ptr, int_vals2, 0, int_vals2_len);
		byte [] bool_vals = new byte[bool_vals_len];
		Marshal.Copy (bool_valsptr, bool_vals, 0, bool_vals_len);
		int [] int_vals3 = new int[int_vals3_len];
		Marshal.Copy (int_vals3ptr, int_vals3, 0, int_vals3_len);
#else
#if (!UNITY_WEBGL || UNITY_EDITOR)
    static
#endif
    void Unity_Function_i_fa_ia_ia_ba_ia(int op,
		float [] coords, int coords_len,
		int [] int_vals, int int_vals_len,
		int [] int_vals2, int int_vals2_len,
		byte [] bool_vals, int bool_vals_len,
		int [] int_vals3, int int_vals3_len)
	{
#endif
		switch (op) {
		case 1:
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_fa_ia_ba_ia_fa))]
	static void Unity_Function_i_fa_ia_ba_ia_fa(int op,
							IntPtr coordsptr, int coords_len,
							IntPtr int_valsptr, int int_vals_len,
							IntPtr bool_valsptr, int bool_vals_len,
							IntPtr int_vals3ptr, int int_vals3_len,
							IntPtr normalsptr, int normals_len)
	{
		float [] coords = new float[coords_len];
		Marshal.Copy (coordsptr, coords, 0, coords_len);
		int [] int_vals = new int[int_vals_len];
		Marshal.Copy (int_valsptr, int_vals, 0, int_vals_len);
		byte [] bool_vals = new byte[bool_vals_len];
		Marshal.Copy (bool_valsptr, bool_vals, 0, bool_vals_len);
		int [] int_vals3 = new int[int_vals3_len];
		Marshal.Copy (int_vals3ptr, int_vals3, 0, int_vals3_len);
		float [] normals = new float[normals_len];
		Marshal.Copy (normalsptr, normals, 0, normals_len);
#else
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_fa_ia_ba_ia_fa(int op,
		float [] coords, int coords_len,
		int [] int_vals, int int_vals_len,
		byte [] bool_vals, int bool_vals_len,
		int [] int_vals3, int int_vals3_len,
		float [] normals, int normals_len)
	{
#endif
		switch (op) {
		case 1:
			SetScreenPlacementsForAnnotationResults (coords, coords_len, int_vals, int_vals_len,
				bool_vals, bool_vals_len, int_vals3, int_vals3_len, normals, normals_len);
			break;
		}
	}

#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_fa_ia_ia_ba_ia_fa))]
	static void Unity_Function_i_fa_ia_ia_ba_ia_fa(int op,
		IntPtr coordsptr, int coords_len,
		IntPtr int_valsptr, int int_vals_len,
		IntPtr int_vals2ptr, int int_vals2_len,
		IntPtr bool_valsptr, int bool_vals_len,
		IntPtr int_vals3ptr, int int_vals3_len,
		IntPtr normalsptr, int normals_len)
	{
		float [] coords = new float[coords_len];
		Marshal.Copy (coordsptr, coords, 0, coords_len);
		int [] int_vals = new int[int_vals_len];
		Marshal.Copy (int_valsptr, int_vals, 0, int_vals_len);
		int [] int_vals2 = new int[int_vals2_len];
		Marshal.Copy (int_vals2ptr, int_vals2, 0, int_vals2_len);
		byte [] bool_vals = new byte[bool_vals_len];
		Marshal.Copy (bool_valsptr, bool_vals, 0, bool_vals_len);
		int [] int_vals3 = new int[int_vals3_len];
		Marshal.Copy (int_vals3ptr, int_vals3, 0, int_vals3_len);
		float [] normals = new float[normals_len];
		Marshal.Copy (normalsptr, normals, 0, normals_len);
#else
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_fa_ia_ia_ba_ia_fa(int op,
		float [] coords, int coords_len,
		int [] int_vals, int int_vals_len,
		int [] int_vals2, int int_vals2_len,
		byte [] bool_vals, int bool_vals_len,
		int [] int_vals3, int int_vals3_len,
		float [] normals, int normals_len)
	{
#endif
		switch (op) {
		case 1:
			break;
		}
	}


#if UNITY_IOS_ONLY
	[MonoPInvokeCallback(typeof(MyDelegate_i_s_fa_ia))]
	static void Unity_Function_i_s_fa_ia(int op, string str, IntPtr coordsptr, int coords_len, IntPtr int_valsptr, int int_vals_len)
	{
		float [] coords = new float[coords_len];
		Marshal.Copy (coordsptr, coords, 0, coords_len);
		int [] int_vals = new int[int_vals_len];
		Marshal.Copy (int_valsptr, int_vals, 0, int_vals_len);
#else
#if (!UNITY_WEBGL || UNITY_EDITOR)
	static
#endif
	void Unity_Function_i_s_fa_ia(int op, string str, float [] coords, int coords_len, int [] int_vals, int int_vals_len)
	{
#endif
	// nothing for now
	}

    // Use this for initialization
    static public bool isInitialized = false;
    static public bool DPisInitialized = false;
#if UNITYRENDER
	IEnumerator Start () {
#else
    void Start() {
        if (!isInitialized)
            ReStart();
#endif
    }

    public static void initLogCallbacks()
    {
#if !UNITY_WEBGL && (!UNITY_ANDROID || (!EDITOR_COMPILES_ANDROID && UNITY_EDITOR))
        if (unitylog_callback_delegate == null)
        {
            unitylog_callback_delegate = new MyDelegate(UnityLogCallBackFunction);
            IntPtr unitylog_intptr_delegate = Marshal.GetFunctionPointerForDelegate(unitylog_callback_delegate);
            log_callback_delegate = new MyDelegate(LogCallBackFunction);
            IntPtr log_intptr_delegate = Marshal.GetFunctionPointerForDelegate(log_callback_delegate);
            err_callback_delegate = new MyDelegate(ErrCallBackFunction);
            IntPtr err_intptr_delegate = Marshal.GetFunctionPointerForDelegate(err_callback_delegate);
            SetUnityLogFunction(unitylog_intptr_delegate);
            SetLogFunction(log_intptr_delegate);
            SetErrFunction(err_intptr_delegate);
        }
#endif
    }

    public static void ReStart()
    {
        initLogCallbacks(); // for debugging purposes
        if (DP_IsUp())
            ClearDP();
        /* TODO : For All existing VMObjects, do we need to reset? */
        //VMObject[] vmobjs = FindObjectsOfType<VMObject>();
        Init();
    }
        static public void Init(){
        if (!isInitialized)
        {
#if !UNITY_WEBGL && (!UNITY_ANDROID || (!EDITOR_COMPILES_ANDROID && UNITY_EDITOR))
            initLogCallbacks();
            obj_exists_callback_delegate = new MyDelegate_rb(UnityObjectExistsCallBackFunction);
            IntPtr obj_exists_intptr_delegate = Marshal.GetFunctionPointerForDelegate(obj_exists_callback_delegate);

            obj_annot_active_callback_delegate = new MyDelegate_rb_i(UnityObjectAnnotationActiveFunction);
            IntPtr obj_annot_active_intptr_delegate = Marshal.GetFunctionPointerForDelegate(obj_annot_active_callback_delegate);
            SetUnityObjectExistsFunction(obj_exists_intptr_delegate);
            SetUnityObjectAnnotationActiveFunction(obj_annot_active_intptr_delegate);

            //BEGIN CreateAndSetDelegate
	unity_i_delegate = new MyDelegate_i( Unity_Function_i );
	IntPtr unity_i_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_delegate);
	SetUnityFunction_i(unity_i_delegate_ptr);
	unity_i_b_delegate = new MyDelegate_i_b( Unity_Function_i_b );
	IntPtr unity_i_b_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_b_delegate);
	SetUnityFunction_i_b(unity_i_b_delegate_ptr);
	unity_i_f_delegate = new MyDelegate_i_f( Unity_Function_i_f );
	IntPtr unity_i_f_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_f_delegate);
	SetUnityFunction_i_f(unity_i_f_delegate_ptr);
	unity_i_f_f_f_delegate = new MyDelegate_i_f_f_f( Unity_Function_i_f_f_f );
	IntPtr unity_i_f_f_f_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_f_f_f_delegate);
	SetUnityFunction_i_f_f_f(unity_i_f_f_f_delegate_ptr);
	unity_i_fa_delegate = new MyDelegate_i_fa( Unity_Function_i_fa );
	IntPtr unity_i_fa_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_fa_delegate);
	SetUnityFunction_i_fa(unity_i_fa_delegate_ptr);
	unity_i_fa_s_delegate = new MyDelegate_i_fa_s( Unity_Function_i_fa_s );
	IntPtr unity_i_fa_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_fa_s_delegate);
	SetUnityFunction_i_fa_s(unity_i_fa_s_delegate_ptr);
	unity_i_i_b_delegate = new MyDelegate_i_i_b( Unity_Function_i_i_b );
	IntPtr unity_i_i_b_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_i_b_delegate);
	SetUnityFunction_i_i_b(unity_i_i_b_delegate_ptr);
	unity_i_i_delegate = new MyDelegate_i_i( Unity_Function_i_i );
	IntPtr unity_i_i_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_i_delegate);
	SetUnityFunction_i_i(unity_i_i_delegate_ptr);
	unity_i_s_delegate = new MyDelegate_i_s( Unity_Function_i_s );
	IntPtr unity_i_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_delegate);
	SetUnityFunction_i_s(unity_i_s_delegate_ptr);
	unity_i_s_ba_delegate = new MyDelegate_i_s_ba( Unity_Function_i_s_ba );
	IntPtr unity_i_s_ba_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_ba_delegate);
	SetUnityFunction_i_s_ba(unity_i_s_ba_delegate_ptr);
	unity_i_i_ia_delegate = new MyDelegate_i_i_ia( Unity_Function_i_i_ia );
	IntPtr unity_i_i_ia_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_i_ia_delegate);
	SetUnityFunction_i_i_ia(unity_i_i_ia_delegate_ptr);
	unity_i_i_fa_delegate = new MyDelegate_i_i_fa( Unity_Function_i_i_fa );
	IntPtr unity_i_i_fa_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_i_fa_delegate);
	SetUnityFunction_i_i_fa(unity_i_i_fa_delegate_ptr);
	unity_i_i_i_fa_delegate = new MyDelegate_i_i_i_fa( Unity_Function_i_i_i_fa );
	IntPtr unity_i_i_i_fa_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_i_i_fa_delegate);
	SetUnityFunction_i_i_i_fa(unity_i_i_i_fa_delegate_ptr);
	unity_i_s_b_delegate = new MyDelegate_i_s_b( Unity_Function_i_s_b );
	IntPtr unity_i_s_b_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_b_delegate);
	SetUnityFunction_i_s_b(unity_i_s_b_delegate_ptr);
	unity_i_s_s_delegate = new MyDelegate_i_s_s( Unity_Function_i_s_s );
	IntPtr unity_i_s_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_s_delegate);
	SetUnityFunction_i_s_s(unity_i_s_s_delegate_ptr);
	unity_i_s_s_b_delegate = new MyDelegate_i_s_s_b( Unity_Function_i_s_s_b );
	IntPtr unity_i_s_s_b_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_s_b_delegate);
	SetUnityFunction_i_s_s_b(unity_i_s_s_b_delegate_ptr);
	unity_i_s_s_s_delegate = new MyDelegate_i_s_s_s( Unity_Function_i_s_s_s );
	IntPtr unity_i_s_s_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_s_s_delegate);
	SetUnityFunction_i_s_s_s(unity_i_s_s_s_delegate_ptr);
	unity_i_i_s_s_delegate = new MyDelegate_i_i_s_s( Unity_Function_i_i_s_s );
	IntPtr unity_i_i_s_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_i_s_s_delegate);
	SetUnityFunction_i_i_s_s(unity_i_i_s_s_delegate_ptr);
	unity_i_i_i_s_s_delegate = new MyDelegate_i_i_i_s_s( Unity_Function_i_i_i_s_s );
	IntPtr unity_i_i_i_s_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_i_i_s_s_delegate);
	SetUnityFunction_i_i_i_s_s(unity_i_i_i_s_s_delegate_ptr);
	unity_i_i_i_s_s_s_delegate = new MyDelegate_i_i_i_s_s_s( Unity_Function_i_i_i_s_s_s );
	IntPtr unity_i_i_i_s_s_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_i_i_s_s_s_delegate);
	SetUnityFunction_i_i_i_s_s_s(unity_i_i_i_s_s_s_delegate_ptr);
	unity_i_s_i_delegate = new MyDelegate_i_s_i( Unity_Function_i_s_i );
	IntPtr unity_i_s_i_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_i_delegate);
	SetUnityFunction_i_s_i(unity_i_s_i_delegate_ptr);
	unity_i_i_i_delegate = new MyDelegate_i_i_i( Unity_Function_i_i_i );
	IntPtr unity_i_i_i_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_i_i_delegate);
	SetUnityFunction_i_i_i(unity_i_i_i_delegate_ptr);
	unity_i_s_i_i_delegate = new MyDelegate_i_s_i_i( Unity_Function_i_s_i_i );
	IntPtr unity_i_s_i_i_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_i_i_delegate);
	SetUnityFunction_i_s_i_i(unity_i_s_i_i_delegate_ptr);
	unity_i_s_i_s_delegate = new MyDelegate_i_s_i_s( Unity_Function_i_s_i_s );
	IntPtr unity_i_s_i_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_i_s_delegate);
	SetUnityFunction_i_s_i_s(unity_i_s_i_s_delegate_ptr);
	unity_i_s_i_fa_delegate = new MyDelegate_i_s_i_fa( Unity_Function_i_s_i_fa );
	IntPtr unity_i_s_i_fa_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_i_fa_delegate);
	SetUnityFunction_i_s_i_fa(unity_i_s_i_fa_delegate_ptr);
	unity_i_s_fa_delegate = new MyDelegate_i_s_fa( Unity_Function_i_s_fa );
	IntPtr unity_i_s_fa_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_fa_delegate);
	SetUnityFunction_i_s_fa(unity_i_s_fa_delegate_ptr);
	unity_i_s_fa_s_delegate = new MyDelegate_i_s_fa_s( Unity_Function_i_s_fa_s );
	IntPtr unity_i_s_fa_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_fa_s_delegate);
	SetUnityFunction_i_s_fa_s(unity_i_s_fa_s_delegate_ptr);
	unity_i_s_s_fa_b_s_delegate = new MyDelegate_i_s_s_fa_b_s( Unity_Function_i_s_s_fa_b_s );
	IntPtr unity_i_s_s_fa_b_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_s_fa_b_s_delegate);
	SetUnityFunction_i_s_s_fa_b_s(unity_i_s_s_fa_b_s_delegate_ptr);
	unity_i_s_s_fa_ba_b_s_delegate = new MyDelegate_i_s_s_fa_ba_b_s( Unity_Function_i_s_s_fa_ba_b_s );
	IntPtr unity_i_s_s_fa_ba_b_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_s_fa_ba_b_s_delegate);
	SetUnityFunction_i_s_s_fa_ba_b_s(unity_i_s_s_fa_ba_b_s_delegate_ptr);
	unity_i_s_fa_b_s_delegate = new MyDelegate_i_s_fa_b_s( Unity_Function_i_s_fa_b_s );
	IntPtr unity_i_s_fa_b_s_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_fa_b_s_delegate);
	SetUnityFunction_i_s_fa_b_s(unity_i_s_fa_b_s_delegate_ptr);
	unity_i_s_fa_fa_delegate = new MyDelegate_i_s_fa_fa( Unity_Function_i_s_fa_fa );
	IntPtr unity_i_s_fa_fa_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_fa_fa_delegate);
	SetUnityFunction_i_s_fa_fa(unity_i_s_fa_fa_delegate_ptr);
	unity_i_s_fa_ia_delegate = new MyDelegate_i_s_fa_ia( Unity_Function_i_s_fa_ia );
	IntPtr unity_i_s_fa_ia_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_fa_ia_delegate);
	SetUnityFunction_i_s_fa_ia(unity_i_s_fa_ia_delegate_ptr);
	unity_i_s_fa_ia_ia_delegate = new MyDelegate_i_s_fa_ia_ia( Unity_Function_i_s_fa_ia_ia );
	IntPtr unity_i_s_fa_ia_ia_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_s_fa_ia_ia_delegate);
	SetUnityFunction_i_s_fa_ia_ia(unity_i_s_fa_ia_ia_delegate_ptr);
	unity_i_fa_ia_ia_ba_ia_delegate = new MyDelegate_i_fa_ia_ia_ba_ia( Unity_Function_i_fa_ia_ia_ba_ia );
	IntPtr unity_i_fa_ia_ia_ba_ia_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_fa_ia_ia_ba_ia_delegate);
	SetUnityFunction_i_fa_ia_ia_ba_ia(unity_i_fa_ia_ia_ba_ia_delegate_ptr);
	unity_i_fa_ia_ia_ba_ia_fa_delegate = new MyDelegate_i_fa_ia_ia_ba_ia_fa( Unity_Function_i_fa_ia_ia_ba_ia_fa );
	IntPtr unity_i_fa_ia_ia_ba_ia_fa_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_fa_ia_ia_ba_ia_fa_delegate);
	SetUnityFunction_i_fa_ia_ia_ba_ia_fa(unity_i_fa_ia_ia_ba_ia_fa_delegate_ptr);
	unity_i_fa_ia_ba_ia_fa_delegate = new MyDelegate_i_fa_ia_ba_ia_fa( Unity_Function_i_fa_ia_ba_ia_fa );
	IntPtr unity_i_fa_ia_ba_ia_fa_delegate_ptr = Marshal.GetFunctionPointerForDelegate(unity_i_fa_ia_ba_ia_fa_delegate);
	SetUnityFunction_i_fa_ia_ba_ia_fa(unity_i_fa_ia_ba_ia_fa_delegate_ptr);
//END CreateAndSetDelegate

#endif
            isInitialized = true;
        }
        if (!DPisInitialized)
        {
            DPisInitialized = true; // to fake, in case InitDP needs to check license
			if (ViewManager.isHololens) {
				SetupDPProperty ("AR", "true");
			}
            InitDP("./dataTypeFiles/");
            if (DP_IsUp())
            {
                DPisInitialized = true;
#if UNITYRENDER
    	        GL.IssuePluginEvent (GetRenderEventFunc (), 1);
	    	    //		CommandBuffer.IssuePluginEvent(GetRenderEventFunc(), 1);
	            yield return StartCoroutine("CallPluginAtEndOfFrames");
#endif
                FindObjectOfType<ViewManager>().CallAllChanged();
                fireActionsToExecuteOnStart();
				InitOrthoCamera ();
            } else {
            	DPisInitialized = false;
            }
        }
	}
	public static Camera InitOrthoCamera(){
		GameObject go = GameObject.Find ("OrthoCamera");
		Camera cam = null;
		if (go != null) {
			cam = go.GetComponent<Camera> ();
		} else {
			go = new GameObject ("OrthoCamera");
		}
		if (cam==null){
			cam = go.AddComponent<Camera> ();
			cam.clearFlags = CameraClearFlags.SolidColor;
			cam.backgroundColor = new Color (49.0f / 255.0f, 77.0f / 255.0f, 121.0f / 255.0f);
			cam.cullingMask = (int)(0xffffffff ^ ((1 << LayerMask.NameToLayer ("UI") | (1 << LayerMask.NameToLayer("OnlyScreenSpace")))));
			cam.orthographic = true;
			cam.nearClipPlane = .3f;
			cam.farClipPlane = 1000.0f;
			cam.rect = new Rect (0.0f, 0.0f, 1.0f, 1.0f);
			cam.depth = 0.0f;
			cam.renderingPath = RenderingPath.UsePlayerSettings;
			cam.useOcclusionCulling = true;
			cam.targetDisplay = 3;
			ViewManager vm = FindObjectOfType<ViewManager> ();
			if (vm.topWorldObject!=null) {
				cam.transform.SetParent (vm.topWorldObject.transform);
			}
		}
		return cam;
	}
	void OnDisable() {
		ClearDP ();
    }

	void OnApplicationQuit(){
        ClearDP();
    }

#if UNITYRENDER
	private IEnumerator CallPluginAtEndOfFrames()
	{
		while (true) {
			// Wait until all frame rendering is done
			yield return new WaitForEndOfFrame();

			// Set time for the plugin
			//Time.realtimeSinceStartup
			SetTimeFromUnity (Time.timeSinceLevelLoad);

			// Issue a plugin event with arbitrary integer identifier.
			// The plugin can distinguish between different
			// things it needs to do based on this ID.
			// For our simple plugin, it does not matter which ID we pass here.
			GL.IssuePluginEvent(GetRenderEventFunc(), 1);
		}
	}
#endif

    static List<DPAction> actionsToExecuteOnStart = new List<DPAction>();
    static Dictionary<string,DPAction> actionsToExecuteWithIDOnStart = new Dictionary<string,DPAction>();
    static List<DPAction> actionsToExecuteAlwaysOnStart = new List<DPAction>();
    static public void fireActionsToExecuteOnStart(){
        foreach (DPAction act in actionsToExecuteAlwaysOnStart){
            act.actionPerformed();
        }
        foreach (DPAction act in actionsToExecuteOnStart)
        {
            act.actionPerformed();
        }
        actionsToExecuteOnStart.Clear();
        foreach (KeyValuePair<string,DPAction> kvact in actionsToExecuteWithIDOnStart)
        {
            kvact.Value.actionPerformed();
        }
        actionsToExecuteWithIDOnStart.Clear();
    }
    static public bool executeWithID(string id, DPAction action)
    {
        if (DPisInitialized)
        {
            action.actionPerformed();
            return true;
        }
        else
        {
            if (actionsToExecuteWithIDOnStart.ContainsKey(id))
                actionsToExecuteWithIDOnStart.Remove(id);
            actionsToExecuteWithIDOnStart.Add(id, action);
            return false;
        }
    }
    static public bool execute(DPAction action)
    {
        if (DPisInitialized)
        {
            action.actionPerformed();
            return true;
        }
        else
        {
            actionsToExecuteOnStart.Add(action);
            return false;
        }
    }
    static public void executeAndOnStart(DPAction action){
        if (DPisInitialized)
            action.actionPerformed();
        actionsToExecuteAlwaysOnStart.Add (action);
	}
	public void fireActionsToExecuteOnStartMember(){
		fireActionsToExecuteOnStart ();
	}
	public void printActionsToExecuteOnStart(){
		Debug.Log ("actionsToExecuteOnStart #=" + actionsToExecuteOnStart.Count + " IsUp=" + IsUp());
	}
	static void SetBothAnnotationsActive(int goid, bool val){
		Dictionary<int, GameObject> vmobjs = VMObject.m_VMObjects;
		GameObject go = vmobjs [goid];
		VMObject vmo = go.GetComponent<VMObject> ();
		if (vmo != null) {
			vmo.SetAnnotationsActive (val, val);
		} else {
			Debug.Log ("SetBothAnnotationsActive: goid=" + goid + " vmo=" + vmo);
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	class OpType {
		[FieldOffset(0)]
		public int op;
	}

	[StructLayout(LayoutKind.Explicit)]
	class IntType {
		[FieldOffset(0)]
		public int i1;
	}
	[StructLayout(LayoutKind.Explicit)]
	class Int2Type {
		[FieldOffset(0)]
		public int i1;
		[FieldOffset(4)]
		public int i2;
	}
	[StructLayout(LayoutKind.Explicit)]
	class IntFloatType {
		[FieldOffset(0)]
		public int i1;
		[FieldOffset(4)]
		public float f1;
	}

	[StructLayout(LayoutKind.Explicit)]
	class IntFloat3Type {
		[FieldOffset(0)]
		public int i1;
		[FieldOffset(4)]
		public float f1;
		[FieldOffset(8)]
		public float f2;
		[FieldOffset(12)]
		public float f3;
	}

	[StructLayout(LayoutKind.Explicit)]
	class Int3Type {
		[FieldOffset(0)]
		public int i1;
		[FieldOffset(4)]
		public int i2;
		[FieldOffset(8)]
		public int i3;
	}

	[StructLayout(LayoutKind.Explicit)]
	class Int4Type {
		[FieldOffset(0)]
		public int i1;
		[FieldOffset(4)]
		public int i2;
		[FieldOffset(8)]
		public int i3;
		[FieldOffset(12)]
		public int i4;
	}

	[StructLayout(LayoutKind.Explicit)]
	class Int5Type {
		[FieldOffset(0)]
		public int i1;
		[FieldOffset(4)]
		public int i2;
		[FieldOffset(8)]
		public int i3;
		[FieldOffset(12)]
		public int i4;
		[FieldOffset(16)]
		public int i5;
	}

	[StructLayout(LayoutKind.Explicit)]
	class Int6Type {
		[FieldOffset(0)]
		public int i1;
		[FieldOffset(4)]
		public int i2;
		[FieldOffset(8)]
		public int i3;
		[FieldOffset(12)]
		public int i4;
		[FieldOffset(16)]
		public int i5;
		[FieldOffset(20)]
		public int i6;
	}

	[StructLayout(LayoutKind.Explicit)]
	class Int7Type {
		[FieldOffset(0)]
		public int i1;
		[FieldOffset(4)]
		public int i2;
		[FieldOffset(8)]
		public int i3;
		[FieldOffset(12)]
		public int i4;
		[FieldOffset(16)]
		public int i5;
		[FieldOffset(20)]
		public int i6;
		[FieldOffset(24)]
		public int i7;
	}

	[StructLayout(LayoutKind.Explicit)]
	class Int9Type {
		[FieldOffset(0)]
		public int i1;
		[FieldOffset(4)]
		public int i2;
		[FieldOffset(8)]
		public int i3;
		[FieldOffset(12)]
		public int i4;
		[FieldOffset(16)]
		public int i5;
		[FieldOffset(20)]
		public int i6;
		[FieldOffset(24)]
		public int i7;
		[FieldOffset(28)]
		public int i8;
		[FieldOffset(32)]
		public int i9;
	}

	[StructLayout(LayoutKind.Explicit)]
	class Int11Type {
		[FieldOffset(0)]
		public int i1;
		[FieldOffset(4)]
		public int i2;
		[FieldOffset(8)]
		public int i3;
		[FieldOffset(12)]
		public int i4;
		[FieldOffset(16)]
		public int i5;
		[FieldOffset(20)]
		public int i6;
		[FieldOffset(24)]
		public int i7;
		[FieldOffset(28)]
		public int i8;
		[FieldOffset(32)]
		public int i9;
		[FieldOffset(36)]
		public int i10;
		[FieldOffset(40)]
		public int i11;
	}

	[StructLayout(LayoutKind.Explicit)]
	class Float1Type {
		[FieldOffset(0)]
		public float f1;
	}

#if UNITY_WEBGL_ONLY
	void Unity_Function_ptr(int iarg){
		IntPtr iptr = new IntPtr(iarg);

		if (!iptr.Equals (IntPtr.Zero)) {
			OpType opt = (OpType)Marshal.PtrToStructure (iptr, typeof(OpType) );
			int op = opt.op;
			IntPtr iptr2 = new IntPtr (iarg + 4);
			bool shouldFree = true;
			switch (op) {
			case 1:
				{ // i_i
					Int2Type iit = (Int2Type)Marshal.PtrToStructure (iptr2, typeof(Int2Type) );
					Unity_Function_i_i (iit.i1, iit.i2);
				}
				break;
			case 2:
				{ // i_s
					Int2Type iit = (Int2Type)Marshal.PtrToStructure (iptr2, typeof(Int2Type) );
					IntPtr stringPtr = new IntPtr(iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					Unity_Function_i_s (iit.i1, strbuffer);
					Marshal.FreeCoTaskMem (stringPtr);
				}
				break;
			case 3:
				{ // i_i_b
					Int3Type iit = (Int3Type)Marshal.PtrToStructure (iptr2, typeof(Int3Type) );
					Unity_Function_i_i_b (iit.i1, iit.i2, iit.i3 != 0);
				}
				break;
			case 4:
				{ // i_s_b
					Int3Type iit = (Int3Type)Marshal.PtrToStructure (iptr2, typeof(Int3Type));
					IntPtr stringPtr = new IntPtr (iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					Unity_Function_i_s_b (iit.i1, strbuffer, iit.i3 != 0);
					Marshal.FreeCoTaskMem (stringPtr);
				}
				break;
			case 5:
				{ // i_s_i
					Int3Type iit = (Int3Type)Marshal.PtrToStructure (iptr2, typeof(Int3Type));
					IntPtr stringPtr = new IntPtr (iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					Unity_Function_i_s_i (iit.i1, strbuffer, iit.i3);
					Marshal.FreeCoTaskMem (stringPtr);
				}
				break;
			case 6:
				{ // i_s_s
					Int3Type iit = (Int3Type)Marshal.PtrToStructure (iptr2, typeof(Int3Type));
					IntPtr stringPtr = new IntPtr (iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					IntPtr stringPtr2 = new IntPtr (iit.i3);
					string strbuffer2 = Marshal.PtrToStringAnsi (stringPtr2);
					Unity_Function_i_s_s (iit.i1, strbuffer, strbuffer2);
					Marshal.FreeCoTaskMem (stringPtr);
					Marshal.FreeCoTaskMem (stringPtr2);
				}
				break;
			case 7:
				{ // i_s_i_s
					Int4Type iit = (Int4Type)Marshal.PtrToStructure (iptr2, typeof(Int4Type));
					IntPtr stringPtr = new IntPtr (iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					IntPtr stringPtr2 = new IntPtr (iit.i4);
					string strbuffer2 = Marshal.PtrToStringAnsi (stringPtr2);
					Unity_Function_i_s_i_s (iit.i1, strbuffer, iit.i3, strbuffer2);
					Marshal.FreeCoTaskMem (stringPtr);
					Marshal.FreeCoTaskMem (stringPtr2);
				}
				break;
			case 8:
				{ // i_s_s_b
					Int4Type iit = (Int4Type)Marshal.PtrToStructure (iptr2, typeof(Int4Type));
					IntPtr stringPtr = new IntPtr (iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					IntPtr stringPtr2 = new IntPtr (iit.i3);
					string strbuffer2 = Marshal.PtrToStringAnsi (stringPtr2);
					Unity_Function_i_s_s_b (iit.i1, strbuffer, strbuffer2, iit.i4 != 0);
					Marshal.FreeCoTaskMem (stringPtr);
					Marshal.FreeCoTaskMem (stringPtr2);
				}
				break;
			case 9:
				{ // i_s_s_s
					Int4Type iit = (Int4Type)Marshal.PtrToStructure (iptr2, typeof(Int4Type));
					IntPtr stringPtr = new IntPtr (iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					IntPtr stringPtr2 = new IntPtr (iit.i3);
					string strbuffer2 = Marshal.PtrToStringAnsi (stringPtr2);
					IntPtr stringPtr3 = new IntPtr (iit.i4);
					string strbuffer3 = Marshal.PtrToStringAnsi (stringPtr3);
					Unity_Function_i_s_s_s (iit.i1, strbuffer, strbuffer2, strbuffer3);
					Marshal.FreeCoTaskMem (stringPtr);
					Marshal.FreeCoTaskMem (stringPtr2);
					Marshal.FreeCoTaskMem (stringPtr3);
				}
				break;
			case 10:
				{ // i_i_i_s_s_s
					Int6Type iit = (Int6Type)Marshal.PtrToStructure (iptr2, typeof(Int6Type));
					IntPtr stringPtr = new IntPtr (iit.i4);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					IntPtr stringPtr2 = new IntPtr (iit.i5);
					string strbuffer2 = Marshal.PtrToStringAnsi (stringPtr2);
					IntPtr stringPtr3 = new IntPtr (iit.i6);
					string strbuffer3 = Marshal.PtrToStringAnsi (stringPtr3);
					Unity_Function_i_i_i_s_s_s (iit.i1, iit.i2, iit.i3, strbuffer, strbuffer2, strbuffer3);
					Marshal.FreeCoTaskMem (stringPtr);
					Marshal.FreeCoTaskMem (stringPtr2);
					Marshal.FreeCoTaskMem (stringPtr3);
				}
				break;
			case 11:
				{ // i_i_fa
					Int4Type iit = (Int4Type)Marshal.PtrToStructure (iptr2, typeof(Int4Type));
					IntPtr farrPtr = new IntPtr (iit.i3);
					float [] farray = new float[iit.i4];
					Marshal.Copy (farrPtr, farray, 0, iit.i4);
					Unity_Function_i_i_fa (iit.i1, iit.i2, farray, iit.i4);
					Marshal.FreeCoTaskMem (farrPtr);
				}
				break;
			case 12:
				{ // i_i_ia
					Int4Type iit = (Int4Type)Marshal.PtrToStructure (iptr2, typeof(Int4Type));
					IntPtr iarrPtr = new IntPtr (iit.i3);
					int [] iarray = new int[iit.i4];
					Marshal.Copy (iarrPtr, iarray, 0, iit.i4);
					Unity_Function_i_i_ia (iit.i1, iit.i2, iarray, iit.i4);
					Marshal.FreeCoTaskMem (iarrPtr);
				}
				break;
			case 13:
				{ // i_s_fa
					Int4Type iit = (Int4Type)Marshal.PtrToStructure (iptr2, typeof(Int4Type));
					IntPtr stringPtr = new IntPtr (iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					IntPtr farrPtr = new IntPtr (iit.i3);
					float [] farray = new float[iit.i4];
					Marshal.Copy (farrPtr, farray, 0, iit.i4);
					Unity_Function_i_s_fa (iit.i1, strbuffer, farray, iit.i4);
					Marshal.FreeCoTaskMem (farrPtr);
					Marshal.FreeCoTaskMem (stringPtr);
				}
				break;
			case 14:
				{ // i_s_fa_fa
					Int6Type iit = (Int6Type)Marshal.PtrToStructure (iptr2, typeof(Int6Type));
					IntPtr stringPtr = new IntPtr (iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					IntPtr farrPtr = new IntPtr (iit.i3);
					float [] farray = new float[iit.i4];
					Marshal.Copy (farrPtr, farray, 0, iit.i4);
					IntPtr farrPtr2 = new IntPtr (iit.i5);
					float [] farray2 = new float[iit.i6];
					Marshal.Copy (farrPtr2, farray2, 0, iit.i6);
					Unity_Function_i_s_fa_fa (iit.i1, strbuffer, farray, iit.i4, farray2, iit.i6);
					Marshal.FreeCoTaskMem (farrPtr);
					Marshal.FreeCoTaskMem (farrPtr2);
					Marshal.FreeCoTaskMem (stringPtr);
				}
				break;
			case 15:
				{ // i_s_s_fa_b_s
					Int7Type iit = (Int7Type)Marshal.PtrToStructure (iptr2, typeof(Int7Type));
					IntPtr stringPtr = new IntPtr (iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					IntPtr stringPtr2 = new IntPtr (iit.i3);
					string strbuffer2 = Marshal.PtrToStringAnsi (stringPtr2);

					IntPtr farrPtr = new IntPtr (iit.i4);
					float [] farray = new float[iit.i5];
					Marshal.Copy (farrPtr, farray, 0, iit.i5);

					IntPtr stringPtr3 = new IntPtr (iit.i7);
					string strbuffer3 = Marshal.PtrToStringAnsi (stringPtr3);

					Unity_Function_i_s_s_fa_b_s (iit.i1, strbuffer, strbuffer2, farray, iit.i5, iit.i6 != 0, strbuffer3);

					Marshal.FreeCoTaskMem (farrPtr);
					Marshal.FreeCoTaskMem (stringPtr);
					Marshal.FreeCoTaskMem (stringPtr2);
				}
				break;
			case 16:
				{ // i_s_s_fa_ba_b_s
					Int9Type iit = (Int9Type)Marshal.PtrToStructure (iptr2, typeof(Int9Type));
					IntPtr stringPtr = new IntPtr (iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					IntPtr stringPtr2 = new IntPtr (iit.i3);
					string strbuffer2 = Marshal.PtrToStringAnsi (stringPtr2);
					IntPtr farrPtr = new IntPtr (iit.i4);
					float [] farray = new float[iit.i5];
					Marshal.Copy (farrPtr, farray, 0, iit.i5);
					IntPtr barrPtr = new IntPtr (iit.i6);
					byte [] barray = new byte[iit.i7];
					Marshal.Copy (barrPtr, barray, 0, iit.i7);
					IntPtr stringPtr3 = new IntPtr (iit.i9);
					string strbuffer3 = Marshal.PtrToStringAnsi (stringPtr3);
					Unity_Function_i_s_s_fa_ba_b_s (iit.i1, strbuffer, strbuffer2, farray, iit.i5, barray, iit.i7, iit.i8 != 0, strbuffer3);
					Marshal.FreeCoTaskMem (farrPtr);
					Marshal.FreeCoTaskMem (barrPtr);
					Marshal.FreeCoTaskMem (stringPtr);
					Marshal.FreeCoTaskMem (stringPtr2);
					Marshal.FreeCoTaskMem (stringPtr3);
				}
				break;
			case 17:
				{ // i_fa_ia_ba_ia_fa
					Int11Type iit = (Int11Type)Marshal.PtrToStructure (iptr2, typeof(Int11Type));
					IntPtr farrPtr = new IntPtr (iit.i2);
					float [] farray = new float[iit.i3];
					Marshal.Copy (farrPtr, farray, 0, iit.i3);
					IntPtr iarrPtr = new IntPtr (iit.i4);
					int [] iarray = new int[iit.i5];
					Marshal.Copy (iarrPtr, iarray, 0, iit.i5);
					IntPtr barrPtr = new IntPtr (iit.i6);
					byte [] barray = new byte[iit.i7];
					Marshal.Copy (barrPtr, barray, 0, iit.i7);
					IntPtr iarrPtr2 = new IntPtr (iit.i8);
					int [] iarray2 = new int[iit.i9];
					Marshal.Copy (iarrPtr2, iarray2, 0, iit.i9);
					IntPtr farrPtr2 = new IntPtr (iit.i10);
					float [] farray2 = new float[iit.i11];
					Marshal.Copy (farrPtr2, farray2, 0, iit.i11);
					Unity_Function_i_fa_ia_ba_ia_fa (iit.i1, farray, iit.i3, iarray, iit.i5, barray, iit.i7, iarray2, iit.i9, farray2, iit.i11);
					Marshal.FreeCoTaskMem (farrPtr);
					Marshal.FreeCoTaskMem (iarrPtr);
					Marshal.FreeCoTaskMem (barrPtr);
					Marshal.FreeCoTaskMem (iarrPtr2);
					Marshal.FreeCoTaskMem (farrPtr2);

				}
				break;
			case 18:
				{
					// UnityObjectExists
					Int2Type iit = (Int2Type)Marshal.PtrToStructure (iptr2, typeof(Int2Type));
					IntPtr stringPtr = new IntPtr (iit.i1);
					string strbuffer = (string)Marshal.PtrToStringAnsi (stringPtr);
					bool exists = (GameObject.Find (strbuffer) != null);
					SetIntAt (iarg + 8, exists ? 1 : 0);
					Marshal.FreeCoTaskMem (stringPtr);
					shouldFree = false;
				}
				break;
			case 19:
				{ // i_f
					IntFloatType iit = (IntFloatType)Marshal.PtrToStructure (iptr2, typeof(IntFloatType));
					Unity_Function_i_f (iit.i1, iit.f1);
				}
				break;
			case 20:
				{ // i_s_i_fa
					Int5Type iit = (Int5Type)Marshal.PtrToStructure (iptr2, typeof(Int5Type));
					IntPtr stringPtr = new IntPtr (iit.i2);
					string strbuffer = Marshal.PtrToStringAnsi (stringPtr);
					IntPtr farrPtr = new IntPtr (iit.i4);
					float [] farray = new float[iit.i5];
					Marshal.Copy (farrPtr, farray, 0, iit.i5);
					Unity_Function_i_s_i_fa (iit.i1, strbuffer, iit.i3, farray, iit.i5);
					Marshal.FreeCoTaskMem (farrPtr);
				}
				break;
			case 21:
				{ // i_f_f_f
					IntFloat3Type iit = (IntFloat3Type)Marshal.PtrToStructure (iptr2, typeof(IntFloat3Type));
					Unity_Function_i_f_f_f (iit.i1, iit.f1, iit.f2, iit.f3);
				}
				break;
			case 22:
				{ // i_i_i
					Int3Type iit = (Int3Type)Marshal.PtrToStructure (iptr2, typeof(Int3Type) );
					Unity_Function_i_i_i (iit.i1, iit.i2, iit.i3);
				}
				break;
			default:
				Debug.Log ("WARNING: Unity_Function_ptr : op=" + opt.op + " is not implemented");
				break;
			}
			if (shouldFree)
				Marshal.FreeCoTaskMem (iptr);
		}
#endif
	static void SetScreenPlacementsForAnnotationResults (float [] coords, int coords_len,
														 int [] int_vals, int int_vals_len,
														 byte [] bool_vals, int bool_vals_len,
														 int [] int_vals2, int int_vals2_len,
														 float [] normals, int normals_len){
		// coords : 6 floats for each: screen coordinates: minx, miny, maxx, maxy and depth: minz, maxz
		// int_vals : InstanceIDs for coordinates (1 for each 6 floats in coords)
		// bool_vals : 2 bools per int_vals : showLine (whether outside line is shown) and hasNormal (if true, 3 floats specificed in normals[])
		// int_vals2 : annotation line info 4 ints for each, just the target point for outside annotation: <all-line-info-for-results-list>, ptx, pty
		// normals : normal specified for result (if 2nd bool in bool_vals is true) for annotation orientation
		ViewManager vm = FindObjectOfType<ViewManager>();
		Dictionary<int, GameObject> vmobjs = VMObject.m_VMObjects;
		if (coords_len != int_vals_len * 6) {
			Debug.Log ("WARNING : SetScreenPlacementsForAnnotationResults: coords_len=" + coords_len + " int_vals_len=" + int_vals_len);
			return;
		}
		Camera curCamera = ViewManager.getCurrentCamera();
        Matrix4x4 camdiff = curCamera.transform.worldToLocalMatrix * curCamera.worldToCameraMatrix.inverse;
        int [] closestToEdgeRect = vm.closestToEdgeRect;
		Matrix4x4 invproj = curCamera.projectionMatrix.inverse;
        float[] zvals = new float[int_vals_len];
        float[] zvalsScreen = new float[int_vals_len];
        int normal_off = 0;
		for (int i = 0; i < int_vals_len; i++) {
			GameObject govmobj = vmobjs [int_vals [i]];
			int coord_off = 6 * i;
			bool showLine = Convert.ToBoolean (bool_vals [i * 2]);
			bool hasNormal = Convert.ToBoolean (bool_vals [i * 2 + 1]);
			VMObject vmobj = govmobj.GetComponent<VMObject> ();
            VMDimension vmdim = vmobj.coordinateSystem;
            Matrix4x4 worldToDirectView = Matrix4x4.identity;
            Matrix4x4 curCamTrans = Matrix4x4.identity;
			bool doNotSetAnnotation = false;
            if (vmobj.requestPlacementWithDirectView)
            {
                doNotSetAnnotation = (!((vmobj.whenShowInDirectPlacement && !vmobj.screenStabilized) || vmobj.placeWithDirectView)) || (vmobj.isFadingOut || vmobj.screenStabilized);
				worldToDirectView = vmobj.worldToDirectViewGet();
                curCamTrans = (camdiff.inverse * worldToDirectView).inverse;
                if (!doNotSetAnnotation)
                {
                    vmobj.isFadingOut = false;
                    vmobj.showingDirectViewPlacementSet(true); // not sure if it should be set to false, 
                                                               // since direct view is updated, it will be different
                }
            }
            else if (vmobj.whenShowInDirectPlacement)  // TODO: do not compute or report when whenShowInDirectPlacement
            {
                continue;
            } else {
				vmobj.showingDirectViewPlacementSet (false);
			}
            float minx = coords[coord_off], miny = coords[coord_off + 1], maxx = coords[coord_off + 2], maxy = coords[coord_off + 3];
            if (vmobj.screenStabilized && vmobj.showWhenNotVisible) {
                if (minx >= closestToEdgeRect[0] && miny >= closestToEdgeRect[1] &&
                    maxx <= closestToEdgeRect[2] && maxy <= closestToEdgeRect[3]){
                    // then unset screen stabilized
                    vmobj.screenStabilized = false;
                }
            }
            if (vmobj.isFadingOut || vmobj.screenStabilized) {
                if (vmobj.requestPlacementWithDirectView)  // if requesting, and fading out or screenStabilized, don't set annotation
                    doNotSetAnnotation = true;
                else
                    continue;
			}
            GameObject goannotation = vmobj.annotationGet();
			float centerx = (minx + maxx) / 2.0f, centery = (miny + maxy) / 2.0f;

			float zval = vmobj.constantZDistance;
			switch (vmobj.zMode) {
			case 0:
				zval = vmobj.constantZDistance;
				break;
			case 1:
				zval = coords [coord_off + 4]; // minz
				break;
			case 2:
				zval = coords [coord_off + 5]; // maxz
				break;
			case 3:
				zval = (coords [coord_off + 4] + coords [coord_off + 5]) / 2.0f; // centerz
				break;
			}
            float screenPtZ = zval;
			if (vmobj.zMode != 0) {
				Vector3 pt1 = invproj.MultiplyPoint (new Vector3 (0, 0, zval));
				zval = -pt1.z;
			} else {
                Vector3 pt1 = curCamera.projectionMatrix.MultiplyPoint(new Vector3(0, 0, -zval));
                screenPtZ = pt1.z; // set prevZval when constant, for screen z value
            }
            float sizey = maxy - miny;
			Vector2 normScreenPoint = new Vector2 (centerx, centery);

            if (goannotation.activeSelf && vmobj.showWhenNotVisible)
            {
                // check if this layout lays outside the rectangle
                if (minx < closestToEdgeRect[0] || miny < closestToEdgeRect[1] ||
                    maxx > closestToEdgeRect[2] || maxy > closestToEdgeRect[3])
                {
                    // then set to screen stabilized and continue
                    vmobj.screenStabilized = true;
                    continue;
                }
            }

			if (!doNotSetAnnotation) {
				if (!showLine) {
					vmobj.annotationlineGet ().SetActive (false);
					vmobj.setRecomputeLine (false);
				}
				vmobj.prevActive = goannotation.activeSelf;
				vmobj.SetAnnotationToActive ();
			}
			zvals [i] = zval;
            zvalsScreen[i] = screenPtZ;
            Vector3 worldPt;
            if (vmobj.requestPlacementWithDirectView)
				worldPt = DPUtils.ScreenToWorldPoint(curCamera.projectionMatrix, camdiff, worldToDirectView, 
                                                     curCamera.pixelWidth, curCamera.pixelHeight,
                                                     new Vector3(normScreenPoint.x, normScreenPoint.y, screenPtZ));
            else 
				worldPt = ViewManager.getWorldToTopMatrix().MultiplyPoint(curCamera.ScreenToWorldPoint(new Vector3(normScreenPoint.x, normScreenPoint.y, zval)));

            if (vmdim != null)
            {
                Vector3 origWorldPt = worldPt;
                worldPt = vmdim.dimToWorldGet().MultiplyPoint(worldPt);
            }
            float height = zval * 2.0f * Mathf.Tan (curCamera.fieldOfView / 2.0f * Mathf.Deg2Rad) / curCamera.pixelHeight;
			float ar = height * sizey / (vmobj.annotationBoundsOrig.size.y);
            bool shouldAnimate = false;
            if (vmobj.shouldInterpolate && vmobj.prevActive) {
                bool eq1 = DPUtils.equals(vmobj.endPoint, worldPt);
                bool eq2 = DPUtils.equals(vmobj.endScale, ar);
                shouldAnimate = !eq1 || !eq2;
            }
			Quaternion resultRot = new Quaternion();
			if (hasNormal) {
				Vector3 norm = new Vector3 (normals [normal_off++], normals [normal_off++], normals [normal_off++]);
				Vector3 up;
				if (vmobj.planeOrientedAxisAligned) {
					up = new Vector3 (0, 1, 0);
				} else {
                    if (vmobj.requestPlacementWithDirectView)
                        up = worldToDirectView.GetRotation() * Vector3.up;
                    else
						up = ViewManager.getWorldToTopMatrix().MultiplyPoint(curCamera.transform.up);
				}
				if (Vector3.Angle (norm, up) < 10) {
                    if (vmobj.requestPlacementWithDirectView)
                        up = worldToDirectView.GetRotation() * Vector3.back;
                    else
						up = ViewManager.getWorldToTopMatrix().MultiplyPoint(curCamera.transform.forward);
				}
				resultRot.SetLookRotation (-norm, up);
				if (!doNotSetAnnotation) {
					if (vmobj.shouldInterpolate && vmobj.prevActive && !DPUtils.equals (vmobj.endQuat, resultRot)) {
						vmobj.startQuat = vmobj.endQuat;
						shouldAnimate = true;
					} else {
						goannotation.transform.eulerAngles = resultRot.eulerAngles;
					}
					vmobj.endQuat = resultRot;
				}
			} else {
                //Quaternion rot = Quaternion.identity ;
                if (vmobj.requestPlacementWithDirectView)
					resultRot = worldToDirectView.inverse.GetRotation();
                else
					resultRot = ViewManager.getWorldToTopMatrix().GetRotation() * curCamera.transform.rotation;
				if (!doNotSetAnnotation) {
					if (vmobj.shouldInterpolate && vmobj.prevActive && !DPUtils.equals (vmobj.endQuat, resultRot)) {
						vmobj.startQuat = vmobj.endQuat;
						shouldAnimate = true;
					} else {
						goannotation.transform.rotation = resultRot;
					}
					vmobj.endQuat = resultRot;
				}
			}
			if (!doNotSetAnnotation) {
				if (shouldAnimate) {
					vmobj.isAnimating = true;
					vmobj.startPoint = vmobj.endPoint;
					vmobj.startScale = vmobj.endScale;
					vmobj.startQuat = resultRot;
					float curTime = Time.time + Time.smoothDeltaTime;
					vmobj.startTime = curTime;
					vmobj.duration = (vmobj.interpolateBetweenTimeMS / 1000.0f);
					vmobj.endTime = curTime + vmobj.duration;
				} else {
					goannotation.transform.localPosition = worldPt;
					goannotation.transform.localScale = new Vector3 (ar, ar, 1.0f);
				}
				vmobj.endPoint = worldPt;
				vmobj.endScale = ar;
			}
			if (vmobj.requestPlacementWithDirectView) { // set VMObject.directView info
				VMObject.DirectViewPlacement placement = new VMObject.DirectViewPlacement();
				placement.fromWorldPoint = ViewManager.getWorldToTopMatrix().MultiplyPoint(curCamera.transform.position);
				placement.worldPoint = worldPt;
				placement.lineShown = false;
				placement.rotation = resultRot;
				placement.scale = ar;
				placement.zdistance = zvals [i];
				vmobj.directViewPlacement = placement;
                if (!showLine)
                    vmobj.requestPlacementWithDirectView = false;
			}
        }
        for (int pl = 0; pl < int_vals2_len; pl+=4) {
			int off = int_vals2 [pl] / 2;  // divide by two since <place> is set by length of <all-show-line-for-results-list>,
										   // and we added a boolean for normals
			int off6 = off * 6;
			GameObject govmobj = vmobjs[int_vals[off]];
			VMObject vmobj = govmobj.GetComponent<VMObject> ();
            VMDimension vmdim = vmobj.coordinateSystem;

            bool doNotSetAnnotation = false;
			if (vmobj.requestPlacementWithDirectView) {
                doNotSetAnnotation = (!((vmobj.whenShowInDirectPlacement && !vmobj.screenStabilized) || vmobj.placeWithDirectView)) || (vmobj.isFadingOut || vmobj.screenStabilized);
            }
            else if (vmobj.isFadingOut || vmobj.screenStabilized)
            {
                continue;
            } else if (vmobj.whenShowInDirectPlacement)  // TODO: do not compute or report when whenShowInDirectPlacement
            {
                continue;
            }
            float minx = coords[off6], miny = coords[off6+1], maxx = coords[off6+2], maxy = coords[off6+3];
			float centerx = (minx + maxx) / 2.0f, centery = (miny + maxy) / 2.0f;
			float w = maxx - minx, h = maxy - miny;
			float w2 = w / 2.0f, h2 = h / 2.0f;
			int endx = int_vals2 [pl + 1], endy = int_vals2 [pl + 2];
			// clip to bounds around annotation in x and y
			float clipx = (endx - centerx), clipy = (endy - centery);

			if (clipx < -w2) { // left
				clipy = - clipy * w2 / clipx; //y = y0 + (y1 - y0) * (xmin - x0) / (x1 - x0);
				clipx = -w2;                  //x = xmin;
			} else if (clipx > w2) { // right
				clipy = clipy * w2 / clipx; //y = y0 + (y1 - y0) * (xmax - x0) / (x1 - x0);
				clipx = w2; //x = xmax;
			}

			if (clipy < -h2) { // bottom
				clipx = - clipx * h2 / clipy; //x = x0 + (x1 - x0) * (ymin - y0) / (y1 - y0);
				clipy = -h2; //y = ymin;
			} else if (clipy > h2) { // top
				clipx = clipx * h2 / clipy; //x = x0 + (x1 - x0) * (ymax - y0) / (y1 - y0);
				clipy = h2; //y = ymax;
			}
			clipx += centerx;
			clipy += centery;
			clipx = Mathf.Round(clipx);
			clipy = Mathf.Round(clipy);
			GameObject goannotationline = vmobj.annotationlineGet();

			float maxOutsideDistance = (float)int_vals2 [pl + 3];

			//			Vector2 lineVect = new Vector2 (centerx - endx, centery - endy);
			Vector2 lineVect = new Vector2 (endx - clipx, endy - clipy);
            Vector3 linept1 = Vector3.zero;
            if (vmobj.requestPlacementWithDirectView)
				linept1 = DPUtils.ScreenToWorldPoint(curCamera.projectionMatrix, camdiff, vmobj.worldToDirectViewGet(), 
                                                     curCamera.pixelWidth, curCamera.pixelHeight, new Vector3(endx, endy, zvalsScreen[off]));
            else
				linept1 = ViewManager.getWorldToTopMatrix().MultiplyPoint(curCamera.ScreenToWorldPoint(new Vector3(endx, endy, zvals[off])));
            if (vmdim != null)
            {
                Vector3 origlinept1 = linept1;
                linept1 = vmdim.dimToWorldGet().MultiplyPoint(linept1);
            }
            Vector3 linept1actual = linept1;
            bool shouldChange = true;
			float mag = lineVect.magnitude;
			float actualDist = mag;
			if (mag > maxOutsideDistance) {
				actualDist = maxOutsideDistance;
			} else {
				shouldChange = false;
			}
			if (shouldChange){
				Vector2 lineVectNorm = new Vector2 (endx - centerx, endy - centery);
				lineVectNorm.Normalize ();
                Vector2 newScreenPoint = lineVectNorm * actualDist;
                newScreenPoint.x += clipx; //endx;
				newScreenPoint.y += clipy; // endy;
                if (vmobj.requestPlacementWithDirectView)
					linept1actual = DPUtils.ScreenToWorldPoint(curCamera.projectionMatrix, camdiff, vmobj.worldToDirectViewGet(),
                                    curCamera.pixelWidth, curCamera.pixelHeight, 
                                    new Vector3(Mathf.Round(newScreenPoint.x), Mathf.Round(newScreenPoint.y), zvalsScreen[off]));
                else
					linept1actual = ViewManager.getWorldToTopMatrix().MultiplyPoint(curCamera.ScreenToWorldPoint (new Vector3 (Mathf.Round(newScreenPoint.x), Mathf.Round(newScreenPoint.y), zvals [off])));
			}

			// TODO: CHECK IF LINE IS LONGER THAN maxOutsideDistance, AND TRUNCATE IT IF IT IS
			vmobj.prevActiveLine = goannotationline.activeSelf;
			LineRenderer lr = goannotationline.GetComponent<LineRenderer> ();
            lr.positionCount = 2;
            float lineWidth = vmobj.outsideLineWidth * DPUtils.getScaleFromZDistance (zvals [off]);
            Vector3 linept2 = Vector3.zero;
            if (vmobj.requestPlacementWithDirectView)
				linept2 = DPUtils.ScreenToWorldPoint(curCamera.projectionMatrix, camdiff, vmobj.worldToDirectViewGet(),
                                    curCamera.pixelWidth, curCamera.pixelHeight,
                                    new Vector3(clipx, clipy, zvalsScreen[off]));
            else
                linept2 = ViewManager.getWorldToTopMatrix().MultiplyPoint(curCamera.ScreenToWorldPoint(new Vector3(clipx, clipy, zvals[off])));
            if (vmdim != null)
            {
                Vector3 origlinept2 = linept2;
                linept2 = vmdim.dimToWorldGet().MultiplyPoint(linept2);
            }
            if (!doNotSetAnnotation) {
				if (vmobj.shouldInterpolate) {
					if (vmobj.prevActiveLine) {
						vmobj.isAnimatingLine = true;
						vmobj.lineStartPoint1 = vmobj.lineEndPoint1;
						vmobj.lineStartPoint2 = vmobj.lineEndPoint2;
						vmobj.lineWidthStart = vmobj.lineWidthEnd;
						vmobj.lineWidthEnd = lineWidth;
					} else {
						if (vmobj.prevActive) {
							// if annotation was previously active (but not line, then line uses both starting points are at the 1st endpoint linept1
							vmobj.lineStartPoint1 = linept1;
							vmobj.lineStartPoint2 = linept1;
							vmobj.isAnimatingLine = true;
						} else {
							lr.positionCount = 2;
							lr.SetPositions (new Vector3[] { linept1actual, linept2 });
						}
						lr.startWidth = lr.endWidth = lineWidth;
						vmobj.lineWidthStart = vmobj.lineWidthEnd = lineWidth;
					}
				} else {
					lr.positionCount = 2;
					lr.SetPositions (new Vector3[] { linept1actual, linept2 });
					lr.startWidth = lr.endWidth = lineWidth;
				}
				vmobj.lineEndPoint1 = linept1actual;
				vmobj.lineEndPoint2 = linept2;
				goannotationline.SetActive (true);
				if (vmobj.scheduleMode == 3) {
					vmobj.setNoRecomputeLine (true);
					vmobj.setRecomputeLine (true);
				}
			}
			if (vmobj.requestPlacementWithDirectView) { // set VMObject.directView info
                vmobj.directViewPlacement.lineShown = true;
				vmobj.directViewPlacement.linePoint1 = linept1actual;
				vmobj.directViewPlacement.linePoint2 = linept2;
                vmobj.directViewPlacement.lineWidth = lineWidth;
				//Debug.Log ("vmobj.directViewPlacement: worldPoint=" + vmobj.directViewPlacement.worldPoint + " lineShown=true: linePoint1=" + vmobj.directViewPlacement.linePoint1 + 
				//		   " linePoint2=" + vmobj.directViewPlacement.linePoint2);
                vmobj.requestPlacementWithDirectView = false;
            }
		}
		MouseScriptImpl ms = FindObjectOfType<MouseScriptImpl> ();
		if (ms!=null)
			ms.clearCurrentMousePosition ();
	}
}
