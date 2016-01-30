using UnityEngine;
using System.Collections;
using System;

public class FloorSwitch : ToggleSwitch {

	void OnTriggerEnter(Collider other) {
		if (OnTriggerPress != null){
			OnTriggerPress(this);
		}
    }

	void OnTriggerExit(Collider other) {
		if (OnTriggerRelease != null){
			OnTriggerRelease(this);
		}
    }
}
