using UnityEngine;
using System.Collections;

public class WorldToScreenAnimationScript : MonoBehaviour {
	public Vector3 screenDestination;
	public float startTime, endTime;
	public Quaternion startQuat;//, endQuat;
	public bool endQuatIsInvCamera;
	public Vector3 startPosition;
	public Vector3 positionOffset = new Vector3();
	public Vector3 startScale, endScale;
	public bool isAtEnd = false;
	public float rotateScale = .01f;
	public int rotationAfterSelectedBehavior = 0;
	bool finished = false;
	public ShoppingProduct sp;
	MouseScript ms;
	public void setMouseScript (MouseScript m){
		ms = m;
	}
	// Use this for initialization
	void Start () {
		// change order so this object can be seen always be in front of panel
		//MeshRenderer [] allmrend = GetComponentsInChildren<MeshRenderer>();
		//Shader sh = Shader.Find ("Custom/StandardNew");
/*		Shader sh = Shader.Find ("Custom/RenderLast");
		foreach (MeshRenderer mr in allmrend) {
			Material mat = mr.material;
			if (mat!=null) {
				mr.sortingLayerName = "Last";
				mat.shader = sh;
			}
		}*/

/*		int layerID = 0;
		if ((layerID = LayerMask.NameToLayer ("Last")) >= 0){
			gameObject.layer = layerID;
		}
		*/
	}
	void OnDestroy(){
		if (finished) {
			ScreenButtonsScript sbs = FindObjectOfType<ScreenButtonsScript> ();
			if (sbs.currentProductAnimation == this) {
				sbs.currentProductAnimation = null;
			}
			sbs.itemCanvas.turnCanvas (false);
			//sbs.itemCanvas.SetActive (false);
			sbs.allPermanentRects.Remove(GetInstanceID());
			if (ms != null) {
				ms.unsetIf (this);
			}
			if (sp != null) {
				if (sp.prefabForClick != null) {
					sp.prefabForClick.SetActive (true);
				}
			}
			ItemCanvasScript ics = FindObjectOfType<ItemCanvasScript> ();
			ics.SetShoppingProduct (null);
		}
	}
	float rotateDegrees = 0.0f;
	// Update is called once per frame
	void Update () {
		bool rotate = rotationAfterSelectedBehavior == 0;
		float curTime = Time.time + Time.smoothDeltaTime;
		//float curTime = startTime;
		Vector3 rotAroundPt;
		float interp = 0.0f;
		Camera curCamera = ViewManager.getCurrentCamera ();
		if (endTime <= curTime) {
			// at end, just place position at screenDestination for now
			rotAroundPt = (endScale.x * positionOffset);
			transform.position = curCamera.ScreenToWorldPoint (screenDestination);
			transform.localScale = endScale;
			if (!finished) {
				ScreenButtonsScript sbs = FindObjectOfType<ScreenButtonsScript> ();
//				sbs.itemCanvas.SetActive (true);
				ItemCanvasScript ics = FindObjectOfType<ItemCanvasScript> ();
				ics.SetShoppingProduct (sp);
				ics.turnCanvas (true);
				RectTransform rt = GameObject.Find("AddToCartButton").GetComponent<RectTransform> ();
				Rect rect = DPUtils.GetRectTransformScreenBounds (rt);
				//Rect rect = new Rect (rt.rect.x + rt.transform.position.x, rt.rect.y + rt.transform.position.y,rt.rect.width, rt.rect.height);
				sbs.allPermanentRects.Add (GetInstanceID (), rect);
				finished = true;
			}
			if (rotationAfterSelectedBehavior == 2) {
				rotate = true;
			}
		} else {
			Vector3 direction = screenDestination - transform.position;
			float distance = direction.magnitude;
			direction.Normalize ();
			float duration = (endTime - startTime);
			interp = (curTime - startTime) / duration;
			Vector3 scale = Vector3.Lerp (startScale, endScale, interp);
			transform.localScale = scale;
			rotAroundPt = scale.x * positionOffset;
			transform.position = curCamera.ScreenToWorldPoint (Vector3.Lerp (startPosition, screenDestination, interp));
		}
		transform.localRotation = Quaternion.identity;
		if (rotate) {
			rotateDegrees += Time.deltaTime / rotateScale;
			transform.Rotate (curCamera.transform.up, rotateDegrees);
		}
		Quaternion endQuat;
		if (endQuatIsInvCamera) {
			endQuat = curCamera.transform.rotation;
		} else {
			endQuat = Quaternion.identity;
		}
		if (endTime <= curTime) {
			transform.Rotate( endQuat.eulerAngles);
		} else {
			transform.Rotate( Quaternion.Lerp (startQuat, endQuat, interp).eulerAngles);
		}
		transform.Translate (rotAroundPt);
	}
}
