using UnityEngine;
using System.Collections;

public class DestroyOnSequenceComplete : MonoBehaviour {

	public OnSequence sequence;
	public AudioClip soundOnOpen;

	[Header("Destroy Animation")]
	private Material mat;
	public Texture2D[] MainTex;
	public Texture2D[] AltTex;
	public int framerate = 5;
	private bool _playing = false;
	private float _startTime = 0;

	// Use this for initialization
	void Start () {
		sequence.OnSequenceComplete += HandleSequenceComplete;
		mat = GetComponent<MeshRenderer>().material;
	}

	void HandleSequenceComplete (){
		sequence.OnSequenceComplete -= HandleSequenceComplete;
		SoundPlayer.Instance.Play(soundOnOpen);

		if (mat == null) {
			Destroy(gameObject);
		} else {
			//Animation
			_startTime = Time.time;
			_playing = true;

			Destroy(gameObject, MainTex.Length * (1.0f / framerate));
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (_playing) {
			float t = Time.time - _startTime; //Offset time
			int frame = Mathf.Min(Mathf.FloorToInt(t / (1.0f / framerate)), MainTex.Length-1);
			mat.SetTexture("_MainTex", MainTex[frame]);
			mat.SetTexture("_AltTex", AltTex[frame]);
		}
	}
}
