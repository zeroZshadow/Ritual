using UnityEngine;
using System.Collections;

public class SwitchToggled : MonoBehaviour {

	public ToggleSwitch toggleSwitch;

	// Use this for initialization
	void Start () {
		toggleSwitch.OnSwitchOn += HandleFloorSwitchPress;
		toggleSwitch.OnSwitchOff += HandleFloorSwitchRelease;
	}

	void HandleFloorSwitchPress (ToggleSwitch floorSwitch){
		gameObject.SetActive(false);
	}

	void HandleFloorSwitchRelease (ToggleSwitch floorSwitch){
		gameObject.SetActive(true);
	}
}
