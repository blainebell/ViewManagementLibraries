using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MouseScriptImpl : MonoBehaviour {
	public abstract void clearCurrentMousePosition();
	public abstract void setRotatePoint(Vector3 rotp);
	public abstract bool getIsRotating();
}
