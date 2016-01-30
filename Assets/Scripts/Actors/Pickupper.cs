using UnityEngine;
using System.Collections;

public class Pickupper : MonoBehaviour {

    Pickupable pickedUpObject;
	ConfigurableInput InputHandler;

	public KeyCode ActionKey;
	public LayerMask mask;

	// Use this for initialization
	void Start () {
		InputHandler = ConfigurableInput.Instance;
		InputHandler.SetKey("Action", ActionKey, KeyCode.None, KeyCode.None, KeyCode.None);
	}
	
	// Update is called once per frame
	void Update () {
		if (InputHandler.GetKeyDown("Action")){
			if (pickedUpObject == null){
				Pickupable pickupable;
				if (GetPickupable(out pickupable)){
					Pickup(pickupable);
				}
			}else{
				TryPlace();
			}
		}
	}

	private void TryPlace(){
		if (!Physics.CheckBox(this.transform.position + (this.transform.forward * 1.5f), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, mask)){
			pickedUpObject.transform.parent = null;
			pickedUpObject.transform.position = this.transform.position + (this.transform.forward * 1.5f);
			pickedUpObject = null;
		}
	}

	private bool GetPickupable(out Pickupable pickupable){


		Collider[] colliders = Physics.OverlapSphere(this.transform.position, 1.5f);
		float closestDistance = float.MaxValue;
		Pickupable closestPickupable = null;

		foreach (Collider collider in colliders){
		    float distance = Vector3.Distance(transform.position, collider.transform.position);
			if (distance < closestDistance && collider.GetComponent<Pickupable>() != null) {
				closestDistance=distance;
				closestPickupable = collider.GetComponent<Pickupable>();
			}
		}

		if (closestPickupable == null){
			pickupable = null;
			return false;
		}
		pickupable = closestPickupable;
		return true;
	}

	private void Pickup(Pickupable pickupable){
		pickedUpObject = pickupable;
		pickedUpObject.transform.parent = this.transform;
		pickedUpObject.transform.localPosition = new Vector3(0,1,0);
    }

	public Pickupable GetCarriedObject(){
		return pickedUpObject;
    }
    
}
