using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShoppingCartScript : MonoBehaviour {
	public GameObject cartItemPrefab;
	public GameObject cartContent;
	public Text numItemsText, amountText;
	public CheckoutScript checkoutCanvas;
	public Image [] addingHighlightObjects;
	SortedDictionary<int, ShoppingCartItemScript> shoppingCartItemsInOrder = new SortedDictionary<int, ShoppingCartItemScript> ();
	SortedDictionary<int, int> shoppingCartItemsIDtoOrder = new SortedDictionary<int, int> ();
	public void clear(){
		ShoppingCartItemScript[] scis = getShoppingCartItems ();
		foreach (ShoppingCartItemScript sci in scis) {
			Destroy (sci.gameObject);
		}
		//Debug.Log ("ShoppingCartScript.clear: # items=" + shoppingCartItemsInOrder.Count);
	}
	public ShoppingCartItemScript getShoppingCartItemScript(int scid){
		int order;
		if (shoppingCartItemsIDtoOrder.TryGetValue (scid, out order)) {
			ShoppingCartItemScript sci;
			if (shoppingCartItemsInOrder.TryGetValue (order, out sci)) {
				return sci;
			}
		}
		return null;
	}
	public ShoppingCartItemScript [] getShoppingCartItems(){
		ShoppingCartItemScript[] ret = new ShoppingCartItemScript[shoppingCartItemsInOrder.Count];
		int pl = 0;
		foreach (ShoppingCartItemScript sci in shoppingCartItemsInOrder.Values) {
			ret [pl] = sci;
			pl++;
		}
		return ret;
	}
	public List<DPAction> totalListeners = new List<DPAction> ();
	int numTotalItems = 0;
	float totalAmount = 0.0f;
	public float getTotalAmount(){
		return totalAmount;
	}
	public void computeTotals(){
		int totalItems = 0;
		float total = 0.0f;
		foreach (ShoppingCartItemScript sci in shoppingCartItemsInOrder.Values) {
			totalItems += sci.quantity;
			total += sci.total;
		}
		if (numTotalItems != totalItems || totalAmount!=total) {
			numTotalItems = totalItems;
			totalAmount = total;
			numItemsText.text = numTotalItems.ToString ();
			amountText.text = totalAmount.ToString ("C");

			foreach (DPAction dpa in totalListeners) {
				dpa.actionPerformed ();
			}
		}
	}


	public void addItem(ShoppingCartItemScript sci){
		int order = shoppingCartItemsInOrder.Count;
		if (sci.hasShoppingProduct ()) {
			int mosID = sci.getMouseOverScriptID ();
			shoppingCartItemsInOrder.Add (order, sci);
			shoppingCartItemsIDtoOrder.Add (mosID, order);
		}
	}
	public void removeItem(ShoppingCartItemScript sci){
		int order;
		if (sci.hasShoppingProduct ()) {
			int mosID = sci.getMouseOverScriptID ();
			if (shoppingCartItemsIDtoOrder.TryGetValue (mosID, out order)) {
				shoppingCartItemsInOrder.Remove (order);
				shoppingCartItemsIDtoOrder.Remove (mosID);
				if (shoppingCartItemsInOrder.Count == 0) {
					gameObject.SetActive (false);
				}
			}
			computeTotals ();
		}
	}
	public void showIfCartIsNotEmpty(){
		if (shoppingCartItemsIDtoOrder.Count > 0)
			gameObject.SetActive (true);
	}
	public void addItemToCart (ShoppingProduct sp){
		gameObject.SetActive (true);
		int spID = sp.GetInstanceID ();
		if (shoppingCartItemsIDtoOrder.ContainsKey (spID)) {
			// already exists in cart, just increment and update total
			int order = -1;
			shoppingCartItemsIDtoOrder.TryGetValue (spID, out order);
			ShoppingCartItemScript sci;
			if (shoppingCartItemsInOrder.TryGetValue (order, out sci)) {
				sci.increment ();
			} else {
				Debug.LogError ("WARNING: addItemToCart CartItem doesn't have mosID=" + spID);
			}
		} else {
			// add new 
			GameObject newGO = Instantiate(cartItemPrefab);
			ShoppingCartItemScript sci = newGO.GetComponent<ShoppingCartItemScript> ();
			sci.setShoppingCart(this, sp);
			newGO.transform.SetParent(cartContent.transform, true);
			addItem (sci);
			newGO.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f); // NOT SURE WHY I NEED TO DO THIS, SOMETIMES THE SCALE/ROTATION GETS MESSED UP?
			newGO.transform.localRotation = Quaternion.identity;
			Vector3 lpos = newGO.transform.position;
			newGO.transform.position = new Vector3 (lpos.x, lpos.y, 0.0f);
		}
		computeTotals ();
	}
	// Use this for initialization
	void Start () {
		ScreenButtonsScript sbs = FindObjectOfType<ScreenButtonsScript> ();
		sbs.shoppingCart = this;
		gameObject.SetActive (false);
		checkoutCanvas.turnCanvas (false);
		checkoutCanvas.gameObject.SetActive (false);
	}

	public void checkout(){
		WorldToScreenAnimationScript [] w2sas = FindObjectsOfType<WorldToScreenAnimationScript> ();
		foreach (WorldToScreenAnimationScript w2sa in w2sas) {
			Destroy (w2sa.gameObject);
		}
		DPUtils.TurnAllToStillLayoutAndOff(false);
		gameObject.SetActive (false);

		checkoutCanvas.gameObject.SetActive (true);
		checkoutCanvas.populate ();
		checkoutCanvas.turnCanvas (true);
	}
}
