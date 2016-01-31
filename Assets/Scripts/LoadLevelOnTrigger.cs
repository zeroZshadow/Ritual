using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadLevelOnTrigger : MonoBehaviour {

	public string LevelName;

	void OnTriggerEnter(Collider other) {
		SceneManager.LoadScene(LevelName);
	}
}
