using UnityEngine;
using System.Collections;

public class FlipSwitch : ToggleSwitch {

	public bool on = false;

	public override void Interact(){
		if (on){
			on = false;
			if (OnTriggerRelease != null){
				OnTriggerRelease(this);
			}
		}else{
			on=true;
			if (OnTriggerPress != null){
				OnTriggerPress(this);
			}
		}
	}
}
