using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class VMGeometry : MonoBehaviour {
	public bool doNotInclude = false;
	public float xMinInset = 0;
	public float yMinInset = 0;
	public float zMinInset = 0;
	public float xMaxInset = 0;
	public float yMaxInset = 0;
	public float zMaxInset = 0;
	public GameObject splitPlanes;
}
