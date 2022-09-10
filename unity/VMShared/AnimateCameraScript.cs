using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AnimateCameraScript : AnimateCameraImpl {
	public Transform controlledTransform;
    public Transform eyeTransform;
    public override Transform getEyeTransform(){
        return eyeTransform;
    }
    public override Transform getEyeOrControlledTransform()
    {
        if (eyeTransform != null)
            return eyeTransform;
        return getControlledTransform();
    }
    public override Transform getControlledTransform()
	{
	    if (controlledTransform != null)
	        return controlledTransform;
	    if (currentCamera!=null)
	        return currentCamera.transform;
		Camera curCamera = ViewManager.getCurrentCamera ();
		if (curCamera != null)
			return curCamera.transform;
	    return null;
	}
	public bool controllingWorld = false;
	override public bool isControllingWorld (){
		return controllingWorld;
	}
	Camera currentCamera ;
	// Use this for initialization
	void Start () {
		currentCamera = ViewManager.getCurrentCamera();
	}
	bool isAnimating = false;
	public bool getIsAnimating(){
		return isAnimating;
	}
	class AnimationLeg {
		public Vector3 startPosition, endPosition;
		public float startTime, endTime;
		public bool useQuat = false;
		public Quaternion startQuat, endQuat;

		public bool useLookAt = false;
		public Vector3 startLookAt, endLookAt;

		public float duration;
		public bool isInverse = false;
	};

	AnimationLeg [] animationLegs = null;
	int currentAnimationLegIdx = -1;
	public float defaultAnimationTime = 2.0f;
	public override float getDefaultAnimationTime(){
		return defaultAnimationTime;
	}
	public float duration = 2.0f;
	float currentDuration = 2.0f;
	public override bool animateCameraToPositionWithDuration(Vector3 pos, float dur){
        return animateCameraToWithDuration(pos, false, new Vector3(), dur, false);
    }
    public override bool animateCameraToPosition(Vector3 pos){
        return animateCameraTo(pos, false, new Vector3(), false);
    }
    public override bool animateCameraTo(Vector3 pos, bool useQuat, Vector3 lookAt, bool inv){
        return animateCameraToWithDuration(pos, useQuat, lookAt, getDefaultAnimationTime(), inv);
    }
	public override bool animateCameraToWithDuration(Vector3 pos, bool useQuat, Vector3 lookAt, float dur, bool inv){
        currentDuration = dur;
		float curTime = Time.time + Time.smoothDeltaTime;
		AnimationLeg al = new AnimationLeg ();
		al.startTime = curTime;
		al.endTime = al.startTime + currentDuration;
		al.duration = currentDuration;
		al.useQuat = useQuat;
		al.useLookAt = false;
		al.isInverse = inv;
		if (inv) {
		    if (eyeTransform != null) {
			    al.startPosition = -(Quaternion.Inverse(eyeTransform.rotation) * eyeTransform.position);
			    al.startQuat = Quaternion.Inverse(eyeTransform.rotation);
		    }
		    else {
			    al.startPosition = -(Quaternion.Inverse(getControlledTransform().rotation) * getControlledTransform().position);
			    al.startQuat = Quaternion.Inverse(getControlledTransform().rotation);
		    }
		} else {
		    if (eyeTransform != null)
			{
			    al.startPosition = eyeTransform.position;
			    al.startQuat = eyeTransform.rotation;
			}
		    else
			{
			    al.startPosition = getControlledTransform().position;
			    al.startQuat = getControlledTransform().rotation;
			}
		}
		al.endPosition = pos;
		al.endQuat = Quaternion.LookRotation(lookAt - pos, Vector3.up);

		if (DPUtils.equals(al.startPosition, al.endPosition) && DPUtils.equals(al.startQuat, al.endQuat)) {
			finishAnimating ();
		} else {
			isAnimating = true;
			animationLegs = new AnimationLeg[1] { al };
			currentAnimationLegIdx = 0;
		}
		return isAnimating;
	}
    public override bool animateCameraAcrossPath(Vector3[] pointsInPath, Vector3[] allViewVectors, bool[] lookAtNext)
    {
		currentDuration = 5.0f;
		float curTime = Time.time + Time.smoothDeltaTime;
		float startTime = curTime + .25f;
		Vector3 startPosition = getControlledTransform().position;
		Quaternion startQuat = getControlledTransform().rotation;
		List<AnimationLeg> animationLegsList = new List<AnimationLeg> ();

		List<Vector3> pointsInPathList = new List<Vector3> ();
		List<Vector3> allViewVectorsList = new List<Vector3> ();
		List<bool> lookAtNextList = new List<bool> ();
		Vector3 prevpos = getControlledTransform().position;
		for (int pl = 0; pl < pointsInPath.Length; pl++) {
			Vector3 pos = pointsInPath [pl];
			Vector3 dvect = (pos - prevpos);
			if (allViewVectors[pl].magnitude < .1)
				continue;
			float angvec = Vector3.Angle (dvect, Vector3.down);
			//Debug.Log ("pl=" + pl + " dvect=" + dvect + " angvec=" + angvec);
			if (angvec < 10.0f || angvec > 170.0f) {
				//Debug.Log ("continuing");
				continue; // looking down
			}
			//Debug.Log ("Adding #" + pl);
			pointsInPathList.Add (pointsInPath [pl]);
			allViewVectorsList.Add (allViewVectors [pl]);
			lookAtNextList.Add (lookAtNext [pl]);
			prevpos = pos;
		}
		//Debug.Log ("pointsInPathArg.Length=" + pointsInPathArg.Length + " pointsInPathList.Count=" + pointsInPathList.Count);
		Vector3 [] pointsInPathArr = new Vector3[pointsInPathList.Count];
		pointsInPathList.CopyTo (pointsInPathArr, 0);
		Vector3[] allViewVectorsArr = new Vector3[allViewVectorsList.Count];
		allViewVectorsList.CopyTo (allViewVectorsArr, 0);
		bool[] lookAtNextArr = new bool[lookAtNextList.Count];
		lookAtNextList.CopyTo (lookAtNextArr, 0);

		prevpos = getControlledTransform().position;
		for (int pl = 1; pl < pointsInPathArr.Length; pl++) {
			prevpos = pointsInPathArr [pl-1];
			Vector3 pos = pointsInPathArr [pl];
			float angvec = Vector3.Angle (allViewVectorsArr[pl-1], Vector3.down);
			if (angvec < 10.0f){
				Vector3 vv = pos - prevpos;
				allViewVectorsArr [pl - 1] = vv.normalized;
			}
		}
		float completeDistance = 0.0f;
		prevpos = getControlledTransform().position;
		for (int pl = 0; pl < pointsInPathArr.Length; pl++) {
			Vector3 pos = pointsInPathArr [pl];
			completeDistance += (pos - prevpos).magnitude;
			prevpos = pos;
		}
		prevpos = getControlledTransform().position;
		bool isLast = false;
		string logStr = "#legs=" + pointsInPath.Length + " currentDuration=" + currentDuration;
		for (int pl = 0; pl < pointsInPathArr.Length; pl++) {
			isLast = ((pl+1) >= pointsInPathArr.Length);
			Vector3 pos = pointsInPathArr [pl];

			float dist = (pos - prevpos).magnitude;
			AnimationLeg al = new AnimationLeg ();
			al.startPosition = startPosition;
			al.startTime = startTime;
			float legduration = currentDuration / 2.0f;
			al.endTime = al.startTime + legduration;
			al.duration = legduration;
			al.endPosition = pos;

			al.useQuat = !lookAtNextArr[pl] || isLast;
			al.startQuat = startQuat;
			Vector3 lookAt = pointsInPathArr[pl] + allViewVectorsArr[pl].normalized;
            Vector3 forw = allViewVectorsArr[pl].normalized; //  lookAt - al.endPosition;
            logStr += " point#" + pl + "  : point: " + pointsInPathArr[pl] + " viewVect: " + allViewVectorsArr[pl] + " forw: " + forw + "\n";

            if (forw.magnitude > DPUtils.TOLER)
			{
                al.endQuat = Quaternion.LookRotation(forw, Vector3.up);
            } else
			{
                al.endQuat = al.startQuat;
			}
            al.useLookAt = !isLast && lookAtNextArr[pl];
            logStr += "     lookAt: "+ lookAt + " endPosition: " + al.endPosition + " forw: " + forw + " startQuat: " + al.startQuat + " endQuat: " + al.endQuat + "  useLookAt: " + al.useLookAt + "\n" ;

            if (al.useLookAt) {
				float pdist = (pointsInPathArr [pl + 1] - startPosition).magnitude;
				Vector3 fdir = startQuat * Vector3.forward;
				al.startLookAt = startPosition + pdist*fdir;
				al.endLookAt = pointsInPathArr [pl + 1];
			}
			animationLegsList.Add (al);
			startTime = al.endTime;
			startPosition = al.endPosition;
			startQuat = al.endQuat;
			prevpos = pos;
		}
        //Debug.Log(logStr);
		animationLegs = animationLegsList.ToArray ();
		isAnimating = animationLegs.Length > 0;
		currentAnimationLegIdx = 0;
		return true;
	}
	private bool shouldExecuteCommandAfterAnimation = false;
	private string executeCommandAfterAnimation = "";
    public override void setShouldExecuteCommandAfterAnimation(bool s)
    {
		shouldExecuteCommandAfterAnimation = s;
	}
    public override void setExecuteCommandAfterAnimation(string s)
    {
		executeCommandAfterAnimation = s;
		shouldExecuteCommandAfterAnimation = s.Length > 0;
	}
	private bool shouldExecuteViewManagementAfterAnimation = false;
    public override void setShouldExecuteViewManagementAfterAnimation(bool s){
		shouldExecuteViewManagementAfterAnimation = s;
	}
	bool tryToRotateAfterAnimation = false;
	public void setTryToRotateAfterAnimation(bool t){
		tryToRotateAfterAnimation = t;
	}
	void finishAnimating(){
		//Debug.Log ("AnimateCameraScript: finishAnimating : shouldExecuteViewManagementAfterAnimation=" + shouldExecuteViewManagementAfterAnimation + " tryToRotateAfterAnimation=" + tryToRotateAfterAnimation);
		MouseScript ms = FindObjectOfType<MouseScript>();
		//if (ms == null)
		//	return;
		VMDimension curDim = null;
		if (ms!=null)
			curDim = ms.currentDimensionGet();
		if (curDim!=null && curDim.nextWhenZoomedToOnNextGet()){
		    curDim.nextWhenZoomedToOnNextSet(false);
		    DPManagerScript.Call_s_s_s_s_b("SetDataValue-map", "UnityMain", "do-not-change-view", "uid", "main-uid", false);
			if (ms!=null)
			    ms.nextPreviousButtonPressed(0);
		    return;
		} else {
			if (shouldExecuteCommandAfterAnimation) {
				DPManagerScript.Call_noargs (executeCommandAfterAnimation);
			}
			if (shouldExecuteViewManagementAfterAnimation) {
				ViewManager vm = FindObjectOfType<ViewManager> ();
				vm.traverseNextFrame = true;
				DPManagerScript.Call_s_s_s_s_b ("SetDataValue-map", "UnityMain", "do-not-change-view", "uid", "main-uid", true);
				DPManagerScript.Call_noargs ("SetupStillView-From-Last-map");
				DPManagerScript.Call_noargs ("Set-Should-Report-Labels-Not-Shown-In-First-CoordinateSystem-map");
				shouldExecuteViewManagementAfterAnimation = false;
				if (tryToRotateAfterAnimation) {
					if (ms != null) {
						if (!ms.isRotating) {
							//Debug.Log ("ms.isRotating=" + ms.isRotating);
							ms.isRotating = true;
						}
						ms.turnRotateOffTmp = false;
					}
					tryToRotateAfterAnimation = false;
				}
			}
			DPManagerScript.Call_b ("Do-All-Annotation-Placements-In-Traversed-CoordinateSystems-map", true);
		}
		animationLegs = null;
		currentAnimationLegIdx = -1;
	}
    void UpdateControlledTransformMat(float interp, Matrix4x4 mat)
    {
        UpdateControlledTransform(interp, mat.GetPosition(), mat.GetRotation());
    }
    void UpdateControlledTransform(float interp, Vector3 pos, Quaternion quat)
    {
        Vector3 dir = quat * Vector3.forward;
        //MoveTo(pos);
        //RotateTo(dir);
        getControlledTransform().SetPositionAndRotation(pos, quat);
    }
    Vector3 savedVector = Vector3.forward;
    void RotateToSaved()
    {
        savedVector.y = 0.0f;
        savedVector.Normalize();
        RotateTo(savedVector);
    }
    void MoveTo(Vector3 moveTo)
    {
        if (eyeTransform != null)
        {
            Vector3 moveV = moveTo - eyeTransform.position;
            moveV = Quaternion.Inverse(getControlledTransform().localRotation) * moveV;
            getControlledTransform().Translate(moveV);
        }
        else
        {
            getControlledTransform().Translate(moveTo - getControlledTransform().position);
        }
    }
    void RotateTo(Vector3 vect)
    {
        Vector3 userDir;
        Vector3 userPos;
        if (eyeTransform != null)
        {
            userDir = eyeTransform.forward;
            userPos = eyeTransform.position - getControlledTransform().position;
        }
        else
        {
            userDir = getControlledTransform().forward;
            userPos = getControlledTransform().position;
        }
        userDir.y = 0.0f;
        userDir.Normalize();
        vect.y = 0.0f;
        vect.Normalize();
        float angle = Vector3.Angle(userDir, vect);
        Vector3 cross = Vector3.Cross(userDir, vect);
        if (cross.y < 0)
            angle = -angle;
        getControlledTransform().RotateAround(userPos, Vector3.up, angle);
    }
    void RotateUser(Vector3 axis, float angle)
    {
        if (eyeTransform != null)
        {
            Transform trans = getControlledTransform();
            getControlledTransform().RotateAround(eyeTransform.position, axis, angle);
            savedVector = eyeTransform.forward;
            Debug.Log("savedVector=" + savedVector);
        }
        else
        {
            getControlledTransform().Rotate(Quaternion.AngleAxis(angle, axis).eulerAngles);
        }
    }
    void MoveUser(Vector3 moveV)
    {
        if (eyeTransform != null)
        {
            moveV = Quaternion.Inverse(getControlledTransform().localRotation) * eyeTransform.rotation * moveV;
            getControlledTransform().Translate(moveV);
        }
        else
        {
            getControlledTransform().Translate(moveV);
        }
    }
    void interpFunc(float interp, AnimationLeg al)
    {
        if (interp > 0.0f)
        {
            if (al.isInverse)
            {
                Vector3 curCenterPos = Vector3.Lerp(al.startPosition, al.endPosition, interp);
                Quaternion curRot = getControlledTransform().rotation; // Quaternion.identity;
                if (al.useQuat)
                    curRot = Quaternion.Lerp(al.startQuat, al.endQuat, interp);
                if (al.useLookAt)
                    curRot = Quaternion.LookRotation(Vector3.Lerp(al.startLookAt, al.endLookAt, interp) - curCenterPos, Vector3.up);
                Matrix4x4 mat = Matrix4x4.identity;
                mat.SetTRS(curCenterPos, curRot, Vector3.one);
                mat = mat.inverse;
                UpdateControlledTransformMat(interp, mat);
            }
            else
            {
                Vector3 pos = Vector3.Lerp(al.startPosition, al.endPosition, interp);
                Quaternion quat = getControlledTransform().rotation; // Quaternion.identity;
                if (al.useQuat)
                    quat = Quaternion.Lerp(al.startQuat, al.endQuat, interp);
                if (al.useLookAt)
                {
                    if (eyeTransform != null)
                        quat = Quaternion.LookRotation(Vector3.Lerp(al.startLookAt, al.endLookAt, interp) - eyeTransform.position, Vector3.up);
                    else
                        quat = Quaternion.LookRotation(Vector3.Lerp(al.startLookAt, al.endLookAt, interp) - getControlledTransform().position, Vector3.up);
                }
                //Debug.Log("interp=" + interp + " endPosition: " + al.endPosition + " endQuat: " + al.endQuat + " pos=" + pos + " quat=" + quat) ;

                UpdateControlledTransform(interp, pos, quat);
            }
        }
    }
    void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RotateUser(Vector3.up, - 10.0f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RotateUser(Vector3.up, 10.0f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MoveUser(new Vector3(-.1f, 0.0f, 0.0f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MoveUser(new Vector3(.1f, 0.0f, 0.0f));
        } else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            RotateToSaved();
        }
        if (isAnimating) {
			AnimationLeg al = animationLegs [currentAnimationLegIdx];
			float curTime = Time.time + Time.smoothDeltaTime;
			bool finish = false;
			if (curTime >= al.endTime) {
				currentAnimationLegIdx++;
				finish = (animationLegs == null) || currentAnimationLegIdx >= animationLegs.Length;
				if (!finish)
				{
				    AnimationLeg newal = animationLegs[currentAnimationLegIdx];
                    //Debug.Log("Leg#" + currentAnimationLegIdx + " : useQuat: " + al.useQuat + " : " + newal.useQuat);
				    if (!al.useQuat && newal.useQuat)
				    {
                        if (eyeTransform != null)
                            newal.startQuat = eyeTransform.rotation;
                        else
                            newal.startQuat = getControlledTransform().rotation;
				    }
				    al = newal;
				} else
                {
                    interpFunc(1.0f, al);
                }
				//Debug.Log ("Moving to next leg: currentAnimationLegIdx=" + currentAnimationLegIdx + " finish=" + finish);
			}
			if (finish){
				// set to end animation values
				if (al.isInverse) {
					Quaternion curRot = getControlledTransform().rotation; // Quaternion.identity;
					if (al.useQuat)
						curRot = al.endQuat;
					if (al.useLookAt)
						curRot = Quaternion.LookRotation (al.endLookAt - al.endPosition, Vector3.up);
					Matrix4x4 mat = Matrix4x4.identity;
					mat.SetTRS (al.endPosition, curRot, Vector3.one);
					mat = mat.inverse;
                    UpdateControlledTransformMat(1.0f, mat);
				} else {
                    Vector3 pos = al.endPosition;
                    Quaternion quat = getControlledTransform().rotation;// Quaternion.identity;
					if (al.useQuat) {
                        quat = al.endQuat;
					}
                    UpdateControlledTransform(1.0f, pos, quat);
				}
				isAnimating = false;
				finishAnimating ();
			} else {
                // TODO: NEED TO INTERPOLATE QUATERNION, NOT POSITION AND LOOKAT POINT SEPARATELY
                float interp = (curTime - al.startTime) / al.duration;
                interpFunc(interp, al);
			}
		}
	}
}
