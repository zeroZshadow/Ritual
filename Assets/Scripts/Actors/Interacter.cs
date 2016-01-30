using UnityEngine;
using System.Collections;

public class Interacter : MonoBehaviour {

	ConfigurableInput InputHandler;

	// Use this for initialization
	void Start () {
		InputHandler = ConfigurableInput.Instance;
	}

	// Update is called once per frame
	void Update () {
		if (InputHandler.GetKeyDown("Action")){
			Interact();
		}
	}

	private void Interact(){
		RaycastHit hit;
		Interactable interactable;
		if (Physics.Raycast(this.transform.position, transform.forward, out hit, 1)){
			if (hit.collider.GetComponent<Interactable>()){
				interactable = hit.collider.GetComponent<Interactable>();
				interactable.Interact();
			}
		}
	}
}
