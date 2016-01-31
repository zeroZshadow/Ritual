using UnityEngine;
using System.Collections;

public class StressTrigger : MonoBehaviour {

	public Player.StressReason reason = Player.StressReason.Red;

	void OnTriggerStay(Collider other) {
		if (Player.reason == Player.StressReason.None && other.tag == "Player") {
			Player.reason = reason;
			Player.stressed = true;
		}
	}

	void OnTriggerExit(Collider other) {
		if (Player.reason == reason && other.tag == "Player") {
			Player.reason = Player.StressReason.None;
			Player.stressed = false;
		}
	}
}
