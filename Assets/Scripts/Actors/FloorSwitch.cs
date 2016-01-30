using UnityEngine;
using System.Collections;
using System;

public class FloorSwitch : ToggleSwitch {

	void OnTriggerEnter(Collider other) {
		if (OnSwitchOn != null){
			OnSwitchOn(this);
		}
    }

	void OnTriggerExit(Collider other) {
		if (OnSwitchOff != null){
			OnSwitchOff(this);
		}
    }
}
