using UnityEngine;
using System.Collections;

public class ToggleSwitch : Interactable {

	public delegate void FloorSwitchAction(ToggleSwitch floorSwitch);

	public FloorSwitchAction OnTriggerPress;
	public FloorSwitchAction OnTriggerRelease;
}
