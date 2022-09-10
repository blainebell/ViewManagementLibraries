using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemCanvasScript : MonoBehaviour {
//	bool initialized = false;
	ShoppingProduct currentMouseOver = null;
	// Use this for initialization
	void Start () {
		turnCanvas (false);
	}

	public GameObject panel;
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
		VMUIObject vmui = panel.GetComponent<VMUIObject> ();
		vmui.check (on);
	}
	public void clearText(){
		Text t = GameObject.Find ("ProductNameText").GetComponent<Text> ();
		t.text = "";
		t = GameObject.Find ("DescriptionText").GetComponent<Text> ();
		t.text = "";
		t = GameObject.Find ("PriceText").GetComponent<Text> ();
		t.text = "";
	}
	public void SetShoppingProduct(ShoppingProduct sp){
		if (sp != null) {
			Text productText = GameObject.Find ("ProductNameText").GetComponent<Text> ();
			productText.text = sp.productName;
			Text descText = GameObject.Find ("DescriptionText").GetComponent<Text> ();
			descText.text = sp.description;
			Text priceText = GameObject.Find ("PriceText").GetComponent<Text> ();
			priceText.text = sp.price.ToString ("C") + " " + sp.priceSuffix;
		} else {
			clearText ();
		}
		currentMouseOver = sp;
	}
	// Update is called once per frame
	void Update () {
/*		if (!initialized) {
			turnCanvas (false);
			initialized = true;
		}*/
	}
	public void addToCart(){
		if (currentMouseOver != null) {
			ScreenButtonsScript sbs = FindObjectOfType<ScreenButtonsScript> ();
			//GameObject shoppingCart = GameObject.FindGameObjectWithTag ("shoppingcartcanvas");
			//ShoppingCartScript scs = shoppingCart.GetComponent<ShoppingCartScript> ();
			ShoppingCartScript scs = sbs.shoppingCart;
			scs.addItemToCart (currentMouseOver);
		} else {
			Debug.Log ("addToCart called: currentMouseOver=null");
		}
	}
}
