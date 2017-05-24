using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForInstantiation : CustomYieldInstruction {
	GameObject go;

	public override bool keepWaiting {
		get {
			return true;
		}
	}

	public WaitForInstantiation(GameObject go) {

	}

}
