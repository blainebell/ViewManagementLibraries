using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DPActionFunc : DPAction {
	public Action actionPerformedFunc;		
	override public void actionPerformed () {
		actionPerformedFunc.Invoke ();
	}
}
