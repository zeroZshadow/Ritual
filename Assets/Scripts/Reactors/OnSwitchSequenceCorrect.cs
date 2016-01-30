using UnityEngine;
using System.Collections;
using System;

public class OnSwitchSequenceCorrect : OnSequence {

	public ToggleSwitch[] switches;
	public bool[] requiredStates;

	// Use this for initialization
	void Start () {
		foreach(ToggleSwitch toggleSwitch in switches){
			toggleSwitch.OnSwitchOn += HandleSwitchOn;
			toggleSwitch.OnSwitchOff += HandleSwitchOff;
		}
	}

	private bool SwitchesInRequiredState(){
		bool SwitchesCorrect = true;
		for (int i = 0; i < switches.Length; i++) {
			if (switches[i].IsOn != requiredStates[i]){
				SwitchesCorrect = false;
				break;
			}
		}
		return SwitchesCorrect;
	}

	void HandleSwitchOn (ToggleSwitch toggleSwitch){
		if (SwitchesInRequiredState()){
			if (OnSequenceComplete != null) OnSequenceComplete();
		}
	}

	void HandleSwitchOff (ToggleSwitch toggleSwitch){
		if (SwitchesInRequiredState()){
			if (OnSequenceComplete != null) OnSequenceComplete();
		}
	}
}
