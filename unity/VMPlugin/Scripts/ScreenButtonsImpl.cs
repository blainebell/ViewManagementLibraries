using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScreenButtonsImpl : MonoBehaviour {
    public abstract void addScreenMenu(int nindent, int level, string menuItemStr, string actionCmdStr, string objectName);
    public abstract void addScreenMenuWithUID(string uid, int nindent, int level, string menuItemStr, string actionCmdStr, string objectName);
    public abstract void removeScreenMenuWithUID(string uid);
    public abstract void removeMenuGreaterThanOrEqualTo (int int_arg);
	public abstract void clearOrderToIndentMenuItem();
	public abstract void addPermanentRect (int key, Rect rct);
	public abstract void removePermanentRect (int key);
	public abstract VMObject getClosestVMObject (out Vector3 connectorCentroid, out bool connectorCentroidIsSet);
	public abstract void GotoTopLevelWithIndex(int toVal);
}
