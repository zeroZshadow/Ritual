using UnityEngine;
using System.Collections;

public class InputTest : MonoBehaviour {

	ConfigurableInput InputHandler;
	
	// Use this for initialization
	void Start () {
		InputHandler = ConfigurableInput.Instance;
		InputHandler.SetKey("Up", KeyCode.W, KeyCode.UpArrow, KeyCode.None, KeyCode.None);
		InputHandler.SetKey("Down", KeyCode.S, KeyCode.DownArrow, KeyCode.None, KeyCode.None);
		InputHandler.SetKey("Left", KeyCode.A, KeyCode.LeftArrow, KeyCode.None, KeyCode.None);
		InputHandler.SetKey("Right", KeyCode.D, KeyCode.RightArrow, KeyCode.None, KeyCode.None);
		InputHandler.SetKey("Run Up", KeyCode.W, KeyCode.UpArrow, KeyCode.LeftShift, KeyCode.LeftShift);
		InputHandler.SetKey("Run Down", KeyCode.S, KeyCode.DownArrow, KeyCode.LeftShift, KeyCode.LeftShift);
		InputHandler.SetKey("Run Down2", KeyCode.S, KeyCode.DownArrow, KeyCode.LeftControl, KeyCode.LeftControl);
		InputHandler.SetKey("Run Left", KeyCode.A, KeyCode.LeftArrow, KeyCode.LeftShift, KeyCode.LeftShift);
		InputHandler.SetKey("Shift", KeyCode.LeftShift, KeyCode.None, KeyCode.None, KeyCode.None);
		InputHandler.SetKey("Run Right", KeyCode.D, KeyCode.RightArrow, KeyCode.LeftShift, KeyCode.LeftShift);
		//InputHandler.SetPrimaryKeyAsAxis("Up", "Joy1 Axis 2-", KeyCode.LeftShift, 0.2f);
		//InputHandler.SetPrimaryKeyAsAxis("Down", "Joy1 Axis 2+", KeyCode.LeftShift, 0.2f);
		InputHandler.SetAxis("Vertical", "Up", "Down");
		InputHandler.OnAxisScanned += AxisScanned;
		InputHandler.OnKeyScanned += KeyScanned;
	}
	
	// Update is called once per frame
	void Update () {
		
		//Debug.Log(InputHandler.GetAxis("Vertical"));
		
		if (InputHandler.GetKeyUp("Up")){
			InputHandler.StartScanning();
		}
		if (InputHandler.GetKeyDown("Down")){
			Debug.Log("Down");
		}
		if (InputHandler.GetKeyUp("Run Down")){
			Debug.Log("Run Down");
		}
		if (InputHandler.GetKeyUp("Run Down2")){
			Debug.Log("Run Down2");
		}
		if (InputHandler.GetKeyUp("Shift")){
			Debug.Log("Shift");
		}
	}
	
	private void KeyScanned(KeyCode key, KeyCode Modifier){
		Debug.Log(key.ToString() + Modifier.ToString());
	}
	
	private void AxisScanned(string axis, KeyCode Modifier){
		Debug.Log(axis + Modifier.ToString());
	}
}
