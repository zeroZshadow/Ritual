using UnityEngine;
using System.Collections;

public class PlayerMover : MonoBehaviour {

	ConfigurableInput InputHandler;

	public KeyCode UpKey;
	public KeyCode DownKey;
	public KeyCode LeftKey;
	public KeyCode RightKey;

	public float playerSpeed = 0.1f;

    private CharacterController characterController;

	// Use this for initialization
	void Start () {
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

        characterController = this.GetComponent<CharacterController>();

		InputHandler.OnAxisScanned += HandleAxisScanned;
	}

	void HandleAxisScanned (string axis, KeyCode modiefier)
	{
		Debug.Log(axis);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 moveVector = Vector3.zero;
    
		if (InputHandler.GetKey("Up")){
			moveVector += new Vector3(1,0,-1);
		}

		if (InputHandler.GetKey("Down")){
			moveVector += new Vector3(-1,0,1);
		}

		if (InputHandler.GetKey("Left")){
			moveVector += new Vector3(1,0,1);
		}

		if (InputHandler.GetKey("Right")){
			moveVector += new Vector3(-1,0,-1);
		}

		moveVector = moveVector.normalized;

		moveVector += new Vector3(InputHandler.GetAxis("Horizontal") + InputHandler.GetAxis("Vertical"), 0, InputHandler.GetAxis("Horizontal") - InputHandler.GetAxis("Vertical"));
		moveVector *= playerSpeed;

        characterController.Move(moveVector);
	}
}
