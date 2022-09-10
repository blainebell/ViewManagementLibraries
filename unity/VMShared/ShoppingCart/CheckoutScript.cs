using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CheckoutScript : MonoBehaviour {
	public GameObject checkoutItemPrefab;
	public GameObject checkoutContent;
	public ShoppingCartScript shoppingCart;
	public ScrollRect scrollRect;
	public Text totalField;
	public GameObject zoomAfterPurchase;
	bool isTurnedOn = false;
	public bool getIsTurnedOn(){
		return isTurnedOn; 
	}
	class CheckoutTotalListener : DPAction {
		CheckoutScript checkoutScript ;
		public CheckoutTotalListener(CheckoutScript cs){
			checkoutScript = cs;
		}
		override public void actionPerformed (){
			checkoutScript.totalField.text = checkoutScript.shoppingCart.getTotalAmount().ToString ("C");
		}
	}
	CheckoutTotalListener ctl;
	public void turnCanvas(bool on){
		Image [] images = GetComponentsInChildren<Image> ();
		//		Debug.Log ("# Images=" + images.Length);
		float alpha = on ? (100.0f / 255.0f) : 0.0f;
		foreach (Image img in images) {
			img.color = new Color (img.color.r, img.color.g, img.color.b, alpha);
		}
		if (on)
			alpha = 1.0f;
		Text [] texts = GetComponentsInChildren<Text> ();
		foreach (Text text in texts) {
			text.color = new Color (text.color.r, text.color.g, text.color.b, alpha);
		}
		if (on) {
			foreach (Image img in images) {
				Button but = img.gameObject.GetComponent<Button> ();
				if (but != null) {
					img.color = new Color (img.color.r, img.color.g, img.color.b, 1.0f);
				}
			}
		}
		VMUIObject vmui = GetComponent<VMUIObject> ();
		vmui.check (on);
		isTurnedOn = on;
		if (isTurnedOn) {
			if (ctl==null)
				ctl = new CheckoutTotalListener (this);
			shoppingCart.totalListeners.Add (ctl);
			ctl.actionPerformed ();
		} else {
			shoppingCart.totalListeners.Remove (ctl);
		}
	}
	public void hide(){
		if (isTurnedOn) {
			var children = new List<GameObject> ();
			foreach (Transform child in checkoutContent.transform)
				children.Add (child.gameObject);
			children.ForEach (child => Destroy (child));
			turnCanvas (false);
			gameObject.SetActive (false);
		}
	}
	// Use this for initialization
	void Start () {
	}
	public void populate(){
		ShoppingCartItemScript [] scss =  shoppingCart.getShoppingCartItems();
		for (int i = 0; i < scss.Length; i++) {
			GameObject newGO = Instantiate (checkoutItemPrefab);
			newGO.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			newGO.transform.localRotation = Quaternion.identity;
			newGO.transform.SetParent (checkoutContent.transform);
			ShoppingCartItemScript newCS = newGO.GetComponent<ShoppingCartItemScript> ();
			newCS.setShoppingCart (shoppingCart, null);
			newCS.set (scss [i]);
		}
		scrollRect.verticalNormalizedPosition = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	class PurchasedListener : DPAction {
		public PurchasedListener(){
		}
		override public void actionPerformed (){
			ScreenButtonsScript sbs = FindObjectOfType<ScreenButtonsScript> ();
			sbs.GotoTopLevel (0);
		}
	}

	public GameObject afterPurchasePrefab;
	public void purchaseCart(){
		if (!isTurnedOn)
			return;
		shoppingCart.clear ();
		hide ();
		GameObject go = Instantiate (afterPurchasePrefab);
		// after purchase, then goto top
		PurchasedListener pl = new PurchasedListener();
		PopupOKScript pok = go.GetComponent<PopupOKScript>();
		pok.allActions.Add (GetInstanceID(), pl);
		pok.messageText.text = "You have purchased " + shoppingCart.numItemsText.text + " items for the amount of " + shoppingCart.amountText.text + ", Thank you!";
		go.SetActive (true);
		MouseScript ms = FindObjectOfType<MouseScript> ();
		ms.SetNoInteraction (true);
	}
}
