using UnityEngine;
using System.Collections;
using System;

public class Button : Interactable {

	public delegate void ButtonPressAction(Button sender);
	public ButtonPressAction OnButtonPressed;

	public override void Interact(){
		if (OnButtonPressed != null) OnButtonPressed(this);
	}
}
