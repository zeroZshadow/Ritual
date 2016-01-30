using UnityEngine;
using System.Collections;
using System;

public class ButtonSequence : OnSequence {

	public Button[] buttons;
	public bool ResetsOnFail = true;

	private int nextInLine = 0;
	private bool SequenceDone = false;

	// Use this for initialization
	void Start () {
		foreach(Button button in buttons){
			button.OnButtonPressed += HandleButtonPress;
		}
	}

	void HandleButtonPress (Button button){
		if (SequenceDone) return;
		if (button == buttons[nextInLine]){
			nextInLine++;
			if (nextInLine == buttons.Length){
				SequenceDone = true;
				if (OnSequenceComplete != null)	OnSequenceComplete();
			}
		}else{
			if (ResetsOnFail) nextInLine = 0;
		}
	}
}
