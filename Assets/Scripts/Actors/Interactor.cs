using UnityEngine;
using System.Collections;

public class Interactor : MonoBehaviour {

	ConfigurableInput InputHandler;
	Pickupper pickupper;

	// Use this for initialization
	void Start () {
		InputHandler = ConfigurableInput.Instance;
		pickupper = GetComponent<Pickupper>();
	}

	// Update is called once per frame
	void Update () {
		if (InputHandler.GetKeyDown("Action")){
			Interact();
		}
	}

	private void Interact(){
		if (pickupper != null && pickupper.GetCarriedObject() != null) return;
		Collider[] colliders = Physics.OverlapSphere(this.transform.position, 1.5f);
		float closestDistance = float.MaxValue;
		Interactable closestInteractible = null;

		foreach (Collider collider in colliders){
			Debug.Log(colliders.Length);
		    float distance = Vector3.Distance(transform.position, collider.transform.position);
			if (distance < closestDistance && collider.GetComponent<Interactable>() != null) {
				closestDistance=distance;
				closestInteractible = collider.GetComponent<Interactable>();
			}
		}

		if (closestInteractible != null){
			closestInteractible.Interact();
		}
	}
}
