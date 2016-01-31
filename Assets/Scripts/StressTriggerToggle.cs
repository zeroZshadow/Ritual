using UnityEngine;
using System.Collections;

public class StressTriggerToggle : MonoBehaviour {

	public Player.StressReason reason = Player.StressReason.Red;

	void OnTriggerEnter(Collider other) {
		if (Player.reason == Player.StressReason.None && other.tag == "Player") {
			Player.reason = reason;
			Player.stressed = true;
		}
	}
}
