using UnityEngine;
using System.Collections;

public class LoadLevelOnTrigger : MonoBehaviour {

	public string LevelName;

	void OnTriggerEnter(Collider other) {
		Application.LoadLevel(LevelName);
    }
}
