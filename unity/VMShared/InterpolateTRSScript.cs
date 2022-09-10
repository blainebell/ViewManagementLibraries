using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolateTRSScript : MonoBehaviour {
    bool isAnimatingT = false, isAnimatingR = false, isAnimatingS = false;
    Vector3 startT, endT;
    Quaternion startR, endR;
    Vector3 startS, endS;
    float startTimeT, startTimeR, startTimeS;

    public float durationT = 0.4f;
    public float durationR = 0.4f;
    public float durationS = 0.4f;

    public void AnimateScaleTo(Vector3 targetScale)
    {
        startS = gameObject.transform.localScale;
        endS = targetScale;
        startTimeS = Time.time;
        isAnimatingS = true;
    }
    public void AnimateTranslationTo(Vector3 targetTranslation)
    {
        startT = gameObject.transform.localPosition;
        endT = targetTranslation;
        startTimeT = Time.time;
        isAnimatingT = true;
    }
    public void AnimateRotationTo(Quaternion targetQuat)
    {
        startR = gameObject.transform.localRotation;
        endR = targetQuat;
        startTimeR = Time.time;
        isAnimatingR = true;
    }
    // Use this for initialization
    void Start () {
	}
	// Update is called once per frame
	void Update () {
        if (isAnimatingT)
        {
            float curTime = Time.time + Time.smoothDeltaTime;
            float interp = (curTime - startTimeT) / durationT;
            if (interp >= 1.0f)
            {
                gameObject.transform.localPosition = endT;
                isAnimatingT = false;
            }
            else
            {
                gameObject.transform.localPosition = Vector3.Lerp(startT, endT, interp);
            }
        }
        if (isAnimatingS)
        {
            float curTime = Time.time + Time.smoothDeltaTime;
            float interp = (curTime - startTimeS) / durationS;
            if (interp >= 1.0f)
            {
                gameObject.transform.localScale = endS;
                isAnimatingS = false;
            }
            else
            {
                gameObject.transform.localScale = Vector3.Lerp(startS, endS, interp);
            }
        }
        if (isAnimatingR)
        {
            float curTime = Time.time + Time.smoothDeltaTime;
            float interp = (curTime - startTimeR) / durationR;
            if (interp >= 1.0f)
            {
                gameObject.transform.localRotation = endR;
                isAnimatingR = false;
            }
            else
            {
                gameObject.transform.localRotation = Quaternion.Lerp(startR, endR, interp);
            }
        }
    }
}
