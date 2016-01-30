using UnityEngine;
using System.Collections;

public class ToggleSwitch : Interactable {

	public delegate void SwitchAction(ToggleSwitch floorSwitch);
	public bool IsOn = false;

	public SwitchAction OnSwitchOn;
	public SwitchAction OnSwitchOff;
}
