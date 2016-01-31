using UnityEngine;
using System.Collections;

public class SoundPlayer : MonoBehaviour {

	public static SoundPlayer Instance;
	AudioSource source;

	void Awake(){
		source = GetComponent<AudioSource>();

		if (Instance == null) {
			Instance = this;
		}else{
			throw new System.InvalidOperationException("Can not have multiple instances of ConfigurableInput");
		}
	}

	public void Play(AudioClip clip){
		source.PlayOneShot(clip);
	}
}
