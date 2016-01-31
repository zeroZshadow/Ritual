using UnityEngine;
using System.Collections;

public class PlayerMover : MonoBehaviour {

	ConfigurableInput InputHandler;

	public KeyCode UpKey;
	public KeyCode DownKey;
	public KeyCode LeftKey;
	public KeyCode RightKey;

	public SpriteRenderer render;
	public Animator anim;

	// GridMove
	public float moveSpeed = 3f;
	public float gridSize = 1f;
	private Vector2 _input;
	private Vector2 _direction = new Vector2(-1, 0);
	private bool _isMoving = false;

	// Stress
	public static float _stress = 0.0f;

	// Use this for initialization
	void Start() {
		InputHandler = ConfigurableInput.Instance;
		InputHandler.SetKey("Up", UpKey, KeyCode.None, KeyCode.None, KeyCode.None);
		InputHandler.SetKey("Down", DownKey, KeyCode.None, KeyCode.None, KeyCode.None);
		InputHandler.SetKey("Left", LeftKey, KeyCode.None, KeyCode.None, KeyCode.None);
		InputHandler.SetKey("Right", RightKey, KeyCode.None, KeyCode.None, KeyCode.None);

		/*
		InputHandler.SetAxis("Vertical", "JoyUp", "JoyDown");
		InputHandler.SetAxis("Horizontal", "JoyLeft", "JoyRight");
		InputHandler.SetPrimaryKeyAsAxis("JoyUp", "Joy1 Axis 2-", KeyCode.None, 0.2f);
		InputHandler.SetPrimaryKeyAsAxis("JoyDown", "Joy1 Axis 2+", KeyCode.None, 0.2f);
		InputHandler.SetPrimaryKeyAsAxis("JoyLeft", "Joy1 Axis 1-", KeyCode.None, 0.2f);
		InputHandler.SetPrimaryKeyAsAxis("JoyRight", "Joy1 Axis 1+", KeyCode.None, 0.2f);
		*/
	}

	// Update is called once per frame
	void Update() {
		if (!_isMoving) {
			if (InputHandler.GetKey("Up")) {
				_input = new Vector2(0, -1);
			} else if (InputHandler.GetKey("Down")) {
				_input = new Vector2(0, 1);
			} else if (InputHandler.GetKey("Left")) {
				_input = new Vector2(1, 0);
			} else if (InputHandler.GetKey("Right")) {
				_input = new Vector2(-1, 0);
			}

			// Make sure we dont go diagonal
			if (Mathf.Abs(_input.x) > Mathf.Abs(_input.y)) {
				_input.y = 0;
			} else {
				_input.x = 0;
			}

			//Move
			if (_input != Vector2.zero) {
				_direction = _input;
				StartCoroutine(move(transform));
			}
		}

		//Debug
		_stress += Time.deltaTime;

		// Animate
		//Get stress state
		string stateSuffix = "";
		if (_stress > 5.0f) {
			stateSuffix = "Glitch";
		}

		if (_direction.y == -1) {
			anim.Play("Back" + stateSuffix);
			render.flipX = false;
		} else if (_direction.y == 1) {
			anim.Play("Front" + stateSuffix);
			render.flipX = true;
		} else if (_direction.x == 1) {
			anim.Play("Back" + stateSuffix);
			render.flipX = true;
		} else if (_direction.x == -1) {
			anim.Play("Front" + stateSuffix);
			render.flipX = false;
		}
	}

	private IEnumerator move(Transform transform) {
		_isMoving = true;
		Vector3 startPosition = transform.position;
		float t = 0;

		// Calculate target position
		Vector3 endPosition = new Vector3(
			startPosition.x + System.Math.Sign(_input.x) * gridSize,
			startPosition.y,
			startPosition.z + System.Math.Sign(_input.y) * gridSize
		);

		Vector3 direction = endPosition - startPosition;
		float length = direction.magnitude;
		direction /= length;

		// Only move if we wont hit anything
		RaycastHit hitInfo;
		if (!Physics.Raycast(startPosition, direction, out hitInfo, length) || hitInfo.collider.isTrigger) {
			while (t < 1f) {
				t += Time.deltaTime * (moveSpeed / gridSize);
				transform.position = Vector3.Lerp(startPosition, endPosition, t);
				yield return null;
			}
		}

		// Reset and return
		_isMoving = false;
		_input = Vector2.zero;
		yield return 0;
	}

	public bool isMoving(){
		return _isMoving;
	}
}
