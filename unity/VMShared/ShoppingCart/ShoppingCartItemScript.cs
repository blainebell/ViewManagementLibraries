using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShoppingCartItemScript : MonoBehaviour {
	ShoppingCartScript shoppingCart;
	ShoppingProduct sp;
	public bool hasShoppingProduct(){
		return sp != null;
	}
	public int getMouseOverScriptID(){
		if (sp!=null)
			return sp.GetInstanceID ();
		return -1;
	}
	public string productName;
	public int quantity;
	public float price;
	public float total;

	public Texture productTexture;

	public Image productImage;
	public Text productText;
	public Text totalField;
	public Text priceField;
	public Text quantityField;

	public Image[] addingHighlightObjects;

	bool shouldUpdate = false;
	int shouldUpdateID = 0;
	public void set(ShoppingCartItemScript scs){
		productName = scs.productName;
		quantity = scs.quantity;
		price = scs.price;
		total = scs.total;

		productText.text = scs.productName;
		productTexture = scs.productTexture;
		Material mat = new Material(Shader.Find("UI/Default"));
		mat.SetTexture ("_MainTex", productTexture);
		productImage.material = mat;
		priceField.text = totalField.text = total.ToString ("C");
		quantityField.text = quantity.ToString ();
		if (scs.sp != null) {
			shouldUpdateID = scs.getMouseOverScriptID ();
			shouldUpdate = true;
		}
	}

	public void setShoppingCart(ShoppingCartScript scs, ShoppingProduct sparg){
		shoppingCart = scs;
		if (sparg!=null)
			setShoppingProduct (sparg);
	}
	public void setShoppingProduct(ShoppingProduct sparg){
		sp = sparg;
		productText.text = productName = sp.productName;
		productTexture = sp.productImage;
		quantity = 1;
		price = total = sp.price;

		Material mat = new Material(Shader.Find("UI/Default"));
		mat.SetTexture ("_MainTex", productTexture);
		productImage.material = mat;
		priceField.text = totalField.text = total.ToString ("C");
		quantityField.text = quantity.ToString ();
	}
	public void increment(){
		quantity++;
		total += price;
		quantityField.text = quantity.ToString ();
		totalField.text = total.ToString ("C");
		startTime = Time.time + Time.smoothDeltaTime;
		isAnimatingHighlightColor = true;
		if (shouldUpdate) {
			shoppingCart.getShoppingCartItemScript (shouldUpdateID).increment ();
		}
		shoppingCart.computeTotals ();
	}
	public void decrement(){
		quantity--;
		if (quantity == 0) {
			removeCartItem ();
		} else {
			total -= price;
			quantityField.text = quantity.ToString ();
			totalField.text = total.ToString ("C");
			startTime = Time.time + Time.smoothDeltaTime;
			isAnimatingHighlightColor = true;
			if (shouldUpdate) {
				shoppingCart.getShoppingCartItemScript (shouldUpdateID).decrement ();
			}
		}
		shoppingCart.computeTotals ();
	}

	public void removeCartItem(){
		if (shouldUpdate) {
			Destroy (shoppingCart.getShoppingCartItemScript (shouldUpdateID).gameObject);
		}
		Destroy (gameObject);
	}
	void OnDestroy(){
		if (shoppingCart!=null)
			shoppingCart.removeItem (this);
	}
	// Use this for initialization
	void Start () {
	
	}
	bool isAnimatingHighlightColor = false;
	float startTime, duration = 1.0f;
	float startValue = 100.0f / 255.0f;
	// Update is called once per frame

	public bool shouldUnsetRotationAndScale = false;
	void Update () {
		if (isAnimatingHighlightColor) {
			float curTime = Time.time + Time.smoothDeltaTime;
			float alpha = 0.0f;
			if (curTime >= startTime + duration) {
				isAnimatingHighlightColor = false;
				alpha = 0.0f;
			} else {
				float interp = (curTime - startTime) / duration;
				alpha = Mathf.Lerp (startValue, 0.0f, interp);
			}
			foreach (Image hi in addingHighlightObjects) {
				hi.color = new Color (hi.color.r, hi.color.g, hi.color.b, alpha);
			}
		}
		Vector3 pos = transform.localPosition;
		transform.localPosition = new Vector3 (pos.x, pos.y, 0.0f);
		if (shouldUnsetRotationAndScale) {
			transform.localScale = new Vector3 (1.0f,1.0f,1.0f);
			transform.localRotation = Quaternion.identity;
		}
	}
}
