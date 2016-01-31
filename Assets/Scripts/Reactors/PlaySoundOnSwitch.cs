using UnityEngine;
using System.Collections;

public class PlaySoundOnSwitch : MonoBehaviour {

	public ToggleSwitch toggleSwitch;
	public AudioClip OnClip;
	public AudioClip OffClip;
	// Use this for initialization
	void Start () {
		if (toggleSwitch == null){
			toggleSwitch = GetComponent<ToggleSwitch>();
		}
		toggleSwitch.OnSwitchOn += HandleSwitchOn;
		toggleSwitch.OnSwitchOff += HandleSwitchOff;
	}
	
	void HandleSwitchOn(ToggleSwitch toggleSwitch){
		SoundPlayer.Instance.Play(OnClip);
	}

	void HandleSwitchOff(ToggleSwitch toggleSwitch){
		SoundPlayer.Instance.Play(OffClip);
	}

}
