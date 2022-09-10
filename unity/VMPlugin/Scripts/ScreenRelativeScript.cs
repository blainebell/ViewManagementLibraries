using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRelativeScript : MonoBehaviour {
    public bool adjustForImageSize = true;
	public Rect screenRelative;
	private float imageWidth = 1.0f, imageHeight = 1.0f;
	public void imageSizeSet(float w, float h){
		imageWidth = w;
		imageHeight = h;
	}
	// Use this for initialization
	void Start () {
	}

    // Update is called once per frame
	void Update() {
		computePlacement();
	}
	void computePlacement() {
		Camera cam = ViewManager.getCurrentCamera();
		float camPixelWidth = cam.pixelWidth;
		float camPixelHeight = cam.pixelHeight;
		float camPixelWidth2 = camPixelWidth / 2.0f;
		float camPixelHeight2 = camPixelHeight / 2.0f;

		Rect screenPixelRect = new Rect(camPixelWidth * screenRelative.x, camPixelHeight * screenRelative.y,
						camPixelWidth * screenRelative.width, camPixelHeight * screenRelative.height);

		if (adjustForImageSize)
		{
			float imgratio = imageWidth / imageHeight;
			float tmpS, tmpSize;

            if ((tmpS = screenPixelRect.width / imgratio) < screenPixelRect.height)
            {
                /* size is bound by width */
                tmpSize = tmpS;
            }
            else
            {
                /* size is bound by height */
                tmpSize = screenPixelRect.height;
            }

            int dw, dh;
            dw = (int)(.5f * Mathf.Round(tmpSize * imgratio));
            dh = (int)(.5f * Mathf.Round(tmpSize));
            gameObject.transform.localScale = new Vector3(dw, dh, 1.0f);
            gameObject.transform.localRotation = Quaternion.identity;
            Vector2 center = screenPixelRect.center;
            gameObject.transform.localPosition = new Vector3(center.x - camPixelWidth2, center.y - camPixelHeight2, 0.0f);
        } else
        {
            gameObject.transform.localScale = new Vector3(camPixelWidth, camPixelHeight, 1.0f);
            gameObject.transform.localRotation = Quaternion.identity;
            Vector2 center = screenPixelRect.center;
            gameObject.transform.localPosition = new Vector3(center.x - camPixelWidth2, center.y - camPixelHeight2, 0.0f);
        }
    }
}
