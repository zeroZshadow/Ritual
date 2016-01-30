using UnityEngine;
using System.Collections;

public class FlipSwitch : ToggleSwitch {

	public override void Interact(){
		if (IsOn){
			IsOn = false;
			if (OnSwitchOff != null){
				OnSwitchOff(this);
			}
		}else{
			IsOn=true;
			if (OnSwitchOn != null){
				OnSwitchOn(this);
			}
		}
	}
}
