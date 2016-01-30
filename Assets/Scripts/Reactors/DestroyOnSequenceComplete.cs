using UnityEngine;
using System.Collections;

public class DestroyOnSequenceComplete : MonoBehaviour {

	public Sequence sequence;

	// Use this for initialization
	void Start () {
		sequence.OnSequenceComplete += HandleSequenceComplete;
	}

	void HandleSequenceComplete (){
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
