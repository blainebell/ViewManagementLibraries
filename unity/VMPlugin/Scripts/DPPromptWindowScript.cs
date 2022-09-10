#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class DPPromptWindowScript : EditorWindow
{
	static DPManagerScript dpManager = null;
	string textArea = "";
	string textInput = "";
	Vector2 scrollView;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/DP Prompt")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(DPPromptWindowScript));
		if (dpManager == null) {
			dpManager = (DPManagerScript)FindObjectOfType (typeof(DPManagerScript));
			Debug.Log ("dpManager=" + dpManager);
		}
	}

	void AddToTextArea(string str){
		string strt = str.Trim ();
		if (strt.Length > 0) {
			textArea += strt + "\n";
			Repaint ();
		}
	}
	void OnGUI()
	{
		Event e = Event.current;
		bool textAdded = false;
		string cmds = null;
		if (e.keyCode == KeyCode.Return) {
			if (e.type == EventType.KeyUp) {
				if (e.shift && textInput.Trim ().Length > 0) { 	// I'd rather have shift add the line in textInput, and return
//					Debug.Log ("textInput=" + textInput);		// by default enter the text, but Unity is quite difficult
					cmds = textInput;
					textInput = "";
					textAdded = true;
				}
			}
		}

		GUILayout.Label ("DP Command Prompt", EditorStyles.boldLabel);
		if (GUILayout.Button ("Clear", new GUILayoutOption[] { GUILayout.MaxWidth(100.0f) } )) {
			textArea = "";
		}
		bool isUp = DPManagerScript.DP_IsUp ();
		if (isUp) {
			GUILayout.Label ("DP Status: Running/Available");
		} else {
			GUILayout.Label ("DP Status: Not Available, currently only in game mode");
		}
		scrollView = EditorGUILayout.BeginScrollView(scrollView, false, false);
		GUI.enabled = true; // false; need to cut/paste from this window
		EditorGUILayout.TextArea (textArea, new GUILayoutOption[] { GUILayout.ExpandHeight(true) });
		if (isUp)
			GUI.enabled = true;
		EditorGUILayout.EndScrollView();
		GUI.SetNextControlName("MyTextField");
		textInput = EditorGUILayout.TextArea (textInput, new GUILayoutOption[] { GUILayout.MinHeight (40.0f) });
		if (!isUp)
			GUI.enabled = true;
		
		if (textAdded){
			GUI.FocusControl ("MyTextField");
			EditorGUI.FocusTextInControl ("MyTextField");
			Repaint ();
		}
		if (cmds != null) {
			if (cmds.IndexOf ('\t') < 0) {
				cmds = cmds.Replace (' ', '\t');
			}
			char[] delimiterChars = { '\r', '\n' };
			string[] cmdlines = cmds.Split(delimiterChars );
			DPManagerScript.Log_Callback = AddToTextArea;
			foreach (string s in cmdlines){
				if (s.Trim().Length > 0) {
					textArea += "DP > " + s.Trim () + "\n";
					DPManagerScript.DPCall_line (s);
				}
			}
			DPManagerScript.Log_Callback = null;
			Repaint ();
//			Debug.Log ("END: textArea='" + textArea + "'");
		}
	}
	void OnLostFocus() {
//		Debug.Log ("OnLostFocus called");
	}
	void Update () {
//		Debug.Log ("enterPressed=" + enterPressed);
	}

}

#endif
