using UnityEngine;
using System.Collections.Generic;
using System;

public class SelectiveFloorSwitch : ToggleSwitch {

	public List<GameObject> PossibleObjects;

	void OnTriggerEnter(Collider other) {
		if (PossibleObjects.Contains(other.gameObject)){
			IsOn = true;
			if (OnSwitchOn != null){
				OnSwitchOn(this);
			}
		}
    }

	void OnTriggerExit(Collider other) {
		if (PossibleObjects.Contains(other.gameObject)){
			IsOn = false;
			if (OnSwitchOff != null){
				OnSwitchOff(this);
			}
		}
    }
}
