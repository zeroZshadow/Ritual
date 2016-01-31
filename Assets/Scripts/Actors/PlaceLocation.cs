using UnityEngine;
using System.Collections;

public class PlaceLocation : OnSequence {

	public Pickupable[] Pickups;
	public Material[] Materials;

	private Renderer spriteRenderer;
	private int nextInLine;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<Renderer>();
		nextInLine = 0;
	}

	public bool TryPlace(Pickupable pickupable){
		if (Pickups[nextInLine] == pickupable){
			spriteRenderer.material = Materials[nextInLine];
			nextInLine++;
			if (nextInLine == Pickups.Length){
				if (OnSequenceComplete != null){
					OnSequenceComplete();
				}
			}
			return true;
		}
		return false;
	}
}
