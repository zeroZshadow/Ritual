using UnityEngine;
using System.Collections;

public class PixelCamera : MonoBehaviour {
	public uint PPUScale = 1;
	public uint PPU = 50;
	
	// Update is called once per frame
	void Update () {
		Camera.main.orthographicSize = ((Screen.height) / (float)(PPUScale * PPU)) * 0.5f;
	}
}
