using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MouseOverScript : MouseOverImpl {
	public Vector3 viewNormal;
	public Vector3 [] closestFromVector;
	public Vector3 [] viewNormalForFromVector;
	public bool hasViewAngle;
	public float viewAngle;
	public bool zoomInto;
	public MouseOverScript linkViewObject;

	public override Vector3 viewNormalGet(){
		return viewNormal;
	}
	public override Vector3 [] closestFromVectorGet(){
		return closestFromVector;
	}
	public override Vector3 [] viewNormalForFromVectorGet(){
		return viewNormalForFromVector;
	}
	public override bool hasViewAngleGet(){
		return hasViewAngle;
	}
	public override bool zoomIntoGet(){
		return zoomInto;
	}
	public override MouseOverImpl linkViewObjectGet(){
		return linkViewObject;
	}
	public int numberOfViewNormalsFromVector(){
		return Math.Min (closestFromVector.Length, viewNormalForFromVector.Length);
	}

	public override bool getClosestViewNormalCheck(Vector3 vect, out Vector3 outViewForward){
		int nviewNormals = numberOfViewNormalsFromVector();
		if (nviewNormals <= 0) {
			outViewForward = viewNormal;
		} else {
			return getClosestViewNormal (vect, out outViewForward);
		}
		return outViewForward.magnitude >= 0;
	}
	public bool getClosestViewNormal(Vector3 vect, out Vector3 outViewForward){
		int closestViewNormal = -1;
		float closestAngle = float.MaxValue;
		int nviewNormals = numberOfViewNormalsFromVector();
		for (int pl = 0; pl < nviewNormals; pl++) {
			float ang = Vector3.Angle (vect, closestFromVector[pl]);
			if (ang < closestAngle) {
				closestViewNormal = pl;
				closestAngle = ang;
			}
		}
		outViewForward = viewNormalForFromVector [closestViewNormal];
        outViewForward.Normalize();

        return true; //useViewNormal = true;
	}
	public bool getClosestViewNormalIdx(Vector3 vect, out int outViewForwardIdx){
		int closestViewNormal = -1;
		float closestAngle = float.MaxValue;
		int nviewNormals = numberOfViewNormalsFromVector();
		for (int pl = 0; pl < nviewNormals; pl++) {
			float ang = Vector3.Angle (vect, closestFromVector[pl]);
			if (ang < closestAngle) {
				closestViewNormal = pl;
				closestAngle = ang;
			}
		}
		outViewForwardIdx = closestViewNormal;
		return true;
	}
	public void getViewNormal(out bool useVect, out Vector3 viewNormalArg)
	{
		useVect = false;
		bool useViewNormal = false;
		viewNormalArg = viewNormal;
		if ((numberOfViewNormalsFromVector()) > 0)
		{
			useViewNormal = getClosestViewNormal(ViewManager.getCurrentCamera().transform.forward, out viewNormalArg);
		}

		if (!useViewNormal)
		{
			if (viewNormalArg.magnitude < DPUtils.TOLER)
			{
				if (hasViewAngle)
				{
					viewNormalArg = DPUtils.GetVectorAtAngleFrom(ViewManager.getCurrentCamera().transform.forward, viewAngle);
					useVect = true;
				}
			}
			else
			{
				useVect = true;
			}
		}
	}

	public void changed(){
		#if UNITY_EDITOR
		if (VMObjectEditor.currentEditor != null && VMObjectEditor.currentEditor.target!=null) {
			EditorUtility.SetDirty (VMObjectEditor.currentEditor.target);
			if (!Application.isPlaying)
				UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
		}
		#endif
	}
}
