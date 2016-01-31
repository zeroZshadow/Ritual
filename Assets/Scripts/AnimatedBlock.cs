using UnityEngine;
using System.Collections;

public class AnimatedBlock : MonoBehaviour {

	public Material mat;

	public Texture2D[] MainTex;
	public Texture2D[] AltTex;
	public int framerate = 5;

	void Awake() {
		if (MainTex.Length != AltTex.Length) {
			Debug.LogError("Programmer error");
		}
	}

	// Update is called once per frame
	void Update () {
		int frame = Mathf.FloorToInt(Time.time / (1.0f/framerate)) % MainTex.Length;
		mat.SetTexture("_MainTex", MainTex[frame]);
		mat.SetTexture("_AltTex", AltTex[frame]);
	}
}
