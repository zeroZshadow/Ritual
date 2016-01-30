using UnityEngine;
using System.Collections;

public class SwitchToggled : MonoBehaviour {

	public ToggleSwitch toggleSwitch;

	// Use this for initialization
	void Start () {
		toggleSwitch.OnTriggerPress += HandleFloorSwitchPress;
		toggleSwitch.OnTriggerRelease += HandleFloorSwitchRelease;
	}

	void HandleFloorSwitchPress (ToggleSwitch floorSwitch){
		gameObject.SetActive(false);
	}

	void HandleFloorSwitchRelease (ToggleSwitch floorSwitch){
		gameObject.SetActive(true);
	}
}
