using UnityEngine;
using System.Collections;

public class PlaySoundOnSwitch : MonoBehaviour {

	public ToggleSwitch toggleSwitch;

	// Use this for initialization
	void Start () {
		if (toggleSwitch == null){
			toggleSwitch = GetComponent<ToggleSwitch>();
		}

		toggleSwitch.OnSwitchOn += HandleSwitchOn;
		toggleSwitch.OnSwitchOff += HandleSwitchOff;
	}
	
	void HandleSwitchOn(ToggleSwitch toggleSwitch){

	}

	void HandleSwitchOff(ToggleSwitch toggleSwitch){

	}

}
