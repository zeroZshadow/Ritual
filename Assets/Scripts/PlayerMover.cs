﻿using UnityEngine;
using System.Collections;

public class PlayerMover : MonoBehaviour {

	ConfigurableInput InputHandler;

	public KeyCode UpKey;
	public KeyCode DownKey;
	public KeyCode LeftKey;
	public KeyCode RightKey;

	public float playerSpeed = 0.1f;

	public CharacterController characterController;
	public SpriteRenderer render;
	public Animator anim;

	// Use this for initialization
	void Start() {
		InputHandler = ConfigurableInput.Instance;
		InputHandler.SetKey("Up", UpKey, KeyCode.None, KeyCode.None, KeyCode.None);
		InputHandler.SetKey("Down", DownKey, KeyCode.None, KeyCode.None, KeyCode.None);
		InputHandler.SetKey("Left", LeftKey, KeyCode.None, KeyCode.None, KeyCode.None);
		InputHandler.SetKey("Right", RightKey, KeyCode.None, KeyCode.None, KeyCode.None);

		InputHandler.SetAxis("Vertical", "JoyUp", "JoyDown");
		InputHandler.SetAxis("Horizontal", "JoyLeft", "JoyRight");
		InputHandler.SetPrimaryKeyAsAxis("JoyUp", "Joy1 Axis 2-", KeyCode.None, 0.2f);
		InputHandler.SetPrimaryKeyAsAxis("JoyDown", "Joy1 Axis 2+", KeyCode.None, 0.2f);
		InputHandler.SetPrimaryKeyAsAxis("JoyLeft", "Joy1 Axis 1-", KeyCode.None, 0.2f);
		InputHandler.SetPrimaryKeyAsAxis("JoyRight", "Joy1 Axis 1+", KeyCode.None, 0.2f);

		InputHandler.OnAxisScanned += HandleAxisScanned;
	}

	void HandleAxisScanned(string axis, KeyCode modiefier) {
		Debug.Log(axis);
	}

	// Update is called once per frame
	void Update() {
		Vector3 moveVector = Vector3.zero;

		if (InputHandler.GetKey("Up")) {
			moveVector += new Vector3(0, 0, -1);
			anim.Play("Back");
			render.flipX = false;
		} else if (InputHandler.GetKey("Down")) {
			moveVector += new Vector3(0, 0, 1);
			anim.Play("Front");
			render.flipX = true;
		} else if (InputHandler.GetKey("Left")) {
			moveVector += new Vector3(1, 0, 0);
			anim.Play("Back");
			render.flipX = true;
		} else if (InputHandler.GetKey("Right")) {
			moveVector += new Vector3(-1, 0, 0);
			anim.Play("Front");
			render.flipX = false;
		}

		// Possible controller input
		//moveVector += new Vector3(InputHandler.GetAxis("Horizontal") + InputHandler.GetAxis("Vertical"), 0, InputHandler.GetAxis("Horizontal") - InputHandler.GetAxis("Vertical"));

		// Normalize
		moveVector = moveVector.normalized;
		moveVector *= playerSpeed;

		characterController.Move(moveVector);
	}
}
