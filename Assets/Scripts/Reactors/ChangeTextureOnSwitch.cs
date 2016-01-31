using UnityEngine;
using System.Collections;

public class ChangeTextureOnSwitch : MonoBehaviour {

	public Material OnTexture;
	public Material OffTexture;

	public ToggleSwitch toggleSwitch;
	private Renderer spriteRenderer;

	// Use this for initialization
	void Start () {
		if (toggleSwitch == null){
			toggleSwitch = GetComponent<ToggleSwitch>();
		}
		spriteRenderer = GetComponent<Renderer>();
		toggleSwitch.OnSwitchOn += HandleSwitchOn;
		toggleSwitch.OnSwitchOff += HandleSwitchOff;
	}
	
	void HandleSwitchOn(ToggleSwitch toggleSwitch){
		spriteRenderer.material = OnTexture;
	}

	void HandleSwitchOff(ToggleSwitch toggleSwitch){
		spriteRenderer.material = OffTexture;
	}
}
