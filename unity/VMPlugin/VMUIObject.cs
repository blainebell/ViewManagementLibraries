using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class VMUIObject : MonoBehaviour {
	public bool blockViewManagerSpace = true;
	public bool manualBlock = false;

	public bool isAdded = false;
//	public Canvas canvas;
	// Use this for initialization
	void Start () {
	}
	public Rect calculateRect(){
		RectTransform rt = GetComponent<RectTransform> ();
		Rect rect = DPUtils.GetRectTransformScreenBounds (rt);

		string namestr = gameObject.name;
		Rect rect2 = new Rect (rt.rect.x + rt.transform.position.x, rt.rect.y + rt.transform.position.y, rt.rect.width, rt.rect.height);
		Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds (rt.transform);
		Debug.Log ("calculateRect: VMUIObject name=" + namestr + " rect=" + rect + " rect2=" + rect2 + " bounds=" + bounds);

		return rect;
	}
	public void check(bool enab){
		if (enab != isAdded) {
			if (enab) {
				//Debug.Log ("VMUIObject.check: enab=" + enab);
				//calculateRect ();
				RectTransform rt = GetComponent<RectTransform> ();
				Rect rect = DPUtils.GetRectTransformScreenBounds (rt);
				string goname = gameObject.name;
				ScreenButtonsImpl sbs = FindObjectOfType<ScreenButtonsImpl> ();
				if (sbs!=null) sbs.addPermanentRect(GetInstanceID (), rect);
			} else {
				ScreenButtonsImpl sbs = FindObjectOfType<ScreenButtonsImpl> ();
				if (sbs!=null){
					int insid = GetInstanceID ();
					sbs.removePermanentRect (insid);
				}
			}
			isAdded = enab;
		}
	}
	int frameEnabled = 0;
	bool shouldCheck = false;
	void Update() {
		if (manualBlock)
			return;
		if (enabled != isAdded) {
			// NEED TO WAIT A FRAME TO GET BOUNDS
			if (enabled) {
				int curFrame = Time.frameCount - 2;
				if (shouldCheck && frameEnabled < curFrame) {
					check (enabled);
					shouldCheck = false;
				}
			} else {
				check (false);
			}
		}
	}

	void OnDisable() {
		if (!manualBlock)
			check (false);
	}

	void OnEnable() {
		if (!manualBlock) {
			// NEED TO WAIT A FRAME TO GET BOUNDS
			int curFrame = Time.frameCount;
			shouldCheck = true;
			frameEnabled = curFrame;
		}
	}
	void OnDestroy(){
		check (false);
	}

}
