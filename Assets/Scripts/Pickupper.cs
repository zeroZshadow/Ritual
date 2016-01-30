using UnityEngine;
using System.Collections;

public class Pickupper : MonoBehaviour {

    Pickupable pickedUpObject;
	ConfigurableInput InputHandler;

	public KeyCode ActionKey;

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
				if (GetPickupableInFront(out pickupable)){
					Pickup(pickupable);
				}
			}else{
				TryPlace();
			}
		}
	}

    void OnControllerColliderHit(ControllerColliderHit hit) {
        /*if (pickedUpObject == null){
            if (hit.gameObject.GetComponent<Pickupable>() != null){
				Pickup(hit.gameObject.GetComponent<Pickupable>());
            }
        }*/
    }

	private void TryPlace(){
		if (!Physics.CheckBox(this.transform.position + (this.transform.forward * 1.5f), new Vector3(0.5f, 0.5f, 0.5f))){
			pickedUpObject.transform.parent = null;
			pickedUpObject.transform.position = this.transform.position + (this.transform.forward * 1.5f);
			pickedUpObject = null;
		}
	}

	private bool GetPickupableInFront(out Pickupable pickupable){
		RaycastHit hit;
		if (Physics.Raycast(this.transform.position, transform.forward, out hit, 1)){
			if (hit.collider.GetComponent<Pickupable>()){
				pickupable = hit.collider.GetComponent<Pickupable>();
				return true;
			}
		}
		pickupable = null;
		return false;
	}

	private void Pickup(Pickupable pickupable){
		pickedUpObject = pickupable;
		pickedUpObject.transform.parent = this.transform;
		pickedUpObject.transform.localPosition = new Vector3(0,1,0);
    }
    
}
