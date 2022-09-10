using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VMConnector : MonoBehaviour {
	public VMObject firstObject;
	public VMObject secondObject;
	public bool isConnectorToOutside;

	public GameObject extraPointGO;

	bool addedToDP = false;

    void AddToDP()
    {
        if (DPManagerScript.DPisInitialized)
        {
            VMObject vmo = GetComponent<VMObject>();
            if (vmo != null)
            {
                int id1 = firstObject.GetVMInstanceID();
                int id2 = 0;
                if (secondObject != null)
                    id2 = secondObject.GetVMInstanceID();
                Renderer rend = GetComponent<Renderer>();
                Vector3 center = rend.bounds.center;
                float[] centerPt = { center.x, center.y, center.z };
                bool hasExtraPoint = false;
                float[] extraPointCenter = null;
                if (extraPointGO != null)
                {
                    Renderer eprend = extraPointGO.GetComponent<Renderer>();
                    if (eprend != null)
                    {
                        Vector3 cen = eprend.bounds.center;
                        extraPointCenter = new float[] { cen.x, cen.y, cen.z };
                        hasExtraPoint = true;
                    }
                } if (!hasExtraPoint) {
                    extraPointCenter = new float[] { 0.0f, 0.0f, 0.0f };
                }
                DPManagerScript.Call_i_i_i_p3_b_b_p3("UnityConnector-map", GetInstanceID(), id1, id2, centerPt, isConnectorToOutside, hasExtraPoint, extraPointCenter);
                addedToDP = true;
            }
        }
        else
        {
            Debug.Log("VMConnector.AddToDP: View Manager not started");
        }
    }
	void RemoveFromDP(){
		DPManagerScript.Call_i( "Remove-UnityConnector-With-ID-map", GetInstanceID() );
		addedToDP = false;
	}

	void Start () {
		
	}

	void Update () {
		if (enabled != addedToDP) {
			if (enabled)
				AddToDP ();
			else
				RemoveFromDP ();
		}
	}

}
