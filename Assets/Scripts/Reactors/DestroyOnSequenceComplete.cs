using UnityEngine;
using System.Collections;

public class DestroyOnSequenceComplete : MonoBehaviour {

	public OnSequence sequence;
	public AudioClip soundOnOpen;
	private AudioSource source;

	// Use this for initialization
	void Start () {
		source = gameObject.GetComponent<AudioSource>();
		sequence.OnSequenceComplete += HandleSequenceComplete;
	}

	void HandleSequenceComplete (){
		sequence.OnSequenceComplete -= HandleSequenceComplete;
		source.PlayOneShot(soundOnOpen);
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
