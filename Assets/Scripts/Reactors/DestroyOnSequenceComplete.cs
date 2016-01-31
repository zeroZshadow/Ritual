using UnityEngine;
using System.Collections;

public class DestroyOnSequenceComplete : MonoBehaviour {

	public OnSequence sequence;
	public AudioClip soundOnOpen;

	// Use this for initialization
	void Start () {
		sequence.OnSequenceComplete += HandleSequenceComplete;
	}

	void HandleSequenceComplete (){
		sequence.OnSequenceComplete -= HandleSequenceComplete;
		SoundPlayer.Instance.Play(soundOnOpen);
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
