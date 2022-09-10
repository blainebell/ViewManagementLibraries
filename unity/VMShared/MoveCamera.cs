// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
// for using the mouse displacement for calculating the amount of camera movement and panning code.

using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour 
{
    public Transform controlledTransform;
    public Transform getControlledTransform()
    {
        if (controlledTransform != null)
            return controlledTransform;
        return transform;
    }

    //
    // VARIABLES
    //

    public float turnSpeed = 4.0f;		// Speed of camera turning when mouse moves in along an axis
	public float panSpeed = 4.0f;		// Speed of the camera when being panned
	public float zoomSpeed = 4.0f;		// Speed of the camera going back and forth

	public bool layoutWhenMouseRelease = false;
	public bool alwaysRotate = false;

	private Vector3 mouseOrigin;	// Position of cursor when mouse dragging starts
	private bool isPanning;		// Is the camera being panned?
	private bool isRotating;	// Is the camera being rotated?
	private bool isZooming;		// Is the camera zooming?

	//
	// UPDATE
	//
	//
	void Update () 
	{
		ScreenButtonsScript sbs = FindObjectOfType<ScreenButtonsScript> ();
		bool skippress = false; 
		if (sbs != null && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))) {
			skippress = sbs.isInsideRect(Input.mousePosition.x, Input.mousePosition.y);
		}
		// Disable movements on button release
		bool mouseIsReleased = false;
		// Get the left mouse button
		if (!alwaysRotate) {
			if (!skippress && Input.GetMouseButtonDown (0)) {
				// Get mouse origin
				mouseOrigin = Input.mousePosition;
				isRotating = true;
			}
			// Get the right mouse button
			if (!alwaysRotate && !skippress && Input.GetMouseButtonDown (1)) {
				// Get mouse origin
				mouseOrigin = Input.mousePosition;
				isPanning = true;
			}
			// Get the middle mouse button
			if (!alwaysRotate && !skippress && Input.GetMouseButtonDown (2)) {
				// Get mouse origin
				mouseOrigin = Input.mousePosition;
				isZooming = true;
			}
			if (!Input.GetMouseButton (0) && isRotating) {
				isRotating = false; mouseIsReleased = true;
			}
			if (!Input.GetMouseButton (1) && isPanning) {
				isPanning = false; mouseIsReleased = true;
			}
			if (!Input.GetMouseButton (2) && isZooming) {
				isZooming = false; mouseIsReleased = true;
			}
		} else {
			Camera cam = ViewManager.getCurrentCamera ();
			mouseOrigin = new Vector2 (cam.pixelWidth / 2, cam.pixelHeight / 2);
			isRotating = true;
		}
		if (layoutWhenMouseRelease && mouseIsReleased) {
			DPManagerScript.Call_noargs ("Do-All-Manual-Placements-map");
		}
		// Rotate camera along X and Y axis
		if (isRotating)
		{
			Vector3 pos = ViewManager.getCurrentCamera().ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            Transform trans = getControlledTransform();
            trans.RotateAround(trans.position, trans.right, -pos.y * turnSpeed);
			trans.RotateAround(trans.position, Vector3.up, pos.x * turnSpeed);
		}

		// Move the camera on it's XY plane
		if (isPanning)
		{
			Vector3 pos = ViewManager.getCurrentCamera().ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
			Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
            getControlledTransform().Translate(move, Space.Self);
//			Debug.Log ("MoveCamera: isPanning: Input.mousePosition=" + Input.mousePosition + " mouseOrigin=" + mouseOrigin + " pos=" + pos);
		}

		// Move the camera linearly along Z axis
		if (isZooming)
		{
			Vector3 pos = ViewManager.getCurrentCamera().ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            Transform trans = getControlledTransform();
            Vector3 move = pos.y * zoomSpeed * trans.forward; 
			trans.Translate(move, Space.World);
		}
		Transform trans2 = getControlledTransform();
		VMObject vmo = trans2.gameObject.GetComponent<VMObject> ();
		if (vmo != null)
			vmo.reloadGeometry ();
	}
}
