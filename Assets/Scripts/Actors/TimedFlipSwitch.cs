using UnityEngine;
using System.Collections;

public class TimedFlipSwitch : ToggleSwitch {

	public float resetTime;
	private float timeTillReset;

	// Update is called once per frame
	void Update () {
		if (IsOn){
			if (timeTillReset < Time.time){
				IsOn = false;
				if (OnSwitchOff != null){
					OnSwitchOff(this);
				}
			}
		}
	}

	public override void Interact(){
		if (IsOn){
			IsOn = false;
			if (OnSwitchOff != null){
				OnSwitchOff(this);
			}
		}else{
			timeTillReset = Time.time + resetTime;
			IsOn=true;
			if (OnSwitchOn != null){
				OnSwitchOn(this);
			}
		}
	}
}
