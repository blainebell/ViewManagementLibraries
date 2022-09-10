using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShoppingProduct : MonoBehaviour {

	public GameObject prefabForClick;
	public string productName;
	public string description;
	public float price;
	public string priceSuffix;
	public Texture productImage;
	public bool rotatePrefabToLocalCoordinateSystem = false;  // when clicked on, object gets rotated towards the camera or stays in world rotation coordinates 
	public int m_rotationAfterSelectedBehavior = 0;  // 0 - from start (default), 1 - no rotation, 2 - after endpoint 
	// : for rotation after selected (e.g., product screen display)

	public void rotationAfterSelectedBehaviorChanged(int value){
		m_rotationAfterSelectedBehavior = value;
		changed ();
	}
	public int rotationAfterSelectedBehavior {
		set {
			if (value != m_rotationAfterSelectedBehavior) {
				rotationAfterSelectedBehaviorChanged(value);
			}
		}
		get { return m_rotationAfterSelectedBehavior; }
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
