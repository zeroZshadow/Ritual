using UnityEngine;
using System.Collections;
using System;

public class FloorSwitch : ToggleSwitch {

	void OnTriggerEnter(Collider other) {
		IsOn = true;
		if (OnSwitchOn != null){
			OnSwitchOn(this);
		}
    }

	void OnTriggerExit(Collider other) {
		IsOn = false;
		if (OnSwitchOff != null){
			OnSwitchOff(this);
		}
    }
}
