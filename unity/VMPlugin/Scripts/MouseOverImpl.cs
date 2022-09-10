using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MouseOverImpl : MonoBehaviour {
	public abstract Vector3 viewNormalGet();
	public abstract Vector3 [] closestFromVectorGet ();
	public abstract Vector3 [] viewNormalForFromVectorGet ();
	public abstract bool hasViewAngleGet ();
	public abstract bool zoomIntoGet();
	public abstract bool getClosestViewNormalCheck(Vector3 vect, out Vector3 outViewForward);
	public abstract MouseOverImpl linkViewObjectGet ();
}
