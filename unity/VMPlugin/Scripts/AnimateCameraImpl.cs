using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimateCameraImpl : MonoBehaviour {
	abstract public float getDefaultAnimationTime ();
    abstract public Transform getControlledTransform();
    abstract public Transform getEyeTransform();
    abstract public Transform getEyeOrControlledTransform();
    abstract public bool isControllingWorld ();
    abstract public bool animateCameraToPosition (Vector3 pos);
	abstract public bool animateCameraToPositionWithDuration (Vector3 pos, float dur);
	abstract public bool animateCameraTo (Vector3 pos, bool useQuat, Vector3 lookAt, bool inv);
	abstract public bool animateCameraToWithDuration (Vector3 pos, bool useQuat, Vector3 lookAt, float dur, bool inv);
	abstract public void setShouldExecuteCommandAfterAnimation (bool s);
	abstract public void setExecuteCommandAfterAnimation (string s);
	abstract public void setShouldExecuteViewManagementAfterAnimation(bool s);
	abstract public bool animateCameraAcrossPath(Vector3 [] pointsInPath, Vector3 [] allViewVectors, bool [] lookAtNext);
}
