using UnityEngine;
using System.Collections;

public class PixelCamera : MonoBehaviour {

	//Orthographic size = ((Vert Resolution)/(PPUScale * PPU)) * 0.5

	public uint PPUScale = 1;
	public uint PPU = 50;

	public Transform target;
	
	// Update is called once per frame
	void Update () {
		Camera.main.orthographicSize = ((Screen.height) / (float)(PPUScale * PPU)) * 0.5f;

		//TODO Move with target
	}
}
