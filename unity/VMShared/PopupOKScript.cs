using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PopupOKScript : MonoBehaviour {
	public Text titleText, messageText;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public SortedDictionary<int, DPAction> allActions = new SortedDictionary<int, DPAction> ();

	public void action(){
		MouseScript ms = FindObjectOfType<MouseScript> ();
		ms.SetNoInteraction (false);

		foreach (DPAction a in allActions.Values) {
			a.actionPerformed ();
		}
		Destroy (gameObject);
	}
}
